using UnityEngine;
using TMPro;
using System;

public class SeedInput : MonoBehaviour
{
    public TMP_InputField inputField;
	public static SeedInput instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		inputField.interactable = enabledState;
	}
	
	public void InputFieldUpdated()
	{
		if(inputField.text.Length <= 0)
		{
			return;
		}
		if(inputField.text == "-")
		{
			inputField.text = "";
		}
		else
		{
			try
			{
				long input = long.Parse(inputField.text);
				if(input > int.MaxValue)
				{
					input = int.MaxValue;
					inputField.text = input.ToString();
				}
			}
			catch(Exception exception)
			{
				Debug.Log($"Issue parsing inputField of SeedInput, {exception.Message}");
			}
		}
	}
	
	public int GetSeed()
	{
		if(inputField.text == "")
		{
			return UnityEngine.Random.Range(0, int.MaxValue);
		}
		else
		{
			try
			{
				int input = int.Parse(inputField.text);
				return input;
			}
			catch(Exception exception)
			{
				MinorNotifications.instance.NewMinorNotification("Issue with GetSeed", LocalInterface.instance.GetMousePosition(), Vector2.zero);
				Debug.Log($"Issue parsing inputField of SeedInput, {exception.Message}");
				return 0;
			}
		}
	}
}
