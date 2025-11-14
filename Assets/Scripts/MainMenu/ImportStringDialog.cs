using UnityEngine;
using TMPro;
using System;

public class ImportStringDialog : MonoBehaviour
{
    public ButtonPlus cancelButton;
    public ButtonPlus confirmButton;
    public ButtonPlus pasteButton;
	public TMP_InputField inputField;
	
	public static ImportStringDialog instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		cancelButton.ChangeButtonEnabled(enabledState);
		pasteButton.ChangeButtonEnabled(enabledState);
		UpdateConfirmButtonInteractablity(enabledState);
		inputField.interactable = enabledState;
	}
	
	public void UpdateConfirmButtonInteractablity(bool enabledState)
	{
		if(inputField.text == string.Empty)
		{
			confirmButton.ChangeButtonEnabled(false);
		}
		else
		{
			confirmButton.ChangeButtonEnabled(enabledState);
		}
	}
	
	public void InputFieldUpdated()
	{
		UpdateConfirmButtonInteractablity(true);
	}
	
	public void ClearText()
	{
		inputField.text = string.Empty;
	}
	
	public void CancelClicked()
	{
		ClearText();
		MovingObjects.instance.mo["ImportStringDialog"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
	public void ConfirmClicked()
	{
		try
		{
			Variant newVariant = new Variant(inputField.text);
			VariantsMenu.instance.loadedVariant = newVariant;
			VariantsMenu.instance.loadedVariantBeforeChanges = newVariant;
			VariantsMenu.instance.loadedVariantSimple.UpdateVariantSimpleForVariant(newVariant);
			MovingObjects.instance.mo["ImportStringDialog"].StartMove("OffScreen");
			MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
			MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
			MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		}
		catch(Exception exception1)
		{
			try
			{
				Variant newVariant = new Variant(inputField.text.Trim());
				VariantsMenu.instance.loadedVariant = newVariant;
				VariantsMenu.instance.loadedVariantBeforeChanges = newVariant;
				VariantsMenu.instance.loadedVariantSimple.UpdateVariantSimpleForVariant(newVariant);
				MovingObjects.instance.mo["ImportStringDialog"].StartMove("OffScreen");
				MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
				MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
				MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
			}
			catch(Exception exception2)
			{
				MinorNotifications.instance.NewMinorNotification("Bad Input!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(confirmButton.rt, LocalInterface.instance.mainMenuCanvas));
				LocalInterface.instance.DisplayError($"Import string exception1={exception1.Message} exception2={exception2.Message}");
				VariantsMenu.instance.loadedVariant = LocalInterface.instance.baseVariant;
				VariantsMenu.instance.loadedVariantBeforeChanges = LocalInterface.instance.baseVariant;
				VariantsMenu.instance.loadedVariantSimple.UpdateVariantSimpleForVariant(LocalInterface.instance.baseVariant);
			}
		}
	}
	
	public void PasteClicked()
	{
		#if UNITY_WEBGL// && !UNITY_EDITOR
			
		#else
			string clipBoard = GUIUtility.systemCopyBuffer;
			inputField.text = clipBoard;
		#endif
	}
}
