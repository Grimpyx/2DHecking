using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(TargetJoint2D))] 
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerHand : MonoBehaviour
{
    [SerializeField] Player player;
    private TargetJoint2D targetJoint2d;
    private RelativeJoint2D relativeJoint2d;
    private PIDJoint2D jointPID2d;
    private Rigidbody2D rb;
    public float handRange_minimum = 0.01f;
    public float handRange_maximum = 1f;

    //public Holdable currentHoldable;

    //public float maxClamp = 1f;
    //public float smoothTimeRot = 0.2f;
    public float torqueMult = 1f;

    [SerializeField] private PID rotationPID;
    [SerializeField] private PID xPID;
    [SerializeField] private PID yPID;
    public float forcePID = 10f;

    // Start is called before the first frame update
    void Start()
    {
        //player = GetComponentInParent<Player>();
        targetJoint2d = GetComponent<TargetJoint2D>();
        targetJoint2d.autoConfigureTarget = false;
        jointPID2d = GetComponent<PIDJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        //relativeJoint2d = GetComponent<RelativeJoint2D>();

        rotationPID = new PID(20, 0, 1, -99999, 99999, 0, PID.DerivativeMeasurement.Velocity);
        xPID = PID.GetDefaultPID();
        yPID = PID.GetDefaultPID();
    }

    public Transform knockPos1, knockPos2;
    public float forceAmountForKnock = 10f;

    // Update is called once per frame
    void Update()
    {

        if(Keyboard.current.xKey.wasPressedThisFrame)
        {
            player.rb.AddForceAtPosition(knockPos1.position, 100f * forceAmountForKnock * Vector2.right);
            rb.AddForceAtPosition(knockPos1.position, 100f * forceAmountForKnock * Vector2.right);
        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            player.rb.AddForceAtPosition(knockPos2.position, 100f * forceAmountForKnock * Vector2.left);
            rb.AddForceAtPosition(knockPos2.position, 100f * forceAmountForKnock * Vector2.left);
        }
    }

    private void FixedUpdate()
    {
        // Hand position
        //Vector2 aimPos = player.aimWorldPoint;
        /*Vector2 localAimPos = (player.aimWorldPoint - (Vector2)player.transform.position);
        float length = localAimPos.magnitude;
        length = Mathf.Clamp(length, handRange_minimum, handRange_maximum); // clamp length
        Vector2 localHandPos = length * localAimPos.normalized;

        targetJoint2d.target = (Vector2)player.transform.position + localHandPos;*/
        
        Vector2 localAimPos = (player.aimWorldPoint - (Vector2)player.transform.position);
        float length = localAimPos.magnitude;
        length = Mathf.Clamp(length, handRange_minimum, handRange_maximum); // clamp length
        Vector2 localHandPos = length * localAimPos.normalized;

        Vector2 actualTarget = (Vector2)player.transform.position + localHandPos;
        /*targetJoint2d.target = actualTarget;

        Vector2 pid = new(
            xPID.Update(Time.fixedDeltaTime, rb.position.x, actualTarget.x),
            yPID.Update(Time.fixedDeltaTime, rb.position.y, actualTarget.y));

        rb.AddForce(Time.fixedDeltaTime * forcePID * pid);*/

        jointPID2d.linOffset = localHandPos;





        // Hand rotation

        /*float curAngle = transform.rotation.eulerAngles.z;

        float targetAngle = Vector2.SignedAngle(Vector2.up, transform.position - player.transform.position);

        float turk = rotationPID.UpdateAngle(Time.fixedDeltaTime, curAngle, targetAngle);
        rb.AddTorque(torqueMult * turk);*/

        if (player.controls.Hecker.AimButton.IsPressed())
            rb.MoveRotation(Vector2.SignedAngle(Vector2.up, player.aimWorldPoint - (Vector2)transform.position));
            //relativeJoint2d.angularOffset = Vector2.SignedAngle(Vector2.up, player.aimWorldPoint - (Vector2)transform.position);
        else if (!player.controls.Hecker.AimButton.IsPressed())
            rb.MoveRotation(Vector2.SignedAngle(Vector2.up, transform.position - player.transform.position));
            //relativeJoint2d.angularOffset = Vector2.SignedAngle(Vector2.up, transform.position - player.transform.position);*/

        //Debug.DrawLine(transform.position, transform.position + 0.5f * (Vector3)Vector2Help.RotateVector(Vector2.up, targetAngle));
        //Debug.DrawLine(transform.position, transform.position + 0.5f * (Vector3)Vector2Help.RotateVector(Vector2.up, curAngle));



    }
}
