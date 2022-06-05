using UnityEngine;

public class ModelAnimationController : MonoBehaviour
{
    public BoolInputListener burnListener = default;
    public Vector2InputListener turnListener = default;
    public Vector2InputListener straffListener = default;

    private Animator animator;
    private float straffValue = 0.5f;
    private float turnValue = 0.5f;
    public float changeTime = 0.25f;
    private float changeSpeed => 1 / changeTime;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        turnValue = Mathf.MoveTowards(turnValue, (turnListener.Value.x + 1) * 0.5f, changeSpeed * Time.deltaTime);
        straffValue = Mathf.MoveTowards(straffValue, (straffListener.Value.x + 1) * 0.5f, changeSpeed * Time.deltaTime);

        animator.SetBool("burning", burnListener.isPressed);
        animator.SetFloat("straff", straffValue);
        animator.SetFloat("turn", turnValue);
    }
}
