using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject triggerPrefab;
    public List<Transform> spawnZones;

    [Header("Настройки спавна")]
    public float initialSpawnRate = 5f;
    public float spawnRateIncrement = 0.5f; // На сколько секунд ускоряется спавн
    public float minSpawnRate = 1f;         // Минимальный интервал
    public float timeBetweenIncrements = 30f; // Интервал увеличения сложности

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
                Debug.Log($"Спавн ускорен! Новый интервал: {currentSpawnRate:F2}с");
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