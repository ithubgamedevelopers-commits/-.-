using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath = Path.Combine(Application.persistentDataPath, "highscore.json");

    public static void SaveScore(int score)
    {
        HighScoreData data = new HighScoreData { score = score, playerName = "AAA" };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
    }

    public static int LoadScore()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            return data.score;
        }
        return 0;
    }
}