public class Item_Magnet : Item
{
    private const float defaultXpGetPercent = 20f;
    private const float secondXpGetPercent = 20f;
    private const float thirdXpGetPercent = 20f;
    private const float fourthXpGetPercent = 40f;

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.xpGetRadius += defaultXpGetPercent;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                Player.Instance.xpGetRadius += secondXpGetPercent;
                break;
            case 3:
                Player.Instance.xpGetRadius += thirdXpGetPercent;
                break;
            case 4:
                Player.Instance.xpGetRadius += fourthXpGetPercent;
                break;
        }
    }
}