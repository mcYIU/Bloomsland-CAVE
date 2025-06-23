using System.Collections;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birds; // Prefab to spawn
    public GameObject[] spawner; // Array of spawn points
    public float spwanFrequency; // Base frequency for spawning

    void Start()
    {
        Spawn();
    }

    // Spawns a bird at a random spawn point and schedules the next spawn
    void Spawn()
    {
        int i = Random.Range(0, spawner.Length);
        Instantiate(birds, spawner[i].transform.position, Quaternion.identity);
        StartCoroutine(WaitDuration(spwanFrequency)); 
    }

    // Waits for a random duration before spawning the next bird
    private IEnumerator WaitDuration(float time)
    {
        float randomFloat = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(time * randomFloat);

        Spawn();
    }
}
