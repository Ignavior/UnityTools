using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventSequencer : MonoBehaviour
{
    [SerializeField] EventStep[] steps;
    [field: SerializeField] public bool IsLooping {get; set;}
    [field: SerializeField] public bool DontContinueToNext {get; set;}

    Coroutine sequenceCoroutine;
    bool sequenceRunning;
    int currentIndex;

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

                step.action?.Invoke();

                if (step.delayAfterEvent > 0)
                    yield return new WaitForSeconds(step.delayAfterEvent);

                if(step.pause)
                    yield return new WaitUntil(() => !step.pause);

                if (DontContinueToNext)
                    yield return new WaitUntil(() => !DontContinueToNext);

                yield return null; // PREVENTS FREEZE

                currentIndex++;
            }
        } while (IsLooping);

        sequenceRunning = false;
    }

    public void UnpauseCurrent()
    {
        steps[currentIndex].pause = false;
    }
}

[Serializable]
public class EventStep
{
    public UnityEvent action;  
    public float delayAfterEvent;
    public bool pause;
}
