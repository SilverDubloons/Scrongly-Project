using UnityEngine;
using System.Collections.Generic;
using static Deck;
using System;

public class RunInformation : MonoBehaviour
{
	// public string saveGameFileVersion;
/* 	public string standardGameSaveFileName;
	public string dailyGameSaveFileName;
	public string customGameSaveFileName; */
	
    public int[] handsPlayed;
	// public int[] zodiacsEarned;
	public int chipsEarned;
	public int cardsDiscarded;
	public int cardsAddedToDeck;
	public int handsPlayedInFatigue;
	public int consecutiveHandsWithMagnetClearingAllChipThresholds;
	public List<CardData> standardCardDatasInLastHand = new List<CardData>();
	public double highestHandScore;
	public int highestHandTier;
	public List<CardData> highestHandScoreCardDatas = new List<CardData>();
	
	public static RunInformation instance;
	
	public void HandPlayed(int handNumber, bool[] handsContained, bool inFatigue)
	{
		handsPlayed[handNumber]++;
		if(handNumber > 8 && handsPlayed[handNumber] == 1)
		{
			HandsInformation.instance.HandUpdated(handsContained);
			if(!V.i.v.variantSpecialOptions["AllBaublesZodiacsUnlocked"].inEffect)
			{
				Shop.instance.AddBaubleToAvailablePool(LocalInterface.instance.GetMultiplierBaubleStringFromHandTier(handNumber));
				Shop.instance.AddZodiacToAvailablePool(LocalInterface.instance.GetZodiacBaubleStringFromHandTier(handNumber));
			}
			CheckForScastronomyDeckUnlock();
		}
		if(inFatigue)
		{
			handsPlayedInFatigue++;
			if(!Decks.instance.decks["Sleepy"].unlocked && !V.i.isDailyGame && !V.i.isCustomGame && handsPlayedInFatigue >= 10)
			{
				Decks.instance.UnlockDeck("Sleepy");
			}
		}
	}
	
	public void CheckForScastronomyDeckUnlock()
	{
		if(!Decks.instance.decks["Scastronomy"].unlocked && !V.i.isDailyGame && !V.i.isCustomGame)
		{
			for(int i = 9; i <=17; i++)
			{
				// Debug.Log($"handsPlayed[{i}]={handsPlayed[i]}");
				if(handsPlayed[i] <= 0)
				{
					return;
				}
			}
			// Decks.instance.DisplayAllDeckData();
			Decks.instance.UnlockDeck("Scastronomy");
		}
	}
	
	public void AllChipThresholdsClearedWithMagnet()
	{
		consecutiveHandsWithMagnetClearingAllChipThresholds++;
		if(!Decks.instance.decks["Sgambler"].unlocked && !V.i.isDailyGame && !V.i.isCustomGame && consecutiveHandsWithMagnetClearingAllChipThresholds >= 2)
		{
			Decks.instance.UnlockDeck("Sgambler");
		}
	}
	
	public void HandFinished(double handScore, int handTier)
	{
		if(handScore > highestHandScore)
		{
			highestHandScoreCardDatas.Clear();
			highestHandScoreCardDatas = new List<CardData>(standardCardDatasInLastHand);
			highestHandTier = handTier;
			highestHandScore = handScore;
		}
		Stats.instance.HandCompleted(handScore, handTier, new List<CardData>(standardCardDatasInLastHand));
	}
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetupRunInformationNewGame()
	{
		// remember to add functionality for loading game
		handsPlayed = new int[18];
		// zodiacsEarned = new int[18];
		chipsEarned = 0;
		cardsDiscarded = 0;
		cardsAddedToDeck = 0;
		highestHandScore = 0;
		handsPlayedInFatigue = 0;
		consecutiveHandsWithMagnetClearingAllChipThresholds = 0;
		highestHandTier = -1;
	}
	
