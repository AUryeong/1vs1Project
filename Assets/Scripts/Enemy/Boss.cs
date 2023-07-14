using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    protected override void Update()
    {
        base.Update();
        if (!dying)
            LiveUpdate(Time.deltaTime);
    }

    protected virtual void LiveUpdate(float deltaTime)
    {
    }

    public override void OnHurt(float damage, bool isCanEvade = true, bool isSkipText = false)
    {
        base.OnHurt(damage, isCanEvade, isSkipText);
        UIManager.Instance.UpdateBossHp(stat.hp / stat.maxHp);
    }

    protected override void Die()
    {
        base.Die();
        InGameManager.Instance.DieBoss();
    }
}