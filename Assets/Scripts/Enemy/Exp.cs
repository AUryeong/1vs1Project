using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    public float exp = 0;
    private bool isGot = false;
    private float gettingSpeed = 6;
    private float gettingDistance = 0.7f;

    private void OnEnable()
    {
        isGot = false;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        Move(deltaTime);
    }

    private void Move(float deltaTime)
    {
        if (isGot)
        {
            transform.Translate(deltaTime * gettingSpeed * (Player.Instance.transform.position - transform.position).normalized);

            if (Vector3.Distance(transform.position, Player.Instance.transform.position) < gettingDistance)
            {
                gameObject.SetActive(false);
                SoundManager.Instance.PlaySound("exp", SoundType.SE, 2f, 2f);
                Player.Instance.Exp += exp;
            }
        }
    }

    public void OnGet()
    {
        if (!isGot)
            isGot = true;
    }
}
