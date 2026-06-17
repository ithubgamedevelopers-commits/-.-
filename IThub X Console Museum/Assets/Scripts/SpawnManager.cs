using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject triggerPrefab;
    public List<Transform> spawnZones;
    
    [Header("Настройки спавна")]
    public float initialSpawnRate = 5f;
    public float spawnRateIncrement = 0.5f; 
    public float minSpawnRate = 1f;         
    public float timeBetweenIncrements = 30f; 

    private float currentSpawnRate;
    private float timeSinceLastIncrement;

    void Start()
    {
        currentSpawnRate = initialSpawnRate;
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnRate);
            SpawnTrigger();
            
            timeSinceLastIncrement += currentSpawnRate;
            if (timeSinceLastIncrement >= timeBetweenIncrements)
            {
                currentSpawnRate = Mathf.Max(minSpawnRate, currentSpawnRate - spawnRateIncrement);
                timeSinceLastIncrement = 0f;
                Debug.Log("Спавн ускорен! Интервал: " + currentSpawnRate.ToString("F2") + "с");
            }
        }
    }

    private void SpawnTrigger()
    {
        if (spawnZones == null || spawnZones.Count == 0) return;
        Transform zone = spawnZones[Random.Range(0, spawnZones.Count)];
        Instantiate(triggerPrefab, zone.position, zone.rotation);
    }
}