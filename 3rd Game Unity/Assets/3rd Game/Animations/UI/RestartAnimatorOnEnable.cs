using UnityEngine;

public class RestartAnimatorOnEnable : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        // Option 1: Rebind forces re-binding of all animation properties, returning to the default state
        animator.Rebind();

        // Option 2: If you need to start from a specific state, use Play
        // animator.Play("Idle", 0, 0f); // Parameters: state name, layer, normalized time (0 = beginning)
    }
}