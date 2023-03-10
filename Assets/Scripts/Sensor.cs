
using UnityEngine;
using UnityEngine.Events;

public class Sensor : MonoBehaviour
{
    [SerializeField]
    private LayerMask collisionMask;

    [SerializeField, Min(0.1f)]
    private float radius = 1.0f;

    [SerializeField, Min(0.1f)]
    private float length = 3.0f;

    public float Ratio{ get; private set; }
    public bool IsColliding{ get; private set; }
    public UnityEvent OnColliding;
    public UnityEvent OnStopColliding;

    private void FixedUpdate() {

        RaycastHit hit;
        bool doHit = Physics.SphereCast(transform.position, radius, transform.forward, out hit, length, collisionMask);
        bool changedState = IsColliding != doHit;
        IsColliding = doHit;

        if( doHit ) { 
            if( changedState ) OnColliding?.Invoke();
            Ratio = Mathf.Min(Vector3.Distance(transform.position, hit.point) / length, 1);
        }
        else if( !doHit ) {
            if( changedState ) OnStopColliding?.Invoke();
            Ratio = 1;
        }
    }

    private void OnDrawGizmosSelected() {
        // Origin
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Max
        Gizmos.DrawWireSphere(transform.position + transform.forward * length, radius);

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * length);

        // Collide
        if( IsColliding ) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward * length * Ratio, radius);
        }
    }
}
