using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Unit
{
    public bool dying
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
        dying = false;
    }

    protected virtual void Update()
    {
        if (!InGameManager.Instance.isGaming) return;
        
        float deltaTime = Time.deltaTime;
        if (!dying)
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
    protected override void OnTriggerExit2D(Collider2D collider2D)
    {
        if (dying) return;
        if (collider2D == null) return;

        if (collider2D.CompareTag("Camera"))
            InGameManager.Instance.inCameraEnemies.Remove(this);
    }

    public virtual void OnHurt(Projectile projectile, bool isSkipHitable = false)
    {
        if (dying) return;
        if (!isSkipHitable && !projectile.isHitable) return;

        projectile.OnHit(this);

        OnHurt(projectile.GetDamage(Player.Instance.GetDamage()));

        if (dying)
            projectile.OnKill();
    }
    public virtual void OnHurt(float damage, bool isCanEvade = true, bool isSkipText = false)
    {
        if (dying) return;

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
            SoundManager.Instance.PlaySound("hurt", SoundType.SE, 0.5f);
            return;
        }
        Die();
    }

    protected virtual void Die()
    {
        SoundManager.Instance.PlaySound("enemy", SoundType.SE, 0.4f);
        SoundManager.Instance.PlaySound("enemy 1", SoundType.SE, 0.5f);

        dying = true;
        InGameManager.Instance.OnKill(this);
        Player.Instance.OnKill(this);

        transform.DOMoveX(transform.position.x + ((direction == Direction.Left) ? 1 : -1), 0.5f);

        spriteRenderer.DOFade(0, 0.5f).
            OnComplete(() => gameObject.SetActive(false));
        spriteRenderer.DOColor(new Color(1, 0.7f, 0.7f), 0.1f).SetEase(Ease.InQuart);
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (dying) return;
        if (collider2D == null) return;
        if (!InGameManager.Instance.isGaming) return;

        if (collider2D.CompareTag("Camera"))
            InGameManager.Instance.inCameraEnemies.Add(this);

        else if (collider2D.CompareTag("Projectile"))
            OnHurt(collider2D.GetComponent<Projectile>());
    }

    protected IEnumerator HitFlashWhite()
    {
        spriteRenderer.material = InGameManager.Instance.flashWhiteMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = originalMaterial;
    }
}
