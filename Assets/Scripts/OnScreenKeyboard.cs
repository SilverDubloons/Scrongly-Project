using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Reflection;
using System;

public class OnScreenKeyboard : MonoBehaviour
{
	public GameObject visibilityObject;
	public GameObject keyboardObject;
	public GameObject numpadObject;
	public OnScreenKeyboardKey[] keyboardKeys;
	public OnScreenKeyboardKey[] numpadKeys;
	public ButtonPlus capsLockButton;
	public ButtonPlus leftShiftButton;
	public ButtonPlus rightShiftButton;
	public ButtonPlus decimalButton;
	public ButtonPlus numpadBackspaceButton;
	public ButtonPlus keyboardBackspaceButton;
	public ButtonPlus doneButton;
	public ButtonPlus keypadNegativeButton;
	public ControllerSelectionGroup keyboardControllerSelectionGroup;
	public ControllerSelectionGroup numpadControllerSelectionGroup;
	public TMP_InputField inputField;
	public TMP_InputField targetInputField;
	
	public bool inputtingInt;
	public int minInt;
	public int maxInt;
	public bool inputtingFloat;
	public float minFloat;
	public float maxFloat;
	public bool inputtingDouble;
	public double minDouble;
	public double maxDouble;
	public bool inputtingString;
	public int maxChars;
	public string regexValidationString;
	public bool inputCanBeEmpty;
	
	public bool capsLock;
	public bool shift;

	public static OnScreenKeyboard instance;
	private FieldInfo caretField;
	
	public void SetupInstance()
	{
		instance = this;
		visibilityObject.SetActive(false);
		caretField = typeof(TMP_InputField).GetField("m_CaretVisible", BindingFlags.NonPublic | BindingFlags.Instance);
	}
	
	public void CapsLockPressed()
	{
		capsLock = !capsLock;
		capsLockButton.ChangeSpecialState(capsLock);
		UpdateKeyDisplay();
	}
	
	public void BackspacePressed()
	{
		int oldCaretPosition = inputField.caretPosition;
		string currentInput = inputField.text;
		string newInput = currentInput.Substring(0, inputField.caretPosition - 1) + currentInput.Substring(inputField.caretPosition);
		inputField.text = newInput;
		inputField.caretPosition = oldCaretPosition - 1;
		InputFieldUpdated();
	}
	
	public void ShiftPressedDown()
	{
		shift = true;
		UpdateKeyDisplay();
	}
	
	public void ShiftReleased()
	{
		shift = false;
		UpdateKeyDisplay();
	}
	
	public void DonePressed()
	{
		targetInputField.text = inputField.text;
		visibilityObject.SetActive(false);
		if(inputtingInt || inputtingFloat || inputtingDouble)
		{
			numpadControllerSelectionGroup.RemoveFromCurrentGroups();
		}
		else if(inputtingString)
		{
			keyboardControllerSelectionGroup.RemoveFromCurrentGroups();
		}
	}
	
	public void CancelClicked(bool activateTargetInputField)
	{
		visibilityObject.SetActive(false);
		if(inputtingInt || inputtingFloat || inputtingDouble)
		{
			numpadControllerSelectionGroup.RemoveFromCurrentGroups();
		}
		else if(inputtingString)
		{
			keyboardControllerSelectionGroup.RemoveFromCurrentGroups();
		}
		if(activateTargetInputField)
		{
			targetInputField.Select();
			targetInputField.ActivateInputField();
		}
	}
	
	public void NumpadNegativeClicked()
	{
		inputField.text = "-" + inputField.text;
	}
	
	public void MoveCaretLeft()
	{
		if(inputField.caretPosition > 0)
		{
			inputField.caretPosition = inputField.caretPosition - 1;
		}
		InputFieldUpdated();
	}
	
	public void MoveCaretRight()
	{
		if(inputField.caretPosition < inputField.text.Length)
		{
			inputField.caretPosition = inputField.caretPosition + 1;
		}
		InputFieldUpdated();
	}
	
