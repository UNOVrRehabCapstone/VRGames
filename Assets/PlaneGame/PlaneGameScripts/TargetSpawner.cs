/**
 * \file TargetSpawner.cs
 * \brief Manages the spawning of target objects within specified spatial bounds, ensuring they do not overlap.
 *
 * The TargetSpawner class is responsible for generating targets in a designated area, with a mechanism to prevent overlap among spawned targets. It supports customizable spawn ranges, target quantities, and ensures conflict-free target placement.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanesGame{
/**
 * \class TargetSpawner
 * \brief Spawns target objects within a specified area, ensuring they do not overlap.
 *
 * This class spawns a specified number of target objects within defined spatial bounds. It ensures that new targets do not overlap with existing ones by checking the intended spawn location for conflicts before instantiation.
 */
public class TargetSpawner : MonoBehaviour
{
    [Header("Referenced Game Objects")]
    //The Target GameObject
    [SerializeField]
    GameObject targetPrefab;

    [SerializeField]
    GameObject leftBoundBox;

    [SerializeField]
    GameObject rightBoundBox;

    [SerializeField]
    GameObject closeBoundBox;

    [SerializeField]
    GameObject farBoundBox;

    [SerializeField]
    GameObject allBoundBox;

    [Header("Dynamic Target Spawns")]
    //Number of "all" targets (targets anywhere within the bounds) to spawn.
    [SerializeField]
    int allTargets;

    //Number of left targets to spawn.
    [SerializeField]
    int leftTargets;

    //Number of right targets to spawn.
    [SerializeField]
    int rightTargets;

    //Number of close targets to spawn.
    [SerializeField]
    int closeTargets;

    //Number of far targets to spawn.
    [SerializeField]
    int farTargets;

    [Header("Initial Target Spawns")]
    //Number of "all" targets (targets anywhere within the bounds) to spawn initially.
    [SerializeField]
    public static int initallTargets;

    //Number of left targets to spawn initially.
    [SerializeField]
    public static int initleftTargets;

    //Number of right targets to spawn initially.
    [SerializeField]
    public static int initrightTargets;

    //Number of close targets to spawn initially.
    [SerializeField]
    public static int initcloseTargets;

    //Number of far targets to spawn initially.
    [SerializeField]
    public static int initfarTargets;

    [Header("Miscellaneous")]
    //Tracks the number of target objects currently in the scene
    [SerializeField]
    int targetsInScene;

    //Used for spawning calculations
    [SerializeField]
    float radius;

    //used for collision free spawning
    Collider[] colliders;

    //used for collision free spawning
    private Bounds allBounds;
    private Bounds leftBounds;
    private Bounds rightBounds;
    private Bounds closeBounds;
    private Bounds farBounds;
    
    //The size of the Target prefab [Obtained automatically in Start()]
    float halfx;
    float halfy;
    float halfz;

    //Orients the target Prefab properly
    Quaternion targetOrientation = Quaternion.Euler(0, -90, 90);

    //Deprecated: did not support spawning targets in specified areas
    //The bounds of the box in which to randomly spawn the targets.
    //float xSpawnRangeLo;
    //float xSpawnRangeHi;
    //float ySpawnRangeLo;
    //float ySpawnRangeHi;
    //float zSpawnRangeLo;
    //float zSpawnRangeHi;



    /**
     * \brief Initializes target spawning by defining bounds and instantiating initial targets.
     *
     * Start is called before the first frame update. It sets up the spawn range, calculates target dimensions, and spawns the initial set of targets.
     */

