using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using static Decks;

public class DeckPicker : MonoBehaviour
{
	public RectTransform rt;
	/* public string deckFileManagerVersion;
	public string decksFileName;
	public Deck[] decksArray;
	public Dictionary<string, Deck> decks = new Dictionary<string, Deck>();
	public List<string> decksOrder; */
	public GameObject deckIconPrefab;
	public Transform deckIconParent;
	public Sprite lockedIcon;
	
	public Color lockedColor;
	public Color unlockedColor;
	public Color selectedColor;
	
	// public string lastSelectedDeck;
	
	public Label deckNameLabel;
	public Label deckDescriptionLabel;
	
	public ButtonPlus playButton;
	public ButtonPlus backButton;
	public ButtonPlus lastDeckButton;
	public ButtonPlus nextDeckButton;
	
	public static DeckPicker instance;
	// public List<DeckSelectorCardBack> deckSelectorCardBacks = new List <DeckSelectorCardBack>();
	public DeckSelectorCardBack firstDeckSelectorCardBack;
	
/*     [System.Serializable]
    public class Deck
	{
		public string deckName;
		public string description;
		public string howToUnlock;
		public Sprite cardBack;
		public Sprite smallIcon;
		public bool unlocked;
		public DeckIcon deckIcon;
		public int deckInt;
	} */
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetDeckPickerButtons(bool enabledState)
	{
		if(enabledState && !Decks.instance.decks[Decks.instance.lastSelectedDeck].unlocked)
		{
			playButton.ChangeButtonEnabled(false);
		}
		else
		{
			playButton.ChangeButtonEnabled(enabledState);
		}
		backButton.ChangeButtonEnabled(enabledState);
		lastDeckButton.ChangeButtonEnabled(enabledState);
		nextDeckButton.ChangeButtonEnabled(enabledState);
	}
	
	/* public void SetupDecksDictionary()
	{
		for(int i = 0; i < decksArray.Length; i++)
		{
			decksArray[i].deckInt = i;
			decks.Add(decksArray[i].deckName, decksArray[i]);
			decksOrder.Add(decksArray[i].deckName);
		}
	}
	
	public void LoadDecks()
	{
		string[] lines = LocalInterface.instance.GetFileTextLines(decksFileName);
		if(lines == null)
		{
			lastSelectedDeck = decksArray[0].deckName;
			ResetDecksFile();
			return;
		}
		string fileVersion = lines[0].Trim();
		if(fileVersion != deckFileManagerVersion)
		{
			LocalInterface.instance.DisplayError("Your deck data file is either corrupted or trying to load an unsupported version. Your version = \"fileVersion\" expected version = \"deckFileManagerVersion\"");
			ResetDecksFile();
			return;
		}
		lastSelectedDeck = lines[1].Replace("lastSelectedDeck=","").Trim();
		for(int i = 2; i < decks.Count + 2; i++)
		{
			string[] lineData = lines[i].Split('=');
			decks[lineData[0]].unlocked = bool.Parse(lineData[1]);
		}
	}
	
	public void ResetDecksFile()
	{
		string newDeckFile = $"{deckFileManagerVersion}\nlastSelectedDeck={lastSelectedDeck}";
		foreach(KeyValuePair<string, Deck> entry in decks)
		{
			newDeckFile = $"{newDeckFile}\n{entry.Key}={entry.Value.unlocked.ToString()}";
		}
		LocalInterface.instance.SetFileText(decksFileName, newDeckFile);
	} */
	
