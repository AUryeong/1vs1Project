public class Item_Book : Item
{
    private const float defaultXpAddPercent = 10f;
    private const float upgradeXpAddPercent = 5f;

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.xpAdd += defaultXpAddPercent;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
            case 3:
            case 4:
                Player.Instance.xpAdd += upgradeXpAddPercent;
                break;
        }
    }
}