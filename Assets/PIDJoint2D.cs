using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDJoint2D : MonoBehaviour
{
    public Transform followedBody;
    public Vector2 linOffset;


    [SerializeField] PID xPID, yPID;
    public float forcePID = 10f;


    public bool followPosition, followRotation;


    private Rigidbody2D rb;


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
    }

    private void FixedUpdate()
    {
        if(followPosition)
        {
            float x = xPID.Update(Time.fixedDeltaTime, transform.position.x, followedBody.position.x + linOffset.x);
            float y = yPID.Update(Time.fixedDeltaTime, transform.position.y, followedBody.position.y + linOffset.y);

            rb.AddForce(Time.fixedDeltaTime * 0.01f * forcePID * new Vector2(x, y));
        }

        if(followRotation)
        {

        }
    }
}
