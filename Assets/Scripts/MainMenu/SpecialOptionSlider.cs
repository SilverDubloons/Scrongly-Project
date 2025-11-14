using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpecialOptionSlider : MonoBehaviour
{
	public RectTransform rt;
    public Label explanationLabel;
	public Slider slider;
	public Label outputLabel;
	public ControllerSelectableObject controllerSelectableObject;
	
	public int val;
	public string specialOptionTag;
	public bool hasChanged;
	public bool setupComplete;
	
	
	public void SetupSpecialOptionSlider(int min, int max, int curVal, string labelText, string tag)
	{
		slider.minValue = min;
		slider.maxValue = max;
		UpdateSlider(curVal);
		explanationLabel.ChangeText(labelText);
		specialOptionTag = tag;
		setupComplete = true;
		controllerSelectableObject.isSlider = true;
		controllerSelectableObject.slider = slider;
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
		slider.interactable = enabledState;
	}
	
	public void SliderUpdated()
	{
		if(!setupComplete)
		{
			return;
		}
/* 		hasChanged = true;
		SpecialOptionsVariantMenu.instance.OptionHasChanged(); */
		val = Mathf.RoundToInt(slider.value);
		outputLabel.ChangeText(val.ToString());
	}
	
	public void UpdateSlider(int newValue)
	{
		val = newValue;
		slider.value = val;
		outputLabel.ChangeText(val.ToString());
	}
}
