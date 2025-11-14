using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Variant;

public class SpecialCardsVariantMenu : MonoBehaviour
{
    public ButtonPlus defaultButton;
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
    public RectTransform specialCardVariantOptionsParent;
	public RectTransform specialCardOptionsContentRectTransform;
	public GameObject specialCardVariantOptionsPrefab;
	public ControllerSelectionGroup controllerSelectionGroup;
	public Scrollbar verticalScrollbar;
	
	public int reasonableMaximumCardCost;
	public double reasonableMaximumCardImpact;
	public int specialCardVariantOptionsWide;
	public float distanceBetweenSpecialCardVariantOptions;
	public Vector2 specialCardVariantOptionsSize;
	
	// Dictionary<string, SpecialCardVariantOptions> specialCardVariantOptions = new Dictionary<string, SpecialCardVariantOptions>();
	public List<SpecialCardVariantOptions> specialCardVariantOptions = new List<SpecialCardVariantOptions>();
	public Dictionary<string, SpecialCardVariantOptions> specialCardVariantOptionsDictionary = new Dictionary<string, SpecialCardVariantOptions>();
	
	public static SpecialCardsVariantMenu instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		for(int i = 0; i < specialCardVariantOptions.Count; i++)
		{
			specialCardVariantOptions[i].SetInteractability(enabledState);
		}
		confirmButton.ChangeButtonEnabled(enabledState);
		defaultButton.ChangeButtonEnabled(enabledState);
		cancelButton.ChangeButtonEnabled(enabledState);	
	}
	
	public void SetupSpecialCardsVariantMenu(Variant variant)
	{
		foreach(KeyValuePair<string, VariantSpecialCard> entry in variant.variantSpecialCards)
		{
			if((entry.Value.category == "Common" || entry.Value.category == "Uncommon" || entry.Value.category == "Rare" || entry.Value.category == "Legendary") && entry.Value.inShop && (!entry.Value.mustBeUnlocked || entry.Value.mustBeUnlocked && LocalInterface.instance.IsSpecialCardUnlocked(entry.Key)))
			{
				GameObject newSpecialCardVariantOptionsGO = Instantiate(specialCardVariantOptionsPrefab, specialCardVariantOptionsParent);
				newSpecialCardVariantOptionsGO.name = entry.Value.specialCardName;
				SpecialCardVariantOptions newSpecialCardVariantOptions = newSpecialCardVariantOptionsGO.GetComponent<SpecialCardVariantOptions>();
				specialCardVariantOptions.Add(newSpecialCardVariantOptions);
				specialCardVariantOptionsDictionary.Add(entry.Key, newSpecialCardVariantOptions);
				newSpecialCardVariantOptions.SetupSpecialCardVariantOptions(entry.Value.sprite, entry.Value.tag, entry.Value.specialCardName, entry.Value.description);
				controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialCardVariantOptions.inShopToggleControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialCardVariantOptions.raritySliderControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialCardVariantOptions.costInputFieldControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialCardVariantOptions.impactInputFieldControllerSelectableObject);
				newSpecialCardVariantOptions.inShopToggleControllerSelectableObject.scrollViewContentRT = specialCardVariantOptionsParent;
				newSpecialCardVariantOptions.raritySliderControllerSelectableObject.scrollViewContentRT = specialCardVariantOptionsParent;
				newSpecialCardVariantOptions.costInputFieldControllerSelectableObject.scrollViewContentRT = specialCardVariantOptionsParent;
				newSpecialCardVariantOptions.impactInputFieldControllerSelectableObject.scrollViewContentRT = specialCardVariantOptionsParent;
				newSpecialCardVariantOptions.inShopToggleControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newSpecialCardVariantOptions.raritySliderControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newSpecialCardVariantOptions.costInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newSpecialCardVariantOptions.impactInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
			}
		}
		specialCardVariantOptions.Sort((x, y) =>
		{
			return x.specialCardName.CompareTo(y.specialCardName);
		});
		for(int i = 0; i < specialCardVariantOptions.Count; i++)
		{
			specialCardVariantOptions[i].rt.anchoredPosition = new Vector2(distanceBetweenSpecialCardVariantOptions + (i % specialCardVariantOptionsWide) * (distanceBetweenSpecialCardVariantOptions + specialCardVariantOptionsSize.x), -distanceBetweenSpecialCardVariantOptions - (i / specialCardVariantOptionsWide) * (distanceBetweenSpecialCardVariantOptions + specialCardVariantOptionsSize.y));
			specialCardVariantOptions[i].inShopToggleControllerSelectableObject.positionInScrollView = specialCardVariantOptions[i].rt.anchoredPosition.y;
			specialCardVariantOptions[i].raritySliderControllerSelectableObject.positionInScrollView = specialCardVariantOptions[i].rt.anchoredPosition.y;
			specialCardVariantOptions[i].costInputFieldControllerSelectableObject.positionInScrollView = specialCardVariantOptions[i].rt.anchoredPosition.y;
			specialCardVariantOptions[i].impactInputFieldControllerSelectableObject.positionInScrollView = specialCardVariantOptions[i].rt.anchoredPosition.y;
		}
		specialCardOptionsContentRectTransform.sizeDelta = new Vector2(specialCardOptionsContentRectTransform.sizeDelta.x, distanceBetweenSpecialCardVariantOptions + ((specialCardVariantOptions.Count + specialCardVariantOptionsWide - 1) / specialCardVariantOptionsWide) * (distanceBetweenSpecialCardVariantOptions + specialCardVariantOptionsSize.y));
	}
	
	public void SetSpecialCardVariantOptionsToVariant(Variant variant)
	{
		for(int i = 0; i < specialCardVariantOptions.Count; i++)
		{
			specialCardVariantOptions[i].SetSpecialCardVariantOptions(variant.variantSpecialCards[specialCardVariantOptions[i].specialCardTag].inShop, variant.variantSpecialCards[specialCardVariantOptions[i].specialCardTag].category, variant.variantSpecialCards[specialCardVariantOptions[i].specialCardTag].cost, variant.variantSpecialCards[specialCardVariantOptions[i].specialCardTag].impact);
		}
	}
	
	public void DefaultButtonClicked()
	{
		SetSpecialCardVariantOptionsToVariant(VariantsMenu.instance.baseVariant);
	}
	
	public void CancelButtonClicked()
	{
		SetSpecialCardVariantOptionsToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["SpecialCardsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
	public void ConfirmButtonClicked()
	{
		for(int i = 0; i < specialCardVariantOptions.Count; i++)
		{
			VariantSpecialCard tempVariantSpecialCard = VariantsMenu.instance.loadedVariant.variantSpecialCards[specialCardVariantOptions[i].specialCardTag];
			tempVariantSpecialCard.inShop = specialCardVariantOptions[i].inShopToggle.isOn;
			tempVariantSpecialCard.category = LocalInterface.instance.ConvertIntToRarity(Mathf.RoundToInt(specialCardVariantOptions[i].raritySlider.value));
			if(specialCardVariantOptions[i].costInputField.text.Length <= 0)
			{
				tempVariantSpecialCard.cost = 0;
			}
			else
			{
				tempVariantSpecialCard.cost = int.Parse(specialCardVariantOptions[i].costInputField.text);
			}
			if(specialCardVariantOptions[i].impactInputField.text.Length <= 0)
			{
				tempVariantSpecialCard.impact = 0;
			}
			else
			{
				tempVariantSpecialCard.impact = double.Parse(specialCardVariantOptions[i].impactInputField.text);
			}
			VariantsMenu.instance.loadedVariant.variantSpecialCards[specialCardVariantOptions[i].specialCardTag] = tempVariantSpecialCard;
		}
		MovingObjects.instance.mo["SpecialCardsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
}
