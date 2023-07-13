using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public enum CharacterType
{
    Bp153,
    Plus3000,
    Flip3
}

public class InGameManager : Singleton<InGameManager>
{
    private readonly Vector3 cameraDistance = new Vector3(0, 0, 100);

    public bool isGaming;
    private float timer;
    
    [Header("맵")]
    public Vector2 minPos;
    public Vector2 maxPos;
    
    [SerializeField] private SpriteRenderer carImage;

    [Header("보스")]
    public bool isBossSummon;
    public bool isBossLiving;

    [Header("적")]
    public int killEnemyCount;
    private const float enemyCoolTime = 0.5f;
    private float enemyDuration = 0;
    private float enemyPower = 0;

    public Material flashWhiteMaterial;

    [Header("경험치")]
    private const float expRandomMin = 0.5f;
    private const float expRandomMax = 1.5f;

    private const float textFadeInTime = 0.2f;
    private const float textFadeOutTime = 0.5f;
    private const float textMoveXPos = 0.7f;
    private const float textMoveYPos = 1f;

    [Header("스테이지")]
    public int stage;
    [SerializeField] private SpriteRenderer[] stageBackgrounds;
    
    [Header("캐릭터")] 
    [SerializeField] private Dictionary<CharacterType, Player> players;

    public int Score => Mathf.RoundToInt(killEnemyCount + (stage - 1) * 1000 + (60 - timer));

    protected override void OnCreated()
    {
        stage = 1;
        timer = 60;
        killEnemyCount = 0;

        MapSetting();
        Player.Instance.gameObject.SetActive(false);

        TransitionManager.Instance.TransitionFadeIn(TransitionType.Fade, () => StartCoroutine(CarIntro()));
    }

