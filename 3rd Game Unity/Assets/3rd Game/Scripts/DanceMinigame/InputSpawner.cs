using UnityEngine;

public class InputSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] inputPrefab;
    [SerializeField] private GameObject spawner;
    [SerializeField] private GameObject endPoint;

    [Header("Spawn Interval")]
    [SerializeField] private float rerollEvery = 4f;

    public static float minIntervalStatic = 0.5f;
    public static float maxIntervalStatic = 3f;

    public static float spawnInterval = 3f;

    private float spawnTimer;
    private float rerollTimer;

    public static void SetIntervalRange(float min, float max)
    {
        minIntervalStatic = min;
        maxIntervalStatic = max;
    }

    private void Start()
    {
        RerollInterval();
        SpawnInputPrefab();
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnInputPrefab();
            spawnTimer = 0f;
        }

        rerollTimer += Time.deltaTime;
        if (rerollTimer >= rerollEvery)
        {
            RerollInterval();
            rerollTimer = 0f;
        }
    }

    private void RerollInterval()
    {
        spawnInterval = Random.Range(minIntervalStatic, maxIntervalStatic);
        Debug.Log($"New spawn interval: {spawnInterval:F2}s");
    }

    private void SpawnInputPrefab()
    {
        int randomIndex = Random.Range(0, inputPrefab.Length);
        GameObject spawnedInput = Instantiate(inputPrefab[randomIndex], spawner.transform.position, Quaternion.identity);
        spawnedInput.transform.SetParent(spawner.transform);
    }

    private void OnDrawGizmos()
    {
        if (spawner != null && endPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(spawner.transform.position, endPoint.transform.position);
        }
    }
}