using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes.Managers
{
    public class LavaClimbGameplayManager : GameplayManager
    {
        public GameObject rockPrefab;
        public GameObject climbingWall;
        public float secondsTilDespawn = 2;

        private List<GameObject> rocks = new List<GameObject>();

        new void Start()
        {
            base.Start();
            PointsManager.updateScoreboardMessage("Start Climbing!");
            PointsManager.addPointTrigger("==", winConditionPoints, "onWinConditionPointsReached");
            spawnRock(true);
        }

        
        
        public override void onWinConditionPointsReached()
        {
            print("You survived the lava!");
            PointsManager.updateScoreboardMessage("You Survived The Lava!");
        }
        
        public void onStart()
        {
            print("Start Climbing!");
            
        }

        //Called by Grabber when some Plane in list is grabbed
        public void onRockGrabbed(GameObject rock)
        {
            PlayerManager.movePlayer(GameObject.FindGameObjectWithTag("ClimbingRock"));
/*            PlayerManager.offsetMovePlayer(GameObject.FindGameObjectWithTag());*/
            PointsManager.addPoints(1);
            if (PointsManager.getPoints() % 2 == 0) {
                spawnRock(true);
            } else {
                spawnRock(false);
            }
        }

        public void onRockReleased(GameObject rock)
        {
            StartCoroutine(despawnCountdown(rock));
        }

        //Removes a Plane from the list and destroys it
        public void killRock(GameObject rock)
        {
            rocks.Remove(rock);
            Destroy(rock);
        }

        //Kills the passed rock and creates a new one
        public void respawnRock(GameObject rock)
        {
            killRock(rock);
            spawnRock(true);
        }

        //Kills all rocks in scene
        public override void reset()
        {
            foreach (GameObject rock in rocks)
            {
                killRock(rock);
            }
        }

        //Creates a new Rock GameObject and adds it to the list
        private void spawnRock(bool left)
        {
            GameObject player = PlayerManager.getPlayer();
            GameObject temp = Instantiate(rockPrefab);
            
          
            if (left)
            {
                temp.transform.position = new Vector3(player.transform.position.x - .3f, climbingWall.transform.localScale.y -  19 + PointsManager.getPoints(), .25f); 
            }
            else
            {
                temp.transform.position = new Vector3(player.transform.position.x + .3f, climbingWall.transform.localScale.y - 19 + PointsManager.getPoints(), .25f);
            }
            
            /*if (left)
            {
               temp.transform.position = player.transform.position + new Vector3(-.3f, player.transform.localScale.y + .80f, .5f); 
            }
            else
            {
                temp.transform.position = player.transform.position + new Vector3(.3f, player.transform.localScale.y + .80f, .5f);
            }*/
            
            rocks.Add(temp);
        }

        //Coroutine for counting down and despawning rock after certain amount of time
        private IEnumerator despawnCountdown(GameObject rock)
        {
            var endTime = Time.realtimeSinceStartup + secondsTilDespawn;
            while (Time.realtimeSinceStartup < endTime)
            {
                yield return new WaitForSeconds(.5f);
            }

            killRock(rock);
        }
    }
}