using UnityEngine;

public class SwitchAnimations : MonoBehaviour
{
    public RuntimeAnimatorController animatorController1;

    public void switchAnimations()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null && animatorController1 != null)
        {
            animator.runtimeAnimatorController = animatorController1;
        }
    }
}
