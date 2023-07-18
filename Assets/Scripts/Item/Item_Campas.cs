using UnityEngine;

public class Item_Campas : Item
{
    private float duration;

    private const float defaultDamagePercent = 0.6f;

    private float damagePercent;

    //업그레이드
    private const float thirdDamagePercent = 1.2f;
    private const float sixthDamagePercent = 1.4f;


    private const float defaultCoolTime = 2f;

    private float coolTime;

    //업그레이드
    private const float fifthCoolTimePercent = 0.8f;


    private readonly Vector3 defaultSize = Vector3.one*0.3f;

    private Vector3 size;

    //업그레이드
    private const float secondSizePercent = 1.2f;
    private const float seventhSizePercent = 1.4f;

    private int campasCount = 1;
    private int defaultCampasCount = 1;
    private int upgradeCampasCount = 1;

    public override void OnReset()
    {
        duration = 0;
        campasCount = defaultCampasCount;
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
                size *= secondSizePercent;
                break;
            case 3:
                damagePercent *= thirdDamagePercent;
                break;
            case 4:
                campasCount += upgradeCampasCount;
                break;
            case 5:
                coolTime *= fifthCoolTimePercent;
                break;
            case 6:
                damagePercent *= sixthDamagePercent;
                break;
            case 7:
                size *= seventhSizePercent;
                break;
            case 8:
                campasCount += upgradeCampasCount;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercent;
    }

    public override void OnUpdate(float detlaTime)
    {
        duration += detlaTime;
        if (duration < coolTime) return;
        
        duration -= coolTime;
        CreateCampas();

    }

    private void CreateCampas()
    {
        for (int i = 0; i < campasCount; i++)
        {
            GameObject projectile = PoolManager.Instance.Init(nameof(Campas));
        
            var campas = projectile.GetComponent<Campas>();
            campas.item = this;

            campas.OnCreate(360/campasCount*(i+1), size);
            campas.transform.localScale = size;
        }
    }
}