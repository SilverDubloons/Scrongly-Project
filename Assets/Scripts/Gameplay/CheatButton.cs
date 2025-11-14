using UnityEngine;
using UnityEngine.UI;
using static Variant;
using static Deck;

public class CheatButton : MonoBehaviour
{
	public RectTransform rt;
    public Image buttonImage;
	public ButtonPlus button;
	
	public string itemType;
	public int quantity;
	public VariantBauble variantBauble;
	public VariantSpecialCard variantSpecialCard;
	
	public void ButtonClicked()
	{
		switch(itemType)
		{
			case "Zodiac":
				Baubles.instance.ZodiacPurchased(variantBauble.tag, Vector2.zero);
			break;
			case "Bauble":
				Baubles.instance.BaublePurchased(variantBauble.tag, Vector2.zero);
			break;
			case "SpecialCard":
				Card newCard = HandArea.instance.SpawnCard(new CardData(-1, -1, variantSpecialCard.tag), HandArea.instance.deckLocation, HandArea.instance.cardParent);
				Deck.instance.cardsInHand.Add(newCard);
				HandArea.instance.ReorganizeHand();
				HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
			break;
			case "Currency":
				GameManager.instance.AddCurrency(quantity);
			break;
		}
		SetInteractability();
	}
	
	public void SetInteractability()
	{
		switch(itemType)
		{
			case "Bauble":
				if(Baubles.instance.GetQuantityOwned(variantBauble.tag, true) >= variantBauble.max && variantBauble.max != 0)
				{
					button.ChangeButtonEnabled(false);
					Shop.instance.RemoveBaubleFromAvailablePool(variantBauble.tag);
				}
			break;
		}
	}
}
