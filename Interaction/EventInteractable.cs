using UnityEngine;
using UnityEngine.Events;

public class EventInteractable : Interactable
{
    [Header("Events")]
    [SerializeField] UnityEvent OnInteractAction;
    [SerializeField]  UnityEvent OnInteractFailAction;

    protected override void OnInteract(RaycastHit hit, Interactor interactor)
    {
        OnInteractAction?.Invoke();
    }

    protected override void OnInteractFail(RaycastHit hit, Interactor interactor)
    {
        OnInteractFailAction?.Invoke();
    }
}
