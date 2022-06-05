using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BoolInputListener : MonoBehaviour
{

    public InputActionReference actionReference;
    private InputAction action;

    public UnityEvent onPressed;
    public UnityEvent onReleased;
    public UnityEvent<bool> onValueChange;
    
    [ReadOnly]
    public bool isPressed = false;

    void Awake() {
        action = actionReference.action;
        if( action.type != InputActionType.Button )
            throw new System.Exception("Expected a Button action here.");
        action.Enable();    

        isPressed = false;

        action.started += Pressed;
        action.canceled += Released;
        action.performed += UpdateValues;
    }

    private void Pressed(InputAction.CallbackContext obj) {
        isPressed = true;
        onValueChange?.Invoke(isPressed);
        onPressed?.Invoke();
    }

    private void Released(InputAction.CallbackContext obj) {
        isPressed = false;
        onValueChange?.Invoke(isPressed);
        onReleased?.Invoke();
    }

    private void UpdateValues(InputAction.CallbackContext obj)
    {
        var curValue = obj.ReadValueAsButton();
        if( curValue != isPressed ) 
        {
            isPressed = curValue;
            onValueChange?.Invoke(curValue);
            if( curValue )
                onPressed?.Invoke();
            else
                onReleased?.Invoke();
        }
    }
}
