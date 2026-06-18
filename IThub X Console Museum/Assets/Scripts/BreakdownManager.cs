using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakdownManager : MonoBehaviour
{
    [Header("Настройки эскалации")]
    public float initialSpawnInterval = 10f;
    public float minSpawnInterval = 2f;
    public float spawnAcceleration = 0.5f;
    public int gameOverThreshold = 8;

    [Header("UI")]
    public GameObject gameOverPanel;
    public UnityEngine.UI.Text gameOverScoreText;
    public UnityEngine.UI.Button menuButton;

    private List<BreakdownPoint> points = new List<BreakdownPoint>();
    private Coroutine spawnCoroutine;
    private float currentSpawnInterval;

    void Start()
    {
        points.AddRange(FindObjectsOfType<BreakdownPoint>());

        if (points.Count == 0)
        {
            Debug.LogWarning("BreakdownManager: на сцене нет ни одной BreakdownPoint!");
            return;
        }

        currentSpawnInterval = initialSpawnInterval;

        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);

        StartSpawning();
    }

    private void StartSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        currentSpawnInterval = initialSpawnInterval;
        spawnCoroutine = StartCoroutine(SpawnBreakdownsRoutine());
    }

    private IEnumerator SpawnBreakdownsRoutine()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            int brokenCount = points.Count(p => p.IsBroken);

            if (brokenCount >= gameOverThreshold)
            {
                TriggerGameOver();
                yield break;
            }

            var availablePoints = points.Where(p => !p.IsBroken).ToList();

            if (availablePoints.Count > 0)
            {
                BreakdownPoint pointToBreak = availablePoints[Random.Range(0, availablePoints.Count)];
                pointToBreak.Break();
                Debug.Log($"Новая поломка! Сломано: {brokenCount + 1}/{gameOverThreshold}. Интервал: {currentSpawnInterval:F1}с");
            }

            yield return new WaitForSeconds(currentSpawnInterval);

            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - spawnAcceleration);
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER!");

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverScoreText != null && SkillCheckManager.Instance != null)
            {
                gameOverScoreText.text = "СЧЁТ: " + SkillCheckManager.Instance.score;
            }
        }

        if (SkillCheckManager.Instance != null)
        {
            SkillCheckManager.Instance.CheckHighScore();
        }
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;

        // Скрыть Game Over панель
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Скрыть Input Panel (если открыта)
        if (HighScoreManager.Instance != null && HighScoreManager.Instance.inputPanel != null)
            HighScoreManager.Instance.inputPanel.SetActive(false);

        // Показать стартовую панель
        if (StartPanelManager.Instance != null)
        {
            StartPanelManager.Instance.startPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        // Сбросить счёт
        if (SkillCheckManager.Instance != null)
            SkillCheckManager.Instance.ResetScore();

        // Починить все точки
        foreach (var point in points)
        {
            if (point.IsBroken)
                point.Fix();
        }

        // Перезапустить спавн
        StartSpawning();

        Debug.Log("Возврат в меню. Счёт сброшен, поломки починены.");
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }
}