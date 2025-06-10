using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverTitle;
    [SerializeField] private TextMeshProUGUI gameOverMessage;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    
    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float delayBeforeShow = 1f;
    
    private CanvasGroup canvasGroup;
    private bool isShowing = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        HideGameOverUI();
        SetupButtons();
        
    }

    private void Start()
    {
        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += HandleGameOver;
        }
    }

    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    private void HandleGameOver(GameOverReason reason)
    {
        if (isShowing) return;
        
        StartCoroutine(ShowGameOverSequence(reason));
    }

    private IEnumerator ShowGameOverSequence(GameOverReason reason)
    {
        isShowing = true;
        
        yield return new WaitForSeconds(delayBeforeShow);
        
        // Setup the UI content based on game over reason
        SetupGameOverContent(reason);
        
        gameOverPanel.SetActive(true);
        yield return StartCoroutine(FadeIn());
  }

    private void SetupGameOverContent(GameOverReason reason)
    {
        if (GameManager.Instance == null) return;
        
        // Set final score and time
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
        }
        
        if (gameTimeText != null)
        {
            int minutes = Mathf.FloorToInt(GameManager.Instance.GameTimer / 60);
            int seconds = Mathf.FloorToInt(GameManager.Instance.GameTimer % 60);
            gameTimeText.text = $"Time Survived: {minutes:00}:{seconds:00}";
        }
        
        // Set title and message based on reason
        switch (reason)
        {
            case GameOverReason.Victory:
                if (gameOverTitle != null)
                    gameOverTitle.text = "VICTORY!";
                if (gameOverMessage != null)
                    gameOverMessage.text = "You have defeated the dragon and survived the nightmare!";
                break;
                
            case GameOverReason.PlayerDeath:
                if (gameOverTitle != null)
                    gameOverTitle.text = "GAME OVER";
                if (gameOverMessage != null)
                    gameOverMessage.text = "The spirits have claimed your soul...";
                break;
                
            case GameOverReason.Timeout:
                if (gameOverTitle != null)
                    gameOverTitle.text = "TIME'S UP!";
                if (gameOverMessage != null)
                    gameOverMessage.text = "You survived but failed to defeat the dragon!";
                break;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; 
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }

    private void HideGameOverUI()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        isShowing = false;
    }

    // Button Methods
    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; 
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Cleanup
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= HandleGameOver;
        }
        
        // Clean up button listeners
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);
    }
}