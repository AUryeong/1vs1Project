using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image mouseCursor;

    [Header("프로필 변수")]
    [SerializeField] private Image profile;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI killEnemyText;

    [Header("게임 오버 변수")]
    [SerializeField] private Image gameOverWindow;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button gameOverButton;

    [Header("무기 획득 변수")]
    [SerializeField] private TextMeshProUGUI levelUpText;
    [SerializeField] private GameObject particleParent;
    private Vector3 particleDistance;
    [SerializeField] private GameObject levelUpUI;
    [SerializeField] private ItemSlot[] itemSlots;

    private bool itemSlotActivating;

    [Header("레벨 변수")] 
    [SerializeField] private Image lvBarImage;
    [SerializeField] private TextMeshProUGUI lvBarText;

    [Header("보스 변수")] 
    [SerializeField] private Image bossBar;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image bossGaugeBar;
    [SerializeField] private Image bossWarning;

    [Header("점수")] 
    [SerializeField] private Image scoreBackground;
    [SerializeField] private TextMeshProUGUI scoreScoreText;
    [SerializeField] private TextMeshProUGUI scoreEnemyText;
    [SerializeField] private TextMeshProUGUI scoreLvText;
    [SerializeField] private Image scoreCharacter;
    [SerializeField] private Image scoreEnemyCount;
    [SerializeField] private Image scoreForeground;
    [SerializeField] private Image scoreWindow;
    [SerializeField] private Button scoreNextStageButton;

    [Header("ESC 변수")] 
    [SerializeField] private Image escBackground;
    [SerializeField] private Image escWindow;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private List<ItemInventory> itemInventories;
    [SerializeField] private Image settingWindow;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button settingExitButton;

    private bool isQuiting = false;

    [Header("캐릭별 이미지")]
    [SerializeField] private Dictionary<CharacterType, Sprite> sdSprites = new Dictionary<CharacterType, Sprite>();
    [SerializeField] private Dictionary<CharacterType, Sprite> ldSprites = new Dictionary<CharacterType, Sprite>();
    [SerializeField] private Dictionary<CharacterType, Sprite> profileSprites = new Dictionary<CharacterType, Sprite>();
    
    [SerializeField] private Image[] sdCharacters;
    [SerializeField] private Image[] ldCharacters;
    [SerializeField] private Image[] profileCharacters;

    protected override void OnCreated()
    {
        profile.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        levelUpUI.gameObject.SetActive(false);
        gameOverWindow.gameObject.SetActive(false);
        bossBar.gameObject.SetActive(false);
        bossWarning.gameObject.SetActive(false);
        scoreBackground.gameObject.SetActive(false);
        escBackground.gameObject.SetActive(false);
        
        itemSlotActivating = false;

        gameOverButton.onClick.RemoveAllListeners();
        gameOverButton.onClick.AddListener(GameExitButton);
        
        scoreNextStageButton.onClick.RemoveAllListeners();
        scoreNextStageButton.onClick.AddListener(InGameManager.Instance.NextStage);
        
        resumeButton.onClick.RemoveAllListeners();
        resumeButton.onClick.AddListener(EscExit);
        
        settingButton.onClick.RemoveAllListeners();
        settingButton.onClick.AddListener(SettingSetting);
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(GameExitButton);
        
        bgmSlider.value = SaveManager.Instance.SaveData.bgmVolume;
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener((value)=>
        {
            SoundManager.Instance.VolumeChange(SoundType.BGM, value);  
        });

        sfxSlider.value = SaveManager.Instance.SaveData.sfxVolume;
        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.AddListener((value)=>
        {
            SoundManager.Instance.VolumeChange(SoundType.Se, value);  
        });
        
        settingExitButton.onClick.RemoveAllListeners();
        settingExitButton.onClick.AddListener(SettingExit);
        
        settingWindow.gameObject.SetActive(false);

        TransitionManager.Instance.background.gameObject.SetActive(false);
        TransitionManager.Instance.transitionSquare.gameObject.SetActive(false);

        Cursor.visible = false;

        particleDistance = particleParent.transform.position;
        particleParent.SetActive(false);

        UpdateLevel();
        UpdateHp(1);
        UpdateKillEnemyCount(0);
        CharacterImageSetting();
    }

    private void SettingSetting()
    {
        SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);
        settingWindow.gameObject.SetActive(true);
        settingWindow.rectTransform.DOKill();
        settingWindow.rectTransform.localScale = Vector3.zero;
        
        settingWindow.rectTransform.DOScale(1, 0.3f).SetEase(Ease.InQuad).SetUpdate(true);
    }

    private void SettingExit()
    {
        SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);
        settingWindow.rectTransform.DOKill();
        settingWindow.rectTransform.localScale = Vector3.one;
        
        settingWindow.rectTransform.DOScale(0, 0.3f).SetUpdate(true).OnComplete(() => settingWindow.gameObject.SetActive(false));
    }

    private void CharacterImageSetting()
    {
        Sprite sdSprite = sdSprites[GameManager.Instance.characterType];
        foreach(var image in sdCharacters)
        {
            image.sprite = sdSprite;
            image.SetNativeSize();
        }

        Sprite ldSprite = ldSprites[GameManager.Instance.characterType];
        foreach (var image in ldCharacters)
        {
            image.sprite = ldSprite;
            image.SetNativeSize();
        }

        Sprite profileSprite = profileSprites[GameManager.Instance.characterType];
        foreach (var image in profileCharacters)
        {
            image.sprite = profileSprite;
            image.SetNativeSize();
        }
    }

    public void UpdateHp(float value)
    {
        hpBar.DOFillAmount(value, 0.3f).SetUpdate(true);
    }

    public void ScoreSetting()
    {
        SoundManager.Instance.PlaySound("score");
        scoreBackground.gameObject.SetActive(true);
        
        scoreEnemyText.text = InGameManager.Instance.killEnemyCount.ToString();
        scoreScoreText.text = InGameManager.Instance.Score + "점";
        scoreLvText.text = "Lv." + Player.Instance.lv;

        scoreForeground.rectTransform.DOAnchorPosY(-703, 2f).SetUpdate(true).From();
        scoreCharacter.rectTransform.DOAnchorPosX(-1338, 1.5f).SetEase(Ease.OutBack).SetUpdate(true).From();
        scoreEnemyCount.rectTransform.DOAnchorPosY(752, 2.5f).SetUpdate(true).From();
        scoreWindow.rectTransform.DOAnchorPosX(1445, 1.5f).SetEase(Ease.OutBack).SetUpdate(true).From();
        scoreLvText.rectTransform.DOScale(0, 1.7f).SetUpdate(true).From();
        scoreNextStageButton.image.rectTransform.DOAnchorPosY(-1580f, 3.5f).From();
    }

    public void NextStageSetting()
    {
        scoreBackground.gameObject.SetActive(false);
    }

    private void Update()
    {
        MouseUpdate();
        if (!InGameManager.Instance.isGaming)
        {
            EscExitUpdate();
            return;
        }

        TextUpdate();
        ScoreUpdate();
        EscEnterUpdate();
    }

    private void EscEnterUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscSetting();
        }
    }

    private void EscExitUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscExit();
        }
    }

    private void EscExit()
    {
        InGameManager.Instance.isGaming = true;
        if(!itemSlotActivating) 
            Time.timeScale = 1;
            
        escBackground.gameObject.SetActive(false);
    }

    private void EscSetting()
    {
        InGameManager.Instance.isGaming = false;
        Time.timeScale = 0;
        
        escBackground.gameObject.SetActive(true);

        settingWindow.gameObject.SetActive(false);
        
        escWindow.rectTransform.DOKill();
        escWindow.rectTransform.localScale = Vector3.zero;
        escWindow.rectTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        var items = Player.Instance.GetInventory();
        for (int i = 0; i < itemInventories.Count; i++)
        {
            if (items.Count > i)
            {
                itemInventories[i].gameObject.SetActive(true);
                itemInventories[i].Init(items[i]);
            }
            else
            {
                itemInventories[i].gameObject.SetActive(false);
            }
        }
    }

    public void GameStart()
    {
        profile.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
    }

    private void TextUpdate()
    {
        if (levelUpUI.gameObject.activeSelf)
        {
            levelUpText.ForceMeshUpdate();
            TMP_TextInfo textInfo = levelUpText.textInfo;
            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                Vector3[] vectors = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                for (int j = 0; j < 4; ++j)
                {
                    var vector = vectors[charInfo.vertexIndex + j];
                    vectors[charInfo.vertexIndex + j] = vector + new Vector3(0, Mathf.Sin(Time.unscaledTime + i) * 10, 0);
                }
            }
            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                var meshinfo = textInfo.meshInfo[i];
                meshinfo.mesh.vertices = meshinfo.vertices;
                levelUpText.UpdateGeometry(meshinfo.mesh, i);
            }
        }
    }

    private void MouseUpdate()
    {
        var vector = Input.mousePosition;
        mouseCursor.rectTransform.anchoredPosition = new Vector2(vector.x, vector.y);
    }

    private void ScoreUpdate()
    {
        scoreText.text = InGameManager.Instance.Score.ToString();
    }

    public void BossSetting(string bossName)
    {
        bossNameText.text = bossName;
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

    public void UpdateKillEnemyCount(int count)
    {
        killEnemyText.text = count.ToString();
    }

    public bool IsActable()
    {
        return !levelUpUI.activeSelf && Time.timeScale != 0 && InGameManager.Instance.isGaming;
    }

    public void GameOver()
    {
        gameOverWindow.gameObject.SetActive(true);
        gameOverText.text = $"획득한 점수 : {InGameManager.Instance.Score}";
    }

    private void GameExitButton()
    {
        if (isQuiting) return;
        isQuiting = true;

        SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);
        TransitionManager.Instance.TransitionFadeOut(TransitionType.Fade, () =>
        {
            SoundManager.Instance.PlaySound("button select");
            InGameManager.Instance.GoToTitle();
        });
    }
    public void StartChooseItem(List<Item> items)
    {
        itemSlotActivating = true;

        particleParent.SetActive(true);
        particleParent.transform.position = Player.Instance.transform.position + particleDistance;
        
        SoundManager.Instance.PlaySound("level up", SoundType.Se , 0.8f);
        levelUpUI.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < items.Count; i++)
        {
            var itemSlot = itemSlots[i];
            itemSlot.Item = items[i];
        }
    }

    public void EndChooseItem(ItemSlot chooseItemSlot)
    {
        if (!itemSlotActivating) return;

        particleParent.SetActive(false);

        itemSlotActivating = false;
        SoundManager.Instance.PlaySound("item get", SoundType.Se , 0.6f);

        Player.Instance.AddItem(chooseItemSlot.Item);

        levelUpUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void UpdateTimer(float timer)
    {
        int timerInt = Mathf.CeilToInt(timer);
        timerText.text = (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
    }

    public void UpdateLevel()
    {
        lvBarImage.DOFillAmount(Player.Instance.Exp / Player.Instance.maxExp, 0.3f).SetUpdate(true);
        lvBarText.text = "LV: " + Player.Instance.lv;
    }
}