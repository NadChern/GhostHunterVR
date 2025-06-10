using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    [Header("UI References")] [SerializeField]
    private TextMeshPro countdownText;

    [Header("Timer Settings")] [SerializeField]
    private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;
    [SerializeField] private float warningThreshold = 30f; // 30 seconds
    [SerializeField] private float criticalThreshold = 10f; // 10 seconds

    private float finalWaveDuration;
    private float finalWaveStartTime;
    private bool isCountdownActive = false;

    private void Start()
    {
        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveStart += OnWaveStart;
        }

        // Hide countdown initially
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isCountdownActive && GameManager.Instance != null && GameManager.Instance.GameActive)
        {
            UpdateCountdown();
        }
    }

    private void OnWaveStart(int waveIndex, WaveSettings waveSettings)
    {
        // Only show countdown on wave 2 (final wave)
        if (waveIndex == 2)
        {
            finalWaveDuration = waveSettings.waveDuration;
            finalWaveStartTime = GameManager.Instance.GameTimer;
            isCountdownActive = true;
            gameObject.SetActive(true);
        }
    }

    private void UpdateCountdown()
    {
        if (finalWaveDuration <= 0) return;
      
        float elapsedTime = GameManager.Instance.GameTimer - finalWaveStartTime;
        float remainingTime = Mathf.Max(0, finalWaveDuration - elapsedTime);
        
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";

        // Change color based on remaining time
        if (remainingTime <= criticalThreshold)
            countdownText.color = criticalColor;
        else if (remainingTime <= warningThreshold)
            countdownText.color = warningColor;
        else
            countdownText.color = normalColor;
    }

    private void OnDestroy()
    {
        var waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveStart -= OnWaveStart;
        }
    }
}