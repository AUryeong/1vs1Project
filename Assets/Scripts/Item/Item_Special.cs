public class Item_Special : Item
{
    private const float critPercentIncrease = 30;
    private const float damagePercentDecrease = 10;

    //업그레이드
    private const float upgradeDamagePercent = 5;
    private const float upgradeCritPercent = 5;
    private const float fourthCritDamagePercent = 20;
    private const float fifthCritDamagePercent = 20;
    private const float sixthCritDmgPercent = 40;

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.crit += critPercentIncrease;
        Player.Instance.stat.damage -= damagePercentDecrease;
    }
    
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Plus3000;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
            case 3:
                Player.Instance.stat.damage += upgradeDamagePercent;
                Player.Instance.stat.crit += upgradeCritPercent;
                break;
            case 4:
                Player.Instance.stat.critDmg += fourthCritDamagePercent;
                break;
            case 5:
                Player.Instance.stat.critDmg += fifthCritDamagePercent;
                break;
            case 6:
                Player.Instance.stat.critDmg += sixthCritDmgPercent;
                break;
        }
    }
}