using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreSaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class ScoreData
    {
        public int highScore = 0; // 기본값 0으로 초기화
    }

    public static class SaveSystem
    {
        private const string FILE = "score.json";
        private static string filePath = Path.Combine(Application.persistentDataPath, FILE);

        public static void SaveScore(int currentScore)
        {
            ScoreData data = new ScoreData();

            int prevHigh = LoadHighScore();
            data.highScore = currentScore > prevHigh ? currentScore : prevHigh;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }

        public static ScoreData LoadScoreData()
        {
            if (!File.Exists(filePath))
            {
                return new ScoreData(); // 파일 없으면 기본 객체 반환
            }

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(json))
            {
                return new ScoreData(); // 빈 파일도 기본 객체 반환
            }

            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            if (data == null)
            {
                return new ScoreData(); // 역직렬화 실패 대비
            }

            return data;
        }

        public static int LoadHighScore()
        {
            return LoadScoreData().highScore;
        }
    }
}
