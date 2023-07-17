using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TransitionType
{
    Fade,
    Square
}

public enum SceneType
{
    Intro,
    Title,
    InGame
}

public class TransitionManager : Singleton<TransitionManager>
{
    protected override bool IsDontDestroying => true;

    [Header("화면 연출")] private bool isSceneFadeInTransitioning = false;
    private bool isSceneFadeOutTransitioning = false;

    public SpriteRenderer background;
    public MeshRenderer transitionSquare;

    public Image blackBackground;

    private Coroutine fadeOutCoroutine;
    private Coroutine fadeInCoroutine;

    protected override void OnCreated()
    {
        background.gameObject.SetActive(false);
        transitionSquare.gameObject.SetActive(false);

        isSceneFadeInTransitioning = false;
        isSceneFadeOutTransitioning = false;
    }

    protected override void OnReset()
    {
        isSceneFadeInTransitioning = false;
        isSceneFadeOutTransitioning = false;
    }

    public void LoadScene(SceneType sceneType)
    {
        SceneManager.LoadScene((int)sceneType);
    }

    private void StopCoroutine()
    {
        if (fadeInCoroutine != null)
            StopCoroutine(fadeInCoroutine);

        if (fadeOutCoroutine != null)
            StopCoroutine(fadeOutCoroutine);
    }

    public void TransitionFadeIn(TransitionType type = TransitionType.Square, Action action = null)
    {
        var pos = GameManager.Instance.MainCamera.transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
        if (type == TransitionType.Fade)
        {
            StopCoroutine();
            fadeInCoroutine = StartCoroutine(FadeIn(action));
            return;
        }

        if (isSceneFadeInTransitioning) return;

        isSceneFadeInTransitioning = true;
        transitionSquare.transform.DOKill(true);

        background.gameObject.SetActive(true);
        transitionSquare.gameObject.SetActive(true);

        transitionSquare.transform.localScale = Vector3.one * 30;
        transitionSquare.transform.DOScale(Vector3.zero, 1).SetEase(Ease.InQuad).OnComplete(() =>
        {
            isSceneFadeInTransitioning = false;

            action?.Invoke();
        });
    }

    public void TransitionFadeOut(TransitionType type = TransitionType.Square, Action action = null)
    {
        var pos = GameManager.Instance.MainCamera.transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
        if (type == TransitionType.Fade)
        {
            StopCoroutine();
            fadeOutCoroutine = StartCoroutine(FadeOut(action));
            return;
        }

        if (isSceneFadeOutTransitioning) return;

        isSceneFadeOutTransitioning = true;
        transitionSquare.transform.DOKill(true);

        background.gameObject.SetActive(true);
        transitionSquare.gameObject.SetActive(true);

        transitionSquare.transform.localScale = Vector3.zero;
        transitionSquare.transform.DOScale(Vector3.one * 30, 1).SetEase(Ease.OutQuad).SetDelay(0.5f).OnComplete(() =>
        {
            isSceneFadeOutTransitioning = false;

            background.gameObject.SetActive(false);
            transitionSquare.gameObject.SetActive(false);

            action?.Invoke();
        });
    }

    private IEnumerator FadeIn(Action action = null)
    {
        blackBackground.gameObject.SetActive(true);
        blackBackground.color = blackBackground.color.FadeChange(1);
        while (blackBackground.color.a > 0)
        {
            blackBackground.color = blackBackground.color.FadeChange(blackBackground.color.a - Time.unscaledDeltaTime / 2);
            yield return null;
        }

        blackBackground.gameObject.SetActive(false);

        action?.Invoke();
    }

    private IEnumerator FadeOut(Action action = null)
    {
        blackBackground.gameObject.SetActive(true);
        blackBackground.color = blackBackground.color.FadeChange(0);
        while (blackBackground.color.a < 1)
        {
            blackBackground.color = blackBackground.color.FadeChange(blackBackground.color.a + Time.unscaledDeltaTime / 2);
            yield return null;
        }

        action?.Invoke();
    }
}