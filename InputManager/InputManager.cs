using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;
    
    static InputManager instance;
    InputActionMap playerMap;

    void Awake()
    {
        instance = this;
        playerMap = inputActions.FindActionMap("Player");
    }

    public static void EnablePlayerInput()
    {
        instance.playerMap.Enable();
    }

    public static void DisablePlayerInput()
    {
        instance.playerMap.Disable();
    }
}
