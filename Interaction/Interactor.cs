using UnityEngine;
using TMPro;

public class Interactor : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI interactText;
    [SerializeField] LayerMask ignoreRaycast;

    void Update()
    {
        if(!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f, ~ignoreRaycast))
        {
            interactText.text = "";
            return;
        }

        IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();

        if (interactable == null)
        {
            interactText.text = "";
            return;
        }

        interactText.text = interactable.LookingAt(hit.distance);

    }
}