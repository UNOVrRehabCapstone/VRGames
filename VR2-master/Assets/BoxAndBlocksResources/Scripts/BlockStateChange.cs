using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockStateChange : MonoBehaviour {
    public GameObject goalSide;         //side of the box the player is trying to move the blocks to
    //public Material goalMat;            //material to change block color upon dropping into the goal side
    public GameObject counttext;        //the gameobject that has the UI text that displays the player's score
    public GameObject spawnblocks;      //spawnpoint and start side
    private bool hitDivider = false;    //if the block has collided with the middle divider
    private int startLayer;             //initial layer for the block


    public Material[] colors;

    public GameObject achievement;
    private List<GameObject> blocks;
    private Material startMat;

    void Start()
    {
        //setting initial layer (left or right grababble)
        achievement = GameObject.Find("Achievements");
        startLayer = gameObject.layer;
        counttext = GameObject.Find("CountText");
        startMat = gameObject.GetComponent<MeshRenderer>().material;

       
    }

	void OnTriggerEnter(Collider collider)
    {
        //if block hits the divider and drops, destroy it
        if (collider.gameObject == spawnblocks && hitDivider)
        {
            blocks = SpawnBlocks.GetSpawnedBlocks();
            GameObject block = gameObject;
            Destroy(block);
            blocks.Remove(block);
        }
        //if block hits the goal side, change color and increment score
        else if (collider.gameObject == goalSide)
        {
            gameObject.GetComponent<MeshRenderer>().material = startMat;
            //Debug.Log("sta" + startMat.name);
            if (startMat.name == "BlueMat (Instance)")
            {
                achievement.GetComponent<Achievements>().blue++;
            }
            else if (startMat == colors[1])
            {

                achievement.GetComponent<Achievements>().red++;
            }
            else
            {
                achievement.GetComponent<Achievements>().green++;

            }
            counttext.GetComponent<CountUp>().incrementCount();
            
            //spawnblocks.GetComponent<SpawnBlocks>().DecrementBlocks();
        }
        //if block drops back into start side, reset its layer to left/right grabbable
        else if (collider.gameObject == spawnblocks)
        {
            gameObject.layer = startLayer;
            gameObject.GetComponent<MeshRenderer>().material = startMat;
        }
    }

    //setting hitdivider from a Grab instance
    public void setHitDivider(bool hit)
    {
        hitDivider = hit;
    }
}
