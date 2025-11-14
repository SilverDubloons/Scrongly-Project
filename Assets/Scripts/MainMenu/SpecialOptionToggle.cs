using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpecialOptionToggle : MonoBehaviour
{
	public RectTransform rt;
    public Toggle toggle;
	public Label label;
	public ControllerSelectableObject controllerSelectableObject;
	
	public bool isOn;
	public string specialOptionTag;
	// public bool hasChanged;
	public bool setupComplete;
	
	public void SetupSpecialOptionToggle(bool startsOn, string labelText, string tag)
	{
		UpdateToggle(startsOn);
		label.ChangeText(labelText);
		specialOptionTag = tag;
		setupComplete = true;
		controllerSelectableObject.isToggle = true;
		controllerSelectableObject.toggle = toggle;
		controllerSelectableObject.isElementInScrollView = true;
		controllerSelectableObject.scrollViewContentRT = SpecialOptionsVariantMenu.instance.specialOptionsVariantContentRectTransform;
		controllerSelectableObject.scrollViewVerticalScrollbar =  SpecialOptionsVariantMenu.instance.verticalScrollbar;
		StartCoroutine(SetPosition());
	}
	
	public IEnumerator SetPosition()
	{
		yield return null;
		controllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
	}
	
	public void SetInteractability(bool enabledState)
	{
		toggle.interactable = enabledState;
	}
	
	public void ToggleUpdated()
	{
		if(!setupComplete)
		{
			return;
		}
		/* hasChanged = true;
		SpecialOptionsVariantMenu.instance.OptionHasChanged(); */
		isOn = toggle.isOn;
	}
	
	public void UpdateToggle(bool newState)
	{
		isOn = newState;
		toggle.isOn = isOn;
	}
}
