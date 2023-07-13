using UnityEngine;

public class Player_Plus3000 : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.GetItem("Bp153"));
    }
}