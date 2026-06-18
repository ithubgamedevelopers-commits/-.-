using UnityEngine;

public class SkillCheckManager : MonoBehaviour
{
    public static SkillCheckManager Instance;

    public int score = 0;
    public UnityEngine.UI.Text scoreText;

    [Header("UI Скиллчека")]
    public GameObject skillCheckPanel;
    private SkillCheckUI skillCheckUI;

    [Header("Всплывающий текст")]
    public ScorePopup scorePopupPrefab;

    private BreakdownPoint activePoint;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (skillCheckPanel != null)
            skillCheckUI = skillCheckPanel.GetComponent<SkillCheckUI>();
    }

    public void StartCheck(BreakdownPoint point)
    {
        activePoint = point;

        if (skillCheckUI != null)
            skillCheckUI.StartCheck(OnSuccess, OnFail);
    }

    private void OnSuccess()
    {
        score += 100;
        UpdateScore();
        ShowFloatingText("+100", Color.green);
        Debug.Log("✅ Компьютер починен! +100 очков");

        if (activePoint != null)
        {
            activePoint.Fix();
            activePoint = null;
        }
    }

    private void OnFail()
    {
        score -= 500;
        UpdateScore();
        ShowFloatingText("-500", Color.red);

        if (skillCheckUI != null)
            skillCheckUI.ResetArrow();

        Debug.Log(" Промах! -500 очков");
    }

    private void ShowFloatingText(string message, Color color)
    {
        if (scorePopupPrefab == null)
        {
            Debug.LogWarning("Не установлен префаб ScorePopupPrefab!");
            return;
        }

        Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f + 100f, 0);
        ScorePopup popup = Instantiate(scorePopupPrefab, transform);
        popup.Setup(message, color, centerScreen);
    }

    private void UpdateScore()
{
    if (scoreText != null)
        scoreText.text = "SCORE: " + score;
}
}