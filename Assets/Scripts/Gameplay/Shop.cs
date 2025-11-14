using UnityEngine;
using static Variant;
using static Deck;
using System.Collections.Generic;
using System.Collections;
using System;

public class Shop : MonoBehaviour
{
	public ButtonPlus nextRoundButton;
	public ButtonPlus rerollButton;
	public RectTransform shopItemParent;
	public RectTransform interestChipParent;
	public Label rerollCostLabel;
	public GameObject layawayLabelObject;
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public GameObject shopItemPrefab;
	
	public Vector2 layawayLocation;
	public Vector2 itemOffset;
	public const float distanceBetweenShopItemsX = 54f;
	public const float distanceBetweenShopItemsY = 104f;
	public const float baseChanceForCardsToBeSpecial = 0.25f;
	
    public List<string> availableCommonBaubles = new List<string>();
    public List<string> availableUncommonBaubles = new List<string>();
    public List<string> availableRareBaubles = new List<string>();
    public List<string> availableLegendaryBaubles = new List<string>();
	public List<string> availableCommonSpecialCards = new List<string>();
    public List<string> availableUnommonSpecialCards = new List<string>();
    public List<string> availableRareSpecialCards = new List<string>();
    public List<string> availableLegendarySpecialCards = new List<string>();
    public List<string> availableZodiacs = new List<string>();
	
	public List<ShopItem> onSaleBaubles = new List<ShopItem>();
	public List<ShopItem> onSaleCards = new List<ShopItem>();
	public List<ShopItem> onSaleZodiacs = new List<ShopItem>();
	public ShopItem layawayItem = null;
	public List<Chip> currentInterestChips = new List<Chip>();
	
	public bool inShop = false;
	public bool shopFinishedOpening = false;
	public bool shopClosing = false;
	public int currentRerollCost;
	public const int offsetThreshold = 7;
	public bool layawayItemHasBeenInShopBetweenRounds;
	
