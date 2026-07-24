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

    void Awake()
    {
        BuildStepDictionary();
    }

    void OnValidate()
    {
        BuildStepDictionary();
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
         if (!TryGetStepIndex(id, out int index))
            return;

        currentIndex = index == 0 
            ? steps.Length 
            : index - 1;
    }

    public void SetContinueToNext(string id, bool value)
    {
        if (!TryGetStepIndex(id, out int index))
            return;

        steps[index].continueToNext = value;
    }

    public void ToggleContinueToNext(string id)
    {
        if (!TryGetStepIndex(id, out int index))
            return;
        
        steps[index].continueToNext = !steps[index].continueToNext;      
    } 

    public void SetInvokeAction(string id, bool value)
    {
        if (!TryGetStepIndex(id, out int index))
            return;
            
        steps[index].invokeAction = value;
    }

    public void ToggleInvokeAction(string id)
    {
        if (!TryGetStepIndex(id, out int index))
            return;
        
        steps[index].invokeAction = !steps[index].invokeAction;      
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

    bool TryGetStepIndex(string id, out int index)
    {
        if (stepDictionary.TryGetValue(id, out index))
            return true;

        Debug.LogError(
            $"EventSequencer '{name}' could not find EventStep with ID '{id}'.",
            this
        );

        return false;
    }

    void BuildStepDictionary()
    {
        stepDictionary = new Dictionary<string, int>();

        for (int i = 0; i < steps.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(steps[i].id))
                stepDictionary[steps[i].id] = i;
        }
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
