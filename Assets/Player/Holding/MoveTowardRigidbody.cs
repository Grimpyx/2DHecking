using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class MoveTowardRigidbody : MonoBehaviour
{
    public Vector3 worldPosFollow;
    public float dampTime = 0.1f;
    public float force = 1f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (worldPosFollow != null)
        {
            Follow();
        }
    }

    void Follow()
    {
        Vector2 vel = Vector2.zero;
        Vector2.SmoothDamp(transform.position, worldPosFollow, ref vel, dampTime);

        rb.AddForce(force * vel);
    }
}
