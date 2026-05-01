using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SlideInTransition : MonoBehaviour
{
    [Header("Slide Settings")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Right;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Position Settings")]
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private bool useCurrentPosition = true;

    [Header("References")]
    [SerializeField] private RectTransform rectTransform;

    private Vector2 startPosition;
    private bool isAnimating = false;

    public enum SlideDirection
    {
        Left,
        Right
    }

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        // Store target position based on current position
        if (useCurrentPosition)
            targetPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        // Store the off-screen start position based on direction
        SetStartPosition();

        // Reset to start position immediately
        rectTransform.anchoredPosition = startPosition;

        // Start slide animation
        if (!isAnimating)
            StartCoroutine(SlideToTarget());
    }

    private void SetStartPosition()
    {
        RectTransform canvasRect = GetCanvasRectTransform();
        float canvasWidth = canvasRect.rect.width;

        startPosition = targetPosition;

        switch (slideDirection)
        {
            case SlideDirection.Left:
                startPosition.x = -canvasWidth;
                break;
            case SlideDirection.Right:
                startPosition.x = canvasWidth;
                break;
        }
    }

    private RectTransform GetCanvasRectTransform()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            return canvas.GetComponent<RectTransform>();

        // Fallback to finding canvas in scene
        canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
            return canvas.GetComponent<RectTransform>();

        Debug.LogError("No Canvas found in scene!");
        return null;
    }

    private IEnumerator SlideToTarget()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsedTime / slideDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;
        isAnimating = false;
    }

    // Public method to manually trigger slide (if needed)
    public void TriggerSlide()
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        else
            OnEnable();
    }

    // Editor visualization
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (rectTransform != null)
            DrawSlideGizmos();
    }

    private void DrawSlideGizmos()
    {
        // Get canvas reference
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector3 worldPosTarget = rectTransform.TransformPoint(targetPosition);

        // Draw target position (green)
        Gizmos.color = Color.green;
        DrawRectGizmo(worldPosTarget, rectTransform.rect.size);

        // Calculate start position based on direction
        float canvasWidth = canvasRect.rect.width;
        Vector2 startPosLocal = targetPosition;

        switch (slideDirection)
        {
            case SlideDirection.Left:
                startPosLocal.x = -canvasWidth;
                break;
            case SlideDirection.Right:
                startPosLocal.x = canvasWidth;
                break;
        }

        Vector3 worldPosStart = rectTransform.TransformPoint(startPosLocal);

        // Draw start position (red)
        Gizmos.color = Color.red;
        DrawRectGizmo(worldPosStart, rectTransform.rect.size);

        // Draw arrow showing slide direction
        Gizmos.color = Color.yellow;
        Vector3 direction = (worldPosTarget - worldPosStart).normalized;
        float distance = Vector3.Distance(worldPosStart, worldPosTarget);
        Vector3 midPoint = Vector3.Lerp(worldPosStart, worldPosTarget, 0.5f);

        // Draw line path
        Gizmos.DrawLine(worldPosStart, worldPosTarget);

        // Draw arrow head
        Vector3 arrowPerp = new Vector3(-direction.y, direction.x, 0);
        Vector3 arrowEnd1 = midPoint - direction * 20f + arrowPerp * 10f;
        Vector3 arrowEnd2 = midPoint - direction * 20f - arrowPerp * 10f;
        Gizmos.DrawLine(midPoint, arrowEnd1);
        Gizmos.DrawLine(midPoint, arrowEnd2);

        // Draw labels using Handles
        UnityEditor.Handles.BeginGUI();

        // Calculate screen positions
        Vector3 screenPosStart = UnityEditor.HandleUtility.WorldToGUIPoint(worldPosStart);
        Vector3 screenPosTarget = UnityEditor.HandleUtility.WorldToGUIPoint(worldPosTarget);

        // Style for labels
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 12;

        // Draw labels
        GUI.Label(new Rect(screenPosStart.x - 40, screenPosStart.y - 20, 80, 20), "START", style);
        GUI.Label(new Rect(screenPosTarget.x - 40, screenPosTarget.y - 20, 80, 20), "TARGET", style);

        UnityEditor.Handles.EndGUI();
    }

    private void DrawRectGizmo(Vector3 worldPosition, Vector2 size)
    {
        Vector3[] corners = new Vector3[4];
        corners[0] = worldPosition + new Vector3(-size.x / 2, -size.y / 2, 0);
        corners[1] = worldPosition + new Vector3(size.x / 2, -size.y / 2, 0);
        corners[2] = worldPosition + new Vector3(size.x / 2, size.y / 2, 0);
        corners[3] = worldPosition + new Vector3(-size.x / 2, size.y / 2, 0);

        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
        }

        // Draw cross in center
        float crossSize = 10f;
        Gizmos.DrawLine(worldPosition + new Vector3(-crossSize, 0, 0), worldPosition + new Vector3(crossSize, 0, 0));
        Gizmos.DrawLine(worldPosition + new Vector3(0, -crossSize, 0), worldPosition + new Vector3(0, crossSize, 0));
    }
#endif
}