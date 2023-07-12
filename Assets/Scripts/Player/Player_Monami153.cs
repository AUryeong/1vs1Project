public class Player_Monami153 : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.GetItem(nameof(Monami153)));
    }
}