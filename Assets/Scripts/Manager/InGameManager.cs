using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class InGameManager : Singleton<InGameManager>
{
    [HideInInspector] public List<Enemy> inCameraEnemies = new List<Enemy>();
    private Vector3 cameraDistance = new Vector3(0, 0, 100);

    public Vector2 minPos;
    public Vector2 maxPos;

    public bool isGaming;
    public float timer { get; private set; }
    public int killEnemyCount;

    #region 보스 관련 변수

    public bool isBossSummon;
    public bool isBossLiving;

    #endregion

    #region 몹 관련 변수

    [SerializeField] private Enemy enemyBase;
    public ParticleSystem enemyKillEffect;

    private float enemyKillEffectDuration = 2;

    private float enemyCooltime = 0.5f;
    private float enemyDuration = 0;
    private float enemyPower = 0;


    public Material flashWhiteMaterial;

    #endregion

    #region 레벨 변수

    [SerializeField] private GameObject exp;
    private float expRandomMin = 0.5f;
    private float expRandomMax = 1.5f;

    #endregion

    #region 움직이는 텍스트 변수

    [SerializeField] private GameObject damageText;
    private float fadeInTime = 0.2f;
    private float fadeOutTime = 0.5f;
    private float moveXPos = 0.7f;
    private float moveYPos = 1f;

    #endregion

    public int stage;
    [SerializeField] private SpriteRenderer[] stageBackgrounds;
    [SerializeField] private Boss[] stageBoss;
    [SerializeField] private SpriteRenderer carImage;

    public int Score
    {
        get
        {
            return Mathf.RoundToInt(killEnemyCount + (stage - 1) * 1000 + (60 - timer));
        }
    }

    public override void OnReset()
    {
        isGaming = false;

        TransitionManager.Instance.TransitionFadeIn(TransitionType.Fade, () => StartCoroutine(CarIntro()));

        stage = 1;
        timer = 60;
        killEnemyCount = 0;

        MapSetting();
    }

    IEnumerator CarIntro()
    {
        Player.Instance.transform.position = Vector3.zero;
        Camera.main.transform.position = Vector3.zero - cameraDistance;
        
        carImage.gameObject.SetActive(true);
        carImage.transform.position = new Vector3(16, 0, 0);
        carImage.transform.DOMoveX(0, 2f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(3);

        SingletonCanvas.Instance.gameObject.SetActive(true);
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

    IEnumerator CarOutro()
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

        GameObject bossObj = PoolManager.Instance.Init(stageBoss[stage - 1].gameObject);
        bossObj.transform.position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle.normalized * 20;

        switch (stage)
        {
            case 1:
                UIManager.Instance.BossSetting("지우개 부릉이");
                break;
            case 2:
                UIManager.Instance.BossSetting("지우개 부릉이");
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

    public void Exit()
    {
        foreach (var stageBackground in stageBackgrounds)
            stageBackground.gameObject.SetActive(false);
    }

    public void OnKill(Enemy enemy)
    {
        killEnemyCount++;
        inCameraEnemies.Remove(enemy);

        CreateExp(enemy);

        GameObject effectObj = PoolManager.Instance.Init(enemyKillEffect.gameObject);
        effectObj.transform.position = enemy.transform.position;

        AutoDestruct autoDestruct = effectObj.GetComponent<AutoDestruct>();
        if (autoDestruct == null)
            autoDestruct = effectObj.AddComponent<AutoDestruct>();

        autoDestruct.duration = enemyKillEffectDuration;
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
        Camera.main.transform.position = new Vector3(Mathf.Clamp(pos.x, -29.7675f, 29.7675f), Mathf.Clamp(pos.y, -24.75f, 24.75f), pos.z);
    }

    public Vector3 GetPosInMap(Vector3 vector, float radius)
    {
        return new Vector3(Mathf.Clamp(vector.x, minPos.x + radius, maxPos.x - radius), Mathf.Clamp(vector.y, minPos.y + radius, maxPos.y - radius), vector.z);
    }

    private void EnemyCreate()
    {
        enemyDuration += Time.deltaTime + Time.deltaTime * 0.002f * enemyPower/2;
        if (enemyDuration >= enemyCooltime)
        {
            enemyDuration -= enemyCooltime;
            enemyPower++;
            GameObject obj = PoolManager.Instance.Init(enemyBase.gameObject);
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
        List<Item> chooseItems = new List<Item>();
        List<Item> itemList = new List<Item>();
        List<Item> upgradeItemList = Player.Instance.GetInven().FindAll((Item x) => x.CanGet());
        foreach (Item item in ResourcesManager.Instance.items.Values)
            if (item.CanGet())
                itemList.Add(item);

        int itemCount = 3;
        if (upgradeItemList.Count > 0)
        {
            Item addItem = RandomManager.SelectOne(upgradeItemList);
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

            Item addItem = RandomManager.SelectOne(itemList);
            chooseItems.Add(addItem);
            itemList.Remove(addItem);
        }


        UIManager.Instance.StartChooseItem(chooseItems);
    }

    #endregion

    #region 레벨 함수

    public void CreateExp(Enemy enemy)
    {
        GameObject expObj = PoolManager.Instance.Init(this.exp);
        Exp exp = expObj.GetComponent<Exp>();

        expObj.transform.position = enemy.transform.position;

        exp.exp = (Random.Range(expRandomMin, expRandomMax) * enemy.stat.maxHp + enemy.stat.damage) * 1.3f;
    }

    #endregion

    #region 움직이는 텍스트 함수

    public void ShowText(string text, Vector3 pos, Color color)
    {
        GameObject damageTextObj = PoolManager.Instance.Init(damageText);
        TextMeshPro textMesh = damageTextObj.GetComponent<TextMeshPro>();

        textMesh.text = text;
        textMesh.color = new Color(color.r, color.g, color.b, 0);

        textMesh.DOFade(1, fadeInTime).SetEase(Ease.InBack).OnComplete(() => textMesh.DOFade(0, fadeOutTime).SetEase(Ease.InBack));

        damageTextObj.transform.position = pos;

        damageTextObj.transform.DOMoveX(damageTextObj.transform.position.x + moveXPos, fadeInTime + fadeOutTime);
        damageTextObj.transform.DOMoveY(damageTextObj.transform.position.y + moveYPos, fadeInTime + fadeOutTime).SetEase(Ease.OutBack).OnComplete(() => damageTextObj.SetActive(false));
    }

    public void ShowInt(int damage, Vector3 pos, Color color)
    {
        ShowText(damage.ToString(), pos, color);
    }

    #endregion

    public void NextStage()
    {
        PoolManager.Instance.GameEnd();
        UIManager.Instance.NextStageSetting();
        stage++;

        Player.Instance.stat.hp = Player.Instance.stat.maxHp;
        timer = 100;
        
        MapSetting();

        StartCoroutine(CarIntro());
    }
}