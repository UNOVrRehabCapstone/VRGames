/**
 * \file PaperPlane.cs
 * \brief Manages the behavior of the paper plane object.
 *
 * The PaperPlane class controls the behavior of a paper plane object in the game, including throwing, aiming, and collision detection.
 */

using Classes;
using UnityEngine;
using OVR;
using UnityEngine.UI;
using UnityEngine.XR;

namespace PlanesGame{
  /**
   * \class PaperPlane
   * \brief Manages the behavior of the paper plane object.
   *
   * The PaperPlane class controls the behavior of a paper plane object in the game, including throwing, aiming, and collision detection.
   */
  public class PaperPlane : MonoBehaviour, IGrabEvent
{
    private bool thrown = false;
    private Rigidbody rb;
    private ConstantForce engine;
    private PlaneGameplayManager manager;
    private bool aimAssist;
    private LineRenderer aimingRay;
    private Vector3 origin;
    private Vector3 endPoint;

    // Distance From Head Throw variables
    public GameObject playerAvatarPrefab; // Assign this in the inspector
    private Transform rightHandAnchor;
    private Transform leftHandAnchor;
    private Transform centerEyeAnchor;
    [SerializeField] public static float throwThreshold;
    private Vector3 lastHandPosition;
    private bool isReadyToThrow = false;

    // Gripless Grabbing variables
    private Transform handTransform; // Transform of the player's hand
    private GameObject currentTarget;
    private GameObject potentialTarget;
    private float currentAimTime;
    private float maxTravelTime = 10;
    private float currentTravelTime = 0;

    // Variable for visual indicator of target lock and auto-throw
    public GameObject progressIndicatorPrefab;
    private GameObject currentIndicator;


    public enum Quest2Button
    {
        AButton = OVRInput.Button.One,
        BButton = OVRInput.Button.Two,
        Joystick = OVRInput.Button.PrimaryThumbstick,
        Trigger = OVRInput.Button.PrimaryIndexTrigger,
        Grip = OVRInput.Button.PrimaryHandTrigger
    }


    [SerializeField] public static float requiredAimTime = 3.0f;
    [SerializeField] public static bool useDistanceFromHead = false;
    [SerializeField] public static bool useGriplessGrabbing = true;
    [SerializeField] public static bool useAutoReleaseTimer = true;
    [SerializeField] public static bool useAutoAim = true;
    [SerializeField] public static bool useButtonPressForThrow = false;
    [SerializeField] public static Quest2Button releaseButton = Quest2Button.AButton;


    /**
     * \brief Perform initial setup and configuration
     */
    void Start()
    {
        aimingRay = gameObject.AddComponent<LineRenderer>();
        aimingRay.startWidth = .005f;
        aimingRay.endWidth = .02f;
        aimingRay.material.color = Color.green;
        aimingRay.startColor = Color.green;
        aimingRay.endColor = Color.green;
        engine = gameObject.GetComponent<ConstantForce>();
        rb = gameObject.GetComponent<Rigidbody>();

        manager = null;
        if (GameplayManager.getManager() is PlaneGameplayManager)
        {
            manager = (PlaneGameplayManager)GameplayManager.getManager();
        }

        GameObject playerAvatar = GameObject.FindWithTag("Player");
        if (playerAvatarPrefab != null) {
            Debug.Log("Player Avatar Found");
            rightHandAnchor = playerAvatar.transform.Find("AvatarGrabberRight");
            leftHandAnchor = playerAvatar.transform.Find("AvatarGrabberLeft");
            centerEyeAnchor = playerAvatar.transform.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        }
        else {
            Debug.Log("No Player Avatar Found");
        }
    }

    /**
     * \brief Update is called once per frame.
     */
    private void Update()
    {
        // if (playerAvatarPrefab == null) {
           
        // }

        if (aimAssist){
            startAimingRay();

            // All gripless Grabbing throws are in here
            if (useGriplessGrabbing) {
                if (useAutoAim || useAutoReleaseTimer) {
                    CheckTargetLock();
                }

                if (useDistanceFromHead) {
                    CheckThrowCondition();
                }

                // Auto-release target on timer
                if (!thrown && useAutoReleaseTimer && currentAimTime >= requiredAimTime) {
                    GriplessRelease();
                }
                // Release target via button press
                else if (!thrown && CheckReleaseButtonPress())
                {
                    GriplessRelease();
                }
            }
        }
        else {
            aimingRay.enabled = false;
        }

        if (thrown) {
            FlyTowardsTarget();
        }
    }

    /**
     * \brief Applies force to accelerate the plane towards the target.
     */
    void ApplyForceTowardsTarget()
    {
        if (currentTarget)
        {
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            float forceMagnitude = 500.0f;  // Adjust the force magnitude as necessary
            rb.AddForce(directionToTarget * forceMagnitude);
        }
    }

    /**
     * \brief Checks if the target is locked on based on aiming duration.
     */
    void CheckTargetLock()
    {
        if (currentAimTime >= requiredAimTime)
        {
            currentTarget = potentialTarget;
        }
    }

