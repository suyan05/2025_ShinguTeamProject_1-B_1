using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreSaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class ScoreData
    {
        public int highScore;
        public int lastScore;
    }

    public static class SaveSystem
    {
        private const string FILE = "score.json";

        private static string filePath = Path.Combine(Application.persistentDataPath, FILE);

        public static void SaveScore(int currentScore)
        {
            ScoreData data = new ScoreData();
            data.lastScore = currentScore;

            int prevHigh = LoadHighScore();
            data.highScore = currentScore > prevHigh ? currentScore : prevHigh;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }

        public static ScoreData LoadScoreData()
        {
            if (!File.Exists(filePath))
            {
                return new ScoreData(); // 기본값: 0점
            }

            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<ScoreData>(json);
        }

        public static int LoadHighScore()
        {
            return LoadScoreData().highScore;
        }

        public static int LoadLastScore()
        {
            return LoadScoreData().lastScore;
        }
    }
}
