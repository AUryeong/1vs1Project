using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SingletonCamera : Singleton<SingletonCamera>
{
    [SerializeField] Camera uiCamera;

    public override void OnReset()
    {
        gameObject.SetActive(true);
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
            canvas.worldCamera = uiCamera;
    }
}
