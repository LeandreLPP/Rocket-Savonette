using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public Vector3 offset = default;
    public float lookForward = 3f;

    private Camera _camera;

    private void Start() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        var curOffset = target.rotation * offset;
        transform.position = target.position + curOffset;

        _camera.transform.LookAt(target.position + target.forward * lookForward);
    }
}
