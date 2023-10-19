using System;
using System.Collections;
using Classes.Managers;
using UnityEngine;

public class StreamSpawnBalloon : MonoBehaviour
{
    public float floatStrength;
    public GameObject scorePopupPrefab;
    private BalloonGameplayManager manager;
    private int spawnCount = 5;

    void Start()
    {
        manager = (BalloonGameplayManager)GameplayManager.getManager();
        this.tag = "SpawnStream";
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position
                                                              + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint"))
        {
            Debug.Log("Popped stream balloon.");
            GetComponent<AudioSource>().Play();
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInParent<Rigidbody>().useGravity = true;

            SpawnStream();

            ShowScorePopup();
        }
    }

    void ShowScorePopup()
    {
        GameObject popup = Instantiate(scorePopupPrefab);
        Vector3 newVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        popup.transform.position = newVector;
    }

    void SpawnStream()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            yield return new WaitForSeconds(0.1f);
            BalloonManager.Instance.SpawnBalloons(true, true);
        }
    }

}
