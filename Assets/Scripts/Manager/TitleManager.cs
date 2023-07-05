using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TitleManager : MonoBehaviour
{
    [SerializeField] protected Image black;

    [Header("화면 연출")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private MeshRenderer transitionSquare;
    private bool isSceneTransitioning = false; 

    [Space(10f)] 
    [SerializeField] protected Image title;
    
    [Header("크레딧")]
    [SerializeField] protected Image credit;
    [SerializeField] private Image producerTop;
    [SerializeField] private Image producerBottom;
    
    [Header("설정")]
    [SerializeField] protected Image setting;
    [SerializeField] private Image bgmWhiteBox;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Image sfxWhiteBox;
    [SerializeField] private Slider sfxSlider;

    private void Awake()
    {
        black.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        transitionSquare.gameObject.SetActive(false);

        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.value = SaveManager.Instance.saveData.bgmVolume;
        bgmSlider.onValueChanged.AddListener((value)=>
        {
          SoundManager.Instance.VolumeChange(SoundType.BGM, value);  
        });

        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.value = SaveManager.Instance.saveData.sfxVolume;
        sfxSlider.onValueChanged.AddListener((value)=>
        {
            SoundManager.Instance.VolumeChange(SoundType.SE, value);  
        });

        isSceneTransitioning = false;
        
        StartCoroutine(FadeOut());
    }

    private void TransitionFadeIn(Action action = null)
    {
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

    private void TransitionFadeOut(Action action = null)
    {
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

    IEnumerator FadeOut()
    {
        black.gameObject.SetActive(true);
        while (black.color.a > 0)
        {
            black.color = new Color(black.color.r, black.color.g, black.color.b, black.color.a - Time.deltaTime);
            yield return null;
        }

        black.gameObject.SetActive(false);
    }

    public void GameStart()
    {
        TransitionFadeIn(() => SceneManager.LoadScene("InGame"));
    }

    public void Back()
    {
        TransitionFadeIn(() =>
        {
            title.gameObject.SetActive(true);
            credit.gameObject.SetActive(false);
            setting.gameObject.SetActive(false);
            TransitionFadeOut();
        });
    }

    public void Credit()
    {
        TransitionFadeIn(() =>
        {
            title.gameObject.SetActive(false);
            credit.gameObject.SetActive(true);

            producerTop.DOKill();
            producerTop.rectTransform.anchoredPosition = new Vector2(0, -109f);
            producerTop.rectTransform.DOAnchorPosY(100,  1).SetDelay(1);

            producerBottom.DOKill();
            producerBottom.rectTransform.anchoredPosition = new Vector2(0, 109f);
            producerBottom.rectTransform.DOAnchorPosY(-100, 1).SetDelay(1);
            
            TransitionFadeOut();
        });
    }

    public void Setting()
    {
        TransitionFadeIn(() =>
        {
            title.gameObject.SetActive(false);
            setting.gameObject.SetActive(true);

            bgmWhiteBox.DOKill();
            bgmWhiteBox.rectTransform.anchoredPosition = new Vector2(-925f, bgmWhiteBox.rectTransform.anchoredPosition.y);
            bgmWhiteBox.rectTransform.DOAnchorPosX(560, 1).SetDelay(1).SetEase(Ease.OutBack);
            
            sfxWhiteBox.DOKill();
            sfxWhiteBox.rectTransform.anchoredPosition = new Vector2(-925f, sfxWhiteBox.rectTransform.anchoredPosition.y);
            sfxWhiteBox.rectTransform.DOAnchorPosX(560, 1).SetDelay(1.5f).SetEase(Ease.OutBack);
            
            TransitionFadeOut();
        });
    }

    public void GameEnd()
    {
        TransitionFadeIn(Application.Quit);
    }
}