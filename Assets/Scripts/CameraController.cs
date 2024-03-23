using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = default;
    public float lookForward = 3f;

    public float dampTime = 5f;

    private Camera _camera;
    private Rigidbody _targetRigidbody;
    private Vector3 _currentVelocity;
    private Vector3 _damp;

    private void Start() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        // var curOffset = target.rotation * offset;
        // transform.position = target.position + curOffset;

        _targetRigidbody ??= target.GetComponent<Rigidbody>();
        _currentVelocity = Vector3.SmoothDamp(_currentVelocity,
            _targetRigidbody.velocity,
            ref _damp,
            dampTime,
            Mathf.Infinity,
            Time.deltaTime);

        transform.position += _currentVelocity * Time.deltaTime;
        
        _camera.transform.LookAt(target.position + target.forward * lookForward);
    }
}
