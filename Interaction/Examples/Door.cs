using System.Collections;
using UnityEngine;

public class Door : Interactable
{
    [Header("Door")]
    [SerializeField] Transform hinge;
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openTime = 1f;
    [SerializeField] bool isOpen;
    [SerializeField] bool isTwoWay;
    [SerializeField] string openText, closeText;

    bool isTransitioning;

    Quaternion closedRotation;
    Quaternion openRotation;

    void Awake()
    {
        closedRotation = hinge.localRotation;

        if (isOpen)
        {
            openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
            hinge.localRotation = openRotation;
        }

        if (string.IsNullOrWhiteSpace(openText) || string.IsNullOrWhiteSpace(closeText))
        {
            openText = base.InteractText;
            closeText = base.InteractText;
        }

        base.InteractText = isOpen ? closeText : openText;
    }


    protected override void OnInteract(RaycastHit hit, Interactor interactor)
    {
        if (isTransitioning)
            return;

        if (isOpen)
        {
            StartCoroutine(Transition(closedRotation));
            return;
        }

        float angle = openAngle;

        if (isTwoWay)
        {
            Vector3 directionToPlayer = interactor.transform.position - hinge.position;

            if (Vector3.Dot(hinge.forward, directionToPlayer) < 0f)
                angle = -angle;
        }

        openRotation = closedRotation * Quaternion.Euler(0f, angle, 0f);

        StartCoroutine(Transition(openRotation));
    }

    protected override void OnInteractFail(RaycastHit hit, Interactor interactor){}

    IEnumerator Transition(Quaternion targetRotation)
    {
        isTransitioning = true;

        Quaternion startRotation = hinge.localRotation;
        float timer = 0f;

        while (timer < openTime)
        {
            timer += Time.deltaTime;

            hinge.localRotation = Quaternion.Lerp(startRotation, targetRotation, timer / openTime);

            yield return null;
        }

        hinge.localRotation = targetRotation;

        isOpen = !isOpen;
        isTransitioning = false;

        base.InteractText = isOpen ? closeText : openText;
    }
}