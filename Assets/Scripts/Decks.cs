using UnityEngine;
using System;
using System.Collections.Generic;

public class Decks : MonoBehaviour
{
    public string deckFileManagerVersion;
	public string unlockedDecksFileName;
	public Dictionary<string, Deck> decks = new Dictionary<string, Deck>();
	public List<string> decksOrder;
	public TextAsset decksSpreadsheet;
	public Sprite[] deckSprites;
	public Sprite smallIconSprite;
	
	public string lastSelectedDeck;
	
	public static Decks instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	[System.Serializable]
    public class Deck
	{
		public string deckName;
		public string description;
		public string howToUnlock;
		public int deckInt;
		public Sprite cardBack;
		public bool unlocked;
		public bool unlockedByDefault;
		public DeckIcon deckIcon;
		public Sprite smallIcon;
		
		public Deck(string deckName, string description, string howToUnlock, int deckInt, Sprite cardBack, bool unlocked)
		{
			this.deckName = deckName;
			this.description = description;
			this.howToUnlock = howToUnlock;
			this.deckInt = deckInt;
			this.cardBack = cardBack;
			this.unlocked = unlocked;
			this.unlockedByDefault = unlocked;
			this.deckIcon = null;
			this.smallIcon = Decks.instance.smallIconSprite;
		}
	}
	
	public void LoadDecksFromSpreadsheet()
	{
		string[] decksRows = decksSpreadsheet.text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 1; i < decksRows.Length; i++)
		{
			string[] deckData = decksRows[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
			int deckInt = int.Parse(deckData[3]);
			bool unlocked = bool.Parse(deckData[4]);
			decks.Add(deckData[0], new Deck(deckData[0], deckData[1].Replace("COMMA", ","), deckData[2].Replace("COMMA", ","), deckInt, deckSprites[deckInt], unlocked));
			decksOrder.Add(deckData[0]);
		}
		LoadUnlockedDecks();
	}
	
	public void LoadUnlockedDecks()
	{
		string[] lines = LocalInterface.instance.GetFileTextLines(unlockedDecksFileName);
		if(lines == null)
		{
			lastSelectedDeck = "Swirly";
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
		string newDeckFileString = $"{deckFileManagerVersion}\nlastSelectedDeck={lastSelectedDeck}";
		foreach(KeyValuePair<string, Deck> entry in decks)
		{
			newDeckFileString = $"{newDeckFileString}\n{entry.Key}={entry.Value.unlocked.ToString()}";
		}
		LocalInterface.instance.SetFileText(unlockedDecksFileName, newDeckFileString);
	}
	
	public void UnlockDeck(string deckName)
	{
		string[] lines = LocalInterface.instance.GetFileTextLines(unlockedDecksFileName);
		if(lines == null)
		{
			LocalInterface.instance.DisplayError($"Failed to unlock deck, unlocks file not found. deckFileManagerVersion={deckFileManagerVersion}");
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
		for(int i = 2; i < decks.Count + 2; i++)
		{
			string[] lineData = lines[i].Split('=');
			if(lineData[0] == deckName)
			{
				lineData[1] = "TRUE";
				lines[i] = $"{lineData[0]}={lineData[1]}";
				continue;
			}
		}
		string newUnlockedDecksTextFileString = string.Empty;
		for(int i = 0; i < lines.Length; i++)
		{
			newUnlockedDecksTextFileString += lines[i];
			if(i < lines.Length - 1)
			{
				newUnlockedDecksTextFileString += "\n";
			}
		}
		decks[deckName].unlocked = true;
		LocalInterface.instance.SetFileText(unlockedDecksFileName, newUnlockedDecksTextFileString);
		ItemEarnedNotifications.instance.Notify("New deck unlocked!", decks[deckName].howToUnlock, "UnlockedDeck", deckName, decks[deckName].cardBack);
	}
	
	public void ChangeLastSelectedDeckInFile()
	{
		string[] lines = LocalInterface.instance.GetFileTextLines(unlockedDecksFileName);
		if(lines == null)
		{
			LocalInterface.instance.DisplayError($"Failed to change last selected deck, unlocks file not found. deckFileManagerVersion={deckFileManagerVersion}");
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
		lines[1] = $"lastSelectedDeck={lastSelectedDeck}";
		string newUnlockedDecksTextFileString = string.Empty;
		for(int i = 0; i < lines.Length; i++)
		{
			newUnlockedDecksTextFileString += lines[i];
			if(i < lines.Length - 1)
			{
				newUnlockedDecksTextFileString += "\n";
			}
		}
		LocalInterface.instance.SetFileText(unlockedDecksFileName, newUnlockedDecksTextFileString);
	}
	
	public void DisplayAllDeckData()
	{
		foreach(KeyValuePair<string, Deck> entry in decks)
		{
			Debug.Log($"Key={entry.Key}, unlocked={entry.Value.unlocked}");
		}
	}
	
/* 	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Z))
		{
			DisplayAllDeckData();
		}
	} */
}
