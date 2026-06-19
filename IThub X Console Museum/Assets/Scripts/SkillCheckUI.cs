using UnityEngine;
using System;
using System.Collections;

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
    public float arrowYOffset = -30f;

    [Header("Звуки")]
    public AudioSource audioSource;
    public AudioClip startSound;    // Звук начала мини-игры
    public AudioClip successSound;  // Звук попадания (с задержкой)
    public AudioClip failSound;     // Звук промаха

    [Tooltip("Задержка перед звуком победы (в секундах)")]
    public float successSoundDelay = 0.3f;

    private float currentX;
    private int direction = 1;
    private bool isActive;
    private Action onSuccess;
    private Action onFail;

    void Update()
    {
        if (!isActive) return;

        currentX += speed * direction * Time.deltaTime;

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

        arrow.anchoredPosition = new Vector2(currentX, arrowYOffset);

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
        currentX = -trackHalf + arrowHalf;
        arrow.anchoredPosition = new Vector2(currentX, arrowYOffset);

        RandomizeZone();

        // Звук начала мини-игры
        PlaySound(startSound);
    }

    private void RandomizeZone()
    {
        float trackHalf = track.rect.width / 2f;
        float zoneHalf = targetZone.rect.width / 2f;
        float minX = -trackHalf + zoneHalf;
        float maxX = trackHalf - zoneHalf;
        targetZone.anchoredPosition = new Vector2(UnityEngine.Random.Range(minX, maxX), 0f);
    }

    private void CheckHit()
    {
        float arrowHalf = arrow.rect.width / 2f;
        float zoneHalf = targetZone.rect.width / 2f;
        float zoneX = targetZone.anchoredPosition.x;

        float arrowLeft = currentX - arrowHalf;
        float arrowRight = currentX + arrowHalf;
        float zoneLeft = zoneX - zoneHalf - margin;
        float zoneRight = zoneX + zoneHalf + margin;

        if (arrowRight >= zoneLeft && arrowLeft <= zoneRight)
        {
            Debug.Log("✅ ПОПАДАНИЕ!");
            isActive = false; // Блокируем ввод, чтобы не спамить пробелом
            
            // Запускаем корутину: ждём → звук → закрытие панели
            StartCoroutine(PlaySuccessAndClose());
        }
        else
        {
            Debug.Log("❌ ПРОМАХ");
            PlaySound(failSound); // Звук промаха — сразу, без задержки
            onFail?.Invoke();
        }
    }

    private IEnumerator PlaySuccessAndClose()
    {
        // Ждём небольшую задержку перед звуком победы
        yield return new WaitForSeconds(successSoundDelay);

        // Играем звук победы (пока панель ещё активна — звук точно проиграется)
        PlaySound(successSound);

        // Ещё небольшая пауза, чтобы звук успел начаться
        yield return new WaitForSeconds(0.1f);

        // Теперь закрываем панель и вызываем колбэк успеха
        panel.SetActive(false);
        onSuccess?.Invoke();
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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}