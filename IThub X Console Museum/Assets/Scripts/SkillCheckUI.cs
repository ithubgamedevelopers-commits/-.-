using UnityEngine;
using TMPro;
using System; // Нужно для Action

public class SkillCheckUI : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject uiPanel;
    public RectTransform track;
    public RectTransform targetZone;
    public RectTransform arrow;

    [Header("Текст промаха")]
    public TextMeshProUGUI missText;
    public float missTextDuration = 1f;
    private float missTimer;

    [Header("Настройки")]
    public float arrowSpeed = 300f;
    public KeyCode confirmKey = KeyCode.Space;
    public float hitMargin = 5f;

    private float arrowX;
    private int direction = 1;
    private bool isActive;

    private Action onSuccess;
    private Action onFail;

    void Update()
    {
        if (!isActive) return;

        float trackWidth = track.rect.width;
        float halfWidth = trackWidth / 2f;

        // Двигаем стрелку
        arrowX += arrowSpeed * direction * Time.deltaTime;

        // Отскок от краев
        if (arrowX >= halfWidth)
        {
            arrowX = halfWidth;
            direction = -1;
        }
        else if (arrowX <= -halfWidth)
        {
            arrowX = -halfWidth;
            direction = 1;
        }

        arrow.anchoredPosition = new Vector2(arrowX, 0f);

        // Проверка нажатия кнопки
        if (Input.GetKeyDown(confirmKey))
        {
            CheckHit();
        }

        // Таймер скрытия текста промаха
        if (missTimer > 0)
        {
            missTimer -= Time.deltaTime;
            if (missTimer <= 0 && missText != null)
            {
                missText.gameObject.SetActive(false);
            }
        }
    }

    public void StartCheck(Action successCallback, Action failCallback)
    {
        onSuccess = successCallback;
        onFail = failCallback;

        uiPanel.SetActive(true);
        isActive = true;
        direction = 1;
        arrowX = -track.rect.width / 2f;
        arrow.anchoredPosition = new Vector2(arrowX, 0f);

        if (missText != null) missText.gameObject.SetActive(false);

        RandomizeTargetZone();
    }

    private void RandomizeTargetZone()
    {
        float trackWidth = track.rect.width;
        float zoneWidth = targetZone.rect.width;
        float halfTrack = trackWidth / 2f;
        float halfZone = zoneWidth / 2f;

        float minX = -halfTrack + halfZone;
        float maxX = halfTrack - halfZone;
        
        // ИСПРАВЛЕНИЕ: Явно указываем UnityEngine.Random, чтобы не было конфликта с System.Random
        float randomX = UnityEngine.Random.Range(minX, maxX);
        targetZone.anchoredPosition = new Vector2(randomX, 0f);
    }

    private void CheckHit()
    {
        float zoneX = targetZone.anchoredPosition.x;
        float zoneHalfWidth = targetZone.rect.width / 2f;
        
        if (arrowX >= (zoneX - zoneHalfWidth - hitMargin) && arrowX <= (zoneX + zoneHalfWidth + hitMargin))
        {
            Debug.Log("SkillCheck: УСПЕХ!");
            StopCheck();
            onSuccess?.Invoke();
        }
        else
        {
            Debug.Log("SkillCheck: ПРОМАХ!");
            // Не вызываем StopCheck(), чтобы игрок мог попробовать снова, 
            // а менеджер сам вызовет ResetArrow() и ShowMissText()
            onFail?.Invoke();
        }
    }

    // --- Методы, которые вызывает SkillCheckManager ---

    public void ShowMissText()
    {
        if (missText != null)
        {
            missText.gameObject.SetActive(true);
            missTimer = missTextDuration;
        }
    }

    public void ResetArrow()
    {
        arrowX = -track.rect.width / 2f;
        direction = 1;
        arrow.anchoredPosition = new Vector2(arrowX, 0f);
    }

    private void StopCheck()
    {
        isActive = false;
        uiPanel.SetActive(false);
    }
}