using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Acts as the Start method in most classes. Use this instead to make sure player has been instantiated and isn't null.
public interface IKnowsPlayer
{
    void onPlayerCreated(GameObject p);
}