	public static Shop instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void AdjustVariantDependantBaubles()
	{
		{
			VariantBauble tempBauble = V.i.v.variantBaubles["IncreaseItemsForSale"];
			if(V.i.v.variantSpecialOptions["StartingShopSize"].impact < 7)
			{
				tempBauble.max = 7 - V.i.v.variantSpecialOptions["StartingShopSize"].impact;
			}
			else
			{
				tempBauble.availableInStore = false;
			}
			// tempBauble.baseCost = Mathf.Max(V.i.v.variantBaubles["IncreaseItemsForSale"].baseCost + V.i.v.variantBaubles["IncreaseItemsForSale"].costStep * 4 - V.i.v.variantSpecialOptions["StartingShopSize"].impact * V.i.v.variantBaubles["IncreaseItemsForSale"].costStep, 1);
			V.i.v.variantBaubles["IncreaseItemsForSale"] = tempBauble;
		}
		{
			VariantBauble tempBauble = V.i.v.variantBaubles["DecreaseRerollCost"];
			if(V.i.v.variantSpecialOptions["RerollBaseCost"].impact - 1 > 0)
			{
				tempBauble.max = V.i.v.variantSpecialOptions["RerollBaseCost"].impact - 1;
			}
			else
			{
				tempBauble.availableInStore = false;
			}
			V.i.v.variantBaubles["DecreaseRerollCost"] = tempBauble;
		}
		{
			VariantBauble tempBauble = V.i.v.variantBaubles["AllowSpecialCardsInStandardDropZones"];
			if(tempBauble.availableInStore)
			{
				tempBauble.availableInStore = !V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect;
			}
			V.i.v.variantBaubles["AllowSpecialCardsInStandardDropZones"] = tempBauble;
		}
		{
			VariantBauble tempBauble = V.i.v.variantBaubles["IncreaseHandsUntilFatigue"];
			if(V.i.chosenDeck == "Sleepy" && !V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
			{
				tempBauble.availableInStore = false;
			}
			V.i.v.variantBaubles["IncreaseHandsUntilFatigue"] = tempBauble;
		}
		{
			VariantBauble tempBauble = V.i.v.variantBaubles["IncreaseInterestMax"];
			if(V.i.chosenDeck == "Sgambler" && !V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
			{
				tempBauble.availableInStore = false;
			}
			V.i.v.variantBaubles["IncreaseInterestMax"] = tempBauble;
		}
	}
	
	public void PopulateAvailableBaubles()
	{
		availableCommonBaubles.Clear();
		availableUncommonBaubles.Clear();
		availableRareBaubles.Clear();
		availableLegendaryBaubles.Clear();
		foreach(KeyValuePair<string, VariantBauble> entry in V.i.v.variantBaubles)
		{
			if(entry.Value.availableInStore && entry.Value.category != "Zodiac" && (!entry.Value.mustBeUnlocked || LocalInterface.instance.IsBaubleUnlocked(entry.Key)))
			{
				if(System.Text.RegularExpressions.Regex.IsMatch(entry.Key, @"^Hand\d{2}Mult$"))
				{
					int handNumber = int.Parse(entry.Key.Substring(4, 2));
					if(handNumber <= 8 || RunInformation.instance.handsPlayed[handNumber] > 0 || V.i.v.variantSpecialOptions["AllBaublesZodiacsUnlocked"].inEffect)
					{
						
					}
					else
					{
						continue;
					}
				}
				if(layawayItem != null && layawayItem.itemType == "Bauble" && layawayItem.itemTag == entry.Key)
				{
					continue;
				}
				AddBaubleToAvailablePool(entry.Key);
			}
			if(entry.Value.category == "Zodiac")
			{
				int zodiacHandNumber = int.Parse(entry.Key.Substring(4, 2));
				if(zodiacHandNumber <= 8 || RunInformation.instance.handsPlayed[zodiacHandNumber] > 0 || V.i.v.variantSpecialOptions["AllBaublesZodiacsUnlocked"].inEffect)
				{
					if(layawayItem != null && layawayItem.itemType == "Zodiac" && layawayItem.itemTag == entry.Key)
					{
						continue;
					}
					availableZodiacs.Add(entry.Key);
				}
			}
		}
	}
	
	public void AddBaubleToAvailablePool(string variantBaubleTag)
	{
		if(Baubles.instance.GetQuantityOwned(variantBaubleTag, true) < V.i.v.variantBaubles[variantBaubleTag].max || V.i.v.variantBaubles[variantBaubleTag].max == 0)
		{
			switch(V.i.v.variantBaubles[variantBaubleTag].category)
			{
				case "Common":
					availableCommonBaubles.Add(variantBaubleTag);
				break;
				case "Uncommon":
					availableUncommonBaubles.Add(variantBaubleTag);
				break;
				case "Rare":
					availableRareBaubles.Add(variantBaubleTag);
				break;
				case "Legendary":
					availableLegendaryBaubles.Add(variantBaubleTag);
				break;
			}
		}
	}
	
	public void RemoveBaubleFromAvailablePool(string variantBaubleTag)
	{
		switch(V.i.v.variantBaubles[variantBaubleTag].category)
		{
			case "Common":
				if(availableCommonBaubles.Contains(variantBaubleTag))
				{
					availableCommonBaubles.Remove(variantBaubleTag);
				}
			break;
			case "Uncommon":
				if(availableUncommonBaubles.Contains(variantBaubleTag))
				{
					availableUncommonBaubles.Remove(variantBaubleTag);
				}
			break;
			case "Rare":
				if(availableRareBaubles.Contains(variantBaubleTag))
				{
					availableRareBaubles.Remove(variantBaubleTag);
				}
			break;
			case "Legendary":
				if(availableLegendaryBaubles.Contains(variantBaubleTag))
				{
					availableLegendaryBaubles.Remove(variantBaubleTag);
				}
			break;
		}
		for(int i = onSaleBaubles.Count - 1; i >= 0; i--)
		{
			if(onSaleBaubles[i].itemTag == variantBaubleTag)
			{
				ShopItem tempBaubleShopItem = onSaleBaubles[i];
				onSaleBaubles.RemoveAt(i);
				Destroy(tempBaubleShopItem.gameObject);
			}
		}
	}
	
	public void AddZodiacToAvailablePool(string zodiacTag)
	{
		availableZodiacs.Add(zodiacTag);
	}
	
	public int GetNumberOfEachItemOnSale(bool ignoreDisabled)
	{
		return V.i.v.variantSpecialOptions["StartingShopSize"].impact + Baubles.instance.GetImpactInt("IncreaseItemsForSale", false, ignoreDisabled);
		// return 7;
	}
	
	public void PopulateAvailableSpecialCards()
	{
		availableCommonSpecialCards.Clear();
		availableUnommonSpecialCards.Clear();
		availableRareSpecialCards.Clear();
		availableLegendarySpecialCards.Clear();
		foreach(KeyValuePair<string, VariantSpecialCard> entry in V.i.v.variantSpecialCards)
		{
			if(entry.Value.inShop && (!entry.Value.mustBeUnlocked || LocalInterface.instance.IsSpecialCardUnlocked(entry.Key)))
			{
				switch(entry.Value.category)
				{
					case "Common":
						availableCommonSpecialCards.Add(entry.Key);
					break;
					case "Uncommon":
						availableUnommonSpecialCards.Add(entry.Key);
					break;
					case "Rare":
						availableRareSpecialCards.Add(entry.Key);
					break;
					case "Legendary":
						availableLegendarySpecialCards.Add(entry.Key);
					break;
				}
			}
		}
	}
	
	public void SetInteractability(bool enabledState)
	{
		nextRoundButton.ChangeButtonEnabled(enabledState);
		SetRerollButtonInteractability(enabledState);
		shopFinishedOpening = enabledState;
		for(int i = 0; i < onSaleBaubles.Count; i++)
		{
			onSaleBaubles[i].SetInteractability(enabledState);
		}
		for(int i = 0; i < onSaleCards.Count; i++)
		{
			onSaleCards[i].SetInteractability(enabledState);
		}
		for(int i = 0; i < onSaleZodiacs.Count; i++)
		{
			onSaleZodiacs[i].SetInteractability(enabledState);
		}
		if(layawayItem != null)
		{
			layawayItem.SetInteractability(enabledState);
		}
	}
	
	public void CurrencyUpdated()
	{
		if(!inShop || shopClosing)
		{
			return;
		}
		UpdateInterest();
		for(int i = 0; i < onSaleBaubles.Count; i++)
		{
			onSaleBaubles[i].CurrencyUpdated();
		}
		for(int i = 0; i < onSaleCards.Count; i++)
		{
			onSaleCards[i].CurrencyUpdated();
		}
		for(int i = 0; i < onSaleZodiacs.Count; i++)
		{
			onSaleZodiacs[i].CurrencyUpdated();
		}
		SetRerollButtonInteractability(true);
		layawayItem?.CurrencyUpdated();
	}
	
	public int GetMaxInterest()
	{
		return V.i.v.variantSpecialOptions["BaseMaxInterest"].impact + Baubles.instance.GetImpactInt("IncreaseInterestMax");
	}
	
	public void UpdateInterest()
	{
		if(V.i.chosenDeck == "Sgambler" && !V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
		{
			return;
		}
		int currentInterestShouldBe = Mathf.Min(GameManager.instance.currency / V.i.v.variantSpecialOptions["ChipToInterestRatio"].impact, GetMaxInterest());
		if(currentInterestChips.Count == currentInterestShouldBe)
		{
			return;
		}
		if(currentInterestChips.Count > currentInterestShouldBe)
		{
			for(int i = currentInterestChips.Count - 1; i >= currentInterestShouldBe; i--)
			{
				// Destroy(currentInterestChips[i].gameObject);
				currentInterestChips[i].rt.SetParent(GameManager.instance.spareChipsParent);
				currentInterestChips[i].gameObject.SetActive(false);
				currentInterestChips.RemoveAt(i);
			}
		}
		else if(currentInterestChips.Count < currentInterestShouldBe)
		{
			int chipsToMake = currentInterestShouldBe - currentInterestChips.Count;
			for(int i = 0; i < chipsToMake; i++)
			{
				if(GameManager.instance.spareChipsParent.childCount > 0)
				{
					Chip spareChip = GameManager.instance.spareChipsParent.GetChild(GameManager.instance.spareChipsParent.childCount - 1).GetComponent<Chip>();
					spareChip.gameObject.SetActive(true);
					spareChip.rt.SetParent(interestChipParent);
					currentInterestChips.Add(spareChip);
				}
				else
				{
					GameObject newChipGO = Instantiate(GameManager.instance.chipPrefab, interestChipParent);
					Chip newChip = newChipGO.GetComponent<Chip>();
					currentInterestChips.Add(newChip);
				}
			}
		}
		float maxHeight = 120f;
		float chipHeight = 16f;
		float squeezeDistance = (maxHeight - chipHeight) / (currentInterestChips.Count - 1);
		float distanceBetweenItems = 4f;
		distanceBetweenItems = Mathf.Min((chipHeight + distanceBetweenItems), squeezeDistance);
		for(int i = 0; i < currentInterestChips.Count; i++)
		{
			currentInterestChips[i].rt.anchoredPosition = new Vector2(0, (currentInterestChips.Count - 1) * (distanceBetweenItems / 2) - (currentInterestChips.Count - i - 1) * distanceBetweenItems);
		}
	}
	
	public void SetRerollButtonInteractability(bool enabledState)
	{
		if(enabledState == false)
		{
			rerollButton.ChangeButtonEnabled(false);
		}
		else
		{
			if(GameManager.instance.currency >= currentRerollCost)
			{
				rerollButton.ChangeButtonEnabled(true);
			}
			else
			{
				rerollButton.ChangeButtonEnabled(false);
			}
		}	
	}
	
	public void RerollClicked()
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage < 16)
			{
				MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(rerollButton.rt, GameManager.instance.gameplayCanvas));
					return;
			}
		}
		SoundManager.instance.PlayShopRerollSound();
		GenerateShopItems();
		GameManager.instance.AddCurrency(-currentRerollCost);
		ChangeCurrentRerollCost(V.i.v.variantSpecialOptions["ShopRerollScaling"].impact);
		if(layawayItem == null)
		{
			layawayItemHasBeenInShopBetweenRounds = false;
		}
		else
		{
			layawayItemHasBeenInShopBetweenRounds = true;
		}
		SetRerollButtonInteractability(true);
	}
	
	public void ShuffleAndContinueClicked()
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage != 18 && Tutorial.instance.currentStage != 24)
			{
				MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(nextRoundButton.rt, GameManager.instance.gameplayCanvas));
					return;
			}
		}
		ControllerSelection.instance.currentControllerSelectionGroups.Add(HandArea.instance.cardsControllerSelectionGroup);
		inShop = false;
		shopClosing = true;
		MovingObjects.instance.mo["PlayArea"].StartMove("OnScreen");
		MovingObjects.instance.mo["HandArea"].StartMove("OnScreen");
		MovingObjects.instance.mo["CardParent"].StartMove("OnScreen");
		MovingObjects.instance.mo["BossInformation"].StartMove("OnScreen");
		MovingObjects.instance.mo["Shop"].StartMove("OffScreen");
		HandArea.instance.StartDrawCards(0.5f / Preferences.instance.gameSpeed, true);
		if(Baubles.instance.GetImpactInt("AddZodiacSpecialCardsEachRound") > 0)
		{
			Baubles.instance.Notify("AddZodiacSpecialCardsEachRound");
			for(int i = 0; i < Baubles.instance.GetImpactInt("AddZodiacSpecialCardsEachRound"); i++)
			{
				HandArea.instance.AddSpecialCardToDeck("EarnZodiac", Vector2.zero + new Vector2(50f, 0) * i);
			}
		}
		if(layawayItem == null)
		{
			layawayItemHasBeenInShopBetweenRounds = false;
		}
		else
		{
			layawayItemHasBeenInShopBetweenRounds = true;
		}
		PlayArea.instance.HandlePreplacedCards();
		if(!Decks.instance.decks["Swiney"].unlocked && !V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(currentInterestChips.Count >= 10)
			{
				Decks.instance.UnlockDeck("Swiney");
			}
		}
		StartCoroutine(CollectInterestCoroutine());
		/* for(int i = 0; i < currentInterestChips.Count; i++)
		{
			currentInterestChips[i].StartMove();
		}
		currentInterestChips.Clear(); */
	}
	
	public IEnumerator CollectInterestCoroutine()
	{
		while(currentInterestChips.Count > 0)
		{
			currentInterestChips[currentInterestChips.Count - 1].StartMove();
			currentInterestChips.RemoveAt(currentInterestChips.Count - 1);
			yield return null;
		}
		currentInterestChips.Clear();
	}
	
	public void OpenShop(bool movePlayObjectsOffScreen = true)
	{
		CardValuesTooltip.instance.HideTooltip();
		if(V.i.isTutorial)
		{
			if(!Tutorial.instance.tutorialFinished)
			{
				Tutorial.instance.IncrementStage();
			}
			if(Tutorial.instance.displayingTips)
			{
				Tutorial.instance.MoveToShopSide();
			}
		}
		currentRerollCost = GetCurrentRerollBaseCost();
		ChangeCurrentRerollCost(0);
		GenerateShopItems();
		UpdateInterest();
		inShop = true;
		shopClosing = false;
		if(movePlayObjectsOffScreen)
		{
			MovingObjects.instance.mo["PlayArea"].StartMove("OffScreen");
			MovingObjects.instance.mo["HandArea"].StartMove("OffScreen");
			MovingObjects.instance.mo["BossInformation"].StartMove("OffScreen");
		}
		ControllerSelection.instance.currentControllerSelectionGroups.Remove(HandArea.instance.cardsControllerSelectionGroup);
		MovingObjects.instance.mo["Shop"].StartMove("OnScreen");
	}
	
		
	public int GetCurrentRerollBaseCost()
	{
		return V.i.v.variantSpecialOptions["RerollBaseCost"].impact - Baubles.instance.GetImpactInt("DecreaseRerollCost");
	}
	
	public void ChangeCurrentRerollCost(int changeToMake)
	{
		currentRerollCost += changeToMake;
		rerollCostLabel.ChangeText(currentRerollCost.ToString());
		rerollCostLabel.StartExpandRetract();
	}
	
	public void GenerateShopItems()
	{
		List<ShopItem> oldOnSaleBaubles = new List<ShopItem>(onSaleBaubles);
		List<ShopItem> oldOnSaleCards = new List<ShopItem>(onSaleCards);
		List<ShopItem> oldOnSaleZodiacs = new List<ShopItem>(onSaleZodiacs);
		for(int i = 0; i < onSaleBaubles.Count; i++)
		{
			if(!onSaleBaubles[i].onLayaway)
			{
				switch(V.i.v.variantBaubles[onSaleBaubles[i].itemTag].category)
				{
					case "Common":
						availableCommonBaubles.Add(onSaleBaubles[i].itemTag);
					break;
					case "Uncommon":
						availableUncommonBaubles.Add(onSaleBaubles[i].itemTag);
					break;
					case "Rare":
						availableRareBaubles.Add(onSaleBaubles[i].itemTag);
					break;
					case "Legendary":
						availableLegendaryBaubles.Add(onSaleBaubles[i].itemTag);
					break;
				}
				onSaleBaubles[i].gameObject.SetActive(false);
			}
		}
		for(int i = 0; i < onSaleCards.Count; i++)
		{
			if(!onSaleCards[i].onLayaway)
			{
				onSaleCards[i].gameObject.SetActive(false);
			}
		}
		for(int i = 0; i < oldOnSaleZodiacs.Count; i++)
		{
			if(!oldOnSaleZodiacs[i].onLayaway)
			{
				availableZodiacs.Add(oldOnSaleZodiacs[i].itemTag);
				oldOnSaleZodiacs[i].gameObject.SetActive(false);
			}
		}
		onSaleBaubles.Clear();
		onSaleCards.Clear();
		onSaleZodiacs.Clear();
		if(layawayItem != null)
		{
			if(oldOnSaleBaubles.Contains(layawayItem))
			{
				oldOnSaleBaubles.Remove(layawayItem);
			}
			else if(oldOnSaleCards.Contains(layawayItem))
			{
				oldOnSaleCards.Remove(layawayItem);
			}
			else if(oldOnSaleZodiacs.Contains(layawayItem))
			{
				oldOnSaleZodiacs.Remove(layawayItem);
			}
		}
		StartCoroutine(DestroyOldShopItems(oldOnSaleBaubles, oldOnSaleCards, oldOnSaleZodiacs));
		StartCoroutine(GenerateShopItemsCoroutine());
	}
	
	public IEnumerator DestroyOldShopItems(List<ShopItem> oldOnSaleBaubles, List<ShopItem> oldOnSaleCards, List<ShopItem> oldOnSaleZodiacs)
	{
		for(int i = 0; i < oldOnSaleBaubles.Count; i++)
		{
			controllerSelectionGroup.controllerSelectableObjects.Remove(oldOnSaleBaubles[i].costButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Remove(oldOnSaleBaubles[i].buyButtonControllerSelectableObject);
			Destroy(oldOnSaleBaubles[i].gameObject);
			yield return null;
		}
		for(int i = 0; i < oldOnSaleCards.Count; i++)
		{
			controllerSelectionGroup.controllerSelectableObjects.Remove(oldOnSaleCards[i].costButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Remove(oldOnSaleCards[i].buyButtonControllerSelectableObject);
			Destroy(oldOnSaleCards[i].gameObject);
			yield return null;
		}
		for(int i = 0; i < oldOnSaleZodiacs.Count; i++)
		{
			controllerSelectionGroup.controllerSelectableObjects.Remove(oldOnSaleZodiacs[i].costButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Remove(oldOnSaleZodiacs[i].buyButtonControllerSelectableObject);
			Destroy(oldOnSaleZodiacs[i].gameObject);
			yield return null;
		}
	}
	
	public IEnumerator GenerateShopItemsCoroutine()
	{
		int numberOfItemsOnSale = GetNumberOfEachItemOnSale(true);
		for(int i = 0; i < numberOfItemsOnSale; i++)
		{
			ShopItem newBaubleShopItem = CreateNewBauble();
			newBaubleShopItem.rt.anchoredPosition = new Vector2(-distanceBetweenShopItemsX / 2 * (numberOfItemsOnSale - 1) + distanceBetweenShopItemsX * i + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.x : 0), distanceBetweenShopItemsY + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.y : 0));
			controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleShopItem.costButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleShopItem.buyButtonControllerSelectableObject);
			yield return null;
			ShopItem newCardShopItem = CreateNewCard();
			newCardShopItem.rt.anchoredPosition = new Vector2(-distanceBetweenShopItemsX / 2 * (numberOfItemsOnSale - 1) + distanceBetweenShopItemsX * i + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.x : 0), (numberOfItemsOnSale >= offsetThreshold ? itemOffset.y : 0));
			if(!newCardShopItem.card.cardData.isSpecialCard)
			{
				newCardShopItem.costButtonControllerSelectableObject.hasTooltip = false;
				newCardShopItem.buyButtonControllerSelectableObject.hasTooltip = false;
			}
			else
			{
				newCardShopItem.costButtonControllerSelectableObject.tooltipObject = newCardShopItem.card.tooltipObject;
				newCardShopItem.buyButtonControllerSelectableObject.tooltipObject = newCardShopItem.card.tooltipObject;
			}
			controllerSelectionGroup.controllerSelectableObjects.Add(newCardShopItem.costButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Add(newCardShopItem.buyButtonControllerSelectableObject);
			yield return null;
			if(!V.i.v.variantSpecialOptions["ZodiacsUnpurchasable"].inEffect)
			{
				ShopItem newZodiacShopItem = CreateNewZodiac();
				newZodiacShopItem.rt.anchoredPosition = new Vector2(-distanceBetweenShopItemsX / 2 * (numberOfItemsOnSale - 1) + distanceBetweenShopItemsX * i + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.x : 0), -distanceBetweenShopItemsY + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.y : 0));
				controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacShopItem.costButtonControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacShopItem.buyButtonControllerSelectableObject);
				yield return null;
			}
		}
	}
	
	public void LoadLayawayItem(string layawayItemString)
	{
		GameObject newShopItemGO = Instantiate(shopItemPrefab, shopItemParent);
		ShopItem newShopItem = newShopItemGO.GetComponent<ShopItem>();
		controllerSelectionGroup.controllerSelectableObjects.Add(newShopItem.costButtonControllerSelectableObject);
		controllerSelectionGroup.controllerSelectableObjects.Add(newShopItem.buyButtonControllerSelectableObject);
		newShopItem.LoadFromString(layawayItemString);
		newShopItem.rt.anchoredPosition = layawayLocation;
		layawayItem = newShopItem;
		layawayLabelObject.SetActive(false);
	}
	
	public ShopItem CreateNewBauble()
	{
		GameObject newBaubleShopItemGO = Instantiate(shopItemPrefab, shopItemParent);
		ShopItem newBaubleShopItem = newBaubleShopItemGO.GetComponent<ShopItem>();
		newBaubleShopItem.itemType = "Bauble";
		int commonBaubleWeight = V.i.v.variantSpecialOptions["CommonBaubleWeight"].impact;
		if(availableCommonBaubles.Count <= 0)
		{
			commonBaubleWeight = 0;
		}
		int uncommonBaubleWeight = V.i.v.variantSpecialOptions["UncommonBaubleWeight"].impact;
		if(availableUncommonBaubles.Count <= 0)
		{
			uncommonBaubleWeight = 0;
		}
		int rareBaubleWeight = V.i.v.variantSpecialOptions["RareBaubleWeight"].impact;
		if(availableRareBaubles.Count <= 0)
		{
			rareBaubleWeight = 0;
		}
		int legendaryBaubleWeight = V.i.v.variantSpecialOptions["LegendaryBaubleWeight"].impact;
		if(availableLegendaryBaubles.Count <= 0)
		{
			legendaryBaubleWeight = 0;
		}
		int r = RNG.instance.shop.Range(0, commonBaubleWeight + uncommonBaubleWeight + rareBaubleWeight + legendaryBaubleWeight);
		if(r < commonBaubleWeight)
		{
			int rb = RNG.instance.shop.Range(0, availableCommonBaubles.Count);
			newBaubleShopItem.itemTag = availableCommonBaubles[rb];
			availableCommonBaubles.RemoveAt(rb);
		}
		else if(r < commonBaubleWeight + uncommonBaubleWeight)
		{
			int rb = RNG.instance.shop.Range(0, availableUncommonBaubles.Count);
			newBaubleShopItem.itemTag = availableUncommonBaubles[rb];
			availableUncommonBaubles.RemoveAt(rb);
		}
		else if(r < commonBaubleWeight + uncommonBaubleWeight + rareBaubleWeight)
		{
			int rb = RNG.instance.shop.Range(0, availableRareBaubles.Count);
			newBaubleShopItem.itemTag = availableRareBaubles[rb];
			availableRareBaubles.RemoveAt(rb);
		}
		else
		{
			int rb = RNG.instance.shop.Range(0, availableLegendaryBaubles.Count);
			newBaubleShopItem.itemTag = availableLegendaryBaubles[rb];
			availableLegendaryBaubles.RemoveAt(rb);
		}
/* 		if(V.i.isTutorial && GameManager.instance.currentRound == 0 && newBaubleShopItem.itemTag == "DecreaseCardsNeededForStraight")
		{
			newBaubleShopItem.SetItemCost(0);
		} */
		newBaubleShopItem.SetItemCost(V.i.v.variantBaubles[newBaubleShopItem.itemTag].baseCost + V.i.v.variantBaubles[newBaubleShopItem.itemTag].costStep * Baubles.instance.GetQuantityOwned(newBaubleShopItem.itemTag, true));
		newBaubleShopItem.tooltipObject.title = V.i.v.variantBaubles[newBaubleShopItem.itemTag].baubleName;
		newBaubleShopItem.tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
		newBaubleShopItem.tooltipObject.subtitle = V.i.v.variantBaubles[newBaubleShopItem.itemTag].category;
		newBaubleShopItem.tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantBaubles[newBaubleShopItem.itemTag].category);
		newBaubleShopItem.tooltipObject.isBauble = true;
		if(newBaubleShopItem.itemTag == "Dice" && Baubles.instance.GetQuantityOwned("Dice", true) > 0)
		{
			int diceAmount = Baubles.instance.GetQuantityOwned("Dice", true);
			newBaubleShopItem.itemImage.sprite = Baubles.instance.diceSprites[10 + diceAmount];
			newBaubleShopItem.tooltipObject.mainText = Baubles.instance.diceDescriptions[diceAmount - 1];
			switch(diceAmount)
			{
				case 1:
					newBaubleShopItem.tooltipObject.title = "D8";
				break;
				case 2:
					newBaubleShopItem.tooltipObject.title = "D10";
				break;
				case 3:
					newBaubleShopItem.tooltipObject.title = "D12";
				break;
				case 4:
					newBaubleShopItem.tooltipObject.title = "D20";
				break;
				default:
					LocalInterface.instance.DisplayError($"Shop die switch failed Case {diceAmount}");
				break;
			}
		}
		else
		{
			newBaubleShopItem.itemImage.sprite = V.i.v.variantBaubles[newBaubleShopItem.itemTag].sprite;
			var resolver = new DescriptionResolver();
			string input = "";
			int quantityOwned = Baubles.instance.GetQuantityOwned(newBaubleShopItem.itemTag);
			if(quantityOwned > 0 && V.i.v.variantBaubles[newBaubleShopItem.itemTag].extraDescriptions.Length > 0)
			{
				input = V.i.v.variantBaubles[newBaubleShopItem.itemTag].extraDescriptions[Mathf.Min(quantityOwned - 1, V.i.v.variantBaubles[newBaubleShopItem.itemTag].extraDescriptions.Length - 1)];
			}
			else
			{
				input = V.i.v.variantBaubles[newBaubleShopItem.itemTag].inGameDescription;
			}
			string output = resolver.Resolve(input);
			newBaubleShopItem.tooltipObject.mainText = output;	
		}
		onSaleBaubles.Add(newBaubleShopItem);
		return newBaubleShopItem;
	}
	
	public ShopItem CreateNewCard()
	{
		GameObject newCardShopItemGO = Instantiate(shopItemPrefab, shopItemParent);
		ShopItem newCardShopItem = newCardShopItemGO.GetComponent<ShopItem>();
		newCardShopItem.itemType = "Card";
		newCardShopItem.itemImage.gameObject.SetActive(false);
		/* GameObject newShopCardGO = Instantiate(HandArea.instance.cardPrefab, newCardShopItem.rt);
		Card newShopCard = newShopCardGO.GetComponent<Card>();
		newShopCard.rt.anchoredPosition = Vector2.zero; */
		CardData newCardData;
		bool newCardIsSpecial = RNG.instance.shop.Range(0, 1f) < baseChanceForCardsToBeSpecial + Baubles.instance.GetImpactFloat("IncreaseLikelihoodOfSpecialCardsInShop") / 100f;
		if(newCardIsSpecial)
		{
			int commonSpecialCardWeight = V.i.v.variantSpecialOptions["CommonSpecialCardWeight"].impact;
			if(availableCommonSpecialCards.Count <= 0)
			{
				commonSpecialCardWeight = 0;
			}
			int uncommonSpecialCardWeight = V.i.v.variantSpecialOptions["UncommonSpecialCardWeight"].impact;
			if(availableUnommonSpecialCards.Count <= 0)
			{
				uncommonSpecialCardWeight = 0;
			}
			int rareSpecialCardWeight = V.i.v.variantSpecialOptions["RareSpecialCardWeight"].impact;
			if(availableRareSpecialCards.Count <= 0)
			{
				rareSpecialCardWeight = 0;
			}
			int legendarySpecialCardWeight = V.i.v.variantSpecialOptions["LegendarySpecialCardWeight"].impact;
			if(availableLegendarySpecialCards.Count <= 0)
			{
				legendarySpecialCardWeight = 0;
			}
			int r = RNG.instance.shop.Range(0, commonSpecialCardWeight + uncommonSpecialCardWeight + rareSpecialCardWeight + legendarySpecialCardWeight);
			if(r < commonSpecialCardWeight)
			{
				int rb = RNG.instance.shop.Range(0, availableCommonSpecialCards.Count);
				newCardShopItem.itemTag = availableCommonSpecialCards[rb];
				// availableCommonSpecialCards.RemoveAt(rb);
			}
			else if(r < commonSpecialCardWeight + uncommonSpecialCardWeight)
			{
				int rb = RNG.instance.shop.Range(0, availableUnommonSpecialCards.Count);
				newCardShopItem.itemTag = availableUnommonSpecialCards[rb];
				// availableUnommonSpecialCards.RemoveAt(rb);
			}
			else if(r < commonSpecialCardWeight + uncommonSpecialCardWeight + rareSpecialCardWeight)
			{
				int rb = RNG.instance.shop.Range(0, availableRareSpecialCards.Count);
				newCardShopItem.itemTag = availableRareSpecialCards[rb];
				// availableRareSpecialCards.RemoveAt(rb);
			}
			else
			{
				int rb = RNG.instance.shop.Range(0, availableLegendarySpecialCards.Count);
				newCardShopItem.itemTag = availableLegendarySpecialCards[rb];
				// availableLegendarySpecialCards.RemoveAt(rb);
			}
			newCardShopItem.SetItemCost(V.i.v.variantSpecialCards[newCardShopItem.itemTag].cost);
			// newShopCard.cardData = new CardData(-1, -1, newCardShopItem.itemTag);
			newCardData = new CardData(-1, -1, newCardShopItem.itemTag);
		}
		else
		{
			int newCardRank = RNG.instance.shop.Range(0, 13);
			int newCardSuit = -1;
			if(Baubles.instance.GetImpactInt("AllCardsInShopAreRainbow", false, true) > 0)
			{
				newCardShopItem.SetItemCost(V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact);
				newCardSuit = 4;
			}
			else
			{
				newCardSuit = RNG.instance.shop.Range(0, 5);
				newCardShopItem.SetItemCost(V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact + (newCardSuit == 4 ? 1 : 0));
			}
			// newShopCard.cardData = new CardData(newCardRank, newCardSuit);
			newCardData = new CardData(newCardRank, newCardSuit);
		}
		// public Card SpawnCard(CardData cardData, Vector2 spawnLocation, Transform parent, bool spawnFaceDown = true, bool startFlipping = true)
		Card newShopCard = HandArea.instance.SpawnCard(newCardData, Vector2.zero, newCardShopItem.rt, false, false);
		// newShopCard.rt.anchoredPosition = Vector2.zero;
		// newShopCard.UpdateGraphics();
		newCardShopItem.card = newShopCard;
		newCardShopItem.tooltipObject.gameObject.SetActive(false);
		onSaleCards.Add(newCardShopItem);
		return newCardShopItem;
	}
	
	public ShopItem CreateNewZodiac()
	{
		GameObject newZodiacShopItemGO = Instantiate(shopItemPrefab, shopItemParent);
		ShopItem newZodiacShopItem = newZodiacShopItemGO.GetComponent<ShopItem>();
		newZodiacShopItem.itemType = "Zodiac";
		
		int rz = RNG.instance.shop.Range(0, availableZodiacs.Count);
		newZodiacShopItem.itemTag = availableZodiacs[rz];
		newZodiacShopItem.itemImage.sprite = V.i.v.variantBaubles[newZodiacShopItem.itemTag].sprite;
		newZodiacShopItem.itemImage.color = LocalInterface.instance.rarityDictionary["Zodiac"].rarityColor;
		availableZodiacs.RemoveAt(rz);
		newZodiacShopItem.SetItemCost(V.i.v.variantBaubles[newZodiacShopItem.itemTag].baseCost);
		newZodiacShopItem.tooltipObject.title = V.i.v.variantBaubles[newZodiacShopItem.itemTag].baubleName;
		newZodiacShopItem.tooltipObject.titleColor = ThemeManager.UIElementType.Zodiac;
		newZodiacShopItem.tooltipObject.subtitle = V.i.v.variantBaubles[newZodiacShopItem.itemTag].category;
		newZodiacShopItem.tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantBaubles[newZodiacShopItem.itemTag].category);
		newZodiacShopItem.tooltipObject.isZodiac = true;
		var resolver = new DescriptionResolver();
		string input = V.i.v.variantBaubles[newZodiacShopItem.itemTag].inGameDescription;
		string output = resolver.Resolve(input);
		output = String.Format(output, LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[newZodiacShopItem.itemTag].impact1), LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[newZodiacShopItem.itemTag].impact2), LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetHandPoints(LocalInterface.instance.GetHandTierFromZodiacTag(newZodiacShopItem.itemTag))), LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetHandMult(LocalInterface.instance.GetHandTierFromZodiacTag(newZodiacShopItem.itemTag))));
		newZodiacShopItem.tooltipObject.mainText = output;
		onSaleZodiacs.Add(newZodiacShopItem);
		return newZodiacShopItem;
	}
	
	public void AddItemToEachCategory()
	{
		int numberOfItemsOnSale = GetNumberOfEachItemOnSale(false);
		for(int i = 0; i < onSaleBaubles.Count; i++)
		{
			if(layawayItem == onSaleBaubles[i])
			{
				layawayItem.shopOriginLocation = new Vector2(layawayItem.shopOriginLocation.x - distanceBetweenShopItemsX / 2f, layawayItem.shopOriginLocation.y);
				if(numberOfItemsOnSale == offsetThreshold)
				{
					layawayItem.shopOriginLocation += itemOffset;
				}
				continue;
			}
			onSaleBaubles[i].rt.anchoredPosition = new Vector2(onSaleBaubles[i].rt.anchoredPosition.x - distanceBetweenShopItemsX / 2f, onSaleBaubles[i].rt.anchoredPosition.y);
			if(numberOfItemsOnSale == offsetThreshold)
			{
				onSaleBaubles[i].rt.anchoredPosition = onSaleBaubles[i].rt.anchoredPosition + itemOffset;
			}
		}
		for(int i = 0; i < onSaleCards.Count; i++)
		{
			if(layawayItem == onSaleCards[i])
			{
				layawayItem.shopOriginLocation = new Vector2(layawayItem.shopOriginLocation.x - distanceBetweenShopItemsX / 2f, layawayItem.shopOriginLocation.y);
				if(numberOfItemsOnSale == offsetThreshold)
				{
					layawayItem.shopOriginLocation += itemOffset;
				}
				continue;
			}
			onSaleCards[i].rt.anchoredPosition = new Vector2(onSaleCards[i].rt.anchoredPosition.x - distanceBetweenShopItemsX / 2f, onSaleCards[i].rt.anchoredPosition.y);
			if(numberOfItemsOnSale == offsetThreshold)
			{
				onSaleCards[i].rt.anchoredPosition = onSaleCards[i].rt.anchoredPosition + itemOffset;
			}
		}
		for(int i = 0; i < onSaleZodiacs.Count; i++)
		{
			if(layawayItem == onSaleZodiacs[i])
			{
				layawayItem.shopOriginLocation = new Vector2(layawayItem.shopOriginLocation.x - distanceBetweenShopItemsX / 2f, layawayItem.shopOriginLocation.y);
				if(numberOfItemsOnSale == offsetThreshold)
				{
					layawayItem.shopOriginLocation += itemOffset;
				}
				continue;
			}
			onSaleZodiacs[i].rt.anchoredPosition = new Vector2(onSaleZodiacs[i].rt.anchoredPosition.x - distanceBetweenShopItemsX / 2f, onSaleZodiacs[i].rt.anchoredPosition.y);
			if(numberOfItemsOnSale == offsetThreshold)
			{
				onSaleZodiacs[i].rt.anchoredPosition = onSaleZodiacs[i].rt.anchoredPosition + itemOffset;
			}
		}
		ShopItem newBaubleShopItem = CreateNewBauble();
		newBaubleShopItem.rt.anchoredPosition = new Vector2(-distanceBetweenShopItemsX / 2 * (numberOfItemsOnSale - 1) + distanceBetweenShopItemsX * (numberOfItemsOnSale - 1) + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.x : 0), distanceBetweenShopItemsY + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.y : 0));
		controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleShopItem.costButtonControllerSelectableObject);
		controllerSelectionGroup.controllerSelectableObjects.Add(newBaubleShopItem.buyButtonControllerSelectableObject);
		ShopItem newCardShopItem = CreateNewCard();
		newCardShopItem.rt.anchoredPosition = new Vector2(-distanceBetweenShopItemsX / 2 * (numberOfItemsOnSale - 1) + distanceBetweenShopItemsX * (numberOfItemsOnSale - 1) + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.x : 0), (numberOfItemsOnSale >= offsetThreshold ? itemOffset.y : 0));
		controllerSelectionGroup.controllerSelectableObjects.Add(newCardShopItem.costButtonControllerSelectableObject);
		controllerSelectionGroup.controllerSelectableObjects.Add(newCardShopItem.buyButtonControllerSelectableObject);
		if(!newCardShopItem.card.cardData.isSpecialCard)
		{
			newCardShopItem.costButtonControllerSelectableObject.hasTooltip = false;
			newCardShopItem.buyButtonControllerSelectableObject.hasTooltip = false;
		}
		else
		{
			newCardShopItem.costButtonControllerSelectableObject.tooltipObject = newCardShopItem.card.tooltipObject;
			newCardShopItem.buyButtonControllerSelectableObject.tooltipObject = newCardShopItem.card.tooltipObject;
		}
		if(!V.i.v.variantSpecialOptions["ZodiacsUnpurchasable"].inEffect)
		{
			ShopItem newZodiacShopItem = CreateNewZodiac();
			newZodiacShopItem.rt.anchoredPosition = new Vector2(-distanceBetweenShopItemsX / 2 * (numberOfItemsOnSale - 1) + distanceBetweenShopItemsX * (numberOfItemsOnSale - 1) + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.x : 0), -distanceBetweenShopItemsY + (numberOfItemsOnSale >= offsetThreshold ? itemOffset.y : 0));
			controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacShopItem.costButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacShopItem.buyButtonControllerSelectableObject);
		}
	}
	
	public void SwitchAllCardsToRainbow()
	{
		for(int i = 0; i < onSaleCards.Count; i++)
		{
			if(!onSaleCards[i].card.cardData.isSpecialCard)
			{
				onSaleCards[i].card.ChangeSuit(4);
				onSaleCards[i].SetItemCost(V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact);
			}
		}
		if(layawayItem != null && layawayItem.itemType == "Card")
		{
			if(!layawayItem.card.cardData.isSpecialCard)
			{
				layawayItem.card.ChangeSuit(4);
				layawayItem.SetItemCost(V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact);
			}
		}
	}
	
	public void HandleDisabledBaubles()
	{
		if(Baubles.instance.disabledBaubles.Contains("IncreaseItemsForSale"))
		{
			for(int i = GetNumberOfEachItemOnSale(true) - 1; i >= V.i.v.variantSpecialOptions["StartingShopSize"].impact; i--)
			{
				ShopItem tempBaubleShopItem = onSaleBaubles[i];
				AddBaubleToAvailablePool(onSaleBaubles[i].itemTag);
				onSaleBaubles.RemoveAt(i);
				Destroy(tempBaubleShopItem.gameObject);
				ShopItem tempCardShopItem = onSaleCards[i];
				onSaleCards.RemoveAt(i);
				Destroy(tempCardShopItem.gameObject);
				ShopItem tempZodiacShopItem = onSaleZodiacs[i];
				AddZodiacToAvailablePool(onSaleZodiacs[i].itemTag);
				onSaleZodiacs.RemoveAt(i);
				Destroy(tempZodiacShopItem.gameObject);
			}
		}
		if(Baubles.instance.disabledBaubles.Contains("AllCardsInShopAreRainbow"))
		{
			for(int i = 0; i < onSaleCards.Count; i++)
			{
				if(!onSaleCards[i].card.cardData.isSpecialCard && onSaleCards[i].card.cardData.suit == 4)
				{
					onSaleCards[i].card.ChangeSuit(RNG.instance.misc.Range(0, 4));
				}
			}
		}
		if(Baubles.instance.disabledBaubles.Contains("IncreaseLikelihoodOfSpecialCardsInShop"))
		{
			for(int i = 0; i < onSaleCards.Count; i++)
			{
				if(onSaleCards[i].card.cardData.isSpecialCard)
				{
					float chanceToReturnToStandard = 0.25f;
					float r = RNG.instance.misc.Range(0, 1f);
					if(r < chanceToReturnToStandard)
					{
						onSaleCards[i].card.cardData.isSpecialCard = false;
						onSaleCards[i].card.cardData.specialCardName = "";
						onSaleCards[i].card.cardData.rank = RNG.instance.misc.Range(0, 13);
						if(!Baubles.instance.disabledBaubles.Contains("AllCardsInShopAreRainbow") && Baubles.instance.GetImpactInt("AllCardsInShopAreRainbow") > 0)
						{
							onSaleCards[i].card.cardData.suit = 4;
							onSaleCards[i].SetItemCost(V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact);
						}
						else
						{
							onSaleCards[i].card.cardData.suit = RNG.instance.misc.Range(0, 5);
							onSaleCards[i].SetItemCost(V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact + (onSaleCards[i].card.cardData.suit == 4 ? 1 : 0));
						}

						onSaleCards[i].card.cardData.baseValue = Deck.GetBaseValueByRank(onSaleCards[i].card.cardData.rank);
	
					}
				}
			}
		}
	}
	
	public void SetItemInCategoryToCost(string tag, string category, int cost)
	{
		switch(category)
		{
			case "Bauble":
				for(int i = 0; i < onSaleBaubles.Count; i++)
				{
					if(onSaleBaubles[i].itemTag == tag)
					{
						onSaleBaubles[i].SetItemCost(cost);
						onSaleBaubles[i].costLabel.StartExpandRetract();
						return;
					}
				}
			break;
			case "Zodiac":
				for(int i = 0; i < onSaleZodiacs.Count; i++)
				{
					if(onSaleZodiacs[i].itemTag == tag)
					{
						onSaleZodiacs[i].SetItemCost(cost);
						onSaleZodiacs[i].costLabel.StartExpandRetract();
						return;
					}
				}
			break;
		}
		LocalInterface.instance.DisplayError($"SetItemInCategoryToCost failed, tag={tag}, category={category}, cost={cost}");
	}
}
