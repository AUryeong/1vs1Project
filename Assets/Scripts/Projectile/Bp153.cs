using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Bp153 : Projectile
{
    private readonly float duration = 3;
    private readonly float fadeOutDuration = 1;
    private readonly float shootSpeed = 25f;

    private bool isPenetrating;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnHit(Enemy enemy)
    {
        if (item.Upgrade >= 8)
        {
            if (isPenetrating)
                gameObject.SetActive(false);

            isPenetrating = true;
            return;
        }

        gameObject.SetActive(false);
    }

    public void OnCreate(Vector3 mousePos, Vector3 size)
    {
        isHitable = true;
        isPenetrating = false;

        spriteRenderer.color = Color.white;
        transform.localScale = size;
        transform.position = Player.Instance.GunPos;

        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180);

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
