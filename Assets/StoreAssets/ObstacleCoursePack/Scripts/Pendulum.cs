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

    // Start is called before the first frame update
    void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = limit * Mathf.Sin(Time.time * speed + random);
        Vector3 rotationAxis;

        switch (movementDirection)
        {
            case MovementDirection.LeftRight:
                rotationAxis = new Vector3(0, 1, 0); // Rotate around y-axis for left-right movement
                break;
            case MovementDirection.ForwardBackward:
                rotationAxis = new Vector3(1, 0, 0); // Rotate around x-axis for forward-backward movement
                break;
            default:
                rotationAxis = new Vector3(0, 1, 0);
                break;
        }

        Quaternion rotation = Quaternion.Euler(rotationAxis.x * angle, rotationAxis.y * angle, rotationAxis.z * angle);
        transform.localRotation = rotation;
    }
}
