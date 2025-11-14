using UnityEngine;
using UnityEngine.UI;

public class DropZone : MonoBehaviour
{
	public RectTransform rt;
	public Image xImage;
	public RectTransform mushroomMultBackdrop;
	public Label mushroomMultLabel;
	public Image mushroomMultMushroom;
	public ScorePlate scorePlate;
	public int dropZoneNumber;
	public bool specialCardsOnly;
	
    public bool locked;
	public bool cardPlaced;
	public Card placedCard;
	
	public void CardPlaced(Card card)
	{
		cardPlaced = true;
		placedCard = card;
		card.rt.SetSiblingIndex(1);
		xImage.rectTransform.SetSiblingIndex(2);
		if(!specialCardsOnly)
		{
			scorePlate.rt.SetSiblingIndex(3);
		}
	}
	
	public void CardRemoved()
	{
		cardPlaced = false;
		placedCard = null;
		xImage.gameObject.SetActive(false);
	}
	
	public void UpdateMushroomData(bool miniMushroomVisible, int newMult)
	{
		mushroomMultBackdrop.gameObject.SetActive(true);
		if(mushroomMultMushroom != null)
		{
			mushroomMultMushroom.gameObject.SetActive(miniMushroomVisible);
		}
		mushroomMultLabel.ChangeText($"x{newMult}");
		if(miniMushroomVisible)
		{
			mushroomMultLabel.rt.offsetMin = new Vector2(12f, 2f);
			mushroomMultBackdrop.sizeDelta = new Vector2(mushroomMultLabel.GetPreferredWidth() + 14f, mushroomMultBackdrop.sizeDelta.y);
		}
		else
		{
			mushroomMultLabel.rt.offsetMin = new Vector2(2f, 2f);
			mushroomMultBackdrop.sizeDelta = new Vector2(mushroomMultLabel.GetPreferredWidth() + 4f, mushroomMultBackdrop.sizeDelta.y);
		}
	}
}
