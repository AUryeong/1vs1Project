using DG.Tweening;
using UnityEngine;

public class CarBoss : Boss
{
    private bool isRiding;
    [SerializeField] private SpriteRenderer warning;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        isRiding = false;
        warning.gameObject.SetActive(false);
    }

    protected override void LiveUpdate(float deltaTime)
    {
        base.LiveUpdate(deltaTime);
        if (!isRiding)
        {
            isRiding = true;

            direction = (Direction)Random.Range(0, 2);
            spriteRenderer.flipX = direction == Direction.Right;

            warning.DOKill();
            warning.color =new Color(1,0,0,0.25f);
            warning.DOFade(0.1f, 1).SetLoops(-1, LoopType.Yoyo);
            warning.gameObject.SetActive(true);
            
            var playerPos = Player.Instance.transform.position;
            transform.position =  new Vector3(40 * (direction == Direction.Left ? 1 : -1), playerPos.y, playerPos.y);
        }
        else
        {
            if (direction == Direction.Left)
            {
                if (transform.position.x <= -40)
                {
                    isRiding = false;
                }
            }
            else
            {
                if (transform.position.x >= 40)
                {
                    isRiding = false;
                }
            }
        }
    }

    protected override void Move(float deltaTime)
    {
        if (isRiding)
        {
            transform.Translate(stat.speed * deltaTime * (direction == Direction.Left ? Vector3.left : Vector3.right));
            warning.transform.position = new Vector3(0, transform.position.y-0.6f, transform.position.z);
        }
    }

}