using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ResourceTab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onHover;
    public UnityEvent onHoverExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit.Invoke();
    }
}
