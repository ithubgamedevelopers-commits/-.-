using UnityEngine;
using UnityEngine.UI;

public class StartPanelManager : MonoBehaviour
{
    public static StartPanelManager Instance;

    [Header("UI")]
    public GameObject startPanel;
    public Button playButton;

    [Header("Звук клика")]
    public AudioSource clickAudioSource;
    public AudioClip clickSound;

    [Header("Фоновая музыка")]
    public AudioSource musicAudioSource;
    public AudioClip backgroundMusic;
    
    [Tooltip("Громкость музыки (0 - 1)")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private bool musicStarted = false; // Чтобы не запускать музыку дважды

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Музыка не прерывается при смене сцен
        }
        else 
        { 
            Destroy(gameObject); 
            return; 
        }
    }

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        
        if (startPanel != null)
        {
            startPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        
        // Настраиваем AudioSource для музыки
        if (musicAudioSource != null)
        {
            musicAudioSource.loop = true;           // Включаем цикл
            musicAudioSource.playOnAwake = false;   // Запускаем вручную
            musicAudioSource.volume = musicVolume;
        }
    }

    public void OnPlayClicked()
    {
        // 1. Звук клика (сразу)
        PlayClickSound();

        // 2. Запускаем фоновую музыку (только один раз за сессию)
        if (!musicStarted)
        {
            PlayBackgroundMusic();
            musicStarted = true;
        }

        // 3. Закрываем панель и запускаем игру
        if (startPanel != null)
            startPanel.SetActive(false);
        Time.timeScale = 1f;
        
        Debug.Log("🎮 Игра началась! Музыка играет.");
    }

    private void PlayClickSound()
    {
        if (clickAudioSource != null && clickSound != null)
        {
            clickAudioSource.PlayOneShot(clickSound);
        }
    }

    private void PlayBackgroundMusic()
    {
        if (musicAudioSource != null && backgroundMusic != null)
        {
            if (!musicAudioSource.isPlaying)
            {
                musicAudioSource.clip = backgroundMusic;
                musicAudioSource.Play();
                Debug.Log("🎵 Фоновая музыка запущена в цикле");
            }
        }
        else
        {
            Debug.LogWarning("Не назначен musicAudioSource или backgroundMusic!");
        }
    }

    // Методы для управления музыкой из других скриптов
    public void StopMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
            musicStarted = false;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicAudioSource != null)
            musicAudioSource.volume = musicVolume;
    }
}