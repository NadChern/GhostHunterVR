using UnityEngine;

[System.Serializable]
public class HighScoreMemento
{
    public int[] topScores = new int[5];
    public float[] topTimes = new float[5];
    
    public void SaveToPlayerPrefs()
    {
        for (int i = 0; i < topScores.Length; i++)
        {
            PlayerPrefs.SetInt($"HighScore_{i}", topScores[i]);
            PlayerPrefs.SetFloat($"HighTime_{i}", topTimes[i]);
        }
    }
    
    public void LoadFromPlayerPrefs()
    {
        for (int i = 0; i < topScores.Length; i++)
        {
            topScores[i] = PlayerPrefs.GetInt($"HighScore_{i}", 0);
            topTimes[i] = PlayerPrefs.GetFloat($"HighTime_{i}", 0f);
        }
    }
}