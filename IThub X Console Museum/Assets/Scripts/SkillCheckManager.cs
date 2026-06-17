using UnityEngine;
using TMPro;

public class SkillCheckManager : MonoBehaviour
{
    public static SkillCheckManager Instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;
    
    [Header("UI Скиллчека")]
    public GameObject skillCheckPanel;
    private SkillCheckUI currentCheck;
    private BreakdownPoint activePoint; // Изменили с GameObject на конкретный компонент

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Теперь принимаем не просто GameObject, а конкретный компонент точки
    public void StartSkillCheck(BreakdownPoint point)
    {
        activePoint = point;
        skillCheckPanel.SetActive(true);
        currentCheck = skillCheckPanel.GetComponent<SkillCheckUI>();
        
        // Передаем колбэки
        currentCheck.StartCheck(OnSuccess, OnFail);
    }

    private void OnSuccess()
    {
        score += 100;
        UpdateScoreUI();
        skillCheckPanel.SetActive(false);
        
        // ИСПРАВЛЕНИЕ: Вместо уничтожения объекта, мы просто "чиним" его.
        // Так BreakdownManager сможет снова создать здесь поломку через 15 сек.
        if (activePoint != null)
        {
            activePoint.Fix();
        }
    }

    private void OnFail()
    {
        score -= 50; // Уменьшил штраф до 50, чтобы 500 не убивали прогресс слишком быстро (можешь вернуть 500)
        UpdateScoreUI();
        
        // Показываем текст и сбрасываем стрелку, чтобы игрок мог попробовать снова
        currentCheck.ShowMissText();
        currentCheck.ResetArrow();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) 
            scoreText.text = "Очки: " + score;
    }
}