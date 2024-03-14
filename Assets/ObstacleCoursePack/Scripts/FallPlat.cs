using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPlat : MonoBehaviour
{
    public float fallTime = 0.5f;
    public float respawnTime = 2f; // Adjust this to change respawn time
    public AudioClip audioLog; // Assign your audio clip in the inspector
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool hasPlayedAudio = false;

    void Start()
    {
        // Store the original position and rotation of the platform
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // We'll manually control physics for falling
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (collision.gameObject.tag == "Player")
            {
                StartCoroutine(FallAndRespawn());
                if (!hasPlayedAudio && audioLog != null)
                {
                    audioSource.Play();
                    hasPlayedAudio = true;
                }
            }
        }
       

    }

    IEnumerator FallAndRespawn()
    {
        // Wait for the fallTime before dropping the platform
        yield return new WaitForSeconds(fallTime);

        // Drop the platform
        rb.isKinematic = false; // Enable physics to make it fall
        yield return new WaitForSeconds(1f); // Wait for the platform to fall properly (you might adjust this delay)

        // Reset the position and rotation of the platform
        rb.isKinematic = true; // Disable physics
        transform.position = originalPosition; // Reset position
        transform.rotation = originalRotation; // Reset rotation

        // Wait for the respawnTime before allowing the platform to fall again
        yield return new WaitForSeconds(respawnTime);

        // Reset audio flag
        hasPlayedAudio = false;
    }

}
