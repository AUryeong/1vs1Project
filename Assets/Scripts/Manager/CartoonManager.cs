using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using TMPro;

public class CartoonManager : Singleton<CartoonManager>
{
    [SerializeField] protected RectTransform cutSceneParent;

    private readonly List<CutScene> cutScenes = new List<CutScene>();
    private CutScene cutScene;

    private Cartoon cartoon;
    private int cartoonIdx = -1;

    private Action endEvent;

    private float waitTime;

    public CartoonManager()
    {
        cartoon = null;
        cutScene = null;
    }

    protected override void OnCreated()
    {
        Cursor.visible = false;

        foreach (RectTransform rectTransform in cutSceneParent)
            cutScenes.Add(rectTransform.GetComponent<CutScene>());

        foreach (var scene in cutScenes)
        {
            for (int i = 0; i < scene.cartoons.Count; i++)
            {
                Transform rectTransform = scene.RectTransform.GetChild(i);
                var image = rectTransform.GetComponent<Image>();
                if (image == null)
                    scene.cartoons[i].text = rectTransform.GetComponent<TextMeshProUGUI>();
                else
                {
                    image.gameObject.SetActive(false);
                    scene.cartoons[i].image = image;
                    if (rectTransform.childCount <= 0) continue;

                    var textMeshProUGUI = rectTransform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    if (textMeshProUGUI == null) continue;

                    scene.cartoons[i].text = textMeshProUGUI;
                }
            }
        }
        
        cutSceneParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (cutScene == null)
            return;

        if (cartoon == null)
            NextCartoon();

        WaitTime();
    }

    private void WaitTime()
    {
        waitTime -= Time.unscaledDeltaTime;
        if (waitTime <= 0 || Input.GetMouseButtonDown(0))
        {
            if (cartoon.image != null)
            {
                cartoon.image.DOComplete(true);
                cartoon.image.rectTransform.DOComplete(true);
            }

            if (cartoon.text != null)
            {
                cartoon.text.DOComplete(true);
                cartoon.text.rectTransform.DOComplete(true);
            }

            NextCartoon();
        }
    }

    private void NextCartoon()
    {
        cartoonIdx++;
        if (cartoonIdx >= cutScene.cartoons.Count)
        {
            cutScene = null;
            cartoonIdx = -1;
            foreach (var cut in cutScenes)
                cut.gameObject.SetActive(false);

            endEvent?.Invoke();
            return;
        }

        cartoon = cutScene.cartoons[cartoonIdx];
        if (cartoon.image != null)
        {
            cartoon.image.gameObject.SetActive(true);
        }
        else
        {
            cartoon.text.gameObject.SetActive(true);
        }

        float topDuration = 0;
        float addDuration = 0;

        RectTransform rect;
        rect = cartoon.image == null ? cartoon.text.rectTransform : cartoon.image.rectTransform;

        foreach (CartoonEffect effect in cartoon.effects)
        {
            switch (effect.type)
            {
                case EffectType.Up_Scale:
                    rect.localScale = Vector2.zero;
                    rect.DOScale(effect.power, effect.duration).SetUpdate(true);
                    break;
                case EffectType.One_Scale:
                    rect.localScale = Vector2.one * effect.power;
                    rect.DOScale(1, effect.duration).SetUpdate(true);
                    break;
                case EffectType.One_Scale_Bounding:
                    rect.localScale = Vector2.one * effect.power;
                    rect.DOScale(1, effect.duration).SetEase(Ease.OutBack).SetUpdate(true);
                    break;
                case EffectType.Fade_In:
                    if (cartoon.image != null)
                    {
                        cartoon.image.color = new Color(cartoon.image.color.r, cartoon.image.color.g, cartoon.image.color.b, effect.power);
                        cartoon.image.DOFade(1, effect.duration).SetUpdate(true);
                    }

                    if (cartoon.text != null)
                    {
                        cartoon.text.color = new Color(cartoon.text.color.r, cartoon.text.color.g, cartoon.text.color.b, effect.power);
                        cartoon.text.DOFade(1, effect.duration).SetUpdate(true);
                    }

                    break;
                case EffectType.Rotate:
                    rect.rotation = Quaternion.identity;
                    rect.DORotate(new Vector3(0, 0, effect.power * 360), effect.duration, RotateMode.FastBeyond360).SetRelative(true).SetUpdate(true);
                    break;
                case EffectType.Move_Left:
                    Vector2 origin = rect.anchoredPosition;
                    rect.anchoredPosition += new Vector2(rect.sizeDelta.x * effect.power, 0);
                    rect.DOAnchorPos(origin, effect.duration).SetUpdate(true);
                    break;
                case EffectType.Move_Down:
                    Vector2 originPos = rect.anchoredPosition;
                    rect.anchoredPosition += new Vector2(0, -rect.sizeDelta.x * effect.power);
                    rect.DOAnchorPos(originPos, effect.duration).SetUpdate(true);
                    break;
                case EffectType.Shake_Pos:
                    rect.DOShakePosition(effect.duration, effect.power).SetUpdate(true);
                    break;
                case EffectType.Wait:
                    addDuration += effect.duration;
                    break;
            }

            if (effect.type != EffectType.Wait && topDuration < effect.duration)
                topDuration = effect.duration;
        }

        waitTime = topDuration + addDuration;
    }


    public void CartoonPlay(int index, Action cartoonEvent = null)
    {
        cutSceneParent.gameObject.SetActive(false);
        
        endEvent = cartoonEvent;
        cutScene = cutScenes[index];
        foreach (var cut in cutScenes)
            cut.gameObject.SetActive(cut == cutScene);

        foreach (var viewCartoon in cutScene.cartoons)
            if (viewCartoon.image == null)
                viewCartoon.text.gameObject.SetActive(false);
            else
                viewCartoon.image.gameObject.SetActive(false);

        cartoonIdx = -1;
        NextCartoon();
    }
}