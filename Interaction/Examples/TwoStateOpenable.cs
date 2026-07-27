using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TwoStateOpenable : Interactable
{
    [Header("TwoStateOpenable")]
    [SerializeField] Transform pivot;
    [SerializeField] Vector3 deltaPosition;
    [SerializeField] Vector3 deltaRotation;
    [SerializeField] public bool isOpen;
    [SerializeField] float openTime, openSpeed;
    [SerializeField] string openText, closeText;

    Vector3 closedPosition, openPosition;
    Quaternion closedRotation, openRotation;

    bool isTransitioning;

    void Awake()
    {
        if(string.IsNullOrWhiteSpace(openText) || string.IsNullOrWhiteSpace(closeText))
        {
            openText = base.InteractText;
            closeText = base.InteractText;
        }
        base.InteractText = isOpen ? closeText : openText;
    }

    void OnEnable()
    {
        CalculateStates();     
    }

    void OnValidate()
    {
        CalculateStates(); 
    }

    protected override void OnInteract(RaycastHit hit, Interactor interactor)
    {
        ToggleState();
    }

    protected override void OnInteractFail(RaycastHit hit, Interactor interactor){}

    public void ToggleState()
    {
        if(isTransitioning)
            return;

        isTransitioning = true;
       
        Vector3 targetPosition = isOpen ? closedPosition : openPosition;
        Quaternion targetRotation = isOpen ? closedRotation : openRotation;

        StartCoroutine(TransitionToState(targetPosition, targetRotation, openTime));
    }

    IEnumerator TransitionToState(Vector3 _targetPosition, Quaternion _targetRotation, float openTime)
    {
        Vector3 startPosition = pivot.localPosition;
        Vector3 targetPosition = _targetPosition;

        Quaternion startRotation = pivot.localRotation;
        Quaternion targetRotation = _targetRotation;
        float timer = 0f;

        while (timer < openTime)
        {
            timer += Time.deltaTime;
           
            pivot.SetLocalPositionAndRotation(
                Vector3.Lerp(startPosition, targetPosition, timer/openTime), 
                Quaternion.Lerp(startRotation, targetRotation, timer/openTime));

            yield return null;
        }
        
        pivot.SetLocalPositionAndRotation(
            targetPosition,
            targetRotation
        );

        isTransitioning = false;
        isOpen = !isOpen;
        base.InteractText = isOpen ? closeText : openText;
    }

    void CalculateStates()
    {
        pivot.GetLocalPositionAndRotation(out Vector3 _position, out Quaternion _rotation);
        Vector3 _deltaPosition = _position + deltaPosition;
        Quaternion _deltaRotation = _rotation * Quaternion.Euler(deltaRotation);

        openPosition = isOpen ? _position : _deltaPosition;
        openRotation = isOpen ? _rotation : _deltaRotation;
        
        closedPosition = !isOpen ? _position : _deltaPosition;
        closedRotation = !isOpen ? _rotation : _deltaRotation;
    }

    
}
