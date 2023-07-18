using UnityEngine;

public class Player_Plus3000 : Player
{
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.GetItem("Bp153"));
    }

    private Animator gunAnimator;
    private static readonly int isShootingHash = Animator.StringToHash("isShooting");

    protected override void Awake()
    {
        base.Awake();
        gunAnimator = gun.GetComponent<Animator>();
    }

    protected override void ShootUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            gunAnimator.SetBool(isShootingHash, true);

        if (Input.GetMouseButtonUp(0))
            gunAnimator.SetBool(isShootingHash, false);

        base.ShootUpdate();
    }
}