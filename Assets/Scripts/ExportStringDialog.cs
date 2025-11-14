using UnityEngine;
using TMPro;

public class ExportStringDialog : MonoBehaviour
{
    public TMP_InputField inputField;
	public ButtonPlus backButton;
	
	public static ExportStringDialog instance;
	
	void Start()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		backButton.ChangeButtonEnabled(enabledState);
		inputField.interactable = enabledState;
	}
	
	public void SetupDialog(string variantString)
	{
		inputField.text = variantString;
	}
	
	public void BackClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["ExportStringDialog"].StartMove("OffScreen");
	}
}
