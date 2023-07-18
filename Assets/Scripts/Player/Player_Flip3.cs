using UnityEngine;

public class Player_Flip3 : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.GetItem("Bp153"));
    }
}