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

    public void OnCreate(Vector3 wantPos, Vector3 size)
    {
        isHitable = true;
        isPenetrating = false;
        spriteRenderer.color = Color.white;

        transform.position = Player.Instance.transform.position;
        float angle = Mathf.Atan2(wantPos.y - transform.position.y, wantPos.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,angle+180);

        transform.localScale = size;
        StartCoroutine(OnDisableCoroutine());
    }

    private void Update()
    {
        transform.Translate(Vector3.left * (Time.deltaTime * shootSpeed));
    }
    private IEnumerator OnDisableCoroutine()
    {
        yield return new WaitForSeconds(duration);
        
        spriteRenderer.DOFade(0, fadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}
