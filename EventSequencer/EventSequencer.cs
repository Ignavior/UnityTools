using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequencer : MonoBehaviour
{
    [SerializeField] List<EventStep> steps;
    [field: SerializeField] public bool IsLooping {get; set;}

    EventStep currentStep;
    Coroutine sequenceCoroutine;

    bool sequenceRunning;


    public void StartSequence()
    {
        if(sequenceRunning)
            return;
        
        sequenceCoroutine = StartCoroutine(RunSequence());
    }

    public void StopSequence()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        sequenceRunning = false;
        currentStep = null;
    }

    public void ContinueCurrent()
    {
        currentStep.continueToNext = true;
    }

    // Reason for this is so methods can be directly called from UnityEvent
    public void ContinueToNext(string id)
    {
        EventStep step = steps.Find(s => s.id == id);

        if(step != null)
            step.continueToNext = true;
    }

    public void DontContinueToNext(string id)
    {
        EventStep step = steps.Find(s => s.id == id);

        if(step != null)
            step.continueToNext = false;
    }

    public void InvokeAction(string id)
    {
        EventStep step = steps.Find(s => s.id == id);

        if(step != null)
            step.invokeAction = true;
    }

    public void DontInvokeAction(string id)
    {
        EventStep step = steps.Find(s => s.id == id);

        if(step != null)
            step.invokeAction = false;
    }


    IEnumerator RunSequence()
    {
        sequenceRunning = true;

        do
        {
            foreach (EventStep step in steps)
            {
                currentStep = step;

                if(step.invokeAction)
                    step.action?.Invoke();

                if(step.delayAfterEvent > 0)
                    yield return new WaitForSeconds(step.delayAfterEvent);

                yield return new WaitUntil(() => step.continueToNext);
            } 
        } while (IsLooping);

        sequenceRunning = false;
        currentStep = null;
    }
}

[Serializable]
public class EventStep
{
    public string id;
    public UnityEvent action;
    public float delayAfterEvent;
    public bool continueToNext = true;
    public bool invokeAction = true;
}
