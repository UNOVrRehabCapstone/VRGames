using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

/**
 * The Balloon_Unit class represents a balloon that contains other balloons, that is a unit of 
 * balloons. This class is useful for when you want the movement of a set of balloons to be 
 * defined by a single transform.
 */
public class Balloon_Unit : _BaseBalloon
{
    private void Start()
    {
        /* Set the spawn location for each of the children. Necessary if you want the scripts of
           the children to easily extend other classes. */
        foreach (Transform child in gameObject.transform) {
            child.gameObject.GetComponent<_BaseBalloon>().SetSpawnLoc(this.spawnLoc);
        }
    }

    private void Update()
    {
        Transform transform = gameObject.transform;
        transform.position = Vector3.Lerp(transform.position, transform.position 
                                        + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength * BalloonGameplayManager.Instance.GetGameSettings().floatStrengthModifier);
    }
}
