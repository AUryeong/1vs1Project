using UnityEngine;

public class Item_Plus3000 : Item
{
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Plus3000;
    }

    private float damagePercent;

    private const float defaultDamagePercent = 0.1f;
    private const float thirdUpgradeDamagePercent = 1.2f;
    private const float sixthUpgradeDamagePercent = 1.3f;

    private float duration;
    private int shootUpgradeCount = 0;
    private const int shootUpgradeCoolCount = 5;

    private float coolTime;

    private const float defaultCoolTime = 0.15f;
    private const float secondUpgradeCoolTimePercent = 0.8f;
    private const float fifthUpgradeCoolTimePercent = 0.7f;

    private readonly Vector3 defaultSize = Vector3.one;
    private const float seventhUpgradeSizePercent = 1.4f;

    private Vector3 size;

    private float skillDuration = 0;
    private const float skillMaxDuration = 5;

    private const float skillCoolTime = 20;
    private const float skillBulletCoolTime = 0.002f;

    public override void OnReset()
    {
        duration = 0;
        coolTime = defaultCoolTime;
        damagePercent = defaultDamagePercent;
        size = defaultSize;
        skillDuration = 0;
        shootUpgradeCount = 0;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                coolTime *= secondUpgradeCoolTimePercent;
                break;
            case 3:
                damagePercent *= thirdUpgradeDamagePercent;
                break;
            case 5:
                coolTime *= fifthUpgradeCoolTimePercent;
                break;
            case 6:
                damagePercent *= sixthUpgradeDamagePercent;
                break;
            case 7:
                size *= seventhUpgradeSizePercent;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercent * Random.Range(0.5f, 1.5f);
    }

    public override void OnShoot()
    {
        if (IsSkillActivating())
        {
            if (duration < skillBulletCoolTime) return;

            duration -= skillBulletCoolTime;
        }
        else
        {
            if (duration < coolTime) return;

            duration -= coolTime;
        }

        bool isDouble = false;

        if (Upgrade >= 4)
        {
            shootUpgradeCount++;
            if (shootUpgradeCount >= shootUpgradeCoolCount)
            {
                shootUpgradeCount -= shootUpgradeCoolCount;
                isDouble = true;
            }
        }

        GameObject projectile = PoolManager.Instance.Init(nameof(Plus3000));

        var plus3000 = projectile.GetComponent<Plus3000>();
        plus3000.item = this;

        Vector3 pos = GameManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);

        InGameManager.Instance.CameraShake(0.02f, 0.03f);
        plus3000.OnCreate(pos, isDouble ? size * 2 : size, isDouble);
    }

    public override void OnKill(Enemy killEnemy)
    {
        if (skillDuration < 0 && Random.Range(0, 100) == 0)
        {
            skillDuration += Time.deltaTime;
        }
    }

    public override void OnUpdate(float detlaTime)
    {
        if (duration < coolTime)
        {
            duration += detlaTime;
        }

        if (skillDuration > 0)
        {
            skillDuration += Time.deltaTime;
            if (skillDuration >= skillCoolTime)
            {
                skillDuration = 0;
            }
        }
    }

    private bool IsSkillActivating()
    {
        return Upgrade >= 8 && skillDuration > 0 && skillDuration < skillMaxDuration;
    }
}