using UnityEngine;
using System.Collections.Generic;

public class ControllerSelectionGroup : MonoBehaviour
{
    public List<ControllerSelectableObject> controllerSelectableObjects = new List<ControllerSelectableObject>();
	public ControllerSelectableObject defaultObject;
	public ControllerSelectableObject lastSelectedObject;
	public int priority; // when there's multiple selection groups active and the controller input is activated, the one with the highest priority's defualt object will be activated.
	public int availabilityPriority; // only the objects in a group with the highest availabilityPriority will be selectable. For most cases, this should be 0. But for example when the menu is open, this will make it so that only the objects in the menu are selectable, despite other groups still being active.
	public bool neverAutoSelect;
	public Canvas canvas;
	public bool ignoreAvailablilityPriority;
	
	public void AddToCurrentGroups()
	{
		ControllerSelection.instance.AddControllerSelectionGroup(this);
	}
	
	public void RemoveFromCurrentGroups()
	{
		ControllerSelection.instance.RemoveControllerSelectionGroup(this);
	}
	
	public void RemoveControllerSelectableObjectFromGroup(ControllerSelectableObject controllerSelectableObject)
	{
		if(controllerSelectableObjects.Contains(controllerSelectableObject))
		{
			controllerSelectableObjects.Remove(controllerSelectableObject);
		}
	}
}