	public string CreateSaveGameString()
	{
		string saveGameString = LocalInterface.instance.saveGameFileVersion;
		saveGameString += "\n";
		saveGameString += V.i.v.ConvertToText();
		saveGameString += "\ndrawPile=";
		for(int i = 0; i < Deck.instance.drawPile.Count; i++)
		{
			saveGameString += Deck.instance.drawPile[i].ConvertToText();
			if(i < Deck.instance.drawPile.Count - 1)
			{
				saveGameString += "%";
			}
		}
		saveGameString += "\ncardsInHand=";
		for(int i = 0; i < Deck.instance.cardsInHand.Count; i++)
		{
			saveGameString += Deck.instance.cardsInHand[i].cardData.ConvertToText();
			if(i < Deck.instance.cardsInHand.Count - 1)
			{
				saveGameString += "%";
			}
		}
		saveGameString += "\ndiscardPile=";
		for(int i = 0; i < Deck.instance.discardPile.Count; i++)
		{
			saveGameString += Deck.instance.discardPile[i].ConvertToText();
			if(i < Deck.instance.discardPile.Count - 1)
			{
				saveGameString += "%";
			}
		}
		if(HandArea.instance.topDeckCard == null)
		{
			saveGameString += "\ntopDeckCard=null";
		}
		else
		{
			saveGameString += $"\ntopDeckCard={HandArea.instance.topDeckCard.cardData.ConvertToText()}";
		}
		saveGameString += $"\ncurrency={GameManager.instance.currency}";
		saveGameString += $"\nhandsUntilFatigueRemaining={GameManager.instance.handsUntilFatigueRemaining}";
		saveGameString += $"\ndiscardsRemaining={GameManager.instance.discardsRemaining}";
		saveGameString += $"\ncurrentRound={GameManager.instance.currentRound}";
		saveGameString += $"\ncurrentRoundScore={ScoreVial.instance.currentRoundScore}";
		saveGameString += $"\nchipThresholds=";
		for(int i = 0; i < ScoreVial.instance.chipThresholds.Count; i++)
		{
			saveGameString += ScoreVial.instance.chipThresholds[i].ConvertToText();
			if(i < ScoreVial.instance.chipThresholds.Count - 1)
			{
				saveGameString += "%";
			}
		}
		saveGameString += $"\nchosenDeck={V.i.chosenDeck}";
		saveGameString += $"\nownedBaubles={Baubles.instance.ConvertOwnedBaublesToString()}";
		saveGameString += $"\ndisabledBaubles={Baubles.instance.ConvertDisabledBaublesToString()}";
		saveGameString += $"\nseed={V.i.seed}";
		saveGameString += $"\ncallCounts={RNG.instance.shuffle.GetCurrentCallCount()}%{RNG.instance.shop.GetCurrentCallCount()}%{RNG.instance.starting.GetCurrentCallCount()}%{RNG.instance.hands.GetCurrentCallCount()}%{RNG.instance.misc.GetCurrentCallCount()}";
		saveGameString += $"\nisDailyGame={V.i.isDailyGame}";
		saveGameString += $"\nisCustomGame={V.i.isCustomGame}";
		saveGameString += $"\nchipsEarned={chipsEarned}";
		saveGameString += $"\nhandsPlayed=";
		for(int i = 0; i < handsPlayed.Length; i++)
		{
			saveGameString += handsPlayed[i].ToString();
			if(i < handsPlayed.Length - 1)
			{
				saveGameString += "%";
			}
		}
		saveGameString += $"\ncardsDiscarded={cardsDiscarded}";
		saveGameString += $"\ncardsAddedToDeck={cardsAddedToDeck}";
		saveGameString += $"\nhighestHandScore={highestHandScore}";
		saveGameString += $"\nhighestHandTier={highestHandTier}";
		saveGameString += $"\nhighestHandScoreCardDatas=";
		for(int i = 0; i < highestHandScoreCardDatas.Count; i++)
		{
			saveGameString += highestHandScoreCardDatas[i].ConvertToText();
			if(i < highestHandScoreCardDatas.Count - 1)
			{
				saveGameString += "%";
			}
		}
		saveGameString += $"\nbossRounds={GameManager.instance.ConvertBossRoundsToString()}";
		if(Shop.instance.layawayItem == null)
		{
			saveGameString += "\nlayawayItem=null";
		}
		else
		{
			saveGameString += $"\nlayawayItem={Shop.instance.layawayItem.ConvertToString()}";
		}
		saveGameString += $"\npreplacedCards={PlayArea.instance.ConvertPreplacedCardsToString()}";
		saveGameString += $"\nhandsPlayedInFatigue={handsPlayedInFatigue}";
		saveGameString += $"\nconsecutiveHandsWithMagnetClearingAllChipThresholds={consecutiveHandsWithMagnetClearingAllChipThresholds}";
		saveGameString += $"\ndateTimeStarted={V.i.dateTimeStarted}";
		saveGameString += $"\ndateTimeSaved={DateTime.Now}";
		saveGameString += $"\nflushZodiacBaubleSuitOrders={GameManager.instance.flushZodiacBaubleSuitOrders[0]}%{GameManager.instance.flushZodiacBaubleSuitOrders[1]}%{GameManager.instance.flushZodiacBaubleSuitOrders[2]}%{GameManager.instance.flushZodiacBaubleSuitOrders[3]}";
		return saveGameString;
	}
	
