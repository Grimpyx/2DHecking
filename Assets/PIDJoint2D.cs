using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDJoint2D : MonoBehaviour
{
    public Transform followedBody;
    public Vector2 linOffset;
    public bool adoptLinearVelocity = true;
    public bool adoptAngularVelocity = true;

    [Header("Linear:")]
    public bool followPosition = true;
    [SerializeField] PID xPID;
    [SerializeField] PID yPID;
    public float forcePID = 1f;

    [Header("Angular:")]
    public bool followRotation = true;
    [SerializeField] PID rotPID;
    public float forceRotPID = 1f;


    private Rigidbody2D rb;
    private Rigidbody2D rbTarget;


    void Start()
    {
        /*xPID = PID.GetDefaultPID();
        xPID.proportionalGain = 50;
        xPID.integralGain = 2;
        xPID.integralSaturation = 1;

        yPID = PID.GetDefaultPID();
        yPID.proportionalGain = 50;
        yPID.integralGain = 2;
        yPID.integralSaturation = 1;*/

        rb = GetComponent<Rigidbody2D>();
        rbTarget = followedBody.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (adoptLinearVelocity)
            rb.velocity = rbTarget.velocity;

        if (adoptAngularVelocity)
            rb.angularVelocity = rbTarget.angularVelocity;

        if (followPosition)
        {
            float x = xPID.Update(Time.fixedDeltaTime, transform.position.x, followedBody.position.x + linOffset.x);
            float y = yPID.Update(Time.fixedDeltaTime, transform.position.y, followedBody.position.y + linOffset.y);

            rb.AddForce(rb.mass * forcePID * new Vector2(x, y), ForceMode2D.Force);
        }

        if(followRotation)
        {
            float angle = rotPID.UpdateAngle(Time.fixedDeltaTime, rb.rotation, rbTarget.rotation);

            rb.AddTorque(rb.mass * forceRotPID * angle, ForceMode2D.Force);
        }
    }
}
