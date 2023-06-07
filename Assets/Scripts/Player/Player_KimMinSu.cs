using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_KimMinSu : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.items["Durandal"]);
    }
}