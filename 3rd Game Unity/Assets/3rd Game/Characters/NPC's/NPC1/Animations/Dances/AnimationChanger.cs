using UnityEngine;

public class AnimationChanger : MonoBehaviour
{
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

    }

    void PlaySelectedAnimation()
    {
        if (animationClip == null) return;
        animator.Play(animationClip.name);
    }
}
