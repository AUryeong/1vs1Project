using UnityEngine;

public class Item
{
    private string name;
    private string[] lore;

    //비 획득은 0, 획득 부터 1 시작
    public int Upgrade { get; private set; }
    public Sprite ItemIcon { get; private set; }
    private int maxUpgrade;

    /// <summary>
    /// 아이템 생성시 발동
    /// </summary>
    /// <param name="itemName">아이템 이름</param>
    /// <param name="itemLore">아이템 설명들</param>
    /// <param name="itemMaxUpgrade">최대 업그레이드 횟수</param>
    /// <param name="icon">아이콘</param>
    public void Init(string itemName, string[] itemLore, int itemMaxUpgrade, Sprite icon)
    {
        name = itemName;
        ItemIcon = icon;
        lore = itemLore;
        maxUpgrade = itemMaxUpgrade;
        OnReset();
    }

    public virtual void OnReset()
    {
        Upgrade = 0;
    }

    public string GetName()
    {
        if (Upgrade < 1)
            return name;
        return name + " LV." + Upgrade;
    }

    public string GetLore()
    {
        return lore[Upgrade];
    }

    public virtual bool CanGet()
    {
        return Upgrade <= maxUpgrade;
    }

    public virtual void OnEquip()
    {
        Upgrade = 1;
    }

    public virtual void OnShoot()
    {
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public virtual void OnUpgrade()
    {
        Upgrade++;
    }

    public virtual void OnKill(Enemy killEnemy)
    {
    }

    public virtual float GetDamage(float damage)
    {
        return damage;
    }

    // true일경우 공격 무시
    public virtual bool OnHit(Enemy enemy)
    {
        return false;
    }
}