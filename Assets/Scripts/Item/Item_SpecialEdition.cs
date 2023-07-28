using UnityEngine;

public class Item_SpecialEdition : Item
{
    private int skillActivatePercent;
    private const int defaultSkillActivatePercent = 1;
    private const int fourthSKillActivatePercent = 2;
    private const int seventhSKillActivatePercent = 3;

    public override void OnReset()
    {
        skillActivatePercent = defaultSkillActivatePercent;
        base.OnReset();
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 4:
                skillActivatePercent += fourthSKillActivatePercent;
                break;
            case 7:
                skillActivatePercent += seventhSKillActivatePercent;
                break;
        }
    }

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_Plus3000;
    }

    public override float GetDamage(float damage)
    {
        float dmg = base.GetDamage(damage);
        if (Upgrade >= 3)
            dmg *= 1.2f;
        if (Upgrade >= 6)
            dmg *= 1.2f;
        return dmg;
    }

    public override void OnKill(Enemy killEnemy)
    {
        if (Random.Range(1, 11) > skillActivatePercent) return;

        base.OnKill(killEnemy);
        var obj = PoolManager.Instance.Init("Special Edition Effect");
        obj.transform.position = killEnemy.transform.position;

        float radius = 3;
        if (Upgrade >= 2)
            radius *= 1.2f;

        var colliders = Physics2D.OverlapCircleAll(killEnemy.transform.position, radius, LayerMask.GetMask(nameof(Enemy)));
        if (colliders == null || colliders.Length <= 0) return;

        float damage = GetDamage(Player.Instance.GetDamage());
        foreach (var collider2D in colliders)
        {
            var enemy = collider2D.GetComponent<Enemy>();
            enemy.OnHurt(damage);
            if (enemy.Dying)
                if (Random.Range(1, 11) == 1)
                    Player.Instance.TakeHeal(10);
        }
    }
}