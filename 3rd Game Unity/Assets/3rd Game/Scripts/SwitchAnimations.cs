using UnityEditor.Animations;
using UnityEngine;

public class SwitchAnimations : MonoBehaviour
{
    public AnimatorController animatorController1;

    public void switchAnimations()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null && animatorController1 != null)
        {
            animator.runtimeAnimatorController = animatorController1;
        }
    }
}
