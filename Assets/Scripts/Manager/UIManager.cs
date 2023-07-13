using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image mouseCursor;

    [Header("프로필 변수")] [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("게임 오버 변수")]
    [SerializeField] private Image gameOverWindow;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button gameOverButton;
    
    [Header("게임 승리 변수")]
    [SerializeField] private Image gameWinWindow;
    [SerializeField] private TextMeshProUGUI gameWinText;
    [SerializeField] private Button gameWinButton;

    [Header("무기 획득 변수")] 
    [SerializeField] private GameObject levelUpUI;
    [SerializeField] private ItemSlot[] itemSlots;

    private bool itemSlotActivating;

    [Header("타이머 변수")] [SerializeField] private TextMeshProUGUI timerText;

    [Header("레벨 변수")] [SerializeField] private Image lvBarImage;
    [SerializeField] private TextMeshProUGUI lvBarText;

    [Header("보스 변수")] 
    [SerializeField] private Image bossBar;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image bossGaugeBar;
    [SerializeField] private Image bossWarning;

    [Header("이동 중 배경")] 
    [SerializeField] private Image moveBackground;
    [SerializeField] private Image moveCartoon;

    [Header("점수")] 
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
        scoreScoreText.text = InGameManager.Instance.Score + "점";
        scoreLvText.text = "Lv." + Player.Instance.lv;
    }

    public void NextStageSetting()
    {
        scoreBackground.gameObject.SetActive(false);
        moveBackground.gameObject.SetActive(false);
    }

    private void Update()
    {
        MouseUpdate();
        if (!InGameManager.Instance.isGaming) return;

        ScoreUpdate();
    }

    private void MouseUpdate()
    {
        var vector = Input.mousePosition;
        mouseCursor.rectTransform.anchoredPosition = new Vector2(vector.x, vector.y);
    }

    private void ScoreUpdate()
    {
        scoreText.text = "점수 : " + InGameManager.Instance.Score;
    }

    public void BossSetting(string bossName)
    {
        bossNameText.text = bossName;
        timerText.text = "!!! 보스 출현 !!!";
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
        gameWinText.text = $"획득한 점수 : {InGameManager.Instance.Score}";
    }

    public void GameOver()
    {
        gameOverWindow.gameObject.SetActive(true);
        gameOverText.text = $"획득한 점수 : {InGameManager.Instance.Score}";
    }

    private void GameOverButton()
    {
        TransitionManager.Instance.TransitionFadeOut(TransitionType.Fade, () =>
        {
            SoundManager.Instance.PlaySound("button select");
            InGameManager.Instance.GoToTitle();
        });
    }
    public void StartChooseItem(List<Item> items)
    {
        itemSlotActivating = true;
        SoundManager.Instance.PlaySound("level up", SoundType.Se , 0.8f);
        levelUpUI.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < items.Count; i++)
        {
            var itemSlot = itemSlots[i];
            itemSlot.Item = items[i];
            itemSlot.Button.onClick.AddListener(() => EndChooseItem(itemSlot));
        }

        foreach (var itemSLot in itemSlots)
            itemSLot.SlotSelect();

        foreach (ItemSlot itemSlot in itemSlots)
            itemSlot.RectTransform.anchoredPosition = Vector2.zero;
    }

    private void EndChooseItem(ItemSlot chooseItemSlot)
    {
        if (!itemSlotActivating) return;
        
        itemSlotActivating = false;
        SoundManager.Instance.PlaySound("item get", SoundType.Se , 0.6f);

        Player.Instance.AddItem(chooseItemSlot.Item);

        levelUpUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void UpdateTimer(float timer)
    {
        int timerInt = Mathf.CeilToInt(timer);
        if (InGameManager.Instance.isBossSummon)
            timerText.text = "이동까지 " + (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
        else
            timerText.text = "보스 출현까지 " + (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
    }

    public void UpdateLevel()
    {
        lvBarImage.DOFillAmount(Player.Instance.Exp / Player.Instance.maxExp, 0.3f).SetUpdate(true);
        lvBarText.text = "LV: " + Player.Instance.lv;
    }
}