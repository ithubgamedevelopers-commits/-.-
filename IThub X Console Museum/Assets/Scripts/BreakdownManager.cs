using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakdownManager : MonoBehaviour
{
    [Header("Настройки эскалации")]
    [Tooltip("Максимальное количество одновременных поломок")]
    [SerializeField] private int maxSimultaneousBreakdowns = 5;
    
    [Tooltip("Интервал проверки и добавления поломки (в секундах)")]
    [SerializeField] private float spawnInterval = 15f;
    
    [Tooltip("Задержка перед самой первой поломкой в начале игры")]
    [SerializeField] private float startDelay = 3f;

    private List<BreakdownPoint> points = new List<BreakdownPoint>();
    private Coroutine spawnCoroutine;

    private void Start()
    {
        // Находим все точки на сцене
        points.AddRange(FindObjectsOfType<BreakdownPoint>());

        if (points.Count == 0)
        {
            Debug.LogWarning("BreakdownManager: на сцене нет ни одной BreakdownPoint!");
            return;
        }

        // Защита: если точек на сцене меньше 5, ограничиваем максимум их количеством
        maxSimultaneousBreakdowns = Mathf.Min(maxSimultaneousBreakdowns, points.Count);

        // Запускаем бесконечный цикл проверки
        spawnCoroutine = StartCoroutine(SpawnBreakdownsRoutine());
    }

    private IEnumerator SpawnBreakdownsRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // Считаем, сколько точек сейчас сломано
            int brokenCount = points.Count(p => p.IsBroken);

            // Если сломано меньше максимума, ломаем еще одну случайную исправную точку
            if (brokenCount < maxSimultaneousBreakdowns)
            {
                var availablePoints = points.Where(p => !p.IsBroken).ToList();
                
                if (availablePoints.Count > 0)
                {
                    BreakdownPoint pointToBreak = availablePoints[Random.Range(0, availablePoints.Count)];
                    pointToBreak.Break();
                    Debug.Log($"Появилась новая поломка! Всего сломано: {brokenCount + 1}");
                }
            }

            // Ждем 15 секунд до следующей проверки
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }
}