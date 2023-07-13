public class Player_Bp153 : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.GetItem("Bp153"));
    }
}