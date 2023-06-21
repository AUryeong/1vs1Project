using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Durandal : Projectile
{
    float downDuration = 0.5f;
    float downWaitDuration = 3;
    float downFadeOutDuration = 1;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnHit(Enemy enemy)
    {
        if (item.upgrade >= 8) return;
        
        gameObject.SetActive(false);
    }

    public void OnCreate(Vector3 wantPos, Vector3 size)
    {
        isHitable = true;
        spriteRenderer.color = Color.white;

        transform.position = Player.Instance.transform.position;
        float angle = Mathf.Atan2(wantPos.y - transform.position.y, wantPos.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle+180);

        transform.localScale = size;
        StartCoroutine(OnDownCoroutine());
    }

    void Update()
    {
        transform.Translate(Vector2.left * (Time.deltaTime * 25));
    }
    IEnumerator OnDownCoroutine()
    {
        yield return new WaitForSeconds(downWaitDuration);
        
        spriteRenderer.DOFade(0, downFadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}
