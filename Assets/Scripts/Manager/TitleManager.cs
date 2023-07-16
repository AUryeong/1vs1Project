using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
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

    [SerializeField] private Image howToPlay;

    private void Awake()
    {
        Cursor.visible = true;
        Time.timeScale = 1;
        PoolManager.Instance.DisableAllObjects();
        
        GameManager.Instance.MainCamera.transform.position = new Vector3(0, 0, -10);

        bgmSlider.value = SaveManager.Instance.SaveData.bgmVolume;
        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener((value)=>
        {
          SoundManager.Instance.VolumeChange(SoundType.BGM, value);  
        });

        sfxSlider.value = SaveManager.Instance.SaveData.sfxVolume;
        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.AddListener((value)=>
        {
            SoundManager.Instance.VolumeChange(SoundType.Se, value);  
        });
        
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Fade);
        SoundManager.Instance.PlaySound("title", SoundType.BGM);
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
            howToPlay.gameObject.SetActive(false);

            TransitionManager.Instance.TransitionFadeOut();
        });
    }

    public void HowToPlay()
    {
        TransitionManager.Instance.TransitionFadeIn(TransitionType.Square, () =>
        {
            title.gameObject.SetActive(false);
            howToPlay.gameObject.SetActive(true);

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