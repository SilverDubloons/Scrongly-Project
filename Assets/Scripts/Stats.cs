using UnityEngine;
using System;
using System.Collections.Generic;
using static Decks;
using static Deck;

public class Stats : MonoBehaviour
{
    public string statsFileName;
	public string statsFileVersion;
	
	public static Stats instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void CheckForStatsFile()
	{
		if(!LocalInterface.instance.DoesFileExist(statsFileName))
		{
			ResetStatsFile();
		}
		string[] statsLines = LocalInterface.instance.GetFileTextLines(statsFileName);
		if(statsLines == null)
		{
			ResetStatsFile();
		}
		else
		{
			if(statsLines[0].Trim() != statsFileVersion)
			{
				LocalInterface.instance.DisplayError($"Unsupported stats file version mismatch. Your version={statsLines[0]}, current version={statsFileVersion}");
				ResetStatsFile();
			}
		}
	}
	
	public Dictionary<string, int> deckLevelsCompleted = new Dictionary<string, int>();
	
	public void ResetStatsFile()
	{
		string newStatFile = string.Empty;
		
		int chipsEarned = 0;
		int chipThresholdsCleared = 0;
		int cardsAddedToDeck = 0;
		int baublesGained = 0;
		int zodiacsGained = 0;
		int numberedCardsScoredWithBauble = 0;
		int faceCardsScoredWithBauble = 0;
		int acesScoredWithBauble = 0;
		int handsPlayed = 0;
		int handsPlayedWhileFatigued = 0;
		int discardsUsed = 0;
		int cardsDiscarded = 0;
		int standardRunsPlayed = 0;
		int standardRunsCompleted = 0;
		int dailyRunsPlayed = 0;
		int dailyCompleted = 0;
		int customRunsPlayed = 0;
		int customRunsCompleted = 0;
		int highestRoundReachedStandard = 0;
		double scoreInHighestRoundReachedStandard = 0;
		int highestScoringHandInStandardTier = 0;
		double highestScoringHandInStandardPoints = 0;
		string highestScoringHandInStandardCardDatas = "none";
		int sevensScored = 0;
		int threeOfAKindsDiscarded = 0;
		int mushroomsPlayed = 0;
		int fiveOfAKindsPlayed = 0;
		foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			deckLevelsCompleted.Add(entry.Key, -1);
		}
		
