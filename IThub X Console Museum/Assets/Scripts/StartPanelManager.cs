using UnityEngine;
using UnityEngine.UI;

public class StartPanelManager : MonoBehaviour
{
    public static StartPanelManager Instance;

    [Header("UI")]
    public GameObject startPanel;
    public Button playButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        startPanel.SetActive(true);
        Time.timeScale = 0f; // Блокировка времени = блокировка ввода
    }

    public void OnPlayClicked()
    {
        startPanel.SetActive(false);
        Time.timeScale = 1f; // Разблокировка
    }
}