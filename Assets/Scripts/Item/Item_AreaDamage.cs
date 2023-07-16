using UnityEngine;

public class Item_AreaDamage : Item
{
    private float duration = 0;
    private const float coolTime = 0.3f;

    private const float defaultDamagePercent = 0.1f;

    private float damagePercent;

    //업그레이드
    private const float threeDamagePercent = 1.3f;


    private readonly Vector3 defaultSize = Vector3.one;
    private Vector3 size;


    private const float overlapMultiplier = 1.5125f;

    //업그레이드
    private const float twoSizePercent = 1.15f;
    private const float fourSizePercent = 1.25f;
    private const float fiveSizePercent = 1.5f;

    private GameObject auraObj;

    public override void OnEquip()
    {
        base.OnEquip();
        auraObj = PoolManager.Instance.Init("AreaDamage");
        auraObj.transform.localScale = size;
        auraObj.GetComponent<Projectile>().isHitable = false;
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercent;
    }

    public override void OnReset()
    {
        base.OnReset();
        if (auraObj != null)
        {
            auraObj.gameObject.SetActive(false);
            auraObj = null;
        }

        damagePercent = defaultDamagePercent;
        size = defaultSize;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                size *= twoSizePercent;
                break;
            case 3:
                damagePercent *= threeDamagePercent;
                break;
            case 4:
                size *= fourSizePercent;
                break;
            case 5:
                size *= fiveSizePercent;
                break;
        }

        auraObj.transform.localScale = size;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (auraObj != null)
            auraObj.transform.position = Player.Instance.transform.position;

        duration += deltaTime;
        if (duration < coolTime)
            return;
        duration -= coolTime;

        var colliders = Physics2D.OverlapCircleAll(Player.Instance.transform.position, size.x * overlapMultiplier, LayerMask.GetMask(nameof(Enemy)));
        if (colliders == null || colliders.Length <= 0) return;
        
        float damage = GetDamage(Player.Instance.GetDamage());
        foreach (var collider2D in colliders)
            collider2D.GetComponent<Enemy>().OnHurt(damage);
    }
}