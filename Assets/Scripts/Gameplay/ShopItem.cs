using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using static Deck;

public class ShopItem : MonoBehaviour
{
    public RectTransform rt;
	public ButtonPlus costButton;
	public ButtonPlus buyButton;
	public Image itemImage;
	public Label costLabel;
	public TooltipObject tooltipObject;
	public ControllerSelectableObject costButtonControllerSelectableObject;
	public ControllerSelectableObject buyButtonControllerSelectableObject;
	
	public string itemType;
	public string itemTag;
	public int itemCost;
	public Card card;
	public bool purchased = false;
	public bool onLayaway = false;
	public Vector2 shopOriginLocation;
	public IEnumerator moveCoroutine;
	public bool moving = false;
	
	public void CostButtonClicked()
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage < 16)
			{
				MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(costButton.rt, GameManager.instance.gameplayCanvas));
				return;
			}
			else
			{
				if(Tutorial.instance.currentStage == 16)
				{
					Tutorial.instance.IncrementStage();
				}
			}
		}
		if(Shop.instance.layawayItemHasBeenInShopBetweenRounds)
		{
			OptionsDialog.instance.SetupDialog("Putting this item on layaway will delete the old layaway item. Continue?", new string[1]{"Yes"}, new ThemeManager.UIElementType[1]{ThemeManager.UIElementType.standardButtonActive}, new UnityAction[1]{MoveItemToLayawayAndDeleteOldItem});
		}
		else
		{
			MoveItemToLayaway();
		}
	}
	
	public string ConvertToString()
	{
		switch(itemType)
		{
			case "Bauble":
			case "Zodiac":
				return $"{itemType}|{itemTag}|{itemCost}";
			case "Card":
				return $"{itemType}|{card.cardData.ConvertToText()}";
		}
		LocalInterface.instance.DisplayError("Failed to convert ShopItem to string");
		return string.Empty;
	}
	
	public void LoadFromString(string shopItemData)
	{
		string[] shopItemDataLines = shopItemData.Split('|');
		itemType = shopItemDataLines[0];
		switch(itemType)
		{
			case "Bauble":
			case "Zodiac":
				onLayaway = true;
				itemTag = shopItemDataLines[1];
				SetItemCost(int.Parse(shopItemDataLines[2]));
				itemImage.sprite = V.i.v.variantBaubles[itemTag].sprite;
				tooltipObject.title = V.i.v.variantBaubles[itemTag].baubleName;
				if(itemType == "Bauble")
				{
					tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
					Shop.instance.RemoveBaubleFromAvailablePool(itemTag);
					Shop.instance.onSaleBaubles.Add(this);
					tooltipObject.isBauble = true;
				}
				else
				{
					tooltipObject.titleColor = ThemeManager.UIElementType.Zodiac;
					Shop.instance.availableZodiacs.Remove(itemTag);
					Shop.instance.onSaleZodiacs.Add(this);
					tooltipObject.isZodiac = true;
				}
				tooltipObject.subtitle = V.i.v.variantBaubles[itemTag].category;
				tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantBaubles[itemTag].category);
				if(itemTag == "Dice" && Baubles.instance.GetQuantityOwned("Dice", true) > 0)
				{
					int diceAmount = Baubles.instance.GetQuantityOwned("Dice", true);
					itemImage.sprite = Baubles.instance.diceSprites[10 + diceAmount];
					tooltipObject.mainText = Baubles.instance.diceDescriptions[diceAmount - 1];
					switch(diceAmount)
					{
						case 1:
							tooltipObject.title = "D8";
						break;
						case 2:
							tooltipObject.title = "D10";
						break;
						case 3:
							tooltipObject.title = "D12";
						break;
						case 4:
							tooltipObject.title = "D20";
						break;
						default:
							LocalInterface.instance.DisplayError($"Shop die switch failed Case {diceAmount}");
						break;
					}
				}
				else
				{
					itemImage.sprite = V.i.v.variantBaubles[itemTag].sprite;
					var resolver = new DescriptionResolver();
					string input = V.i.v.variantBaubles[itemTag].inGameDescription;
					string output = resolver.Resolve(input);
					tooltipObject.mainText = output;	
				}
			break;
			case "Card":
				onLayaway = true;
				itemImage.gameObject.SetActive(false);
				if(shopItemDataLines.Length <= 2)
				{
					CardData newCardData = new CardData(shopItemDataLines[1]);
					Card newShopCard = HandArea.instance.SpawnCard(newCardData, Vector2.zero, rt, false, false);
					card = newShopCard;
					SetItemCost(V.i.v.variantSpecialCards[newCardData.specialCardName].cost);
				}
				else
				{
					CardData newCardData = new CardData($"{shopItemDataLines[1]}|{shopItemDataLines[2]}|{shopItemDataLines[3]}");
					Card newShopCard = HandArea.instance.SpawnCard(newCardData, Vector2.zero, rt, false, false);
					card = newShopCard;
					int cardCost = V.i.v.variantSpecialOptions["StandardCardBaseCost"].impact;
					if(Baubles.instance.GetImpactInt("AllCardsInShopAreRainbow") == 0 && newCardData.suit == 4)
					{
						cardCost++;
					}
					SetItemCost(cardCost);
				}
				tooltipObject.gameObject.SetActive(false);
				Shop.instance.onSaleCards.Add(this);
			break;
		}
	}
	
	public void DestroyThisShopItem()
	{
		Shop.instance.controllerSelectionGroup.controllerSelectableObjects.Remove(costButtonControllerSelectableObject);
		Shop.instance.controllerSelectionGroup.controllerSelectableObjects.Remove(buyButtonControllerSelectableObject);
		Destroy(this.gameObject);
	}
	
	public void MoveItemToLayawayAndDeleteOldItem()
	{
		Shop.instance.controllerSelectionGroup.controllerSelectableObjects.Remove(Shop.instance.layawayItem.costButtonControllerSelectableObject);
		Shop.instance.controllerSelectionGroup.controllerSelectableObjects.Remove(Shop.instance.layawayItem.buyButtonControllerSelectableObject);
		switch(Shop.instance.layawayItem.itemType)
		{
			case "Bauble":
				Shop.instance.AddBaubleToAvailablePool(Shop.instance.layawayItem.itemTag);
			break;
			case "Card":
				// nothing special
			break;
			case "Zodiac":
				Shop.instance.availableZodiacs.Add(Shop.instance.layawayItem.itemTag);
			break;
		}
		Shop.instance.layawayItem.DestroyThisShopItem();
		Shop.instance.layawayItem = null;
		MoveItemToLayaway();
		OptionsDialog.instance.SetVisibility(false);
	}
	
	public void MoveItemToLayaway()
	{
		if(Shop.instance.layawayItem != null)
		{
			Shop.instance.layawayItem.StartMoveCoroutine(Shop.instance.layawayItem.shopOriginLocation);
			Shop.instance.layawayItem.onLayaway = false;
			Shop.instance.layawayItem.costButton.ChangeButtonEnabled(true);
		}
		shopOriginLocation = rt.anchoredPosition;
		Shop.instance.layawayItemHasBeenInShopBetweenRounds = false;
		Shop.instance.layawayItem = this;
		Shop.instance.layawayLabelObject.SetActive(false);
		onLayaway = true;
		costButton.ChangeButtonEnabled(false);
		StartMoveCoroutine(Shop.instance.layawayLocation);
		if(buyButtonControllerSelectableObject.hasTooltip)
		{
			buyButtonControllerSelectableObject.tooltipObject.DisableTooltip();
		}
	}
	
	public void StartMoveCoroutine(Vector2 destinationLocation)
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCoroutine(destinationLocation);
		StartCoroutine(moveCoroutine);
	}
	
	public IEnumerator MoveCoroutine(Vector2 destinationLocation)
	{
		moving = true;
		Vector2 originLocation = rt.anchoredPosition;
		float t = 0;
		float moveTime = 1f;
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
			rt.anchoredPosition = Vector2.Lerp(originLocation, destinationLocation, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			if(ControllerSelection.instance.usingController)
			{
				if(ControllerSelection.instance.currentlySelectedObject == costButtonControllerSelectableObject)
				{
					ControllerSelection.instance.RepositionControllerSelectionRT(costButtonControllerSelectableObject, Shop.instance.controllerSelectionGroup);
				}
				else if(ControllerSelection.instance.currentlySelectedObject == buyButtonControllerSelectableObject)
				{
					ControllerSelection.instance.RepositionControllerSelectionRT(buyButtonControllerSelectableObject, Shop.instance.controllerSelectionGroup);
				}
			}
			yield return null;
		}
		moving = false;
	}
	
	public void SetItemCost(int newCost)
	{
		itemCost = newCost;
		costLabel.ChangeText(newCost.ToString());
		if(newCost >= 100)
		{
			costLabel.ChangeFontSize(8);
		}
	}
	
	public void BuyButtonClicked()
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(itemTag == "DecreaseCardsNeededForStraight")
			{
				if(itemCost != 0)
				{
					MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(buyButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
				else
				{
					Tutorial.instance.snakesButtonObject.SetActive(false);
					if(Baubles.instance.GetImpactInt("Hand04Power") > 0)
					{
						Tutorial.instance.IncrementStage();
					}
				}
			}
			else if(itemTag == "Hand04Power")
			{
				if(itemCost != 0)
				{
					MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(buyButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
				else
				{
					Tutorial.instance.straightZodiacButtonObject.SetActive(false);
					if(Baubles.instance.GetImpactInt("DecreaseCardsNeededForStraight") > 0)
					{
						Tutorial.instance.IncrementStage();
					}
				}
			}
		}
		Shop.instance.controllerSelectionGroup.controllerSelectableObjects.Remove(costButtonControllerSelectableObject);
		Shop.instance.controllerSelectionGroup.controllerSelectableObjects.Remove(buyButtonControllerSelectableObject);
		if(Shop.instance.layawayItem == this)
		{
			Shop.instance.layawayItemHasBeenInShopBetweenRounds = false;
			Shop.instance.layawayItem = null;
			Shop.instance.layawayLabelObject.SetActive(true);
		}
		SoundManager.instance.PlayItemPurchasedSound();
		switch(itemType)
		{
			case "Bauble":
				Baubles.instance.BaublePurchased(itemTag, rt.anchoredPosition);
				Shop.instance.onSaleBaubles.Remove(this);
				Shop.instance.AddBaubleToAvailablePool(itemTag);
			break;
			case "Card":
				card.rt.SetParent(HandArea.instance.movingCardsParent);
				card.StartMove(HandArea.instance.deckLocation, Vector3.zero, false, true, false, true);
				card.StartFlip();
				RunInformation.instance.CardAddedToDeck(card.cardData);
				Shop.instance.onSaleCards.Remove(this);
			break;
			case "Zodiac":
				Baubles.instance.ZodiacPurchased(itemTag, rt.anchoredPosition);
				Shop.instance.onSaleZodiacs.Remove(this);
				HandsInformation.instance.UpdateAllHandInfos();
				Shop.instance.availableZodiacs.Add(itemTag);
			break;
		}
		purchased = true; // so we don't check this when iterating through shop items after spending money
		GameManager.instance.AddCurrency(-itemCost);
		buyButton.ChangeButtonEnabled(false);
		costButton.ChangeButtonEnabled(false);
		Destroy(this.gameObject);
		if(ControllerSelection.instance.usingController)
		{
			if(buyButtonControllerSelectableObject.hasTooltip)
			{
				buyButtonControllerSelectableObject.tooltipObject.DisableTooltip();
			}
			ControllerSelection.instance.MoveSelectionToObject(ControllerSelection.instance.FindClosestActiveObject(LocalInterface.instance.GetCanvasPositionOfRectTransform(buyButton.rt, GameManager.instance.gameplayCanvas)), Shop.instance.controllerSelectionGroup);
		}
	}
	
	void Start()
	{
		StartCoroutine(ExpandRetract());
		SetInteractability(Shop.instance.shopFinishedOpening);
	}
	
	public IEnumerator ExpandRetract()
	{
		float expandTime = 0.1f;
		Vector3 expandDestination = new Vector3(1.3f, 1.3f, 1);
		float t = 0;
		while(t < expandTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, expandTime);
			rt.localScale = Vector3.Lerp(Vector3.one, expandDestination, t / expandTime);
			yield return null;
		}
		t = 0;
		while(t < expandTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, expandTime);
			rt.localScale = Vector3.Lerp(expandDestination, Vector3.one, t / expandTime);
			yield return null;
		}
	}
	
	public void SetInteractability(bool enabledState)
	{
		if(onLayaway)
		{
			costButton.ChangeButtonEnabled(false);
		}
		else
		{
			costButton.ChangeButtonEnabled(enabledState);
		}
		if(enabledState)
		{
			CurrencyUpdated();
		}
		else
		{
			buyButton.ChangeButtonEnabled(false);
		}
	}
	
	public void CurrencyUpdated()
	{
		if(purchased)
		{
			return;
		}
		if(itemCost > GameManager.instance.currency)
		{
			buyButton.ChangeButtonEnabled(false);
		}
		else
		{
			buyButton.ChangeButtonEnabled(true);
		}
	}
}