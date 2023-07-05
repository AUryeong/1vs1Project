using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum GameOverButton
{
    Restart,
    Title
}

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image mouseCursor;

    #region 프로필 변수

    [Header("프로필 변수")] [SerializeField] private Image hpBar;

    #endregion

    [SerializeField] private Image gameOverWindow;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button gameOverButton;

    public Image image;

    #region 무기 인벤토리 변수

    [Header("무기 인벤토리 변수")] [SerializeField]
    GridLayoutGroup inventoryLayoutGroup;

    protected RectTransform inventoryRectTransform;

    [SerializeField] ItemInven itemInventoryImage;
    protected List<ItemInven> itemInventories = new List<ItemInven>();

    private float itemInventoryFadeInPosX = 160f;
    private float itemInventoryFadeOutPosX = -56f;
    private float itemInventoryFadeDuration = 0.3f;

    private bool inventoryOpen = true;

    #endregion

    #region 무기 획득 변수

    [Header("무기 획득 변수")] [SerializeField] GameObject levelUpUI;
    [SerializeField] TMP_Text levelUpText;

    [SerializeField] RectTransform itemSlotsParent;
    [SerializeField] ItemSlot[] itemSlots;

    //[SerializeField] ParticleSystem itemSlotParticle;

    private Vector3 itemSlotsParentPos = new Vector3(1280, 0, 0);
    private float levelUpUIAppearDuration = 0.5f;

    private int itemSlotChooseIdx = 0;
    private bool itemSlotActiviting = false;

    private float deselctItemSlotMovePosX = 1500;
    private float selctItemSlotMovePosX = -2000;
    private float ItemSlotMoveDuration = 1;

    private float levelUpTextSinPower = 15;
    private float levelUpTextSinAdder = 0.5f;

    #endregion

    #region 타이머 변수

    [Header("타이머 변수")] [SerializeField] TextMeshProUGUI timerText;
    public float timer { get; private set; }

    #endregion

    #region 레벨 변수

    [Header("레벨 변수")] [SerializeField] private Image lvBarImage;
    [SerializeField] private TextMeshProUGUI lvBarText;

    #endregion


    public override void OnReset()
    {
        levelUpUI.gameObject.SetActive(false);
        gameOverWindow.gameObject.SetActive(false);
        itemSlotActiviting = false;

        inventoryOpen = true;
        itemInventories.Clear();
        inventoryRectTransform = inventoryLayoutGroup.GetComponent<RectTransform>();

        inventoryRectTransform.DOAnchorPosX(itemInventoryFadeOutPosX, itemInventoryFadeDuration);
        inventoryRectTransform.DOScaleX(1, itemInventoryFadeDuration);
        
        gameOverButton.onClick.RemoveAllListeners();
        gameOverButton.onClick.AddListener(GameOverButton);

        image.gameObject.SetActive(false);

        Cursor.visible = false;

        // gameOverWindow.gameObject.SetActive(false);

        timer = 0;

        UpdateLevel();
        UpdateHp(1);
    }

    public void UpdateHp(float value)
    {
        hpBar.DOFillAmount(value, 0.3f).SetUpdate(true);
    }

    private void Update()
    {
        var vector = Input.mousePosition;
        mouseCursor.rectTransform.anchoredPosition = new Vector2(vector.x, vector.y);
        if (!InGameManager.Instance.isGaming)
        {
            return;
        }

        CheckInventoryUI();
        if (levelUpUI.gameObject.activeSelf)
            UpdateLevelUpUI();
        UpdateTimer();
    }

    public bool IsActable()
    {
        return !levelUpUI.gameObject.activeSelf && Time.timeScale != 0 && InGameManager.Instance.isGaming;
    }

    #region 게임 오버 함수

    public void GameOver()
    {
        gameOverWindow.gameObject.SetActive(true);
        int timerInt = (int)timer;
        gameOverText.text = $"모나미 153은 {(timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2")} 만큼 생존했습니다.";
    }

    public void GameOverButton()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        image.gameObject.SetActive(true);
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.unscaledDeltaTime);
            yield return null;
        }
        SoundManager.Instance.PlaySound("button select", SoundType.SE);

        if (Player.Instance != null)
            Destroy(Player.Instance.gameObject);

        Cursor.visible = true;
        DOTween.KillAll();
        PoolManager.Instance.GameEnd();
        SingletonCanvas.Instance.gameObject.SetActive(false);

        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
    #endregion

    #region 무기 인벤토리 함수

    private void CheckInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (inventoryOpen)
            {
                inventoryRectTransform.DOAnchorPosX(itemInventoryFadeInPosX, itemInventoryFadeDuration).SetUpdate(true);
                inventoryRectTransform.DOScaleX(0, itemInventoryFadeDuration).SetUpdate(true);
            }
            else
            {
                inventoryRectTransform.DOAnchorPosX(itemInventoryFadeOutPosX, itemInventoryFadeDuration).SetUpdate(true);
                inventoryRectTransform.DOScaleX(1, itemInventoryFadeDuration).SetUpdate(true);
            }

            inventoryOpen = !inventoryOpen;
        }
    }

    public void UpdateItemInven(Item item)
    {
        ItemInven itemInventory = itemInventories.Find((ItemInven x) => x.gameObject.activeSelf && x.item == item);
        if (itemInventory == null)
        {
            itemInventory = PoolManager.Instance.Init(itemInventoryImage.gameObject).GetComponent<ItemInven>();
            itemInventory.rectTransform.SetParent(itemInventoryImage.rectTransform.parent);
            itemInventory.rectTransform.anchoredPosition3D = Vector3.zero;
            itemInventory.rectTransform.localScale = Vector3.one;
            itemInventories.Add(itemInventory);
        }

        int itemInventoryCount = itemInventories.FindAll((ItemInven x) => x.gameObject.activeSelf).Count;
        if (itemInventoryCount <= 3)
        {
            inventoryLayoutGroup.constraintCount = itemInventoryCount;
        }

        itemInventory.item = item;
    }

    #endregion

    #region 무기 획득 함수

    private void UpdateLevelUpUI()
    {
        //itemSlotParticle.transform.position = itemSlots[itemSlotChooseIdx].rectTransform.position;
        //TMPUpdate();
    }

    private void TMPUpdate()
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
                vectors[charInfo.vertexIndex + j] = vector + new Vector3(0, Mathf.Sin(Time.unscaledTime + i * levelUpTextSinAdder) * levelUpTextSinPower, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshinfo = textInfo.meshInfo[i];
            meshinfo.mesh.vertices = meshinfo.vertices;
            levelUpText.UpdateGeometry(meshinfo.mesh, i);
        }
    }

    public void StartChooseItem(List<Item> items)
    {
        itemSlotActiviting = true;
        SoundManager.Instance.PlaySound("level up", SoundType.SE);
        levelUpUI.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < items.Count; i++)
        {
            var a = itemSlots[i];
            a.item = items[i];
            a.GetComponent<Button>().onClick.AddListener(() => EndChooseItem(a));
        }

        itemSlotChooseIdx = 0;
        UpdateChooseSlot();

        foreach (ItemSlot itemSlot in itemSlots)
            itemSlot.rectTransform.anchoredPosition = Vector2.zero;
        //itemSlotsParent.anchoredPosition = itemSlotsParentPos;

        //itemSlotsParent.DOAnchorPosX(0, levelUpUIAppearDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void UpdateChooseSlot()
    {
        for (int i = 0; i < itemSlots.Length; i++)
            itemSlots[i].SlotSelect();
    }

    public void EndChooseItem(ItemSlot chooseItemSlot)
    {
        if (!itemSlotActiviting) return;
        itemSlotActiviting = false;
        SoundManager.Instance.PlaySound("item get", SoundType.SE);

        Player.Instance.AddItem(chooseItemSlot.item);

        levelUpUI.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    #endregion

    #region 타이머 함수

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
        int timerInt = (int)timer;
        timerText.text = "생존 시간 " + (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
    }

    #endregion

    #region 레벨 함수

    public void UpdateLevel()
    {
        lvBarImage.DOFillAmount(Player.Instance.Exp / Player.Instance.maxExp, 0.3f).SetUpdate(true);
        lvBarText.text = "LV: " + Player.Instance.lv;
    }

    #endregion
}