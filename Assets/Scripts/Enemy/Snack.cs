using UnityEngine;

public class Snack : MonoBehaviour
{
    public float healAmount;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(nameof(Player)))
        {
            gameObject.SetActive(false);
            Player.Instance.TakeHeal(healAmount);
        }
    }
}