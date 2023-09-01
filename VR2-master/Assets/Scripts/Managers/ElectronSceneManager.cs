using UnityEngine;
using UnityEngine.SceneManagement;

namespace Classes.Managers
{
    public class ElectronSceneManager : MonoBehaviour
    {
        public static void GoToServer()
        {
            SceneManager.LoadScene("Server");
        }
        public static void GoToBoxAndBlocks()
        {
            SceneManager.LoadScene("Blocks");
        }
        
        public static void GoToBalloonGame()
        {
            SceneManager.LoadScene("Balloons");
        }
        
        public static void GoToClimbingGame()
        {
            SceneManager.LoadScene("Climbing");
        }
        
        public static void GoToPlaneGame()
        {
            SceneManager.LoadScene("Planes");
        }
        public static void GoToTestGame()
        {
            SceneManager.LoadScene("TestAddition");
        }
    }
}