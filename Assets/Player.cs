using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Space(5)] [SerializeField] private bool debug = true;
    private Hooker hooker;

    [SerializeField] private float groundedRange;
    public Vector2 localGroundedDirection;
    private float cooldownMax_Grounded = 0.1f;
    private float cooldownCurrent_Grounded = 0f;
    private float groundedAngleTolerance = 25f;
    public float groundedGravityScale = 1f;
    private Rigidbody2D rb;

    [Space(5)]
    [Header("Movement:")]
    public bool autoJump = true;
    [Space(5)]
    public float maxSpeedGround = 5f;
    public float moveAcceleration;
    [Space(5)]
    public float maxSpeedAir = 10f;
    public float airMoveMultiplier = 0.5f;
    [Space(5)] 
    public float velocitySlowMoveThreshold = 0.1f;
    public float velocitySlowMoveMultiplier = 4f;
    [Space(3)]
    public float dampenForce = 1;
    public float velocityThresholdDampen = 1;
    [Space(3)]
    public float jumpAcceleration;

    [Space(5)] [Header("Inputs:")] [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference hookAction;
    [SerializeField] private InputActionReference moveAction;
    public Controls controls;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        hooker = GetComponent<Hooker>();
    }

    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();
        controls.Hecker.Jump.performed += Jump;
        controls.Hecker.Hook.performed += Hook;
    }

    private void OnDisable()
    {
        controls.Hecker.Jump.performed -= Jump;
        controls.Hecker.Hook.performed -= Hook;
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -15)
            transform.position = Vector3.zero;

        if (Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        /*if (Keyboard.current.spaceKey.isPressed)
        {
            float angle = 45;
            Vector2 v = RotateVector(Vector2.up, angle);
            print("Vector: " + v);

            angle *= Mathf.Deg2Rad;
            //float[,] rotMatrix = new float[2, 2] { { Mathf.Cos(angle), -Mathf.Sin(angle) }, { Mathf.Sin(angle), Mathf.Cos(angle) } };
            //print("(0,0): " + rotMatrix[0, 0]);
            //print("(1,0): " + rotMatrix[1, 0]);
            //print("(0,1): " + rotMatrix[0, 1]);
            //print("(1,1): " + rotMatrix[1, 1]);


        }*/

        if (cooldownCurrent_Grounded > 0)
        {
            cooldownCurrent_Grounded -= Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        CastGroundedRays();
        GroundedMechanics();
        Move();
        VelocityDampen();
    }

    private void VelocityDampen()
    {
        if(localGroundedDirection != Vector2.zero) // Whilst grounded
        {
            Vector2 negVelDir = -rb.velocity.normalized;
            rb.AddForce(dampenForce * rb.mass * negVelDir, ForceMode2D.Impulse);
        }
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (localGroundedDirection != Vector2.zero) // && (ctx.action.WasPressedThisFrame() || (ctx.action.IsPressed() && autoJump)))
        {
            //float moveInputX = controls.Hecker.Move.ReadValue<Vector2>().x; // [-1, 1]
            //float angleModify = moveInputX * 30f; // [-30,30]
            //angleModify *= Mathf.Sign(Vector2.Dot(-localGroundedDirection, Vector2.up)); // Multiply by -1 if jump is negative
            Vector2 jumpDir = -localGroundedDirection;

            Vector2 moveInput = controls.Hecker.Move.ReadValue<Vector2>();
            float angleModify = Mathf.Clamp(Vector2.SignedAngle(moveInput, jumpDir), -20, 20); // Worked without this: if (Vector2.Dot(moveInput, jumpDir) < 0) angleModify *= -1; // If we're upside down
            if (angleModify != 0) jumpDir = Vector2Help.RotateVector(-localGroundedDirection, angleModify); // Rotate jump vector

            if (debug) Debug.DrawLine(transform.position, transform.position + (Vector3)jumpDir, Color.red, 2f);

            rb.AddForce(jumpAcceleration * rb.mass * jumpDir, ForceMode2D.Impulse);
            localGroundedDirection = Vector2.zero;
            cooldownCurrent_Grounded = cooldownMax_Grounded;
        }
    }
    private void Hook(InputAction.CallbackContext ctx)
    {
        hooker.Hook();
    }
    private void Move()
    {
        Vector2 input = controls.Hecker.Move.ReadValue<Vector2>();
        if (input == Vector2.zero) return;


        if (localGroundedDirection == Vector2.zero) // mid air
        {
            // If max speed
            // if velocity is above max speed, then zero the composant in the velocity direction
            if (rb.velocity.magnitude > maxSpeedAir && Vector2.Angle(rb.velocity, input) < 90)
                input -= Vector2Help.ProjectVector(input, rb.velocity.normalized);

            Vector2 force = 0.01f * airMoveMultiplier * moveAcceleration * rb.mass * input;
            if (rb.velocity.magnitude < velocitySlowMoveThreshold) force *= velocitySlowMoveMultiplier;
            force.y -= Mathf.Clamp(rb.gravityScale, 0, 1) * force.y;
            rb.AddForce(force, ForceMode2D.Impulse);

        }
        else // When attached to the ground
        {
            // If max speed
            // if velocity is above max speed, then zero the composant in the velocity direction
            if (rb.velocity.magnitude > maxSpeedGround && Vector2.Angle(rb.velocity, input) < 90)
                input -= Vector2Help.ProjectVector(input, rb.velocity.normalized);

            Vector2 moveDir = input - Vector2Help.ProjectVector(input, localGroundedDirection);

            Vector2 force = 0.01f * moveAcceleration * rb.mass * moveDir;
            if (rb.velocity.magnitude < velocitySlowMoveThreshold) force *= velocitySlowMoveMultiplier;

            if (debug) Debug.DrawLine(transform.position, transform.position + (Vector3)Vector2Help.ProjectVector(input, -localGroundedDirection), Color.green);
            if (debug) Debug.DrawLine(transform.position, transform.position + (Vector3)moveDir, Color.blue);

            // Should only move perpendicular to the point of "gravity"
            rb.AddForce(force, ForceMode2D.Impulse);

        }
    }

    private void GroundedMechanics()
    {
        if (localGroundedDirection != Vector2.zero) // meaning player IS GROUNDED
        {
            rb.gravityScale = 0;

            // Fake gravity
            rb.AddForce((groundedGravityScale * 9.81f * rb.mass) * localGroundedDirection, ForceMode2D.Force);
        }
        else
        {
            rb.gravityScale = 1;
        }

    }
    private void CastGroundedRays()
    {
        if (cooldownCurrent_Grounded > 0) return; // Return if grounded is on cooldown

        localGroundedDirection = Vector2.zero;

        //RaycastHit hitResult = null;
        for (int i = 0; i < 24; i++)
        {
            Vector2 rayDir = Vector2Help.RotateVector(Vector2.up, 15*i);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, groundedRange, LayerMask.GetMask("Terrain"));

            if (hit.collider != null) // (ray, out RaycastHit hit, groundedRange, mask, QueryTriggerInteraction.Ignore))
            {
                localGroundedDirection += rayDir;

                /*if (Vector2.Angle(-hit.normal, rayDir) < groundedAngleTolerance)
                {
                    localGroundedDirection = -hit.normal;

                    if (debug) Debug.DrawLine(transform.position, transform.position + groundedRange * (Vector3)rayDir, Color.yellow);
                }
                else
                {
                    if (debug) Debug.DrawLine(transform.position, transform.position + groundedRange * (Vector3)rayDir, Color.red);
                }*/

                if (debug) Debug.DrawLine(transform.position, transform.position + groundedRange * (Vector3)rayDir, Color.yellow);
            }
            else
            {
                if (debug) Debug.DrawLine(transform.position, transform.position + 0.45f * (Vector3)rayDir, Color.white);
            }
        }
        localGroundedDirection.Normalize();
        if (localGroundedDirection != Vector2.zero && debug) Debug.DrawLine(transform.position, transform.position + (Vector3)localGroundedDirection, Color.green);
    }
}
