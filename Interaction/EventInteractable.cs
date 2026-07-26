using UnityEngine;
using UnityEngine.Events;

public class EventInteractable : Interactable
{
    [field: SerializeField] public UnityEvent OnInteractAction { get; set; }
    [field: SerializeField] public UnityEvent OnInteractFailAction { get; set; }

    protected override void OnInteract(RaycastHit hit, Interactor interactor)
    {
        OnInteractAction?.Invoke();
    }

    protected override void OnInteractFail(RaycastHit hit, Interactor interactor)
    {
        OnInteractFailAction?.Invoke();
    }
}
