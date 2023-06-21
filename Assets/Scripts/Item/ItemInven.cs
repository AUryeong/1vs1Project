using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInven : MonoBehaviour
{
    protected Item _item;
    [SerializeField] protected TextMeshProUGUI itemText;

    public RectTransform rectTransform
    {
        get
        {
            return (RectTransform)transform;
        }
    }

    public Item item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;
            itemText.text = _item.GetName();
        }
    }
}
