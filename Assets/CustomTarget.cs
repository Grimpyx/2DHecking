using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class CustomTarget : MonoBehaviour
{
    Rigidbody2D rb;

    public Transform follow;
    private Rigidbody2D rbFollow;

    public float linearForce;
    [SerializeField] PID linPID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        linPID = PID.GetDefaultPID();
    }

    private void FixedUpdate()
    {
        rbFollow = rbFollow == null ? follow.GetComponent<Rigidbody2D>() : rbFollow;

        //Vector2 dir = 
        //rb.AddForce(linearForce);

        rb.position = follow.position;
        rb.rotation = follow.rotation.eulerAngles.z;
        rb.velocity = rbFollow.velocity;
        rb.angularVelocity = rbFollow.angularVelocity;
    }
}
