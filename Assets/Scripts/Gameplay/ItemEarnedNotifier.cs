using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static MovingObject;

public class ItemEarnedNotifier : MonoBehaviour
{
    public RectTransform rt;
	public Image backdropImage; // different colors for unlocks and in game earned
	public Label titleLabel;
	public Label descriptionLabel;
	public Image itemImage;
	public ButtonPlus confirmButton;
	public Label confirmButtonLabel;
	public MovingObject movingObject;
	public TooltipObject tooltipObject;
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public string[] buttonPhrases;
	public Color earnedItemColor;
	public Color unlockedItemColor;
	
	public string itemType;
	public string itemTag;
	
	public void SetupItemEarned(string title, string description, string type, string tag, Sprite itemSprite = null)
	{
		controllerSelectionGroup.canvas = GameManager.instance.gameplayCanvas;
		movingObject.SetupLocationsDictionary();
		titleLabel.ChangeText(title);
		descriptionLabel.ChangeText(description);
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, 120f + descriptionLabel.GetPreferredHeight());
		if(itemSprite != null)
		{
			itemImage.sprite = itemSprite;
		}
		itemType = type;
		itemTag = tag;
		switch(itemType)
		{
			case "EarnedBauble":
				backdropImage.color = earnedItemColor;
				movingObject.TeleportTo("OffScreenItemEarned");
				movingObject.StartMove("OnScreenItemEarned");
			break;
			case "UnlockedBauble":
			case "UnlockedDeck":
			case "UnlockedSpecialCard":
				backdropImage.color = unlockedItemColor;
				// movingObject.DisplayLocationDictionary();
				movingObject.TeleportTo("OffScreenItemUnlocked");
				movingObject.StartMove("OnScreenItemUnlocked");	
			break;
		}
		switch(itemType)
		{
			case "EarnedBauble":
			case "UnlockedBauble":
				tooltipObject.title = V.i.v.variantBaubles[itemTag].baubleName;
				tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
				tooltipObject.subtitle = V.i.v.variantBaubles[itemTag].category;
				tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantBaubles[itemTag].category);
				var resolver = new DescriptionResolver();
				string input = V.i.v.variantBaubles[itemTag].inGameDescription;
				string output = resolver.Resolve(input);
				tooltipObject.mainText = output;
			break;
			case "UnlockedDeck":
				tooltipObject.title = itemTag;
				tooltipObject.titleColor = ThemeManager.UIElementType.DeckName;
				tooltipObject.subtitle = string.Empty;
				tooltipObject.mainText = Decks.instance.decks[itemTag].description;
			break;
			case "UnlockedSpecialCard":
				tooltipObject.title = V.i.v.variantSpecialCards[itemTag].specialCardName;
				tooltipObject.titleColor = ThemeManager.UIElementType.CardName;
				tooltipObject.subtitle = V.i.v.variantSpecialCards[itemTag].category;
				tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantSpecialCards[itemTag].category);
				var resolver2 = new DescriptionResolver();
				string input2 = V.i.v.variantSpecialCards[itemTag].description;
				string output2 = resolver2.Resolve(input2);
				tooltipObject.mainText = output2;
			break;
		}
		confirmButtonLabel.ChangeText(buttonPhrases[UnityEngine.Random.Range(0, buttonPhrases.Length)]);
	}
	
	public void ConfirmButtonClicked()
	{
		switch(itemType)
		{
			case "EarnedBauble":
				movingObject.StartMove("OffScreenItemEarned");
			break;
			case "UnlockedBauble":
			case "UnlockedDeck":
			case "UnlockedSpecialCard":
				movingObject.StartMove("OffScreenItemUnlocked");
			break;
		}
		SetInteractability(false);
	}
	
	public void SetInteractability(bool enabledState)
	{
		confirmButton.ChangeButtonEnabled(enabledState);
		if(enabledState)
		{
			controllerSelectionGroup.AddToCurrentGroups();
		}
		else
		{
			controllerSelectionGroup.RemoveFromCurrentGroups();
		}
	}
	
	public void DestroyItemEarnedNotifier()
	{
		Destroy(this.gameObject);
	}
}
