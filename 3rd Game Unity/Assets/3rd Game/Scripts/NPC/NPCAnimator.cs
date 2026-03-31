using UnityEngine;

public class NPCAnimator : NPCComponent
{
    private void Update()
    {
        npc.animator.SetFloat("Horizontal", npc.currentSpeed);
    }
}
