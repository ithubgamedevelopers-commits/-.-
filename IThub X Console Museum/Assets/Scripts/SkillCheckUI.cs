using System.Numerics;
using TMPro;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class SkillCheckUI : MonoBehaviour
{
    public RectTransform track;       // Красная полоска (Pivot: center)
    public RectTransform greenZone;   // Зеленая зона (дочерний объект track)
    public RectTransform arrowVisual; // Модель стрелки

    [Header("Настройки")]
    public float arrowSpeed = 3f;
    [Tooltip("Доля от визуального размера стрелки, которая считается попаданием (0.5 = в 2 раза меньше)")]
    public float hitboxMultiplier = 0.5f;

    public TextMeshProUGUI missText;
    public float missTextDuration = 1f;

    private System.Action onSuccess;
    private System.Action onFail;
    private float arrowPos; // От -width/2 до width/2
    private float direction = 1f;
    private bool isActive;
    private float missTimer;

    void Update()
    {
        if (!isActive) return;

        float trackWidth = track.rect.width;
        arrowPos += arrowSpeed * direction * Time.deltaTime;

        if (arrowPos >= trackWidth / 2f || arrowPos <= -trackWidth / 2f)
            direction *= -1f;

        arrowVisual.anchoredPosition = new Vector2(arrowPos, 0);

        if (Input.GetKeyDown(KeyCode.Space)) // Кнопка действия
            CheckHit();

        if (missTimer > 0)
        {
            missTimer -= Time.deltaTime;
            if (missTimer <= 0) missText.gameObject.SetActive(false);
        }
    }

    public void StartCheck(System.Action success, System.Action fail)
    {
        onSuccess = success;
        onFail = fail;
        isActive = true;
        arrowPos = -track.rect.width / 2f;
        direction = 1f;
        missText.gameObject.SetActive(false);
    }

    private void CheckHit()
    {
        float effectiveArrowWidth = arrowVisual.rect.width * hitboxMultiplier;
        float arrowLeft = arrowPos - (effectiveArrowWidth / 2f);
        float arrowRight = arrowPos + (effectiveArrowWidth / 2f);

        float greenLeft = greenZone.anchoredPosition.x - (greenZone.rect.width / 2f);
        float greenRight = greenZone.anchoredPosition.x + (greenZone.rect.width / 2f);

        // Проверка пересечения уменьшенной зоны стрелки с зеленой зоной
        if (arrowRight >= greenLeft && arrowLeft <= greenRight)
        {
            isActive = false;
            onSuccess?.Invoke();
        }
        else
        {
            onFail?.Invoke();
        }
    }

    public void ShowMissText()
    {
        missText.gameObject.SetActive(true);
        missTimer = missTextDuration;
    }

    public void ResetArrow()
    {
        arrowPos = -track.rect.width / 2f;
        direction = 1f;
    }
}