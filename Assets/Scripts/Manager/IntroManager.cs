using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Image car;

    [BoxGroup("Scroll")]
    [SerializeField] private Image[] backgrounds;
    [BoxGroup("Scroll")]
    [SerializeField] private Image[] roads;
    
    [Space(15)]
    [SerializeField] private TextMeshProUGUI dialogText;

    [SerializeField] private Button skipButton;

    private Vector2 carPos;
    private bool isLoading = false;

    private void Awake()
    {
        carPos = car.rectTransform.anchoredPosition;
        
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(()=>
        {
            SoundManager.Instance.PlaySound("button", SoundType.Se, 2f);
            GoToTitle();
        });
    }

    private void Start()
    {
        StartCoroutine(Intro());
    }

    private IEnumerator Text(string text)
    {
        dialogText.text = "";
        dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 1);
        
        var wait = new WaitForSeconds(0.05f);
        for (int i = 0; i < text.Length; i++)
        {
            dialogText.text = text.Substring(0, i+1);
            yield return wait;
        }
        yield return wait;
        while (dialogText.color.a > 0)
        {
            dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b,
                dialogText.color.a - Time.deltaTime);
            yield return null;
        }
        dialogText.color = new Color(dialogText.color.r, dialogText.color.g, dialogText.color.b, 0);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator Intro()
    {
        yield return StartCoroutine(Text("평화로웠을 대한민국,"));
        yield return StartCoroutine(Text("어느날 갑자기, 필기구들이 변하기 시작했다."));
        yield return StartCoroutine(Text("지우개와 같은 필기구들이 사람급의 크기가 되어,"));
        yield return StartCoroutine(Text("사람을 공격하기 시작한 것이다."));
        yield return StartCoroutine(Text("이러한 현상을 막고자 모나미에선 사람이 된 볼펜들을 투입한다."));
        yield return StartCoroutine(Text("이건 그 이야기이다."));
        GoToTitle();
    }

    private void GoToTitle()
    {
        if (isLoading) return;

        isLoading = true;

        TransitionManager.Instance.TransitionFadeOut(TransitionType.Fade, () =>
        {
            TransitionManager.Instance.LoadScene(SceneType.Title);
            TransitionManager.Instance.TransitionFadeIn(TransitionType.Fade);
        });
    }

    private void Update()
    {
        BackgroundMoving();
    }

    private void BackgroundMoving()
    {
        foreach (var background in backgrounds)
        {
            background.rectTransform.anchoredPosition += new Vector2(Time.deltaTime * 300,0);
            if (background.rectTransform.anchoredPosition.x >= 1920)
            {
                background.rectTransform.anchoredPosition -= new Vector2(3840, 0);
            }
        }
        foreach (var road in roads)
        {
            road.rectTransform.anchoredPosition += new Vector2(Time.deltaTime * 1100,0);
            if (road.rectTransform.anchoredPosition.x >= 1920)
            {
                road.rectTransform.anchoredPosition -= new Vector2(3840, 0);
            }
        }

        car.rectTransform.anchoredPosition = carPos + Random.insideUnitCircle * 2.5f;
    }
}