    private IEnumerator CarIntro()
    {
        Player.Instance.transform.position = Vector3.zero;
        GameManager.Instance.MainCamera.transform.position = Vector3.zero - cameraDistance;
        
        carImage.gameObject.SetActive(true);
        carImage.transform.position = new Vector3(16, 0, 0);
        carImage.transform.DOMoveX(0, 1f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(2);

        Player.Instance.gameObject.SetActive(true);

        isGaming = true;
        switch (stage)
        {
            case 1:
                SoundManager.Instance.PlaySound("bgm", SoundType.BGM);
                break;
            case 2:
                SoundManager.Instance.PlaySound("bgm2", SoundType.BGM);
                break;
        }

        yield return new WaitForSeconds(1);

        carImage.transform.DOMoveX(-48, 6f).SetEase(Ease.InBack).OnComplete(() => { carImage.gameObject.SetActive(false); });
    }

    private IEnumerator CarOutro()
    {
        isGaming = false;
        Vector3 playerPos = Player.Instance.transform.position;

        carImage.gameObject.SetActive(true);
        carImage.transform.position = new Vector3(playerPos.x + 16, playerPos.y, playerPos.y);
        carImage.transform.DOMoveX(playerPos.x, 2f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(3);

        Player.Instance.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        carImage.transform.DOMoveX(playerPos.x - 24, 3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            carImage.gameObject.SetActive(false); 
            UIManager.Instance.MoveBackgroundSetting();
        });
    }

    private void TimerUpdate()
    {
        if (isBossLiving) return;

        timer -= Time.deltaTime;
        UIManager.Instance.UpdateTimer(timer);
        if (timer <= 0)
        {
            if (stage == 1 || isBossSummon)
            {
                isBossLiving = true;
                StartCoroutine(CarOutro());
            }
            else
            {
                BossSummon();
            }
        }
    }

    private void BossSummon()
    {
        isBossSummon = true;
        isBossLiving = true;

        timer = 10;

        SoundManager.Instance.PlaySound("boss", SoundType.BGM);
        SoundManager.Instance.PlaySound("warning");

        GameObject bossObj = PoolManager.Instance.Init("Boss" + stage);
        bossObj.transform.position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle.normalized * 20;

        switch (stage)
        {
            case 1:
                UIManager.Instance.BossSetting("지우개 부릉이");
                break;
            case 2:
                UIManager.Instance.BossSetting("화이트 걸");
                break;
        }
    }

    private void MapSetting()
    {
        isBossSummon = false;
        isBossLiving = false;

        for (int i = 0; i < stageBackgrounds.Length; i++)
        {
            stageBackgrounds[i].gameObject.SetActive(i == stage - 1);
        }
    }

    public void GameOver()
    {
        if (!isGaming) return;

        Time.timeScale = 0;
        isGaming = false;
        UIManager.Instance.GameOver();
    }

    public void GoToTitle()
    {
        DOTween.KillAll();
        Cursor.visible = true;
        Time.timeScale = 1;
        PoolManager.Instance.DisableAllObjects();
        
        TransitionManager.Instance.LoadScene(SceneType.Title);
        
        TransitionManager.Instance.TransitionFadeOut();
    }

    public void OnKill(Enemy enemy)
    {
        killEnemyCount++;

        var exp = PoolManager.Instance.Init("Exp").GetComponent<Exp>();

        exp.transform.position = enemy.transform.position;
        exp.exp = (Random.Range(expRandomMin, expRandomMax) * enemy.stat.maxHp + enemy.stat.damage) * 1.3f;

        var effectObj = PoolManager.Instance.Init("Enemy Kill Effect");
        effectObj.transform.position = enemy.transform.position;
    }

    private void Update()
    {
        if (!isGaming)
            return;

        CameraMove();
        EnemyCreate();
        TimerUpdate();
    }

    private void CameraMove()
    {
        Vector3 pos = Player.Instance.transform.position - cameraDistance;
        GameManager.Instance.MainCamera.transform.position = new Vector3(Mathf.Clamp(pos.x, minPos.x*0.698f, maxPos.x * 0.698f), Mathf.Clamp(pos.y, minPos.y * 0.773f, maxPos.y * 0.773f), pos.z);
    }

    public Vector3 GetPosInMap(Vector3 vector, float radius)
    {
        return new Vector3(Mathf.Clamp(vector.x, minPos.x + radius, maxPos.x - radius), Mathf.Clamp(vector.y, minPos.y + radius, maxPos.y - radius), vector.z);
    }

    private void EnemyCreate()
    {
        enemyDuration += Time.deltaTime + Time.deltaTime * 0.002f * enemyPower/2;
        if (enemyDuration >= enemyCoolTime)
        {
            enemyDuration -= enemyCoolTime;
            enemyPower++;
            GameObject obj = PoolManager.Instance.Init("Enemy");
            obj.transform.position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle.normalized * 15;
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.stat.damage = 5 + 0.01f * enemyPower/2;
            enemy.stat.maxHp = 10 + 0.03f * enemyPower/2;
            enemy.stat.hp = enemy.stat.maxHp;
        }
    }

    #region 무기 획득 함수

    public void AddWeapon()
    {
        var chooseItems = new List<Item>();
        var itemList = new List<Item>();
        List<Item> upgradeItemList = Player.Instance.GetInven().FindAll(item => item.CanGet());
        foreach (Item item in ResourcesManager.Instance.GetAllItems())
            if (item.CanGet())
                itemList.Add(item);

        int itemCount = 3;
        if (upgradeItemList.Count > 0)
        {
            Item addItem = upgradeItemList.SelectOne();
            chooseItems.Add(addItem);
            itemList.Remove(addItem);
            itemCount--;
        }

        for (int i = 0; i < itemCount; i++)
        {
            if (itemList.Count <= 0)
            {
                Debug.LogAssertion("오류!");
                return;
            }

            Item addItem = itemList.SelectOne();
            chooseItems.Add(addItem);
            itemList.Remove(addItem);
        }
        
        UIManager.Instance.StartChooseItem(chooseItems);
    }

    #endregion

    #region 움직이는 텍스트 함수

    public void ShowText(string text, Vector3 pos, Color color)
    {
        GameObject damageTextObj = PoolManager.Instance.Init("DamageText");
        TextMeshPro textMesh = damageTextObj.GetComponent<TextMeshPro>();

        textMesh.text = text;
        textMesh.color = new Color(color.r, color.g, color.b, 0);

        textMesh.DOFade(1, textFadeInTime).SetEase(Ease.InBack).OnComplete(() => textMesh.DOFade(0, textFadeOutTime).SetEase(Ease.InBack));

        damageTextObj.transform.position = pos;

        damageTextObj.transform.DOMoveX(damageTextObj.transform.position.x + textMoveXPos, textFadeInTime + textFadeOutTime);
        damageTextObj.transform.DOMoveY(damageTextObj.transform.position.y + textMoveYPos, textFadeInTime + textFadeOutTime).SetEase(Ease.OutBack).OnComplete(() => damageTextObj.SetActive(false));
    }

    public void ShowInt(int damage, Vector3 pos, Color color)
    {
        ShowText(damage.ToString(), pos, color);
    }

    #endregion

    public void NextStage()
    {
        PoolManager.Instance.DisableAllObjects();
        UIManager.Instance.NextStageSetting();
        stage++;

        Player.Instance.stat.hp = Player.Instance.stat.maxHp;
        timer = 100;
        
        MapSetting();

        StartCoroutine(CarIntro());
    }

    public Player CreatePlayer()
    {
        return Instantiate(players[GameManager.Instance.characterType], Vector3.zero, Quaternion.identity);
    }
}