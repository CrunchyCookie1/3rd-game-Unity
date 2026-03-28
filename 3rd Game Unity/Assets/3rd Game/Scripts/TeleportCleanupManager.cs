using UnityEngine;
using System.Collections.Generic;

public class TeleportCleanupManager : MonoBehaviour
{
    private static TeleportCleanupManager instance;

    [Header("Cleanup Settings")]
    [SerializeField] private bool enableCleanup = true;
    [SerializeField] private float cleanupDelay = 0.5f; // Delay after teleport before cleanup
    [SerializeField] private bool unloadObjectsByDistance = true;
    [SerializeField] private float unloadDistance = 50f; // Unload objects beyond this distance
    [SerializeField] private bool unloadByTag = false;
    [SerializeField] private string[] tagsToUnload;
    [SerializeField] private bool unloadByLayer = false;
    [SerializeField] private LayerMask layersToUnload;

    [Header("Loading Settings")]
    [SerializeField] private bool loadObjectsOnTeleport = true;
    [SerializeField] private float loadRadius = 30f; // Load objects within this radius

    [Header("Object Management")]
    [SerializeField] private List<GameObject> objectsToManage = new List<GameObject>();
    [SerializeField] private bool autoDetectObjects = true;
    [SerializeField] private string[] autoDetectTags;

    [Header("Performance")]
    [SerializeField] private bool useObjectPooling = false;
    [SerializeField] private int poolSize = 20;

    private GameObject currentPlayer;
    private Dictionary<GameObject, bool> objectActiveState = new Dictionary<GameObject, bool>();
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    private bool isTeleporting = false;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (autoDetectObjects)
        {
            AutoDetectObjects();
        }

        if (useObjectPooling)
        {
            InitializeObjectPool();
        }
    }

    private void AutoDetectObjects()
    {
        // Find objects by tag
        if (autoDetectTags != null && autoDetectTags.Length > 0)
        {
            foreach (string tag in autoDetectTags)
            {
                GameObject[] foundObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in foundObjects)
                {
                    if (!objectsToManage.Contains(obj))
                    {
                        objectsToManage.Add(obj);
                    }
                }
            }
        }

        // Find objects by layer
        if (unloadByLayer)
        {
            // Updated to use FindObjectsByType instead of FindObjectsOfType
            GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                if (((1 << obj.layer) & layersToUnload) != 0)
                {
                    if (!objectsToManage.Contains(obj))
                    {
                        objectsToManage.Add(obj);
                    }
                }
            }
        }
    }

    private void InitializeObjectPool()
    {
        // Create object pool for frequently spawned objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject pooledObj = new GameObject($"PooledObject_{i}");
            pooledObj.SetActive(false);
            objectPool.Enqueue(pooledObj);
        }
    }

    public static void OnPlayerTeleported(Vector3 newPosition)
    {
        if (instance == null || !instance.enableCleanup) return;

        instance.StartCoroutine(instance.PerformCleanup(newPosition));
    }

    private System.Collections.IEnumerator PerformCleanup(Vector3 newPosition)
    {
        if (isTeleporting) yield break;

        isTeleporting = true;

        // Wait for teleport effects to finish
        yield return new WaitForSeconds(cleanupDelay);

        // Unload far objects
        if (unloadObjectsByDistance)
        {
            UnloadFarObjects(newPosition);
        }

        // Unload by tag
        if (unloadByTag && tagsToUnload != null)
        {
            UnloadObjectsByTag();
        }

        // Unload by layer
        if (unloadByLayer)
        {
            UnloadObjectsByLayer();
        }

        // Load nearby objects
        if (loadObjectsOnTeleport)
        {
            LoadNearbyObjects(newPosition);
        }

        isTeleporting = false;
    }

    private void UnloadFarObjects(Vector3 playerPosition)
    {
        List<GameObject> objectsToDeactivate = new List<GameObject>();

        foreach (GameObject obj in objectsToManage)
        {
            if (obj == null) continue;

            float distance = Vector3.Distance(playerPosition, obj.transform.position);

            if (distance > unloadDistance)
            {
                if (obj.activeSelf)
                {
                    objectsToDeactivate.Add(obj);
                    objectActiveState[obj] = true;
                }
            }
        }

        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false);
            Debug.Log($"Unloaded object: {obj.name} (Distance: {Vector3.Distance(playerPosition, obj.transform.position)})");
        }
    }

    private void UnloadObjectsByTag()
    {
        foreach (string tag in tagsToUnload)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                if (obj.activeSelf)
                {
                    if (!objectActiveState.ContainsKey(obj))
                    {
                        objectActiveState[obj] = true;
                    }
                    obj.SetActive(false);
                    Debug.Log($"Unloaded object by tag: {obj.name} (Tag: {tag})");
                }
            }
        }
    }

    private void UnloadObjectsByLayer()
    {
        // Updated to use FindObjectsByType instead of FindObjectsOfType
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (((1 << obj.layer) & layersToUnload) != 0)
            {
                if (obj.activeSelf)
                {
                    if (!objectActiveState.ContainsKey(obj))
                    {
                        objectActiveState[obj] = true;
                    }
                    obj.SetActive(false);
                    Debug.Log($"Unloaded object by layer: {obj.name} (Layer: {LayerMask.LayerToName(obj.layer)})");
                }
            }
        }
    }

    private void LoadNearbyObjects(Vector3 playerPosition)
    {
        foreach (var kvp in objectActiveState)
        {
            GameObject obj = kvp.Key;
            bool wasActive = kvp.Value;

            if (obj == null) continue;

            float distance = Vector3.Distance(playerPosition, obj.transform.position);

            if (distance <= loadRadius && wasActive && !obj.activeSelf)
            {
                obj.SetActive(true);
                Debug.Log($"Loaded object: {obj.name} (Distance: {distance})");
            }
        }
    }

    public static void RegisterObject(GameObject obj)
    {
        if (instance == null) return;

        if (!instance.objectsToManage.Contains(obj))
        {
            instance.objectsToManage.Add(obj);
            instance.objectActiveState[obj] = obj.activeSelf;
        }
    }

    public static void UnregisterObject(GameObject obj)
    {
        if (instance == null) return;

        if (instance.objectsToManage.Contains(obj))
        {
            instance.objectsToManage.Remove(obj);
            instance.objectActiveState.Remove(obj);
        }
    }

    public static void ForceCleanup()
    {
        if (instance == null || instance.currentPlayer == null) return;

        instance.StartCoroutine(instance.PerformCleanup(instance.currentPlayer.transform.position));
    }
}