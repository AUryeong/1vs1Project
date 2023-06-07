using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInven : MonoBehaviour
{
    protected Item _item;
    [SerializeField]protected Image image;
    [SerializeField]
    protected TextMeshProUGUI lvText;

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
            image.sprite = _item.icon;
            lvText.text = "LV: " + (_item.upgrade - 1);
        }
    }
}
