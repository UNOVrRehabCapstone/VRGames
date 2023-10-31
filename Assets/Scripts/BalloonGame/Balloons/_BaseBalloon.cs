using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class _BaseBalloon : MonoBehaviour
{
    [SerializeField] protected float floatStrength;
    [SerializeField] protected int   pointValue;
    protected GameObject             spawnLoc;

    public GameObject GetSpawnLoc()
    {
        return this.spawnLoc;
    }

    public virtual void SetSpawnLoc(GameObject spawnLoc)
    {
        this.spawnLoc = spawnLoc;
    }
}
