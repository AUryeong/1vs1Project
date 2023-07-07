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
        Camera.main.transform.position = new Vector3(0, 0, -10);
        SingletonCanvas.Instance.gameObject.SetActive(false);

        black.gameObject.SetActive(false);
        TransitionManager.Instance.blackBackground.gameObject.SetActive(false);

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

        
        StartCoroutine(FadeOut());
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
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Square, () => 
        {
            SceneManager.LoadScene("InGame");
        });
    }

    public void Back()
    {
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Square, () =>
        {
            title.gameObject.SetActive(true);
            credit.gameObject.SetActive(false);
            setting.gameObject.SetActive(false);

            TransitionManager.Instance.TransitionFadeOut();
        });
    }

    public void Credit()
    {
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Square, () =>
        {
            title.gameObject.SetActive(false);
            credit.gameObject.SetActive(true);

            producerTop.DOKill();
            producerTop.rectTransform.anchoredPosition = new Vector2(0, -109f);
            producerTop.rectTransform.DOAnchorPosY(100,  1).SetDelay(1);

            producerBottom.DOKill();
            producerBottom.rectTransform.anchoredPosition = new Vector2(0, 109f);
            producerBottom.rectTransform.DOAnchorPosY(-100, 1).SetDelay(1);

            TransitionManager.Instance.TransitionFadeOut();
        });
    }

    public void Setting()
    {
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Square, () =>
        {
            title.gameObject.SetActive(false);
            setting.gameObject.SetActive(true);

            bgmWhiteBox.DOKill();
            bgmWhiteBox.rectTransform.anchoredPosition = new Vector2(-925f, bgmWhiteBox.rectTransform.anchoredPosition.y);
            bgmWhiteBox.rectTransform.DOAnchorPosX(560, 1).SetDelay(1).SetEase(Ease.OutBack);
            
            sfxWhiteBox.DOKill();
            sfxWhiteBox.rectTransform.anchoredPosition = new Vector2(-925f, sfxWhiteBox.rectTransform.anchoredPosition.y);
            sfxWhiteBox.rectTransform.DOAnchorPosX(560, 1).SetDelay(1.5f).SetEase(Ease.OutBack);

            TransitionManager.Instance.TransitionFadeOut();
        });
    }

    public void GameEnd()
    {
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Square, Application.Quit);
    }
}