using UnityEngine;

public class CameraController : MonoBehaviour
{
    public VehiculeController vehicule;

    public Vector3 offset = default;
    public float lookForward = 3f;

    new private Camera camera;

    private void Start() {
        camera = GetComponent<Camera>();
    }

    private void Update() {
        var curOffset = vehicule.transform.rotation * offset;
        transform.position = vehicule.transform.position + curOffset;

        camera.transform.LookAt(vehicule.transform.position + vehicule.transform.forward * lookForward);
    }
}
