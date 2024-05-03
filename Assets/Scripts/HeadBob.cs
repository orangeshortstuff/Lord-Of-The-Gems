using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobbingSpeed = 0.1f; // Speed of the head bobbing
    public float bobbingAmount = 0.05f; // Amount of head bobbing

    private float defaultPosY; // Default Y position of the head
    private float timer = 0f;

    void Start()
    {
        defaultPosY = transform.localPosition.y;
    }

    void Update()
    {
        float waveslice = 0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed;

            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

            // Limit head bobbing when crouching
            if (totalAxes > 0)
            {
                translateChange = Mathf.Clamp(translateChange, -bobbingAmount, bobbingAmount);
            }

            Vector3 newPos = transform.localPosition;
            newPos.y = defaultPosY + translateChange;
            transform.localPosition = newPos;
        }
        else
        {
            Vector3 newPos = transform.localPosition;
            newPos.y = defaultPosY;
            transform.localPosition = newPos;
        }
    }
}
