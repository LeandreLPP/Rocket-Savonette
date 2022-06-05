
using UnityEngine;

public class VehiculeController : MonoBehaviour
{
    public BoolInputListener burnListener = default;
    public Vector2InputListener turnListener = default;
    public Vector2InputListener straffListener = default;
    public float acceleration = 1.0f;
    public float straff = 1.0f;
    public float rotation = 15.0f;

    public Sensor rearSensor;
    public float bonusAccelerationMax = 500f;
    public AnimationCurve bonusAccelerationCurve = default;

    public float dragMin = 0.5f;
    public float dragMax = 2f;
    public float bonusDragTargetSpeed = 150f;
    public AnimationCurve bonusDragCurve = default;

    new private Rigidbody rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        // // Ensure more drag when going sideways
        // var dragRatio = 1 - Vector3.Dot(rigidbody.velocity.normalized, transform.forward.normalized);
        // rigidbody.drag = Mathf.Lerp(frontalDrag, lateralDrag, dragRatio);

        rigidbody.drag = 0;

        // Acceleration
        if( burnListener.isPressed ) {
            // Increase drag when burning
            var dragRatio = Mathf.InverseLerp(0, bonusDragTargetSpeed, rigidbody.velocity.magnitude);
            // dragRatio = bonusDragCurve.Evaluate(dragRatio);
            rigidbody.drag = Mathf.LerpUnclamped(dragMin, dragMax, dragRatio);

            var curAcceleration = acceleration * Time.fixedDeltaTime;

            // Accelerate stronger when burning against walls
            if( rearSensor.IsColliding ) {
                var bonusAcceleration = bonusAccelerationMax * bonusDragCurve.Evaluate(1 - rearSensor.Ratio);
                curAcceleration += bonusAcceleration;
            }

            rigidbody.AddForce( transform.forward * curAcceleration, ForceMode.Acceleration );
        }

        // Straffing
        rigidbody.AddForce( transform.right * straff * straffListener.Value.x *Time.fixedDeltaTime, ForceMode.Acceleration );

        // Turning
        rigidbody.AddRelativeTorque(0, turnListener.Value.x * rotation * Time.fixedDeltaTime, 0, ForceMode.Acceleration);
    }
}
