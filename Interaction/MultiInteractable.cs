using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MultiInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] Interaction[] interactions;
    [field: SerializeField] public bool GlobalCanInteract { get; set; } = true;
    [field: SerializeField] public string GlobalCantInteractText { get; set; }
    [field: SerializeField] public string GlobalCooldownText { get; set; }
    [field: SerializeField] public float GlobalCooldown { get; set; } = 0f;

    float globalTimeOfLastInteraction = Mathf.NegativeInfinity;

    // TODO: maybe id system like in EventSequencer, maybe overkill
    public string LookingAt(RaycastHit hit, Interactor interactor)
    {      
        if(!enabled)
            return "";

        if(!GlobalCanInteract)
            return GlobalCantInteractText;

        float currentTime = Time.time;

        float globalTimeSinceLastInteraction = currentTime - globalTimeOfLastInteraction;

        if (globalTimeSinceLastInteraction < GlobalCooldown)
        {
            if (!string.IsNullOrWhiteSpace(GlobalCooldownText))
            {
                float countdown = GlobalCooldown - globalTimeSinceLastInteraction;
                return BasicInteractable.FormatCooldownText(GlobalCooldownText, countdown);
            }       
            return "";
        }
            

        string interactText = "";
        bool hasInteracted = false;

        foreach(Interaction interaction in interactions)
        {
            if (hit.distance > interaction.interactRange)
                continue;

            bool isPressed = interaction.input.action.IsPressed();
            bool wasPressedThisFrame = interaction.input.action.WasPressedThisFrame(); 

            bool failInteract = interaction.failContinuous 
                    ? isPressed
                    : wasPressedThisFrame;
            

            if (!interaction.canInteract)
            {
                if(failInteract)
                    interaction.onInteractFail?.Invoke();

                if(!string.IsNullOrWhiteSpace(interaction.cantInteractText))
                    interactText += $"{interaction.cantInteractText}\n";
                
                continue;
            }

            float timeSinceLastInteraction = currentTime - interaction.timeOfLastInteraction;

            if(timeSinceLastInteraction < interaction.cooldown)
            {
                if(failInteract)
                    interaction.onInteractFail?.Invoke();

                if (!string.IsNullOrWhiteSpace(interaction.cooldownText))
                {
                    float countdown = interaction.cooldown - timeSinceLastInteraction;
                    interactText += $"{BasicInteractable.FormatCooldownText(interaction.cooldownText, countdown)}\n";
                }         
                continue;
            }

            bool interact = interaction.continuous 
                ? interaction.input.action.IsPressed() 
                : interaction.input.action.WasPressedThisFrame();

            if (interact && (!interaction.independent || !hasInteracted))
            {
                interaction.timeOfLastInteraction = currentTime;
                globalTimeOfLastInteraction = currentTime;
                interaction.onInteract?.Invoke();
                hasInteracted = true;   
            }

            interactText += $"{interaction.interactText}\n";
        }

        return interactText;
    }  

    public void SetCanInteractTrue(int index)
    {
        interactions[index].canInteract = true;
    }
    public void SetCanInteractFalse(int index)
    {
        interactions[index].canInteract = false;
    }
    public void ToggleCanInteract(int index)
    {
        interactions[index].canInteract = !interactions[index].canInteract;
    }
}

[Serializable]
public class Interaction
{
    public UnityEvent onInteract;
    public UnityEvent onInteractFail;
    public InputActionReference input; 
    public float interactRange = 1.5f;
    public string interactText= "[E] Interact";
    public string cantInteractText;
    public string cooldownText;
    public bool canInteract = true;
    public bool independent;
    public bool continuous;
    public bool failContinuous;
    public float cooldown;
    [NonSerialized] public float timeOfLastInteraction = Mathf.NegativeInfinity;
}