using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WhiteGirlBoss : Boss
{
    private bool isShooting;
    private bool isLaserShooting;

    private float shootDuration = 0;
    private const float shootCoolTime = 5;
    private float shootAttackDuration = 0;

    private float laserShootDuration = 0;
    private const float laserShootCoolTime = 12;
    private float laserBeamDuration = 0;

    [SerializeField] private Animator laserAnimator;
    private BoxCollider2D laserCollider2D;

    [SerializeField] private ParticleSystem whiteShootParticle;
    private Animator animator;

    private static readonly int animShootingHash = Animator.StringToHash("isShooting");

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        laserCollider2D = laserAnimator.GetComponent<BoxCollider2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isShooting = false;
        isLaserShooting = false;
    }

    protected override void LiveUpdate(float deltaTime)
    {
        if (isShooting)
        {
            shootAttackDuration += Time.deltaTime;
            if (shootAttackDuration >= 1)
            {
                isShooting = false;
                shootAttackDuration = 0;
                
                animator.SetBool(animShootingHash, false);

                for (int i = 0; i < 10; i++)
                {
                    var bomb = PoolManager.Instance.Init("WhiteGirlBomb").GetComponent<WhiteGirlBomb>();
                    bomb.OnCreated((transform.position + (Vector3)(Random.insideUnitCircle * 20)).ZChange(), stat);
                }
            }
        }
        else if (isLaserShooting)
        {
            laserBeamDuration += Time.deltaTime;
            if (laserBeamDuration >= 3)
            {
                laserAnimator.gameObject.SetActive(false);
                isLaserShooting = false;
                laserBeamDuration = 0;
            }
            else if (laserBeamDuration >= 2.5f)
            {
                laserAnimator.SetBool(animShootingHash, false);
            }
            else if (laserBeamDuration >= 0.5f)
            {
                var result = new List<Collider2D>();
                Physics2D.OverlapCollider(laserCollider2D, new ContactFilter2D()
                {
                    layerMask = LayerMask.GetMask("Player")
                }, result);

                if (result.Count > 0)
                    foreach (var player in result)
                        if (player.CompareTag("Player"))
                            player.GetComponent<Player>().TakeDamage(stat.damage / 2);
            }
        }
        else
        {
            shootDuration += Time.deltaTime;
            if (shootDuration > shootCoolTime)
            {
                shootDuration -= shootCoolTime;
                isShooting = true;
                animator.SetBool(animShootingHash, true);

                whiteShootParticle.gameObject.SetActive(true);
                whiteShootParticle.Play();
                InGameManager.Instance.CameraShake(0.5f, 0.1f);
                return;
            }

            laserShootDuration += Time.deltaTime;
            if (laserShootDuration > laserShootCoolTime)
            {
                laserShootDuration -= laserShootCoolTime;
                isLaserShooting = true;

                laserAnimator.gameObject.SetActive(true);
                laserAnimator.SetBool(animShootingHash, true);
                InGameManager.Instance.CameraShake(2.5f, 0.2f);
            }
        }
    }

    protected override void Update()
    {
        if (!InGameManager.Instance.isGaming) return;

        if (!Dying)
        {
            LiveUpdate(Time.deltaTime);
            if (!isShooting)
                Move(Time.deltaTime);
        }
    }

    protected override void Move(float deltaTime)
    {
        transform.Translate(stat.speed * deltaTime * (Player.Instance.transform.position - transform.position).normalized);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

        if (Player.Instance.transform.position.x - transform.position.x < 0)
        {
            direction = Direction.Left;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            direction = Direction.Right;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public override void Die()
    {
        base.Die();
        PoolManager.Instance.ActionObjects("WhiteGirlBomb", (obj) =>
        {
            obj.gameObject.SetActive(false);
        });
    }
}