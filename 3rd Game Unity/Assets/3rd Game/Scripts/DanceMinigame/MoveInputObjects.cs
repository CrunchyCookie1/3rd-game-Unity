using UnityEngine;

public class MoveInputObjects : MonoBehaviour
{
    [SerializeField] private MinigameSettings minigameSettings;
    public static float dm_Speed = 200f;
    public static float hitWindowPadding = 0.5f; // forgiving timing window

    [Header("Assigned by RandomInputIcon")]
    public int currentIconIndex; // 0=Up, 1=Right, 2=Left, 3=Down

    [SerializeField] private float timeUntilDestroy;
    private bool isInsideTrigger;
    private bool hasBeenResolved; // hit or missed already
    private Collider2D triggerCollider;

    private void Start()
    {
        minigameSettings = FindFirstObjectByType<MinigameSettings>();
    }

    private void Update()
    {
        TwoDMoveRight();

        timeUntilDestroy += Time.deltaTime;
        if (timeUntilDestroy >= 20f)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyObjects()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EndPoint"))
        {
            // Player didn't hit it in time — count as a miss, then destroy.
            if (!hasBeenResolved)
            {
                hasBeenResolved = true;
                DancingMinigameController.Instance?.UnregisterActiveNote(this);
                DancingMinigameController.Instance?.OnNoteMissed(this);
            }
            minigameSettings.DecrementScoreValue(1);
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("dm_TriggerLine"))
        {
            isInsideTrigger = true;
            triggerCollider = collision;
            DancingMinigameController.Instance?.RegisterActiveNote(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("dm_TriggerLine"))
        {
            // Note left the line — just stop treating it as "on the line".
            // It is NOT destroyed here; it keeps moving toward EndPoint.
            isInsideTrigger = false;
            triggerCollider = null;
            DancingMinigameController.Instance?.UnregisterActiveNote(this);
        }
    }

    /// <summary>
    /// Returns true if the icon is currently overlapping the trigger line.
    /// </summary>
    public bool IsOnTrigger() => isInsideTrigger;

    public bool IsResolved() => hasBeenResolved;

    /// <summary>
    /// Called when the player successfully hits this note.
    /// </summary>
    public void ResolveHit()
    {
        if (hasBeenResolved) return;
        hasBeenResolved = true;
        DancingMinigameController.Instance?.UnregisterActiveNote(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Called when the note is removed due to a wrong input.
    /// </summary>
    public void ResolveMiss()
    {
        if (hasBeenResolved) return;
        hasBeenResolved = true;
        DancingMinigameController.Instance?.UnregisterActiveNote(this);
        Destroy(gameObject);
    }

    private void TwoDMoveRight()
    {
        transform.Translate(Vector3.right * Time.deltaTime * dm_Speed);
    }
}