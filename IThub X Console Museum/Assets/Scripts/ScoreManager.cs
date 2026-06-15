using UnityEngine;
using System.IO;

public class ScoreManager  : MonoBehaviour
{
    public static ScoreManager Instance;

    public string PlayerName = "";
    public int Score = 0;

    private string savePath;

    [System.Serializable]
    public class SaveData
    {
        public string playerName;
        public int score;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
        LoadGame();
    }

    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            playerName = PlayerName,
            score = Score
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Игра сохранена: {json}");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            PlayerName = data.playerName;
            Score = data.score;
            Debug.Log($"Игра загружена: {json}");
        }
        else
        {
            PlayerName = "";
            Score = 0;
            Debug.Log("Файл сохранения не найден, создан новый профиль.");
        }
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }
}