    /**
     * \brief Checks if the conditions to throw have been met, and if they are, throws the plane.
     */
    private void CheckThrowCondition()
    {
        // Current positions of the hand and head
        Vector3 currentHandPosition;
        // If you have the same hand anchor on both sides, it's fun to play with, ngl
        if (handTransform.parent.CompareTag("RightGrabber")) { currentHandPosition = rightHandAnchor.position; }
        else { currentHandPosition = leftHandAnchor.position;}
   
        
        Vector3 currentHeadPosition = centerEyeAnchor.position;

        // Debug.Log(Vector3.Distance(currentHandPosition, currentHeadPosition));

        //Debug.Log(Vector3.Distance(currentHandPosition, currentHeadPosition));

        // Check if the hand has moved away from its last position by more than the threshold
        if (isReadyToThrow && Vector3.Distance(currentHandPosition, currentHeadPosition) > (throwThreshold / 100))
        {
            // The hand has moved enough distance; we consider this a throw
            GriplessRelease();
            isReadyToThrow = false; // Reset the throw readiness
        }
        else if (!isReadyToThrow) {
            lastHandPosition = currentHandPosition;

            Vector3 localEulerAngles;

            if (handTransform.parent.CompareTag("RightGrabber")) { localEulerAngles = rightHandAnchor.rotation.eulerAngles; }
            else { localEulerAngles = leftHandAnchor.rotation.eulerAngles; }   

            // Normalize to make angle values the same number shown in inspector
            localEulerAngles = NormalizeAngles(localEulerAngles);
            Debug.Log(localEulerAngles);

            if (localEulerAngles.x <= -80  &&
            localEulerAngles.y >= -90 && localEulerAngles.y <= 90 &&
            localEulerAngles.z >= -90 && localEulerAngles.z <= 90)
            {
                Debug.Log("X Rotation Met");
                ReadyToThrow();
            }
        }

        // Update the last hand position
        
    }

    /**
     * \brief Checks if the release button is pressed on either controller.
     * \return True if the release button is pressed, otherwise false.
     */
    bool CheckReleaseButtonPress() {
        // Check the selected button on both the right and left controllers
        if (useButtonPressForThrow) {
                if (OVRInput.GetDown((OVRInput.Button)releaseButton, OVRInput.Controller.RTouch) ||
                    OVRInput.GetDown((OVRInput.Button)releaseButton, OVRInput.Controller.LTouch))
                {
                    return true;
                }
        }
        return false;
    }

    /**
     * \brief Controls the flight of the paper plane towards the target.
     * 
     * If automatic aiming is enabled, the plane rotates towards the target and moves forward.
     * If manual aiming is used or no target is locked, the plane moves forward in a straight line.
     * 
     * \note The plane's speed and rotation are adjusted based on the target's position and the current settings.
     */
    void FlyTowardsTarget()
    {
        // Disable gravity on throw
        rb.useGravity = true;

        // Automatic aiming
        if (currentTarget && useAutoAim) {
            Quaternion modelForwardCorrection = Quaternion.Euler(-90, -200, 203);
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position);    

            // Calculate the corrected target orientation
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion correctedTargetRotation = targetRotation * modelForwardCorrection;

            // Optionally, smooth the rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedTargetRotation, Time.deltaTime * 5);
            float speed = 10.0f;

            directionToTarget = (currentTarget.transform.position - transform.position);    
            rb.velocity = directionToTarget.normalized * speed;        
        }
        // Manual aiming
        else {
            gameObject.transform.position -= transform.up * .1f;
        }

        currentTravelTime += Time.deltaTime;
        if (currentTravelTime >= maxTravelTime) {
            manager.KillPlane(gameObject);
        }
    }

    /*
     * \brief Triggers the plane's automatic release toward the locked-on target.
     */
    void GriplessRelease() {
        if (!thrown)
        {
            // Detach the plane properly ensuring world position and rotation are maintained
            Vector3 worldPosition = transform.position;
            Quaternion worldRotation = transform.rotation;
            Debug.Log("Trying to unset parent");
            transform.SetParent(null);
            transform.position = worldPosition;
            transform.rotation = worldRotation;

            // Ensure the Rigidbody takes over with appropriate settings
            rb.isKinematic = false;
            rb.freezeRotation = true;

            // Set flags to reflect new state
            thrown = true;
            aimAssist = false;
            manager.OnPlaneReleased(gameObject);
        }
    }

    /**
     * \brief Normalizes Euler angles to ensure they fall within the range of -180 to 180 degrees.
     * 
     * \param angles The Euler angles to normalize.
     * \return Normalized Euler angles.
     */
    private Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

    /**
     * \brief Normalizes an angle to ensure it falls within the range of -180 to 180 degrees.
     * 
     * \param angle The angle to normalize.
     * \return Normalized angle.
     */
    private float NormalizeAngle(float angle)
    {
        // Adjust the angle to be within -180 and 180 degrees.
        if (angle > 180) {
            angle -= 360;
        }
        return angle;
    }

    /// <summary>
    /// Called when the object is grabbed, initializes the aiming process.
    /// </summary>
    /// <param name="hand">The hand GameObject that initiated the grab.</param>
    public void onGrab(GameObject hand){
        if (!useGriplessGrabbing)
        {
            manager.OnPlaneGrabbed(gameObject);
            aimAssist = true;
        }
    }

