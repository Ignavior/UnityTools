using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSequencer : MonoBehaviour
{
    [SerializeField] EventStep[] steps;
    Dictionary<string, int> stepDictionary;
    [field: SerializeField] public bool IsLooping {get; set;}
    [field: SerializeField] public bool IsPaused {get; set;}

    Coroutine sequenceCoroutine;
    bool sequenceRunning;
    int currentIndex;

    void Start()
    {
        stepDictionary = new();

        for (int i = 0; i < steps.Length; i++)
        {
            stepDictionary[steps[i].id] = i;
        }
    }

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
            currentIndex = 0;

            while(currentIndex < steps.Length)
            {
                EventStep step = steps[currentIndex];

                if (step.invokeAction)
                    step.action?.Invoke();

                if (step.delayAfterEvent > 0)
                    yield return new WaitForSeconds(step.delayAfterEvent);

                if (!step.continueToNext)
                    yield return new WaitUntil(() => step.continueToNext);
                
                if (IsPaused)
                    yield return new WaitUntil(() => !IsPaused);

                yield return null; // PREVENTS FREEZE

                currentIndex++;
            } 
        } while (IsLooping);

        sequenceRunning = false;

    }

    // This method is mainly designed to be called from EventSequencer action
    public void SkipTo(string id)
    {
        if (stepDictionary.TryGetValue(id, out int i))
           currentIndex = i == 0 ? steps.Length : i - 1;
    }

    public void SetContinueToNext(string id, bool value)
    {
        EventStep step = steps[stepDictionary[id]];

        if (step != null)
            step.continueToNext = value;
    }

    public void ToggleContinueToNext(string id)
    {
        EventStep step = steps[stepDictionary[id]];

        if (step != null)
            step.continueToNext = !step.continueToNext;        
    } 

    public void SetInvokeAction(string id, bool value)
    {
        EventStep step = steps[stepDictionary[id]];

        if (step != null)
            step.invokeAction = value;
    }

    public void ToggleInvokeAction(string id)
    {
        EventStep step = steps[stepDictionary[id]];

        if (step != null)
            step.invokeAction = !step.invokeAction;        
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
