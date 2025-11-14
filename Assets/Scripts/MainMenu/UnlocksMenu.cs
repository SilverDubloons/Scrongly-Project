using UnityEngine;
using static Decks;
using static Variant;
using System.Collections.Generic;

public class UnlocksMenu : MonoBehaviour
{
    public RectTransform decksBackdrop;
    public RectTransform decksContent;
    public RectTransform baublesBackdrop;
    public RectTransform baublesContent;
    public RectTransform specialCardsBackdrop;
    public RectTransform specialCardsContent;
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public GameObject unlockableObjectPrefab;
	public ButtonPlus backButton;
	
	public void SetInteractability(bool enabledState)
	{
		backButton.ChangeButtonEnabled(enabledState);
	}
	
	public void SetupUnlocksMenu()
	{
		int index = 0;
		foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			if(!entry.Value.unlockedByDefault)
			{
				GameObject newUnlockableObjectGO = Instantiate(unlockableObjectPrefab, decksContent);
				UnlockableObject newUnlockableObject = newUnlockableObjectGO.GetComponent<UnlockableObject>();
				newUnlockableObject.image.sprite = entry.Value.cardBack;
	
				newUnlockableObject.tooltipObject.title = entry.Key;
				newUnlockableObject.tooltipObject.titleColor = ThemeManager.UIElementType.DeckName;
				if(entry.Value.unlocked)
				{
					newUnlockableObject.tooltipObject.mainText = entry.Value.description;
					newUnlockableObject.lockedObject.SetActive(false);
				}
				else
				{
					newUnlockableObject.tooltipObject.mainText = entry.Value.howToUnlock;
					newUnlockableObject.tooltipObject.subtitle = "To Unlock:";
					newUnlockableObject.tooltipObject.subtitleColor = ThemeManager.UIElementType.DeckName;
					newUnlockableObject.lockedObject.SetActive(true);
				}
				newUnlockableObject.rt.anchoredPosition = new Vector2(5f + 49f * index, -5f);
				controllerSelectionGroup.controllerSelectableObjects.Add(newUnlockableObject.controllerSelectableObject);
				index++;
			}
		}
		decksContent.sizeDelta = new Vector2(index * 49f + 5f, decksContent.sizeDelta.y);
		decksBackdrop.sizeDelta = new Vector2(Mathf.Min(index * 49f + 15f, 630f), 74);
		if(decksBackdrop.sizeDelta.x <= 630f + LocalInterface.instance.epsilon)
		{
			decksBackdrop.sizeDelta = new Vector2(decksBackdrop.sizeDelta.x, 64);
		}
		index = 0;
		foreach(KeyValuePair<string, VariantBauble> entry in LocalInterface.instance.baseVariant.variantBaubles)
		{
			if(entry.Value.mustBeUnlocked)
			{
				GameObject newUnlockableObjectGO = Instantiate(unlockableObjectPrefab, baublesContent);
				UnlockableObject newUnlockableObject = newUnlockableObjectGO.GetComponent<UnlockableObject>();
				newUnlockableObject.image.sprite = entry.Value.sprite;
				newUnlockableObject.blackWhenLockedController.Initialize();
				newUnlockableObject.lockedObject.SetActive(false);
				if(LocalInterface.instance.unlockedBaubles.Contains(entry.Key))
				{
					newUnlockableObject.tooltipObject.title = LocalInterface.instance.baseVariant.variantBaubles[entry.Key].baubleName;
					newUnlockableObject.tooltipObject.subtitle = entry.Value.category;
					newUnlockableObject.tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(entry.Value.category);
					newUnlockableObject.tooltipObject.mainText = entry.Value.menuDescription;
					newUnlockableObject.blackWhenLockedController.SetLocked(false);
				}
				else
				{
					newUnlockableObject.tooltipObject.title = "???";
					newUnlockableObject.tooltipObject.subtitle = "To Unlock:";
					newUnlockableObject.tooltipObject.subtitleColor = ThemeManager.UIElementType.BaubleName;
					newUnlockableObject.tooltipObject.mainText = entry.Value.howToUnlock;
					newUnlockableObject.blackWhenLockedController.SetLocked(true);
					if(entry.Key != "IncreaseMushroomPowerSames" && entry.Key != "ZodiacsFromFlushes")
					{
						newUnlockableObject.tooltipObject.hasProgressBar = true;
						newUnlockableObject.tooltipObject.progressBarTag = entry.Key;
					}
				}
				newUnlockableObject.tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
				newUnlockableObject.rt.anchoredPosition = new Vector2(5f + 49f * index, -5f);
				controllerSelectionGroup.controllerSelectableObjects.Add(newUnlockableObject.controllerSelectableObject);
				index++;
			}
		}
		baublesContent.sizeDelta = new Vector2(index * 49f + 5f, baublesContent.sizeDelta.y);
		baublesBackdrop.sizeDelta = new Vector2(Mathf.Min(index * 49f + 15f, 630f), 74);
		if(baublesBackdrop.sizeDelta.x <= 630f + LocalInterface.instance.epsilon)
		{
			baublesBackdrop.sizeDelta = new Vector2(baublesBackdrop.sizeDelta.x, 64);
		}
		index = 0;
		foreach(KeyValuePair<string, VariantSpecialCard> entry in LocalInterface.instance.baseVariant.variantSpecialCards)
		{
			if(entry.Value.mustBeUnlocked)
			{
				GameObject newUnlockableObjectGO = Instantiate(unlockableObjectPrefab, specialCardsContent);
				UnlockableObject newUnlockableObject = newUnlockableObjectGO.GetComponent<UnlockableObject>();
				newUnlockableObject.image.sprite = entry.Value.sprite;
				newUnlockableObject.blackWhenLockedController.Initialize();
				newUnlockableObject.lockedObject.SetActive(false);
				if(LocalInterface.instance.unlockedSpecialCards.Contains(entry.Key))
				{
					newUnlockableObject.tooltipObject.title = entry.Value.specialCardName;
					newUnlockableObject.tooltipObject.subtitle = entry.Value.category;
					newUnlockableObject.tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(entry.Value.category);
					newUnlockableObject.tooltipObject.mainText = entry.Value.description;
					newUnlockableObject.blackWhenLockedController.SetLocked(false);
				}
				else
				{
					newUnlockableObject.tooltipObject.title = "???";
					newUnlockableObject.tooltipObject.subtitle = "???";
					newUnlockableObject.tooltipObject.subtitleColor = ThemeManager.UIElementType.CardName;
					newUnlockableObject.tooltipObject.mainText = entry.Value.howToUnlock;
					newUnlockableObject.blackWhenLockedController.SetLocked(true);
					if(entry.Key != "ZodiacContainedHands")
					{
						newUnlockableObject.tooltipObject.hasProgressBar = true;
						newUnlockableObject.tooltipObject.progressBarTag = entry.Key;
					}
				}
				newUnlockableObject.tooltipObject.titleColor = ThemeManager.UIElementType.CardName;
				newUnlockableObject.rt.anchoredPosition = new Vector2(5f + 49f * index, -5f);
				controllerSelectionGroup.controllerSelectableObjects.Add(newUnlockableObject.controllerSelectableObject);
				index++;
			}
		}
		specialCardsContent.sizeDelta = new Vector2(index * 49f + 5f, specialCardsContent.sizeDelta.y);
		specialCardsBackdrop.sizeDelta = new Vector2(Mathf.Min(index * 49f + 15f, 630f), 74);
		if(specialCardsBackdrop.sizeDelta.x <= 630f + LocalInterface.instance.epsilon)
		{
			specialCardsBackdrop.sizeDelta = new Vector2(specialCardsBackdrop.sizeDelta.x, 64);
		}
		/* decksBackdrop.anchoredPosition += new Vector2(0, -60);
		baublesBackdrop.anchoredPosition += new Vector2(0, -60);
		specialCardsBackdrop.gameObject.SetActive(false); */
	}
	
	public void BackClicked()
	{
		MovingObjects.instance.mo["MainMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["Version"].StartMove("OnScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OnScreen");
		MovingObjects.instance.mo["Title"].StartMove("OnScreen");
		MovingObjects.instance.mo["UnlocksMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["ExitButton"].StartMove("OnScreen");
	}
}
