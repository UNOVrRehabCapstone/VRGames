// ProgressIndicator.cs
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour
{
    private Image fillImage;

    void Awake()
    {
        // Assuming the Image component is a direct child of the GameObject this script is attached to
        fillImage = GetComponentInChildren<Image>();
        if (fillImage == null)
        {
            Debug.LogError("No Image component found on the ProgressIndicator or its children.");
        }
    }

    public void UpdateProgressIndicator(float currentAimTime, float requiredAimTime)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = Mathf.Clamp(currentAimTime / requiredAimTime, 0f, 1f);
        }
    }

    public void InitializeProgressIndicator(Vector3 targetPosition)
    {
        // Position this GameObject's transform at the target position
        transform.position = targetPosition;

        // Reset the fill amount
        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
        }
    }
    
    public void DestroyProgressIndicator()
    {
        Destroy(gameObject);
    }
}
