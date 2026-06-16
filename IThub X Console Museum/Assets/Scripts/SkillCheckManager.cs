using UnityEngine;
using TMPro;

public class SkillCheckManager : MonoBehaviour
{
    public static SkillCheckManager Instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("UI")]
    public GameObject skillCheckPanel;
    private SkillCheckUI currentCheck;
    private GameObject activeTrigger;

    void Awake() => Instance = this;

    public void StartSkillCheck(GameObject trigger)
    {
        activeTrigger = trigger;
        skillCheckPanel.SetActive(true);
        currentCheck = skillCheckPanel.GetComponent<SkillCheckUI>();
        currentCheck.StartCheck(OnSuccess, OnFail);
    }

    private void OnSuccess()
    {
        score += 100;
        UpdateScoreUI();
        skillCheckPanel.SetActive(false);
        if (activeTrigger != null) Destroy(activeTrigger);
    }

    private void OnFail()
    {
        score -= 500;
        UpdateScoreUI();
        currentCheck.ShowMissText();
        currentCheck.ResetArrow(); // Скиллчек продолжается
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Очки: {score}";
    }
}