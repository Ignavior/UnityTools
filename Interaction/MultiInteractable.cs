using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MultiInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] Interaction[] interactions;
    public bool canInteract = true;

    // TODO: different texts foor cooldown and stuff
    public string LookingAt(float distance, Interactor interactor)
    {
        string interactText = "";

        if(!canInteract)
            return interactText;

        foreach(Interaction interaction in interactions)
        {
            if (distance > interaction.interactRange || !interaction.canInteract)
                continue;

            interactText += $"{interaction.interactText}\n";

            if(Time.time - interaction.timeOfLastInteraction < interaction.cooldown)
                continue;

            bool interact = interaction.continuous 
                ? interaction.input.action.IsPressed() 
                : interaction.input.action.WasPressedThisFrame();

            if (interact)
            {
                interaction.timeOfLastInteraction = Time.time;
                interaction.onInteract.Invoke();
            }
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
    public bool continuous;
    public float interactRange = 1.5f;
    public string interactText= "[E] Interact";
    public bool canInteract = true;
    public float cooldown;
    [NonSerialized] public float timeOfLastInteraction = Mathf.NegativeInfinity;
}