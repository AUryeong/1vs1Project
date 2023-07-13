using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class ItemSlot : MonoBehaviour
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
    public Button Button
    {
        get;
        private set;
    }
    private readonly float selectMovePosX = -100;
    private readonly float deselectMovePosX = 0;

    protected void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Button = GetComponent<Button>();
    }

    public void SlotSelect()
    {
        RectTransform.DOAnchorPosX(selectMovePosX, 0.3f).SetUpdate(true);
    }

    public void SlotDeselect()
    {
        RectTransform.DOAnchorPosX(deselectMovePosX, 0.3f).SetUpdate(true);
    }
}
