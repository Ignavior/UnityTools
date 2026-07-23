using UnityEngine;
using UnityEngine.Events;

public class OldBasicInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public UnityEvent OnInteract { get; set; }
    [field: SerializeField] public KeyCode Key { get; set; } = KeyCode.E;
    [field: SerializeField] public float InteractRange { get; set; } = 1.5f;
    [field: SerializeField] public string InteractText { get; set; } = "[E] Interact";
    [field: SerializeField] public bool CanInteract { get; set; } = true;
    [field: SerializeField] public bool IsContinuous { get; set; }
    [field: SerializeField] public float Cooldown { get; set; } = 0f;

    float timeSinceInteraction = Mathf.Infinity;

    public string LookingAt(float distance)
    {
        if (distance > InteractRange || !CanInteract)
            return "";

        if (timeSinceInteraction < Cooldown)
            return InteractText;

        bool interact = IsContinuous ? Input.GetKey(Key) : Input.GetKeyDown(Key);

        if (interact)
        {
            timeSinceInteraction = 0f;
            OnInteract.Invoke();
        }

        return InteractText;
    }

    void Update()
    {
        timeSinceInteraction += Time.deltaTime;
    }
}