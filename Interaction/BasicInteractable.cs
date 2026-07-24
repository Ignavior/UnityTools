using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BasicInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public UnityEvent OnInteract { get; set; }
    [field: SerializeField] public InputActionReference Input { get; set; }
    [field: SerializeField] public float InteractRange { get; set; } = 2f;
    [field: SerializeField] public string InteractText { get; set; } = "[E] Interact";
    [field: SerializeField] public string CantInteractText { get; set; }
    [field: SerializeField] public string CooldownText { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; } = true;
    [field: SerializeField] public bool Continuous { get; set; }
    [field: SerializeField] public float Cooldown { get; set; } = 0f;

    float timeOfLastInteraction = Mathf.NegativeInfinity;

    public string LookingAt(float distance, Interactor interactor)
    {
        if (distance > InteractRange)
            return "";

        if(!CanInteract)
            return CantInteractText;

        if (Time.time - timeOfLastInteraction < Cooldown)
            return CooldownText;

        bool interact = Continuous 
                ? Input.action.IsPressed() 
                : Input.action.WasPressedThisFrame();

        if (interact)
        {
            timeOfLastInteraction = Time.time;
            OnInteract.Invoke();
        }

        return InteractText;
    }
}
