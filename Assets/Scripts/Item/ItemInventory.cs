using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    
    public void Init(Item item)
    {
        itemIcon.sprite = item.ItemIcon;
        itemNameText.text = item.GetName();
    }
}