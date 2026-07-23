using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequencer : MonoBehaviour
{
    [SerializeField] List<EventStep> steps;
    [field: SerializeField] public bool IsLooping {get; set;}
    [field: SerializeField] public bool IsPaused {get; set;}

    Coroutine sequenceCoroutine;

    bool sequenceRunning;


    public void StartSequence()
    {
        if (sequenceRunning)
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
    }

    IEnumerator RunSequence()
    {
        sequenceRunning = true;

        do
        {
            foreach (EventStep step in steps)
            {
                if (step.invokeAction)
                    step.action?.Invoke();

                if (step.delayAfterEvent > 0)
                    yield return new WaitForSeconds(step.delayAfterEvent);

                if (!step.continueToNext)
                    yield return new WaitUntil(() => step.continueToNext);
                
                if (IsPaused)
                    yield return new WaitUntil(() => !IsPaused);
            } 
        } while (IsLooping);

        sequenceRunning = false;

    }

    public void SetContinueToNext(string id, bool value)
    {
        EventStep step = steps.Find(s => s.id == id);

        if (step != null)
            step.continueToNext = value;
    }

    public void SetInvokeAction(string id, bool value)
    {
        EventStep step = steps.Find(s => s.id == id);

        if (step != null)
            step.invokeAction = value;
    }

    public void SetContinueToNextTrue(string id)
    {
        SetContinueToNext(id, true);
    }

    public void SetContinueToNextFalse(string id)
    {
        SetContinueToNext(id, false);
    }

    public void SetInvokeActionTrue(string id)
    {
        SetInvokeAction(id, true);
    }

    public void SetInvokeActionFalse(string id)
    {
        SetInvokeAction(id, false);
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
