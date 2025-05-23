using System.Collections;
using UnityEngine;
using TMPro;

public class WaveMessageDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI messageUI;
    
    [Header("Display Settings")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    [Header("Optional Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip messageSound;
    
    private Coroutine currentMessageCoroutine;
    private Color originalColor;

    private void Awake()
    {
        if (messageUI != null)
        {
            originalColor = messageUI.color;
            messageUI.gameObject.SetActive(false);
        }
    }

     public void ShowMessage(string text)
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }
        
        currentMessageCoroutine = StartCoroutine(Display(text));
    }
    
    // Predefined messages for UnityEvents
    public void ShowWave2Message()
    {
        Debug.Log("Wave 2 Message Triggered");
        ShowMessage("You survived! You can see them now.");
    }
    
    public void ShowWave3Message()
    {
        Debug.Log("Wave 3 Message Triggered");
        ShowMessage("Use the hunter ball for next enemy!");
    }

    private IEnumerator Display(string text)
    {
        if (messageUI == null) yield break;

        messageUI.text = text;
        messageUI.gameObject.SetActive(true);
        
        // Play sound
        if (audioSource != null && messageSound != null)
        {
            audioSource.PlayOneShot(messageSound);
        }

        // Fade in using TextMeshPro alpha
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalColor.a, elapsedTime / fadeInDuration);
            messageUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        messageUI.color = originalColor;

        // Display duration
        yield return new WaitForSeconds(duration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeOutDuration);
            messageUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        messageUI.gameObject.SetActive(false);
        currentMessageCoroutine = null;
    }
}
