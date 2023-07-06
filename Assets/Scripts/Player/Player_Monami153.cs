using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Monami153 : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.items["Monami153"]);
    }
}