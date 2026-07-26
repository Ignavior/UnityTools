using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TransformController : MonoBehaviour
{
    [SerializeField] UnityEvent onMotionComplete, onMotionFrame;
    
    [field: SerializeField] bool InvokeOnMotionComplete {get; set;}
    [field: SerializeField] bool InvokeOnMotionFrame {get; set;}
    
    [field: SerializeField] public float MoveTime {get; set;} = 1f;
    [Header("Motion")]
    [field: SerializeField] public float MoveX {get; set;} 
    [field: SerializeField] public float MoveY {get; set;} 
    [field: SerializeField] public float MoveZ {get; set;} 
   [Header("Rotation")]
    [field: SerializeField] public float RotateX {get; set;} 
    [field: SerializeField] public float RotateY {get; set;} 
    [field: SerializeField] public float RotateZ {get; set;} 

    public void LerpPositionToTransform(Transform target, float moveTime)
    {
        StartCoroutine(C_LerpPositionToTransform(target, moveTime));
    }

    public void LerpPositionToTransform(Transform target)
    {
        StartCoroutine(C_LerpPositionToTransform(target, MoveTime));
    }

    public void LerpPosition(Vector3 delta, float moveTime)
    {
        StartCoroutine(C_LerpWorldPosition(delta, moveTime));
    }

    public void LerpPosition()
    {
        StartCoroutine(C_LerpWorldPosition(new(MoveX, MoveY, MoveZ), MoveTime));
    }

    public void LerpLocalPosition(Vector3 delta, float moveTime)
    {
        StartCoroutine(C_LerpLocalPosition(delta, moveTime));
    }

    public void LerpLocalPosition()
    {
        StartCoroutine(C_LerpLocalPosition(new(MoveX, MoveY, MoveZ), MoveTime));
    }

    public void LerpRotationToTransform(Transform target, float moveTime)
    {
        StartCoroutine(C_LerpRotationToTransform(target, moveTime));
    }

    public void LerpRotationToTransform(Transform target)
    {
        StartCoroutine(C_LerpRotationToTransform(target, MoveTime));
    }

    public void LerpRotation(Vector3 delta, float moveTime)
    {
        StartCoroutine(C_LerpWorldRotation(delta, moveTime));
    }

    public void LerpRotation()
    {
        StartCoroutine(C_LerpWorldRotation(new(RotateX, RotateY, RotateZ), MoveTime));
    }

    public void LerpLocalRotation(Vector3 delta, float moveTime)
    {
        StartCoroutine(C_LerpLocalRotation(delta, moveTime));
    }

    public void LerpLocalRotation()
    {
        StartCoroutine(C_LerpLocalRotation(new(RotateX, RotateY, RotateZ), MoveTime));
    }

    //Single axis
    public void LerpXPosition(float delta)
    {
        StartCoroutine(C_LerpWorldPosition(new(delta, 0, 0), MoveTime));
    }

    public void LerpXLocalPosition(float delta)
    {
        StartCoroutine(C_LerpLocalPosition(new(delta, 0, 0), MoveTime));
    }

    public void LerpYPosition(float delta)
    {
        StartCoroutine(C_LerpWorldPosition(new(0, delta, 0), MoveTime));
    }

    public void LerpYLocalPosition(float delta)
    {
        StartCoroutine(C_LerpLocalPosition(new(0, delta, 0), MoveTime));
    }

    public void LerpZPosition(float delta)
    {
        StartCoroutine(C_LerpWorldPosition(new(0, 0, delta), MoveTime));
    }

    public void LerpZLocalPosition(float delta)
    {
        StartCoroutine(C_LerpLocalPosition(new(0, 0, delta), MoveTime));
    }

    public void LerpXRotation(float delta)
    {
        StartCoroutine(C_LerpWorldRotation(new(delta, 0, 0), MoveTime));
    }

    public void LerpXLocalRotation(float delta)
    {
        StartCoroutine(C_LerpLocalRotation(new(delta, 0, 0), MoveTime));
    }

    public void LerpYRotation(float delta)
    {
        StartCoroutine(C_LerpWorldRotation(new(0, delta, 0), MoveTime));
    }

    public void LerpYLocalRotation(float delta)
    {
        StartCoroutine(C_LerpLocalRotation(new(0, delta, 0), MoveTime));
    }

    public void LerpZRotation(float delta)
    {
        StartCoroutine(C_LerpWorldRotation(new(0, 0, delta), MoveTime));
    }

    public void LerpZLocalRotation(float delta)
    {
        StartCoroutine(C_LerpLocalRotation(new(0, 0, delta), MoveTime));
    }

    IEnumerator C_LerpPositionToTransform(Transform target, float moveTime)
    {
        Vector3 startPosition = transform.position;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, target.position, timer/moveTime);          

            if(InvokeOnMotionFrame)
                onMotionFrame?.Invoke();

            yield return null;
        }

        transform.position = target.position;

        if(InvokeOnMotionComplete)
            onMotionComplete?.Invoke();
    }

    IEnumerator C_LerpLocalPosition(Vector3 delta, float moveTime)
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = startPosition + delta;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, timer/moveTime);
            
            if(InvokeOnMotionFrame)
                onMotionFrame?.Invoke();

            yield return null;
        }

        transform.localPosition = targetPosition;

        if(InvokeOnMotionComplete)
            onMotionComplete?.Invoke();
    }

    IEnumerator C_LerpWorldPosition(Vector3 delta, float moveTime)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + delta;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, timer/moveTime);
            
            if(InvokeOnMotionFrame)
                onMotionFrame?.Invoke();

            yield return null;
        }

        transform.position = targetPosition;

        if(InvokeOnMotionComplete)
            onMotionComplete?.Invoke();
    }

    IEnumerator C_LerpRotationToTransform(Transform target, float moveTime)
    {
        Quaternion startRotation = transform.rotation;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, target.rotation, timer/moveTime);

            if(InvokeOnMotionFrame)
                onMotionFrame?.Invoke();

            yield return null;
        }

        transform.rotation = target.rotation;

        if(InvokeOnMotionComplete)
            onMotionComplete?.Invoke();
    }
    
    IEnumerator C_LerpLocalRotation(Vector3 delta, float moveTime)
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(delta);
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, timer/moveTime);
            
            if(InvokeOnMotionFrame)
                onMotionFrame?.Invoke();

            yield return null;
        }

        transform.localRotation = targetRotation;

        if(InvokeOnMotionComplete)
            onMotionComplete?.Invoke();
    }

    IEnumerator C_LerpWorldRotation(Vector3 delta, float moveTime)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(delta);
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timer/moveTime);
            
            if(InvokeOnMotionFrame)
                onMotionFrame?.Invoke();

            yield return null;
        }

        transform.rotation = targetRotation;

        if(InvokeOnMotionComplete)
            onMotionComplete?.Invoke();
    }
}
