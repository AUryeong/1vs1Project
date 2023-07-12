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

    [Header("화면 연출")] 
    private bool isSceneFadeInTransitioning = false;
    private bool isSceneFadeOutTransitioning = false;

    public SpriteRenderer background;
    public MeshRenderer transitionSquare;

    public Image blackBackground;

    protected override void OnCreated()
    {
        background.gameObject.SetActive(false);
        transitionSquare.gameObject.SetActive(false);
        
        isSceneFadeInTransitioning = false;
        isSceneFadeOutTransitioning = false;
    }

    public void LoadScene(SceneType sceneType)
    {
        SceneManager.LoadScene((int)sceneType);
    }

    public void TransitionFadeIn(TransitionType type = TransitionType.Square, Action action = null)
    {
        if (type == TransitionType.Fade)
        {
            StartCoroutine(FadeIn(action));
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
        if (type == TransitionType.Fade)
        {
            StartCoroutine(FadeOut(action));
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

    IEnumerator FadeIn(Action action = null)
    {
        blackBackground.gameObject.SetActive(true);
        blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b, 1);
        while (blackBackground.color.a > 0)
        {
            blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b,
                blackBackground.color.a - Time.unscaledDeltaTime / 2);
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
            blackBackground.color = new Color(blackBackground.color.r, blackBackground.color.g, blackBackground.color.b,
                blackBackground.color.a + Time.unscaledDeltaTime / 2);
            yield return null;
        }

        action?.Invoke();
    }
}