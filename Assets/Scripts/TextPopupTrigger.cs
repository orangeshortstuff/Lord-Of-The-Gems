using UnityEngine;
using UnityEngine.UI;

public class TextPopupTrigger : MonoBehaviour
{
    public GameObject textBox; // Reference to the UI text box

    void Start()
    {
        // Ensure the text box is initially hidden
        if (textBox != null)
        {
            textBox.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger area
        if (other.CompareTag("Player"))
        {
            // Show the text box
            if (textBox != null)
            {
                textBox.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the player exited the trigger area
        if (other.CompareTag("Player"))
        {
            // Hide the text box
            if (textBox != null)
            {
                textBox.SetActive(false);
            }
        }
    }
}
