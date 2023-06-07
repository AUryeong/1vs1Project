using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public enum TitleButton
{
    Start,
    Option,
    Help,
    Developer,
    Exit
}

public class TitleManager : MonoBehaviour
{
    [SerializeField] Sprite buttonDeselectSprite;
    [SerializeField] Sprite buttonSelectSprite;
    [SerializeField] Image[] buttons;

    [SerializeField] VerticalLayoutGroup recordLayout;
    [SerializeField] TextMeshProUGUI recordText;
    [SerializeField] TextMeshProUGUI maxRecordText;

    [SerializeField] VerticalLayoutGroup soundLayout;
    [SerializeField] TextMeshProUGUI soundText;

    [SerializeField] TextMeshProUGUI developerText;
    [SerializeField] TextMeshProUGUI helpText;

    private int slotIdx = 0;

    private float buttonHighlightMovePosX = -200;
    private float buttonSelectMovePosX = -100;
    private float buttonDeselectMovePosX = 0;
    private bool isMovable = true;
    private void Start()
    {
        SoundManager.Instance.PlaySound("bgm", SoundType.BGM);
        slotIdx = 0;
        isMovable = true;
        SingletonCanvas2.Instance.gameObject.SetActive(true);
        UpdateSlot();

        SaveData saveData = SaveManager.Instance.saveData;
        recordText.text = (saveData.timer / 60).ToString("D2") + ":" + (saveData.timer % 60).ToString("D2");
        maxRecordText.text = (saveData.maxTimer / 60).ToString("D2") + ":" + (saveData.maxTimer % 60).ToString("D2");
        soundText.text = saveData.soundVolume + "%";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            PressUpKey();
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            PressDownKey();
        else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            PressSelectKey();
    }

    private void PressUpKey()
    {
        if (isMovable)
            MoveUpSlot();
        else if (soundLayout.gameObject.activeSelf)
            SoundUp();
    }
    private void PressDownKey()
    {
        if (isMovable)
            MoveDownSlot();
        else if (soundLayout.gameObject.activeSelf)
            SoundDown();
    }
    private void PressSelectKey()
    {
        SoundManager.Instance.PlaySound("button select", SoundType.SE);
        if (isMovable)
            SelectSlot();
        else if (soundLayout.gameObject.activeSelf)
            DeSelectOption();
        else if (developerText.gameObject.activeSelf)
            DeSelectDeveloper();
        else if (helpText.gameObject.activeSelf)
            DeSelectHelp();
    }
    private void DeSelectDeveloper()
    {
        buttons[slotIdx].rectTransform.DOAnchorPosX(buttonSelectMovePosX, 0.3f).SetUpdate(true);
        isMovable = true;
        developerText.gameObject.SetActive(false);
        recordLayout.gameObject.SetActive(true);
    }
    private void SelectDeveloper()
    {
        buttons[slotIdx].rectTransform.DOAnchorPosX(buttonHighlightMovePosX, 0.3f).SetUpdate(true);
        isMovable = false;
        developerText.gameObject.SetActive(true);
        recordLayout.gameObject.SetActive(false);
    }

    private void DeSelectHelp()
    {
        buttons[slotIdx].rectTransform.DOAnchorPosX(buttonSelectMovePosX, 0.3f).SetUpdate(true);
        isMovable = true;
        helpText.gameObject.SetActive(false);
        recordLayout.gameObject.SetActive(true);
    }
    private void SelectHelp()
    {
        buttons[slotIdx].rectTransform.DOAnchorPosX(buttonHighlightMovePosX, 0.3f).SetUpdate(true);
        isMovable = false;
        helpText.gameObject.SetActive(true);
        recordLayout.gameObject.SetActive(false);
    }
    private void SelectOption()
    {
        buttons[slotIdx].rectTransform.DOAnchorPosX(buttonHighlightMovePosX, 0.3f).SetUpdate(true);
        isMovable = false;
        soundLayout.gameObject.SetActive(true);
        recordLayout.gameObject.SetActive(false);
    }
    private void DeSelectOption()
    {
        buttons[slotIdx].rectTransform.DOAnchorPosX(buttonSelectMovePosX, 0.3f).SetUpdate(true);
        isMovable = true;
        soundLayout.gameObject.SetActive(false);
        recordLayout.gameObject.SetActive(true);
    }

    private void SoundUp()
    {
        SaveData saveData = SaveManager.Instance.saveData;

        saveData.soundVolume = Mathf.Min(100, saveData.soundVolume + 5);
        soundText.text = saveData.soundVolume + "%";
        SoundManager.Instance.VolumeChange(saveData.soundVolume / 100f);
        SoundManager.Instance.PlaySound("hurt");
    }

    private void SoundDown()
    {
        SaveData saveData = SaveManager.Instance.saveData;

        saveData.soundVolume = Mathf.Max(0, saveData.soundVolume - 5);
        soundText.text = saveData.soundVolume + "%";
        SoundManager.Instance.VolumeChange(saveData.soundVolume / 100f);
        SoundManager.Instance.PlaySound("hurt");
    }
    private void MoveUpSlot()
    {
        slotIdx--;
        if (slotIdx < 0)
            slotIdx += buttons.Length;
        UpdateSlot();
    }
    private void MoveDownSlot()
    {
        slotIdx++;
        if (slotIdx >= buttons.Length)
            slotIdx -= buttons.Length;
        UpdateSlot();
    }

    private void SelectSlot()
    {
        TitleButton button = (TitleButton)slotIdx;
        switch (button)
        {
            case TitleButton.Start:
                Destroy(SingletonCanvas2.Instance.gameObject);
                DOTween.KillAll();
                SceneManager.LoadScene("InGame");
                break;
            case TitleButton.Option:
                SelectOption();
                break;
            case TitleButton.Help:
                SelectHelp();
                break;
            case TitleButton.Developer:
                SelectDeveloper();
                break;
            case TitleButton.Exit:
                Application.Quit();
                break;
        }
    }

    private void UpdateSlot()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image image = buttons[i];
            if (i == slotIdx)
            {
                image.sprite = buttonSelectSprite;
                image.rectTransform.DOAnchorPosX(buttonSelectMovePosX, 0.3f).SetUpdate(true);
            }
            else
            {
                image.sprite = buttonDeselectSprite;
                image.rectTransform.DOAnchorPosX(buttonDeselectMovePosX, 0.3f).SetUpdate(true);
            }
        }
    }
}
