using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterType characterType;
    protected override bool IsDontDestroying => true;
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            return mainCamera;
        }
    }
    private Camera mainCamera;

    protected override void OnCreated()
    {
        base.OnReset();
    }
    
    protected override void OnReset()
    {
        base.OnReset();
        SetResolution(MainCamera);
    }


    private void SetResolution(Camera changeCamera)
    {
        if (changeCamera == null) return;
        
        int setWidth = 1920;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

        float screenMultiplier = (float)setWidth / setHeight;
        float deviceMultiplier = (float)deviceWidth / deviceHeight;

        if (screenMultiplier < deviceMultiplier)
        {
            float newWidth = screenMultiplier / deviceMultiplier;
            changeCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceMultiplier / screenMultiplier;
            changeCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }

}