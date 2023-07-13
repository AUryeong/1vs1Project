public class Item_GodBless : Item
{
    private float duration;
    private float coolTime;

    private const float defaultCoolTime = 60;

    //업그레이드
    private const float upgradeCoolTimePercent = 0.9f;


    private const float upgradeDamageAdder = 5;

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Bp153;
    }

    public override void OnReset()
    {
        base.OnReset();
        coolTime = defaultCoolTime;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.damage += upgradeDamageAdder;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                coolTime *= upgradeCoolTimePercent;
                break;
            case 3:
                coolTime *= upgradeCoolTimePercent;
                Player.Instance.stat.damage += upgradeDamageAdder;
                break;
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        duration += deltaTime;
        if (duration >= coolTime)
        {
            duration = 0;
            Player.Instance.TakeHeal(Player.Instance.stat.maxHp - Player.Instance.stat.hp);
        }
    }
}