// InteractionZone.cs - Attach this to your interactionZone GameObject
using UnityEngine;

public class InteractionZone : MonoBehaviour
{
    public NPCManager npcManager; // Reference to the NPC manager

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (npcManager != null)
            {
                npcManager.SetPlayerInZone(true, other.gameObject);
                Debug.Log("Entered NPC interaction zone - Press Interact key");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (npcManager != null)
            {
                npcManager.SetPlayerInZone(false);
                Debug.Log("Exited NPC interaction zone");
            }
        }
    }
}