	public void UpdateKeyDisplay()
	{
		Regex regex = new Regex(regexValidationString);
		for(int i = 0; i < keyboardKeys.Length; i++)
		{
			if(keyboardKeys[i].altChar != '\0')
			{
				if(capsLock && keyboardKeys[i].affectedByCapsLock)
				{
					if(shift)
					{
						keyboardKeys[i].buttonPlus.ChangeButtonText(keyboardKeys[i].baseChar.ToString());
						bool inputAcceptable = regex.IsMatch(keyboardKeys[i].baseChar.ToString());
						keyboardKeys[i].buttonPlus.ChangeButtonEnabled(inputAcceptable);
					}
					else
					{
						keyboardKeys[i].buttonPlus.ChangeButtonText(keyboardKeys[i].altChar.ToString());
						bool inputAcceptable = regex.IsMatch(keyboardKeys[i].altChar.ToString());
						keyboardKeys[i].buttonPlus.ChangeButtonEnabled(inputAcceptable);
					}
				}
				else
				{
					if(shift)
					{
						keyboardKeys[i].buttonPlus.ChangeButtonText(keyboardKeys[i].altChar.ToString());
						bool inputAcceptable = regex.IsMatch(keyboardKeys[i].altChar.ToString());
						keyboardKeys[i].buttonPlus.ChangeButtonEnabled(inputAcceptable);
					}
					else
					{
						keyboardKeys[i].buttonPlus.ChangeButtonText(keyboardKeys[i].baseChar.ToString());
						bool inputAcceptable = regex.IsMatch(keyboardKeys[i].baseChar.ToString());
						keyboardKeys[i].buttonPlus.ChangeButtonEnabled(inputAcceptable);
					}
				}
			}
		}
	}
	
	public void KeyboardButtonPressed(char baseChar, char altChar, bool affectedByCapsLock)
	{
		char newChar = GetButtonChar(baseChar, altChar, affectedByCapsLock);
		string currentInput = inputField.text;
		string newInput = currentInput.Substring(0, inputField.caretPosition) + newChar + currentInput.Substring(inputField.caretPosition);
		inputField.text = newInput;
		inputField.caretPosition = inputField.caretPosition + 1;
		InputFieldUpdated();
	}
	
	public char GetButtonChar(char baseChar, char altChar, bool affectedByCapsLock)
	{
		char newChar = '\0';
		if(capsLock && affectedByCapsLock)
		{
			if(shift)
			{
				newChar = baseChar;
			}
			else
			{
				newChar = altChar;
			}
		}
		else
		{
			if(altChar == '\0')
			{
				newChar = baseChar;
			}
			else
			{
				if(shift)
				{
					newChar = altChar;
				}
				else
				{
					newChar = baseChar;
				}
			}
		}
		return newChar;
	}
	
	public void InputFieldUpdated()
	{
		string currentInput = inputField.text;
/* 		string stringStart = currentInput.Substring(0, inputField.caretPosition);
		string stringEnd = currentInput.Substring(inputField.caretPosition); */
		Regex regex = new Regex(regexValidationString);
		bool inputAcceptable = regex.IsMatch(currentInput);
		if(inputCanBeEmpty && inputField.text.Length == 0)
		{
			doneButton.ChangeButtonEnabled(true);
		}
		else
		{
			doneButton.ChangeButtonEnabled(inputAcceptable);
		}
		if(inputtingInt || inputtingFloat || inputtingDouble)
		{
/* 			for(int i = 0; i < numpadKeys.Length; i++)
			{
				if(inputField.text.Length >= maxChars)
				{
					numpadKeys[i].buttonPlus.ChangeButtonEnabled(false);
				}
				else
				{
					char newChar = GetButtonChar(numpadKeys[i].baseChar,  numpadKeys[i].altChar, numpadKeys[i].affectedByCapsLock);
					string newString = stringStart + newChar + stringEnd;
					numpadKeys[i].buttonPlus.ChangeButtonEnabled(regex.IsMatch(newString));
				}
			}
			Debug.Log($"currentInput.Length={currentInput.Length}, inputField.caretPosition={inputField.caretPosition}"); */
			if(currentInput.Length <= 0 || inputField.caretPosition == 0)
			{
				numpadBackspaceButton.ChangeButtonEnabled(false);
			}
			else
			{
				numpadBackspaceButton.ChangeButtonEnabled(true);
/* 				string newString = stringStart.Substring(0, stringStart.Length - 1) + stringEnd;
				numpadBackspaceButton.ChangeButtonEnabled(regex.IsMatch(newString)); */
			}
			if(inputtingInt && inputField.text.Length > 0 && inputAcceptable)
			{
				try
				{
					long input = long.Parse(inputField.text);
					if(input > int.MaxValue)
					{
						inputField.text = int.MaxValue.ToString();
					}
					if(input < int.MinValue)
					{
						inputField.text = int.MinValue.ToString();
					}
					if(input > maxInt)
					{
						inputField.text = maxInt.ToString();
					}
					if(input < minInt)
					{
						inputField.text = minInt.ToString();
					}
				}
				catch(Exception exception)
				{
					Debug.Log($"Issue parsing inputField int of OnScreenKeyboard, {exception.Message}");
					inputField.text = "";
					InputFieldUpdated();
				}
			}
			else if(inputtingFloat && inputField.text.Length > 0 && inputAcceptable)
			{
				try
				{
					float input = float.Parse(inputField.text);
					if(input > maxFloat)
					{
						inputField.text = maxFloat.ToString();
					}
					if(input < minFloat)
					{
						inputField.text = minFloat.ToString();
					}
				}
				catch(Exception exception)
				{
					Debug.Log($"Issue parsing inputField float of OnScreenKeyboard, {exception.Message}");
					inputField.text = "";
					InputFieldUpdated();
				}
			}
			else if(inputtingDouble && inputField.text.Length > 0 && inputAcceptable)
			{
				try
				{
					double input = double.Parse(inputField.text);
					if(input > maxDouble)
					{
						inputField.text = maxDouble.ToString();
					}
					if(input < minDouble)
					{
						inputField.text = minDouble.ToString();
					}
				}
				catch(Exception exception)
				{
					Debug.Log($"Issue parsing inputField double of OnScreenKeyboard, {exception.Message}");
					inputField.text = "";
					InputFieldUpdated();
				}
			}
		}
		else if(inputtingString)
		{
			if(currentInput.Length <= 0 || inputField.caretPosition == 0)
			{
				keyboardBackspaceButton.ChangeButtonEnabled(false);
			}
			else
			{
				keyboardBackspaceButton.ChangeButtonEnabled(true);
			}
			if(inputField.text.Length > 0 && inputAcceptable)
			{
				if(inputField.text.Length > maxChars)
				{
					doneButton.ChangeButtonEnabled(false);
				}
			}
		}
		inputField.Select();
		inputField.ActivateInputField();
		caretField?.SetValue(inputField, true);
	}
	
