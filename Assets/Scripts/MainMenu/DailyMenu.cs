using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Linq;
using System;

public class DailyMenu : MonoBehaviour
{
    public Label titleLabel;
	public Image deckCardbackImage;
	public Label deckEffectLabel;
	public GameObject saveVariantBackdrop;
	public VariantSimple variantSimple;
	public ButtonPlus playButton;
	public ButtonPlus backButton;
	public ButtonPlus saveVariantButton;
	
	[TextArea]
	public string[] dailyGameVariants;
	public RandomNumbers dailyRandomNumbers;
	
	public int dailySeed;
	public string dailyDeck;
	public string dailyVariantString;
	public string dailyVariantName;
	
	public static DailyMenu instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		playButton.ChangeButtonEnabled(enabledState);
		backButton.ChangeButtonEnabled(enabledState);
		if(saveVariantBackdrop.activeSelf)
		{
			saveVariantButton.ChangeButtonEnabled(enabledState);	
		}
	}
	
	public void DailyMenuOpened()
	{
		// disable saveVariantBackdrop if variant with same name is in variants folder
		if(LocalInterface.instance.DoesFileExist($"{VariantsMenu.instance.variantsFolderPath}{dailyVariantName}"))
		{
			saveVariantBackdrop.SetActive(false);
		}
		else
		{
			saveVariantBackdrop.SetActive(true);
		}
	}
	
	public void SetupDailyMenu()
	{
		string dateTodayString = DateTime.Now.ToString("d MMMM yyyy");
		titleLabel.ChangeText($"{dateTodayString} Daily Game");
		using (SHA256 sha256 = SHA256.Create())
		{
			byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dateTodayString));
			dailySeed = BitConverter.ToInt32(hashBytes, 0);
			dailySeed = Mathf.Abs(dailySeed) % (int.MaxValue - 1);
		}
		dailyRandomNumbers.ChangeSeed(dailySeed);
		dailyDeck = Decks.instance.decks.ElementAt(dailyRandomNumbers.Range(0, Decks.instance.decks.Count)).Value.deckName;
		deckCardbackImage.sprite = Decks.instance.decks[dailyDeck].cardBack;
		deckEffectLabel.ChangeText(Decks.instance.decks[dailyDeck].description);
		dailyVariantString = dailyGameVariants[dailyRandomNumbers.Range(0, dailyGameVariants.Length)];
		// dailyVariantString = dailyGameVariants[3];
		variantSimple.UpdateVariantSimpleForVariant(new Variant(dailyVariantString));
		dailyVariantName = variantSimple.variant.variantName;
	}
	
	public void PlayClicked()
	{
		MovingObjects.instance.mo["DailyMenu"].StartMove("OffScreen");
		V.i.loadingGame = false;
		V.i.seed = dailySeed;
		V.i.v = new Variant(variantSimple.variant);
		V.i.currentDifficulty = -1;
		V.i.chosenDeck = dailyDeck;
		V.i.chosenDeckSprite = Decks.instance.decks[dailyDeck].cardBack;
		V.i.chosenDeckDescription = Decks.instance.decks[dailyDeck].description;
		V.i.isCustomGame = false;
		V.i.isDailyGame = true;
		V.i.isTutorial = false;
		V.i.dateTimeStarted = DateTime.Now;
		Stats.instance.AdjustStatInt("dailyRunsPlayed", 1);
		TransitionStinger.instance.StartStinger("GameplayScene");
	}
	
	public void BackClicked()
	{
		MovingObjects.instance.mo["DailyMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["Title"].StartMove("OnScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OnScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OnScreen");
	}
	
	public void SaveVariantClicked()
	{
		VariantsMenu.instance.SaveVariantToFile(dailyVariantString, dailyVariantName);
		saveVariantButton.ChangeButtonEnabled(false);
		MinorNotifications.instance.NewMinorNotification("Saved!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(saveVariantButton.rt, LocalInterface.instance.mainMenuCanvas));
	}
}
