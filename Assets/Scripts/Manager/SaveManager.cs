using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class SaveData
{
    public int timer = 0;
    public int maxTimer = 0;
    public int soundVolume = 100;
}

public class SaveManager : Singleton<SaveManager>
{
    SaveData _saveData;
    public SaveData saveData
    {
        get
        {
            if (_saveData == null)
                LoadGameData();
            return _saveData;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
    public override void OnReset()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        string s = PlayerPrefs.GetString("SaveData", "none");
        if (s == "none")
            _saveData = new SaveData();
        else
            _saveData = JsonUtility.FromJson<SaveData>(s);
    }

    private void SaveGameData()
    {
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(saveData));
    }
}
