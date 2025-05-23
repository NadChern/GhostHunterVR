using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimerTrigger : MonoBehaviour
{
    [Header("Timer Configuration")]
    [SerializeField] private string timerName = "Timer"; 
    
    [SerializeField] private float delay = 1f;
    [SerializeField] private UnityEvent onTimerComplete;
    
    private Coroutine timerCoroutine;
    
    public void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
            
        timerCoroutine = StartCoroutine(TimerRoutine());
    }
    
    public void StartTimer(float customDelay)
    {
        delay = customDelay;
        StartTimer();
    }
    
    private IEnumerator TimerRoutine()
    {
        yield return new WaitForSeconds(delay);
        onTimerComplete?.Invoke();
        timerCoroutine = null;
    }
    
    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }
}

