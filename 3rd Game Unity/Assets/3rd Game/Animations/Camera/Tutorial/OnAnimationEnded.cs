using UnityEngine;
using UnityEngine.Events;

public class OnAnimationEnded : MonoBehaviour
{
    public UnityEvent onAnimationEnded;

    private void Update()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            onAnimationEnded?.Invoke();
        }
    }
}
