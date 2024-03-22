using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    //The Target GameObject
    [SerializeField]
    GameObject targetPrefab;

    //The number of targets to spawn in each "wave"
    [SerializeField]
    int targetsAtOnce;


    [SerializeField]
    float radius;

    Collider[] colliders;

    //The bounds of the box in which to randomly spawn the targets.
    float xSpawnRangeLo;
    float xSpawnRangeHi;
    float ySpawnRangeLo;
    float ySpawnRangeHi;
    float zSpawnRangeLo;
    float zSpawnRangeHi;

    //The size of the Target prefab [Obtained automatically in Start()]
    float halfx;
    float halfy;
    float halfz;

    Quaternion targetOrientation = Quaternion.Euler(0, -90, 90);

    // Start is called before the first frame update
    void Start()
    {
        //Define the bounds within which to spawn targets
        xSpawnRangeLo = -5;
        xSpawnRangeHi = 5;
        ySpawnRangeLo = 1;
        ySpawnRangeHi = 3;
        zSpawnRangeLo = 5;
        zSpawnRangeHi = 15;

        //Spawn a dummy target way out of bounds to retrieve its dimensions
        GameObject tempTarget = SpawnTarget(new Vector3(xSpawnRangeHi + 500, ySpawnRangeHi + 4, 0));
        MeshRenderer tempTargetMeshRenderer = tempTarget.GetComponent<MeshRenderer>();

        halfx = tempTargetMeshRenderer.bounds.extents.x;
        halfy = tempTargetMeshRenderer.bounds.extents.y;
        halfz = tempTargetMeshRenderer.bounds.extents.z;

        Destroy(tempTarget);

        //Spawn the initial "wave" of targets
        SpawnConflictFree(targetsAtOnce);
    }

    GameObject SpawnTarget (Vector3 pos)
    {
        return Instantiate(targetPrefab, pos, targetOrientation);
    }

    //Spawn n targets while checking that they are not occupying the same space.
    void SpawnConflictFree(int n)
    {
        Vector3 spawnPos;

        bool spawnCollision = true;
        int maxSpawnAttempts = 80;
        int spawnAttempt = 0;

        //Randomly select a position to attempt to spawn a target, check if it collides with an existing target. If it does: try again. if not: spawn it.
        for (int i = 0; i < n; i++)
        {
            do
            {
                spawnAttempt++;
                spawnPos = SpawnInRange();
                spawnCollision = CollisionOccurs(spawnPos);
            } while (spawnCollision && spawnAttempt < maxSpawnAttempts);

            if (!spawnCollision)
            {
                SpawnTarget(spawnPos);
            }
            else
            {
                Debug.Log("skip!");
            }

            spawnAttempt = 0;
        }
    }

    //Use Random to generate a potential spawn position.
    Vector3 SpawnInRange()
    {
        float x = Random.Range(xSpawnRangeLo, xSpawnRangeHi);
        float y = Random.Range(ySpawnRangeLo, ySpawnRangeHi);
        float z = Random.Range(zSpawnRangeLo, zSpawnRangeHi);

        Vector3 spawnPos = new Vector3(x, y, z);
        
        return spawnPos;
    }

    //Verify that a spawnPos does not collide with an existing target.
    bool CollisionOccurs(Vector3 spawnPos)
    {
        bool collisionOccurs = false;

        colliders = Physics.OverlapSphere(spawnPos, radius);

        for (int i = 0; i < colliders.Length;i++) 
        {
            Vector3 lowerLeft = colliders[i].bounds.min;
            Vector3 upperRight = colliders[i].bounds.max;

            collisionOccurs = (spawnPos.x + halfx >= lowerLeft.x && upperRight.x >= spawnPos.x -halfx) &&
                                (spawnPos.y + halfy >= lowerLeft.y && upperRight.y >= spawnPos.y - halfy) &&
                                (spawnPos.z + halfz >= lowerLeft.z && upperRight.z >= spawnPos.z - halfz);

            if (collisionOccurs)
            {
                break;
            }
        }

        return collisionOccurs;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
