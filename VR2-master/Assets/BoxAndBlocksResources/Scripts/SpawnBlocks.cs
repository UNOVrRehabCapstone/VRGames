using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour
{
    public GameObject[] blockList;
    public int numOfBlocks;         //goal number of total blocks (150)
    public int blockInterval;       //number of blocks to start out with
    public GameObject otherSpawnPoint;

    int layer = 0; 
    private int currBlocks = 0;     //count of blocks spawned
    private static List<GameObject> spawnedBlocks = new List<GameObject>();

    void SpawnBlock()
    {
        GameObject block = Instantiate(genRandomBlock(), this.transform.position + new Vector3(Random.Range(-.2f, .2f), 0, Random.Range(-.2f, .2f)), Quaternion.identity);
        spawnedBlocks.Add(block);
        if (layer != 0)
        {
            block.layer = layer;
            block.GetComponent<BlockStateChange>().spawnblocks = gameObject;
            block.GetComponent<BlockStateChange>().goalSide = otherSpawnPoint;
        }
        
        currBlocks++;
        if (currBlocks % blockInterval == 0)
            CancelInvoke();
    }

    //when a block is placed on the goal side of the box, a new block will be spawned to replace it on the start side
    public void DecrementBlocks()
    {
        SpawnBlock();
    }

    public static List<GameObject> GetSpawnedBlocks()
    {
        return spawnedBlocks;
    }

    public void StartSpawning(int grabbableLayer)
    {
        /* foreach (GameObject block in blockList)
         {
             block.layer = grabbableLayer;
             block.GetComponent<BlockStateChange>().spawnblocks = gameObject;
             block.GetComponent<BlockStateChange>().goalSide = otherSpawnPoint;
         }*/
        layer = grabbableLayer;
        //spawn the block and wait before spawning another, 
        //to avoid block collisions and strange behavior before landing in box
        InvokeRepeating("SpawnBlock", 0, 0.1f);
    }
    public GameObject genRandomBlock()
    {
        return blockList[Random.Range(0, blockList.Length)];
    }
}
