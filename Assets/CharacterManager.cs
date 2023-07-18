using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterManager : Singleton<CharacterManager>
{
    [SerializeField] private Image characterImage;
    [SerializeField] private RectTransform scrollRect;

    [SerializeField] private Image itemViewBackground;

    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button returnButton;

    [SerializeField] private List<CharacterItemSlot> slots = new List<CharacterItemSlot>();

    private bool isScrolling = false;
    private CharacterType characterType = CharacterType.Bp153;

    [Header("캐릭별 달라지는거")]
    [SerializeField] private Dictionary<CharacterType, Sprite> characterLdSprites = new Dictionary<CharacterType, Sprite>();
    [SerializeField] private Dictionary<CharacterType, Sprite> characterSlotImages = new Dictionary<CharacterType, Sprite>();
    [SerializeField] private Dictionary<CharacterType, string[]> characterItems = new Dictionary<CharacterType, string[]>();

    protected override void OnCreated()
    {
        base.OnCreated();
        TransitionManager.Instance.TransitionFadeOut();

        scrollRect.anchoredPosition = Vector2.zero;

        itemViewBackground.color = itemViewBackground.color.FadeChange(0);

        gameStartButton.onClick.RemoveAllListeners();
        gameStartButton.onClick.AddListener(GameStart);

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnCharacterSelect);
    }

    private void GameStart()
    {
        SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);

        GameManager.Instance.characterType = characterType;

        TransitionManager.Instance.TransitionFadeOut(TransitionType.Fade, () =>
        {
            TransitionManager.Instance.LoadScene(SceneType.InGame);
        });
    }

    private void ReturnCharacterSelect()
    {
        if (!isScrolling) return;

        SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);

        isScrolling = false;

        scrollRect.DOKill();
        scrollRect.DOAnchorPosX(0, 1);

        itemViewBackground.DOKill();
        itemViewBackground.DOFade(0, 1).SetEase(Ease.Linear);
    }

    public void CharacterSetting(CharacterType type)
    {
        if (isScrolling) return;

        SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);

        characterType = type;
        isScrolling = true;

        gameStartButton.image.rectTransform.anchoredPosition = new Vector2(gameStartButton.image.rectTransform.anchoredPosition.x, -172);
        returnButton.image.rectTransform.anchoredPosition = new Vector2(returnButton.image.rectTransform.anchoredPosition.x, -172);

        itemViewBackground.DOKill();
        itemViewBackground.DOFade(1, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameStartButton.image.rectTransform.DOAnchorPosY(127.4f, 0.6f);

            returnButton.image.rectTransform.DOAnchorPosY(127.4f, 0.6f);
        });

        scrollRect.DOKill();
        scrollRect.DOAnchorPosX(-2200, 1);

        var items = characterItems[type];
        var slotImage = characterSlotImages[type];
        for (int i = 0; i < slots.Count; i++)
        {
            CharacterItemSlot slot = slots[i];
            slot.SlotImage.sprite = slotImage;

            slot.RectTransform.anchoredPosition = new Vector2(slot.RectTransform.anchoredPosition.x, 810);
            slot.RectTransform.DOAnchorPosY(-50, 1f).SetDelay(i * 0.25f).SetEase(Ease.OutBack);
            
            slot.Item = ResourcesManager.Instance.GetItem(items[i]);
        }
        characterImage.sprite = characterLdSprites[type];
        characterImage.SetNativeSize();
    }
}
