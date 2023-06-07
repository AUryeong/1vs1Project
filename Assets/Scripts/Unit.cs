using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Stat
{
    public float speed;
    public float hp;
    public float maxHp;
    public float crit;
    public float critDmg;
    public float damage;
    public float evade;
}
public enum Direction
{
    Left,
    Right
}
public class Unit : MonoBehaviour
{
    public Stat stat;

    protected virtual void OnTriggerEnter2D(Collider2D collider2D)
    {
    }

    protected virtual void OnTriggerExit2D(Collider2D collider2D)
    {
    }
    public virtual float GetDamage()
    {
        float damage = stat.damage;
        damage += Random.Range(stat.damage / -10f, stat.damage / 10f);
        if (Random.Range(0f, 100f) <= stat.crit)
        {
            damage *= stat.critDmg / 100;
        }
        return damage;
    }
}
