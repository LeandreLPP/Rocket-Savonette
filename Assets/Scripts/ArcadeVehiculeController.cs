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
    public float accelerationTime = 15f;

    [Header("Straff")] 
    public float straffSpeed = 5f;
    public float straffAccelerationTime = 0.1f;
    
    [Header("Rotation")] 
    public float rotationSpeed = 5f;
    public float rotationAccelerationTime = 1f;

    [Header("Runtime")] 
    [ReadOnly] public Vector3 burnVelocity;
    [ReadOnly] public Vector3 straffVelocity;
    [ReadOnly] public float currentRotationSpeed;

    private Vector3 _burnDampVelocity;
    private Vector3 _straffDampVelocity;
    private float _rotationDampVelocity;
    
    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (burnListener.isPressed)
        {
            var targetBurn = transform.forward * maxSpeed;
            burnVelocity = Vector3.SmoothDamp(
                burnVelocity,
                targetBurn,
                ref _burnDampVelocity,
                accelerationTime,
                float.MaxValue,
                Time.deltaTime);
        }
        
        { // Straff
            var straffInput = straffListener.Value;
            var targetStraff = transform.rotation * new Vector3(straffInput.x, 0, 0) * straffSpeed;
            straffVelocity = Vector3.SmoothDamp(
                straffVelocity,
                targetStraff,
                ref _straffDampVelocity,
                straffAccelerationTime,
                float.MaxValue,
                Time.deltaTime);
        }
        _rigidbody.velocity = burnVelocity + straffVelocity;
        
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
}
