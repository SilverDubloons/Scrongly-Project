using UnityEngine;
using System.Collections.Generic;
using static Variant;

public class BaubleVariantsMenu : MonoBehaviour
{
	public ButtonPlus defaultButton;
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
	public ControllerSelectionGroup controllerSelectionGroup;
	public UnityEngine.UI.Scrollbar verticalScrollbar;
	
    public Transform baubleVariantOptionsParent;
	public RectTransform baubleVariantOptionsContentRectTransform;
	public GameObject baubleVariantOptionsPrefab;
	
	public int reasonableMaximumForBaubles;
	public int reasonableBaubleCostMaximum;
	public int baublesVariantOptionsWide;
	public float distanceBetweenBaubleVariantOptions;
	public Vector2 baubleVariantOptionsSize;
	
	// public bool baubleHasChanged;
	List<BaubleVariantOptions> baubleVariantOptions = new List<BaubleVariantOptions>();
	// Dictionary<string, BaubleVariantOptions> baubleVariantOptionsByTag = new Dictionary<string, BaubleVariantOptions>();
	
	public static BaubleVariantsMenu instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetBaubleVariantsMenuButtons(bool enabledState)
	{
		/* if(baubleHasChanged)
		{	
			
		}
		else
		{
			confirmButton.ChangeButtonEnabled(false);
		} */
		for(int i = 0; i < baubleVariantOptions.Count; i++)
		{
			baubleVariantOptions[i].SetBaubleVariantOptionsButtons(enabledState);
		}
		confirmButton.ChangeButtonEnabled(enabledState);
		defaultButton.ChangeButtonEnabled(enabledState);
		cancelButton.ChangeButtonEnabled(enabledState);	
	}
	
	public void SetupBaubleVariantMenu(Variant variant)
	{
		 // for sorting later, by category 
		foreach(KeyValuePair<string, VariantBauble> entry in variant.variantBaubles)
		{
			if((entry.Value.category == "Common" || entry.Value.category == "Uncommon" || entry.Value.category == "Rare" || entry.Value.category == "Legendary") && entry.Value.availableInStore && (!entry.Value.mustBeUnlocked || LocalInterface.instance.unlockedBaubles.Contains(entry.Key)))
			{
				GameObject newBaubleVariantOptionsGO = Instantiate(baubleVariantOptionsPrefab,  baubleVariantOptionsParent);
				newBaubleVariantOptionsGO.name = entry.Value.baubleName;
				BaubleVariantOptions newBaubleVariantOptions = newBaubleVariantOptionsGO.GetComponent<BaubleVariantOptions>();
				baubleVariantOptions.Add(newBaubleVariantOptions);
				// baubleVariantOptionsByTag.Add(Entry.Key, newBaubleVariantOptions);
				int maxBaubles = reasonableMaximumForBaubles;
				if(entry.Value.max != 0)
				{
					maxBaubles = entry.Value.max;
				}
				newBaubleVariantOptions.SetupBaubleVariantOptions(true, entry.Value.sprite, entry.Value.category, entry.Value.baseCost, entry.Value.costStep, entry.Value.startingQuantity, maxBaubles, entry.Value.tag, entry.Value.baubleName, entry.Value.menuDescription);
				controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleVariantOptions.inShopToggleControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleVariantOptions.baubleRaritySliderControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleVariantOptions.baseCostInputFieldControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleVariantOptions.costStepInputFieldControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleVariantOptions.startingQuantitySliderControllerSelectableObject);
				switch(entry.Key)
				{
					case "IncreaseItemsForSale":
						newBaubleVariantOptions.startingQuantitySlider.gameObject.SetActive(false);
					break;
				}
			}
		}
		baubleVariantOptions.Sort((x, y) =>
		{
			int rarityComparison = x.rarityInt - y.rarityInt;
			if(rarityComparison != 0)
			{
				return rarityComparison;
			}
			int baseCostComparison = x.baseCost - y.baseCost;
			if(baseCostComparison != 0)
			{
				return baseCostComparison;
			}
			int costStepComparison = x.costStep - y.costStep;
			return costStepComparison;
		});
		
