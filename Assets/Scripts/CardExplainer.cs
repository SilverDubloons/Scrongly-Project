using UnityEngine;
using static Deck;

public class CardExplainer : MonoBehaviour
{
    public RectTransform rt;
	public Card card;
	public Label quantityLabel;
	
	public void SetupCardExplainer(Vector2 location, CardData cardData, int quantity)
	{
		rt.anchoredPosition = location;
		card.cardData = cardData;
		card.UpdateGraphics();
		quantityLabel.ChangeText(quantity.ToString());
	}
}
