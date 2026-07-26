using System;
using UnityEngine;
using UnityEngine.Events;

public class MultiEventInteractable : MultiInteractable<EventInteraction>
{
    protected override void OnInteract(RaycastHit hit, Interactor interactor, EventInteraction interaction)
    {
        interaction.onInteractAction?.Invoke();
    }

    protected override void OnInteractFail(RaycastHit hit, Interactor interactor, EventInteraction interaction)
    {
        interaction.onInteractFailAction?.Invoke();
    }
}

[Serializable]
public class EventInteraction : Interaction
{
    public UnityEvent onInteractAction;
    public UnityEvent onInteractFailAction;
}
