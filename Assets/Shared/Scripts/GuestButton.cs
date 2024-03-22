using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



//This script controls the guest button and the different sub-buttons. - EO
public class GuestButton : MonoBehaviour
{
    public GameObject balloonButton;
    public GameObject planesButton;
    public GameObject climbingButton;
    public GameObject blocksButton;
    // Start is called before the first frame update
    void Start()
    {
        //To determine if this is the main button, check if there is a binded sub-button.
        //If this is the main guest button, disable all sub-buttons.
        if (balloonButton != null)
        {
            balloonButton.SetActive(false);
            planesButton.SetActive(false);
            climbingButton.SetActive(false);
            blocksButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //When a sub-button is clicked, the scene is changed to the scene with the same name as the button
    public void changeScene()
    {

        SceneManager.LoadScene(gameObject.name);
    }

    //When the main button is clicked, it toggles the group of sub-buttons on and off
    public void toggleShowGames()
    {
        balloonButton.SetActive(!balloonButton.activeSelf);
        planesButton.SetActive(!planesButton.activeSelf);
        climbingButton.SetActive(!climbingButton.activeSelf);
        blocksButton.SetActive(!blocksButton.activeSelf);


    }
}
