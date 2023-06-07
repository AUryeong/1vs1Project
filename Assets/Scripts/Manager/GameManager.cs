using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public List<Enemy> inCameraEnemies = new List<Enemy>();
    private Vector3 cameraDistance = new Vector3(0, 0, 100);
    public bool isGaming
    {
        get;
        private set;
    }

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
    [SerializeField]
    private GameObject damageText;
    private float fadeInTime = 0.2f;
    private float fadeOutTime = 0.5f;
    private float moveXPos = 0.7f;
    private float moveYPos = 1f;
    #endregion

    public override void OnReset()
    {
        ResourcesManager.Instance.OnReset();
        UIManager.Instance.OnReset();
        PoolManager.Instance.OnReset();

        isGaming = true;
    }

    public void GameOver()
    {
        if (!isGaming)
            return;
        Time.timeScale = 0;
        isGaming = false;
        UIManager.Instance.GameOver();
    }

    public void OnKill(Enemy enemy)
    {
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
        Camera.main.transform.position = Player.Instance.transform.position - cameraDistance;
        EnemyCreate();
    }

    private void EnemyCreate()
    {
        enemyDuration += Time.deltaTime + Time.deltaTime * 0.002f * enemyPower;
        if (enemyDuration >= enemyCooltime)
        {
            enemyDuration -= enemyCooltime;
            enemyPower++;
            GameObject obj = PoolManager.Instance.Init(enemyBase.gameObject);
            obj.transform.position = Player.Instance.transform.position + (Vector3)Random.insideUnitCircle.normalized * 10;
            Enemy enemy = obj.GetComponent<Enemy>();
            enemy.stat.damage = 5 + 0.01f * enemyPower;
            enemy.stat.maxHp = 10 + 0.03f * enemyPower;
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

        exp.exp = Random.Range(expRandomMin, expRandomMax) * enemy.stat.maxHp + enemy.stat.damage;
    }

    #endregion

    #region 움직이는 텍스트 함수

    public void ShowText(string text, Vector3 pos, Color color)
    {
        GameObject damageTextObj = PoolManager.Instance.Init(damageText);
        TextMeshPro textMesh = damageTextObj.GetComponent<TextMeshPro>();

        textMesh.text = text;
        textMesh.color = new Color(color.r, color.g, color.b, 0);

        textMesh.DOFade(1, fadeInTime).SetEase(Ease.InBack).
            OnComplete(() => textMesh.DOFade(0, fadeOutTime).SetEase(Ease.InBack));

        damageTextObj.transform.position = pos;

        damageTextObj.transform.DOMoveX(damageTextObj.transform.position.x + moveXPos, fadeInTime + fadeOutTime);
        damageTextObj.transform.DOMoveY(damageTextObj.transform.position.y + moveYPos, fadeInTime + fadeOutTime).SetEase(Ease.OutBack).
            OnComplete(() => damageTextObj.SetActive(false));
    }
    public void ShowInt(int damage, Vector3 pos, Color color)
    {
        ShowText(damage.ToString(), pos, color);
    }
    #endregion
}
