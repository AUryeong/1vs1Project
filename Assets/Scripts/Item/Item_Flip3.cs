using UnityEngine;

public class Item_Flip3 : Item
{
    private float duration;

    private const float defaultDamagePercent = 0.5f;
    private const float fourthDamagePercent = 1.2f;
    private const float seventhDamagePercent = 1.4f;

    private float damagePercent;

    private const float coolTime = 1.5f;

    private Vector3 size = Vector3.one;
    
    private readonly Vector3 defaultSize = Vector3.one;
    private const float thirdSizePercent = 1.2f;
    private const float sixthSizePercent = 1.2f;

    public override void OnReset()
    {
        duration = 0;
        damagePercent = defaultDamagePercent;
        size =defaultSize;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                break;
            case 3:
                size *= thirdSizePercent;
                break;
            case 4:
                damagePercent *= fourthDamagePercent;
                break;
            case 5:
                break;
            case 6:
                size *= sixthSizePercent;
                break;
            case 7:
                damagePercent *= seventhDamagePercent;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        if (Upgrade >= 8)
            damage *= Player.Instance.stat.speed / 100f;
        return damage * damagePercent;
    }

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Flip3;
    }

    public override void OnShoot()
    {
        if (duration < coolTime) return;

        duration -= coolTime;

        GameObject projectile = PoolManager.Instance.Init(nameof(Flip3));
        
        var flip3 = projectile.GetComponent<Flip3>();
        flip3.item = this;
        
        var scale = size;
        if (Upgrade >= 8)
            scale *= Player.Instance.stat.speed / 100f;
        flip3.transform.localScale = scale;

        Vector3 pos = GameManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        flip3.OnCreate(pos);
    }

    public override void OnUpdate(float deltaTime)
    {
        if (duration < coolTime)
        {
            duration += deltaTime;
        }
    }
}