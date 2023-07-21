using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Plus3000 : Projectile
{
    private const float duration = 3;
    private const float fadeOutDuration = 1;
    private const float shootSpeed = 25f;

    private bool isDouble;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override float GetDamage(float damage)
    {
        return isDouble ? base.GetDamage(damage) * 2 : base.GetDamage(damage);
    }

    public override void OnHit(Enemy enemy)
    {
        gameObject.SetActive(false);
    }

    public void OnCreate(Vector3 mousePos, Vector3 size, bool isDoubleDamage)
    {
        isHitable = true;
        isDouble = isDoubleDamage;

        spriteRenderer.color = Color.white;
        transform.localScale = size;
        transform.position = Player.Instance.GunPos;

        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180);
        transform.position += (Vector3)Random.insideUnitCircle * 0.3f;

        StartCoroutine(OnDisableCoroutine());
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Time.deltaTime * shootSpeed * Vector3.left);
    }

    private IEnumerator OnDisableCoroutine()
    {
        yield return new WaitForSeconds(duration);

        spriteRenderer.DOFade(0, fadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}