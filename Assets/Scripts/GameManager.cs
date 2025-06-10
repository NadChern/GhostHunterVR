using System;
using UnityEngine;

// Singleton pattern on GameManager Instance
// Flyweight pattern ref on gameplay settings 
// Observer pattern (events subscription)

public enum GameOverReason
{
    Victory,
    PlayerDeath,
    Timeout
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private WaveManager waveManager;

    // Events 
    public event Action<GameOverReason> OnGameOver;
    public event Action<int> OnScoreChange;
    public event Action<float> OnTimeChanged;

    // Game state tracking
    private GameStateMemento checkpoint;
    private bool gameActive = false;
    private bool gameOver = false;
    private bool dragonDefeated = false;
    private int currentScore = 0;
    private float gameTimer = 0f;

    // Properties (for UI, game analytics, save/load func) 
    public int CurrentScore => currentScore;
    public float GameTimer => gameTimer;
    public bool GameActive => gameActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }

        GhostLogic.OnGhostDeath += HandleGhostDeath;
        DragonLogic.OnDragonDeath += HandleDragonDeath;
    }

    private void Update()
    {
        if (gameActive && !gameOver)
        {
            gameTimer += Time.deltaTime; // update global timer
            OnTimeChanged?.Invoke(gameTimer);
            CheckWaveProgression(); // check wave triggers + dragon
        }
    }

    // Invoked trough input system on Flashlight prefab
    public void StartWaveFromFlashlight()
    {
        if (gameActive || gameOver) return;
        gameActive = true;
        currentScore = 0;
        gameTimer = 0;
        gameOver = false;
        dragonDefeated = false;

        waveManager.StartFirstWave();
    }

    private void CheckWaveProgression()
    {
        if (gameOver) return;

        if (waveManager.CurrentWave == 0 && gameTimer >= waveManager.GetTimeToStartWave(1))
        {
            Debug.Log("Time to start Wave 1");
            waveManager.AdvanceWave();
        }
        else if (waveManager.CurrentWave == 1 && gameTimer >= waveManager.GetTimeToStartWave(2))
        {
            Debug.Log("Time to start Wave 2");
            waveManager.AdvanceWave();
        }
        else if (gameTimer >= waveManager.GetTimeToStartWave(3))
        {
            Debug.Log("Reached final wave duration, checking victory/timeout");
            if (dragonDefeated)
                TriggerGameOver(GameOverReason.Victory);
            else
                TriggerGameOver(GameOverReason.Timeout);
        }
    }

    private void HandlePlayerDeath()
    {
        TriggerGameOver(GameOverReason.PlayerDeath);
    }

    private void TriggerGameOver(GameOverReason reason)
    {
        if (!gameOver)
        {
            gameOver = true;
            gameActive = false;
            OnGameOver?.Invoke(reason);
            Debug.Log($"Game Over - Reason: {reason}");
        }
    }

    private void HandleDragonDeath(DragonLogic dragon)
    {
        dragonDefeated = true;
        Debug.Log("[GameManager] Dragon has been defeated! Victory condition met.");
        AddScore(100);
    }

    private void HandleGhostDeath(GhostLogic ghost)
    {
        int score = waveManager.GetCurrentWaveSettings().scoreValue;
        AddScore(score);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChange?.Invoke(currentScore);
    }

    // Save checkpoint at wave transitions
    public void SaveCheckpoint()
    {
        if (playerHealth != null)
        {
            checkpoint = new GameStateMemento(
                currentScore,
                gameTimer,
                waveManager.CurrentWave,
                playerHealth.CurrentHealth,
                dragonDefeated
            );
        }
    }

    // Restore from checkpoint (for continue feature)
    public void LoadCheckpoint()
    {
        if (checkpoint != null)
        {
            currentScore = checkpoint.savedScore;
            gameTimer = checkpoint.savedTime;
            dragonDefeated = checkpoint.savedDragonDefeated;

            if (playerHealth != null)
                playerHealth.SetHealth(checkpoint.savedPlayerHealth);

            // Restore wave state
            waveManager.SetCurrentWave(checkpoint.savedWave);

            OnScoreChange?.Invoke(currentScore);
            OnTimeChanged?.Invoke(gameTimer);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }

        GhostLogic.OnGhostDeath -= HandleGhostDeath;
        DragonLogic.OnDragonDeath -= HandleDragonDeath;
    }
}