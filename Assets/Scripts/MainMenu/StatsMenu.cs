using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class StatsMenu : MonoBehaviour
{
	public RectTransform statsBackdrop;
	public RectTransform statsContent;
	public RectTransform decksParent;
	
    public ButtonPlus standardButton;
    public ButtonPlus dailyButton;
    public ButtonPlus customButton;
    public ButtonPlus statsButton;
    public ButtonPlus decksButton;
    public ButtonPlus viewGamesButton;
    public ButtonPlus backButton;
    public ButtonPlus previousRunButton;
    public ButtonPlus nextRunButton;
    public ButtonPlus saveVariantButton;
	
	public ControllerSelectionGroup optionsControllerSelectionGroup;
	public ControllerSelectionGroup statsControllerSelectionGroup;
	public ControllerSelectionGroup runStatsPanelControllerSelectionGroup;
	
	public GameObject statsVisibilityObject;
	public GameObject decksVisibilityObject;
	public GameObject gameTypeObject;
	public GameObject subGroupObject;
	public GameObject saveVariantButtonBackdropObject;
	public RunStatsPanel runStatsPanel;
	public Label runStatsExtraInfoLabel;
	
	public DeckStats[] deckStats;
	
	public GameObject statLinePrefab;
	public List<StatLine> statLines = new List<StatLine>();
	public StatToDisplay[] standardStatsToDisplay;
	public StatToDisplay[] dailyStatsToDisplay;
	public StatToDisplay[] customStatsToDisplay;
	
	public List<string> oldStandardGames = new List<string>();
	public List<string> oldDailyGames = new List<string>();
	public List<string> oldCustomGames = new List<string>();
	public int currentRunIndex;
	public string currentVariantString;
	public string currentVariantName;
	public string longestDailyStreakString;
	
	public static StatsMenu instance;
	
	[System.Serializable]
	public class StatToDisplay
	{
		public string description;
		public string type; // int, float
		public string tag;
	}
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetupStatsMenu()
	{
		string oldStandardGamesDirectory = $"{LocalInterface.instance.localFilesDirectory}{LocalInterface.instance.completedStandardGamesFolderName}/";
		LoadAllSavedGamesInDirectoryIntoList(oldStandardGamesDirectory, $"{LocalInterface.instance.completedStandardGamesFolderName}/", oldStandardGames, "Standard");
		string oldDailyGamesDirectory = $"{LocalInterface.instance.localFilesDirectory}{LocalInterface.instance.completedDailyGamesFolderName}/";
		LoadAllSavedGamesInDirectoryIntoList(oldDailyGamesDirectory, $"{LocalInterface.instance.completedDailyGamesFolderName}/", oldDailyGames, "Daily");
		string oldCustomGamesDirectory = $"{LocalInterface.instance.localFilesDirectory}{LocalInterface.instance.completedCustomGamesFolderName}/";
		LoadAllSavedGamesInDirectoryIntoList(oldCustomGamesDirectory, $"{LocalInterface.instance.completedCustomGamesFolderName}/", oldCustomGames, "Custom");
		StatsClicked();
		StandardClicked();
		int deckIndex = 0;
		foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			deckStats[deckIndex].cardbackImage.sprite = entry.Value.cardBack;
			deckStats[deckIndex].valueLabel.ChangeText((Stats.instance.GetStatInt(Stats.instance.GetHighestDifficultyCompletedDeckParameter(entry.Value.deckName)) + 1).ToString());
			deckIndex++;
		}
	}
	
	public void LoadAllSavedGamesInDirectoryIntoList(string fullDirectory, string subdirectory, List<string> list, string type)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(fullDirectory);
		FileInfo[] fileInfos = directoryInfo.GetFiles();
		List<DateTime> dateTimes = new List<DateTime>();
		foreach(FileInfo file in fileInfos)
		{
			string fileText = LocalInterface.instance.GetFileText($"{subdirectory}{file.Name.Substring(0, file.Name.Length - 4)}");
			list.Add(fileText);
			dateTimes.Add(LocalInterface.instance.GetDateTimeStartedFromVariantString(fileText));
		}
		list.Sort((x, y) =>
		{
			return LocalInterface.instance.GetDateTimeStartedFromVariantString(x).CompareTo(LocalInterface.instance.GetDateTimeStartedFromVariantString(y));
		});
		if(type == "Daily")
		{
			string longestStreak = FindLongestStreak(dateTimes);
			longestDailyStreakString = longestStreak;
		}
	}
	
	public string FindLongestStreak(List<DateTime> gameDates)
    {
        if (gameDates == null || gameDates.Count == 0)
            return "No Daily Game Streak";
            
        if (gameDates.Count == 1)
            return "No Daily Game Streak"; // FormatSingleDate(gameDates[0]);

        // Extract just the date portion (without time) and remove duplicates
        var uniqueDates = gameDates
            .Select(d => d.Date)  // Get only the date part
            .Distinct()           // Remove duplicate dates
            .OrderBy(d => d)      // Sort chronologically
            .ToList();
        
        int maxStreakLength = 1;
        int currentStreakLength = 1;
        int maxStreakStartIndex = 0;
        int currentStreakStartIndex = 0;

        for (int i = 1; i < uniqueDates.Count; i++)
        {
            // Check if current date is consecutive to previous date
            if (uniqueDates[i] == uniqueDates[i - 1].AddDays(1))
            {
                currentStreakLength++;
                
                // Update max streak if current streak is longer
                if (currentStreakLength > maxStreakLength)
                {
                    maxStreakLength = currentStreakLength;
                    maxStreakStartIndex = currentStreakStartIndex;
                }
            }
            else
            {
                // Reset current streak
                currentStreakLength = 1;
                currentStreakStartIndex = i;
            }
        }

        // Handle case where there are no consecutive days
        if (maxStreakLength == 1)
        {
            return "No Daily Game Streak";
        }

        // Get the start and end dates of the longest streak
        DateTime streakStart = uniqueDates[maxStreakStartIndex];
        DateTime streakEnd = uniqueDates[maxStreakStartIndex + maxStreakLength - 1];
        
        return FormatDateRange(streakStart, streakEnd, maxStreakLength);
    }

    // ... (rest of the formatting methods remain the same)
    private string FormatSingleDate(DateTime date)
    {
        return $"Longest Daily Game Streak: {date:MMMM} {GetOrdinalSuffix(date.Day)} {date:yyyy} (1 Day)";
    }

    private string FormatDateRange(DateTime startDate, DateTime endDate, int maxStreakLength)
    {
        string startStr = $"{GetOrdinalSuffix(startDate.Day)} {startDate:MMMM} {startDate:yyyy}";
        string endStr = $"{GetOrdinalSuffix(endDate.Day)} {endDate:MMMM} {endDate:yyyy}";
        
        return $"Longest Daily Game Streak: {startStr} - {endStr} ({maxStreakLength} Days) ";
    }

    private string GetOrdinalSuffix(int day)
    {
        if (day < 1 || day > 31) return day.ToString();
        
        switch (day % 10)
        {
            case 1 when day != 11:
                return $"{day}st";
            case 2 when day != 12:
                return $"{day}nd";
            case 3 when day != 13:
                return $"{day}rd";
            default:
                return $"{day}th";
        }
    }
	
	public void SetInteractability(bool enabledState)
	{
		standardButton.ChangeButtonEnabled(enabledState);
		dailyButton.ChangeButtonEnabled(enabledState);
		customButton.ChangeButtonEnabled(enabledState);
		statsButton.ChangeButtonEnabled(enabledState);
		if(standardButton.specialState)
		{
			decksButton.ChangeButtonEnabled(enabledState);
		}
		else
		{
			decksButton.ChangeButtonEnabled(false);
		}
		viewGamesButton.ChangeButtonEnabled(enabledState);
		backButton.ChangeButtonEnabled(enabledState);
	}
	
	public void StandardClicked()
	{
		if(standardButton.specialState)
		{
			return;
		}
		standardButton.ChangeSpecialState(true);
		dailyButton.ChangeSpecialState(false);
		customButton.ChangeSpecialState(false);
		if(statsButton.specialState)
		{
			SetupStandardStats();
		}
		else if(decksButton.specialState)
		{
			// this can't happen as of now as per deck stats are only tracked for standard runs
		}
		decksButton.ChangeButtonEnabled(true);
		viewGamesButton.ChangeButtonEnabled(oldStandardGames.Count > 0);
	}
	
	public void SetupStandardStats()
	{
		for(int i = 0; i < standardStatsToDisplay.Length; i++)
		{
			if(statLines.Count > i)
			{
				statLines[i].gameObject.SetActive(true);
				statLines[i].rt.anchoredPosition = new Vector2(statLines[i].rt.anchoredPosition.x, -5 - 25 * i);
				switch(standardStatsToDisplay[i].type)
				{
					case "int":
						statLines[i].SetupStatLine(standardStatsToDisplay[i].description, Stats.instance.GetStatInt(standardStatsToDisplay[i].tag).ToString());
					break;
					case "double":
						statLines[i].SetupStatLine(standardStatsToDisplay[i].description, LocalInterface.instance.ConvertDoubleToString(Stats.instance.GetStatDouble(standardStatsToDisplay[i].tag)));
					break;
				}
			}
			else
			{
				GameObject newStatLineGO = Instantiate(statLinePrefab, statsContent);
				StatLine newStatLine = newStatLineGO.GetComponent<StatLine>();
				statLines.Add(newStatLine);
				newStatLine.rt.anchoredPosition = new Vector2(statLines[i].rt.anchoredPosition.x, -5f - 25f * i);
				switch(standardStatsToDisplay[i].type)
				{
					case "int":
						newStatLine.SetupStatLine(standardStatsToDisplay[i].description, Stats.instance.GetStatInt(standardStatsToDisplay[i].tag).ToString());
					break;
					case "double":
						newStatLine.SetupStatLine(standardStatsToDisplay[i].description, LocalInterface.instance.ConvertDoubleToString(Stats.instance.GetStatDouble(standardStatsToDisplay[i].tag)));
					break;
				}
			}
		}
		for(int i = standardStatsToDisplay.Length; i < statLines.Count; i++)
		{
			statLines[i].gameObject.SetActive(false);
		}
		statsContent.sizeDelta = new Vector2(statsContent.sizeDelta.x, 5f + standardStatsToDisplay.Length * 25f);
		statsBackdrop.sizeDelta = new Vector2(statsBackdrop.sizeDelta.x, Mathf.Min(statsContent.sizeDelta.y + 10f, 272f));
		runStatsExtraInfoLabel.gameObject.SetActive(false);
	}
	
	public void DailyClicked()
	{
		if(dailyButton.specialState)
		{
			return;
		}
		standardButton.ChangeSpecialState(false);
		dailyButton.ChangeSpecialState(true);
		customButton.ChangeSpecialState(false);
		if(statsButton.specialState)
		{
			SetupDailyStats();
		}
		else if(decksButton.specialState)
		{
			StatsClicked();
		}
		decksButton.ChangeButtonEnabled(false);
		viewGamesButton.ChangeButtonEnabled(oldDailyGames.Count > 0);
	}
	
	public void SetupDailyStats()
	{
		for(int i = 0; i < dailyStatsToDisplay.Length; i++)
		{
			if(statLines.Count > i)
			{
				statLines[i].gameObject.SetActive(true);
				statLines[i].rt.anchoredPosition = new Vector2(statLines[i].rt.anchoredPosition.x, -5 - 25 * i);
				switch(dailyStatsToDisplay[i].type)
				{
					case "int":
						statLines[i].SetupStatLine(dailyStatsToDisplay[i].description, Stats.instance.GetStatInt(dailyStatsToDisplay[i].tag).ToString());
					break;
					case "double":
						statLines[i].SetupStatLine(dailyStatsToDisplay[i].description, LocalInterface.instance.ConvertDoubleToString(Stats.instance.GetStatDouble(dailyStatsToDisplay[i].tag)));
					break;
				}
			}
			else
			{
				GameObject newStatLineGO = Instantiate(statLinePrefab, statsContent);
				StatLine newStatLine = newStatLineGO.GetComponent<StatLine>();
				statLines.Add(newStatLine);
				newStatLine.rt.anchoredPosition = new Vector2(statLines[i].rt.anchoredPosition.x, -5f - 25f * i);
				switch(dailyStatsToDisplay[i].type)
				{
					case "int":
						newStatLine.SetupStatLine(dailyStatsToDisplay[i].description, Stats.instance.GetStatInt(dailyStatsToDisplay[i].tag).ToString());
					break;
					case "double":
						newStatLine.SetupStatLine(dailyStatsToDisplay[i].description, LocalInterface.instance.ConvertDoubleToString(Stats.instance.GetStatDouble(dailyStatsToDisplay[i].tag)));
					break;
				}
			}
		}
		for(int i = dailyStatsToDisplay.Length; i < statLines.Count; i++)
		{
			statLines[i].gameObject.SetActive(false);
		}
		runStatsExtraInfoLabel.gameObject.SetActive(true);
		runStatsExtraInfoLabel.ChangeText(longestDailyStreakString);
		runStatsExtraInfoLabel.rt.anchoredPosition = new Vector2(runStatsExtraInfoLabel.rt.anchoredPosition.x, -5f - dailyStatsToDisplay.Length * 25f);
		statsContent.sizeDelta = new Vector2(statsContent.sizeDelta.x, 10f + dailyStatsToDisplay.Length * 25f + runStatsExtraInfoLabel.GetPreferredHeight());
		statsBackdrop.sizeDelta = new Vector2(statsBackdrop.sizeDelta.x, Mathf.Min(statsContent.sizeDelta.y + 10f, 272f));
		
	}
	
	public void SetupCustomStats()
	{
		for(int i = 0; i < customStatsToDisplay.Length; i++)
		{
			if(statLines.Count > i)
			{
				statLines[i].gameObject.SetActive(true);
				statLines[i].rt.anchoredPosition = new Vector2(statLines[i].rt.anchoredPosition.x, -5 - 25 * i);
				switch(customStatsToDisplay[i].type)
				{
					case "int":
						statLines[i].SetupStatLine(customStatsToDisplay[i].description, Stats.instance.GetStatInt(customStatsToDisplay[i].tag).ToString());
					break;
					case "double":
						statLines[i].SetupStatLine(customStatsToDisplay[i].description, LocalInterface.instance.ConvertDoubleToString(Stats.instance.GetStatDouble(customStatsToDisplay[i].tag)));
					break;
				}
			}
			else
			{
				GameObject newStatLineGO = Instantiate(statLinePrefab, statsContent);
				StatLine newStatLine = newStatLineGO.GetComponent<StatLine>();
				statLines.Add(newStatLine);
				newStatLine.rt.anchoredPosition = new Vector2(statLines[i].rt.anchoredPosition.x, -5f - 25f * i);
				switch(customStatsToDisplay[i].type)
				{
					case "int":
						newStatLine.SetupStatLine(customStatsToDisplay[i].description, Stats.instance.GetStatInt(customStatsToDisplay[i].tag).ToString());
					break;
					case "double":
						newStatLine.SetupStatLine(customStatsToDisplay[i].description, LocalInterface.instance.ConvertDoubleToString(Stats.instance.GetStatDouble(customStatsToDisplay[i].tag)));
					break;
				}
			}
		}
		for(int i = customStatsToDisplay.Length; i < statLines.Count; i++)
		{
			statLines[i].gameObject.SetActive(false);
		}
		statsContent.sizeDelta = new Vector2(statsContent.sizeDelta.x, 5f + customStatsToDisplay.Length * 25f);
		statsBackdrop.sizeDelta = new Vector2(statsBackdrop.sizeDelta.x, Mathf.Min(statsContent.sizeDelta.y + 10f, 272f));
		runStatsExtraInfoLabel.gameObject.SetActive(false);
	}
	
	public void CustomClicked()
	{
		if(customButton.specialState)
		{
			return;
		}
		dailyButton.ChangeSpecialState(false);
		standardButton.ChangeSpecialState(false);
		customButton.ChangeSpecialState(true);
		viewGamesButton.ChangeButtonEnabled(oldCustomGames.Count > 0);
		if(statsButton.specialState)
		{
			SetupCustomStats();
		}
		else if(decksButton.specialState)
		{
			StatsClicked();
		}
		decksButton.ChangeButtonEnabled(false);
		viewGamesButton.ChangeButtonEnabled(oldCustomGames.Count > 0);
	}
	
	public void StatsClicked()
	{
		if(statsButton.specialState)
		{
			return;
		}
		statsButton.ChangeSpecialState(true);
		decksButton.ChangeSpecialState(false);
		statsVisibilityObject.SetActive(true);
		decksVisibilityObject.SetActive(false);
		runStatsPanel.gameObject.SetActive(false);
		if(standardButton.specialState)
		{
			SetupStandardStats();
		}
		else if(customButton.specialState)
		{
			SetupCustomStats();
		}
		else if(dailyButton.specialState)
		{
			SetupDailyStats();
		}
	}
	
	public void DecksClicked()
	{
		statsButton.ChangeSpecialState(false);
		decksButton.ChangeSpecialState(true);
		statsVisibilityObject.SetActive(false);
		decksVisibilityObject.SetActive(true);
		runStatsPanel.gameObject.SetActive(false);
	}
	
	public string GetSaveParameter(string saveGameString, string parameter)
	{
		string[] saveGameLines = saveGameString.Split('\n');
		if(saveGameLines == null)
		{
			return null;
		}
		for(int i = 0; i < saveGameLines.Length; i++)
		{
			string[] saveGameLineData = saveGameLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(saveGameLineData.Length > 1)
			{
				if(saveGameLineData[0].Trim() == parameter)
				{
					return saveGameLineData[1].Trim();
				}
			}
		}
		return null;
	}
	
	public bool GetWhetherGameWasWonFromSaveString(string saveGameString)
	{
		int currentRound = int.Parse(GetSaveParameter(saveGameString, "currentRound"));
		if(currentRound >= 30)
		{
			return true;
		}
		else if(currentRound <= 28)
		{
			return false;
		}
		else
		{
			double currentRoundScore = double.Parse(GetSaveParameter(saveGameString, "currentRoundScore"));
			string variantRoundsString = GetSaveParameter(saveGameString, "variantRounds");
			if(variantRoundsString == null || variantRoundsString.Length == 0)
			{
				return currentRoundScore >= LocalInterface.instance.baseVariant.variantRounds[29].scoreNeeded;
			}
			string[] variantRounds = variantRoundsString.Split('%', StringSplitOptions.RemoveEmptyEntries);
			if(variantRounds.Length == 0)
			{
				return currentRoundScore >= LocalInterface.instance.baseVariant.variantRounds[29].scoreNeeded;
			}
			else
			{
				for(int i = 0; i < variantRounds.Length; i++)
				{
					string[] variantRoundData = variantRounds[i].Split('#');
					if(variantRoundData.Length > 1)
					{
						if(int.Parse(variantRoundData[0]) == 29)
						{
							return currentRoundScore >= double.Parse(variantRoundData[1]);
						}
					}
				}
			}
			return currentRoundScore >= LocalInterface.instance.baseVariant.variantRounds[29].scoreNeeded;
		}
	}
	
	public void ViewGamesClicked()
	{
		statsVisibilityObject.SetActive(false);
		decksVisibilityObject.SetActive(false);
		gameTypeObject.SetActive(false);
		subGroupObject.SetActive(false);
		runStatsPanel.gameObject.SetActive(true);
		if(standardButton.specialState)
		{
			currentRunIndex = oldStandardGames.Count - 1;
			UpdateRunStatsPanelForSaveString(oldStandardGames[currentRunIndex]);
			if(oldStandardGames.Count > 1)
			{
				previousRunButton.ChangeButtonEnabled(true);
				nextRunButton.ChangeButtonEnabled(true);
			}
			else
			{
				previousRunButton.ChangeButtonEnabled(false);
				nextRunButton.ChangeButtonEnabled(false);
			}
		}
		else if(dailyButton.specialState)
		{
			currentRunIndex = oldDailyGames.Count - 1;
			UpdateRunStatsPanelForSaveString(oldDailyGames[currentRunIndex]);
			if(oldDailyGames.Count > 1)
			{
				previousRunButton.ChangeButtonEnabled(true);
				nextRunButton.ChangeButtonEnabled(true);
			}
			else
			{
				previousRunButton.ChangeButtonEnabled(false);
				nextRunButton.ChangeButtonEnabled(false);
			}
		}
		else if(customButton.specialState)
		{
			currentRunIndex = oldCustomGames.Count - 1;
			UpdateRunStatsPanelForSaveString(oldCustomGames[currentRunIndex]);
			if(oldCustomGames.Count > 1)
			{
				previousRunButton.ChangeButtonEnabled(true);
				nextRunButton.ChangeButtonEnabled(true);
			}
			else
			{
				previousRunButton.ChangeButtonEnabled(false);
				nextRunButton.ChangeButtonEnabled(false);
			}
		}
		optionsControllerSelectionGroup.RemoveFromCurrentGroups();
		statsControllerSelectionGroup.RemoveFromCurrentGroups();
		runStatsPanelControllerSelectionGroup.AddToCurrentGroups();
	}
	
	public void UpdateRunStatsPanelForSaveString(string saveGameString)
	{
		runStatsPanel.UpdateFromSaveFile(saveGameString);
		bool gameWasWon = GetWhetherGameWasWonFromSaveString(saveGameString);
		bool isDailyGame = bool.Parse(GetSaveParameter(saveGameString, "isDailyGame"));
		bool isCustomGame = bool.Parse(GetSaveParameter(saveGameString, "isCustomGame"));
		DateTime dateTimeStarted = DateTime.Parse(GetSaveParameter(saveGameString, "dateTimeStarted"));
		if(isDailyGame)
		{
			if(gameWasWon)
			{
				runStatsPanel.titleLabel.ChangeText($"{dateTimeStarted.ToString("d MMMM yyyy")} Completed Daily Game");
			}
			else
			{
				runStatsPanel.titleLabel.ChangeText($"{dateTimeStarted.ToString("d MMMM yyyy")} Failed Daily Game");
			}
		}
		else if(isCustomGame)
		{
			if(gameWasWon)
			{
				runStatsPanel.titleLabel.ChangeText("Completed Custom Game");
			}
			else
			{
				runStatsPanel.titleLabel.ChangeText("Failed Custom Game");
			}
		}
		else
		{
			if(gameWasWon)
			{
				runStatsPanel.titleLabel.ChangeText("Completed Game");
			}
			else
			{
				runStatsPanel.titleLabel.ChangeText("Failed Game");
			}
		}
		string[] loadGameData = saveGameString.Split('\n');
		int dataIndex = 0;
		if(loadGameData[dataIndex] != LocalInterface.instance.saveGameFileVersion)
		{
			LocalInterface.instance.DisplayError($"Unsupported version mismatch in save game file, current version={LocalInterface.instance.saveGameFileVersion}, file version={loadGameData[dataIndex]}");
			return;
		}
		dataIndex++;
		string variantData = string.Empty;
		int variantStartDataIndex = dataIndex;
		for(int i = variantStartDataIndex; i < variantStartDataIndex + 6; i++)
		{
			variantData += loadGameData[dataIndex];
			dataIndex++;
			if(dataIndex < variantStartDataIndex + 6)
			{
				variantData += "\n";
			}
		}
		currentVariantString = variantData;
		Variant currentVariant = new Variant(variantData);
		currentVariantName = currentVariant.variantName;
		if(isDailyGame || isCustomGame)
		{
			if(VariantsMenu.instance.GetVariantFile(currentVariant.variantName) == null)
			{
				saveVariantButtonBackdropObject.SetActive(true);
			}
			else
			{
				saveVariantButtonBackdropObject.SetActive(false);
			}
		}
		else
		{
			saveVariantButtonBackdropObject.SetActive(false);
		}
		runStatsPanel.variantSimple.UpdateVariantSimpleForVariant(currentVariant);
	}
	
	public void BackClicked()
	{
		if(runStatsPanel.gameObject.activeSelf)
		{
			if(statsButton.specialState)
			{
				statsVisibilityObject.SetActive(true);
			}
			if(decksButton.specialState)
			{
				decksVisibilityObject.SetActive(true);
			}
			gameTypeObject.SetActive(true);
			subGroupObject.SetActive(true);
			runStatsPanel.gameObject.SetActive(false);
			optionsControllerSelectionGroup.AddToCurrentGroups();
			statsControllerSelectionGroup.AddToCurrentGroups();
			runStatsPanelControllerSelectionGroup.RemoveFromCurrentGroups();
		}
		else
		{
			MovingObjects.instance.mo["MainMenu"].StartMove("OnScreen");
			MovingObjects.instance.mo["Version"].StartMove("OnScreen");
			MovingObjects.instance.mo["SelfPromotion"].StartMove("OnScreen");
			MovingObjects.instance.mo["Title"].StartMove("OnScreen");
			MovingObjects.instance.mo["StatsMenu"].StartMove("OffScreen");
			MovingObjects.instance.mo["ExitButton"].StartMove("OnScreen");
		}
	}
	
	public void PreviousRunClicked()
	{
		currentRunIndex--;
		if(standardButton.specialState)
		{
			if(currentRunIndex < 0)
			{
				currentRunIndex = oldStandardGames.Count - 1;
			}
			UpdateRunStatsPanelForSaveString(oldStandardGames[currentRunIndex]);
		}
		else if(dailyButton.specialState)
		{
			if(currentRunIndex < 0)
			{
				currentRunIndex = oldDailyGames.Count - 1;
			}
			UpdateRunStatsPanelForSaveString(oldDailyGames[currentRunIndex]);
		}
		else if(customButton.specialState)
		{
			if(currentRunIndex < 0)
			{
				currentRunIndex = oldCustomGames.Count - 1;
			}
			UpdateRunStatsPanelForSaveString(oldCustomGames[currentRunIndex]);
		}
	}
	
	public void NextRunClicked()
	{
		currentRunIndex++;
		if(standardButton.specialState)
		{
			if(currentRunIndex >= oldStandardGames.Count)
			{
				currentRunIndex = 0;
			}
			UpdateRunStatsPanelForSaveString(oldStandardGames[currentRunIndex]);
		}
		else if(dailyButton.specialState)
		{
			if(currentRunIndex >= oldDailyGames.Count)
			{
				currentRunIndex = 0;
			}
			UpdateRunStatsPanelForSaveString(oldDailyGames[currentRunIndex]);
		}
		else if(customButton.specialState)
		{
			if(currentRunIndex >= oldCustomGames.Count)
			{
				currentRunIndex = 0;
			}
			UpdateRunStatsPanelForSaveString(oldCustomGames[currentRunIndex]);
		}
	}
	
	public void SaveVariantClicked()
	{
		VariantsMenu.instance.SaveVariantToFile(currentVariantString, currentVariantName);
		saveVariantButton.ChangeButtonEnabled(false);
		MinorNotifications.instance.NewMinorNotification("Saved!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(saveVariantButton.rt, LocalInterface.instance.mainMenuCanvas));
	}
}
