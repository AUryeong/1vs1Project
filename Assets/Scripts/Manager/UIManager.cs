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

    #region ������ ����

    [Header("������ ����")] [SerializeField] private Image hpBar;

    #endregion

    #region ���� ���� ����

    [Header("���� ���� ����")] [SerializeField] Image gameOverWindow;
    [SerializeField] TextMeshProUGUI killText;
    [SerializeField] TextMeshProUGUI recordText;
    [SerializeField] Image[] gameOverButtons;

    [SerializeField] Sprite selectGameOverButton;
    [SerializeField] Sprite deselectGameOverButton;

    private float deselctGameOverMovePosY = 0;
    private float selectGameOverMovePosY = 50;

    private bool gameOverActivating = false;

    private int gameOverSlotIdx;

    #endregion

    #region ���� �κ��丮 ����

    [Header("���� �κ��丮 ����")] [SerializeField]
    GridLayoutGroup inventoryLayoutGroup;

    protected RectTransform inventoryRectTransform;

    [SerializeField] ItemInven itemInventoryImage;
    protected List<ItemInven> itemInventories = new List<ItemInven>();

    private float itemInventoryFadeInPosX = 160f;
    private float itemInventoryFadeOutPosX = -56f;
    private float itemInventoryFadeDuration = 0.3f;

    private bool inventoryOpen = true;

    #endregion

    #region ���� ȹ�� ����

    [Header("���� ȹ�� ����")] [SerializeField] GameObject levelUpUI;
    [SerializeField] TMP_Text levelUpText;

    [SerializeField] RectTransform itemSlotsParent;
    [SerializeField] ItemSlot[] itemSlots;

    //[SerializeField] ParticleSystem itemSlotParticle;

    public Sprite deselectSlotSprite;
    public Sprite selectSlotSprite;

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

    #region Ÿ�̸� ����

    [Header("Ÿ�̸� ����")] [SerializeField] TextMeshProUGUI timerText;
    public float timer { get; private set; }

    #endregion

    #region ���� ����

    [Header("���� ����")] [SerializeField] private Image lvBarImage;
    [SerializeField] private TextMeshProUGUI lvBarText;

    #endregion

    public override void OnReset()
    {
        levelUpUI.gameObject.SetActive(false);
        itemSlotActiviting = false;

        inventoryOpen = true;
        itemInventories.Clear();
        inventoryRectTransform = inventoryLayoutGroup.GetComponent<RectTransform>();

        inventoryRectTransform.DOAnchorPosX(itemInventoryFadeOutPosX, itemInventoryFadeDuration);
        inventoryRectTransform.DOScaleX(1, itemInventoryFadeDuration);

        Cursor.visible = false;

        // gameOverWindow.gameObject.SetActive(false);

        timer = 0;

        UpdateLevel();
    }

    public void UpdateHp(float value)
    {
        hpBar.DOFillAmount(value, 0.3f).SetUpdate(true);
    }

    private void Update()
    {
        if (!GameManager.Instance.isGaming)
        {
            UpdateGameOver();
            return;
        }

        var vector = Input.mousePosition;
        mouseCursor.rectTransform.anchoredPosition = new Vector2(vector.x, vector.y);
        CheckInventoryUI();
        if (levelUpUI.gameObject.activeSelf)
            UpdateLevelUpUI();
        UpdateTimer();
    }

    public bool IsActable()
    {
        return !levelUpUI.gameObject.activeSelf && Time.timeScale != 0 && GameManager.Instance.isGaming;
    }

    #region ���� ���� �Լ�

    public void GameOver()
    {
        gameOverSlotIdx = 1;
        SaveManager.Instance.saveData.timer = (int)timer;
        SaveManager.Instance.saveData.maxTimer = Mathf.Max((int)timer, SaveManager.Instance.saveData.maxTimer);
        gameOverActivating = true;
        SelectGameOverSlot();

        /*  gameOverWindow.gameObject.SetActive(true);
          gameOverSlotIdx = 0;
          gameOverActivating = false;
  
          killText.gameObject.SetActive(false);
          recordText.gameObject.SetActive(false);
          foreach (Image image in gameOverButtons)
              image.gameObject.SetActive(false);
  
          StartCoroutine(GameOverCoroutine());*/
    }

    IEnumerator GameOverCoroutine()
    {
        var wait = new WaitForSecondsRealtime(1.5f);
        SaveData saveData = SaveManager.Instance.saveData;
        int maxTimer = saveData.maxTimer;

        SoundManager.Instance.PlaySound("", SoundType.BGM);
        SoundManager.Instance.PlaySound("button select");
        killText.gameObject.SetActive(true);
        yield return wait;

        SoundManager.Instance.PlaySound("button select");
        recordText.gameObject.SetActive(true);
        recordText.text = "�ְ� ��� : " + ((int)maxTimer / 60).ToString("D2") + " : " + ((int)maxTimer % 60).ToString("D2");
        yield return wait;

        SoundManager.Instance.PlaySound("button select");
        recordText.text += "\n���� ��� : " + ((int)timer / 60).ToString("D2") + " : " + ((int)timer % 60).ToString("D2");
        if (timer >= maxTimer)
        {
            recordText.text = "<#ffee00>�ְ� ��� : " + ((int)timer / 60).ToString("D2") + " : " + ((int)timer % 60).ToString("D2");
            recordText.text += "\n<#ffffff>���� ��� : " + ((int)timer / 60).ToString("D2") + " : " + ((int)timer % 60).ToString("D2");
            recordText.text += "\n<#ffee00>NEW!";
        }

        yield return wait;
        SoundManager.Instance.PlaySound("item get");
        foreach (Image image in gameOverButtons)
            image.gameObject.SetActive(true);

        SaveManager.Instance.saveData.timer = (int)timer;
        SaveManager.Instance.saveData.maxTimer = Mathf.Max((int)timer, SaveManager.Instance.saveData.maxTimer);

        gameOverActivating = true;
        UpdateGameOverSlot();
    }

    private void UpdateGameOver()
    {
        if (gameOverActivating)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                MoveLeftGameOverSlot();
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                MoveRightGameOverSlot();
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
                SelectGameOverSlot();
        }
    }

    private void MoveLeftGameOverSlot()
    {
        gameOverSlotIdx--;
        if (gameOverSlotIdx < 0)
            gameOverSlotIdx += gameOverButtons.Length;
        UpdateGameOverSlot();
    }

    private void MoveRightGameOverSlot()
    {
        gameOverSlotIdx++;
        if (gameOverSlotIdx >= gameOverButtons.Length)
            gameOverSlotIdx -= gameOverButtons.Length;
        UpdateGameOverSlot();
    }

    void GameOverTitle()
    {
        SceneManager.LoadScene("Title");
    }

    void GameOverRestart()
    {
        SceneManager.LoadScene("InGame");
        SoundManager.Instance.PlaySound("bgm", SoundType.BGM);
        GameManager.Instance.OnReset();
    }

    public void UpdateGameOverSlot()
    {
        for (int i = 0; i < gameOverButtons.Length; i++)
            if (i == gameOverSlotIdx)
            {
                gameOverButtons[i].sprite = selectGameOverButton;
                gameOverButtons[i].rectTransform.DOAnchorPosY(selectGameOverMovePosY, 0.3f).SetUpdate(true);
            }
            else
            {
                gameOverButtons[i].sprite = deselectGameOverButton;
                gameOverButtons[i].rectTransform.DOAnchorPosY(deselctGameOverMovePosY, 0.3f).SetUpdate(true);
            }
    }

    void SelectGameOverSlot()
    {
        SoundManager.Instance.PlaySound("button select", SoundType.SE);

        if (Player.Instance != null)
            Destroy(Player.Instance.gameObject);
        DOTween.KillAll();
        PoolManager.Instance.GameEnd();
        SingletonCanvas.Instance.gameObject.SetActive(false);
        gameOverWindow.gameObject.SetActive(false);
        gameOverActivating = false;

        Time.timeScale = 1;

        GameOverButton gameOverButton = (GameOverButton)gameOverSlotIdx;
        switch (gameOverButton)
        {
            case GameOverButton.Restart:
                GameOverRestart();
                break;
            case GameOverButton.Title:
                GameOverTitle();
                break;
        }
    }

    #endregion

    #region ���� �κ��丮 �Լ�

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

    #region ���� ȹ�� �Լ�

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

    #region Ÿ�̸� �Լ�

    private void UpdateTimer()
    {
        timer += Time.deltaTime;
        int timerInt = (int)timer;
        timerText.text = "���� �ð� " + (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
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