using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //components
    private Rigidbody rb;
    private Animator animator;
    [SerializeField]
    private ParticleSystem ps;
    private CapsuleCollider m_collider;
    [SerializeField]
    private Camera m_camera;

    //properties
    [SerializeField]
    private float speed = 4f;
    [SerializeField]
    private float gravityOffset = 0f;
    [SerializeField]
    private float jumpSpeed = 12f;
    [SerializeField]
    private float dashDuration = 0.5f;

    //states
    private bool canJump;
    private bool jumped;
    private bool onGround;
    private bool dashed;
    private bool groundDash;
    private bool airDash;
    private bool onSlope;

    //Data
    private Vector3 movementAxis;
    private Vector3 cacheMovementDirection;
    private Vector3 groundNormal;
    private Vector3 wallNormal;
    private float wallAngle;
    private Quaternion slopeRotation;
    private Vector3 transformOffset;
    private float dashTime;
    private bool dashUpdate;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        m_collider = GetComponent<CapsuleCollider>();

        //Initial State
        canJump = true;
        jumped = false;
    }

    //Debug
    void OnGUI()
    {
        GUILayout.Label("velocity: " + rb.velocity.ToString());
        GUILayout.Label("movementAxis: " + movementAxis.ToString());
        GUILayout.Label("canJump: " + canJump.ToString());
        GUILayout.Label("jumped: " + jumped.ToString());
        GUILayout.Label("onGround: " + onGround.ToString());
        GUILayout.Label("groundDash: " + groundDash.ToString());
        GUILayout.Label("airDash: " + airDash.ToString());
        GUILayout.Label("wallAngle: " + wallAngle.ToString());
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && canJump)
        {
            jumped = true;
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (!onGround)
            {
                airDash = true;
                groundDash = false;
            }

            else
            {
                groundDash = true;
                airDash = false;
            }

            dashUpdate = true;
            cacheMovementDirection = movementAxis.normalized;
        }

        if (dashUpdate)
        {
            if (dashTime <= dashDuration && !(airDash && onGround))
            {
                dashTime += Time.deltaTime;
                dashed = true;
            }

            else
            {
                if (onGround)
                {
                    dashTime = 0;
                    dashUpdate = false;
                    dashed = false;
                }

                if (onSlope)
                {
                    dashed = false;
                }
                
                airDash = false;
            }
        }
    }

    void Jump()
    {
        animator.SetTrigger("Jump");
        rb.velocity = new Vector3(0, jumpSpeed, 0);
        ps.Emit(10);
        jumped = false;
        canJump = false;
    }

    void Dash()
    {
        if (airDash)
        {
            //airdash
            rb.velocity = cacheMovementDirection * 12;
            rb.useGravity = false;
        }

        else
        {
            //ground dash
            Vector3 cacheVelocity = rb.velocity;
            cacheVelocity *= 2;
            cacheVelocity.y = rb.velocity.y;
            rb.velocity = cacheVelocity;
        }
    }

    void FixedUpdate()
    {
        rb.useGravity = true;
        //First, we need to get the input from the joystick/kb
        movementAxis = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movementAxis = movementAxis.normalized;

        //then, multiply it by our speed magnitude
        movementAxis *= speed;

        //Check if we're touching the ground
        float rayLength = 0.55f;
        transformOffset = transform.position;
        transformOffset.y -= 0.4f;

        Vector3 camRef = m_camera.transform.forward;
        camRef.y = 0;
        camRef = camRef.normalized;
        
        Quaternion cameraShift = Quaternion.FromToRotation(Vector3.forward, camRef);
        movementAxis = cameraShift * movementAxis;

        float playerAngle = Vector3.SignedAngle(Vector3.forward, movementAxis, Vector3.up);
        Quaternion playerShift = Quaternion.AngleAxis(playerAngle, Vector3.up);
        
        if(movementAxis.magnitude > 0.05f)
        transform.rotation = playerShift;


        RaycastHit groundHit;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down * rayLength), Color.cyan);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out groundHit, rayLength))
        {
            //you're on ground babyyyy

            rb.useGravity = false;
            canJump = true;

            onGround = true;
            onSlope = false;

            groundNormal = groundHit.normal;
            slopeRotation = Quaternion.FromToRotation(transform.up, groundNormal);
            Debug.DrawRay(groundHit.point, groundNormal, Color.magenta);
            movementAxis = slopeRotation * movementAxis;

            Vector3 groundVelocity = movementAxis;

            // if (groundDash && !dashed && onGround)
            // {
            //     groundDash = false;
            // }

            RaycastHit wallHit;
            Debug.DrawRay(transform.position, movementAxis.normalized * rayLength, Color.yellow);
            if (Physics.Raycast(transform.position, movementAxis.normalized, out wallHit, rayLength))
            {
                Debug.Log("hit wall");
                groundVelocity = Vector3.zero;
            }


            rb.velocity = groundVelocity;
        }

        else
        {
            onGround = false; //you're on air babyyyy

            RaycastHit slopeHit;
            Debug.DrawRay(transformOffset, movementAxis.normalized * rayLength * 0.7f, Color.green);
            if (Physics.Raycast(transformOffset, movementAxis.normalized * rayLength * 0.7f, out slopeHit, rayLength))
            {
                wallNormal = slopeHit.normal;
                wallAngle = Vector3.Angle(wallNormal, Vector3.up);
                if (wallAngle <= 75)
                {

                }

                onSlope = true;
            }

            else
            {
                Vector3 airVelocity = new Vector3(movementAxis.x, rb.velocity.y, movementAxis.z);
                rb.velocity = airVelocity;
            }
        }

        if (!onGround && !airDash)
        {
            rb.AddForce(new Vector3(0, gravityOffset, 0)); //Local Gravity adjustment for individual tweaks (on air)
        }


        //apply dash
        if (dashed)
        {
            Dash();
        }

        //apply jump
        if (jumped)
        {
            Jump();
        }






    }
}
