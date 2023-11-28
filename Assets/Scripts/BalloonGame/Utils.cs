using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    /**
     * The AdjustPosition method adjusts the position of the gameobject using the passed in offset
     * values.
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
