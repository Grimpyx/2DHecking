using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hooker : MonoBehaviour
{
    public enum AimType { mouse, joystick }
    public AimType aimInputType = AimType.mouse;

    [Space(5)] public float hookForce = 10f;
    public float wrongDirectionMultiplier = 3f;
    public bool stiff = false;
    private float initialRopeLength = 0f;
    public float ropeLengthenSpeed = 0.03f;
    public float ropeJumpSpeedGain = 0.5f;

    private Player player;

    private Vector2 attachedPoint;
    /*public Vector2 AttachedPoint
    {
        get => attachedPoint;
        set
        {
            attachedPoint = value;

        }
    }*/

    private bool isHooked = false;
    //private Rigidbody2D hookedBody;
    //private Vector2 rotationPoint;

    private LineRenderer ropeRenderer;
    //private DistanceJoint2D ropeJoint;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        ropeRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        if(isHooked)
        {
            // Set rope visuals
            ropeRenderer.enabled = true;
            ropeRenderer.positionCount = 2;
            Vector3[] positions = new Vector3[2] { transform.position, attachedPoint };
            ropeRenderer.SetPositions(positions);
        }
    }

    private void FixedUpdate()
    {
        if (isHooked)
        {
            // Pull in slightly if you press space whilst hooked (velocity direction change, and slight boost)
            if(player.controls.Hecker.Jump.IsPressed())
            {
                isHooked = false;
                Debug.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized, Color.blue, 2f);
                float alpha = Mathf.Clamp(Vector2.SignedAngle((attachedPoint - (Vector2)transform.position).normalized, rb.velocity.normalized), -10, 10);
                rb.velocity = Vector2Help.RotateVector(rb.velocity + ropeJumpSpeedGain * rb.velocity.normalized, alpha);
                Debug.DrawLine(transform.position, transform.position + (Vector3)rb.velocity.normalized, Color.red, 2f);
            }

            Vector2 dir = (attachedPoint - (Vector2)transform.position).normalized;

            // Apply force
            float velDirAngle = Vector2.Angle(rb.velocity.normalized, dir);
            if (stiff)
            {
                // Stop rope from extending beyond it's "length"
                Vector2 relativePosToAttach = (Vector2)transform.position - attachedPoint;
                if (relativePosToAttach.magnitude > initialRopeLength)
                {
                    rb.position = attachedPoint + initialRopeLength * relativePosToAttach.normalized;

                    rb.velocity -= Vector2Help.ProjectVector(rb.velocity, dir);
                }

                // pull in or go down
                Vector2 moveInput = player.controls.Hecker.Move.ReadValue<Vector2>();
                if(moveInput.y != 0)
                {
                    if (moveInput.y > 0)
                    {
                        Vector2 force = moveInput.y * 0.001f * hookForce * 9.81f * 2f * rb.mass * dir;
                        rb.AddForce(force, ForceMode2D.Impulse);
                        initialRopeLength = relativePosToAttach.magnitude;
                    }
                    else if (moveInput.y < 0)
                    {
                        initialRopeLength += -moveInput.y * ropeLengthenSpeed;
                    }

                }
            }
            else
            {
                float forceMult = 1f;
                if (velDirAngle > 90)
                    forceMult = wrongDirectionMultiplier;
                rb.AddForce(0.001f * forceMult * hookForce * 9.81f * 2f * rb.mass * dir, ForceMode2D.Impulse);
            }


            //if(moving away from point, high force)

            //if(moving toward point, low force)
            // rb.AddForce(0.001f * hookForce * 9.81f * 2f * rb.mass * dir, ForceMode2D.Impulse);

        }
        else
        {
            ropeRenderer.enabled = false;
        }
    }

    public void Hook()
    {
        if (isHooked)
        {
            isHooked = false;
            ropeRenderer.enabled = true;
            return;
        }

        Vector2 aimInput = player.controls.Hecker.Aim.ReadValue<Vector2>();
        Vector2 aimWorldPoint = Camera.main.ScreenToWorldPoint(aimInput);

        Vector3 aimDirection = Vector3.zero;
        switch (aimInputType)
        {
            case AimType.mouse:
                aimDirection = (aimWorldPoint - (Vector2)transform.position).normalized;
                break;
            case AimType.joystick:
                aimDirection = aimWorldPoint.normalized; // not tried and tested
                break;
            default:
                break;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 15f, LayerMask.GetMask("Terrain"));
        
        if(hit)
        {
            isHooked = true;
            //ropeJoint = gameObject.AddComponent<FixedJoint2D>();
            attachedPoint = hit.point;
            //rotationPoint = hit.point;
            //hookedBody = hit.rigidbody;
            initialRopeLength = (attachedPoint - (Vector2)transform.position).magnitude;
        }

        //Ray ray = Camera.current.ScreenPointToRay(Mouse.current.position.ReadValue());
        //Vector2 mouseInput = new Vector2(ray.origin.x, ray.origin.y);

        //Vector2 mouseInput = Camera.current.ScreenToViewportPoint(Mouse.current.position.ReadValue());


        //print("Mousepos: " + mouseInput);
    }
}
