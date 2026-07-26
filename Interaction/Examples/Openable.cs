using System.Collections;
using UnityEngine;

public class Openable : EventInteractable
{
    [Header("Openable")]
    [SerializeField] Transform pivot;
    [SerializeField] Vector3 deltaPosition;
    [SerializeField] Vector3 deltaRotation;
    [SerializeField] public bool IsOpen;
    [SerializeField] float openTime, openSpeed;
    [SerializeField] string openText, closeText;

    Vector3 closedPosition, openPosition;
    Quaternion closedRotation, openRotation;

    bool isMoving;

    void Awake()
    {
        if(string.IsNullOrWhiteSpace(openText) || string.IsNullOrWhiteSpace(closeText))
        {
            openText = base.InteractText;
            closeText = base.InteractText;
        }
        base.InteractText = IsOpen ? closeText : openText;
    }

    void OnEnable()
    {
        CalculateStates();     
    }

    void OnValidate()
    {
        CalculateStates(); 
    }

    public void ToggleState()
    {
        if(isMoving)
            return;

        isMoving = true;
       
        Vector3 targetPosition = IsOpen ? closedPosition : openPosition;
        Quaternion targetRotation = IsOpen ? closedRotation : openRotation;

        StartCoroutine(TransitionToState(targetPosition, targetRotation));
    }

    IEnumerator TransitionToState(Vector3 _targetPosition, Quaternion _targetRotation)
    {
        Vector3 startPosition = pivot.localPosition;
        Vector3 targetPosition = _targetPosition;

        Quaternion startRotation = pivot.localRotation;
        Quaternion targetRotation = _targetRotation;
        float timer = 0f;

        while (timer < openTime)
        {
            timer += Time.fixedDeltaTime;
           
            pivot.SetLocalPositionAndRotation(
                Vector3.Lerp(startPosition, targetPosition, timer/openTime), 
                Quaternion.Lerp(startRotation, targetRotation, timer/openTime));

            yield return new WaitForFixedUpdate();
        }
        
        pivot.SetLocalPositionAndRotation(
            targetPosition,
            targetRotation
        );

        isMoving = false;
        IsOpen = !IsOpen;
        base.InteractText = IsOpen ? closeText : openText;
    }

    void CalculateStates()
    {
        pivot.GetLocalPositionAndRotation(out Vector3 _position, out Quaternion _rotation);
        Vector3 _deltaPosition = _position + deltaPosition;
        Quaternion _deltaRotation = _rotation * Quaternion.Euler(deltaRotation);

        openPosition = IsOpen ? _position : _deltaPosition;
        openRotation = IsOpen ? _rotation : _deltaRotation;
        
        closedPosition = !IsOpen ? _position : _deltaPosition;
        closedRotation = !IsOpen ? _rotation : _deltaRotation;
    }
}
