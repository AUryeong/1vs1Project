using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Item item;
    public bool isHitable = true;

    public virtual void OnHit(Enemy enemy)
    {
    }

    public virtual float GetDamage(float damage)
    {
        return item.GetDamage(damage);
    }

    public virtual void OnKill()
    {
    }

}
