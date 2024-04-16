using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use for classes that trigger code when grabbed/released.
public interface IGrabEvent
{
    void onGrab(GameObject hand);
    void onRelease(GameObject hand);
}
