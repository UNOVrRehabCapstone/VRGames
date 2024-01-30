using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Classes.Managers
{
    public class BoxAndBlocksGameplayManager : GameplayManager
    {
        public int blockInterval;
        public GameObject blockPrefab;
        public Material[] blockMaterials;
        public Material grabbedMat;
        public bool leftHand;
        public GameObject leftHandBtn;
        public GameObject rightHandBtn;
        private int currBlocks;
        private List<GameObject> spawnedBlocks = new List<GameObject>();
        private GameObject goalSide;
        private GameObject startSide;
        private bool timerIsRunning;
        private float timeRemaining;
        private bool isGrabbed;
        private bool isValidPoint;

        public bool Grabbed
        {
            get { return isGrabbed; }
            set { isGrabbed = value; }
        }

        public bool ValidPoint
        {
            get { return isValidPoint;}
            set { isValidPoint = value; }
        }

        new void Start()
        {
            base.Start();
            timeRemaining = maxTime;
            PointsManager.updateScoreboardMessage("BOX & BLOCKS!");
        }

        private void FixedUpdate()
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTimer(timeRemaining);
                }
                else
                {
                    timeRemaining = 0;
                    DisplayTimer(timeRemaining);
                    onTimeExpired();
                    timerIsRunning = false;
                }
            }
        }

        public void onFirstPointReached()
        {
            PointsManager.updateScoreboardMessage("Congrats on your 1st point!!!");
        }

        public void onTimeExpired()
        {
            reset();
            PointsManager.updateScoreboardMessage("Sit Still And Quiet!");
            toggleButtons(true);
        }
        
        public async void toggleButtons(bool toggle)
        {
            await Task.Delay(3000);
            leftHandBtn.gameObject.SetActive(toggle);
            rightHandBtn.gameObject.SetActive(toggle);
        }
        
        public void startSpawning()
        {
            timerIsRunning = true;
            if (startSide != null && goalSide != null)
            {
                PointsManager.resetBothHandPoints();
                InvokeRepeating("spawnBlock", 0, 0.0009f);
            }
        }
        //Called by Grabber when some Block in list is grabbed
        public void onBlockGrabbed(GameObject block)
        {
            Grabbed = true;
        }

        public void onBlockReleased(GameObject block)
        {
            Grabbed = false;
        }
        
        //Removes a Block from the list and destroys it
        public void killBlock(GameObject block)
        {
            if (block != null)
            {
                spawnedBlocks.Remove(block);
                Destroy(block);
            }
        }
        
        public override void reset()
        {
            foreach (GameObject block in spawnedBlocks)
            {
                Destroy(block);
            }
            timeRemaining = maxTime;
        }
        
        public GameObject genRandomBlock()
        {
            GameObject block = Instantiate(blockPrefab,
                startSide.transform.position + new Vector3(Random.Range(-.2f, .2f), 0, Random.Range(-.2f, .2f)),
                Quaternion.identity);
            block.GetComponent<MeshRenderer>().material = blockMaterials[Random.Range(0, blockMaterials.Length)];
            return block;
        }
        
        public void setStartSide(GameObject side)
        {
            startSide = side;
        }
        
        public void setGoalSide(GameObject side)
        {
            goalSide = side;
        }
        
        public GameObject getGoalSide()
        {
            return goalSide;
        }
        
        void DisplayTimer(float time)
        {
            GameObject scoreboardTimer = GameObject.FindGameObjectWithTag("TimerText");
            scoreboardTimer.GetComponentInChildren<TextMesh>().text = time.ToString("0.00");
        }
        
        void spawnBlock()
        { 
            spawnedBlocks.Add(genRandomBlock());
            currBlocks++;
            if (currBlocks % blockInterval == 0)
            {
                CancelInvoke();
                PointsManager.updateScoreboardMessage("Start Grabbing Blocks!");
            }
        }
    }
}