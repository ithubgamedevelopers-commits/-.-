using UnityEngine;
using System;

public class SkillCheckUI : MonoBehaviour
{
    [Header("Ссылки")]
    public GameObject panel;
    public RectTransform track;
    public RectTransform targetZone;
    public RectTransform arrow;
    public UnityEngine.UI.Text missText;

    [Header("Настройки")]
    public float speed = 350f;
    public float margin = 15f;
    [Tooltip("Насколько стрелка опущена относительно центра полоски (отрицательное число = вниз)")]
    public float arrowYOffset = -30f;

    private float currentX;
    private int direction = 1;
    private bool isActive;
    private Action onSuccess;
    private Action onFail;

    void Update()
    {
        if (!isActive) return;

        // 1. Движение
        currentX += speed * direction * Time.deltaTime;

        // 2. Жёсткие границы (учитываем ширину самой стрелки)
        float trackHalf = track.rect.width / 2f;
        float arrowHalf = arrow.rect.width / 2f;
        float limit = trackHalf - arrowHalf;

        if (currentX > limit)
        {
            currentX = limit;
            direction = -1;
        }
        else if (currentX < -limit)
        {
            currentX = -limit;
            direction = 1;
        }

        // 3. Применяем позицию
        arrow.anchoredPosition = new Vector2(currentX, arrowYOffset);

        // 4. Ввод
        if (Input.GetKeyDown(KeyCode.Space))
            CheckHit();
    }

    public void StartCheck(Action success, Action fail)
    {
        onSuccess = success;
        onFail = fail;

        panel.SetActive(true);
        if (missText != null) missText.gameObject.SetActive(false);

        isActive = true;
        direction = 1;

        float trackHalf = track.rect.width / 2f;
        float arrowHalf = arrow.rect.width / 2f;
        currentX = -trackHalf + arrowHalf; // Стартуем от левого края
        arrow.anchoredPosition = new Vector2(currentX, arrowYOffset);

        RandomizeZone();
    }

    private void RandomizeZone()
    {
        float trackHalf = track.rect.width / 2f;
        float zoneHalf = targetZone.rect.width / 2f;
        
        // Зона не должна вылезать за края трека
        float minX = -trackHalf + zoneHalf;
        float maxX = trackHalf - zoneHalf;

        targetZone.anchoredPosition = new Vector2(UnityEngine.Random.Range(minX, maxX), 0f);
    }

    private void CheckHit()
    {
        float arrowHalf = arrow.rect.width / 2f;
        float zoneHalf = targetZone.rect.width / 2f;
        float zoneX = targetZone.anchoredPosition.x;

        // Вычисляем реальные края объектов в локальном пространстве Track
        float arrowLeft = currentX - arrowHalf;
        float arrowRight = currentX + arrowHalf;
        
        float zoneLeft = zoneX - zoneHalf - margin;
        float zoneRight = zoneX + zoneHalf + margin;

        // Стандартная проверка пересечения отрезков
        if (arrowRight >= zoneLeft && arrowLeft <= zoneRight)
        {
            Debug.Log("✅ ПОПАДАНИЕ!");
            isActive = false;
            panel.SetActive(false);
            onSuccess?.Invoke();
        }
        else
        {
            Debug.Log("❌ ПРОМАХ");
            onFail?.Invoke();
        }
    }

    public void ResetArrow()
    {
        float trackHalf = track.rect.width / 2f;
        float arrowHalf = arrow.rect.width / 2f;
        
        currentX = -trackHalf + arrowHalf;
        direction = 1;
        arrow.anchoredPosition = new Vector2(currentX, arrowYOffset);
        
        if (missText != null)
        {
            missText.gameObject.SetActive(true);
            Invoke(nameof(HideMissText), 1f);
        }
    }

    private void HideMissText()
    {
        if (missText != null) missText.gameObject.SetActive(false);
    }
}