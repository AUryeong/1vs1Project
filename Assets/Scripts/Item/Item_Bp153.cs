using UnityEngine;

public class Item_Bp153 : Item
{
    private float duration;

    private readonly float defaultDamagePercent = 0.6f;

    private float damagePercent;

    //업그레이드
    private const float twoDamagePercent = 1.2f;
    private const float fiveDamagePercent = 1.4f;


    private const float defaultCoolTime = 0.5f;

    private float coolTime;

    //업그레이드
    private const float threeCoolTimePercent = 0.7f;
    private const float sixCoolTimePercent = 0.5f;


    private readonly Vector3 defaultSize = Vector3.one*0.3f;

    private Vector3 size;

    //업그레이드
    private const float fourSizePercent = 1.25f;
    private const float sevenSizePercent = 1.5f;

    public override void OnReset()
    {
        duration = 0;
        coolTime = defaultCoolTime;
        damagePercent = defaultDamagePercent;
        size = defaultSize;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                damagePercent *= twoDamagePercent;
                break;
            case 3:
                coolTime *= threeCoolTimePercent;
                break;
            case 4:
                size *= fourSizePercent;
                break;
            case 5:
                damagePercent *= fiveDamagePercent;
                break;
            case 6:
                coolTime *= sixCoolTimePercent;
                break;
            case 7:
                size *= sevenSizePercent;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercent;
    }

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Bp153;
    }

    public override void OnShoot()
    {
        if (duration < coolTime)
        {
            return;
        }

        duration -= coolTime;

        GameObject projectile = PoolManager.Instance.Init(nameof(Bp153));
        
        var bp153 = projectile.GetComponent<Bp153>();
        bp153.item = this;

        Vector3 pos = GameManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        bp153.OnCreate(pos, size);
    }

    public override void OnUpdate(float detlaTime)
    {
        if (duration < coolTime)
        {
            duration += detlaTime;
        }
    }
}