	public void SetupOnScreenKeyboard(TMP_InputField targetInputField, ControllerSelectableObject controllerSelectableObject)
	{
		// Debug.Log("SetupOnScreenKeyboard");
		visibilityObject.SetActive(true);
		maxChars = targetInputField.characterLimit;
		this.targetInputField = targetInputField;
		inputField.text = targetInputField.text;
		inputField.caretPosition = inputField.text.Length;
		this.inputCanBeEmpty = controllerSelectableObject.inputCanBeEmpty;
		if(controllerSelectableObject.inputString)
		{
			numpadObject.SetActive(false);
			keyboardObject.SetActive(true);
			regexValidationString = controllerSelectableObject.regexValidationString;
			inputtingInt = false;
			inputtingFloat = false;
			inputtingDouble = false;
			inputtingString = true;
			keyboardControllerSelectionGroup.AddToCurrentGroups();
			UpdateKeyDisplay();
		}
		else 
		{
			numpadObject.SetActive(true);
			keyboardObject.SetActive(false);
			inputtingString = false;
			numpadControllerSelectionGroup.AddToCurrentGroups();
			if(controllerSelectableObject.inputFloat)
			{
				keypadNegativeButton.ChangeButtonEnabled(controllerSelectableObject.minInputFloat < 0);
				if(controllerSelectableObject.inputCanBeEmpty)
				{
					regexValidationString = "^(?:-?\\d*\\.?\\d+)?$";
				}
				else
				{
					regexValidationString = "^-?\\d*\\.?\\d+$";
				}
				decimalButton.ChangeButtonEnabled(true);
				minFloat = controllerSelectableObject.minInputFloat;
				maxFloat = controllerSelectableObject.maxInputFloat;
				inputtingInt = false;
				inputtingFloat = true;
				inputtingDouble = false;
			}
			else if(controllerSelectableObject.inputInt)
			{
				keypadNegativeButton.ChangeButtonEnabled(controllerSelectableObject.minInputInt < 0);
				if(controllerSelectableObject.inputCanBeEmpty)
				{
					regexValidationString = "^(-?[1-9]\\d*|0|-?0)?$";
				}
				else
				{
					// regexValidationString = "^[0-9]+$";
					regexValidationString = "^(-?[1-9]\\d*|0|-?0)?$";
				}
				decimalButton.ChangeButtonEnabled(false);
				minInt = controllerSelectableObject.minInputInt;
				maxInt = controllerSelectableObject.maxInputInt;
				inputtingInt = true;
				inputtingFloat = false;
				inputtingDouble = false;
			}
			else if(controllerSelectableObject.inputDouble)
			{
				keypadNegativeButton.ChangeButtonEnabled(controllerSelectableObject.minInputDouble < 0);
				if(controllerSelectableObject.inputCanBeEmpty)
				{
					regexValidationString = "^(?:-?\\d*\\.?\\d+)?$";
				}
				else
				{
					regexValidationString = "^-?\\d*\\.?\\d+$";
				}
				decimalButton.ChangeButtonEnabled(true);
				minDouble = controllerSelectableObject.minInputDouble;
				maxDouble = controllerSelectableObject.maxInputDouble;
				inputtingInt = false;
				inputtingFloat = false;
				inputtingDouble = true;
			}
		}
		InputFieldUpdated();
		inputField.Select();
		inputField.ActivateInputField();
	}
}