using UnityEngine;
using UnityEngine.Events;

public class BuildingCard : MonoBehaviour
{
    public UnityEvent onHover;
    private void OnMouseEnter()
    {
        onHover.Invoke();
    }
}
