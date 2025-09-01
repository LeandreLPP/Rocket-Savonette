using UnityEngine;

[CreateAssetMenu(fileName = "VC3Config", menuName = "Configs/VC3Config", order = 1)]
public class VC3Config : ScriptableObject
{
    [Header("Speed")]
    public float maxSpeed = 20f;
    public AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float maxAcceleration = 10f;
    public float accelerationMultAgainstWall = 2f;
    [Space]
    public float decceleration = 5f;

    [Header("Charge Burst")]
    public AnimationCurve chargeSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float maxChargeSpeed = 20f;
    public float maxChargeTime = 2f;
    public float perfectChargeTime = 1.5f;
    public float perfectChargeTolerance = 0.2f;

    [Header("Rotation")]
    public float noBurnRotationSpeed = 5f;
    public float burnRotationSpeed = 1f;
} 