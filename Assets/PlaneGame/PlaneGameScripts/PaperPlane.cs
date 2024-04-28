using Classes;
using UnityEngine;
using OVR;
using UnityEngine.UI;
using UnityEngine.XR;


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
    public float throwThreshold;
    private Vector3 lastHandPosition;
    private bool isReadyToThrow = false;

    // Gripless Grabbing variables
    private Transform handTransform; // Transform of the player's hand
    private GameObject currentTarget;
    private GameObject potentialTarget;
    private float currentAimTime;
    private float maxTravelTime = 30;
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


    [SerializeField] private float requiredAimTime = 3.0f;
    [SerializeField] private bool useDistanceFromHead = false;
    [SerializeField] private bool useGriplessGrabbing = false;
    [SerializeField] private bool useAutoReleaseTimer = false;
    [SerializeField] private bool useAutoAim = false;
    [SerializeField] private bool useButtonPressForThrow = false;
    [SerializeField] private Quest2Button releaseButton = Quest2Button.AButton;


    /// <summary>
    /// Perform initial setup and configuration.
    /// </summary>
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

    /// <summary>
    /// Update is called once per frame and handles user input and lock-on logic.
    /// </summary>
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

    // Accelerate plane with a force for constant acceleration
    void ApplyForceTowardsTarget()
    {
        if (currentTarget)
        {
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            float forceMagnitude = 500.0f;  // Adjust the force magnitude as necessary
            rb.AddForce(directionToTarget * forceMagnitude);
        }
    }

    /// <summary>
    /// Checks if the target is locked on based on aiming duration.
    /// </summary>
    void CheckTargetLock()
    {
        if (currentAimTime >= requiredAimTime)
        {
            currentTarget = potentialTarget;
        }
    }

    private void CheckThrowCondition()
    {
        // Current positions of the hand and head
        Vector3 currentHandPosition = rightHandAnchor.position;
        Vector3 currentHeadPosition = centerEyeAnchor.position;

        Debug.Log(Vector3.Distance(currentHandPosition, currentHeadPosition));

        // Check if the hand has moved away from its last position by more than the threshold
        if (isReadyToThrow && Vector3.Distance(currentHandPosition, currentHeadPosition) > throwThreshold)
        {
            // The hand has moved enough distance; we consider this a throw
            GriplessRelease();
            isReadyToThrow = false; // Reset the throw readiness
        }
        else if (!isReadyToThrow) {
            lastHandPosition = currentHandPosition;
            Vector3 localEulerAngles = rightHandAnchor.rotation.eulerAngles;            

            // Normalize to make angle values the same number shown in inspector
            localEulerAngles = NormalizeAngles(localEulerAngles);

            if (localEulerAngles.x <= -30  &&
            localEulerAngles.y >= -90 && localEulerAngles.y <= 90 &&
            localEulerAngles.z >= -90 && localEulerAngles.z <= 90)
            {
                Debug.Log("X Rotation Met");
                ReadyToThrow();
            }
        }

        // Update the last hand position
        
    }

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

    // Fly plane
    void FlyTowardsTarget()
    {
        // Automatic aiming
        if (currentTarget && useAutoAim) {
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position);

            Quaternion modelForwardCorrection = Quaternion.Euler(-90, -200, 203);

            // Calculate the corrected target orientation
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion correctedTargetRotation = targetRotation * modelForwardCorrection;

            // Optionally, smooth the rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, correctedTargetRotation, Time.deltaTime * 5);
            float speed = 3.0f;
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

    /// <summary>
    /// Triggers the plane's automatic release toward the locked-on target.
    /// </summary>
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

    private Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = NormalizeAngle(angles.x);
        angles.y = NormalizeAngle(angles.y);
        angles.z = NormalizeAngle(angles.z);
        return angles;
    }

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
            DestroyProgressIndicator();

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
            Debug.Log("Hit something, but not target");
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
            else {
                DestroyProgressIndicator();
                potentialTarget = null;
            }

        }
    }

    // --> PROGRESS INDICATOR FUNCTIONS <--
    void DestroyProgressIndicator() {
        if (useAutoAim || useAutoReleaseTimer) {
            if (currentIndicator != null) {
                ProgressIndicator progressScript = currentIndicator.GetComponent<ProgressIndicator>();
                progressScript.DestroyProgressIndicator();
                currentIndicator = null;
            }
        }
    }

    void SpawnProgressIndicator(Vector3 position) {
        if (useAutoAim || useAutoReleaseTimer) {
            currentIndicator = Instantiate(progressIndicatorPrefab);
            ProgressIndicator progressScript = currentIndicator.GetComponent<ProgressIndicator>();
            progressScript.InitializeProgressIndicator(position);
        }
    }


    void UpdateProgressIndicator(float current, float required) {
        if (useAutoAim || useAutoReleaseTimer) {
            if (currentIndicator != null) {
                ProgressIndicator progressIndicatorScript = currentIndicator.GetComponent<ProgressIndicator>();
                progressIndicatorScript.UpdateProgressIndicator(current, required);
            }
        }
    }
}