using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingFloorManager : MonoBehaviour
{

    public float floorSpeed = .01f;
    public float LevelOneSpeed = .01f;
    public float LevelTwoSpeed = .01f;
    public float LevelThreeSpeed = .01f;

    private GameObject player;
    public GameObject Manager;
    private Vector3 moveDirection = Vector3.up;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        setFloorSpeed();
        raiseFloor();
    }

    void raiseFloor()
    {
        if (transform.position.y <= 20)
        {
            transform.Translate(Vector3.up * Time.deltaTime * floorSpeed);
        }
    }

    void setFloorSpeed()
    {
        int difficultyLevel = Manager.GetComponent<GameplayManager>().getDifficulty();
        switch(difficultyLevel)
        {
            case 1: 
                floorSpeed = LevelOneSpeed;
                break;
            case 2: 
                floorSpeed = LevelTwoSpeed;
                break;
            case 3: 
                floorSpeed = LevelThreeSpeed;
                break;
        }
    }
}
