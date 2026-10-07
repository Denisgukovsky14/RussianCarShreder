using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickTouchBlocker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // Блокируем передачу клика камере
        eventData.Use();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        eventData.Use();
    }
}
