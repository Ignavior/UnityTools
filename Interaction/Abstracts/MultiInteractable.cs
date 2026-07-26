using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MultiInteractable<TInteraction> : MonoBehaviour, IInteractable where TInteraction : Interaction
{
    public TInteraction[] interactions;
    [field: SerializeField] public bool CanInteract { get; set; } = true;
    [field: SerializeField] public string CantInteractText { get; set; }
    [field: SerializeField] public string CooldownText { get; set; }
    [field: SerializeField] public float Cooldown { get; set; } = 0f;

    float globalTimeOfLastInteraction = Mathf.NegativeInfinity;

    // TODO: maybe id system like in EventSequencer, maybe overkill
    public string LookingAt(RaycastHit hit, Interactor interactor)
    {      
        if(!enabled)
            return "";

        if(!CanInteract)
            return CantInteractText;

        float currentTime = Time.time;

        float globalTimeSinceLastInteraction = currentTime - globalTimeOfLastInteraction;

        if (globalTimeSinceLastInteraction < Cooldown)
        {
            if (!string.IsNullOrWhiteSpace(CooldownText))
            {
                float countdown = Cooldown - globalTimeSinceLastInteraction;
                return Interactable.FormatCooldownText(CooldownText, countdown);
            }       
            return "";
        }
            

        string interactText = "";
        bool hasInteracted = false;

        foreach(TInteraction interaction in interactions.Cast<TInteraction>())
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
                    OnInteractFail(hit, interactor, interaction);

                if(!string.IsNullOrWhiteSpace(interaction.cantInteractText))
                    interactText += $"{interaction.cantInteractText}\n";
                
                continue;
            }

            float timeSinceLastInteraction = currentTime - interaction.timeOfLastInteraction;

            if(timeSinceLastInteraction < interaction.cooldown)
            {
                if(failInteract)
                    OnInteractFail(hit, interactor, interaction);

                if (!string.IsNullOrWhiteSpace(interaction.cooldownText))
                {
                    float countdown = interaction.cooldown - timeSinceLastInteraction;
                    interactText += $"{Interactable.FormatCooldownText(interaction.cooldownText, countdown)}\n";
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
                OnInteract(hit, interactor, interaction);
                hasInteracted = true;   
            }

            interactText += $"{interaction.interactText}\n";
        }

        return interactText;
    }  

    protected abstract void OnInteract(RaycastHit hit, Interactor interactor, TInteraction interaction);

    protected abstract void OnInteractFail(RaycastHit hit, Interactor interactor, TInteraction interaction);

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