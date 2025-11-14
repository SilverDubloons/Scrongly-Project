using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Variant;
using System;
using UnityEngine.Events;


// D:\Unity Projects\ScronglyEnhanced\Assets\Text

public class VariantsMenu : MonoBehaviour
{
	public ButtonPlus loadButton;
	public ButtonPlus saveButton;
	public ButtonPlus newButton;
	public ButtonPlus editButton;
	public ButtonPlus importButton;
	public ButtonPlus exportButton;
	
	public ButtonPlus specialOptionsButton;
	public ButtonPlus baublesButton;
	public ButtonPlus cardsButton;
	public ButtonPlus antesButton;
	public ButtonPlus specialCardsButton;
	public ButtonPlus zodiacsButton;
	
	public bool variantLoaded;
	public VariantSimple loadedVariantSimple;
	public Variant loadedVariant;
	public Variant loadedVariantBeforeChanges;
	public string variantsFolderPath;
	// public bool selectedVariantHasChanged;
	
	public TextAsset baseVariantBaublesText;
	public TextAsset baseVariantSpecialOptionsText;
	public TextAsset baseVariantRoundsText;
	public TextAsset baseVariantSpecialCardsText;
	public TextAsset bossRoundsText;
	public Sprite[] baubleImages;
	public Sprite[] specialCardImages;
	public Sprite[] variantImages;
	
	/* public string unlockedBaublesFileName;
	public string unlockedBaublesFileVersion;
	public List<string> unlockedBaubles = new List<String>();
	
	public string unlockedSpecialCardsFileName;
	public string unlockedSpecialCardsFileVersion;
	public List<string> unlockedSpecialCards = new List<String>(); */
	
	public Variant baseVariant;
	public List<BaseGameLevel> baseGameLevels = new List<BaseGameLevel>();
	
	public BaubleVariantsMenu baubleVariantsMenu;
	public SpecialOptionsVariantMenu specialOptionsVariantMenu;
	public DeckVariantMenu deckVariantMenu;
	public RoundsVariantMenu roundsVariantMenu;
	public SpecialCardsVariantMenu specialCardsVariantMenu;
	public ZodiacsVariantMenu zodiacsVariantMenu;
	public SpritePicker spritePicker;
	public LoadVariantMenu loadVariantMenu;
	public static VariantsMenu instance;
	
 	public void SetupInstance(bool firstSceneChange)
	{
		instance = this;
		baubleVariantsMenu.SetupInstance();
		specialOptionsVariantMenu.SetupInstance();
		roundsVariantMenu.SetupInstance(firstSceneChange);
		deckVariantMenu.SetupInstance();
		specialCardsVariantMenu.SetupInstance();
		zodiacsVariantMenu.SetupInstance();
		spritePicker.SetupInstance();
		loadVariantMenu.SetupInstance();
	}
	
	[System.Serializable]
	public struct BaseGameLevel
	{
		public string levelName;
		public string levelDescription;
	}
	
	public void SetVariantsMenuButtons(bool enabledState)
	{
		loadButton.ChangeButtonEnabled(enabledState);
		newButton.ChangeButtonEnabled(enabledState);
		importButton.ChangeButtonEnabled(enabledState);
		if(variantLoaded)
		{
			saveButton.ChangeButtonEnabled(enabledState);
			editButton.ChangeButtonEnabled(enabledState);
			exportButton.ChangeButtonEnabled(enabledState);
			specialOptionsButton.ChangeButtonEnabled(enabledState);
			baublesButton.ChangeButtonEnabled(enabledState);
			cardsButton.ChangeButtonEnabled(enabledState);
			antesButton.ChangeButtonEnabled(enabledState);
			specialCardsButton.ChangeButtonEnabled(enabledState);
			zodiacsButton.ChangeButtonEnabled(enabledState);
			loadedVariantSimple.SetVariantButton(enabledState);
		}
		else
		{
			saveButton.ChangeButtonEnabled(false);
			editButton.ChangeButtonEnabled(false);
			exportButton.ChangeButtonEnabled(false);
			specialOptionsButton.ChangeButtonEnabled(false);
			baublesButton.ChangeButtonEnabled(false);
			cardsButton.ChangeButtonEnabled(false);
			antesButton.ChangeButtonEnabled(false);
			specialCardsButton.ChangeButtonEnabled(false);
			zodiacsButton.ChangeButtonEnabled(false);
		}
	}
	
