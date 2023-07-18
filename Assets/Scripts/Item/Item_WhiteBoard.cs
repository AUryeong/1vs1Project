using UnityEngine;

public class Item_WhiteBoard : Item
{
    private GameObject shieldObj;

    private int shieldCount;
    private int shieldRegenCount;
    
    private const int defaultShieldCount = 1;
    private const int upgradeShieldCount = 2;
    
    private float duration;
    private const float shieldRegenDuration = 15;

    public override void OnReset()
    {
        base.OnReset();
        
        shieldRegenCount = defaultShieldCount;
        duration = 0;
        
        if (shieldObj != null)
        {
            shieldObj.gameObject.SetActive(false);
            shieldObj = null;
        }
    }

    public override void OnEquip()
    {
        base.OnEquip();
        ShieldRegen();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                shieldRegenCount = upgradeShieldCount;
                ShieldRegen();
                break;
        }
    }

    private void ShieldRegen()
    {
        shieldCount = shieldRegenCount;
        if (shieldObj != null) return;
        
        shieldObj = PoolManager.Instance.Init("WhiteBoard");
        shieldObj.transform.position = Player.Instance.transform.position;
        shieldObj.GetComponent<Projectile>().isHitable = false;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (shieldObj != null)
        {
            shieldObj.transform.position = Player.Instance.transform.position;
        }
        else
        {
            duration += deltaTime;
            if (duration >= shieldRegenDuration)
            {
                duration -= shieldRegenDuration;
                ShieldRegen();
            }
        }
    }

    public override bool OnHit(Enemy enemy)
    {
        if (shieldCount > 0)
        {
            shieldCount--;
            if (shieldCount <= 0)
            {
                shieldObj.gameObject.SetActive(false);
                shieldObj = null;
            }
            return true;
        }
        return base.OnHit(enemy);
    }
}