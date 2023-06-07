using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemLore;
    Image slotImage;
    public RectTransform rectTransform
    {
        get;
        private set;
    }
    private Item _item;
    public Item item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
            itemIcon.sprite = _item.icon;
            itemName.text = "[ " + _item.GetName() + " ]";
            itemLore.text = _item.GetLore();
        }
    }
    private float selectMovePosX = -100;
    private float deselectMovePosX = 0;

    protected void Awake()
    {
        slotImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SlotSelect()
    {
        slotImage.sprite = UIManager.Instance.selectSlotSprite;
        rectTransform.DOAnchorPosX(selectMovePosX, 0.3f).SetUpdate(true);
    }

    public void SlotDeselect()
    {
        slotImage.sprite = UIManager.Instance.deselectSlotSprite;
        rectTransform.DOAnchorPosX(deselectMovePosX, 0.3f).SetUpdate(true);
    }
}
