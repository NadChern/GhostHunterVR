
[System.Serializable]
public class GameStateMemento
{
    public int savedScore;
    public float savedTime;
    public int savedWave;
    public float savedPlayerHealth;
    public bool savedDragonDefeated;
    
    public GameStateMemento(int score, float time, int wave, float health, bool dragonKilled)
    {
        savedScore = score;
        savedTime = time;
        savedWave = wave;
        savedPlayerHealth = health;
        savedDragonDefeated = dragonKilled;
    }
}