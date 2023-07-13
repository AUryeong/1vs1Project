public class Item_Daybreak : Item
{
    private const float critPercentDecrease = 30;
    private const float damagePercentIncrease = 30;

    //업그레이드
    private const float twoDamagePercentIncrease = 10;
    private const float threeDamagePercentIncrease = 15;
    private const float fourDamagePercentIncrease = 20;

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Bp153;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.crit -= critPercentDecrease;
        Player.Instance.stat.damage += damagePercentIncrease;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                Player.Instance.stat.damage += twoDamagePercentIncrease;
                break;
            case 3:
                Player.Instance.stat.damage += threeDamagePercentIncrease;
                break;
            case 4:
                Player.Instance.stat.damage += fourDamagePercentIncrease;
                break;
        }
    }
}