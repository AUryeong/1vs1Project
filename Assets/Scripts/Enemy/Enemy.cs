using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Enemy : Unit
{
    public bool Dying
    {
        get;
        private set;
    }
    protected Direction direction;
    protected SpriteRenderer spriteRenderer;
    private Coroutine enemyFlashWhiteCo;
    private Material originalMaterial;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }
    protected virtual void OnEnable()
    {
        stat.hp = stat.maxHp;
        spriteRenderer.color = Color.white;
        Dying = false;
    }

    protected virtual void Update()
    {
        if (!InGameManager.Instance.isGaming) return;
        
        float deltaTime = Time.deltaTime;
        if (!Dying)
        {
            Move(deltaTime);
        }
    }

    protected virtual void Move(float deltaTime)
    {
        transform.Translate(stat.speed * deltaTime * (Player.Instance.transform.position - transform.position).normalized);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

        if (Player.Instance.transform.position.x - transform.position.x < 0)
        {
            direction = Direction.Left;
            spriteRenderer.flipX = false;
        }
        else
        {
            direction = Direction.Right;
            spriteRenderer.flipX = true;
        }
    }

    protected virtual void OnHurt(Projectile projectile, bool isSkipHitable = false)
    {
        if (Dying) return;
        if (!isSkipHitable && !projectile.isHitable) return;

        projectile.OnHit(this);

        OnHurt(projectile.GetDamage(Player.Instance.GetDamage()));

        if (Dying)
            projectile.OnKill();
    }
    public virtual void OnHurt(float damage, bool isCanEvade = true, bool isSkipText = false)
    {
        if (Dying) return;

        if (isCanEvade)
            if (Random.Range(0f, 100f) <= stat.evade)
            {
                if (!isSkipText)
                    InGameManager.Instance.ShowText("MISS", transform.position, Color.white);
                return;
            }
        if (!isSkipText)
            InGameManager.Instance.ShowInt((int)damage, transform.position, Color.white);

        stat.hp -= damage;
        if (enemyFlashWhiteCo != null)
            StopCoroutine(enemyFlashWhiteCo);
        enemyFlashWhiteCo = StartCoroutine(HitFlashWhite());

        if (stat.hp > 0)
        {
            SoundManager.Instance.PlaySound("hurt", SoundType.Se, 0.5f);
            return;
        }
        Die();
    }

    public virtual void Die()
    {
        SoundManager.Instance.PlaySound("enemy", SoundType.Se, 0.4f);
        SoundManager.Instance.PlaySound("enemy 1", SoundType.Se, 0.5f);

        Dying = true;
        InGameManager.Instance.OnKill(this);
        Player.Instance.OnKill(this);

        transform.DOMoveX(transform.position.x + ((direction == Direction.Left) ? 1 : -1), 0.5f);

        spriteRenderer.DOFade(0, 0.5f).
            OnComplete(() => gameObject.SetActive(false));
        spriteRenderer.DOColor(new Color(1, 0.7f, 0.7f), 0.1f).SetEase(Ease.InQuart);
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (Dying) return;
        if (collider2D == null) return;
        if (!InGameManager.Instance.isGaming) return;

        if (collider2D.CompareTag("Projectile"))
            OnHurt(collider2D.GetComponent<Projectile>());
    }

    private IEnumerator HitFlashWhite()
    {
        spriteRenderer.material = InGameManager.Instance.flashWhiteMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = originalMaterial;
    }
}
