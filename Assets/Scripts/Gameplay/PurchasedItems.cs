using UnityEngine;
using static Deck;

public class PurchasedItems : MonoBehaviour
{
    public RectTransform rt;
	
	public Vector2 baubleDestination;
	public Vector2 zodiacDestination;
	public Vector2 cardDestination;
	
	public GameObject purchasedItemPrefab;
	public GameObject cardPrefab;
	
	public static PurchasedItems instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void BaublePurchased(string baubleTag, Vector2 origin)
	{
		GameObject newPurchasedItemGO = Instantiate(purchasedItemPrefab, rt);
		newPurchasedItemGO.name = $"purchased {baubleTag}";
		PurchasedItem newPurchasedItem = newPurchasedItemGO.GetComponent<PurchasedItem>();
		if(baubleTag == "Dice" && Baubles.instance.GetImpactInt("Dice") > 1)
		{
			switch(Baubles.instance.GetImpactInt("Dice"))
			{
				case 2:
					newPurchasedItem.image.sprite = Baubles.instance.diceSprites[11];
				break;
				case 3:
					newPurchasedItem.image.sprite = Baubles.instance.diceSprites[12];
				break;
				case 4:
					newPurchasedItem.image.sprite = Baubles.instance.diceSprites[13];
				break;
				case 5:
					newPurchasedItem.image.sprite = Baubles.instance.diceSprites[14];
				break;
				default:
					LocalInterface.instance.DisplayError($"PurchasedItems die upgrade failed. Case {Baubles.instance.GetImpactInt("Dice")}");
				break;
			}
		}
		else
		{
			newPurchasedItem.image.sprite = V.i.v.variantBaubles[baubleTag].sprite;
		}
		newPurchasedItem.StartMoving(origin, baubleDestination);
	}
	
	public void ZodiacPurchased(string baubleTag, Vector2 origin, bool isNegative = false)
	{
		GameObject newPurchasedItemGO = Instantiate(purchasedItemPrefab, rt);
		newPurchasedItemGO.name = $"purchased {baubleTag}";
		PurchasedItem newPurchasedItem = newPurchasedItemGO.GetComponent<PurchasedItem>();
		newPurchasedItem.image.sprite = V.i.v.variantBaubles[baubleTag].sprite;
		if(isNegative)
		{
			newPurchasedItem.image.color = LocalInterface.instance.negativeZodiacColor;
		}
		else
		{
			newPurchasedItem.image.color = LocalInterface.instance.rarityDictionary["Zodiac"].rarityColor;
		}
		newPurchasedItem.StartMoving(origin, zodiacDestination);
	}
	
	public void CardPurchased(CardData cardDataToAdd, Vector2 origin)
	{
		GameObject newCardGO = Instantiate(cardPrefab, rt);
		newCardGO.name = "purchased card";
		Card newCard = newCardGO.GetComponent<Card>();
		newCard.cardData = cardDataToAdd;
		newCard.back.sprite = V.i.chosenDeckSprite;
		newCard.rt.anchoredPosition = origin;
		newCard.UpdateGraphics();
		newCard.StartMove(cardDestination, new Vector3(0, 0, 0), false, true, false, true);
		newCard.StartFlip();
	}
}
