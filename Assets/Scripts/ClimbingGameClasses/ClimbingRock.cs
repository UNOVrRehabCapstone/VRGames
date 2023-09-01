using Classes.Managers;
using UnityEngine;

namespace Classes
{
    public class ClimbingRock  : MonoBehaviour, IGrabEvent
    {
        //private bool thrown = false;
        private LavaClimbGameplayManager manager;

        void Start()
        {
            manager = (LavaClimbGameplayManager)GameplayManager.getManager();
        }

        //Called by Grabber
        public void onGrab(GameObject hand){
            manager.onRockGrabbed( gameObject );
        }
        public void onRelease(GameObject hand){
            
            manager.onRockReleased( gameObject );
        }
    }
}