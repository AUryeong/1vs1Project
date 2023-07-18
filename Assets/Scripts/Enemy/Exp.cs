using UnityEngine;

public class Exp : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    
    public float exp = 0;
    private bool isGot = false;
    private float duration;

    private const float getDuration = 0.75f;
    
    private Vector3 startPos;
    private Vector3 middlePos;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        isGot = false;
        duration = 0;
        
        boxCollider2D.size = 3 * Player.Instance.xpGetRadius / 100f * Vector2.one;
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
