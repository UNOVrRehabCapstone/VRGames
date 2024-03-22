using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour {
    public GameObject spawnPoint;
    public Material pressedMat;
    public Material defaultMat;
    public GameObject otherButton;
    public int grabbableLayer;
    public GameObject databtn;
    public GameObject data;

    void OnTriggerEnter(Collider collider)
    {
        //if collision with hand, change color and enable left/right spawnpoint
        if (collider.gameObject.tag == "Player")
        {
            gameObject.GetComponent<MeshRenderer>().material = pressedMat;
            switch (this.gameObject.tag)
            {
                case "Button":
                    

                    //wait a second before disabling the button
                    Invoke("Pressed", 1);
                    break;

                case "Info":
                    data.gameObject.SetActive(true); 

                    break;

            }
            
        }
    }

    void OnTriggerExit(Collider collider)
    {
        //if collision with hand, change color and enable left/right spawnpoint
        if (collider.gameObject.tag == "Player")
        {
            gameObject.GetComponent<MeshRenderer>().material = defaultMat;
            Debug.Log(this.gameObject.tag);
            switch (this.gameObject.tag)
            {

                case "Info":
                    data.gameObject.SetActive(false);

                    break;

            }

        }
    }

    public void Start()
    {
       // Pressed();
    }

    

    //disable pressed button, reset default material, and start block spawning
    private void Pressed()
    {

        otherButton.gameObject.SetActive(false);
        databtn.gameObject.SetActive(false);
        data.gameObject.SetActive(false);
        //wait a second before disabling the button
       
        CountUp.hand = grabbableLayer;
        
        this.gameObject.SetActive(false);
        gameObject.GetComponent<MeshRenderer>().material = defaultMat;
        spawnPoint.GetComponent<SpawnBlocks>().StartSpawning(grabbableLayer);
    }
}
