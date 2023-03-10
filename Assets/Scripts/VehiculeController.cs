
using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class VehiculeController : MonoBehaviour
{
    [Header("Inputs")]
    public BoolInputListener burnListener;    
    public BoolInputListener hoverListener;
    [Space]
    public Vector2InputListener turnListener;
    public Vector2InputListener straffListener;

    [Header("Acceleration settings")] 
    public float forwardAcceleration = 1.0f;
    public float forwardMaxSpeed = 50f;
    [ReadOnly] public float forwardDrag = 1f;
    // [ReadOnly] public float forwardTimeToZero = 1f;
    
    [Header("Straff Settings")]
    public float straffAcceleration = 1.0f;
    public float straffMaxSpeed = 50f;
    [ReadOnly] public float straffDrag = 1f;
    // [ReadOnly] public float straffTimeToZero = 1f;
    
    [Header("Rotation Settings")]
    public float angularAcceleration = 15.0f;
    public float angularMaxSpeed = 50f;
    [ReadOnly] public float angularDrag = 5f;
    // [ReadOnly] public float angularTimeToZero = 1f;

    public float backwardDrag = 10f;

    [Header("Near walls")]
    public Sensor rearSensor;
    public float bonusAccelerationMax = 500f;
    public AnimationCurve bonusAccelerationCurve;

    private Rigidbody _rigidbody;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {

        // Acceleration
        if( burnListener.isPressed ) {
            // Increase drag when burning
            // float dragRatio = Mathf.InverseLerp(0, bonusDragTargetSpeed, _rigidbody.velocity.magnitude);
            // dragRatio = bonusDragCurve.Evaluate(dragRatio);
             //Mathf.LerpUnclamped(dragMin, dragMax, dragRatio);

            float curAcceleration = forwardAcceleration;

            // Accelerate stronger when burning against walls
            if( rearSensor.IsColliding ) {
                float bonusAcceleration = bonusAccelerationMax * bonusAccelerationCurve.Evaluate(1 - rearSensor.Ratio);
                curAcceleration += bonusAcceleration;
            }

            _rigidbody.AddForce(transform.forward * curAcceleration, ForceMode.Acceleration);
        }

        if (hoverListener.isPressed)
        {
            _rigidbody.drag = 0;
            _rigidbody.angularDrag = 0;
        }
        else
        {
            _rigidbody.drag = forwardDrag;
            _rigidbody.angularDrag = angularDrag;
        }

        // Straffing
        _rigidbody.AddForce(transform.right * straffAcceleration * straffListener.Value.x, ForceMode.Acceleration);
        
        // Turning
        _rigidbody.AddRelativeTorque(0, turnListener.Value.x * angularAcceleration, 0, ForceMode.Acceleration);
    }

    private void OnValidate()
    {
        forwardDrag = RigidbodyUtils.GetDragFromAcceleration(forwardAcceleration, forwardMaxSpeed);
        straffDrag = RigidbodyUtils.GetDragFromAcceleration(straffAcceleration, straffMaxSpeed);
        angularDrag = RigidbodyUtils.GetDragFromAcceleration(angularAcceleration, angularMaxSpeed);
    }
}
