using UnityEngine;

public class Campas : Projectile
{
    private float duration = 0;
    private const float moveDuration = 3;

    private int penetrateCount;
    private Vector3 startPos;
    private Vector3 middlePos;
    private Vector3 endPos;
    private const int defaultPenetrateCount = 5;


    public override void OnHit(Enemy enemy)
    {
        penetrateCount--;
        if (penetrateCount <= 0)
            gameObject.SetActive(false);
    }

    public void OnCreate(float angle, Vector3 size)
    {
        duration = 0;
        isHitable = true;
        penetrateCount = defaultPenetrateCount;

        transform.localScale = size;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        var pos = Player.Instance.transform.position;
        transform.position = pos;

        startPos = pos;
        middlePos = pos + transform.up * 10;
        endPos = pos - transform.up * 30;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        duration += Time.deltaTime;
        transform.Rotate(0,0, 500 *Time.deltaTime);
        transform.position = Utility.Beizer(startPos, middlePos, endPos, duration / moveDuration);
        if (duration >= moveDuration)
        {
            gameObject.SetActive(false);
        }
    }

}