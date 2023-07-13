using UnityEngine;

public class Exp : MonoBehaviour
{
    public float exp = 0;
    private bool isGot = false;
    private float duration;

    private const float getDuration = 0.75f;
    
    private Vector3 startPos;
    private Vector3 middlePos;

    private void OnEnable()
    {
        isGot = false;
        duration = 0;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!isGot) return;

        duration += Time.deltaTime;
        transform.position = Utility.Beizer(startPos, middlePos, Player.Instance.transform.position, duration / getDuration);
        
        if (!(duration >= getDuration)) return;
        
        gameObject.SetActive(false);
        SoundManager.Instance.PlaySound("exp", SoundType.Se, 2f, 2f);
        Player.Instance.Exp += exp;
    }

    public void OnGet()
    {
        if (isGot) return;
        
        isGot = true;
            
        startPos = transform.position;
        middlePos = startPos + (Vector3)Random.insideUnitCircle * Random.Range(1, 4.5f);
    }
}
