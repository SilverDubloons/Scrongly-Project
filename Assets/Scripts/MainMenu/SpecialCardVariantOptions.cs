using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpecialCardVariantOptions : MonoBehaviour
{
    public RectTransform rt;
	public Toggle inShopToggle;
	public Image specialCardImage;
	public Image rarityImage;
	public Slider raritySlider;
	public TMP_InputField costInputField;
	public GameObject impactInfoLabel;
	public TMP_InputField impactInputField;
	public GameObject notInShopIndicator;
	public Label costLabel;
	public Label impactLabel;
	public Label rarityLabel;
	public ControllerSelectableObject inShopToggleControllerSelectableObject;
	public ControllerSelectableObject raritySliderControllerSelectableObject;
	public ControllerSelectableObject costInputFieldControllerSelectableObject;
	public ControllerSelectableObject impactInputFieldControllerSelectableObject;
	
	public string specialCardTag;
	public string specialCardName;
	public TooltipObject tooltipObject;
	
	public void SetupSpecialCardVariantOptions(Sprite specialCardSprite, string specialCardTag, string specialCardName, string specialCardDescription)
	{
		specialCardImage.sprite = specialCardSprite;
		this.specialCardTag = specialCardTag;
		this.specialCardName = specialCardName;
		tooltipObject.mainText = specialCardDescription;
		tooltipObject.title = specialCardName;
		tooltipObject.titleColor = ThemeManager.UIElementType.CardName;
		if(VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].impact < LocalInterface.instance.epsilon)
		{
			impactLabel.gameObject.SetActive(false);
			impactInfoLabel.SetActive(false);
			impactInputField.gameObject.SetActive(false);
		}
	}
	
	public void SetInteractability(bool enabledState)
	{
		inShopToggle.interactable = enabledState;
		raritySlider.interactable = enabledState;
		costInputField.interactable = enabledState;
	}
	
	public void SetSpecialCardVariantOptions(bool inShop, string rarity, int cardCost, double impact)
	{
		inShopToggle.isOn = inShop;
		int rarityInt = LocalInterface.instance.rarityDictionary[rarity].rarityInt;
		raritySlider.value = rarityInt;
		costInputField.text = cardCost.ToString();
		if(inShop)
		{
			notInShopIndicator.SetActive(false);
		}
		else
		{
			notInShopIndicator.SetActive(true);
		}
		if(VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].cost == cardCost)
		{
			costLabel.ClearText();
		}
		else
		{
			costLabel.ChangeText($"${cardCost}");
		}
		rarityLabel.ChangeText(rarity);
		rarityLabel.ChangeColor(LocalInterface.instance.rarityDictionary[rarity].rarityColor);
		if(VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].category == rarity)
		{
			rarityImage.gameObject.SetActive(false);
		}
		else
		{
			rarityImage.gameObject.SetActive(true);
			rarityImage.color = LocalInterface.instance.rarityDictionary[rarity].rarityColor;
		}
		tooltipObject.subtitle = rarity;
		tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(rarity);
		impactInputField.text = impact.ToString();
		if(impactInfoLabel.activeSelf)
		{
			if(Math.Abs(VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].impact - impact) < LocalInterface.instance.epsilon)
			{
				impactLabel.ClearText();
			}
			else
			{
				impactLabel.ChangeText($"i{impact.ToString()}");
			}
		}
		UpdateTooltip();
	}
	
	public void UpdateTooltip()
	{
		if(VariantsMenu.instance.loadedVariant == null)
		{
			tooltipObject.mainText = VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].description;
		}
		else
		{
			var resolver = new DescriptionResolver();
			string input = VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].description;
			string output = resolver.Resolve(input);
			tooltipObject.mainText = output;
		}
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
				else if(costInput > SpecialCardsVariantMenu.instance.reasonableMaximumCardCost)
				{
					costInputField.text = $"{SpecialCardsVariantMenu.instance.reasonableMaximumCardCost}";
				}
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Error parsing costInputField of {specialCardTag}. {exception.Message}");
			costInputField.text = $"{VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].cost}";
		}
		try
		{
			if(impactInputField.text != string.Empty)
			{
				double impactInput = double.Parse(impactInputField.text);
				if(impactInput < 0)
				{
					impactInputField.text = "0";
				}
				else if (impactInput > SpecialCardsVariantMenu.instance.reasonableMaximumCardImpact)
				{
					impactInputField.text = $"{SpecialCardsVariantMenu.instance.reasonableMaximumCardImpact}";
				}
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Error parsing impactInputField of {specialCardTag}. {exception.Message}");
			impactInputField.text = VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].impact.ToString();
		}
	}
	
	public void InputUpdated()
	{
		int costInput = VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].cost;
		if(costInputField.text != string.Empty)
		{
			try
			{
				costInput = int.Parse(costInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Error parsing costInputField of {specialCardTag}. {exception.Message}");
			}
		}
		else
		{
			costInput = 0;
		}
		double impactInput = VariantsMenu.instance.baseVariant.variantSpecialCards[specialCardTag].impact;
		if(impactInputField.text != string.Empty)
		{
			try
			{
				impactInput = double.Parse(impactInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Error parsing impactInputField of {specialCardTag}. {exception.Message}");;
			}
		}
		else
		{
			impactInput = 0;
		}
		SetSpecialCardVariantOptions(inShopToggle.isOn, LocalInterface.instance.ConvertIntToRarity(Mathf.RoundToInt(raritySlider.value)), costInput, impactInput);
	}
}
