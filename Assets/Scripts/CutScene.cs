using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum EffectType
{
    Up_Scale,
    One_Scale,
    One_Scale_Bounding,
    Fade_In,
    Rotate,
    Wait,
    Move_Left,
    Move_Down,
    Shake_Pos
}
[System.Serializable]
public class CartoonEffect
{
    public EffectType type;
    
    [Range(0f, 3f)]
    public float power;
    
    [Range(0f, 5f)]
    public float duration;

}
[System.Serializable]
public class Cartoon
{
    [HideInInspector]
    public Image image;
    
    [HideInInspector]
    public TextMeshProUGUI text;
    
    public List<CartoonEffect> effects = new List<CartoonEffect>();
}
public class CutScene : MonoBehaviour
{
    private RectTransform rectTransform;
    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            return rectTransform;
        }
    }
    public List<Cartoon> cartoons = new List<Cartoon>();
}