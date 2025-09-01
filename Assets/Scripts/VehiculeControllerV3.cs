using NaughtyAttributes;
using UnityEngine;

public class VehiculeControllerV3 : MonoBehaviour
{
    [Header("Inputs")]
    public BoolInputListener burnListener;
    public BoolInputListener burstHoldListener;
    public BoolInputListener brakeListener;    
    [Space]
    public Vector2InputListener turnListener;
    public Vector2InputListener straffListener;
    [Space]
    public Sensor backWallSensor;

    [Header("Config")]
    [Expandable] public VC3Config config;

    [Header("Debug & Runtime")] public float debugScale = 5;

    [ShowNonSerializedField] private Rigidbody rb;
    [ShowNonSerializedField] private Vector3 velocity;
    [ShowNonSerializedField] private float currentChargeTime;
    [ShowNonSerializedField] private bool isInPerfectChargeZone;
    [ShowNonSerializedField] private bool wasChargingBurstLastFrame;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        velocity = Vector3.zero;
        currentChargeTime = 0f;
        isInPerfectChargeZone = false;
        wasChargingBurstLastFrame = false;
    }

    private void FixedUpdate()
    {
        bool isChargingBurst = burnListener.isPressed && brakeListener.isPressed;
        bool isBurning = !isChargingBurst && burnListener.isPressed;
        bool isBreaking = !isChargingBurst && brakeListener.isPressed;

        if (isChargingBurst)
        {
            if (!wasChargingBurstLastFrame)
                currentChargeTime = 0;

            currentChargeTime = Mathf.Min(config.maxChargeTime, currentChargeTime + Time.fixedDeltaTime);
            isInPerfectChargeZone = Mathf.Abs(currentChargeTime - config.perfectChargeTime) <= config.perfectChargeTolerance;
            wasChargingBurstLastFrame = true;
        }
        else if (isBurning)
        {
            float currentSpeedInAxis = Vector3.Project(velocity, transform.forward).magnitude;
            float accelerationRatio = config.accelerationCurve.Evaluate(currentSpeedInAxis / config.maxSpeed);

            float acceleration = config.maxAcceleration *
                accelerationRatio *
                (backWallSensor.IsColliding ? config.accelerationMultAgainstWall : 1f) *
                Time.fixedDeltaTime;

            velocity += acceleration * transform.forward;
            velocity = Vector3.ClampMagnitude(velocity, config.maxSpeed);


            wasChargingBurstLastFrame = false;
        }
        else if (isBreaking)
        {
            float currentSpeed = velocity.magnitude;
            float newSpeed = Mathf.Max(0, currentSpeed - config.decceleration * Time.fixedDeltaTime);
            velocity = velocity.normalized * newSpeed;

            wasChargingBurstLastFrame = false;
        }
        else
        {
            wasChargingBurstLastFrame = false;
            // Natural decceleration
            // None because I assume space friction is negligible
        }

        { // Rotation
            float rotationSpeed = isBurning ? config.burnRotationSpeed : config.noBurnRotationSpeed;
            float turnInput = turnListener.Value.x;
            if (turnInput != 0)
            {
                float rotationAmount = turnInput * rotationSpeed * Time.fixedDeltaTime;
                transform.Rotate(Vector3.up, rotationAmount, Space.World);
            }
        }

        { // Apply velocity
            rb.linearVelocity = velocity;
        }
    }

    private void OnDrawGizmos()
    {

        float chargeVelocity = config.chargeSpeedCurve.Evaluate(currentChargeTime / config.maxChargeTime) * config.maxChargeSpeed;

        DrawDirection(velocity, Color.cyan);
        DrawDirection(transform.forward * chargeVelocity, isInPerfectChargeZone ? Color.yellow : Color.magenta);
    }

    private void DrawDirection(Vector3 direction, Color color)
    {
        var position = transform.position + Vector3.up;
        var oldColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(position, position + direction / config.maxSpeed * debugScale);
        Gizmos.color = oldColor;
    }
}
