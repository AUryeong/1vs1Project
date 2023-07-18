using UnityEngine;

public class Item_Snack : Item
{
    private float healAmountPercent = 10;
    private const float defaultHealAmountPercent = 10;
    private const float secondHealAmountPercent = 2f;

    private float duration;

    private float coolTime;
    private const float defaultCoolTime = 20;
    private const float thirdCoolTime = 0.9f;
    private const float fourthCoolTime = 0.9f;

    public override void OnReset()
    {
        base.OnReset();
        duration = 0;
        coolTime = defaultCoolTime;
        healAmountPercent = defaultHealAmountPercent;
    }

    public override void OnUpdate(float deltaTime)
    {
        duration += deltaTime;
        if (duration < coolTime) return;

        duration -= coolTime;
        var obj = PoolManager.Instance.Init("Snack");
        obj.transform.position = (Player.Instance.transform.position + (Vector3)Random.insideUnitCircle * 10).ZChange();

        obj.GetComponent<Snack>().healAmount = healAmountPercent;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                healAmountPercent *= secondHealAmountPercent;
                break;
            case 3:
                coolTime *= thirdCoolTime;
                break;
            case 4:
                coolTime *= fourthCoolTime;
                break;
        }
    }
}