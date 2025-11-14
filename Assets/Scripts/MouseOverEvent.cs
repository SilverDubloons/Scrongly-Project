using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MouseOverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
    public UnityEvent onPointerEnterEvent;
	[SerializeField]
    public UnityEvent onPointerExitEvent;
	
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
		onPointerEnterEvent.Invoke();
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		onPointerExitEvent.Invoke();
	}
}
