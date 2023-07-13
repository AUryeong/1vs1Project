using UnityEngine;
using TMPro;

public class ItemInven : MonoBehaviour
{
    private Item item;
    [SerializeField] protected TextMeshProUGUI itemText;

    public RectTransform RectTransform => (RectTransform)transform;

    public Item Item
    {
        get => item;
        set
        {
            item = value;
            itemText.text = item.GetName();
        }
    }
}
