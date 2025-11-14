using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class MainMenu : MonoBehaviour
{
	public string version;
	public bool steamBuild;
	public Label versionLabel;
	public RectTransform exitButtonObject;
	
	public ButtonPlus playButton;
	public ButtonPlus statsButton;
	public ButtonPlus unlocksButton;
	public ButtonPlus optionsButton;
	
	public ButtonPlus newGameButton;
	// public ButtonPlus loadGameButton;
	public ButtonPlus dailyGameButton;
	public ButtonPlus customGameButton;
	public ButtonPlus playMenuBackButton;
	public ButtonPlus exitButton;
	
	public ButtonPlus discordButton;
	public ButtonPlus blueSkyButton;
	public ButtonPlus koFiButton;
	
	public static MainMenu instance;
	
	public void Start()
	{
		versionLabel.ChangeText($"Version\n{version}");
		TransitionStinger.instance.sceneLoaded = true;
		StartCoroutine(MainMenuLoadedCoroutine());
		#if UNITY_WEBGL && !UNITY_EDITOR
		//#if UNITY_EDITOR
			exitButtonObject.gameObject.SetActive(false);
		#endif
		if(steamBuild)
		{
			koFiButton.gameObject.SetActive(false);
			discordButton.rt.anchoredPosition = new Vector2(-39, -23);
			discordButton.rt.sizeDelta = new Vector2(74, 22);
			blueSkyButton.rt.anchoredPosition = new Vector2(39, -23);
			blueSkyButton.rt.sizeDelta = new Vector2(74, 22);
		}
	}
	
	public void SetupInstance()
	{
		instance = this;
		DisableVisibilityOfChildren(MovingObjects.instance.mo["PlayMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["DeckPicker"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["DifficultySelector"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["DailyMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["UnlocksMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["StatsMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["VariantsMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["SeedInput"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["BaubleVariantsMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["SpecialOptionsVariantMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["DeckVariantMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["RoundsVariantMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["SpecialCardsVariantMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["ZodiacsVariantMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["LoadVariantMenu"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["VariantDetailsInput"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["ImportStringDialog"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["SpritePicker"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["ColorPicker"].rt);
		DisableVisibilityOfChildren(MovingObjects.instance.mo["ExportStringDialog"].rt);
	}
	
	public IEnumerator MainMenuLoadedCoroutine()
	{
		yield return null;
		TransitionStinger.instance.sceneLoaded = true;
	}
	
	public void SetMainMenuButtons(bool enabledState)
	{
		playButton.ChangeButtonEnabled(enabledState);
		statsButton.ChangeButtonEnabled(enabledState);
		unlocksButton.ChangeButtonEnabled(enabledState);
		optionsButton.ChangeButtonEnabled(enabledState);
		exitButton.ChangeButtonEnabled(enabledState);
	}
	
	public void SetPlayMenuButtons(bool enabledState)
	{
		newGameButton.ChangeButtonEnabled(enabledState);
		// loadGameButton.ChangeButtonEnabled(enabledState);
		if(Stats.instance.GetStatInt("standardRunsPlayed") > 0)
		{
			dailyGameButton.ChangeButtonEnabled(enabledState);
		}
		else
		{
			dailyGameButton.ChangeButtonEnabled(false);
		}
		if(Stats.instance.GetStatInt("dailyRunsPlayed") > 0)
		{
			customGameButton.ChangeButtonEnabled(enabledState);
		}
		else
		{
			customGameButton.ChangeButtonEnabled(false);
		}
		playMenuBackButton.ChangeButtonEnabled(enabledState);
	}
	
	public void SetSelfPromotionButtons(bool enabledState)
	{
		discordButton.ChangeButtonEnabled(enabledState);
		blueSkyButton.ChangeButtonEnabled(enabledState);
		koFiButton.ChangeButtonEnabled(enabledState);
	}
	
	public void PlayClicked()
	{
		MovingObjects.instance.mo["MainMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["Version"].StartMove("OffScreen");
		MovingObjects.instance.mo["ExitButton"].StartMove("OffScreen");
	}
	
	public void PlayMenuBackClicked()
	{
		MovingObjects.instance.mo["MainMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["Version"].StartMove("OnScreen");
		MovingObjects.instance.mo["ExitButton"].StartMove("OnScreen");
	}
	
	public void NewGameClicked()
	{
		string standardGameFileText = LocalInterface.instance.GetFileText(LocalInterface.instance.standardGameSaveFileName);
		if(standardGameFileText == null)
		{
			if(Stats.instance.GetStatInt("standardRunsPlayed") == 0)
			{
				OptionsDialog.instance.SetupDialog($"Would you like to play the tutorial?", new string[2]{"No", "Yes"}, new ThemeManager.UIElementType[2]{ThemeManager.UIElementType.warningButtonActive, ThemeManager.UIElementType.standardButtonActive}, new UnityAction[2]{OpenStandardGameMenu, StartTutorial});
			}
			else
			{
				OpenStandardGameMenu();
			}
		}
		else
		{
			OptionsDialog.instance.SetupDialog($"Load your game in progress? Starting a new game will overwrite your save.", new string[2]{"Overwrite", "Load"}, new ThemeManager.UIElementType[2]{ThemeManager.UIElementType.warningButtonActive, ThemeManager.UIElementType.standardButtonActive}, new UnityAction[2]{OverwriteStandardGame, LoadStandardGame});
		}
	}
	
	public void StartTutorial()
	{
		OptionsDialog.instance.SetVisibility(false);
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		
		MovingObjects.instance.mo["MainMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["Version"].StartMove("OffScreen");
		MovingObjects.instance.mo["ExitButton"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreen");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["BaubleVariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SpecialOptionsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["RoundsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SpecialCardsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["ZodiacsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["SpritePicker"].StartMove("OffScreen");
		MovingObjects.instance.mo["ColorPicker"].StartMove("OffScreen");
		MovingObjects.instance.mo["LoadVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["ImportStringDialog"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DifficultySelector"].StartMove("OffScreen");
		MovingObjects.instance.mo["UnlocksMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DailyMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["StatsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["ExportStringDialog"].StartMove("OffScreen");
		V.i.loadingGame = false;
		V.i.seed = 1;
		int difficulty = 0;
		V.i.v = new Variant(DifficultySelector.instance.difficultyVariantStrings[difficulty]);
		V.i.currentDifficulty = difficulty; // 0-9
		V.i.chosenDeck = "Swirly";
		V.i.chosenDeckSprite = Decks.instance.decks["Swirly"].cardBack;
		V.i.chosenDeckDescription = Decks.instance.decks["Swirly"].description;
		V.i.isCustomGame = false;
		V.i.isDailyGame = false;
		V.i.isTutorial = true;
		V.i.dateTimeStarted = DateTime.Now;
		Stats.instance.AdjustStatInt("standardRunsPlayed", 1);
		TransitionStinger.instance.StartStinger("GameplayScene");
	}
	
	public void CustomGameClicked()
	{
		string customGameFileText = LocalInterface.instance.GetFileText(LocalInterface.instance.customGameSaveFileName);
		if(customGameFileText == null)
		{
			OpenCustomGameMenu();
		}
		else
		{
			OptionsDialog.instance.SetupDialog($"Load your custom game in progress? Starting a new game will overwrite your save.", new string[2]{"Overwrite", "Load"}, new ThemeManager.UIElementType[2]{ThemeManager.UIElementType.warningButtonActive, ThemeManager.UIElementType.standardButtonActive}, new UnityAction[2]{OverwriteCustomGame, LoadCustomGame}); //new Color(1f, 0.5f, 0f, 1f)
		}
	}
	
	public void DailyGameClicked()
	{
		//if daily game already played
		string dailyGameFileText = LocalInterface.instance.GetFileText(LocalInterface.instance.dailyGameSaveFileName);
		if(dailyGameFileText == null)
		{
			OpenDailyGameMenu();
		}
		else
		{
			// say something different if it is a new day
			OptionsDialog.instance.SetupDialog($"Load your daily game in progress? Starting a new game will overwrite your save.", new string[2]{"Overwrite", "Load"}, new ThemeManager.UIElementType[2]{ThemeManager.UIElementType.warningButtonActive, ThemeManager.UIElementType.standardButtonActive}, new UnityAction[2]{OverwriteDailyGame, LoadDailyGame});
		}
	}
	
	public void OpenDailyGameMenu()
	{
		DailyMenu.instance.DailyMenuOpened();
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DailyMenu"].StartMove("OnScreen");
		OptionsDialog.instance.SetVisibility(false);
	}
	
	public void OpenStandardGameMenu()
	{
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreen");
		MovingObjects.instance.mo["DifficultySelector"].StartMove("OnScreen");
		OptionsDialog.instance.SetVisibility(false);
	}
	
	public void OverwriteCustomGame()
	{
		LocalInterface.instance.MoveSaveGameToFinishedGame(false, true, LocalInterface.instance.GetDateTimeStartedFromSaveFile(LocalInterface.instance.customGameSaveFileName));
		OpenCustomGameMenu();
	}
	
	public void OverwriteDailyGame()
	{
		LocalInterface.instance.MoveSaveGameToFinishedGame(true, false, LocalInterface.instance.GetDateTimeStartedFromSaveFile(LocalInterface.instance.dailyGameSaveFileName));
		OpenDailyGameMenu();
	}
	
	public void OverwriteStandardGame()
	{
		LocalInterface.instance.MoveSaveGameToFinishedGame(false, false, LocalInterface.instance.GetDateTimeStartedFromSaveFile(LocalInterface.instance.standardGameSaveFileName));
		OpenStandardGameMenu();
	}
	
	public void OpenCustomGameMenu()
	{
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].TeleportTo("OffScreenVariant");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		OptionsDialog.instance.SetVisibility(false);
	}
	
	public void LoadCustomGame()
	{
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		OptionsDialog.instance.SetVisibility(false);
		TransitionStinger.instance.StartStinger("GameplayScene");
		V.i.loadingGame = true;
		V.i.isTutorial = false;
		V.i.loadingGameInformation = LocalInterface.instance.GetFileText(LocalInterface.instance.customGameSaveFileName);
	}
	
	public void LoadDailyGame()
	{
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		OptionsDialog.instance.SetVisibility(false);
		TransitionStinger.instance.StartStinger("GameplayScene");
		V.i.loadingGame = true;
		V.i.isTutorial = false;
		V.i.loadingGameInformation = LocalInterface.instance.GetFileText(LocalInterface.instance.dailyGameSaveFileName);
	}
	
	public void LoadStandardGame()
	{
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["PlayMenu"].StartMove("OffScreen");
		OptionsDialog.instance.SetVisibility(false);
		TransitionStinger.instance.StartStinger("GameplayScene");
		V.i.loadingGame = true;
		V.i.isTutorial = false;
		V.i.loadingGameInformation = LocalInterface.instance.GetFileText(LocalInterface.instance.standardGameSaveFileName);
	}
	
	public void OptionsButtonClicked()
	{
		Preferences.instance.OpenMenu();
	}
	
	public void UnlocksClicked()
	{
		MovingObjects.instance.mo["MainMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["Version"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["UnlocksMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["ExitButton"].StartMove("OffScreen");
	}
	
	public void StatsClicked()
	{
		MovingObjects.instance.mo["MainMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["Version"].StartMove("OffScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OffScreen");
		MovingObjects.instance.mo["Title"].StartMove("OffScreen");
		MovingObjects.instance.mo["StatsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["ExitButton"].StartMove("OffScreen");
	}
	
	public void ExitButtonClicked()
	{
		Preferences.instance.ExitGame();
	}
	
	public void EnableVisibilityOfChildren(RectTransform rectTransform)
	{
		foreach(RectTransform child in rectTransform)
		{
			child.gameObject.SetActive(true);
		}
	}
	
	public void DisableVisibilityOfChildren(RectTransform rectTransform)
	{
		foreach(RectTransform child in rectTransform)
		{
			child.gameObject.SetActive(false);
		}
	}
	
	public void SetVisibilityOfExitButtonObject(bool visible)
	{
		#if !UNITY_WEBGL || UNITY_EDITOR
			exitButtonObject.gameObject.SetActive(visible);
		#endif
	}
}
