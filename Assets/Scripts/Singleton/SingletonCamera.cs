using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonCamera : Singleton<SingletonCamera>
{
    public override void OnReset()
    {
        gameObject.SetActive(true);
    }
}
