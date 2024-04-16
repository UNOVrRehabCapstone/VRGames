using UnityEngine;

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
        }
        else
        {
            aimingRay.enabled = false;
        }
        if (thrown)
        {
            gameObject.transform.position -= transform.up * .1f;
        }
    }

    void startAimingRay()
    {
        aimingRay.SetPosition(0, transform.position - transform.up * .2f + transform.forward * .02f);
        origin = aimingRay.GetPosition(0);
        endPoint = origin - transform.up * 10f - transform.forward * .4f;
        aimingRay.SetPosition(1, endPoint);
        aimingRay.enabled = true;
    }

    //Called by Grabber
    public void onGrab(GameObject hand){
        manager.OnPlaneGrabbed( gameObject );
        aimAssist = true;
    }
    public void onRelease(GameObject hand){
        Rigidbody handRb = hand.GetComponent<Rigidbody>();
        thrown = true;
        /*engine.force = rb.velocity * 300;
        rb.AddRelativeForce(0, Vector3.Distance(handRb.velocity, Vector3.zero), 0);*/
        /*rb.angularVelocity = Vector3.zero;
        rb.velocity *= 30;
        engine.relativeForce = new Vector3(0, -80, 0);
        rb.AddForce(handRb.velocity * 90);
        engine.torque = Vector3.zero;*/
        

        rb.freezeRotation = true;
        manager.OnPlaneReleased( gameObject );
        aimAssist = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hoop"))
        {
            Debug.Log("Hit the Target!");
            other.GetComponentInChildren<ParticleSystem>().Play();
            
            manager.KillPlane(gameObject);
        }
    }
}