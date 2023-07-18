using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterItemSlot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemLore;

    public Image SlotImage
    {
        get;
        private set;
    }
    public RectTransform RectTransform
    {
        get;
        private set;
    }
    public Item Item
    {
        set
        {
            itemIcon.sprite = value.ItemIcon;
            itemName.text = "[ " + value.Name + " ]";
            itemLore.text = value.Lore;
        }
    }
    private void Awake()
    {
        SlotImage = GetComponent<Image>();
        RectTransform = transform as RectTransform;
    }
}
