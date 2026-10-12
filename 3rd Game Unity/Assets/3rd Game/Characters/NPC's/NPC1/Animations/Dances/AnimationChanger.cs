using UnityEngine;

public class AnimationChanger : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    public void ChangeAnimationController(RuntimeAnimatorController _controller)
    {
        animator.runtimeAnimatorController = _controller;
    }
}
