public class Item_Shoes : Item
{
    private const float defaultSpeedPercent = 10f;
    private const float secondUpgradeSpeedPercent = 20f;
    private const float thirdUpgradeSpeedPercent = 20f;
    private const float fourthUpgradeSpeedPercent = 30f;

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.speed += defaultSpeedPercent;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                Player.Instance.stat.speed += secondUpgradeSpeedPercent;
                break;
            case 3:
                Player.Instance.stat.speed += thirdUpgradeSpeedPercent;
                break;
            case 4:
                Player.Instance.stat.speed += fourthUpgradeSpeedPercent;
                break;
        }
    }
}