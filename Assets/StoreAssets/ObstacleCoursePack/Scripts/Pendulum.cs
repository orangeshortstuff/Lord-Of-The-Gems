using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public enum MovementDirection { LeftRight, ForwardBackward }

    public float speed = 1.5f;
    public float limit = 75f; // Limit in degrees of the movement
    public bool randomStart = false; // If you want to modify the start position
    public MovementDirection movementDirection = MovementDirection.LeftRight; // Default to left-right movement
    private float random = 0;

    void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);
    }

    void Update()
    {
        float angle = limit * Mathf.Sin(Time.time * speed + random);

        switch (movementDirection)
        {
            case MovementDirection.LeftRight:
                transform.localRotation = Quaternion.Euler(0, 0, angle); // Swing left-right
                break;
            case MovementDirection.ForwardBackward:
                transform.localRotation = Quaternion.Euler(angle, 0, 0); // Swing forward-backward
                break;
        }
    }
}
