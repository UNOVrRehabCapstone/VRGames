﻿using Classes;
using UnityEngine;
using OVR;

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

    private GameObject currentTarget;
    private GameObject potentialTarget;
    private float currentAimTime;

    // Gripless Grabbing variables
    private Transform handTransform; // Transform of the player's hand


    [SerializeField] private float requiredAimTime = 3.0f;
    [SerializeField] private bool useDistanceFromHead = false;
    [SerializeField] private bool useGriplessGrabbing = false;
    [SerializeField] private bool useAutoReleaseTimer = false;
    [SerializeField] private bool useAutoAim = false;
    [SerializeField] private bool useAButtonPressForThrow = false;

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
    }

    private void Update()
    {
        if (aimAssist)
        {
            startAimingRay();

            if (useGriplessGrabbing) {
                CheckTargetLock();

                if (!thrown && useAutoReleaseTimer && currentAimTime >= requiredAimTime) {
                    GriplessRelease();
                }
                else if (!thrown && CheckReleaseButtonPress())
                {
                    thrown = true;
                    aimAssist = false;
                }
            }
            
        }
        else
        {
            aimingRay.enabled = false;
        }


        if (thrown && !useAutoAim)
        {
            gameObject.transform.position -= transform.up * .1f;
        }
        else if (thrown && useAButtonPressForThrow) {
            FlyTowardsTarget();
        }
    }

    // Accelerate plane with a force
    void ApplyForceTowardsTarget()
    {
        if (currentTarget)
        {
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            float forceMagnitude = 500.0f;  // Adjust the force magnitude as necessary
            rb.AddForce(directionToTarget * forceMagnitude);
        }
    }


    void CheckTargetLock()
    {
        if (currentAimTime >= requiredAimTime)
        {
            currentTarget = potentialTarget;
        }
    }

    bool CheckReleaseButtonPress() {
        if (useAButtonPressForThrow
            && OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)
            || OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)) {
            return true;
        }
        else {
            return false;
        }
    }

    // Aiming algorithm towards target
    void FlyTowardsTarget()
    {
        if (currentTarget)
        {
            rb.freezeRotation = true;
            manager.OnPlaneReleased(gameObject);
            rb.isKinematic = false;
            Vector3 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
            float speed = 30.0f;  // Adjust the speed as necessary
            rb.velocity = directionToTarget * speed;
        }
    }


    void GriplessRelease() {
        if (currentTarget && !thrown)
        {
            // Detach the plane properly ensuring world position and rotation are maintained
            Vector3 worldPosition = transform.position;
            Quaternion worldRotation = transform.rotation;
            Debug.Log("Trying to unset parent");
            transform.SetParent(null);
            transform.position = worldPosition;
            transform.rotation = worldRotation;

            // Ensure the Rigidbody takes over with appropriate settings
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.freezeRotation = true;
            }

            // Set flags to reflect new state
            thrown = true;
            aimAssist = false;
            manager.OnPlaneReleased(gameObject);
        }
    }

    //Called by Grabber
    public void onGrab(GameObject hand){
        if (!useGriplessGrabbing)
        {
            manager.OnPlaneGrabbed(gameObject);
            aimAssist = true;
        }
    }
    public void onRelease(GameObject hand){
        if (!useGriplessGrabbing) {
            thrown = true;
            rb.freezeRotation = true;
            manager.OnPlaneReleased(gameObject);
            aimAssist = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hoop"))
        {
            Debug.Log("Hit the Target!");
            other.GetComponentInChildren<ParticleSystem>().Play();
            
            manager.RespawnPlane(gameObject);

            other.GetComponent<Target>().hitTarget();

        }

        if (other.CompareTag("RightGrabber") || other.CompareTag("LeftGrabber"))
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

    void startAimingRay()
    {
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
            if (hit.collider.gameObject.CompareTag("Hoop"))
            {

                if (potentialTarget != hit.collider.gameObject)
                {
                    potentialTarget = hit.collider.gameObject;
                    currentAimTime = 0;
                    Debug.Log("Aim time reset");
                }
                currentAimTime += Time.deltaTime;
                Debug.Log("Pointing at target");
            }
        }
    }
}