using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image character;
    [SerializeField] private CharacterType characterType;

    private readonly Color enterColor = new Color(117 / 255f, 115 / 255f, 140 / 255f);
    private const float enterScale = 0.8f;

    private void Awake()
    {
        character.color = enterColor;
        character.rectTransform.localScale = Vector3.one * enterScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CharacterManager.Instance.CharacterSetting(characterType);
        SoundManager.Instance.PlaySound("button", SoundType.Se, 2);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        character.DOKill();
        character.rectTransform.DOKill();

        character.DOColor(Color.white, 0.2f);
        character.rectTransform.DOScale(1, 0.2f);
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        character.DOKill();
        character.rectTransform.DOKill();

        character.DOColor(enterColor, 0.4f);
        character.rectTransform.DOScale(enterScale, 0.4f);
    }
}
