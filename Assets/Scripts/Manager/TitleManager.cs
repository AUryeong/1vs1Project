using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public Image image;
    private void Awake()
    {
        image.gameObject.SetActive(false);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        image.gameObject.SetActive(true);
        while (image.color.a > 0)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime);
            yield return null;
        }
        image.gameObject.SetActive(false);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("InGame");
    }

    public void GameEnd()
    {
        Application.Quit();
    }
}
