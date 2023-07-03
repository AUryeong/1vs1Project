using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Image[] backgrounds;
    [SerializeField] private Image[] roads;
    [SerializeField] private Image car;

    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    private Vector2 vector;
    public Image image;

    private void Start()
    {
        vector = car.rectTransform.anchoredPosition;
        image.gameObject.SetActive(false);
        StartCoroutine(Intro());
    }

    IEnumerator FadeIn()
    {
        image.gameObject.SetActive(true);
        while (image.color.a < 1)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime);
            yield return null;
        }
        SceneManager.LoadScene("Title");
    }

    IEnumerator Text(string text)
    {
        textMeshProUGUI.text = "";
        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 1);
        for (int i = 0; i < text.Length; i++)
        {
            textMeshProUGUI.text = text.Substring(0, i+1);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);
        while (textMeshProUGUI.color.a > 0)
        {
            textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b,
                textMeshProUGUI.color.a - Time.deltaTime/2);
            yield return null;
        }
        textMeshProUGUI.color = new Color(textMeshProUGUI.color.r, textMeshProUGUI.color.g, textMeshProUGUI.color.b, 0);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator Intro()
    {
        yield return StartCoroutine(Text("평화로웠을 대한민국,"));
        yield return StartCoroutine(Text("어느날 갑자기, 필기구들이 변하기 시작했다."));
        yield return StartCoroutine(Text("지우개와 같은 필기구들이 사람같이 되어, 사람을 공격하기 시작한 것이다."));
        yield return StartCoroutine(Text("이러한 현상을 막고자 모나미에선 사람이 된 볼펜들을 투입한다."));
        yield return StartCoroutine(Text("이건 그 이야기이다."));
        GoTitle();
    }

    public void GoTitle()
    {
        StartCoroutine(FadeIn());
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

        car.rectTransform.anchoredPosition = vector + Random.insideUnitCircle * 2.5f;
    }
}