    // Start is called before the first frame update
    void Start()
    {
        //A box containing the entire plane spawning bounds.
        allBounds = allBoundBox.GetComponent<Collider>().bounds;
        //The bounds of the left target box.
        leftBounds = leftBoundBox.GetComponent<Collider>().bounds;
        //The bounds of the right target box.
        rightBounds = rightBoundBox.GetComponent<Collider>().bounds;
        //the bounds of the far target box.
        farBounds = farBoundBox.GetComponent<Collider>().bounds;
        //the bounds of the close target box.
        closeBounds = closeBoundBox.GetComponent<Collider>().bounds;

        //Deprecated: did not support spawning targets in specified areas.
        //Define the bounds within which to spawn targets
        //xSpawnRangeLo = -5;
        //xSpawnRangeHi = 5;
        //ySpawnRangeLo = 1;
        //ySpawnRangeHi = 3;
        //zSpawnRangeLo = 5;
        //zSpawnRangeHi = 15;

        //Spawn a dummy target way out of bounds to retrieve its dimensions
        GameObject tempTarget = SpawnTarget(new Vector3(1000, 1000, 0));
        MeshRenderer tempTargetMeshRenderer = tempTarget.GetComponent<MeshRenderer>();

        halfx = tempTargetMeshRenderer.bounds.extents.x;
        halfy = tempTargetMeshRenderer.bounds.extents.y;
        halfz = tempTargetMeshRenderer.bounds.extents.z;

        Destroy(tempTarget);

        //Spawn the initial "wave" of targets
        SpawnConflictFree(allTargets, allBounds);
        allTargets = 0;
        SpawnConflictFree(leftTargets, leftBounds);
        leftTargets = 0;
        SpawnConflictFree(rightTargets, rightBounds);
        rightTargets = 0;
        SpawnConflictFree(farTargets, farBounds);
        farTargets = 0;
        SpawnConflictFree(closeTargets, closeBounds);
        closeTargets = 0;

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
    void SpawnConflictFree(int n, Bounds bounds)
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
                spawnPos = SpawnInBounds(bounds);
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

    //Deprecated: did not support spawning targets in specified areas
    //Use Random to generate a potential spawn position.
    /**
     * \brief Generates a random spawn position within the defined bounds.
     * 
     * \return A Vector3 representing the generated spawn position.
     */
    //Vector3 SpawnInRange()
    //{
    //    float x = Random.Range(xSpawnRangeLo, xSpawnRangeHi);
    //    float y = Random.Range(ySpawnRangeLo, ySpawnRangeHi);
    //    float z = Random.Range(zSpawnRangeLo, zSpawnRangeHi);
    //
    //    Vector3 spawnPos = new Vector3(x, y, z);
    //    
    //    return spawnPos;
    //}

    Vector3 SpawnInBounds(Bounds bounds)
    {
        float withinX = Random.Range(-bounds.extents.x, bounds.extents.x);
        float withinY = Random.Range(-bounds.extents.y, bounds.extents.y);
        float withinZ = Random.Range(-bounds.extents.z, bounds.extents.z);


        Vector3 spawnPos = new Vector3(bounds.center.x + withinX, bounds.center.y + withinY, bounds.center.z + withinZ);

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

        LayerMask mask = LayerMask.GetMask("Target");

        colliders = Physics.OverlapSphere(spawnPos, radius, mask);

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

    //Public function to track the addition of one target to the scene
    public void AddTarget()
    {
        this.targetsInScene++;
    }

    //Public function to track the removal of one target from the scene
    public void RemoveTarget()
    {
        this.targetsInScene--;
    }

    //Public function to add targets to the scene in a specified area
    // Anywhere: 0
    // Left: 1
    // Right: 2
    // Close: 3
    // Far: 4
    public void QueueTargetSpawn(int numTargets, int area)
    {
        if (numTargets > 0)
        {
            switch (area)
            {
                case 0:
                    if (this.allTargets < 0)
                    {
                        this.allTargets = 0;
                    }
                    this.allTargets += numTargets;
                    break;
                case 1:
                    if (this.leftTargets < 0)
                    {
                        this.leftTargets = 0;
                    }
                    this.leftTargets += numTargets;
                    break;
                case 2:
                    if (this.rightTargets < 0)
                    {
                        this.rightTargets = 0;
                    }
                    this.rightTargets += numTargets;
                    break;
                case 3:
                    if (this.closeTargets < 0)
                    {
                        this.closeTargets = 0;
                    }
                    this.closeTargets += numTargets;
                    break;
                case 4:
                    if (this.farTargets < 0)
                    {
                        this.farTargets = 0;
                    }
                    this.farTargets += numTargets;
                    break;
            }
        }
    }

    //Public function to add exactly one target to the scene
    public void SpawnOneTarget(int area)
    {
        QueueTargetSpawn(1, area);
    }



    // Update is called once per frame
    void Update()
    {
       if(this.targetsInScene <= 0)
        {
            if (initallTargets > 0)
            QueueTargetSpawn(initallTargets, 0);
            
            if (initleftTargets > 0)
            QueueTargetSpawn(initleftTargets, 1);

            if (initrightTargets > 0)
            QueueTargetSpawn(initrightTargets, 2);

            if (initcloseTargets > 0)
            QueueTargetSpawn(initcloseTargets, 3);

            if (initfarTargets > 0)
            QueueTargetSpawn(initfarTargets, 4);

        }

        while (this.allTargets > 0)
        {
            SpawnConflictFree(1, allBounds);
            this.allTargets--;
        }
        while (this.leftTargets > 0)
        {
            SpawnConflictFree(1, leftBounds);
            this.leftTargets--;
        }
        while (this.rightTargets > 0)
        {
            SpawnConflictFree(1, rightBounds);
            this.rightTargets--;
        }
        while (this.closeTargets > 0)
        {
            SpawnConflictFree(1, closeBounds);
            this.closeTargets--;
        }
        while (this.farTargets > 0)
        {
            SpawnConflictFree(1, farBounds);
            this.farTargets--;
        }
    }
}}
