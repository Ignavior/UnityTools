using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MotionController : MonoBehaviour
{
    [SerializeField] UnityEvent onMotionComplete;
    [field: SerializeField] bool InvokeAction;
    public float MoveTime {get; set;}
    

    IEnumerator C_LerpPositionToTransform(Transform target, float moveTime)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.position;
        float timer = 0f;

        if(moveTime > 0f)
        {
            while (timer < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, timer/moveTime);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        transform.position = targetPosition;

        if(InvokeAction)
            onMotionComplete?.Invoke();
    }

    IEnumerator C_LerpPositionBy(Vector3 delta, float moveTime)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + delta;
        float timer = 0f;

        if(moveTime > 0f)
        {
            while (timer < moveTime)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, timer/moveTime);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        transform.position = targetPosition;

        if(InvokeAction)
            onMotionComplete?.Invoke();
    }


    public void LerpPositionToTransform(Transform target, float moveTime)
    {
        StartCoroutine(C_LerpPositionToTransform(target, moveTime));
    }

    public void LerpPositionToTransform(Transform target)
    {
        StartCoroutine(C_LerpPositionToTransform(target, MoveTime));
    }

    public void LerpPositionBy(Vector3 delta, float moveTime)
    {
        StartCoroutine(C_LerpPositionBy(delta, moveTime));
    }

    public void LerpPositionByX(float x)
    {
        StartCoroutine(C_LerpPositionBy(new(x, 0, 0), MoveTime));
    }
    public void LerpPositionByY(float y)
    {
        StartCoroutine(C_LerpPositionBy(new(0, y, 0), MoveTime));
    }
    public void LerpPositionByZ(float z)
    {
        StartCoroutine(C_LerpPositionBy(new(0, 0, z), MoveTime));
    }
}
