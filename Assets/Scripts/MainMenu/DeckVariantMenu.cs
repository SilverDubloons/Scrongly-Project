using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Variant;
using TMPro;
using System;

public class DeckVariantMenu : MonoBehaviour
{
    public ButtonPlus defaultButton;
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
	public TMP_InputField numberOfRandomStandardCardsToAddToDeckInputField;
	public Toggle includeRainbowInRandomCardsToggle;
	public TMP_InputField numberOfRandomSpecialCardsToAddToDeckInputField;
	public Toggle considerRarityToggle;
	public RectTransform cardParent;
	public RectTransform specialOptionsDeckContentRectTransform;
	public ControllerSelectionGroup controllerSelectionGroup;
	public Scrollbar verticalScrollbar;
	
	public GameObject deckVariantCardPrefab;
	public Sprite[] rankSprites;
	public Sprite[] suitSprites;
	public Sprite[] cardDetails;

	public int maximumIndividualCardQuantity;
	public int maximumRandomCardQuantity;
	public int cardsWide;
	public float distanceBetweenCards;
	public Vector2 cardSize;
	
	// public bool deckHasChanged;
	public bool setupComplete;
	
	public List<DeckVariantCard> deckVariantStandardCards = new List<DeckVariantCard>();
	public Dictionary<string, DeckVariantCard> deckVariantSpecialCards = new Dictionary<string, DeckVariantCard>();
	
	public static DeckVariantMenu instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetDeckVariantMenuButtons(bool enabledState)
	{
		numberOfRandomStandardCardsToAddToDeckInputField.interactable = enabledState;
		includeRainbowInRandomCardsToggle.interactable = enabledState;
		numberOfRandomSpecialCardsToAddToDeckInputField.interactable = enabledState;
		considerRarityToggle.interactable = enabledState;
		confirmButton.ChangeButtonEnabled(enabledState);
		defaultButton.ChangeButtonEnabled(enabledState);
		cancelButton.ChangeButtonEnabled(enabledState);
		for(int i = 0; i < deckVariantStandardCards.Count; i++)
		{
			deckVariantStandardCards[i].SetDeckVariantCardButtons(enabledState);
		}
		foreach(KeyValuePair<string, DeckVariantCard> entry in deckVariantSpecialCards)
		{
			entry.Value.SetDeckVariantCardButtons(enabledState);
		}
		setupComplete = enabledState;
	}
	
