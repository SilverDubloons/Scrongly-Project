using UnityEngine;
using UnityEngine.UI;
using static Deck;

public class SpecialCardExplainer : MonoBehaviour
{
    public RectTransform rt;
	public Card card;
	public Image rarityImage;
	public GameObject notInShopIndicator;
	public Label impactLabel;
	public Label costLabel;
	
	public void SetupSpecialCardExplainer(Vector2 location, string tag)
	{
		rt.anchoredPosition = location;
		card.cardData = new CardData(-1, -1, tag);
		card.UpdateGraphics();
		
		if(V.i.v.variantSpecialCards[tag].inShop)
		{
			notInShopIndicator.SetActive(false);
			rarityImage.gameObject.SetActive(true);
			rarityImage.color = LocalInterface.instance.rarityDictionary[V.i.v.variantSpecialCards[tag].category].rarityColor;
			costLabel.gameObject.SetActive(true);
			costLabel.ChangeText($"${LocalInterface.instance.ConvertDoubleToString(V.i.v.variantSpecialCards[tag].cost)}");
		}
		else
		{
			notInShopIndicator.SetActive(true);
			rarityImage.gameObject.SetActive(false);
			costLabel.gameObject.SetActive(false);
		}
		if(V.i.v.variantSpecialCards[tag].impact == 0)
		{
			impactLabel.gameObject.SetActive(false);
		}
		else
		{
			impactLabel.gameObject.SetActive(true);
			impactLabel.ChangeText($"i{LocalInterface.instance.ConvertDoubleToString(V.i.v.variantSpecialCards[tag].impact)}");
		}
		
	}
}
