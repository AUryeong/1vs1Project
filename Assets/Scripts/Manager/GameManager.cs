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
}