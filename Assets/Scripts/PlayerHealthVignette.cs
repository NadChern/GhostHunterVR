using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// VFX on player damage (uses vignette intensity) 
public class PlayerHealthVignette : MonoBehaviour
{
    [Header("Post-processing References")] 
    [SerializeField] private Volume postProcessingVolume;

    [Header("Vignette Settings")]
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float pulseMinIntensity = 0.0f;      
    [SerializeField] private float pulseMaxIntensity = 1.0f;     

    [SerializeField] private PlayerHealth playerHealth;

    private Vignette vignette;
    private Coroutine pulseCoroutine;
    private float lastHealthValue; // tracks increase-decrease in health

    private void Start()
    {
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out Vignette vig))
        {
            vignette = vig;
            // set vignette intensity to 0 - invisible
            if (vignette.intensity.overrideState)
            {
                vignette.intensity.Override(0);
            }
        }

        // subscribe to health change events
        if (playerHealth != null)
        {
            lastHealthValue = playerHealth.CurrentHealth;
            playerHealth.OnHealthChanged += OnHealthChanged;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        // chake if player took damage or heal
        bool tookDamage = currentHealth < lastHealthValue;
        lastHealthValue = currentHealth;

        // trigger when taking damage
        if (tookDamage)
        {
            TriggerPulse();
        }
    }

   
   private void TriggerPulse()
    {
        // Stop existing pulse if running
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }

        // Start new pulse
        pulseCoroutine = StartCoroutine(PulseEffect());
    }

    private IEnumerator PulseEffect()
    {
        float startTime = Time.time;
        
        UpdateVignetteIntensity(pulseMaxIntensity);
 
        // Fade back to base intensity
        while (Time.time - startTime < pulseDuration)
        {
            float t = (Time.time - startTime) / pulseDuration;
            float currentIntensity = Mathf.Lerp(pulseMaxIntensity, pulseMinIntensity, t);
            UpdateVignetteIntensity(currentIntensity);
            yield return null;
        }

        // Ensure end at zero intensity
        UpdateVignetteIntensity(pulseMinIntensity);
        pulseCoroutine = null;
    }

    private void UpdateVignetteIntensity(float intensity)
    {
        if (vignette != null && vignette.intensity.overrideState)
        {
            vignette.intensity.Override(intensity);
        }
    }
}