	public void BaublesClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["BaubleVariantsMenu"].StartMove("OnScreen");
		BaubleVariantsMenu.instance.SetBaubleVariantOptionsToVariant(loadedVariant);
	}
	
	public void SpecialOptionsClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["SpecialOptionsVariantMenu"].StartMove("OnScreen");
		SpecialOptionsVariantMenu.instance.SetSpecialOptionsToVariant(loadedVariant);
	}
	
	public void DeckClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["DeckVariantMenu"].StartMove("OnScreen");
		DeckVariantMenu.instance.SetDeckVariantMenuToVariant(loadedVariant);
	}
	
	public void RoundsClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["RoundsVariantMenu"].StartMove("OnScreen");
		RoundsVariantMenu.instance.SetRoundOptionsToVariant(loadedVariant);
	}
	
	public void SpecialCardsClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["SpecialCardsVariantMenu"].StartMove("OnScreen");
		SpecialCardsVariantMenu.instance.SetSpecialCardVariantOptionsToVariant(loadedVariant);
	}
	
	public void ZodiacsClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["ZodiacsVariantMenu"].StartMove("OnScreen");
		ZodiacsVariantMenu.instance.SetZodiacsVariantMenuToVariant(loadedVariant);
	}
	
	public void SaveClicked()
	{
		string existingFileText = GetVariantFile(loadedVariant.variantName);
		if(existingFileText == null)
		{
			MinorNotifications.instance.NewMinorNotification("Saved!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(saveButton.rt, LocalInterface.instance.mainMenuCanvas));
			SaveOverClicked();
		}
		else
		{
			OptionsDialog.instance.SetupDialog($"Would you Like to Save Over {loadedVariant.variantName}?", new string[2]{"Yes", "Save New"}, new ThemeManager.UIElementType[2]{ThemeManager.UIElementType.warningButtonActive, ThemeManager.UIElementType.standardButtonActive}, new UnityAction[2]{SaveOverClicked, SaveAsClicked});
		}
	}
	
	public void SaveOverClicked()
	{
		string variantString = loadedVariant.ConvertToText();
		SaveVariantToFile(variantString, loadedVariant.variantName);
		loadedVariantBeforeChanges = new Variant(loadedVariant);
		OptionsDialog.instance.SetVisibility(false);
	}
	
	public void SaveVariantToFile(string variantString, string variantName)
	{
		LocalInterface.instance.SetFileText($"{variantsFolderPath}{variantName}", variantString, false);
	}
	
	public void SaveAsClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OnScreen");
		VariantDetailsInput.instance.SetDetailsToVariant(loadedVariant);
		VariantDetailsInput.instance.oldVariantName = string.Empty;
		OptionsDialog.instance.SetVisibility(false);
		VariantDetailsInput.instance.makingNewVariant = false;
	}
	
	public void LoadClicked()
	{
		LoadVariantMenu.instance.SpawnVariantSimples();
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["LoadVariantMenu"].StartMove("OnScreen");
	}
	
	public void EditClicked()
	{
		VariantDetailsInput.instance.SetDetailsToVariant(loadedVariant);
		VariantDetailsInput.instance.oldVariantName = loadedVariant.variantName;
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OnScreen");
		VariantDetailsInput.instance.makingNewVariant = false;
	}
	
	public void NewClicked()
	{
		VariantDetailsInput.instance.oldVariantName = string.Empty;
		VariantDetailsInput.instance.SetDetailsToNewVariant();
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OnScreen");
		VariantDetailsInput.instance.makingNewVariant = true;
	}
	
	public void ImportClicked()
	{
		ImportStringDialog.instance.ClearText();
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
		MovingObjects.instance.mo["ImportStringDialog"].StartMove("OnScreen");
	}
	
	public void ExportClicked()
	{
		string variantString = loadedVariant.ConvertToText();
		#if UNITY_WEBGL
		// #if UNITY_EDITOR
			MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
			MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
			MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
			MovingObjects.instance.mo["ExportStringDialog"].StartMove("OnScreen");
			ExportStringDialog.instance.SetupDialog(variantString);
		#else
			GUIUtility.systemCopyBuffer = variantString;
			MinorNotifications.instance.NewMinorNotification("Copied to Clipboard!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(exportButton.rt, LocalInterface.instance.mainMenuCanvas));
		#endif
		
	}
	
	public string GetVariantFile(string variantName)
	{
		string variantFileText = LocalInterface.instance.GetFileText($"{variantsFolderPath}{variantName}");
		if(variantFileText == null)
		{
			return null;
		}
		return variantFileText;
	}
	
	public void DeleteVariant(string variantName)
	{
		LocalInterface.instance.DeleteFile($"{variantsFolderPath}{variantName}");
	}
	
	public void LoadBaseVariant()
	{
		List<VariantCard> startingDeck = new List<VariantCard>();
		for(int i = 0; i < 52; i++)
		{
			startingDeck.Add(new VariantCard(i % 13, i / 13));
		}
		Dictionary<string, VariantSpecialOption> variantSpecialOptions = new Dictionary<string, VariantSpecialOption>();
		string[] specialOptionsRows = baseVariantSpecialOptionsText.text.Split('\n');
		for(int i = 1; i < specialOptionsRows.Length; i++)
		{
			string[] columns = specialOptionsRows[i].Split(',');
			string tag = columns[0];
			string label = columns[1];
			string description = columns[2];
			string type = columns[3];
			Vector2Int range = Vector2Int.zero;
			bool inEffect = true;
			int impact = 0;
			if(type == "Toggle")
			{
				inEffect = bool.Parse(columns[5]);
			}
			else
			{
				string[] rangeStrings = columns[4].Split('-');
				range = new Vector2Int(int.Parse(rangeStrings[0]), int.Parse(rangeStrings[1]));
				impact = int.Parse(columns[5]);
				if(impact == -1)
				{
					inEffect = false;
				}
			}
			VariantSpecialOption newVariantSpecialOption = new VariantSpecialOption(tag, label, description, type, range, inEffect, impact);
			variantSpecialOptions.Add(tag, newVariantSpecialOption);
		}
		Dictionary<string, VariantBauble> variantBaubles = new Dictionary<string, VariantBauble>();
		string[] baublesRows = baseVariantBaublesText.text.Split('\n');
		for(int i = 1; i < baublesRows.Length; i++)
		{
			string[] columns = baublesRows[i].Split(',');
			string tag = columns[0];
			string baubleName = columns[1];
			string menuDescription = columns[2].Replace("COMMA", ",");;
			string inGameDescription = columns[3].Replace("COMMA", ",");;
			int max = int.Parse(columns[4]);
			int baseCost = int.Parse(columns[5]);
			int costStep = int.Parse(columns[6]);
			double impact1 = double.Parse(columns[7]);
			double impact2 = double.Parse(columns[8]);
			string category = columns[9];
			Sprite sprite = GetSpriteFromCoordinates(columns[10], baubleImages);
			bool startsAvailable = bool.Parse(columns[11]);
			bool mustBeUnlocked = bool.Parse(columns[12]);
			string howToUnlock = columns[13];
			List<string> extraDescriptions = new List<string>();
			for(int j = 14; j < columns.Length; j++)
			{
				/* if(columns[j].Length > 0 && columns[j] != "")
				{
					extraDescriptions.Add(columns[j].Replace("COMMA", ","));
					Debug.Log($"Adding '{columns[j].Replace("COMMA", ",")}' to extraDescriptions[{extraDescriptions.Count - 1}] of {baubleName}");
				} */
				string extraDescription = columns[j].Trim('\r', '\n', ' ');
				if(!string.IsNullOrEmpty(extraDescription))
				{
					extraDescriptions.Add(extraDescription.Replace("COMMA", ","));
					// Debug.Log($"Adding '{extraDescription.Replace("COMMA", ",")}' to extraDescriptions[{extraDescriptions.Count - 1}] of {baubleName}");
				}
			}
			// Debug.Log($"tag={tag}, howToUnlock={howToUnlock}");
			bool availableInStore = startsAvailable;
			// if(startsAvailable && (!mustBeUnlocked || (mustBeUnlocked && IsBaubleUnlocked(tag))))
			if(mustBeUnlocked)
			{
				if(!LocalInterface.instance.IsBaubleUnlocked(tag))
				{
					// availableInStore = false;
				}
			}
			VariantBauble newBaubleVariant = new VariantBauble(tag, baubleName, menuDescription, inGameDescription, max, baseCost, costStep, impact1, impact2, category, sprite, availableInStore, mustBeUnlocked, 0, howToUnlock, extraDescriptions.ToArray());
			variantBaubles.Add(tag, newBaubleVariant);
		}
		Dictionary<int, VariantRound> variantRounds = new Dictionary<int, VariantRound>();
		string[] baseRounds = baseVariantRoundsText.text.Split('\n');
		for(int i = 0; i < 50; i ++)
		{
			VariantRound newVariantRound = new VariantRound(i, double.Parse(baseRounds[i]), string.Empty);
			variantRounds.Add(i, newVariantRound);
		}
		Dictionary<string, VariantSpecialCard> variantSpecialCards = new Dictionary<string, VariantSpecialCard>();
		string[] specialCardsRows = baseVariantSpecialCardsText.text.Split('\n');
		for(int i = 1; i < specialCardsRows.Length; i++)
		{
			string[] columns = specialCardsRows[i].Split(',');
			string tag = columns[0];
			string specialCardName = columns[1];
			string description = columns[2].Replace("COMMA", ",");
			string category = columns[3];
			Sprite sprite = GetSpriteFromCoordinates(columns[4], specialCardImages);
			Sprite playedSprite = GetSpriteFromCoordinates(columns[5], specialCardImages);
			int cost = int.Parse(columns[6]);
			double impact = double.Parse(columns[7]);
			bool startsAvailable = bool.Parse(columns[8]);
			bool mustBeUnlocked = bool.Parse(columns[9]);
			bool inShop = startsAvailable;
			string howToUnlock = columns[10];
			if(mustBeUnlocked)
			{
				if(!LocalInterface.instance.IsSpecialCardUnlocked(tag))
				{
					// inShop = false;
				}
			}
			VariantSpecialCard newVariantSpecialCard = new VariantSpecialCard(tag, specialCardName, description, category, sprite, playedSprite, cost, impact, inShop, mustBeUnlocked, howToUnlock);
			variantSpecialCards.Add(tag, newVariantSpecialCard);
		}
		baseVariant = new Variant(baseGameLevels[0].levelName, baseGameLevels[0].levelDescription, startingDeck, 0, false, 0, false, variantSpecialOptions, variantBaubles, variantRounds, variantSpecialCards, "Variant", 17, Color.red);
		LocalInterface.instance.baseVariant = new Variant(baseVariant);
	}
	
	public Sprite GetSpriteFromCoordinates(string coords, Sprite[] spriteArray)
	{
		string rowString = coords.Substring(0, 1).ToUpper();
		char rowChar = char.Parse(rowString);
		int rowInt = rowChar - 'A';
		string columnString = coords.Substring(1);
		int columnInt = int.Parse(columnString);
		int imageIndex = rowInt * 16 + columnInt;
		return spriteArray[imageIndex];
	}
	
	public Sprite GetVariantSprite(string category, int index)
	{
		switch(category)
		{
			case "Variant":
				return variantImages[index];
			case "Bauble":
				return baubleImages[index];
			case "SpecialCard":
				return specialCardImages[index];
		}
		return variantImages[0];
	}
	
/* 	public void PopulateUnlockedBaubles()
	{
		unlockedBaubles.Clear();
		string[] unlockedBaublesLines = LocalInterface.instance.GetFileTextLines(unlockedBaublesFileName);
		if(unlockedBaublesLines != null)
		{
			if(unlockedBaublesLines[0].Trim() != unlockedBaublesFileVersion)
			{
				LocalInterface.instance.DisplayError($"Version mismatch in unlocked Baubles file. File version is {unlockedBaublesLines[0].Trim()}, current version is {unlockedBaublesFileVersion}");
				return;
			}
			if(unlockedBaublesLines.Length > 1)
			{
				for(int i = 1; i < unlockedBaublesLines.Length; i++)
				{
					unlockedBaubles.Add(unlockedBaublesLines[i].Trim());
				}
			}
		}
		else
		{
			string unlockedBaublesFileText = unlockedBaublesFileVersion;
			LocalInterface.instance.SetFileText(unlockedBaublesFileName, unlockedBaublesFileVersion);
		}
	}
	
	public void UnlockBauble(string tag)
	{
		if(unlockedBaubles.Contains(tag))
		{
			return;
		}
		unlockedBaubles.Add(tag);	// Should new baubles be unlocked in the current run? I say, why not?
		// AddBaubleToAvailableBaubles(tag);
		string unlockedBaublesFileText = LocalInterface.instance.GetFileText(unlockedBaublesFileName);
		unlockedBaublesFileText = $"{unlockedBaublesFileText}\n{tag}";
		LocalInterface.instance.SetFileText(unlockedBaublesFileName, unlockedBaublesFileText);
	}
	
	public bool IsBaubleUnlocked(string tag)
	{
		if(unlockedBaubles.Contains(tag))
		{
			return true;
		}
		return false;
	}
	
	public void PopulateUnlockedSpecialCards()
	{
		unlockedSpecialCards.Clear();
		string[] unlockedSpecialCardLines = LocalInterface.instance.GetFileTextLines(unlockedSpecialCardsFileName);
		if(unlockedSpecialCardLines != null)
		{
			if(unlockedSpecialCardLines[0].Trim() != unlockedSpecialCardsFileVersion)
			{
				LocalInterface.instance.DisplayError($"Version mismatch in unlocked Special Cards file. File version is {unlockedSpecialCardLines[0].Trim()}, current version is {unlockedSpecialCardsFileVersion}");
				return;
			}
			if(unlockedSpecialCardLines.Length > 1)
			{
				for(int i = 1; i < unlockedSpecialCardLines.Length; i++)
				{
					unlockedSpecialCards.Add(unlockedSpecialCardLines[i].Trim());
				}
			}
		}
		else
		{
			string unlockedSpecialCardsFileText = unlockedSpecialCardsFileVersion;
			LocalInterface.instance.SetFileText(unlockedSpecialCardsFileName, unlockedSpecialCardsFileVersion);
		}
	}
	
	public void UnlockSpecialCard(string tag)
	{
		if(unlockedSpecialCards.Contains(tag))
		{
			return;
		}
		unlockedSpecialCards.Add(tag);
		string unlockedSpecialCardsFileText = LocalInterface.instance.GetFileText(unlockedSpecialCardsFileName);
		unlockedSpecialCardsFileText = $"{unlockedSpecialCardsFileText}\n{tag}";
		LocalInterface.instance.SetFileText(unlockedSpecialCardsFileName, unlockedSpecialCardsFileText);
	}
	
	public bool IsSpecialCardUnlocked(string tag)
	{
		if(unlockedSpecialCards.Contains(tag))
		{
			return true;
		}
		return false;
	} */
	
	public void SetupMenuForVariant(Variant variant)
	{
		baubleVariantsMenu.SetupBaubleVariantMenu(variant);
		specialOptionsVariantMenu.SetupSpecialOptionsVariantMenu(variant);
		deckVariantMenu.SetupDeckVariantMenu(variant);
		roundsVariantMenu.SetupRoundsVariantMenu(variant);
		specialCardsVariantMenu.SetupSpecialCardsVariantMenu(variant);
		zodiacsVariantMenu.SetupZodiacsVariantMenu(variant);
		spritePicker.SetupSpritePicker();
		loadedVariantSimple.UpdateVariantSimpleForVariant(variant);
		/* loadedVariantSimple.variantNameLabel.ChangeText(variant.variantName);
		loadedVariantSimple.variantDescriptionButton.ChangeButtonText(variant.variantDescription);
		loadedVariantSimple.variantIcon.sprite = variant.variantSprite;
		loadedVariantSimple.variantIcon.color = variant.variantSpriteColor; */
		loadedVariant = new Variant(variant);
		loadedVariantBeforeChanges = new Variant(variant);
		variantLoaded = true;
	}
		
	public void UpdateVariantSimpleToLoadedVariant()
	{
		if(variantLoaded)
		{
			loadedVariantSimple.UpdateVariantSimpleForVariant(loadedVariant);
		}
	}
	
/* 	void Update()
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			string baseVariantString = "BaseVariant=\n";
			baseVariantString += baseVariant.GetPrintedVariantString();
			Debug.Log(baseVariantString);
		}
		if(Input.GetKeyDown(KeyCode.L))
		{
			string loadedVariantString = "LoadedVariant=\n";
			loadedVariantString += loadedVariant.GetPrintedVariantString();
			Debug.Log(loadedVariantString);
		}
		if(Input.GetKeyDown(KeyCode.Q))
		{
			string baseVariantString = "BaseVariantConvertToText=\n";
			baseVariantString += baseVariant.ConvertToText();
			Debug.Log(baseVariantString);
		}
		if(Input.GetKeyDown(KeyCode.W))
		{
			string loadedVariantString = "LoadedVariantConvertToText=\n";
			loadedVariantString += loadedVariant.ConvertToText();
			Debug.Log(loadedVariantString);
		}
	} */
}
