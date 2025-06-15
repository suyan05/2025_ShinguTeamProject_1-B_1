using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreSaveSystem : MonoBehaviour
{
    [System.Serializable]
    public class ScoreData
    {
        public int highScore = 0; // �⺻�� 0���� �ʱ�ȭ
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
                return new ScoreData(); // ���� ������ �⺻ ��ü ��ȯ
            }

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(json))
            {
                return new ScoreData(); // �� ���ϵ� �⺻ ��ü ��ȯ
            }

            ScoreData data = JsonUtility.FromJson<ScoreData>(json);
            if (data == null)
            {
                return new ScoreData(); // ������ȭ ���� ���
            }

            return data;
        }

        public static int LoadHighScore()
        {
            return LoadScoreData().highScore;
        }
    }
}
