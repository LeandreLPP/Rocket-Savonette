using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Vector2InputListener : MonoBehaviour
{

    public InputActionReference actionReference;
    private InputAction action;

    // public UnityEvent onPressed;
    // public UnityEvent onReleased;
    public UnityEvent<Vector2> onValueChange;
    
    private Vector2 _value;

    public Vector2 Value => _value;

    void Awake() {
        action = actionReference.action;
        if( action.type != InputActionType.Value )
            throw new System.Exception("Expected a Value action here.");
        action.Enable();    

        _value = Vector2.zero;

        action.started += UpdateValues;
        action.canceled += UpdateValues;
        action.performed += UpdateValues;
    }

    // private void Pressed(InputAction.CallbackContext obj) {
    //     _isPressed = true;
    //     onValueChange?.Invoke(IsPressed);
    //     onPressed?.Invoke();
    // }

    // private void Released(InputAction.CallbackContext obj) {
    //     _isPressed = false;
    //     onValueChange?.Invoke(IsPressed);
    //     onReleased?.Invoke();
    // }

    private void UpdateValues(InputAction.CallbackContext obj)
    {
        var curValue = obj.ReadValue<Vector2>();
        if( curValue != _value ) 
        {
            _value = curValue;
            onValueChange?.Invoke(curValue);
            // if( curValue )
            //     onPressed?.Invoke();
            // else
            //     onReleased?.Invoke();
        }
    }
}
