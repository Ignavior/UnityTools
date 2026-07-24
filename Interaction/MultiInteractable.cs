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
    public string LookingAt(float distance, Interactor interactor)
    {      
        if(!enabled)
            return "";

        if(!GlobalCanInteract)
            return GlobalCantInteractText;

        float globalTimeSinceLastInteraction = Time.time - globalTimeOfLastInteraction;

        if (globalTimeSinceLastInteraction < GlobalCooldown)
        {
            float countdown = GlobalCooldown - globalTimeSinceLastInteraction;
            return BasicInteractable.FormatCooldownText(GlobalCooldownText, countdown);
        }
            

        string interactText = "";

        foreach(Interaction interaction in interactions)
        {
            if (distance > interaction.interactRange)
                continue;

            if (!interaction.canInteract)
            {
                interactText += $"{interaction.cantInteractText}\n";
                continue;
            }

            float timeSinceLastInteraction = Time.time - interaction.timeOfLastInteraction;

            if(timeSinceLastInteraction < interaction.cooldown)
            {
                float countdown = interaction.cooldown - timeSinceLastInteraction;
                interactText += $"{BasicInteractable.FormatCooldownText(interaction.cooldownText, countdown)}\n";
                continue;
            }

            bool interact = interaction.continuous 
                ? interaction.input.action.IsPressed() 
                : interaction.input.action.WasPressedThisFrame();

            if (interact)
            {
                interaction.timeOfLastInteraction = Time.time;
                globalTimeOfLastInteraction = Time.time;
                interaction.onInteract.Invoke();
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
    public InputActionReference input; 
    public float interactRange = 1.5f;
    public string interactText= "[E] Interact";
    public string cantInteractText;
    public string cooldownText;
    public bool canInteract = true;
    public bool continuous;
    public float cooldown;
    [NonSerialized] public float timeOfLastInteraction = Mathf.NegativeInfinity;
}