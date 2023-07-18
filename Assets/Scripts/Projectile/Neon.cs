using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Neon : Projectile
{
    private SpriteRenderer spriteRenderer;

    private float attackDuration;
    private const float attackCoolTime = 1f;

    private const float disableDuration = 1;
    private const float fadeOutDuration = 2;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void OnCreate()
    {
        isHitable = true;
        
        spriteRenderer.color = Color.white;
        transform.position = Player.Instance.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f,360f));

        attackDuration = 0;

        spriteRenderer.DOFade(0, fadeOutDuration).SetDelay(disableDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        attackDuration += Time.deltaTime;
        if (attackDuration < attackCoolTime) return;
        
        attackDuration -= attackCoolTime;
        var colliders = Physics2D.OverlapCircleAll(transform.position, 1.2f, LayerMask.GetMask(nameof(Enemy)));
        if (colliders == null || colliders.Length <= 0) return;
        
        float damage = GetDamage(Player.Instance.GetDamage());
        foreach (var collider2D in colliders)
            collider2D.GetComponent<Enemy>().OnHurt(damage);
    }
}