	public void SaveGame()
	{
		if(V.i.isDailyGame)
		{
			LocalInterface.instance.SetFileText(LocalInterface.instance.dailyGameSaveFileName, CreateSaveGameString());
		}
		else if(V.i.isCustomGame)
		{
			LocalInterface.instance.SetFileText(LocalInterface.instance.customGameSaveFileName, CreateSaveGameString());
		}
		else
		{
			LocalInterface.instance.SetFileText(LocalInterface.instance.standardGameSaveFileName, CreateSaveGameString());
		}
	}
	
	public void LoadGame(string loadGameString)
	{
		GameManager.instance.flushZodiacBaubleSuitOrders = new int[4];
		string[] loadGameData = loadGameString.Split('\n');
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
		V.i.v = new Variant(variantData);
		Deck.instance.drawPile = new List<CardData>(Deck.instance.GetListOfCardDatasFromText(loadGameData[dataIndex].Replace("drawPile=", string.Empty)));
		dataIndex++;
		List<CardData> cardsInHandCardDatas = new List<CardData>(Deck.instance.GetListOfCardDatasFromText(loadGameData[dataIndex].Replace("cardsInHand=", string.Empty)));
		for(int i = 0; i < cardsInHandCardDatas.Count; i++)
		{
			Card newCard = HandArea.instance.SpawnCard(cardsInHandCardDatas[i], HandArea.instance.deckLocation, HandArea.instance.cardParent);
			Deck.instance.cardsInHand.Add(newCard);
			HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
		}
		dataIndex++;
		Deck.instance.discardPile = new List<CardData>(Deck.instance.GetListOfCardDatasFromText(loadGameData[dataIndex].Replace("discardPile=", string.Empty)));
		dataIndex++;
		string topDeckCardString = loadGameData[dataIndex].Replace("topDeckCard=", string.Empty);
		if(topDeckCardString != "null")
		{
			Card newCard = HandArea.instance.SpawnCard(new CardData(topDeckCardString), Vector2.zero, HandArea.instance.topDeckCardParent);
			HandArea.instance.topDeckCard = newCard;
		}
		Deck.instance.UpdateCardsInDrawPile();
		Deck.instance.UpdateCardsInDiscardPile();
		HandArea.instance.ReorganizeHand();
		dataIndex++;
		GameManager.instance.SetCurrency(int.Parse(loadGameData[dataIndex].Replace("currency=", string.Empty)));
		dataIndex++;
		GameManager.instance.SetHandsUntilFatigue(int.Parse(loadGameData[dataIndex].Replace("handsUntilFatigueRemaining=", string.Empty)));
		dataIndex++;
		GameManager.instance.SetDiscards(int.Parse(loadGameData[dataIndex].Replace("discardsRemaining=", string.Empty)));
		dataIndex++;
		GameManager.instance.LoadGameToRound(int.Parse(loadGameData[dataIndex].Replace("currentRound=", string.Empty)));
		dataIndex++;
		ScoreVial.instance.LoadScore(double.Parse(loadGameData[dataIndex].Replace("currentRoundScore=", string.Empty)));
		dataIndex++;
		ScoreVial.instance.LoadChipThresholds(loadGameData[dataIndex].Replace("chipThresholds=", string.Empty));
		dataIndex++;
		V.i.chosenDeck = loadGameData[dataIndex].Replace("chosenDeck=", string.Empty);
		V.i.chosenDeckSprite = Decks.instance.decks[V.i.chosenDeck].cardBack;
		V.i.chosenDeckDescription = Decks.instance.decks[V.i.chosenDeck].description;
		dataIndex++;
		Baubles.instance.ConvertStringToOwnedBaubles(loadGameData[dataIndex].Replace("ownedBaubles=", string.Empty));
		dataIndex++;
		Baubles.instance.ConvertStringToDisabledBaubles(loadGameData[dataIndex].Replace("disabledBaubles=", string.Empty));
		dataIndex++;
		V.i.seed = int.Parse(loadGameData[dataIndex].Replace("seed=", string.Empty));
		dataIndex++;
		RNG.instance.LoadCallCountsFromString(loadGameData[dataIndex].Replace("callCounts=", string.Empty));
		dataIndex++;
		V.i.isDailyGame = bool.Parse(loadGameData[dataIndex].Replace("isDailyGame=", string.Empty));
		dataIndex++;
		V.i.isCustomGame = bool.Parse(loadGameData[dataIndex].Replace("isCustomGame=", string.Empty));
		dataIndex++;
		chipsEarned = int.Parse(loadGameData[dataIndex].Replace("chipsEarned=", string.Empty));
		dataIndex++;
		string handsPlayedString = loadGameData[dataIndex].Replace("handsPlayed=", string.Empty);
		string[] handsPlayedData = handsPlayedString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		handsPlayed = new int[18];
		for(int i = 0; i < handsPlayedData.Length; i++)
		{
			handsPlayed[i] = int.Parse(handsPlayedData[i]);
		}
		dataIndex++;
		cardsDiscarded = int.Parse(loadGameData[dataIndex].Replace("cardsDiscarded=", string.Empty));
		dataIndex++;
		cardsAddedToDeck = int.Parse(loadGameData[dataIndex].Replace("cardsAddedToDeck=", string.Empty));
		dataIndex++;
		highestHandScore = double.Parse(loadGameData[dataIndex].Replace("highestHandScore=", string.Empty));
		dataIndex++;
		highestHandTier = int.Parse(loadGameData[dataIndex].Replace("highestHandTier=", string.Empty));
		dataIndex++;
		highestHandScoreCardDatas.Clear();
		string highestHandScoreCardDatasString = loadGameData[dataIndex].Replace("highestHandScoreCardDatas=", string.Empty);
		string[] highestHandScoreCardDatasData = highestHandScoreCardDatasString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < highestHandScoreCardDatasData.Length; i++)
		{
			highestHandScoreCardDatas.Add(new CardData(highestHandScoreCardDatasData[i]));
		}
		dataIndex++;
		GameManager.instance.LoadBossRoundsFromString(loadGameData[dataIndex].Replace("bossRounds=", string.Empty));
		dataIndex++;
		string layawayItemString = loadGameData[dataIndex].Replace("layawayItem=", string.Empty);
		if(layawayItemString != "null")
		{
			Shop.instance.LoadLayawayItem(layawayItemString);
		}
		dataIndex++;
		string preplacedCardsString = loadGameData[dataIndex].Replace("preplacedCards=", string.Empty);
		if(preplacedCardsString != "none")
		{
			PlayArea.instance.ConvertStringToPreplacedCards(preplacedCardsString);
		}
		dataIndex++;
		handsPlayedInFatigue = int.Parse(loadGameData[dataIndex].Replace("handsPlayedInFatigue=", string.Empty));
		dataIndex++;
		consecutiveHandsWithMagnetClearingAllChipThresholds = int.Parse(loadGameData[dataIndex].Replace("consecutiveHandsWithMagnetClearingAllChipThresholds=", string.Empty));
		dataIndex++;
		V.i.dateTimeStarted = DateTime.Parse(loadGameData[dataIndex].Replace("dateTimeStarted=", string.Empty));
		dataIndex++;
		// dateTimeSaved
		dataIndex++;
		// Debug.Log($"loadGameData.Length={loadGameData.Length}, dataIndex={dataIndex}");
		if(loadGameData.Length > dataIndex)
		{		
			string flushZodiacBaubleSuitOrdersData = loadGameData[dataIndex].Replace("flushZodiacBaubleSuitOrders=", string.Empty);
			string[] flushZodiacBaubleSuitOrdersArray = flushZodiacBaubleSuitOrdersData.Split('%', StringSplitOptions.RemoveEmptyEntries);
			// Debug.Log($"flushZodiacBaubleSuitOrdersData={flushZodiacBaubleSuitOrdersData}, flushZodiacBaubleSuitOrdersArray.Length={flushZodiacBaubleSuitOrdersArray.Length}");
			GameManager.instance.flushZodiacBaubleSuitOrders = new int[4]{int.Parse(flushZodiacBaubleSuitOrdersArray[0]), int.Parse(flushZodiacBaubleSuitOrdersArray[1]), int.Parse(flushZodiacBaubleSuitOrdersArray[2]), int.Parse(flushZodiacBaubleSuitOrdersArray[3])};
			// Debug.Log($"Loading flushZodiacBaubleSuitOrders, flushZodiacBaubleSuitOrdersData={flushZodiacBaubleSuitOrdersData}");
		}
		else
		{
			GameManager.instance.flushZodiacBaubleSuitOrders = LocalInterface.instance.GetRandomizedArrayOfInts(4, 4, 3);
			Debug.Log($"Randomizing flushZodiacBaubleSuitOrders");
		}
		// to reset the possibly loaded cards that had to be loaded before setting flushZodiacBaubleSuitOrders
		Deck.instance.ResetGraphicsOfAllSpecialCardsWithTag("ZodiacsFromFlushes");
		BaublesInformation.instance.UpdateSpecialBaubleIconsDescriptions();
	}
	
	public string GetCurrentSaveFileName()
	{
		if(V.i.isDailyGame)
		{
			return LocalInterface.instance.dailyGameSaveFileName;
		}
		else if (V.i.isCustomGame)
		{
			return LocalInterface.instance.customGameSaveFileName;
		}
		return LocalInterface.instance.standardGameSaveFileName;
	}
	
	public string GetSaveParameter(string parameter)
	{
		string saveGameName = GetCurrentSaveFileName();
		string[] saveGameLines = LocalInterface.instance.GetFileTextLines(saveGameName);
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
	
	public void CardAddedToDeck(CardData cardData)
	{
		cardsAddedToDeck++;
		Stats.instance.CardAddedToDeck();
		if(cardData.suit == 4)
		{
			CheckForSchromaticUnlock(12);
		}
	}
	
	public void CheckForSchromaticUnlock(int cardsNeeded = 13)
	{
		if(!Decks.instance.decks["Schromatic"].unlocked && !V.i.isDailyGame && !V.i.isCustomGame)
		{
			List<CardData> cardDatasInDeck = Deck.instance.GetAllCardDatasInDeck();
			int rainbowCards = 0;
			for(int i = 0; i < cardDatasInDeck.Count; i++)
			{
				if(cardDatasInDeck[i].suit == 4)
				{
					rainbowCards++;
				}
				if(rainbowCards >= cardsNeeded)
				{
					Decks.instance.UnlockDeck("Schromatic");
					return;
				}
			}
		}
	}
}