	public void SetupSelectableDecks()
	{
		float maxWidth = rt.sizeDelta.x - 10f - 4f;
		float iconWidth = 10f;
		float squeezeDistance = (maxWidth - iconWidth) / (Decks.instance.decks.Count - 1);
		float distanceBetweenIcons = Mathf.Min(14f, squeezeDistance);
		int index = 0;
		foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			GameObject newIconGO = Instantiate(deckIconPrefab, Vector3.zero, Quaternion.identity, deckIconParent);
			DeckIcon newDeckIcon = newIconGO.GetComponent<DeckIcon>();
			Decks.instance.decks[entry.Key].deckIcon = newDeckIcon;
			float xDestination = (Decks.instance.decks.Count - 1) * (distanceBetweenIcons / 2f) - (Decks.instance.decks.Count - index - 1) * distanceBetweenIcons;
			newDeckIcon.rt.anchoredPosition = new Vector2(xDestination, 0);
			// StartCoroutine(RepositionRectTransform(newDeckIcon.rt, new Vector2(xDestination, 0)));
			// Debug.Log($"entry.Key={entry.Key}, pos={newDeckIcon.rt.anchoredPosition}");
			if(Decks.instance.decks[entry.Key].unlocked)
			{
				newDeckIcon.image.sprite = Decks.instance.decks[entry.Key].smallIcon;
				newDeckIcon.image.color = unlockedColor;
				newDeckIcon.imageRt.sizeDelta = new Vector2(Decks.instance.decks[entry.Key].smallIcon.rect.width, Decks.instance.decks[entry.Key].smallIcon.rect.height);
			}
			else
			{
				newDeckIcon.image.sprite = lockedIcon;
				newDeckIcon.image.color = lockedColor;
				newDeckIcon.imageRt.sizeDelta = new Vector2(lockedIcon.rect.width, lockedIcon.rect.height);
			}
			newDeckIcon.index = index;
			index++;
		}
		SelectedDeckUpdated();
	}
	
	public IEnumerator RepositionRectTransform(RectTransform rectTransform, Vector2 newPos)
	{
		yield return null;
		rectTransform.anchoredPosition = newPos;
/* 		foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			entry.Value.deckIcon.rt.anchoredPosition = new Vector2(-20f, 0);
		} */
	}
	
	public void StartSettingUpDecks()
	{
		StartCoroutine(SetUpDecks());
	}
	
	public IEnumerator SetUpDecks()
	{
		yield return null;
		SetupSelectableDecks();
	}
	
	public void SelectedDeckUpdated()
	{
		Decks.instance.decks[Decks.instance.lastSelectedDeck].deckIcon.image.color = selectedColor;
		firstDeckSelectorCardBack.image.sprite = Decks.instance.decks[Decks.instance.lastSelectedDeck].cardBack;
		if(Decks.instance.decks[Decks.instance.lastSelectedDeck].unlocked)
		{
			deckNameLabel.ChangeText(Decks.instance.decks[Decks.instance.lastSelectedDeck].deckName);
			deckDescriptionLabel.ChangeText(Decks.instance.decks[Decks.instance.lastSelectedDeck].description);
			firstDeckSelectorCardBack.lockedObject.SetActive(false);
			playButton.ChangeButtonEnabled(true);
		}
		else
		{
			deckNameLabel.ChangeText("To Unlock:");
			deckDescriptionLabel.ChangeText(Decks.instance.decks[Decks.instance.lastSelectedDeck].howToUnlock);
			firstDeckSelectorCardBack.lockedObject.SetActive(true);
			playButton.ChangeButtonEnabled(false);
		}
	}
	
	public void NextDeckClicked()
	{
		ChangeColorOfLastSelectedDeckBasedOnUnlockStatus();
		int nextIndex = Decks.instance.decksOrder.IndexOf(Decks.instance.lastSelectedDeck) + 1;
		if(nextIndex >= Decks.instance.decksOrder.Count)
		{
			nextIndex = 0;
		}
		Decks.instance.lastSelectedDeck = Decks.instance.decksOrder[nextIndex];
		SelectedDeckUpdated();
	}
	
	public void LastDeckClicked()
	{
		ChangeColorOfLastSelectedDeckBasedOnUnlockStatus();
		int nextIndex = Decks.instance.decksOrder.IndexOf(Decks.instance.lastSelectedDeck) - 1;
		if(nextIndex < 0)
		{
			nextIndex = Decks.instance.decksOrder.Count - 1;
		}
		Decks.instance.lastSelectedDeck = Decks.instance.decksOrder[nextIndex];
		SelectedDeckUpdated();
	}
	
	public void ChangeColorOfLastSelectedDeckBasedOnUnlockStatus()
	{
		if(Decks.instance.decks[Decks.instance.lastSelectedDeck].unlocked)
		{
			Decks.instance.decks[Decks.instance.lastSelectedDeck].deckIcon.image.color = unlockedColor;
		}
		else
		{
			Decks.instance.decks[Decks.instance.lastSelectedDeck].deckIcon.image.color = lockedColor;
		}
	}
	
	public void PlayClicked()
	{
		string currentLocation = MovingObjects.instance.mo["DeckPicker"].GetCurrentLocation();
		Decks.instance.ChangeLastSelectedDeckInFile();
		if(currentLocation == "OnScreen")
		{
			MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreen");
			MovingObjects.instance.mo["DifficultySelector"].StartMove("OffScreen");
			V.i.loadingGame = false;
			V.i.seed = UnityEngine.Random.Range(0, int.MaxValue);
			int difficulty = DifficultySelector.instance.GetDifficulty();
			V.i.v = new Variant(DifficultySelector.instance.difficultyVariantStrings[difficulty]);
			V.i.currentDifficulty = difficulty; // 0-9
			V.i.chosenDeck = Decks.instance.lastSelectedDeck;
			V.i.chosenDeckSprite = Decks.instance.decks[Decks.instance.lastSelectedDeck].cardBack;
			V.i.chosenDeckDescription = Decks.instance.decks[Decks.instance.lastSelectedDeck].description;
			V.i.isCustomGame = false;
			V.i.isDailyGame = false;
			V.i.isTutorial = false;
			Stats.instance.AdjustStatInt("standardRunsPlayed", 1);
			TransitionStinger.instance.StartStinger("GameplayScene");
			Preferences.instance.lastSelectedDifficulty = difficulty;
			Preferences.instance.SetPreferencesFileToCurrentSettings();
		}
		else if(currentLocation == "OnScreenVariant")
		{
			MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
			MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
			MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
			V.i.loadingGame = false;
			V.i.seed = SeedInput.instance.GetSeed();
			V.i.v = VariantsMenu.instance.loadedVariant;
			V.i.chosenDeck = Decks.instance.lastSelectedDeck;
			V.i.chosenDeckSprite = Decks.instance.decks[Decks.instance.lastSelectedDeck].cardBack;
			V.i.chosenDeckDescription = Decks.instance.decks[Decks.instance.lastSelectedDeck].description;
			V.i.isCustomGame = true;
			V.i.isDailyGame = false;
			V.i.isTutorial = false;
			V.i.currentDifficulty = -1;
			Stats.instance.AdjustStatInt("customRunsPlayed", 1);
			TransitionStinger.instance.StartStinger("GameplayScene");
			Preferences.instance.lastSelectedVariant = VariantsMenu.instance.loadedVariant.variantName;
			Preferences.instance.SetPreferencesFileToCurrentSettings();
		}
		V.i.dateTimeStarted = DateTime.Now;
	}
	
	public void BackClicked()
	{
		MovingObjects.instance.mo["PlayMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["Title"].StartMove("OnScreen");
		MovingObjects.instance.mo["SelfPromotion"].StartMove("OnScreen");
		string currentLocation = MovingObjects.instance.mo["DeckPicker"].GetCurrentLocation();
		// Debug.Log($"DeckPicker currentLocation={currentLocation}");
		if(currentLocation == "OnScreen")
		{
			MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreen");
			MovingObjects.instance.mo["DifficultySelector"].StartMove("OffScreen");
		}
		else if(currentLocation == "OnScreenVariant")
		{
			MovingObjects.instance.mo["DeckPicker"].StartMove("OffScreenVariant");
			MovingObjects.instance.mo["VariantsMenu"].StartMove("OffScreen");
			MovingObjects.instance.mo["SeedInput"].StartMove("OffScreen");
		}
	}
}