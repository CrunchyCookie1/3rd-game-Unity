using Unity.VisualScripting;
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{
    public float speedMultiplier = 1f; // Drag this float in the Inspector
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = speedMultiplier;
    }
}