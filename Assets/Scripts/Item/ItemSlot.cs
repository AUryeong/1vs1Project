using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemLore;
    public RectTransform RectTransform
    {
        get;
        private set;
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
