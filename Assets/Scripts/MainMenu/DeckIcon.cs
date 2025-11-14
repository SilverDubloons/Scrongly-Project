using UnityEngine;
using UnityEngine.UI;

public class DeckIcon : MonoBehaviour
{
    public RectTransform rt;
	public Image image;
	public RectTransform imageRt;
	public int index;
	
	public void ClickedOn()
	{
		DeckPicker.instance.ChangeColorOfLastSelectedDeckBasedOnUnlockStatus();
		Decks.instance.lastSelectedDeck = Decks.instance.decksOrder[index];
		DeckPicker.instance.SelectedDeckUpdated();
	}
}
