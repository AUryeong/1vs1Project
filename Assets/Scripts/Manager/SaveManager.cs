using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float sfxVolume = 1f;
    public float bgmVolume = 1f;
}

public class SaveManager : Singleton<SaveManager>
{
    protected override bool IsDontDestroying => true;
    private SaveData saveData;
    public SaveData SaveData
    {
        get
        {
            if (saveData == null)
                LoadGameData();
            return saveData;
        }
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
    protected override void OnCreated()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        string s = PlayerPrefs.GetString("SaveData", "none");
        saveData = s == "none" ? new SaveData() : JsonUtility.FromJson<SaveData>(s);
    }

    private void SaveGameData()
    {
        PlayerPrefs.SetString("SaveData", JsonUtility.ToJson(SaveData));
    }
}
