using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using UnityEditor;

public class CharacterControls : MonoBehaviour
{
    public float speed = 5.0f;
    public float runSpeed = 10.0f;
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

    // Audio variables
    public AudioClip[] footstepSounds;
    // Step interval variables
    public float stepInterval = 0.5f; 
    private float nextStepTime = 0f;
    public AudioClip jumpSound;
    public AudioClip landSound;
    private AudioSource audioSource;

    private bool isWalking;

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

        // Get reference to AudioSource component
        audioSource = GetComponent<AudioSource>();

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
        // Input handling for mouse movement
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        RotateView(h, v);

        // Input handling for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        moveDir = cam.transform.TransformDirection(movement).normalized;

        // Check if sprint key is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetSprint();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 5.0f;
        }

        // Check if character is on a slide
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, GetComponent<Collider>().bounds.extents.y + 0.1f))
        {
            slide = hit.transform.CompareTag("Slide");
        }

        // If there's no movement input, reset the moveDir vector to zero
        if (moveHorizontal == 0 && moveVertical == 0)
        {
            moveDir = Vector3.zero;
        }

        // Play footstep sounds
        if (isWalking && IsGrounded() && Time.time > nextStepTime)
        {
            PlayFootstepAudio();
            nextStepTime = Time.time + stepInterval; // Update next step time
        }
    }

    // Play footstep sounds
    void PlayFootstepAudio()
    {
        if (footstepSounds.Length == 0)
            return;

        // Randomly select a footstep sound from the array
        int index = Random.Range(0, footstepSounds.Length);
        audioSource.clip = footstepSounds[index];
        audioSource.PlayOneShot(audioSource.clip);
    }

    void SetSprint()
    {
        speed = runSpeed;
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
