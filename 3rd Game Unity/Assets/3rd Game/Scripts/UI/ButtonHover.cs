using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onHoverEvent;
    public UnityEvent onHoverExitEvent;
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHoverEvent?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExitEvent?.Invoke();
    }
}
