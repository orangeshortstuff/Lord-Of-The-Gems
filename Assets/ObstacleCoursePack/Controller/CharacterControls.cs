using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;


public class CharacterControls : MonoBehaviour
{
    public float speed = 10.0f;
    public float airVelocity = 8f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public float maxFallSpeed = 20.0f;

    private Vector3 moveDir;
    private Rigidbody rb;
    public MouseLook mouseLook = new MouseLook();

    private float distToGround;

    private bool canMove = true; // If player is not hitted
    private bool isStunned = false;
    private bool wasStunned = false; // If player was stunned before getting stunned another time
    private float pushForce;
    private Vector3 pushDir;

    public Vector3 checkPoint;
    private bool slide = false;

    private Camera cam; // Declare the cam variable

    void Start()
    {
        // get the distance to ground
        distToGround = GetComponent<Collider>().bounds.extents.y;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Assign the main camera to the cam variable
        cam = Camera.main;

        // Initialize the mouseLook object
        mouseLook.Init(transform, cam.transform);

        Cursor.visible = false;
    }




    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                Vector3 targetDir = moveDir; //Direction of the character

                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
            }

            if (IsGrounded())
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = moveDir;
                targetVelocity *= speed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
                {
                    targetVelocity = velocity;
                    rb.velocity /= 1.1f;
                }
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                if (!slide)
                {
                    if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                        rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                {
                    rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                    //Debug.Log(rb.velocity.magnitude);
                }

                // Jump
                if (IsGrounded() && Input.GetButton("Jump"))
                {
                    rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                }
            }
            else
            {
                if (!slide)
                {
                    Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
                    Vector3 velocity = rb.velocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                    if (velocity.y < -maxFallSpeed)
                        rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                {
                    rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            rb.velocity = pushDir * pushForce;
        }
        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
    }

    private void Update()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        RotateView(h, v);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        moveDir = cam.transform.TransformDirection(movement).normalized;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
        {
            slide = hit.transform.CompareTag("Slide");
        }
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    private void RotateView(float horizontalInput, float verticalInput)
    {
        mouseLook.LookRotation(transform, cam.transform);

    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        rb.velocity = velocityF;

        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    public void LoadCheckPoint()
    {
        transform.position = checkPoint;
    }

    private IEnumerator Decrease(float value, float duration)
    {
        if (isStunned)
            wasStunned = true;
        isStunned = true;
        canMove = false;

        float delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
            if (!slide) //Reduce the force if the ground isnt slide
            {
                pushForce = pushForce - Time.deltaTime * delta;
                pushForce = pushForce < 0 ? 0 : pushForce;
            }
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0)); // Add gravity
        }

        if (wasStunned)
        {
            wasStunned = false;
        }
        else
        {
            isStunned = false;
            canMove = true;
        }
    }
}
