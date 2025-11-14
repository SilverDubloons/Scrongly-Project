using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static Deck;
using System.Text.RegularExpressions;

public class RunStatsPanel : MonoBehaviour
{
    public RectTransform rt;
	public GameObject visibilityObject;
	public RectTransform shadowRT;
	public RectTransform buttonsBackdropRT;
	public RectTransform cardsParent;
	public ButtonPlus endlessModeButton;
	public ButtonPlus mainMenuButton;
	public Image cardBackImage;
	public VariantSimple variantSimple;
	public Label titleLabel;
	public Label bestHandNameLabel;
	public Label bestHandScoreLabel;
	public Label handsPlayedLabel;
	public Label cardsDiscardedLabel;
	public Label baublesGainedLabel;
	public Label zodiacsGainedLabel;
	public Label cardsAddedToDeckLabel;
	public Label chipsEarnedLabel;
	public Label mostPlayedHandNameLabel;
	public Label mostPlayedHandQuantityLabel;
	public Label roundReachedLabel;
	public Label scoreInFinalRoundLabel;
	public Label seedLabel;
	public Label dateLabel;
	
	public bool runStatsPanelIsOnScreen = false;
	
	public List<Card> cards = new List<Card>();
	
	public const float cardAreaWidth = 280f;
	public const float optimalDistanceBetweenCards = 5f;
	
	public static RunStatsPanel instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetVisibility(bool enabledState)
	{
		visibilityObject.SetActive(enabledState);
	}
	
	public void SetInteractability(bool enabledState)
	{
		mainMenuButton.ChangeButtonEnabled(enabledState);
		if(endlessModeButton.gameObject.activeSelf)
		{
			endlessModeButton.ChangeButtonEnabled(enabledState);
		}
	}
	
	public void SetAvailabilityOfButtons(bool mainMenu, bool endless)
	{
		if(!mainMenu && !endless)
		{
			buttonsBackdropRT.gameObject.SetActive(false);
			shadowRT.offsetMax = new Vector2(0, shadowRT.offsetMax.y);
		}
		else
		{
			buttonsBackdropRT.gameObject.SetActive(true);
			endlessModeButton.gameObject.SetActive(endless);
			mainMenuButton.gameObject.SetActive(mainMenu);
			shadowRT.offsetMax = new Vector2(-175f, shadowRT.offsetMax.y);
			if(mainMenu && endless)
			{
				buttonsBackdropRT.sizeDelta = new Vector2(buttonsBackdropRT.sizeDelta.x, 63f);
				
			}
			else if(mainMenu && !endless)
			{
				buttonsBackdropRT.sizeDelta = new Vector2(buttonsBackdropRT.sizeDelta.x, 34f);
			}
		}
	}
	
