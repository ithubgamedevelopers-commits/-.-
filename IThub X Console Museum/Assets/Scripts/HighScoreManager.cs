using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [Header("Топ-3 рекордов")]
    public HighScoreData[] topScores = new HighScoreData[3];

    [Header("Личности по умолчанию (заполняют лидерборд при первом запуске)")]
    public string[] defaultNames = { "SYS", "MOM", "GOD" };
    public int[] defaultScores = { 500, 200, 50 };

    [Header("UI")]
    public GameObject inputPanel;
    public Text inputDisplayText;
    public VirtualKeyboard virtualKeyboard;
    public Text errorText;

    [Header("Отображение рекордов в меню")]
    public Text[] scoreTexts;   // 3 текста для очков
    public Text[] nameTexts;    // 3 текста для имён

    [Header("Фильтр запрещённых имён")]
    public List<string> forbiddenWords = new List<string>
    {
        // Маты транслитом
        "blyat", "blyad", "suka", "suki", "pizda", "pizdec", "pizd",
        "ebat", "ebal", "ebalo", "ebuch", "xyi", "hui", "huy",
        "mudak", "mudil", "govno", "govna", "dermo", "zalupa",
        "pidor", "peder", "gey", "gay", "churka", "khach", "hach",
        "zhid", "yevrey", "negr",

        // Обходы (повторы букв убираются кодом)
        "bl", "ss", "eb", "pzd", "suk",

        // Исторические личности и политика
        "hitler", "gitler", "stalin", "lenin", "putin", "zelensky",
        "trump", "biden", "obama", "saddam", "binladen",

        // Опасные идеи
        "nazi", "fascist", "faschist", "terrorist", "isil", "igil",
        "isis", "alqaeda",

        // Системные и запрещённые
        "admin", "moderator", "developer", "creator", "system",
        "null", "undefined", "root", "sex", "porn", "xxx", "lolita"
    };

    private int pendingScore = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadHighScores();
        UpdateLeaderboardDisplay();
        if (errorText != null) errorText.gameObject.SetActive(false);
    }

    public void LoadHighScores()
    {
        for (int i = 0; i < 3; i++)
        {
            topScores[i] = new HighScoreData();
            topScores[i].playerName = PlayerPrefs.GetString($"HighScoreName_{i}", defaultNames[i]);
            topScores[i].score = PlayerPrefs.GetInt($"HighScore_{i}", defaultScores[i]);
        }
    }

    // Вызывается при Game Over
    public void CheckHighScore()
    {
        pendingScore = SkillCheckManager.Instance != null ? SkillCheckManager.Instance.score : 0;

        if (pendingScore <= 0)
        {
            Debug.Log("Счёт = 0, рекорд не проверяем.");
            return;
        }

        if (pendingScore > topScores[2].score)
        {
            Debug.Log($"🏆 Новый рекорд! {pendingScore} попал в топ-3.");
            Time.timeScale = 1f;
            inputPanel.SetActive(true);
            if (errorText != null) errorText.gameObject.SetActive(false);
            if (virtualKeyboard != null) virtualKeyboard.Initialize();
        }
        else
        {
            Debug.Log($"Рекорд не побит. Минимальный в топ-3: {topScores[2].playerName} — {topScores[2].score}");
        }
    }

    // Вызывается виртуальной клавиатурой при нажатии ENTER
    public void SubmitName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length != 3)
        {
            ShowError("Имя должно содержать ровно 3 символа!");
            return;
        }

        if (IsNameForbidden(name))
        {
            ShowError("Это имя запрещено!");
            return;
        }

        HighScoreData newScore = new HighScoreData();
        newScore.playerName = name.ToUpper();
        newScore.score = pendingScore;

        InsertScore(newScore);
        SaveHighScores();

        inputPanel.SetActive(false);
        UpdateLeaderboardDisplay();
        Debug.Log($"✅ Рекорд сохранён: {newScore.playerName} — {newScore.score}");
    }

    private void InsertScore(HighScoreData newScore)
    {
        int insertIndex = 2;

        for (int i = 0; i < 3; i++)
        {
            if (newScore.score > topScores[i].score)
            {
                insertIndex = i;
                break;
            }
        }

        for (int i = 2; i > insertIndex; i--)
        {
            topScores[i] = topScores[i - 1];
        }

        topScores[insertIndex] = newScore;
    }

    private void SaveHighScores()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetString($"HighScoreName_{i}", topScores[i].playerName);
            PlayerPrefs.SetInt($"HighScore_{i}", topScores[i].score);
        }
        PlayerPrefs.Save();
    }

    void UpdateLeaderboardDisplay()
    {
        if (scoreTexts == null || nameTexts == null) return;

        for (int i = 0; i < 3; i++)
        {
            if (scoreTexts[i] != null)
                scoreTexts[i].text = topScores[i].score.ToString();
            
            if (nameTexts[i] != null)
                nameTexts[i].text = topScores[i].playerName;
        }
    }

    // === УМНЫЙ ФИЛЬТР ===
    private bool IsNameForbidden(string name)
    {
        string lower = name.ToLower();
        string normalized = RemoveConsecutiveDuplicates(lower);

        foreach (string word in forbiddenWords)
        {
            string w = word.ToLower();
            if (normalized.Contains(w) || lower.Contains(w))
            {
                Debug.LogWarning($"🚫 Запрещённое слово: {word} (в имени: {name})");
                return true;
            }
        }

        return false;
    }

    private string RemoveConsecutiveDuplicates(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(input[0]);
        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] != input[i - 1])
                sb.Append(input[i]);
        }
        return sb.ToString();
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
            Invoke(nameof(HideError), 2f);
        }
    }

    private void HideError()
    {
        if (errorText != null) errorText.gameObject.SetActive(false);
    }
}