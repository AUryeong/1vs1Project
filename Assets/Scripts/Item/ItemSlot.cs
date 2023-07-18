using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemLore;

    private RectTransform rectTransform;
    private Item item;
    public Item Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
            itemIcon.sprite = item.ItemIcon;
            itemName.text = "[ " + item.Name + " ]";
            itemLore.text = item.Lore;
        }
    }

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOScale(Vector3.one * 1.25f, 0.3f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOScale(Vector3.one, 0.3f).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.EndChooseItem(this);
    }
}
