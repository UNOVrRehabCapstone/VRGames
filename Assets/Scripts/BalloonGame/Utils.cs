using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The Utils class holds some useful utily methods.
 */
public class Utils
{
    /**
     * The AdjustPosition method adjusts the position of the gameobject using the passed in offset
     * values.
     *
     * @param obj    The object whose position will be adjusted.
     * @param deltaX The offset in the x direction.
     * @param deltaY The offset in the y direction.
     * @param deltaZ The offset in the z direction.
     */
    public static void AdjustPosition(GameObject obj, float deltaX, float deltaY, float deltaZ)
    {
        Vector3 newPosition = obj.transform.position;

        newPosition.x += deltaX;
        newPosition.y += deltaY;
        newPosition.z += deltaZ;

        obj.transform.position = newPosition;
    }
}
