using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DancingMinigameController : MonoBehaviour
{
    public static DancingMinigameController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MinigameSettings minigameSettings;
    [SerializeField] private Transform triggerLine;

    [Header("Input")]
    [SerializeField] private InputActionReference danceAction;
    [SerializeField] private float deadzone = 0.5f;

    [Header("Scoring")]
    [SerializeField] private int wrongInputPenalty = 1;

    [Header("Audio")]
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioSource missAudioSource;
    [SerializeField] private AudioClip missClip;

    [Header("Input Events")]
    [SerializeField] private UnityEvent onUpInput;
    [SerializeField] private UnityEvent onRightInput;
    [SerializeField] private UnityEvent onLeftInput;
    [SerializeField] private UnityEvent onDownInput;

    private readonly List<MoveInputObjects> activeNotes = new List<MoveInputObjects>();

    private const int UP = 0, RIGHT = 1, LEFT = 2, DOWN = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        if (danceAction != null)
        {
            danceAction.action.Enable();
            danceAction.action.performed += OnDanceInput;
        }
    }

    private void OnDisable()
    {
        if (danceAction != null)
            danceAction.action.performed -= OnDanceInput;
    }

    private void OnDanceInput(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        if (v.sqrMagnitude < deadzone * deadzone) return;

        int index;
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            index = v.x > 0 ? RIGHT : LEFT;
        else
            index = v.y > 0 ? UP : DOWN;

        HandleInput(index);
    }

    private void FireDirectionEvent(int index)
    {
        switch (index)
        {
            case UP: onUpInput?.Invoke(); break;
            case RIGHT: onRightInput?.Invoke(); break;
            case LEFT: onLeftInput?.Invoke(); break;
            case DOWN: onDownInput?.Invoke(); break;
        }
    }

    public void RegisterActiveNote(MoveInputObjects note)
    {
        if (!activeNotes.Contains(note))
            activeNotes.Add(note);
    }

    public void UnregisterActiveNote(MoveInputObjects note)
    {
        activeNotes.Remove(note);
    }

    private void HandleInput(int inputIndex)
    {
        MoveInputObjects matchingNote = null;

        foreach (var note in activeNotes)
        {
            if (note == null || note.IsResolved()) continue;
            if (note.IsOnTrigger() && note.currentIconIndex == inputIndex)
            {
                matchingNote = note;
                break;
            }
        }

        if (matchingNote != null)
        {
            matchingNote.ResolveHit();
            minigameSettings.IncrementScoreValue();
            PlayHitSound();
            FireDirectionEvent(inputIndex);
            return;
        }

        MoveInputObjects noteToPenalize = GetClosestActiveNote();
        if (noteToPenalize != null)
        {
            noteToPenalize.ResolveMiss();
            minigameSettings.DecrementScoreValue(wrongInputPenalty);
            PlayMissSound();
        }
    }

    private MoveInputObjects GetClosestActiveNote()
    {
        MoveInputObjects closest = null;
        float closestDist = float.MaxValue;
        Vector3 origin = triggerLine != null ? triggerLine.position : Vector3.zero;

        foreach (var note in activeNotes)
        {
            if (note == null || note.IsResolved()) continue;
            float d = Vector3.Distance(note.transform.position, origin);
            if (d < closestDist) { closestDist = d; closest = note; }
        }
        return closest;
    }

    private void PlayHitSound()
    {
        if (hitAudioSource != null && hitClip != null)
            hitAudioSource.PlayOneShot(hitClip);
    }

    private void PlayMissSound()
    {
        if (missAudioSource != null && missClip != null)
            missAudioSource.PlayOneShot(missClip);
    }

    public void OnNoteMissed(MoveInputObjects note)
    {
    }
}