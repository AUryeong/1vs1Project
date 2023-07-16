using UnityEngine;

public class Item
{
    private string name;
    private string[] lore;

    //�� ȹ���� 0, ȹ�� ���� 1 ����
    public int Upgrade { get; private set; }
    public Sprite ItemIcon { get; private set; }
    private int maxUpgrade;

    /// <summary>
    /// ������ ������ �ߵ�
    /// </summary>
    /// <param name="itemName">������ �̸�</param>
    /// <param name="itemLore">������ �����</param>
    /// <param name="itemMaxUpgrade">�ִ� ���׷��̵� Ƚ��</param>
    /// <param name="icon">������</param>
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

    // true�ϰ�� ���� ����
    public virtual bool OnHit(Enemy enemy)
    {
        return false;
    }
}