using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ZodiacVariantOptions : MonoBehaviour
{
    public RectTransform rt;
	public Image zodiacImage;
	public TMP_InputField costInputField;
	public Label costLabel;
	public TMP_InputField pointsInputField;
	public Label pointsLabel;
	public TMP_InputField multiplierInputField;
	public Label multiplierLabel;
	public ControllerSelectableObject costInputFieldControllerSelectableObject;
	public ControllerSelectableObject pointsInputFieldControllerSelectableObject;
	public ControllerSelectableObject multiplierInputFieldControllerSelectableObject;
	
	public TooltipObject tooltipObject;
	public string zodiacTag;
	public int handNumber;
	
	public void SetupZodiacVariantOptions(Sprite zodiacSprite, string zodiacTag, int handNumber, string zodiacDescription, string zodiacName)
	{
		zodiacImage.sprite = zodiacSprite;
		zodiacImage.color = LocalInterface.instance.rarities[5].rarityColor;
		this.zodiacTag = zodiacTag;
		this.handNumber = handNumber;
		// tooltipObject.mainText = zodiacDescription;
		tooltipObject.title = zodiacName;
		tooltipObject.titleColor = ThemeManager.UIElementType.Zodiac;
	}
	
	public void UpdateToolTip()
	{
		string tooltipMainText = VariantsMenu.instance.loadedVariant.variantBaubles[zodiacTag].menuDescription.Replace("[HandNameColor]", $"{LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.handNameColor)}");
		tooltipMainText = tooltipMainText.Replace("[PointsColor]", $"{LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.pointsColor)}");
		tooltipMainText = tooltipMainText.Replace("[MultColor]", $"{LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.multiplierColor)}");
		try
		{
			double pointsInput = double.Parse(pointsInputField.text);
			double multInput = double.Parse(multiplierInputField.text);
			tooltipObject.mainText = String.Format(tooltipMainText, LocalInterface.instance.ConvertDoubleToString(pointsInput), LocalInterface.instance.ConvertDoubleToString(multInput));
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"UpdateToolTip in ZodiacVariantOptions of tag={zodiacTag} failed, exception={exception.Message}");
		}
	}
	
	public void SetInteractability(bool enabledState)
	{
		costInputField.interactable = enabledState;
		pointsInputField.interactable = enabledState;
		multiplierInputField.interactable = enabledState;
	}
	
	public void SetZodiacVariantOptions(int cost, double points, double mult)
	{
		costInputField.text = $"{cost}";
		pointsInputField.text = $"{points}";
		multiplierInputField.text = $"{mult}";
		if(VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].baseCost == cost)
		{
			costLabel.ClearText();
		}
		else
		{
			costLabel.ChangeText($"{cost}");
		}
		if(Math.Abs(VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].impact1 - points) < LocalInterface.instance.epsilon)
		{
			pointsLabel.ClearText();
		}
		else
		{
			pointsLabel.ChangeText($"{points}");
		}
		if(Math.Abs(VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].impact2 - mult) < LocalInterface.instance.epsilon)
		{
			multiplierLabel.ClearText();
		}
		else
		{
			multiplierLabel.ChangeText($"{mult}");
		}
		UpdateToolTip();
	}
	
	public void InputFieldUpdated()
	{
		try
		{
			if(costInputField.text != string.Empty)
			{
				int costInput = int.Parse(costInputField.text);
				if(costInput < 0)
				{
					costInputField.text = "0";
				}
				else if(costInput > ZodiacsVariantMenu.instance.reasonableMaximumZodiacCost)
				{
					costInputField.text = $"{ZodiacsVariantMenu.instance.reasonableMaximumZodiacCost}";
				}
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Error parsing costInputField of {zodiacTag}. {exception.Message}");
			costInputField.text = $"{VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].baseCost}";
		}
		try
		{
			if(pointsInputField.text != string.Empty)
			{
				double pointsInput = double.Parse(pointsInputField.text);
				if(pointsInput < 0)
				{
					pointsInputField.text = "0";
				}
				else if (pointsInput > ZodiacsVariantMenu.instance.reasonableMaximumZodiacImpact)
				{
					pointsInputField.text = $"{ZodiacsVariantMenu.instance.reasonableMaximumZodiacImpact}";
				}
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Error parsing pointsInputField of {zodiacTag}. {exception.Message}");
			pointsInputField.text = VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].impact1.ToString();
		}
		try
		{
			if(multiplierInputField.text != string.Empty)
			{
				double multInput = double.Parse(multiplierInputField.text);
				if(multInput < 0)
				{
					multiplierInputField.text = "0";
				}
				else if (multInput > ZodiacsVariantMenu.instance.reasonableMaximumZodiacImpact)
				{
					multiplierInputField.text = $"{ZodiacsVariantMenu.instance.reasonableMaximumZodiacImpact}";
				}
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Error parsing multiplierInputField of {zodiacTag}. {exception.Message}");
			multiplierInputField.text = VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].impact2.ToString();
		}
	}
	
	public void InputUpdated()
	{
		int costInput = VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].baseCost;
		if(costInputField.text != string.Empty)
		{
			try
			{
				costInput = int.Parse(costInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Error parsing costInputField of {zodiacTag}. {exception.Message}");
			}
		}
		double pointsInput = VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].impact1;
		if(pointsInputField.text != string.Empty)
		{
			try
			{
				pointsInput = double.Parse(pointsInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Error parsing pointsInputField of {zodiacTag}. {exception.Message}");
			}
		}
		double multInput = VariantsMenu.instance.baseVariant.variantBaubles[zodiacTag].impact2;
		if(multiplierInputField.text != string.Empty)
		{
			try
			{
				multInput = double.Parse(multiplierInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Error parsing multiplierInputField of {zodiacTag}. {exception.Message}");
			}
		}
		SetZodiacVariantOptions(costInput, pointsInput, multInput);
	}
}