	public void UpdateFromSaveFile(string saveString)
	{
		visibilityObject.SetActive(true);
		string[] saveLines = saveString.Split('\n'); //, StringSplitOptions.RemoveEmptyEntries
		bool isDailyGame = false;
		bool isCustomGame = false;
		baublesGainedLabel.ChangeText("0");
		mostPlayedHandNameLabel.ChangeText("None");
		mostPlayedHandQuantityLabel.ChangeText("0");
		handsPlayedLabel.ChangeText("0");
		for(int i = 0; i < cards.Count; i++)
		{
			cards[i].gameObject.SetActive(false);
		}
		roundReachedLabel.ChangeText("-1");
		scoreInFinalRoundLabel.ChangeText("-1");
		seedLabel.ChangeText("-1");
		chipsEarnedLabel.ChangeText("-1");
		cardsDiscardedLabel.ChangeText("-1");
		cardsAddedToDeckLabel.ChangeText("-1");
		bestHandScoreLabel.ChangeText("-1");
		bestHandNameLabel.ChangeText("None");
		zodiacsGainedLabel.ChangeText("0");
		dateLabel.ChangeText("-1");
		for(int i = 0; i < saveLines.Length; i++)
		{
			string[] lineData = saveLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(lineData.Length > 1)
			{
				switch(lineData[0])
				{
					case "currentRound":
						roundReachedLabel.ChangeText((int.Parse(lineData[1]) + 1).ToString());
					break;
					case "currentRoundScore":
						scoreInFinalRoundLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(double.Parse(lineData[1])));
					break;
					case "chosenDeck":
						cardBackImage.sprite = Decks.instance.decks[lineData[1]].cardBack;
					break;
					case "ownedBaubles":
						string[] ownedBaublesStrings = lineData[1].Split('%', StringSplitOptions.RemoveEmptyEntries);
						int totalOwnedBaubles = 0;
						int totalOwnedZodiacs = 0;
						for(int j = 0; j < ownedBaublesStrings.Length; j++)
						{
							string[] ownedBaubleData = ownedBaublesStrings[j].Split('|', StringSplitOptions.RemoveEmptyEntries);
							if(LocalInterface.BaubleNameIsZodiac(ownedBaubleData[0]))
							{
								if(ownedBaubleData.Length > 1)
								{
									totalOwnedZodiacs += int.Parse(ownedBaubleData[1]);
								}
							}
							else
							{
								if(ownedBaubleData.Length > 1)
								{
									totalOwnedBaubles += int.Parse(ownedBaubleData[1]);
								}
							}
						}
						if(totalOwnedBaubles > 999)
						{
							baublesGainedLabel.ChangeFontSize(8);
						}
						else
						{
							baublesGainedLabel.ChangeFontSize(16);
						}
						baublesGainedLabel.ChangeText(totalOwnedBaubles.ToString());
						if(totalOwnedZodiacs > 999)
						{
							zodiacsGainedLabel.ChangeFontSize(8);
						}
						else
						{
							zodiacsGainedLabel.ChangeFontSize(16);
						}
						zodiacsGainedLabel.ChangeText(totalOwnedZodiacs.ToString());
					break;
					case "seed":
						seedLabel.ChangeText(lineData[1].ToString());
					break;
					case "isDailyGame":
						isDailyGame = bool.Parse(lineData[1]);
					break;
					case "isCustomGame":
						isCustomGame = bool.Parse(lineData[1]);
					break;
					case "chipsEarned":
						if(int.Parse(lineData[1]) > 999)
						{
							chipsEarnedLabel.ChangeFontSize(8);
						}
						else
						{
							chipsEarnedLabel.ChangeFontSize(16);
						}
						chipsEarnedLabel.ChangeText(lineData[1]);
					break;
					case "handsPlayed":
						string[] handsPlayedStrings = lineData[1].Split('%', StringSplitOptions.RemoveEmptyEntries);
						int mostPlayedHand = -1;
						int playedQuantityOfMostPlayedHand = -1;
						int totalHandsPlayed = 0;
						for(int j = 0; j < handsPlayedStrings.Length; j++)
						{
							int playedQuantity = int.Parse(handsPlayedStrings[j]);
							totalHandsPlayed += playedQuantity;
							if(playedQuantity > playedQuantityOfMostPlayedHand && playedQuantity > 0)
							{
								mostPlayedHand = j;
								playedQuantityOfMostPlayedHand = playedQuantity;
							}
						}
						if(mostPlayedHand < 0)
						{
							mostPlayedHandNameLabel.ChangeFontSize(16);
							mostPlayedHandNameLabel.ChangeText("None");
							mostPlayedHandQuantityLabel.ChangeFontSize(16);
							mostPlayedHandQuantityLabel.ChangeText("0");
						}
						else
						{
							string mostPlayedHandString = LocalInterface.instance.handNames[mostPlayedHand];
							if(mostPlayedHandString.Length > 10)
							{
								mostPlayedHandNameLabel.ChangeFontSize(8);
							}
							else
							{
								mostPlayedHandNameLabel.ChangeFontSize(16);
							}
							mostPlayedHandNameLabel.ChangeText(mostPlayedHandString);
							if(playedQuantityOfMostPlayedHand > 999)
							{
								mostPlayedHandQuantityLabel.ChangeFontSize(8);
							}
							else
							{
								mostPlayedHandQuantityLabel.ChangeFontSize(16);
							}
							mostPlayedHandQuantityLabel.ChangeText(playedQuantityOfMostPlayedHand.ToString());
						}
						if(totalHandsPlayed > 999)
						{
							handsPlayedLabel.ChangeFontSize(8);
						}
						else
						{
							handsPlayedLabel.ChangeFontSize(16);
						}
						handsPlayedLabel.ChangeText(totalHandsPlayed.ToString());
					break;
					case "cardsDiscarded":
						if(int.Parse(lineData[1]) > 999)
						{
							cardsDiscardedLabel.ChangeFontSize(8);
						}
						else
						{
							cardsDiscardedLabel.ChangeFontSize(16);
						}
						cardsDiscardedLabel.ChangeText(lineData[1]);
					break;
					case "cardsAddedToDeck":
						if(int.Parse(lineData[1]) > 999)
						{
							cardsAddedToDeckLabel.ChangeFontSize(8);
						}
						else
						{
							cardsAddedToDeckLabel.ChangeFontSize(16);
						}
						cardsAddedToDeckLabel.ChangeText(lineData[1]);
					break;
					case "highestHandScore":
						bestHandScoreLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(double.Parse(lineData[1])));
					break;
					case "highestHandTier":
						int highestHandTierInt = int.Parse(lineData[1]);
						if(highestHandTierInt >= 0)
						{
							string bestHandName = LocalInterface.instance.handNames[highestHandTierInt];
							if(bestHandName.Length > 10)
							{
								bestHandNameLabel.ChangeFontSize(8);
							}
							else
							{
								bestHandNameLabel.ChangeFontSize(16);	
							}
							bestHandNameLabel.ChangeText(bestHandName);
						}
					break;
					case "highestHandScoreCardDatas":
						string[] cardDatasStrings = lineData[1].Split('%', StringSplitOptions.RemoveEmptyEntries);
						float squeezeDistance = (cardAreaWidth - LocalInterface.instance.cardSize.x) / (cardDatasStrings.Length - 1);
						float distanceBetweenCards = Mathf.Min((optimalDistanceBetweenCards + LocalInterface.instance.cardSize.x), squeezeDistance);
						// Debug.Log($"lineData[1]={lineData[1]}, cardDatasStrings.Length={cardDatasStrings.Length}, squeezeDistance={squeezeDistance}, distanceBetweenCards={distanceBetweenCards}, cardAreaWidth={cardAreaWidth}, LocalInterface.instance.cardSize.x={LocalInterface.instance.cardSize.x}, optimalDistanceBetweenCards={optimalDistanceBetweenCards}");
						for(int j = 0; j < cardDatasStrings.Length; j++)
						{
							float xDestination = (cardDatasStrings.Length - 1) * (distanceBetweenCards / 2f) - (cardDatasStrings.Length - j - 1) * distanceBetweenCards;
							if(cards.Count > j)
							{
								cards[j].gameObject.SetActive(true);
								cards[j].cardData = new CardData(cardDatasStrings[j]);
								cards[j].UpdateGraphics();
								cards[j].rt.anchoredPosition = new Vector2(xDestination, 0);
							}
							else
							{
								GameObject newCardGO = Instantiate(LocalInterface.instance.cardPrefab, cardsParent);
								Card newCard = newCardGO.GetComponent<Card>();
								cards.Add(newCard);
								newCard.cardData = new CardData(cardDatasStrings[j]);
								newCard.isDeckviewerClone = true;
								newCard.UpdateGraphics();
								newCard.rt.anchoredPosition = new Vector2(xDestination, 0);
							}
						}
						for(int j = cardDatasStrings.Length; j < cards.Count; j++)
						{
							cards[j].gameObject.SetActive(false);
						}
					break;
					case "dateTimeStarted":
						DateTime dateTimeStarted = DateTime.Parse(lineData[1]);
						dateLabel.ChangeText(dateTimeStarted.ToString("d MMMM yyyy"));
					break;
				}
			}
		}
	}
	
	public void MainMenuClicked()
	{
		if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene")
		{
			Stats.instance.GameCompleted();
			Preferences.instance.ReturnToMenu();
			if(endlessModeButton.gameObject.activeSelf)
			{
				LocalInterface.instance.MoveSaveGameToFinishedGame(V.i.isDailyGame, V.i.isCustomGame, V.i.dateTimeStarted);
			}
		}
	}
	
	public void EndlessModeClicked()
	{
		runStatsPanelIsOnScreen = false;
		MovingObjects.instance.mo["RunStatsPanel"].StartMove("OffScreen");
		HandArea.instance.StartShuffleDiscardPileIntoDrawPile();
		Shop.instance.OpenShop(false);
		ScoreVial.instance.StartDrainVial();
		ScoreVial.instance.vialTop.StartReturn(1f);
		PlayArea.instance.HandUpdated();
	}
	
	public void UpdateTitleForWonEndless()
	{
		if(V.i.isDailyGame)
		{
			titleLabel.ChangeText($"{V.i.dateTimeStarted.ToString("d MMMM yyyy")} daily game endless mode complete!");
		}
		else if(V.i.isCustomGame)
		{
			titleLabel.ChangeText("Custom game endless mode complete!");
		}
		else
		{
			if(V.i.chosenDeck == "Screpublic")
			{
				titleLabel.ChangeText("Sic semper tyrannis (endless mode heck yeah!)!");
			}
			else
			{
				titleLabel.ChangeText("Endless mode complete!");
			}
		}
	}
	
	public void UpdateTitleForLosingGame()
	{
		if(V.i.isDailyGame)
		{
			titleLabel.ChangeText($"{V.i.dateTimeStarted.ToString("d MMMM yyyy")} daily game over");
		}
		else if(V.i.isCustomGame)
		{
			titleLabel.ChangeText("Custom game over");
		}
		else
		{
			titleLabel.ChangeText("Game over");
		}
	}
	
	public void UpdateTitleWon()
	{
		if(V.i.isDailyGame)
		{
			titleLabel.ChangeText($"{V.i.dateTimeStarted.ToString("d MMMM yyyy")} Daily Game Complete!");
		}
		else if(V.i.isCustomGame)
		{
			titleLabel.ChangeText("Custom Game Complete!");
		}
		else
		{
			if(V.i.chosenDeck == "Screpublic")
			{
				titleLabel.ChangeText("Sic semper tyrannis!");
			}
			else
			{
				titleLabel.ChangeText("Game Won!");
			}
		}
	}
}
