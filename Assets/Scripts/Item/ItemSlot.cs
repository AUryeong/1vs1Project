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

    private RectTransform RectTransform
    {
        get;
        set;
    }
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
            itemName.text = "[ " + item.GetName() + " ]";
            itemLore.text = item.GetLore();
        }
    }

    protected void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RectTransform.DOScale(Vector3.one * 1.2f, 0.5f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RectTransform.DOScale(Vector3.one, 0.5f).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.Instance.EndChooseItem(this);
    }
}
