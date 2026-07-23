using UnityEngine;
using TMPro;

public class Interactor : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI interactText;
    [SerializeField] float maxRange = 100f;
    [SerializeField] LayerMask ignoreRaycast;

    void Update()
    {
        if(!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxRange, ~ignoreRaycast))
        {
            interactText.text = "";
            return;
        }

        if (!hit.collider.TryGetComponent<IInteractable>(out var interactable))
        {
            interactText.text = "";
            return;
        }

        interactText.text = interactable.LookingAt(hit.distance);

    }
}