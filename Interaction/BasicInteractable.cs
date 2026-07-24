using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BasicInteractable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public UnityEvent OnInteract { get; set; }
    [field: SerializeField] public UnityEvent OnInteractFail { get; set; }
    [field: SerializeField] public InputActionReference Input { get; set; }
    [field: SerializeField] public float InteractRange { get; set; } = 2f;
    [field: SerializeField] public string InteractText { get; set; } = "[E] Interact";
    [field: SerializeField] public string CantInteractText { get; set; }
    [field: SerializeField] public string CooldownText { get; set; }
    [field: SerializeField] public bool CanInteract { get; set; } = true;
    [field: SerializeField] public bool Continuous { get; set; }
    [field: SerializeField] public bool FailContinuous { get; set; }
    [field: SerializeField] public float Cooldown { get; set; } = 0f;

    float timeOfLastInteraction = Mathf.NegativeInfinity;

    public string LookingAt(float distance, Interactor interactor)
    {
        if (distance > InteractRange || !enabled)
            return "";

        bool isPressed = Input.action.IsPressed();
        bool wasPressedThisFrame = Input.action.WasPressedThisFrame(); 

        bool failInteract = FailContinuous 
                ? isPressed
                : wasPressedThisFrame;

        if (!CanInteract)
        {
            if(failInteract)
                OnInteractFail?.Invoke();

            return CantInteractText;
        }
            
        float timeSinceLastInteraction = Time.time - timeOfLastInteraction;

        if (timeSinceLastInteraction < Cooldown)
        {
            if(failInteract)
                OnInteractFail?.Invoke();

            float countdown = Cooldown - timeSinceLastInteraction;
            return FormatCooldownText(CooldownText, countdown);
        }
            
        bool interact = Continuous 
                ? isPressed
                : wasPressedThisFrame;

        if (interact)
        {
            timeOfLastInteraction = Time.time;
            OnInteract?.Invoke();
        }

        return InteractText;
    }

    private static readonly Regex TimeRegex = new(@"\{time(?::([^}]+))?\}");
    public static string FormatCooldownText(string text, float time)
    {
        return TimeRegex.Replace(
            text,
            match =>
            {
                string format = match.Groups[1].Success
                    ? match.Groups[1].Value
                    : "F0";

                if (format.StartsWith("F") && int.TryParse(format[1..], out int decimals))
                {
                    float multiplier = Mathf.Pow(10f, decimals);
                    time = Mathf.Ceil(time * multiplier) / multiplier;
                }

                return time.ToString(format);
            }            
        );
    }
}
