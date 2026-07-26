using UnityEngine;

public interface IInteractable
{
    string LookingAt(RaycastHit raycastHit, Interactor interactor);
}