using UnityEngine;
using UnityEngine.Events;

public class OptionsDialog : MonoBehaviour
{
	public RectTransform backdropRT;
    public GameObject[] activationObjects;
	public Label dialogLabel;
	public RectTransform buttonParent;
	public ButtonPlus[] buttons;
	public ButtonPlus cancelButton;
	public float buttonWidth;
	public float distanceBetweenButtons;
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public static OptionsDialog instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	void Start()
	{
		SetVisibility(false);
	}
	
	// OptionsDialog.instance.SetupDialog($"Would you Like to Save Over {loadedVariant.variantName}?", new string[2]{"Yes", "Save New"}, new Color[2]{Color.blue, Color.blue}, new UnityAction[2]{SaveOverClicked, SaveAsClicked});
	
	// OptionsDialog.instance.SetupDialog("Putting this item on layaway will delete the old layaway item. Continue?", new string[1]{"Yes"}, new Color[1]{Color.blue}, new UnityAction[1]{MoveItemToLayawayAndDeleteOldItem});
	
	// include this in the actions
	// OptionsDialog.instance.SetVisibility(false);
	// using UnityEngine.Events;
	
	public void SetupDialog(string dialogString, string[] buttonTexts, ThemeManager.UIElementType[] buttonColors, UnityAction[] buttonActions, bool includeCancel = true)
	{
		SetVisibility(true);
		int numberOfButtons = buttonTexts.Length;
		int buttonIndex = 0;
		if(includeCancel)
		{
			numberOfButtons++;
			buttonIndex++;
			cancelButton.gameObject.SetActive(true);
			cancelButton.ChangeButtonBaseColor(ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.backButtonActive));
		}
		else
		{
			cancelButton.gameObject.SetActive(false);
		}
		if(numberOfButtons >= 4)
		{
			backdropRT.sizeDelta = new Vector2(380 + 125 * (numberOfButtons - 3), 0);
		}
		else
		{
			backdropRT.sizeDelta = new Vector2(380, 0);
		}
		dialogLabel.ChangeText(dialogString);
		backdropRT.sizeDelta = new Vector2(backdropRT.sizeDelta.x, dialogLabel.GetPreferredHeight() + 39f);
		for(int i = 0; i < buttons.Length; i++)
		{
			if(i < buttonTexts.Length)
			{
				buttons[i].gameObject.SetActive(true);
				buttons[i].ChangeButtonText(buttonTexts[i]);
				buttons[i].ChangeButtonBaseColor(ThemeManager.instance.GetColorFromCurrentTheme(buttonColors[i]));
				buttons[i].onClickEvent.RemoveAllListeners();
				buttons[i].onClickEvent.AddListener(buttonActions[i]);
				buttons[i].rt.anchoredPosition = new Vector2(-backdropRT.sizeDelta.x / 2 + buttonWidth / 2 + distanceBetweenButtons + (buttonWidth + distanceBetweenButtons) * buttonIndex, 0);
				if(numberOfButtons == 2 && buttonIndex == 1)
				{
					buttons[i].rt.anchoredPosition = new Vector2(buttons[i].rt.anchoredPosition.x + buttonWidth + distanceBetweenButtons, 0);
				}
				buttonIndex++;
			}
			else
			{
				buttons[i].gameObject.SetActive(false);
			}
		}
		controllerSelectionGroup.AddToCurrentGroups();
	}
	
	public void SetVisibility(bool visibleState)
	{
		for(int i = 0; i < activationObjects.Length; i++)
		{
			activationObjects[i].SetActive(visibleState);
		}
		controllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public void CancelClicked()
	{
		SetVisibility(false);
	}
}
