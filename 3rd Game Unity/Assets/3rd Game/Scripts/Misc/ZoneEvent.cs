using UnityEngine;
using UnityEngine.Events;

public class ZoneEvent : MonoBehaviour
{
    public UnityEvent onTriggerEvent;
    public Transform tpLocation;

    public bool TpOnTrigger = true;
    [SerializeField] bool destroyAfterTrigger = false;

    [Header("Save")]
    [SerializeField] private string saveKey;
    [SerializeField] private bool saveValue = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SaveManager.Instance != null && !string.IsNullOrEmpty(saveKey))
                SaveManager.Instance.SaveFlag(saveKey, saveValue);

            onTriggerEvent?.Invoke();

            if (tpLocation != null && TpOnTrigger == true)
            {
                other.transform.position = tpLocation.position;
                other.transform.rotation = tpLocation.rotation;
            }
            if (destroyAfterTrigger == true)
            {
                Destroy(gameObject);
            }
        }
    }

    public void TpToLocation()
    {
        if (tpLocation != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = tpLocation.position;
                player.transform.rotation = tpLocation.rotation;
            }
        }
    }
}