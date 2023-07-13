using UnityEngine;

public class Player_Flip3 : Player
{
    private Animator gunAnimator;
    private static readonly int isShootingHash = Animator.StringToHash("isShooting");

    protected override void Awake()
    {
        gunAnimator = gun.GetComponent<Animator>();
    }
    protected override void Start()
    {
        base.Start();
        AddItem(ResourcesManager.Instance.GetItem("Bp153"));
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