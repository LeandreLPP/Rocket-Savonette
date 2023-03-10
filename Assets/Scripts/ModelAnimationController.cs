using UnityEngine;

public class ModelAnimationController : MonoBehaviour
{
    public BoolInputListener burnListener = default;
    public Vector2InputListener turnListener = default;
    public Vector2InputListener straffListener = default;

    private Animator animator;
    private float straffValue;
    private float turnValue;
    
    public float changeTime = 0.25f;
    private float changeSpeed => 1 / changeTime;

    private static readonly int _STRAFF = Animator.StringToHash("straff");
    private static readonly int _TURN = Animator.StringToHash("turn");
    private static readonly int _BURNING = Animator.StringToHash("burning");
    
    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        turnValue = Mathf.MoveTowards(turnValue, turnListener.Value.x, changeSpeed * Time.deltaTime);
        straffValue = Mathf.MoveTowards(straffValue, straffListener.Value.x, changeSpeed * Time.deltaTime);

        animator.SetBool(_BURNING, burnListener.isPressed);
        animator.SetFloat(_STRAFF, straffValue);
        animator.SetFloat(_TURN, turnValue);
    }
}
