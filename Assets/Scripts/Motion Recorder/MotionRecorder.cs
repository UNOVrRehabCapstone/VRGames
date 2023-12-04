using UnityEngine;
using UnityEngine.XR;
using Network;
using System.Collections;

[RequireComponent(typeof(InputData))]
public class MotionRecorder : MonoBehaviour
{
    private InputData _inputData;
    private NetworkManager networkManager;

    private void Start()
    {
       // _inputData = GetComponent<InputData>();
      //  networkManager = NetworkManager.getManager();
       // StartCoroutine(PatientCoordinates());
    }

    private void Update()
    {
        
    }

    private IEnumerator PatientCoordinates()
    {
        while (true) {
            if (_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPosition) &&
            _inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPosition) &&
            _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
            {
                string left = leftPosition.ToString();
                string right = rightPosition.ToString();
                string head = headPosition.ToString();
                networkManager.SendPositionalData("LEFT" + leftPosition.x + "");
                networkManager.SendPositionalData("RIGHT" + right);
                networkManager.SendPositionalData("HEAD" + head);
            }
            yield return new WaitForSeconds(5f);
        }
        
    }
}