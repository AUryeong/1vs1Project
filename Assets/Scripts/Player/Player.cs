using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Unit
{
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = InGameManager.Instance.CreatePlayer();
            }

            return instance;
        }
    }
    private static Player instance;

    //캐릭터 기본 스텟, 일반 stat은 퍼센트로 표현
    public Stat defaultStat;
    private readonly List<Item> items = new List<Item>();

    public Vector3 GunPos
    {
        get
        {
            return gun.transform.position;
        }
    }

    #region 레벨 변수

    public float xpAdd = 100f;
    public int lv = 0;
    private float exp = 0;
    public float maxExp = 100f;
    public float xpGetRadius = 100f;

    public float Exp
    {
        get => exp;
        set
        {
            exp = value * xpAdd / 100;
            if (exp >= maxExp)
            {
                exp -= maxExp;
                lv++;
                maxExp = Mathf.Pow(lv, 1.5f) * 100;
                InGameManager.Instance.AddWeapon();
            }

            UIManager.Instance.UpdateLevel();
        }
    }

    #endregion

    #region 몹 충돌 변수

    private bool hurtInv;

    private readonly Color hitTextColor = new Color(255 / 255f, 66 / 255f, 66 / 255f);
    private readonly Color healTextColor = new Color(101 / 255f, 255 / 255f, 76 / 255f);
    
    private const float HIT_FADE_IN_ALPHA = 0.8f;
    private const float HIT_FADE_OUT_ALPHA = 1f;
    private const float HIT_FADE_TIME = 0.1f;

    private const float DAMAGE_RANDOM_PERCENT = 10;

    [HideInInspector] public List<Enemy> collisionEnemyList = new List<Enemy>();

    #endregion

    public bool IsMoving
    {
        get;
        private set;
    }

    #region 애니메이션 변수

    [SerializeField] protected SpriteRenderer gun;

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rigid;
    
    private static readonly int IS_WALKING_ANIMATION_HASH = Animator.StringToHash("isWalking");

    #endregion


    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (UIManager.Instance.IsActable())
        {
            float deltaTime = Time.deltaTime;
            Move();
            HitCheck();
            ShootUpdate();
            foreach (Item item in items)
                item.OnUpdate(deltaTime);

            var mouse = GameManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mouse.y - gun.transform.position.y, mouse.x - gun.transform.position.x) * Mathf.Rad2Deg;
            gun.flipY = (mouse.x - gun.transform.position.x >= 0);
            if (gun.flipY)
            {
                angle = Mathf.Clamp(angle, -40, 40);
            }
            else
            {
                if (angle > 0 && angle < 140)
                    angle = 140;
                else if (angle < 0 && angle > -140)
                    angle = -140f;
            }

            gun.transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            if (Input.GetKeyDown(KeyCode.F))
            {
                Exp += maxExp;
            }
        }
        else
        {
            rigid.velocity = Vector2.zero;
            animator.SetBool(IS_WALKING_ANIMATION_HASH, false);
        }
    }


    public void OnKill(Enemy enemy)
    {
        foreach (var item in items)
            item.OnKill(enemy);
    }

    public List<Item> GetInventory()
    {
        return new List<Item>(items);
    }

    public void AddItem(Item addItem)
    {
        var item = items.Find(item => item == addItem);
        if (item == null)
        {
            item = addItem;
            items.Add(item);
            item.OnEquip();
        }
        else
            item.OnUpgrade();
    }

    protected virtual void ShootUpdate()
    {
        if (Input.GetMouseButton(0))
            foreach (var item in items)
                item.OnShoot();
    }

    protected virtual void Die()
    {
        InGameManager.Instance.GameOver();
    }

    public override float GetDamage()
    {
        float damage = stat.damage / 100 * defaultStat.damage;
        damage += Random.Range(damage / -DAMAGE_RANDOM_PERCENT, damage / DAMAGE_RANDOM_PERCENT);
        if (Random.Range(0f, 100f) <= stat.crit)
        {
            damage *= stat.critDmg / 100;
        }

        return damage;
    }

    public void TakeHeal(float healAmount, bool isSkipText = false)
    {
        stat.hp += healAmount;
        if (stat.hp > stat.maxHp)
        {
            stat.hp = stat.maxHp;
        }

        UIManager.Instance.UpdateHp(stat.hp / (stat.maxHp / 100 * defaultStat.maxHp));
        if (!isSkipText)
            InGameManager.Instance.ShowInt((int)healAmount, transform.position, healTextColor);
    }

    public void TakeDamage(float damage, bool invAttack = false, bool isSkipText = false)
    {
        if (hurtInv || stat.hp <= 0) return;
        
        hurtInv = true;

        spriteRenderer.DOFade(HIT_FADE_IN_ALPHA, HIT_FADE_TIME)
            .OnComplete(() => spriteRenderer.DOFade(HIT_FADE_OUT_ALPHA, HIT_FADE_TIME).OnComplete(() => hurtInv = false));
        
        if (invAttack)
        {
            if (!isSkipText)
                InGameManager.Instance.ShowText("MISS", transform.position, Color.white);
            return;
        }

        SoundManager.Instance.PlaySound("hurt");
        stat.hp -= damage;
        if (stat.hp <= 0)
        {
            stat.hp = 0;
            Die();
        }

        UIManager.Instance.UpdateHp(stat.hp / (stat.maxHp / 100 * defaultStat.maxHp));
        if (!isSkipText)
            InGameManager.Instance.ShowInt((int)damage, transform.position, hitTextColor);
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D == null)
            return;

        if (collider2D.CompareTag("Enemy"))
            collisionEnemyList.Add(collider2D.GetComponent<Enemy>());
        if (collider2D.CompareTag("Exp"))
            collider2D.GetComponent<Exp>().OnGet();
    }

    protected override void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D == null)
            return;

        if (collider2D.CompareTag("Enemy"))
            collisionEnemyList.Remove(collider2D.GetComponent<Enemy>());
    }

    private void HitCheck()
    {
        if (hurtInv || stat.hp <= 0) return;

        foreach (Enemy enemy in collisionEnemyList.ToArray())
        {
            if (enemy == null || !enemy.gameObject.activeSelf || enemy.Dying) return;

            if (Random.Range(0f, 100f) <= stat.evade)
            {
                TakeDamage(0, true);
                return;
            }

            bool invAttack = false;
            foreach (Item item in items)
                if (item.OnHit(enemy))
                    invAttack = true;
            
            TakeDamage(enemy.GetDamage(), invAttack);
        }
    }

    private void Move()
    {
        float speedX = 0;
        float speedY = 0;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            speedX = stat.speed / 100 * defaultStat.speed;
            spriteRenderer.flipX = true;
            gun.transform.localPosition = new Vector3(0.3f, gun.transform.localPosition.y, gun.transform.localPosition.z);
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            speedX -= stat.speed / 100 * defaultStat.speed;
            spriteRenderer.flipX = false;
            gun.transform.localPosition = new Vector3(-0.3f, gun.transform.localPosition.y, gun.transform.localPosition.z);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            speedY = stat.speed / 100 * defaultStat.speed;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            speedY -= stat.speed / 100 * defaultStat.speed;
        }
        animator.SetBool(IS_WALKING_ANIMATION_HASH, speedX != 0 || speedY != 0);
        IsMoving = speedX != 0 || speedY != 0;

        rigid.velocity = new Vector2(speedX, speedY );

        transform.position = InGameManager.Instance.GetPosInMap(transform.position, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
}