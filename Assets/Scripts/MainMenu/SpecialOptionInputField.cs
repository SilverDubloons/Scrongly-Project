using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class SpecialOptionInputField : MonoBehaviour
{
    public RectTransform rt;
	public Label explanationLabel;
	public TMP_InputField inputField;
	public ControllerSelectableObject controllerSelectableObject;
	
	public int min;
	public int max;
	public int val;
	public string specialOptionTag;
	public bool hasChanged;
	public bool setupComplete;
	
	
	public void SetupSpecialOptionInputField(int min, int max, int curVal, string labelText, string tag)
	{
		this.min = min;
		this.max = max;
		UpdateInputField(curVal);
		explanationLabel.ChangeText(labelText);
		specialOptionTag = tag;
		setupComplete = true;
		controllerSelectableObject.inputInt = true;
		controllerSelectableObject.minInputInt = min;
		controllerSelectableObject.maxInputInt = max;
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
		inputField.interactable = enabledState;
	}
	
	public void InputFieldUpdated()
	{
		if(!setupComplete)
		{
			return;
		}
		if(inputField.text != string.Empty)
		{
			int inputInt = min - 1;
			try
			{
				inputInt = int.Parse(inputField.text);
			}
			catch(Exception exception)
			{
				Debug.LogError($"InvalidCharacter in {name} SpecialOptionInputField: {exception.Message}");
			}
			if(inputInt < min)
			{
				inputInt = min;
				inputField.text = inputInt.ToString();
			}
			else if(inputInt > max)
			{
				inputInt = max;
				inputField.text = inputInt.ToString();
			}
		}
	}
	
	public void InputFieldFinished()
	{
		if(!setupComplete)
		{
			return;
		}
		if(inputField.text == string.Empty)
		{
			inputField.text = min.ToString();
		}
		val = int.Parse(inputField.text);
/* 		hasChanged = true;
		SpecialOptionsVariantMenu.instance.OptionHasChanged(); */
	}
	
	public void UpdateInputField(int newValue)
	{
		val = newValue;
		inputField.text = val.ToString();
	}
}
