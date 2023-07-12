using UnityEngine;

public class SingletonCamera : Singleton<SingletonCamera>
{
    protected override bool IsDontDestroying => true;

    protected override void OnCreated()
    {
        OnReset();
    }

    protected override void OnReset()
    {
        gameObject.SetActive(true);
        foreach (var canvas in FindObjectsOfType<Canvas>())
            canvas.worldCamera = GetComponent<Camera>();
    }
}
