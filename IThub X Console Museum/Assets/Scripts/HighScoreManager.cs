using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    public HighScoreData currentRecord;

    [Header("UI")]
    public GameObject inputPanel;
    public TextMeshProUGUI recordDisplayText;
    public VirtualKeyboard virtualKeyboard;

    [Header("Фильтр никнеймов")]
    public List<string> badWords = new List<string> { "bad", "admin", "test" };

    private string savePath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        savePath = Path.Combine(Application.persistentDataPath, "highscore.json");
        LoadRecord();
    }

    public void LoadRecord()
    {
        if (File.Exists(savePath))
            currentRecord = JsonUtility.FromJson<HighScoreData>(File.ReadAllText(savePath));
        else
            currentRecord = new HighScoreData { playerName = "AAA", score = 0 };

        UpdateRecordDisplay();
    }

    public void CheckAndPromptHighScore(int finalScore)
    {
        if (finalScore > currentRecord.score && finalScore > 0)
        {
            inputPanel.SetActive(true);
            virtualKeyboard.Initialize(finalScore);
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length != 5) return false;

        string lowerName = name.ToLower();
        foreach (string bad in badWords)
        {
            if (lowerName.Contains(bad)) return false;
        }
        return true;
    }

    public void SaveRecord(string name, int score)
    {
        if (!IsValidName(name))
        {
            virtualKeyboard.ShowError("НЕДОПУСТИМО");
            return;
        }

        currentRecord.playerName = name.ToUpper();
        currentRecord.score = score;
        File.WriteAllText(savePath, JsonUtility.ToJson(currentRecord));

        inputPanel.SetActive(false);
        UpdateRecordDisplay();

        Time.timeScale = 1f;
    }

    public void UpdateRecordDisplay()
    {
        if (recordDisplayText != null)
            recordDisplayText.text = "ЛУЧШИЙ: " + currentRecord.playerName + " — " + currentRecord.score;
    }
}