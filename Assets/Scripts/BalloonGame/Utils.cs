using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static void AdjustSpawn(GameObject obj, float deltaX, float deltaY, float deltaZ)
    {
        Vector3 objPosition = obj.transform.position;

        objPosition.x += deltaX;
        objPosition.y += deltaY;
        objPosition.z += deltaZ;
    }
}
