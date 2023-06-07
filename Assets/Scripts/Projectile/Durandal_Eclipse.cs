using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Durandal_Eclipse : Projectile
{
    SpriteRenderer spriteRenderer;

    float downDuration = 0.35f;
    float downWaitDuration = 1;
    float downFadeOutDuration = 1;
    Vector3 increaseHeight = new Vector3(10, 10, -10);

    float damagePercent = 0.5f;

    [SerializeField]
    Sprite downSprite;
    Sprite defaultSprite;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }
    public override float GetDamage(float damage)
    {
        return base.GetDamage(damage) * damagePercent;
    }
    public void OnCreate(Vector3 wantPos)
    {
        isHitable = false;
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = Color.white;

        wantPos = new Vector3(wantPos.x, wantPos.y, wantPos.y - spriteRenderer.size.y);
        transform.position = wantPos + increaseHeight;

        transform.DOMove(wantPos, downDuration).SetEase(Ease.InQuint).
            OnComplete(() => StartCoroutine(OnDownCoroutine()));
    }
    IEnumerator OnDownCoroutine()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, spriteRenderer.bounds.size.x / 2, LayerMask.GetMask(nameof(Enemy)));
        if (enemies != null && enemies.Length > 0)
            foreach (Collider2D collider2D in enemies)
                collider2D.GetComponent<Enemy>().OnHurt(this, true);

        spriteRenderer.sprite = downSprite;

        yield return new WaitForSeconds(downWaitDuration);

        spriteRenderer.DOFade(0, downFadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}
