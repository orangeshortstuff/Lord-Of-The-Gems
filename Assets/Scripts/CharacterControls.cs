using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterControls : MonoBehaviour
{
    public float speed = 5.0f;
    public float airVelocity = 8f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public float maxFallSpeed = 20.0f;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchingSpeed = 2.5f;
    public float walkSpeed = 5.0f;
    public float crouchSpeedMultiplier = 0.5f;
    public float sprintSpeed = 10.0f;
    public float crouchedSprintSpeed = 5.0f;
    public Slider staminaSlider;

    private bool isCrouching = false;
    private float originalHeight;

    private Vector3 moveDir;
    private Rigidbody rb;
    public MouseLook mouseLook = new MouseLook();

    private float distToGround;

    private bool canMove = true;
    private bool isStunned = false;
    private bool wasStunned = false;
    private float pushForce;
    private Vector3 pushDir;

    public Vector3 checkPoint;
    private bool slide = false;

    private Camera cam;

    // Audio variables
    public AudioClip[] footstepSounds;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.25f;
    private float stepInterval;
    private float nextStepTime = 0f;
    public AudioClip jumpSound;
    public AudioClip landSound;
    private AudioSource audioSource;

    private bool isWalking;
    private bool wasGrounded;

    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f;
    public float sprintStaminaCost = 25f;
    private bool isSprinting = false;
    private PowerupManager pm;

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cam = Camera.main;
        originalHeight = transform.localScale.y;

        mouseLook.Init(transform, cam.transform);

        audioSource = GetComponent<AudioSource>();

        Cursor.visible = false;

        currentStamina = maxStamina;
        staminaSlider = GameObject.Find("Stamina").GetComponent<Slider>();
        staminaSlider.maxValue = maxStamina;

        pm = GetComponent<PowerupManager>();
        walkSpeed = speed;

        // Initialize the step interval to walking step interval
        stepInterval = walkStepInterval;
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
                Vector3 targetDir = moveDir;
                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion tr = Quaternion.LookRotation(targetDir);
            }

            if (IsGrounded())
            {
                Vector3 targetVelocity = moveDir;
                targetVelocity *= speed;

                Vector3 velocity = rb.velocity;
                if (targetVelocity.magnitude < velocity.magnitude)
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
                }
            }
            else
            {
                if (!slide)
                {
                    Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity * pm.speedMultiplier, rb.velocity.y, moveDir.z * airVelocity * pm.speedMultiplier);
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
        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
    }

    private void Update()
    {
        UpdateStaminaSlider();

        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");
        RotateView(h, v);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        moveDir = cam.transform.TransformDirection(movement).normalized;

        if (Input.GetKey(KeyCode.C))
        {
            StartCrouch();
        }
        else
        {
            StopCrouch();
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
            audioSource.PlayOneShot(jumpSound);
        }

        if (moveHorizontal == 0 && moveVertical == 0)
        {
            moveDir = Vector3.zero;
        }

        isWalking = moveHorizontal != 0 || moveVertical != 0;

        if (isWalking && IsGrounded() && Time.time > nextStepTime)
        {
            PlayFootstepAudio();
            nextStepTime = Time.time + stepInterval;
        }

        if (IsGrounded() && !wasGrounded)
        {
            audioSource.PlayOneShot(landSound);
        }

        wasGrounded = IsGrounded();

        if (Input.GetKeyDown(KeyCode.LeftShift) && currentStamina > 0)
        {
            SetSprint(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || currentStamina <= 0)
        {
            SetSprint(false);
        }

        if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        if (Input.GetKeyDown(KeyCode.E) && pm.state == PowerupManager.PowerupState.Inactive)
        {
            pm.state = PowerupManager.PowerupState.Active;
            pm.timeLeft = pm.duration;
        }

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    void UpdateStaminaSlider()
    {
        staminaSlider.value = currentStamina;
    }

    void StartCrouch()
    {
        if (!isCrouching)
        {
            isCrouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchingHeight, transform.localScale.z);
            speed *= crouchSpeedMultiplier;
        }
    }

    void StopCrouch()
    {
        if (isCrouching)
        {
            isCrouching = false;
            transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);
            speed /= crouchSpeedMultiplier;
        }
    }

    void PlayFootstepAudio()
    {
        if (footstepSounds.Length == 0)
            return;

        int index = Random.Range(0, footstepSounds.Length);
        audioSource.clip = footstepSounds[index];
        audioSource.PlayOneShot(audioSource.clip);
    }

    void SetSprint(bool sprint)
    {
        isSprinting = sprint;
        stepInterval = sprint ? sprintStepInterval : walkStepInterval;
        if (sprint)
        {
            StartCoroutine(Sprint());
        }
    }

    IEnumerator Sprint()
    {
        float sprintSpeedToUse = isCrouching ? crouchedSprintSpeed : sprintSpeed;

        while (isSprinting && currentStamina > 0)
        {
            speed = sprintSpeedToUse * pm.speedMultiplier;
            if (!pm.freeSprint)
            {
                currentStamina -= sprintStaminaCost * Time.deltaTime;
            }
            yield return null;
        }

        speed = isCrouching ? crouchingSpeed * pm.speedMultiplier : walkSpeed * pm.speedMultiplier;
    }

    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * jumpHeight * pm.heightMultiplier * gravity);
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
            if (!slide)
            {
                pushForce = pushForce - Time.deltaTime * delta;
                pushForce = pushForce < 0 ? 0 : pushForce;
            }
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
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
    bool IsNearWall(out Vector3 hitNormal)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, 1.0f))
        {
            if (hit.collider.CompareTag("WallRun")) // Check if the collider belongs to a wall with the "WallRun" tag
            {
                hitNormal = hit.normal;
                return true;
            }
        }
        hitNormal = Vector3.zero;
        return false;
    }
}
