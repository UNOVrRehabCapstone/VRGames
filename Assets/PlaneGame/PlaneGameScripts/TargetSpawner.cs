/**
 * \file TargetSpawner.cs
 * \brief Manages the spawning of target objects within specified spatial bounds, ensuring they do not overlap.
 *
 * The TargetSpawner class is responsible for generating targets in a designated area, with a mechanism to prevent overlap among spawned targets. It supports customizable spawn ranges, target quantities, and ensures conflict-free target placement.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \class TargetSpawner
 * \brief Spawns target objects within a specified area, ensuring they do not overlap.
 *
 * This class spawns a specified number of target objects within defined spatial bounds. It ensures that new targets do not overlap with existing ones by checking the intended spawn location for conflicts before instantiation.
 */
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

    /**
     * \brief Initializes target spawning by defining bounds and instantiating initial targets.
     *
     * Start is called before the first frame update. It sets up the spawn range, calculates target dimensions, and spawns the initial set of targets.
     */

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

    /**
     * \brief Instantiates a target at a given position with a predefined orientation.
     * 
     * \param pos The position at which to spawn the target.
     * \return The instantiated target GameObject.
     */
    GameObject SpawnTarget (Vector3 pos)
    {
        return Instantiate(targetPrefab, pos, targetOrientation);
    }

    //Spawn n targets while checking that they are not occupying the same space.
    /**
     * \brief Spawns a specified number of targets, ensuring no spatial conflicts.
     * 
     * \param n The number of targets to spawn.
     */
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
    /**
     * \brief Generates a random spawn position within the defined bounds.
     * 
     * \return A Vector3 representing the generated spawn position.
     */
    Vector3 SpawnInRange()
    {
        float x = Random.Range(xSpawnRangeLo, xSpawnRangeHi);
        float y = Random.Range(ySpawnRangeLo, ySpawnRangeHi);
        float z = Random.Range(zSpawnRangeLo, zSpawnRangeHi);

        Vector3 spawnPos = new Vector3(x, y, z);
        
        return spawnPos;
    }

    //Verify that a spawnPos does not collide with an existing target.
    /**
     * \brief Checks for collisions at a potential spawn position to prevent overlapping targets.
     * 
     * \param spawnPos The position to check for potential collisions.
     * \return A boolean indicating whether a collision occurs at the given position.
     */
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
