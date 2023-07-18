public class Item_Plus3000 : Item
{
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Plus3000;
    }
}