using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakdownManager : MonoBehaviour
{
    [Header("Настройки эскалации")]
    public int maxSimultaneousBreakdowns = 5;
    public float spawnInterval = 15f;
    public float startDelay = 3f;

    private List<BreakdownPoint> points = new List<BreakdownPoint>();
    private Coroutine spawnCoroutine;

    void Start()
    {
        points.AddRange(FindObjectsOfType<BreakdownPoint>());

        if (points.Count == 0)
        {
            Debug.LogWarning("BreakdownManager: на сцене нет ни одной BreakdownPoint!");
            return;
        }

        maxSimultaneousBreakdowns = Mathf.Min(maxSimultaneousBreakdowns, points.Count);

        spawnCoroutine = StartCoroutine(SpawnBreakdownsRoutine());
    }

    private IEnumerator SpawnBreakdownsRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            int brokenCount = points.Count(p => p.IsBroken);

            if (brokenCount < maxSimultaneousBreakdowns)
            {
                var availablePoints = points.Where(p => !p.IsBroken).ToList();
                
                if (availablePoints.Count > 0)
                {
                    BreakdownPoint pointToBreak = availablePoints[Random.Range(0, availablePoints.Count)];
                    pointToBreak.Break();
                    Debug.Log($" Новая поломка! Всего сломано: {brokenCount + 1}/{maxSimultaneousBreakdowns}");
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }
}