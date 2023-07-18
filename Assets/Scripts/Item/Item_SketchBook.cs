using UnityEngine;

public class Item_SketchBook : Item
{
    private GameObject sketchBookObj;
    private GameObject sketchBookSecondObj;

    private float damagePercent;
    private const float defaultDamagePercent = 0.5f;
    private const float thirdDamagePercent = 1.2f;

    private float spinSpeed;
    private const float defaultSpinSpeed = 3;
    private const float secondSpinSpeed = 1.2f;

    private float spinRadius;
    private const float defaultSpinRadius = 2.5f;
    private const float sixthSpinRadiusPercent = 1.5f;

    private const float defaultSizePercent = 1.2f;
    private const float fourthSizePercent = 1.2f;
    private const float sixthSizePercent = 1.4f;

    public override void OnReset()
    {
        base.OnReset();
        if (sketchBookObj != null)
        {
            sketchBookObj.gameObject.SetActive(false);
            sketchBookObj = null;
        }
        if (sketchBookSecondObj != null)
        {
            sketchBookSecondObj.gameObject.SetActive(false);
            sketchBookSecondObj = null;
        }

        damagePercent = defaultDamagePercent;
        spinRadius = defaultSpinRadius;
        spinSpeed = defaultSpinSpeed;
    }
    public override void OnEquip()
    {
        base.OnEquip();
        sketchBookObj = PoolManager.Instance.Init("SketchBook");
        sketchBookObj.transform.localScale = Vector3.one * defaultSizePercent;
        sketchBookObj.GetComponent<Projectile>().item = this;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (Upgrade)
        {
            case 2:
                spinSpeed *= secondSpinSpeed;
                break;
            case 3:
                damagePercent *= thirdDamagePercent;
                break;
            case 4:
                sketchBookObj.transform.localScale *= fourthSizePercent;
                break;
            case 5:
                sketchBookSecondObj = PoolManager.Instance.Init("SketchBook");
                sketchBookSecondObj.transform.localScale = Vector3.one * defaultSizePercent * fourthSizePercent;
                sketchBookSecondObj.GetComponent<Projectile>().item = this;
                break;
            case 6:
                spinRadius *= sixthSpinRadiusPercent;
                sketchBookObj.transform.localScale *= sixthSizePercent;
                sketchBookSecondObj.transform.localScale *= sixthSizePercent;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercent;
    }

    public override void OnUpdate(float detlaTime)
    {
        if (sketchBookObj != null)
        {
            var pos = new Vector3(Mathf.Cos(Time.time * spinSpeed) * spinRadius, Mathf.Sin(Time.time * spinSpeed) * spinRadius);
            sketchBookObj.transform.position = Player.Instance.transform.position + pos;
        }
        if (sketchBookSecondObj != null)
        {
            var pos = new Vector3(Mathf.Sin(Time.time * spinSpeed) * spinRadius, Mathf.Cos(Time.time * spinSpeed) * spinRadius);
            sketchBookSecondObj.transform.position = Player.Instance.transform.position + pos;
        }
    }

    public override void OnKill(Enemy killEnemy)
    {
        base.OnKill(killEnemy);
        if (Upgrade >= 8)
            Player.Instance.TakeHeal(1, true);
    }
}