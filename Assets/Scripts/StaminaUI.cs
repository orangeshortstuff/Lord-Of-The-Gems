using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public Image filledPortionImage;
    public float deletionDuration = 5f; // Duration for the gradual deletion (in seconds)

    private float maxStamina;
    private float currentStamina;
    private float deletionStartTime;
    private bool isDeletionInProgress = false;

    void Start()
    {
        // Initialize stamina values
        maxStamina = GetComponent<CharacterControls>().maxStamina;
        currentStamina = maxStamina;

        // Start with full stamina bar
        filledPortionImage.fillAmount = 1f;
    }

    void Update()
    {
        // If deletion is in progress, update the filled portion image
        if (isDeletionInProgress)
        {
            // Calculate deletion percentage based on elapsed time since deletion started
            float deletionPercentage = (Time.time - deletionStartTime) / deletionDuration;

            // Clamp deletion percentage between 0 and 1
            deletionPercentage = Mathf.Clamp01(deletionPercentage);

            // Update filled portion image
            filledPortionImage.fillAmount = 1f - deletionPercentage;

            // Check if deletion is complete
            if (deletionPercentage >= 1f)
            {
                isDeletionInProgress = false;
            }
        }
        else
        {
            // Update filled portion image based on current stamina
            filledPortionImage.fillAmount = currentStamina / maxStamina;
        }
    }

    // Function to decrease stamina and start deletion of filled portion image
    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;

        // If stamina becomes negative, set it to 0
        currentStamina = Mathf.Max(currentStamina, 0f);

        // Start deletion if it's not already in progress
        if (!isDeletionInProgress)
        {
            StartDeletion();
        }
    }

    // Function to increase stamina and stop deletion of filled portion image
    public void IncreaseStamina(float amount)
    {
        currentStamina += amount;

        // If stamina exceeds max stamina, set it to max stamina
        currentStamina = Mathf.Min(currentStamina, maxStamina);

        // Stop deletion
        isDeletionInProgress = false;
    }

    // Function to start deletion of filled portion image
    private void StartDeletion()
    {
        isDeletionInProgress = true;
        deletionStartTime = Time.time;
    }
}
