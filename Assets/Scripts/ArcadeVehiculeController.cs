using System;
using System.Net.Sockets;
using NaughtyAttributes;
using UnityEngine;

public class ArcadeVehiculeController : MonoBehaviour
{
    [Header("Inputs")]
    public BoolInputListener burnListener;    
    public BoolInputListener hoverListener;
    [Space]
    public Vector2InputListener turnListener;
    public Vector2InputListener straffListener;
    
    private Rigidbody _rigidbody;

    [Header("Settings")] 
    public float maxSpeed = 20f;
    public float accelerationTime = 2f;
    public float deccelerationTime = 1f;

    [Header("Straff")] 
    public float straffSpeed = 5f;
    public float straffAccelerationTime = 0.1f;
    
    [Header("Rotation")] 
    public float rotationSpeed = 5f;
    public float rotationAccelerationTime = 1f;

    [Header("Debug")] public float debugScale = 5;

    [Header("Runtime")] 
    [ReadOnly] public Vector3 burnVelocity;
    [ReadOnly] public Vector3 straffVelocity;
    [ReadOnly] public float currentRotationSpeed;

    private Vector3 _burnDampVelocity;
    private Vector3 _straffDampVelocity;
    private float _rotationDampVelocity;
    
    private Vector3 _forwardDampVelocity;
    private Vector3 _rightDampVelocity;
    private float _rightStraffDampVelocity;
    
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ExtractVelocities(_rigidbody.velocity,
            out var forwardVector,
            out var forwardVelocity,
            out var rightVector,
            out var rightVelocity);
        if (burnListener.isPressed)
        {
            var targetForward = transform.forward * maxSpeed;
            forwardVector = Vector3.SmoothDamp(
                forwardVector,
                targetForward,
                ref _forwardDampVelocity,
                forwardVelocity > 0 ? accelerationTime : deccelerationTime,
                float.MaxValue,
                Time.deltaTime);
        }
        else
        {
            _forwardDampVelocity = Vector3.zero;
        }

        var targetRightSpeed = straffListener.Value.x * straffSpeed;
        if (targetRightSpeed != 0 && targetRightSpeed > 0 != rightVelocity > 0 || 
            Mathf.Abs(rightVelocity) < Mathf.Abs(targetRightSpeed) || 
            burnListener.isPressed)
        {
            rightVelocity = Mathf.SmoothDamp(
                rightVelocity,
                targetRightSpeed,
                ref _rightStraffDampVelocity,
                straffAccelerationTime,
                float.MaxValue,
                Time.deltaTime);
            
            rightVector = transform.right * rightVelocity;
        }
        else
        {
            _rightStraffDampVelocity = 0;
        }

        burnVelocity = forwardVector + rightVector;
        _rigidbody.velocity = burnVelocity;
        
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
            var targetRotationSpeed = turnListener.Value.x * rotationSpeed;
            currentRotationSpeed = Mathf.SmoothDamp(
                currentRotationSpeed,
                targetRotationSpeed,
                ref _rotationDampVelocity,
                rotationAccelerationTime,
                float.MaxValue,
                Time.deltaTime);
            
            _rigidbody.angularVelocity = new Vector3(0, currentRotationSpeed, 0);
        }
        
        // _rigidbody.drag = hoverListener.isPressed ? 
    }

    private void OnDrawGizmos()
    {
        float forwardVelocity, rightVelocity;
        Vector3 forwardVector, rightVector;
        
        var velocity = _rigidbody != null ? _rigidbody.velocity : new Vector3(maxSpeed * 0.3f, 0, maxSpeed * 0.4f);
        ExtractVelocities(velocity, out forwardVector, out forwardVelocity, out rightVector, out rightVelocity);

        DrawDirection(velocity, Color.cyan);
        DrawDirection(transform.forward * forwardVelocity, forwardVelocity > 0 ? Color.blue : Color.yellow);
        DrawDirection(transform.right * rightVelocity, rightVelocity > 0 ? Color.green : Color.red);
        
        // DrawDirection(forwardVector, Color.blue);
        // DrawDirection(rightVector, Color.green);
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
