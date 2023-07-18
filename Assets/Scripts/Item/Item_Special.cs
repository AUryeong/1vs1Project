public class Item_Special : Item
{
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Plus3000;
    }
}