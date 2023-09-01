using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountUp : MonoBehaviour {
    private int count;  //count of blocks on goal side
    public static int hand = 0;
    public  static string txt;
 
    void Start() {
        count = 0;  //start at 0
        txt = "Please Select a Hand..."; 
    }

    void Update()
    {
        //display UI each frame
        switch (hand)
        {
            case 8:
                txt = "Right Hand: ";
                break;
            case 9:
                txt = "Left Hand: ";
                break;
        }
        if (hand != 0)
        {
            gameObject.GetComponent<Text>().text = txt + count;
        }
        else
        {
            gameObject.GetComponent<Text>().text = txt;
        }
            
    }
	
	public void setCount(int count)
    {
        this.count = count;
    }

    public int getCount()
    {
        return count;
    }
    public void incrementCount()
    {
        count++; 
      
        
    }
}
