using UnityEngine;
using UnityEngine.UI;

public class UnlockableObject : MonoBehaviour
{
	public RectTransform rt;
    public Image image;
	public TooltipObject tooltipObject;
	public GameObject lockedObject;
	public BlackWhenLockedController blackWhenLockedController;
	public ControllerSelectableObject controllerSelectableObject;
}
