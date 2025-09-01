using System;
using NaughtyAttributes;
using UnityEngine;

public class ArcadeVehiculeController : MonoBehaviour
{
    [Header("Inputs")]
    public BoolInputListener burnListener;    
    public BoolInputListener hoverListener;
    public BoolInputListener brakeListener;    
    [Space]
    public Vector2InputListener turnListener;
    public Vector2InputListener straffListener;
    [Space] 
    public Sensor backWallSensor;
    
    private Rigidbody _rigidbody;

    [Header("Drag")] 
    public float normalDrag = 0.01f;
    public float hoverDrag = 0f;
    public float brakingDrag = 5f;

    [Header("Settings")] 
    public float maxSpeed = 20f;
    public float maxSpeedAgainstWall = 40f;
    public float accelerationTime = 2f;
    public float deccelerationTime = 1f;

    [Header("Straff")] 
    public float straffSpeed = 5f;
    public float straffBurnSpeed = 5f;
    public float straffAccelerationTime = 0.1f;
    
    [Header("Rotation")] 
    public float rotationSpeed = 5f;
    public float rotationBurnSpeed = 5f;
    public float rotationAccelerationTime = 1f;

    [Header("Debug")] public float debugScale = 5;

    // [Header("Runtime")] 
    // [ReadOnly] public Vector3 burnVelocity;
    // [ReadOnly] public Vector3 straffVelocity;
    // [ReadOnly] public float currentRotationSpeed;

    private float _currentRotationSpeed;
    private Vector3 _burnDampVelocity;
    // private Vector3 _straffDampVelocity;
    private float _rotationDampVelocity;
    
    private Vector3 _forwardDampVelocity;
    private Vector3 _rightDampVelocity;
    private float _rightStraffDampVelocity;
    
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ExtractVelocities(_rigidbody.linearVelocity,
            out var forwardVector,
            out float forwardVelocity,
            out var rightVector,
            out float rightVelocity);
        
        if (burnListener.isPressed)
        {
            float targetForwardSpeed = backWallSensor ? maxSpeedAgainstWall : maxSpeed;
            var targetVelocity = transform.forward * targetForwardSpeed;

            targetVelocity += transform.right * straffListener.Value.x * straffSpeed;
            _rigidbody.linearVelocity = Vector3.SmoothDamp(
                _rigidbody.linearVelocity,
                targetVelocity,
                ref _burnDampVelocity,
                forwardVelocity > 0 ? accelerationTime : deccelerationTime,
                float.MaxValue,
                Time.deltaTime);
        }
        else
        {
            _burnDampVelocity = Vector3.zero;
        }

        _rigidbody.linearDamping =
            brakeListener.isPressed ? brakingDrag :
            hoverListener.isPressed ? hoverDrag : 
            normalDrag;

        // float targetRightSpeed = straffListener.Value.x * straffSpeed;
        // if (targetRightSpeed != 0 && targetRightSpeed > 0 != rightVelocity > 0 || 
        //     Mathf.Abs(rightVelocity) < Mathf.Abs(targetRightSpeed))
        // {
        //     rightVelocity = Mathf.SmoothDamp(
        //         rightVelocity,
        //         targetRightSpeed,
        //         ref _rightStraffDampVelocity,
        //         straffAccelerationTime,
        //         float.MaxValue,
        //         Time.deltaTime);
        //     
        //     rightVector = transform.right * rightVelocity;
        // }
        // else
        // {
        //     _rightStraffDampVelocity = 0;
        // }
        //
        // burnVelocity = forwardVector + rightVector;
        // _rigidbody.velocity = burnVelocity;
        
        // { // Straff
        //     var straffInput = straffListener.Value;
        //     var targetStraff = transform.rotation * new Vector3(straffInput.x, 0, 0) * straffSpeed;
        //     straffVelocity = Vector3.SmoothDamp(
        //         straffVelocity,
        //         targetStraff,
        //         ref _straffDampVelocity,
        //         straffAccelerationTime,
        //         float.MaxValue,
        //         Time.deltaTime);
        // }
         // + straffVelocity;
        
        { // Rotation
            float targetRotationSpeed =
                turnListener.Value.x * (burnListener.isPressed ? rotationBurnSpeed : rotationSpeed);
            
            _currentRotationSpeed = Mathf.SmoothDamp(
                _currentRotationSpeed,
                targetRotationSpeed,
                ref _rotationDampVelocity,
                rotationAccelerationTime,
                float.MaxValue,
                Time.deltaTime);
            
            _rigidbody.angularVelocity = new Vector3(0, _currentRotationSpeed, 0);
        }
        
        // _rigidbody.drag = hoverListener.isPressed ? 
    }

    private void OnDrawGizmos()
    {
        float forwardVelocity, rightVelocity;
        Vector3 forwardVector, rightVector;
        
        var velocity = _rigidbody != null ? _rigidbody.linearVelocity : new Vector3(maxSpeed * 0.3f, 0, maxSpeed * 0.4f);
        ExtractVelocities(velocity, out forwardVector, out forwardVelocity, out rightVector, out rightVelocity);

        DrawDirection(velocity, Color.cyan);
        DrawDirection(transform.forward * forwardVelocity, forwardVelocity > 0 ? Color.blue : Color.yellow);
        DrawDirection(transform.right * rightVelocity, rightVelocity > 0 ? Color.green : Color.red);
    }

    private void DrawDirection(Vector3 direction, Color color)
    {
        var position = transform.position + Vector3.up;
        var oldColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(position, position + direction / maxSpeed * debugScale);
        Gizmos.color = oldColor;
    }

    private void ExtractVelocities(Vector3 velocity, out Vector3 forwardVector, out float forwardVelocity, out Vector3 rightVector, out float rightVelocity)
    {
        if (velocity.magnitude <= 0)
        {
            forwardVector = rightVector = Vector3.zero;
            forwardVelocity = rightVelocity = 0;
            return;
        }

        forwardVector = Vector3.Project(velocity, transform.forward);
        rightVector = Vector3.Project(velocity, transform.right);
        
        forwardVelocity = forwardVector.magnitude;
        if (Vector3.Dot(forwardVector, transform.forward) < 0)
            forwardVelocity *= -1;
        
        rightVelocity = rightVector.magnitude;
        if (Vector3.Dot(rightVector, transform.right) < 0)
            rightVelocity *= -1;
    }
}
