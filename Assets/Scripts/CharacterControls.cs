using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using UnityEditor;
using UnityEngine.UI;

public class CharacterControls : MonoBehaviour
{
    public float speed = 5.0f;
    public float runSpeed = 10.0f;
    public float airVelocity = 8f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public float maxFallSpeed = 20.0f;
    public float standingHeight = 2f; // Height of the character when standing
    public float crouchingHeight = 1f; // Height of the character when crouching
    public float crouchingSpeed = 2.5f;
    public float crouchSpeedMultiplier = 0.5f; // Movement speed multiplier when crouching
    public float sprintSpeed = 10.0f; // Sprinting movement speed
    public float crouchedSprintSpeed = 5.0f; // Sprinting movement speed when crouched
    public Slider staminaSlider;

    private bool isCrouching = false;
    private float originalHeight;

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

    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f; // Stamina regenerated per second
    public float sprintStaminaCost = 25f; // Stamina cost per second while sprinting
    private bool isSprinting = false;


    void Start()
    {
        // get the distance to ground
        distToGround = GetComponent<Collider>().bounds.extents.y;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Assign the main camera to the cam variable
        cam = Camera.main;
        originalHeight = transform.localScale.y;

        // Initialize the mouseLook object
        mouseLook.Init(transform, cam.transform);

        // Get reference to AudioSource component
        audioSource = GetComponent<AudioSource>();

        Cursor.visible = false;

        currentStamina = maxStamina;
        staminaSlider = GameObject.Find("Stamina").GetComponent<Slider>();
        staminaSlider.maxValue = maxStamina;
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
        UpdateStaminaSlider();

        // Input handling for mouse movement
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        RotateView(h, v);

        // Input handling for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        moveDir = cam.transform.TransformDirection(movement).normalized;

        // Check if the "C" key is held down
        if (Input.GetKey(KeyCode.C))
        {
            StartCrouch();
        }
        else
        {
            StopCrouch();
        }

        
        // Stamina regeneration
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
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

        // Sprinting
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentStamina > 0)
        {
            SetSprint(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || currentStamina <= 0)
        {
            SetSprint(false);
        }

        // Stamina regeneration
        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    void UpdateStaminaSlider()
    {
        // Update the stamina slider value
        staminaSlider.value = currentStamina;
    }

    void StartCrouch()
    {
        if (!isCrouching)
        {
            isCrouching = true;

            // Reduce the height of the character
            transform.localScale = new Vector3(transform.localScale.x, crouchingHeight, transform.localScale.z);

            // Optionally adjust camera position if needed
            // For example, lower the camera position to match the new height

            // Optionally adjust movement speed
            // For example, reduce movement speed when crouching
            speed *= crouchSpeedMultiplier;
        }
    }
    void StopCrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;

            // Restore the height of the character
            transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);

            // Optionally adjust camera position if needed
            // For example, raise the camera position to match the standing height

            // Optionally restore movement speed
            // For example, restore the original movement speed
            speed /= crouchSpeedMultiplier;
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

    void SetSprint(bool sprint)
    {
        isSprinting = sprint;
        if (sprint)
        {
            StartCoroutine(Sprint());
        }
    }

    IEnumerator Sprint()
    {
        float sprintSpeedToUse = isCrouching ? crouchedSprintSpeed : runSpeed;

        while (isSprinting && currentStamina > 0)
        {
            speed = sprintSpeedToUse;
            currentStamina -= sprintStaminaCost * Time.deltaTime;
            yield return null;
        }

        speed = isCrouching ? crouchingSpeed : 5.0f;
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