		for(int i = 0; i < baubleVariantOptions.Count; i++)
		{
			baubleVariantOptions[i].rt.anchoredPosition = new Vector2(distanceBetweenBaubleVariantOptions + (i % baublesVariantOptionsWide) * (distanceBetweenBaubleVariantOptions + baubleVariantOptionsSize.x), -distanceBetweenBaubleVariantOptions - (i / baublesVariantOptionsWide) * (distanceBetweenBaubleVariantOptions + baubleVariantOptionsSize.y));
		}
		baubleVariantOptionsContentRectTransform.sizeDelta = new Vector2(baubleVariantOptionsContentRectTransform.sizeDelta.x, distanceBetweenBaubleVariantOptions + ((baubleVariantOptions.Count + 2) / baublesVariantOptionsWide) * (baubleVariantOptionsSize.y + distanceBetweenBaubleVariantOptions));
	}
	
	public void SetBaubleVariantOptionsToVariant(Variant variant)
	{
		for(int i = 0; i < baubleVariantOptions.Count; i++)
		{
			baubleVariantOptions[i].setupComplete = false;
			baubleVariantOptions[i].rarityInt = LocalInterface.instance.rarityDictionary[variant.variantBaubles[baubleVariantOptions[i].baubleTag].category].rarityInt;
			baubleVariantOptions[i].rarity = variant.variantBaubles[baubleVariantOptions[i].baubleTag].category;
			baubleVariantOptions[i].baseCost = variant.variantBaubles[baubleVariantOptions[i].baubleTag].baseCost;
			baubleVariantOptions[i].costStep = variant.variantBaubles[baubleVariantOptions[i].baubleTag].costStep;
			baubleVariantOptions[i].inShop = variant.variantBaubles[baubleVariantOptions[i].baubleTag].availableInStore;
			baubleVariantOptions[i].startingQuantity = variant.variantBaubles[baubleVariantOptions[i].baubleTag].startingQuantity;
			baubleVariantOptions[i].UpdateLabels();
			baubleVariantOptions[i].UpdateInput();
			baubleVariantOptions[i].setupComplete = true;
		}
	}
	
/* 	public void BaubleHasChanged()
	{
		if(baubleHasChanged)
		{
			return;
		}
		baubleHasChanged = true;
		confirmButton.ChangeButtonEnabled(true);
	} */
	
/* 	public void ResetBaublesChangedStatus()
	{
		for(int i = 0; i < baubleVariantOptions.Count; i++)
		{
			baubleVariantOptions[i].hasChanged = false;
		}
	} */
	
	public void DefaultButtonClicked()
	{
		SetBaubleVariantOptionsToVariant(VariantsMenu.instance.baseVariant);
	}
	
	public void CancelButtonClicked()
	{
		SetBaubleVariantOptionsToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["BaubleVariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
	public void ConfirmButtonClicked()
	{
		for(int i = 0; i < baubleVariantOptions.Count; i++)
		{
/* 			if(baubleVariantOptions[i].hasChanged)
			{ */
				VariantBauble tempVariantBauble = VariantsMenu.instance.loadedVariant.variantBaubles[baubleVariantOptions[i].baubleTag];
				tempVariantBauble.category = baubleVariantOptions[i].rarity;
				tempVariantBauble.baseCost = baubleVariantOptions[i].baseCost;
				tempVariantBauble.costStep = baubleVariantOptions[i].costStep;
				tempVariantBauble.availableInStore = baubleVariantOptions[i].inShop;
				tempVariantBauble.startingQuantity = baubleVariantOptions[i].startingQuantity;
				VariantsMenu.instance.loadedVariant.variantBaubles[baubleVariantOptions[i].baubleTag] = tempVariantBauble;
				// VariantsMenu.instance.selectedVariantHasChanged = true;
			// }
		}
		MovingObjects.instance.mo["BaubleVariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
}
