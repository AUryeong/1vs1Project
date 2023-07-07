using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TransitionType
{
    Fade,
    Square
}

public class TransitionManager : Singleton<TransitionManager>
{
    [Header("화면 연출")]
    private bool isSceneTransitioning = false;

    public SpriteRenderer background;
    public MeshRenderer transitionSquare;

    public Image blackBackground;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            background.gameObject.SetActive(false);
            transitionSquare.gameObject.SetActive(false);
            isSceneTransitioning = false;
        }
    }

    public override void OnReset()
    {
    }

    public void TransitionFadeIn(TransitionType type = TransitionType.Square, Action action = null)
    {
        if (type == TransitionType.Fade)
        {
            StartCoroutine(FadeIn(action));
            return;
        }
        if (isSceneTransitioning) return;

        isSceneTransitioning = true;
        background.gameObject.SetActive(true);
        transitionSquare.gameObject.SetActive(true);
        transitionSquare.transform.localScale = Vector3.one * 30;

        transitionSquare.transform.DOScale(Vector3.zero, 1).SetEase(Ease.InQuad).OnComplete(() =>
        {
            isSceneTransitioning = false;

            action?.Invoke();
        });
    }

    public void TransitionFadeOut(TransitionType type = TransitionType.Square, Action action = null)
    {
        if (type == TransitionType.Fade)
        {
            StartCoroutine(FadeOut(action));
            return;
        }
        if (isSceneTransitioning) return;

        isSceneTransitioning = true;
        background.gameObject.SetActive(true);
        transitionSquare.gameObject.SetActive(true);
        transitionSquare.transform.localScale = Vector3.zero;

        transitionSquare.transform.DOScale(Vector3.one * 30, 1).SetEase(Ease.OutQuad).SetDelay(0.5f).OnComplete(() =>
        {
            isSceneTransitioning = false;

            background.gameObject.SetActive(false);
            transitionSquare.gameObject.SetActive(false);

            action?.Invoke();
        });
    }

    IEnumerator FadeIn(Action action = null)
    {
        blackBackground.gameObject.SetActive(true);
        blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, 1);
        while (blackBackground.color.a > 0)
        {
            blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, blackBackground.color.a - Time.unscaledDeltaTime);
            yield return null;
        }
        blackBackground.gameObject.SetActive(false);
        action?.Invoke();
    }

    IEnumerator FadeOut(Action action = null)
    {
        blackBackground.gameObject.SetActive(true);
        blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, 0);
        while (blackBackground.color.a < 1)
        {
            blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, blackBackground.color.a + Time.unscaledDeltaTime);
            yield return null;
        }
        action?.Invoke();
    }
}
