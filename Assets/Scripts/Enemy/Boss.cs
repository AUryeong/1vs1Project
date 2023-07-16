using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    protected override void Update()
    {
        if (!InGameManager.Instance.isGaming) return;
        
        base.Update();
        if (!Dying)
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

    public override void Die()
    {
        base.Die();
        for (int i = 0; i < 5; i++)
        {
            var exp = PoolManager.Instance.Init("Exp").GetComponent<Exp>();

            exp.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * 5);
            exp.exp = (Random.Range(0.5f, 1.5f) * stat.maxHp + stat.damage) * 0.25f;
        }
        InGameManager.Instance.DieBoss();
    }
}