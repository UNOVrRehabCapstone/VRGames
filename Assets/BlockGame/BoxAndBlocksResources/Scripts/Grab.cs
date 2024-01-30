using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {

    public LayerMask grabbablelayer;    //the layer gameobjects must be on to be grabbable
    public int droppablelayer;          //the default layer, blocks will switch to this when dropped
    public GameObject prefab;           //block prefab
    public GameObject spawnblocks;      //spawn point
    public GameObject timeCount;        //ui text for displaying count down
    public Material grabbedMat;         //material block will be changed to when grabbed
    public Material defaultMat;         //material block will be set to when dropped

    private bool isGrabbing;                //if the hand is grabbing at an object
    private bool isHolding;                 //if the hand triggers are being pressed
    //private bool hitDivider;                //if the hand hits the middle divider
    private GameObject block;               //the gameobject being held
    private RaycastHit[] hits;              //objects near hand
    private OVRInput.Button pointerTrigger; //pointer finger trigger
    private OVRInput.Button middleTrigger;  //middle finger trigger

    void Start () {
        //default all to false
        isGrabbing = false;
        isHolding = false;
        //hitDivider = false;

        //determining which hand this instance is tracking
        if (this.name == "hand_left")
        {
            pointerTrigger = OVRInput.Button.PrimaryIndexTrigger;
            middleTrigger = OVRInput.Button.PrimaryHandTrigger;
        }
        else
        {
            pointerTrigger = OVRInput.Button.SecondaryIndexTrigger;
            middleTrigger = OVRInput.Button.SecondaryHandTrigger;
        }      
	}
	
	// Update is called once per frame
	void Update () {
        //finding nearby objects on the grabbable layer
        hits = Physics.SphereCastAll(transform.position, 0.03f, transform.forward, 0f, grabbablelayer);
        //if the hand triggers are pressed, isholding = true
        if (OVRInput.Get(pointerTrigger) && OVRInput.Get(middleTrigger))
        {
            isHolding = true;
            //start countdown if grabbing something
            if(isGrabbing)
                timeCount.GetComponent<CountDown>().StartCountDown();
        }
        else
        {
            isHolding = false;
        }

        //move block with hand if holding one
        if (isGrabbing == true)
        {
            block.transform.position = this.transform.position;
            block.transform.rotation = this.transform.rotation;
        }

        //drop
        if (block != null && !isHolding)
        {
            isGrabbing = false;
            block.GetComponent<Rigidbody>().isKinematic = false;
            block.layer = 0;
            block.GetComponent<MeshRenderer>().material = defaultMat;
            block = null;
        }
        //grab
        else if(hits.Length > 0 && !isGrabbing && isHolding)
        {
            isGrabbing = true;

            int closestHit = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < hits[closestHit].distance) closestHit = i;
            }
            block = hits[closestHit].transform.gameObject;
            block.GetComponent<MeshRenderer>().material = grabbedMat;
            block.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    //dropping and destroying block if hand runs into divider
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.name == "Divider" && block != null && isHolding)
        {
            isGrabbing = false;
            isHolding = false;
            block.GetComponent<BlockStateChange>().setHitDivider(true);
            block.layer = 0;
            block.GetComponent<Rigidbody>().isKinematic = false;
            block = null;
        }
    }

    public void StopGrabbing()
    {
        this.isGrabbing = false;
        this.block = null;
    }
}
