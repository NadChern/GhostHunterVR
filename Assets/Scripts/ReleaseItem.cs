using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ReleaseItem : MonoBehaviour
{
    public UnityEvent OnRelease;
    private bool isWorking = false;

    public void SetBool(bool state)
    {
        isWorking = state;
    }

    public void ReleaseTrigger(InputAction.CallbackContext context)
    {
        if (context.performed && isWorking)
        {
            OnRelease?.Invoke();
            Debug.Log("ReleaseTrigger");
        }
    }
}