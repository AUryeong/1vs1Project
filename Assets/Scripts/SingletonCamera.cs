using UnityEngine;

public class SingletonCamera : Singleton<SingletonCamera>
{
    protected override bool IsDontDestroying => true;
    private Camera mainCamera;

    protected override void OnCreated()
    {
        OnReset();
        mainCamera = GetComponent<Camera>();
    }

    protected override void OnReset()
    {
        gameObject.SetActive(true);
        foreach (var canvas in FindObjectsOfType<Canvas>())
            canvas.worldCamera = mainCamera;
    }
    
    private void OnPreCull()
    {
        Rect rect = mainCamera.rect;
        Rect newRect = new Rect(0, 0, 1, 1);
        mainCamera.rect = newRect;
        GL.Clear(true, true, Color.black);
        mainCamera.rect = rect;
    }
}
