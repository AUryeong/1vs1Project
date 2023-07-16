using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WhiteGirlBomb : MonoBehaviour
{
    [SerializeField] private SpriteRenderer warningPoison;
    [SerializeField] private SpriteRenderer whitePoison;
    private BoxCollider2D whitePoisonCollider2D;
    [SerializeField] private SpriteRenderer bomb;

    private Stat stat;
    private const float downDuration = 2;
    private bool isAttacking = false;

    private void Awake()
    {
        whitePoisonCollider2D = whitePoison.GetComponent<BoxCollider2D>();
    }

    public void OnCreated(Vector3 pos, Stat bossStat)
    {
        stat = bossStat;
        isAttacking = false;

        transform.position = pos.ZChange();
        whitePoison.gameObject.SetActive(false);
        warningPoison.gameObject.SetActive(true);
        bomb.gameObject.SetActive(true);

        warningPoison.DOFade(0.1f, 1).SetLoops(-1, LoopType.Yoyo);

        bomb.transform.localPosition = new Vector3(20, 20, -1);
        bomb.transform.DOLocalMove(Vector3.zero, downDuration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            warningPoison.gameObject.SetActive(false);
            whitePoison.gameObject.SetActive(true);
            bomb.gameObject.SetActive(false);
            isAttacking = true;

            whitePoison.DOFade(0, 1).SetDelay(12).OnComplete(() => gameObject.SetActive(false));
        });
    }

    private void Update()
    {
        AttackUpdate();
    }

    private void AttackUpdate()
    {
        if (!isAttacking) return;

        var result = new List<Collider2D>();
        Physics2D.OverlapCollider(whitePoisonCollider2D, new ContactFilter2D()
        {
            layerMask = LayerMask.GetMask("Player")
        }, result);

        if (result != null && result.Count > 0)
            foreach (var player in result)
                player.GetComponent<Player>().TakeDamage(stat.damage / 2);
    }
}