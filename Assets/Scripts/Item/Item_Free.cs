using UnityEngine;

public class Item_Free : Item
{
    private GameObject auraObj;
    private float damagePercent;
    
    private float maxDamagePercent;
    private const float defaultMaxDamagePercent = 20f;
    private const float secondMaxDamagePercent = 10f;
    private const float fourthMaxDamagePercent = 20f;

    private float healDuration = 0;
    private const float healCoolTime = 5;

    public override void OnEquip()
    {
        base.OnEquip();
        auraObj = PoolManager.Instance.Init("Free");
        auraObj.GetComponent<Projectile>().isHitable = false;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                maxDamagePercent += secondMaxDamagePercent;
                break;
            case 4:
                maxDamagePercent += fourthMaxDamagePercent;
                break;
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        if (auraObj != null)
        {
            auraObj.gameObject.SetActive(false);
            auraObj = null;
        }

        healDuration = 0;
        damagePercent = 0;
        maxDamagePercent = defaultMaxDamagePercent;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (auraObj != null)
            auraObj.transform.position = Player.Instance.transform.position;
        
        float percent = damagePercent;
        damagePercent = Player.Instance.IsMoving ? Mathf.Min(damagePercent + deltaTime * 3, maxDamagePercent) : Mathf.Max(damagePercent - deltaTime * 9, 0);
        if (Upgrade >= 3 && damagePercent >= maxDamagePercent)
        {
            healDuration += deltaTime;
            if (healDuration >= healCoolTime)
            {
                healDuration -= healCoolTime;
                Player.Instance.TakeHeal(3);
            }
        }

        Player.Instance.stat.damage += damagePercent - percent;
    }

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Flip3;
    }
}