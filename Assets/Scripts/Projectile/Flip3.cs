using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Flip3 : Projectile
{
    private const float duration = 4;
    private const float fadeOutDuration = 1;
    private const float shootSpeed = 10f;

    private SpriteRenderer spriteRenderer;

    private Vector2 direction;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnHit(Enemy enemy)
    {
        gameObject.SetActive(false);
        var obj = PoolManager.Instance.Init("Bomb Effect");
        obj.transform.position = enemy.transform.position;

        float radius = 5;
        if (item.Upgrade >= 2)
            radius *= 1.2f;
        if (item.Upgrade >= 5)
            radius *= 1.4f;
        
        var colliders = Physics2D.OverlapCircleAll(enemy.transform.position, radius, LayerMask.GetMask(nameof(Enemy)));
        if (colliders == null || colliders.Length <= 0) return;
        
        float damage = GetDamage(Player.Instance.GetDamage());
        foreach (var collider2D in colliders)
            collider2D.GetComponent<Enemy>().OnHurt(damage);
    }

    public void OnCreate(Vector3 mousePos)
    {
        isHitable = true;

        spriteRenderer.color = Color.white;
        transform.position = Player.Instance.GunPos;

        direction = ((Vector2)(mousePos - transform.position)).normalized;

        StartCoroutine(OnDisableCoroutine());
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(direction * (Time.deltaTime * shootSpeed));
    }

    private IEnumerator OnDisableCoroutine()
    {
        yield return new WaitForSeconds(duration);

        spriteRenderer.DOFade(0, fadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}