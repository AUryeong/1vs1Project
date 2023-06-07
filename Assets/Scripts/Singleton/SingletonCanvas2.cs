using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonCanvas2 : Singleton<SingletonCanvas2>
{
    public override void OnReset()
    {
        gameObject.SetActive(true);
        GetComponent<Canvas>().worldCamera = Camera.allCameras[1];
    }
}
