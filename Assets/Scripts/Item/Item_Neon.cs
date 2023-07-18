public class Item_Neon : Item
{
    private float duration = 0;
    private const float coolTime = 0.2f;

    private const float speedPercent = 20;
    private const float upgradeSpeedPercent = 10;

    private float speedAddDuration = 0;
    private const float speedAddCoolTime = 5;
    private const float speedAddPercent = 40;

    public override void OnReset()
    {
        base.OnReset();
        duration = 0;
        speedAddDuration = 0;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.speed += speedPercent;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
            case 3:
                Player.Instance.stat.speed += upgradeSpeedPercent;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        return base.GetDamage(damage) * 0.05f;
    }

    public override bool OnHit(Enemy enemy)
    {
        if (Upgrade < 4) return base.OnHit(enemy);

        if (speedAddDuration <= 0)
        {
            Player.Instance.stat.speed += speedAddPercent;
            speedAddDuration = speedAddCoolTime;
        }
        else
        {
            speedAddDuration = speedAddCoolTime;
        }

        return base.OnHit(enemy);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (speedAddDuration > 0)
        {
            speedAddDuration -= deltaTime;
            if (speedAddDuration <= 0)
                Player.Instance.stat.speed -= speedAddPercent;
        }

        if (Player.Instance.IsMoving)
            duration += deltaTime;
        
        if (duration < coolTime) return;

        duration -= coolTime;
        var neon = PoolManager.Instance.Init("Neon").GetComponent<Neon>();
        neon.item = this;
        neon.OnCreate();
    }

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Flip3;
    }
}