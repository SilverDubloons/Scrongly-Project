using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class BaubleVariantOptions : MonoBehaviour
{
	public RectTransform rt;
	public Toggle inShopToggle;
    public Image baubleImage;
	public Image baubleRarityImage;
	public Slider baubleRaritySlider;
	public Label baubleRarityLabel;
	public TMP_InputField baseCostInputField;
	public Label baseCostLabel;
	public Label baseCostInfoLabel;
	public TMP_InputField costStepInputField;
	public Label costStepLabel;
	public Label costStepInfoLabel;
	public Slider startingQuantitySlider;
	public Label startingQuantityLabel;
	public GameObject notInShopIndicator;
	public ControllerSelectableObject inShopToggleControllerSelectableObject;
	public ControllerSelectableObject baubleRaritySliderControllerSelectableObject;
	public ControllerSelectableObject baseCostInputFieldControllerSelectableObject;
	public ControllerSelectableObject costStepInputFieldControllerSelectableObject;
	public ControllerSelectableObject startingQuantitySliderControllerSelectableObject;
	
	public int rarityInt;
	public string rarity;
	public int baseCost;
	public int costStep;
	public bool inShop;
	public string baubleTag;
	public string baubleName;
	public int startingQuantity;
	public TooltipObject tooltipObject;
	public bool setupComplete;
	// public bool hasChanged;
	
	public void SetupBaubleVariantOptions(bool baubleInShop, Sprite baubleSprite, string baubleRarity, int baubleBaseCost, int baubleCostStep, int baubleStartingQuantity, int maxQuantity, string baubleTag, string baubleName, string baubleDescription)
	{
		inShopToggle.isOn = baubleInShop;
		inShop = baubleInShop;
		baubleImage.sprite = baubleSprite;
		rarity = baubleRarity;
		this.baubleTag = baubleTag;
		// baubleRarityImage.color = LocalInterface.instance.rarityDictionary[baubleRarity].rarityColor;
		rarityInt = LocalInterface.instance.rarityDictionary[baubleRarity].rarityInt;
		baubleRaritySlider.value = rarityInt;
		// baubleRarityLabel.ChangeText(baubleRarity);
		// baseCostLabel.ChangeText($"${baubleBaseCost}");
		baseCost = baubleBaseCost;
		baseCostInputField.text = baseCost.ToString();
		// Debug.Log($"{baubleName} baubleBaseCost = {baubleBaseCost} baseCost = {baseCost} baseCostInputField.text = {baseCostInputField.text}");
		costStep = baubleCostStep;
		costStepInputField.text = costStep.ToString();
		if(maxQuantity == 1)
		{
			// costStepInputField.interactable = false;
			costStepInputField.gameObject.SetActive(false);
			costStepInfoLabel.gameObject.SetActive(false);
			baseCostInfoLabel.ChangeText("Cost");
		}
		// costStepLabel.ChangeText($"+{baubleCostStep}");
		startingQuantitySlider.value = baubleStartingQuantity;
		startingQuantitySlider.maxValue = maxQuantity;
		startingQuantity = baubleStartingQuantity;
		// startingQuantityLabel.ChangeText(baubleStartingQuantity.ToString());
		this.baubleName = baubleName;
		// tooltipObject.mainText = baubleDescription;
		// UpdateTooltip();
		tooltipObject.title = baubleName;
		tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
		// tooltipObject.subtitle = baubleRarity;
		// tooltipObject.subtitleColor = LocalInterface.instance.rarityDictionary[baubleRarity].rarityColor;
		UpdateLabels();
		inShopToggleControllerSelectableObject.scrollViewVerticalScrollbar = BaubleVariantsMenu.instance.verticalScrollbar;
		baubleRaritySliderControllerSelectableObject.scrollViewVerticalScrollbar = BaubleVariantsMenu.instance.verticalScrollbar;
		baseCostInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = BaubleVariantsMenu.instance.verticalScrollbar;
		costStepInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = BaubleVariantsMenu.instance.verticalScrollbar;
		startingQuantitySliderControllerSelectableObject.scrollViewVerticalScrollbar = BaubleVariantsMenu.instance.verticalScrollbar;
		inShopToggleControllerSelectableObject.scrollViewContentRT = BaubleVariantsMenu.instance.baubleVariantOptionsContentRectTransform;
		baubleRaritySliderControllerSelectableObject.scrollViewContentRT = BaubleVariantsMenu.instance.baubleVariantOptionsContentRectTransform;
		baseCostInputFieldControllerSelectableObject.scrollViewContentRT = BaubleVariantsMenu.instance.baubleVariantOptionsContentRectTransform;
		costStepInputFieldControllerSelectableObject.scrollViewContentRT = BaubleVariantsMenu.instance.baubleVariantOptionsContentRectTransform;
		startingQuantitySliderControllerSelectableObject.scrollViewContentRT = BaubleVariantsMenu.instance.baubleVariantOptionsContentRectTransform;
		baseCostInputFieldControllerSelectableObject.minInputInt = 0;
		baseCostInputFieldControllerSelectableObject.maxInputInt = 999;
		costStepInputFieldControllerSelectableObject.minInputInt = 0;
		costStepInputFieldControllerSelectableObject.maxInputInt = 999;
		StartCoroutine(SetPosition());
		setupComplete = true;
	}
	
	public IEnumerator SetPosition()
	{
		yield return null;
		inShopToggleControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
		baubleRaritySliderControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
		baseCostInputFieldControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
		costStepInputFieldControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
		startingQuantitySliderControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
	}
	
	public void SetBaubleVariantOptionsButtons(bool enabledState)
	{
		inShopToggle.interactable = enabledState;
		baubleRaritySlider.interactable = enabledState;
		baseCostInputField.interactable = enabledState;
		costStepInputField.interactable = enabledState;
		startingQuantitySlider.interactable = enabledState;
	}
	
	public void UpdateLabels()
	{
		if(inShop)
		{
			notInShopIndicator.SetActive(false);
		}
		else
		{
			notInShopIndicator.SetActive(true);
		}
		if(VariantsMenu.instance.baseVariant.variantBaubles[baubleTag].baseCost == baseCost)
		{
			baseCostLabel.ClearText();
		}
		else
		{
			baseCostLabel.ChangeText($"${baseCost}");
		}
		if(VariantsMenu.instance.baseVariant.variantBaubles[baubleTag].costStep == costStep)
		{
			costStepLabel.ClearText();
		}
		else
		{
			costStepLabel.ChangeText($"+{costStep}");
		}
		if(startingQuantity == 0)
		{
			startingQuantityLabel.ClearText();
		}
		else
		{
			startingQuantityLabel.ChangeText($"{startingQuantity}");
		}
		baubleRarityLabel.ChangeText(rarity);
		baubleRarityLabel.ChangeColor(LocalInterface.instance.rarityDictionary[rarity].rarityColor);
		tooltipObject.subtitle = rarity;
		tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(rarity);
		if(VariantsMenu.instance.baseVariant.variantBaubles[baubleTag].category == rarity)
		{
			baubleRarityImage.gameObject.SetActive(false);
		}
		else
		{
			baubleRarityImage.gameObject.SetActive(true);
			baubleRarityImage.color = LocalInterface.instance.rarityDictionary[rarity].rarityColor;
		}
		UpdateTooltip();
	}
	
	public void UpdateTooltip()
	{
		if(VariantsMenu.instance.loadedVariant == null)
		{
			tooltipObject.mainText = VariantsMenu.instance.baseVariant.variantBaubles[baubleTag].menuDescription;
		}
		else
		{
			var resolver = new DescriptionResolver();
			string input = VariantsMenu.instance.baseVariant.variantBaubles[baubleTag].menuDescription;
			string output = resolver.Resolve(input);
			tooltipObject.mainText = output;
		}
	}
	
	public void UpdateInput()
	{
		inShopToggle.isOn = inShop;
		baubleRaritySlider.value = rarityInt;
		// Debug.Log($"{baubleTag}'s startingQuantity={startingQuantity}");
		startingQuantitySlider.value = startingQuantity;
		baseCostInputField.text = baseCost.ToString();
		if(Mathf.RoundToInt(startingQuantitySlider.maxValue) > 1)
		{
			costStepInputField.text = costStep.ToString();
		}
	}
	
	public void InputFieldsFinished()
	{
		if(!setupComplete)
		{
			return;
		}
		// Debug.Log($"{baubleName} InputFieldsFinished");
		if(baseCostInputField.text == string.Empty)
		{
			baseCostInputField.text = "0";
		}
		if(costStepInputField.text == string.Empty)
		{
			costStepInputField.text = "0";
		}
		baseCost = int.Parse(baseCostInputField.text);
		costStep = int.Parse(costStepInputField.text);
		BaubleOptionsUpdated();
	}
	
	public void InputFieldsUpdated()
	{
		if(!setupComplete)
		{
			return;
		}
		// Debug.Log($"{baubleName} InputFieldsUpdated");
		if(baseCostInputField.text != string.Empty)
		{
			int baseCostInput = -1;
			try
			{
				baseCostInput = int.Parse(baseCostInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"InvalidCharacter in {name} BaubleVariantOptions baseCostInputField: {exception.Message}");
			}
			if(baseCostInput < 0)
			{
				baseCostInputField.text = "0";
			}
			else if(baseCostInput > BaubleVariantsMenu.instance.reasonableBaubleCostMaximum)
			{
				baseCostInputField.text = BaubleVariantsMenu.instance.reasonableBaubleCostMaximum.ToString();
			}
		}
		if(costStepInputField.text != string.Empty)
		{
			int costStepInput = -1;
			try
			{
				costStepInput = int.Parse(costStepInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"InvalidCharacter in {name} BaubleVariantOptions costStepInputField: {exception.Message}");
			}
			if(costStepInput < 0)
			{
				costStepInputField.text = "0";
			}
			else if(costStepInput > BaubleVariantsMenu.instance.reasonableBaubleCostMaximum)
			{
				costStepInputField.text = BaubleVariantsMenu.instance.reasonableBaubleCostMaximum.ToString();
			}
		}
	}
	
	public void BaubleOptionsUpdated()
	{
		if(!setupComplete)
		{
			return;
		}
		inShop = inShopToggle.isOn;
		rarityInt = Mathf.RoundToInt(baubleRaritySlider.value);
		rarity = LocalInterface.instance.ConvertIntToRarity(rarityInt);
		startingQuantity = Mathf.RoundToInt(startingQuantitySlider.value);
		// hasChanged = true;
		// BaubleVariantsMenu.instance.BaubleHasChanged();
		UpdateLabels();
	}
}
