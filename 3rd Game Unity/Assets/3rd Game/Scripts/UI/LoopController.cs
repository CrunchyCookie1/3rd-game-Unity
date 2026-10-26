using UnityEngine;

public class LoopController : MonoBehaviour
{
    private Animator animator;
    private int targetLayer = 0;
    private bool hasTriggered = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        hasTriggered = false;
        animator.Rebind();
    }

    void Update()
    {
        if (hasTriggered) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(targetLayer);
        int loopCount = (int)stateInfo.normalizedTime;

        if (loopCount >= 3)
        {
            hasTriggered = true;
            gameObject.SetActive(false);
        }
    }
}