    /// <summary>
    /// Called when the object is released, performs the throw of the paper plane.
    /// </summary>
    /// <param name="hand">The hand GameObject that released the plane.</param>
    public void onRelease(GameObject hand){
        if (!useGriplessGrabbing) {
            thrown = true;
            rb.freezeRotation = true;
            manager.OnPlaneReleased(gameObject);
            aimAssist = false;
        }
    }

    /// <summary>
    /// Checks for collision with a target and triggers appropriate behavior.
    /// </summary>
    /// <param name="other">The collider the plane collided with.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hoop"))
        {
            Debug.Log("Hit the Target!");
            other.GetComponentInChildren<ParticleSystem>().Play();
            
            manager.KillPlane(gameObject);
            DestroySingleProgressIndicator();

            other.GetComponent<Target>().HitTarget();

        }

        if (other.CompareTag("RightGrabber") || other.CompareTag("LeftGrabber") && !thrown)
        {
            if (!thrown && useGriplessGrabbing) {
                handTransform = other.transform;
                transform.SetParent(handTransform);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                rb.isKinematic = true;
                aimAssist = true;
            }
        }
    }

    /**
     * \brief Sets the plane as ready to be thrown.
     * 
     * Sets the flag indicating that the plane is ready to be thrown and updates the last hand position.
     */
    public void ReadyToThrow()
    {
        isReadyToThrow = true;
        lastHandPosition = rightHandAnchor.position; // Reset the last hand position
    }

    /// <summary>
    /// Starts the aiming process with a raycast and updates the lock-on progress.
    /// </summary>
    void startAimingRay() {
        aimingRay.SetPosition(0, transform.position - transform.up * .2f + transform.forward * .02f);
        origin = aimingRay.GetPosition(0);
        endPoint = origin - transform.up * 10f - transform.forward * .4f;
        aimingRay.SetPosition(1, endPoint);
        aimingRay.enabled = true;

        RaycastHit hit;
        if (useGriplessGrabbing && Physics.Raycast(origin, (endPoint - origin).normalized, out hit, 50))
        {
            Debug.Log("Hit something");
            Debug.Log(hit.collider.gameObject.tag);
            if (hit.collider.gameObject.CompareTag("Hoop")) {
                if (potentialTarget != hit.collider.gameObject) {
                    DestroyProgressIndicator();
                    potentialTarget = hit.collider.gameObject;
                    currentAimTime = 0;
                    Debug.Log("Aim time reset");
                    SpawnProgressIndicator(potentialTarget.transform.position);
                }
                currentAimTime += Time.deltaTime;
                UpdateProgressIndicator(currentAimTime, requiredAimTime);
                Debug.Log("Pointing at target");
            }
        }
    }

    // --> PROGRESS INDICATOR FUNCTIONS <--

    /**
     * \brief Destroys all progress indicators in the scene.
     * 
     * Finds all progress indicators in the scene and destroys them. Resets the potential target to null.
     */
    void DestroyProgressIndicator() {
        ProgressIndicator[] allIndicators = FindObjectsOfType<ProgressIndicator>();

        foreach (ProgressIndicator indicator in allIndicators) {
            // Call the indicator's custom destruction method if it exists
            indicator.DestroyProgressIndicator();
        }
        potentialTarget = null;
    }

    /**
     * \brief Destroys the current progress indicator.
     * 
     * Destroys the current progress indicator if it exists and if auto-aim or auto-release timer is enabled.
     */
    void DestroySingleProgressIndicator() {
        if (useAutoAim || useAutoReleaseTimer) {
            if (currentIndicator != null) {
                ProgressIndicator progressScript = currentIndicator.GetComponent<ProgressIndicator>();
                progressScript.DestroyProgressIndicator();
                currentIndicator = null;
            }
        }
    }

    /**
     * \brief Spawns a progress indicator at the specified position.
     * 
     * Creates a new progress indicator GameObject at the given position if auto-aim or auto-release timer is enabled.
     * 
     * \param position The position to spawn the progress indicator.
     */
    void SpawnProgressIndicator(Vector3 position) {
        if (useAutoAim || useAutoReleaseTimer) {
            currentIndicator = Instantiate(progressIndicatorPrefab);
            ProgressIndicator progressScript = currentIndicator.GetComponent<ProgressIndicator>();
            progressScript.InitializeProgressIndicator(position);
        }
    }


    /**
     * \brief Updates the progress indicator with the current and required progress.
     * 
     * Updates the progress indicator with the current and required progress if auto-aim or auto-release timer is enabled.
     * 
     * \param current The current progress value.
     * \param required The required progress value.
     */
    void UpdateProgressIndicator(float current, float required) {
        if (useAutoAim || useAutoReleaseTimer) {
            if (currentIndicator != null) {
                ProgressIndicator progressIndicatorScript = currentIndicator.GetComponent<ProgressIndicator>();
                progressIndicatorScript.UpdateProgressIndicator(current, required);
            }
        }
    }
}}
