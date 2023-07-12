using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image mouseCursor;

    #region ������ ����

    [Header("������ ����")] [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI scoreText;

    #endregion

    [Header("���� ���� ����")]
    [SerializeField] private Image gameOverWindow;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button gameOverButton;
    
    [Header("���� �¸� ����")]
    [SerializeField] private Image gameWinWindow;
    [SerializeField] private TextMeshProUGUI gameWinText;
    [SerializeField] private Button gameWinButton;

    #region ���� ȹ�� ����

    [Header("���� ȹ�� ����")] 
    [SerializeField] private GameObject levelUpUI;
    [SerializeField] private ItemSlot[] itemSlots;

    private bool itemSlotActivating;

    #endregion

    #region Ÿ�̸� ����

    [Header("Ÿ�̸� ����")] [SerializeField] private TextMeshProUGUI timerText;

    #endregion

    #region ���� ����

    [Header("���� ����")] [SerializeField] private Image lvBarImage;
    [SerializeField] private TextMeshProUGUI lvBarText;

    #endregion

    [Header("���� ����")] 
    [SerializeField] private Image bossBar;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image bossGaugeBar;
    [SerializeField] private Image bossWarning;

    [Header("�̵� �� ���")] 
    [SerializeField] private Image moveBackground;
    [SerializeField] private Image moveCartoon;

    [Header("����")] 
    [SerializeField] private Image scoreBackground;
    [SerializeField] private TextMeshProUGUI scoreScoreText;
    [SerializeField] private TextMeshProUGUI scoreEnemyText;
    [SerializeField] private TextMeshProUGUI scoreLvText;
    [SerializeField] private Button scoreNextStageButton;


    protected override void OnCreated()
    {
        levelUpUI.gameObject.SetActive(false);
        gameOverWindow.gameObject.SetActive(false);
        gameWinWindow.gameObject.SetActive(false);
        bossBar.gameObject.SetActive(false);
        bossWarning.gameObject.SetActive(false);
        scoreBackground.gameObject.SetActive(false);
        moveBackground.gameObject.SetActive(false);
        
        itemSlotActivating = false;

        gameOverButton.onClick.RemoveAllListeners();
        gameOverButton.onClick.AddListener(GameOverButton);

        gameWinButton.onClick.RemoveAllListeners();
        gameWinButton.onClick.AddListener(GameOverButton);
        
        scoreNextStageButton.onClick.RemoveAllListeners();
        scoreNextStageButton.onClick.AddListener(InGameManager.Instance.NextStage);

        TransitionManager.Instance.background.gameObject.SetActive(false);
        TransitionManager.Instance.transitionSquare.gameObject.SetActive(false);

        Player.Instance.gameObject.SetActive(false);

        Cursor.visible = false;

        UpdateLevel();
        UpdateHp(1);
    }

    public void UpdateHp(float value)
    {
        hpBar.DOFillAmount(value, 0.3f).SetUpdate(true);
    }

    public void MoveBackgroundSetting()
    {
        moveBackground.gameObject.SetActive(true);

        moveBackground.color = moveBackground.color.FadeChange(0);
        moveBackground.DOFade(1, 1).OnComplete(ScoreSetting);
        moveBackground.DOFade(0, 1).SetDelay(3).OnComplete(() => moveBackground.gameObject.SetActive(false));

        moveCartoon.color = moveCartoon.color.FadeChange(0);
        moveCartoon.DOFade(1, 1);
        moveCartoon.DOFade(0, 1).SetDelay(3).OnComplete(() => moveCartoon.gameObject.SetActive(false));
    }

    private void ScoreSetting()
    {
        scoreBackground.gameObject.SetActive(true);
        scoreEnemyText.text = InGameManager.Instance.killEnemyCount.ToString();
        scoreScoreText.text = InGameManager.Instance.Score + "��";
        scoreLvText.text = "Lv." + Player.Instance.lv;
    }

    public void NextStageSetting()
    {
        scoreBackground.gameObject.SetActive(false);
        moveBackground.gameObject.SetActive(false);
    }

    private void Update()
    {
        var vector = Input.mousePosition;
        mouseCursor.rectTransform.anchoredPosition = new Vector2(vector.x, vector.y);
        if (!InGameManager.Instance.isGaming)
        {
            return;
        }

        ScoreUpdate();
    }

    private void ScoreUpdate()
    {
        scoreText.text = "���� : " + InGameManager.Instance.Score;
    }

    public void BossSetting(string bossName)
    {
        bossNameText.text = bossName;
        timerText.text = "!!! ���� ���� !!!";
        bossWarning.gameObject.SetActive(true);
        bossWarning.color = new Color(1, 0, 0, 0);
        
        bossWarning.DOFade(0.2f, 0.6f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.Linear).OnComplete(() => bossWarning.gameObject.SetActive(false));
        
        bossBar.rectTransform.anchoredPosition = new Vector2(0, -140);
        bossBar.gameObject.SetActive(true);
        
        bossGaugeBar.fillAmount = 0;    

        bossBar.rectTransform.DOAnchorPosY(71f, 1).SetEase(Ease.OutBack);
        bossGaugeBar.DOFillAmount(1, 1).SetDelay(0.5f);
    }

    public void BossRemoveSetting()
    {
        bossBar.gameObject.SetActive(false);
    }

    public void UpdateBossHp(float fillAmount)
    {
        bossGaugeBar.fillAmount = fillAmount;
    }

    public bool IsActable()
    {
        return !levelUpUI.gameObject.activeSelf && Time.timeScale != 0 && InGameManager.Instance.isGaming;
    }

    public void GameWin()
    {
        gameWinWindow.gameObject.SetActive(true);
        gameWinText.text = $"ȹ���� ���� : {InGameManager.Instance.Score}";
    }

    #region ���� ���� �Լ�

    public void GameOver()
    {
        gameOverWindow.gameObject.SetActive(true);
        gameOverText.text = $"ȹ���� ���� : {InGameManager.Instance.Score}";
    }

    private void GameOverButton()
    {
        TransitionManager.Instance.TransitionFadeOut(TransitionType.Fade, () =>
        {
            SoundManager.Instance.PlaySound("button select");
            InGameManager.Instance.GoToTitle();
        });
    }

    #endregion

    #region ���� ȹ�� �Լ�

    public void StartChooseItem(List<Item> items)
    {
        itemSlotActivating = true;
        SoundManager.Instance.PlaySound("level up");
        levelUpUI.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < items.Count; i++)
        {
            var a = itemSlots[i];
            a.item = items[i];
            a.GetComponent<Button>().onClick.AddListener(() => EndChooseItem(a));
        }

        foreach (var itemSLot in itemSlots)
            itemSLot.SlotSelect();

        foreach (ItemSlot itemSlot in itemSlots)
            itemSlot.rectTransform.anchoredPosition = Vector2.zero;
    }

    private void EndChooseItem(ItemSlot chooseItemSlot)
    {
        if (!itemSlotActivating) return;
        
        itemSlotActivating = false;
        SoundManager.Instance.PlaySound("item get");

        Player.Instance.AddItem(chooseItemSlot.item);

        levelUpUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    #endregion

    #region Ÿ�̸� �Լ�

    public void UpdateTimer(float timer)
    {
        int timerInt = Mathf.CeilToInt(timer);
        if (InGameManager.Instance.isBossSummon || InGameManager.Instance.stage == 1)
            timerText.text = "�̵����� " + (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
        else
            timerText.text = "���� �������� " + (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
    }

    #endregion

    #region ���� �Լ�

    public void UpdateLevel()
    {
        lvBarImage.DOFillAmount(Player.Instance.Exp / Player.Instance.maxExp, 0.3f).SetUpdate(true);
        lvBarText.text = "LV: " + Player.Instance.lv;
    }

    #endregion
}