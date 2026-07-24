using UnityEngine;
using TMPro;
using System;

public class Interactor : MonoBehaviour
{
    [SerializeField] GameObject interactPrompt;
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] float maxRange = 100f;
    [SerializeField] LayerMask ignoreRaycast;

    void Update()
    {
        string text = "";

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxRange, ~ignoreRaycast))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                text = interactable.LookingAt(hit.distance, this);
            }
        }

        if (!string.IsNullOrWhiteSpace(text))
        {
            interactPrompt.SetActive(true);
            interactText.text = text;
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }
}