using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The _BaseBalloon is an abstract class that defines the bare minimum for what it means to be 
 * a balloon.
 */
public abstract class _BaseBalloon : MonoBehaviour
{
    [SerializeField] protected float floatStrength;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected int   pointValue;
    protected GameObject             spawnLoc;

    /**
     * The GetSpawnLoc method returns the spawn location of the balloon.
     *
     * @returns A game object which is the spawn location.
     */
    public GameObject GetSpawnLoc()
    {
        return this.spawnLoc;
    }

    /**
     * The SetSpawnLoc method sets the spawn location of the balloon.
     *
     * @param spawnLoc The spawn location of the balloon.
     */
    public virtual void SetSpawnLoc(GameObject spawnLoc)
    {
        this.spawnLoc = spawnLoc;
    }
}
