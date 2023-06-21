using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TitleManager : MonoBehaviour
{

    public void GameStart()
    {
        SceneManager.LoadScene("InGame");
    }

    public void GameEnd()
    {
        Application.Quit();
    }
}