	public void SetupDeckVariantMenu(Variant variant)
	{
		for(int i = 0; i < 65; i++)
		{
			GameObject newVariantCardGO = Instantiate(deckVariantCardPrefab, cardParent);
			DeckVariantCard newVariantCard = newVariantCardGO.GetComponent<DeckVariantCard>();
			newVariantCard.SetupDeckVariantCard(0);
			deckVariantStandardCards.Add(newVariantCard);
			int rank = i % 13;
			int suit = i / 13;
			newVariantCardGO.name = LocalInterface.instance.ConvertRankAndSuitToString(rank, suit);
			if(suit < 4)
			{
				newVariantCard.rankImage.sprite = rankSprites[rank];
				newVariantCard.rankImage.color = LocalInterface.instance.suitColors[suit];
				newVariantCard.suitImage.color = LocalInterface.instance.suitColors[suit];
				if(rank <= 8 || rank == 12)
				{
					newVariantCard.detailImage.color = LocalInterface.instance.suitColors[suit];
				}
			}
			else
			{
				newVariantCard.rankImage.sprite = rankSprites[rank + 13];
			}
			newVariantCard.rankImageRT.sizeDelta = new Vector2(rankSprites[rank].rect.width, rankSprites[rank].rect.height);
			newVariantCard.suitImage.sprite = suitSprites[suit];
			newVariantCard.suitImageRT.sizeDelta = new Vector2(suitSprites[suit].rect.width, suitSprites[suit].rect.height);
			newVariantCard.detailImage.sprite = cardDetails[i];
			newVariantCard.detailImageRT.sizeDelta = new Vector2(cardDetails[i].rect.width, cardDetails[i].rect.height);
			newVariantCard.rt.anchoredPosition = new Vector2(distanceBetweenCards + (distanceBetweenCards + cardSize.x) * (i % 13), -distanceBetweenCards - (distanceBetweenCards + cardSize.y) * (i / 13));
			controllerSelectionGroup.controllerSelectableObjects.Add(newVariantCard.minusButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Add(newVariantCard.plusButtonControllerSelectableObject);
		}
		int specialCardIndex = 0;
		foreach(KeyValuePair<string, VariantSpecialCard> entry in variant.variantSpecialCards)
		{
			if(!entry.Value.mustBeUnlocked || entry.Value.mustBeUnlocked && LocalInterface.instance.IsSpecialCardUnlocked(entry.Key))
			{
				GameObject newVariantCardGO = Instantiate(deckVariantCardPrefab, cardParent);
				newVariantCardGO.name = entry.Value.tag;
				DeckVariantCard newVariantCard = newVariantCardGO.GetComponent<DeckVariantCard>();
				
				newVariantCard.SetupDeckVariantCard(0, entry.Value.tag);
				newVariantCard.rankImage.gameObject.SetActive(false);
				newVariantCard.suitImage.gameObject.SetActive(false);
				newVariantCard.detailImage.sprite = entry.Value.sprite;
				newVariantCard.detailImageRT.sizeDelta = new Vector2(entry.Value.sprite.rect.width, entry.Value.sprite.rect.height);
				newVariantCard.detailImageRT.anchoredPosition = Vector2.zero;
				deckVariantSpecialCards.Add(entry.Value.tag, newVariantCard);
				newVariantCard.rt.anchoredPosition = new Vector2(distanceBetweenCards + (distanceBetweenCards + cardSize.x) * (specialCardIndex % 13), (-distanceBetweenCards * 6) - (cardSize.y * 5) - ((distanceBetweenCards + cardSize.y) * (specialCardIndex / 13)));
				controllerSelectionGroup.controllerSelectableObjects.Add(newVariantCard.minusButtonControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newVariantCard.plusButtonControllerSelectableObject);
				specialCardIndex++;
			}
		}
		SetCardQuantitiesByStartingDeck(variant);
		if(variant.numberOfRandomStandardCardsToAddToDeck <= 0)
		{
			numberOfRandomStandardCardsToAddToDeckInputField.text = string.Empty;
		}
		else
		{
			numberOfRandomStandardCardsToAddToDeckInputField.text = variant.numberOfRandomStandardCardsToAddToDeck.ToString();
		}
		includeRainbowInRandomCardsToggle.isOn = variant.includeRainbowInRandomCards;
		if(variant.numberOfRandomSpecialCardsToAddToDeck <= 0)
		{
			numberOfRandomSpecialCardsToAddToDeckInputField.text = string.Empty;
		}
		else
		{
			numberOfRandomSpecialCardsToAddToDeckInputField.text = variant.numberOfRandomSpecialCardsToAddToDeck.ToString();
		}
		considerRarityToggle.isOn = variant.considerRarity;
	}
	
	public void SetAllCardQuantitiesToZero()
	{
		for(int i = 0; i < 65; i++)
		{
			deckVariantStandardCards[i].UpdateQuantity(0);
		}
		foreach(KeyValuePair<string, DeckVariantCard> entry in deckVariantSpecialCards)
		{
			entry.Value.UpdateQuantity(0);
		}
	}
	
	public void SetCardQuantitiesByStartingDeck(Variant variant)
	{
		SetAllCardQuantitiesToZero();
		for(int i = 0; i < variant.startingDeck.Count; i++)
		{
			if(variant.startingDeck[i].isSpecial)
			{
				deckVariantSpecialCards[variant.startingDeck[i].specialCardTag].PlusClicked();
			}
			else
			{
				int cardNumber = variant.startingDeck[i].rank + variant.startingDeck[i].suit * 13;
				deckVariantStandardCards[cardNumber].PlusClicked();
			}
		}
	}
	
/* 	public void DeckHasChanged()
	{
		if(deckHasChanged)
		{
			return;
		}
		deckHasChanged = true;
		confirmButton.ChangeButtonEnabled(true);
	} */
	
/* 	public void ResetCardsHaveChangedStatus()
	{
		for(int i = 0; i < 65; i++)
		{
			deckVariantStandardCards[i].hasChanged = false;
		}
		foreach(KeyValuePair<string, DeckVariantCard> entry in deckVariantSpecialCards)
		{
			entry.Value.hasChanged = false;
		}
	} */
	
/* 	public void SetCardsSetupCompleteStatus(bool isComplete)
	{
		setupComplete = isComplete;
		for(int i = 0; i < 65; i++)
		{
			deckVariantStandardCards[i].setupComplete = isComplete;
		}
		foreach(KeyValuePair<string, DeckVariantCard> entry in deckVariantSpecialCards)
		{
			entry.Value.setupComplete = isComplete;
		}
	} */
	
	public void SetDeckVariantMenuToVariant(Variant variant)
	{
		SetCardQuantitiesByStartingDeck(variant);
		if(variant.numberOfRandomStandardCardsToAddToDeck <= 0)
		{
			numberOfRandomStandardCardsToAddToDeckInputField.text = string.Empty;
		}
		else
		{
			numberOfRandomStandardCardsToAddToDeckInputField.text = variant.numberOfRandomStandardCardsToAddToDeck.ToString();
		}
		includeRainbowInRandomCardsToggle.isOn = variant.includeRainbowInRandomCards;
		if(variant.numberOfRandomSpecialCardsToAddToDeck <= 0)
		{
			numberOfRandomSpecialCardsToAddToDeckInputField.text = string.Empty;
		}
		else
		{
			numberOfRandomSpecialCardsToAddToDeckInputField.text = variant.numberOfRandomSpecialCardsToAddToDeck.ToString();
		}
		considerRarityToggle.isOn = variant.considerRarity;
	}
	
	public void DefaultButtonClicked()
	{
		SetDeckVariantMenuToVariant(VariantsMenu.instance.baseVariant);
	}
	
	public void CancelButtonClicked()
	{
		SetDeckVariantMenuToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["DeckVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
	public void ConfirmButtonClicked()
	{
		VariantsMenu.instance.loadedVariant.startingDeck.Clear();
		for(int i = 0; i < 65; i++)
		{
			for(int j = 0; j < deckVariantStandardCards[i].quantity; j++)
			{
				VariantsMenu.instance.loadedVariant.startingDeck.Add(new VariantCard(i % 13, i / 13));
			}
		}
		foreach(KeyValuePair<string, DeckVariantCard> entry in deckVariantSpecialCards)
		{
			for(int j = 0; j < entry.Value.quantity; j++)
			{
				VariantsMenu.instance.loadedVariant.startingDeck.Add(new VariantCard(0, 0, true, entry.Value.specialCardTag));
			}
		}
		if(numberOfRandomStandardCardsToAddToDeckInputField.text.Length <= 0)
		{
			VariantsMenu.instance.loadedVariant.numberOfRandomStandardCardsToAddToDeck = 0;
		}
		else
		{
			VariantsMenu.instance.loadedVariant.numberOfRandomStandardCardsToAddToDeck = int.Parse(numberOfRandomStandardCardsToAddToDeckInputField.text);
		}
		VariantsMenu.instance.loadedVariant.includeRainbowInRandomCards = includeRainbowInRandomCardsToggle.isOn;
		if(numberOfRandomSpecialCardsToAddToDeckInputField.text.Length <= 0)
		{
			VariantsMenu.instance.loadedVariant.numberOfRandomSpecialCardsToAddToDeck = 0;
		}
		else
		{
			VariantsMenu.instance.loadedVariant.numberOfRandomSpecialCardsToAddToDeck = int.Parse(numberOfRandomSpecialCardsToAddToDeckInputField.text);
		}
		VariantsMenu.instance.loadedVariant.considerRarity = considerRarityToggle.isOn;
		MovingObjects.instance.mo["DeckVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
/* 	public void ToggleUpdated()
	{
		DeckHasChanged();
	} */
	
	public void InputFieldsFinished()
	{
		if(!setupComplete)
		{
			return;
		}
		/* if(numberOfRandomStandardCardsToAddToDeckInputField.text == string.Empty)
		{
			numberOfRandomStandardCardsToAddToDeckInputField.text = "0";
		}
		if(numberOfRandomSpecialCardsToAddToDeckInputField.text == string.Empty)
		{
			numberOfRandomSpecialCardsToAddToDeckInputField.text = "0";
		} */
		// DeckHasChanged();
	}
	
	public void InputFieldsUpdated()
	{
		if(!setupComplete)
		{
			return;
		}
		if(numberOfRandomStandardCardsToAddToDeckInputField.text.Length > 0)
		{
			int rsc = -1;
			try
			{
				rsc = int.Parse(numberOfRandomStandardCardsToAddToDeckInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"InvalidCharacter in {name} DeckVariantMenu numberOfRandomStandardCardsToAddToDeckInputField: {exception.Message}");
			}
			if(rsc < 0)
			{
				numberOfRandomStandardCardsToAddToDeckInputField.text = "0";
			}
			else if(rsc > maximumRandomCardQuantity)
			{
				numberOfRandomStandardCardsToAddToDeckInputField.text = maximumRandomCardQuantity.ToString();
			}
		}
		if(numberOfRandomSpecialCardsToAddToDeckInputField.text.Length > 0)
		{
			int rsc = -1;
			try
			{
				rsc = int.Parse(numberOfRandomSpecialCardsToAddToDeckInputField.text);
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"InvalidCharacter in {name} DeckVariantMenu numberOfRandomSpecialCardsToAddToDeckInputField: {exception.Message}");
			}
			if(rsc < 0)
			{
				numberOfRandomSpecialCardsToAddToDeckInputField.text = "0";
			}
			else if(rsc > maximumRandomCardQuantity)
			{
				numberOfRandomSpecialCardsToAddToDeckInputField.text = maximumRandomCardQuantity.ToString();
			}
		}
	}
}