		string[] statsLines = LocalInterface.instance.GetFileTextLines(statsFileName);
		if(statsLines != null)
		{
			for(int i = 0; i < statsLines.Length; i++)
			{
				string[] statLineData = statsLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
				if(statLineData.Length > 1)
				{
					switch(statLineData[0])
					{
						case("chipsEarned"):
							chipsEarned = int.Parse(statLineData[1]);
						break;
						case("chipThresholdsCleared"):
							chipThresholdsCleared = int.Parse(statLineData[1]);
						break;
						case("cardsAddedToDeck"):
							cardsAddedToDeck = int.Parse(statLineData[1]);
						break;
						case("baublesGained"):
							baublesGained = int.Parse(statLineData[1]);
						break;
						case("zodiacsGained"):
							zodiacsGained = int.Parse(statLineData[1]);
						break;
						case("numberedCardsScoredWithBauble"):
							numberedCardsScoredWithBauble = int.Parse(statLineData[1]);
						break;
						case("faceCardsScoredWithBauble"):
							faceCardsScoredWithBauble = int.Parse(statLineData[1]);
						break;
						case("acesScoredWithBauble"):
							acesScoredWithBauble = int.Parse(statLineData[1]);
						break;
						case("handsPlayed"):
							handsPlayed = int.Parse(statLineData[1]);
						break;
						case("handsPlayedWhileFatigued"):
							handsPlayedWhileFatigued = int.Parse(statLineData[1]);
						break;
						case("discardsUsed"):
							discardsUsed = int.Parse(statLineData[1]);
						break;
						case("cardsDiscarded"):
							cardsDiscarded = int.Parse(statLineData[1]);
						break;
						case("standardRunsPlayed"):
							standardRunsPlayed = int.Parse(statLineData[1]);
						break;
						case("standardRunsCompleted"):
							standardRunsCompleted = int.Parse(statLineData[1]);
						break;
						case("dailyRunsPlayed"):
							dailyRunsPlayed = int.Parse(statLineData[1]);
						break;
						case("dailyCompleted"):
							dailyCompleted = int.Parse(statLineData[1]);
						break;
						case("customRunsPlayed"):
							customRunsPlayed = int.Parse(statLineData[1]);
						break;
						case("customRunsCompleted"):
							customRunsCompleted = int.Parse(statLineData[1]);
						break;
						case("highestRoundReachedStandard"):
							highestRoundReachedStandard = int.Parse(statLineData[1]);
						break;
						case("scoreInHighestRoundReachedStandard"):
							scoreInHighestRoundReachedStandard = double.Parse(statLineData[1]);
						break;
						case("highestScoringHandInStandardTier"):
							highestScoringHandInStandardTier = int.Parse(statLineData[1]);
						break;
						case("highestScoringHandInStandardPoints"):
							highestScoringHandInStandardPoints = double.Parse(statLineData[1]);
						break;
						case("highestScoringHandInStandardCardDatas"):
							highestScoringHandInStandardCardDatas = statLineData[1];
						break;
						case("sevensScored"):
							sevensScored = int.Parse(statLineData[1]);
						break;
						case("threeOfAKindsDiscarded"):
							threeOfAKindsDiscarded = int.Parse(statLineData[1]);
						break;
						case("mushroomsPlayed"):
							mushroomsPlayed = int.Parse(statLineData[1]);
						break;
						case("fiveOfAKindsPlayed"):
							fiveOfAKindsPlayed = int.Parse(statLineData[1]);
						break;
						default:
							if(statLineData[0].Length > 30)
							{
								if(statLineData[0].Substring(0, 30) == "highestDifficultyCompletedWith")
								{
									string deckName = statLineData[0].Substring(30);
									deckLevelsCompleted[deckName] = int.Parse(statLineData[1]);
								}
							}
						break;
					}
				}
			}
		}
		newStatFile += statsFileVersion;
		newStatFile += $"\nchipsEarned={chipsEarned}";
		newStatFile += $"\nchipThresholdsCleared={chipThresholdsCleared}";
		newStatFile += $"\ncardsAddedToDeck={cardsAddedToDeck}";
		newStatFile += $"\nbaublesGained={baublesGained}";
		newStatFile += $"\nzodiacsGained={zodiacsGained}";
		newStatFile += $"\nnumberedCardsScoredWithBauble={numberedCardsScoredWithBauble}";
		newStatFile += $"\nfaceCardsScoredWithBauble={faceCardsScoredWithBauble}";
		newStatFile += $"\nacesScoredWithBauble={acesScoredWithBauble}";
		newStatFile += $"\nhandsPlayed={handsPlayed}";
		newStatFile += $"\nhandsPlayedWhileFatigued={handsPlayedWhileFatigued}";
		newStatFile += $"\ndiscardsUsed={discardsUsed}";
		newStatFile += $"\ncardsDiscarded={cardsDiscarded}";
		newStatFile += $"\nstandardRunsPlayed={standardRunsPlayed}";
		newStatFile += $"\nstandardRunsCompleted={standardRunsCompleted}";
		newStatFile += $"\ndailyRunsPlayed={dailyRunsPlayed}";
		newStatFile += $"\ndailyCompleted={dailyCompleted}";
		newStatFile += $"\ncustomRunsPlayed={customRunsPlayed}";
		newStatFile += $"\ncustomRunsCompleted={customRunsCompleted}";
		newStatFile += $"\nhighestRoundReachedStandard={highestRoundReachedStandard}";
		newStatFile += $"\nscoreInHighestRoundReachedStandard={scoreInHighestRoundReachedStandard}";
		newStatFile += $"\nhighestScoringHandInStandardTier={highestScoringHandInStandardTier}";
		newStatFile += $"\nhighestScoringHandInStandardPoints={highestScoringHandInStandardPoints}";
		newStatFile += $"\nhighestScoringHandInStandardCardDatas={highestScoringHandInStandardCardDatas}";
		newStatFile += $"\nsevensScored={sevensScored}";
		newStatFile += $"\nthreeOfAKindsDiscarded={threeOfAKindsDiscarded}";
		newStatFile += $"\nmushroomsPlayed={mushroomsPlayed}";
		newStatFile += $"\nfiveOfAKindsPlayed={fiveOfAKindsPlayed}";
		/* foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			newStatFile += $"\nhighestDifficultyCompletedWith{entry.Value.deckName}=-1";
		} */
		foreach(KeyValuePair<string, int> entry in deckLevelsCompleted)
		{
			newStatFile += $"\nhighestDifficultyCompletedWith{entry.Key}={entry.Value}";
		}
		LocalInterface.instance.SetFileText(statsFileName, newStatFile);
	}
	
	public string GetHighestDifficultyCompletedDeckParameter(string deckName)
	{
		return $"highestDifficultyCompletedWith{deckName}";
	}
	
	public int GetHighestDifficultyCompleted()
	{
		int highestDifficulty = -1;
		foreach(KeyValuePair<string, Decks.Deck> entry in Decks.instance.decks)
		{
			int nextDeckHighestDifficulty = GetStatInt(GetHighestDifficultyCompletedDeckParameter(entry.Value.deckName));
			if(nextDeckHighestDifficulty > highestDifficulty)
			{
				highestDifficulty = nextDeckHighestDifficulty;
			}
		}
		return highestDifficulty;
	}
	
	public int GetStatInt(string parameter)
	{
		string[] statsLines = LocalInterface.instance.GetFileTextLines(statsFileName);
		if(statsLines == null)
		{
			// LocalInterface.instance.DisplayError($"GetStatInt called with parameter {parameter} but statsLines was null");
			ResetStatsFile();
			return -9001;
		}
		for(int i = 0; i < statsLines.Length; i++)
		{
			string[] statLineData = statsLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(statLineData.Length > 1)
			{
				if(statLineData[0].Trim() == parameter)
				{
					return int.Parse(statLineData[1].Trim());
				}
			}
		}
		// LocalInterface.instance.DisplayError($"GetStatInt called with parameter {parameter} which doesn't exist in the file");
		ResetStatsFile();
		return -9001;
	}
	
	public double GetStatDouble(string parameter)
	{
		string[] statsLines = LocalInterface.instance.GetFileTextLines(statsFileName);
		if(statsLines == null)
		{
			// LocalInterface.instance.DisplayError($"GetStatDouble called with parameter {parameter} but statsLines was null");
			ResetStatsFile();
			return -9001;
		}
		for(int i = 0; i < statsLines.Length; i++)
		{
			string[] statLineData = statsLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(statLineData.Length > 1)
			{
				if(statLineData[0].Trim() == parameter)
				{
					return double.Parse(statLineData[1].Trim());
				}
			}
		}
		// LocalInterface.instance.DisplayError($"GetStatDouble called with parameter {parameter} which doesn't exist in the file");
		ResetStatsFile();
		return -9001;
	}
	
	public void AdjustStatInt(string parameter, int adjustment)
	{
		string[] statsLines = LocalInterface.instance.GetFileTextLines(statsFileName);
		string newStatsLines = string.Empty;
		if(statsLines == null)
		{
			return;
		}
		for(int i = 0; i < statsLines.Length; i++)
		{
			string[] statLineData = statsLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(statLineData.Length > 1)
			{
				if(statLineData[0].Trim() == parameter)
				{
					int statLineInteger = int.Parse(statLineData[1].Trim());
					newStatsLines += $"{statLineData[0]}={statLineInteger + adjustment}\n";
				}
				else
				{
					newStatsLines += $"{statsLines[i]}\n";
				}
			}
			else
			{
				newStatsLines += $"{statsLines[i]}\n";
			}
		}
		LocalInterface.instance.SetFileText(statsFileName, newStatsLines);
	}
	
	public void SetStat(string parameter, double newValue)
	{
		SetStat(parameter, newValue.ToString());
	}
	
	public void SetStat(string parameter, int newValue)
	{
		SetStat(parameter, newValue.ToString());
	}
	
	public void SetStat(string parameter, string newValue)
	{
		string[] statsLines = LocalInterface.instance.GetFileTextLines(statsFileName);
		string newStatsLines = string.Empty;
		if(statsLines == null)
		{
			return;
		}
		for(int i = 0; i < statsLines.Length; i++)
		{
			string[] statLineData = statsLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(statLineData.Length > 1)
			{
				if(statLineData[0].Trim() == parameter)
				{
					newStatsLines += $"{statLineData[0]}={newValue}\n";
				}
				else
				{
					newStatsLines += $"{statsLines[i]}\n";
				}
			}
			else
			{
				newStatsLines += $"{statsLines[i]}\n";
			}
		}
		LocalInterface.instance.SetFileText(statsFileName, newStatsLines);
	}
	
	public void GameCompleted()	// triggers when game is over
	{
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(GameManager.instance.currentRound >= GetStatInt("highestRoundReachedStandard"))
			{
				if(GameManager.instance.currentRound > GetStatInt("highestRoundReachedStandard"))
				{
					SetStat("highestRoundReachedStandard", GameManager.instance.currentRound);
					SetStat("scoreInHighestRoundReachedStandard", ScoreVial.instance.currentRoundScore.ToString());
				}
				else
				{
					if(ScoreVial.instance.currentRoundScore > GetStatDouble("scoreInHighestRoundReachedStandard"))
					{
						SetStat("scoreInHighestRoundReachedStandard", ScoreVial.instance.currentRoundScore.ToString());
					}
				}
			}
		}
		else if(V.i.isDailyGame)
		{
			
		}
		else if(V.i.isCustomGame)
		{
			
		}
	}
	
	public void RunWon() // triggers on completing round 29
	{
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			AdjustStatInt("standardRunsCompleted", 1);
			if(GetStatInt(GetHighestDifficultyCompletedDeckParameter(V.i.chosenDeck)) < V.i.currentDifficulty)
			{
				SetStat(GetHighestDifficultyCompletedDeckParameter(V.i.chosenDeck), V.i.currentDifficulty);
			}
		}
		else if(V.i.isDailyGame)
		{
			AdjustStatInt("dailyRunsCompleted", 1);
		}
		else if(V.i.isCustomGame)
		{
			AdjustStatInt("customRunsCompleted", 1);
		}
	}
	
	public void HandCompleted(double handScore, int handTier, List<CardData> cardDatas)
	{
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(handScore > GetStatDouble("highestScoringHandInStandardPoints"))
			{
				SetStat("highestScoringHandInStandardPoints", handScore);
				SetStat("highestScoringHandInStandardTier", handTier);
				string cardDatasString = string.Empty;
				for(int i = 0; i < cardDatas.Count; i++)
				{
					cardDatasString += cardDatas[i].ConvertToText();
					if(i < cardDatas.Count - 1)
					{
						cardDatasString += "%";
					}
				}
				SetStat("highestScoringHandInStandardCardDatas", cardDatasString);
			}
			if(GameManager.instance.currentRound >= GetStatInt("highestRoundReachedStandard"))
			{
				if(GameManager.instance.currentRound > GetStatInt("highestRoundReachedStandard"))
				{
					SetStat("highestRoundReachedStandard", GameManager.instance.currentRound);
					SetStat("scoreInHighestRoundReachedStandard", ScoreVial.instance.currentRoundScore);
				}
				else
				{
					if(ScoreVial.instance.currentRoundScore > GetStatDouble("scoreInHighestRoundReachedStandard"))
					{
						SetStat("scoreInHighestRoundReachedStandard", ScoreVial.instance.currentRoundScore);
					}
				}
			}
			if(!LocalInterface.instance.IsBaubleUnlocked("GetMushroomsFromHands") && (handTier >= 16 || handTier == 13 || handTier == 9))
			{
				AdjustStatIntIfStandardGame("fiveOfAKindsPlayed", 1);
				if(GetStatInt("fiveOfAKindsPlayed") >= 20)
				{
					LocalInterface.instance.UnlockBauble("GetMushroomsFromHands");
					ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["GetMushroomsFromHands"].howToUnlock, "UnlockedBauble", "GetMushroomsFromHands", V.i.v.variantBaubles["GetMushroomsFromHands"].sprite);
				}
			}
		}
		AdjustStatIntIfStandardGame("handsPlayed", 1);
		if(GameManager.instance.IsPlayerFatigued())
		{
			AdjustStatIntIfStandardGame("handsPlayedWhileFatigued", 1);
		}
	}
	
	public void AdjustStatIntIfStandardGame(string parameter, int change)
	{
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			AdjustStatInt(parameter, change);
		}
	}
	
	public void CurrencyEarned(int change)
	{
		AdjustStatIntIfStandardGame("chipsEarned", change);
	}
	
	public void ChipThresholdCleared()
	{
		AdjustStatIntIfStandardGame("chipThresholdsCleared", 1);
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(!LocalInterface.instance.IsBaubleUnlocked("IncreaseChipThresholds") && GetStatInt("chipThresholdsCleared") >= 150)
			{
				LocalInterface.instance.UnlockBauble("IncreaseChipThresholds");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["IncreaseChipThresholds"].howToUnlock, "UnlockedBauble", "IncreaseChipThresholds", V.i.v.variantBaubles["IncreaseChipThresholds"].sprite);
			}
		}
	}
	
	public void CardAddedToDeck()
	{
		AdjustStatIntIfStandardGame("cardsAddedToDeck", 1);
	}
	
	public void BaubleGained()
	{
		AdjustStatIntIfStandardGame("baublesGained", 1);
	}
	
	public void ZodiacGained()
	{
		AdjustStatIntIfStandardGame("zodiacsGained", 1);
	}
	
	public void NumberedCardScoredWithBauble()
	{
		AdjustStatIntIfStandardGame("numberedCardsScoredWithBauble", 1);
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(!LocalInterface.instance.IsBaubleUnlocked("AllCardsAreNumberedCards") && GetStatInt("numberedCardsScoredWithBauble") >= 300)
			{
				LocalInterface.instance.UnlockBauble("AllCardsAreNumberedCards");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["AllCardsAreNumberedCards"].howToUnlock, "UnlockedBauble", "AllCardsAreNumberedCards", V.i.v.variantBaubles["AllCardsAreNumberedCards"].sprite);
			}
		}
	}
	
	public void FaceCardScoredWithBauble()
	{
		AdjustStatIntIfStandardGame("faceCardsScoredWithBauble", 1);
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(!LocalInterface.instance.IsBaubleUnlocked("AllCardsAreFaceCards") && GetStatInt("faceCardsScoredWithBauble") >= 50)
			{
				LocalInterface.instance.UnlockBauble("AllCardsAreFaceCards");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["AllCardsAreFaceCards"].howToUnlock, "UnlockedBauble", "AllCardsAreFaceCards", V.i.v.variantBaubles["AllCardsAreFaceCards"].sprite);
			}
		}
	}
	
	public void AceScoredWithBauble()
	{
		AdjustStatIntIfStandardGame("acesScoredWithBauble", 1);
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(!LocalInterface.instance.IsBaubleUnlocked("AllCardsAreAces") && GetStatInt("acesScoredWithBauble") >= 50)
			{
				LocalInterface.instance.UnlockBauble("AllCardsAreAces");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["AllCardsAreAces"].howToUnlock, "UnlockedBauble", "AllCardsAreAces", V.i.v.variantBaubles["AllCardsAreAces"].sprite);
			}
		}
	}
	
	public void SevenScored()
	{
		AdjustStatIntIfStandardGame("sevensScored", 1);
		if(!V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(!LocalInterface.instance.IsSpecialCardUnlocked("SlotMachine") && GetStatInt("sevensScored") >= 50)
			{
				LocalInterface.instance.UnlockSpecialCard("SlotMachine");
			}
		}
	}
	
	public void DiscardUsed(int cardsDiscarded)
	{
		AdjustStatIntIfStandardGame("discardsUsed", 1);
		AdjustStatIntIfStandardGame("cardsDiscarded", cardsDiscarded);
	}
	
	public void ThreeOfAKindDiscarded()
	{
		if(!LocalInterface.instance.IsBaubleUnlocked("DiscardTriplesForMushrooms"))
		{
			AdjustStatIntIfStandardGame("threeOfAKindsDiscarded", 1);
			if(GetStatInt("threeOfAKindsDiscarded") >= 10)
			{
				LocalInterface.instance.UnlockBauble("DiscardTriplesForMushrooms");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["DiscardTriplesForMushrooms"].howToUnlock, "UnlockedBauble", "DiscardTriplesForMushrooms", V.i.v.variantBaubles["DiscardTriplesForMushrooms"].sprite);
			}
		}
	}
	
	public void MushroomsPlayed(int numberOfMushrooms)
	{
		if(!LocalInterface.instance.IsBaubleUnlocked("IncreaseMushroomPowerTriples"))
		{
			AdjustStatIntIfStandardGame("mushroomsPlayed", numberOfMushrooms);
			if(GetStatInt("mushroomsPlayed") >= 15)
			{
				LocalInterface.instance.UnlockBauble("IncreaseMushroomPowerTriples");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["IncreaseMushroomPowerTriples"].howToUnlock, "UnlockedBauble", "IncreaseMushroomPowerTriples", V.i.v.variantBaubles["IncreaseMushroomPowerTriples"].sprite);
			}
		}
	}
}