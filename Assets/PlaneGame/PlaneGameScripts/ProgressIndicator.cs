/**
 * \file ProgressIndicator.cs
 * \brief Manages the progress indicator UI element.
 *
 * The ProgressIndicator class handles the management of a progress indicator UI element, which visually represents the progress of a certain action or process.
 */

using UnityEngine;
using UnityEngine.UI;

namespace PlanesGame
{
  /**
   * \class ProgressIndicator
   * \brief Manages the progress indicator UI element.
   *
   * The ProgressIndicator class handles the management of a progress indicator UI element, which visually represents the progress of a certain action or process.
   */
  public class ProgressIndicator : MonoBehaviour
  {
    /// The image component used for the progress fill.
    private Image fillImage;

    /**
     * \brief Initializes the progress indicator by finding the fill image component.
     */
    void Awake()
    {
      // Assuming the Image component is a direct child of the GameObject this script is attached to
      fillImage = GetComponentInChildren<Image>();
      if (fillImage == null)
      {
        Debug.LogError("No Image component found on the ProgressIndicator or its children.");
      }
    }

    /**
     * \brief Updates the progress indicator based on the current and required aim time.
     * 
     * \param currentAimTime The current time spent aiming.
     * \param requiredAimTime The total time required for aiming.
     */
    public void UpdateProgressIndicator(float currentAimTime, float requiredAimTime)
    {
      if (fillImage != null)
      {
        fillImage.fillAmount = Mathf.Clamp(currentAimTime / requiredAimTime, 0f, 1f);
      }
    }

    /**
     * \brief Initializes the progress indicator at a specified target position.
     * 
     * \param targetPosition The position at which to initialize the progress indicator.
     */
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

    /**
     * \brief Destroys the progress indicator GameObject.
     */
    public void DestroyProgressIndicator()
    {
      Destroy(gameObject);
    }
  }
}
