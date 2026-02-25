using UnityEngine;
using static Deck;
using System.Collections.Generic;
using System.Collections;

/*

from 10-sep-2025 daily, jammed card, scastronomy, start of round 35, had just raised the speed to 4, worked fine after discarding

UnityException: Transform child out of bounds
  at (wrapper managed-to-native) UnityEngine.Transform.GetChild_Injected(intptr,int)
  at UnityEngine.Transform.GetChild (System.Int32 index) [0x0000f] in <2ea3be0347904a3e8110797b2beee96b>:0 
  at HandArea.ReorganizeHand () [0x000e4] in <68bf8687764d4fd69da83c3940af96f3>:0 
  at HandArea+<DrawCardsCoroutine>d__37.MoveNext () [0x00157] in <68bf8687764d4fd69da83c3940af96f3>:0 
  at UnityEngine.SetupCoroutine.InvokeMoveNext (System.Collections.IEnumerator enumerator, System.IntPtr returnValueAddress) [0x00026] in <2ea3be0347904a3e8110797b2beee96b>:0 

*/

public class HandArea : MonoBehaviour
{
    public GameObject cardPrefab;
	public GameObject cardBackOnlyPrefab;
	public RectTransform cardParent;
	public RectTransform looseCardParent;
	public RectTransform discardedCardsParent;
	public RectTransform shufflingCardsParent;
	public RectTransform movingCardsParent;
	public RectTransform miscCardsParent; // used for zodiac cards before being dissolved
	public RectTransform topDeckCardParent;
	public ControllerSelectionGroup cardsControllerSelectionGroup;
	
	public Sprite[] rankSprites;
	public Sprite[] bigSuitSprites;
	public Sprite[] cardDetails;
	
	public ButtonPlus sortByRankButton;
	public ButtonPlus sortBySuitButton;
	public ButtonPlus selectAllButton;
	public ButtonPlus playSelectedButton;
	public ButtonPlus discardButton;
	public ButtonPlus recallButton;
	public int alwaysSortType; // 0 = no sorting, 1 = rank, 2 = suit
	public AnimationCurve handParabola;
	public AnimationCurve handRotationParabola;
	
	public Vector2 deckLocation;
	public Vector2 discardPileLocation;
	
	public List<Card> selectedCards;
	public int siblingIndexOfLooseCard = -1;
	public GameObject handAreaCardZone;
	public bool shufflingDiscardPileIntoDrawPile;
	private IEnumerator drawCardsCoroutine;
	public bool drawingCards;
	public Card topDeckCard;
	
	public float maxDistanceBetweenCards;
	public float handAreaWidth;
	public float distanceDifferenceOfSelectedCard;
	
	public static HandArea instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		sortByRankButton.ChangeButtonEnabled(enabledState);
		sortBySuitButton.ChangeButtonEnabled(enabledState);
		selectAllButton.ChangeButtonEnabled(enabledState);
		playSelectedButton.ChangeButtonEnabled(false);
		discardButton.ChangeButtonEnabled(false);
		recallButton.ChangeButtonEnabled(false);
	}
	
	public void StartDrawCards(float delay = 0, bool shuffleFirst = false)
	{
		if(drawingCards)
		{
			StopCoroutine(drawCardsCoroutine);
		}
		drawCardsCoroutine = DrawCardsCoroutine(delay, shuffleFirst);
		StartCoroutine(drawCardsCoroutine);
	}
	
	public IEnumerator DrawCardsCoroutine(float delay = 0, bool shuffleFirst = false)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		if(shuffleFirst)
		{
			Deck.instance.ShuffleDrawPile();
		}
		while((Deck.instance.drawPile.Count > 0 || topDeckCard != null) && Deck.instance.GetNumberOfStandardCardsInHand() < GameManager.instance.GetMaxHandSize())
		{
			SoundManager.instance.PlayCardPickupSound();
			if(topDeckCard != null)
			{
				Deck.instance.cardsInHand.Add(topDeckCard);
				topDeckCard.rt.SetParent(cardParent);
				topDeckCard.rt.SetSiblingIndex(0);
				cardsControllerSelectionGroup.controllerSelectableObjects.Add(topDeckCard.controllerSelectableObject);
				topDeckCard = null;
				// Debug.Log("Set topDeckCard to null");
			}
			else
			{
				Card newCard = SpawnCard(Deck.instance.drawPile[Deck.instance.drawPile.Count - 1], deckLocation, cardParent);
				Deck.instance.cardsInHand.Add(newCard);
				Deck.instance.drawPile.RemoveAt(Deck.instance.drawPile.Count - 1);
				cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
				// Debug.Log($"Just added {newCard.gameObject.name} to selectable cards");
				// Deck.instance.UpdateCardsInDrawPile();
			}
			Deck.instance.UpdateCardsInDrawPile();
			ReorganizeHand();
			yield return new WaitForSeconds((LocalInterface.instance.animationDuration / 5) / Preferences.instance.gameSpeed);
		}
		if(Deck.instance.drawPile.Count > 0 && Baubles.instance.GetImpactInt("SeeTopCardOfDeck") > 0)
		{
			Card newCard = SpawnCard(Deck.instance.drawPile[Deck.instance.drawPile.Count - 1], Vector2.zero, topDeckCardParent);
			Deck.instance.drawPile.RemoveAt(Deck.instance.drawPile.Count - 1);
			topDeckCard = newCard;
			Deck.instance.UpdateCardsInDrawPile();
			// Debug.Log("Set topDeckCard to newCard.name");
			
		}
		if(Deck.instance.drawPile.Count <= 0 && Deck.instance.GetNumberOfStandardCardsInHand() <= 0)
		{
			// Debug.Log("Game Over");
			RunInformation.instance.SaveGame();
			SoundManager.instance.PlayGameOverSound();
			RunStatsPanel.instance.UpdateFromSaveFile(RunInformation.instance.CreateSaveGameString());
			RunStatsPanel.instance.SetAvailabilityOfButtons(true, false);
			RunStatsPanel.instance.runStatsPanelIsOnScreen = true;
			Stats.instance.GameCompleted();
			MovingObjects.instance.mo["RunStatsPanel"].StartMove("OnScreen");
			MovingObjects.instance.mo["HandArea"].StartMove("OffScreen");
			MovingObjects.instance.mo["CardParent"].StartMove("OffScreen");
			MovingObjects.instance.mo["PlayArea"].StartMove("OffScreen");
			MovingObjects.instance.mo["BossInformation"].StartMove("OffScreen");
			RunStatsPanel.instance.UpdateTitleForLosingGame();
			LocalInterface.instance.MoveSaveGameToFinishedGame(V.i.isDailyGame, V.i.isCustomGame, V.i.dateTimeStarted);
		}
		else
		{
			if(V.i.isTutorial)
			{
				if(Tutorial.instance.currentStage > 2 && !Tutorial.instance.tutorialFinished)
				{
					Tutorial.instance.IncrementStage();
				}
				if(Tutorial.instance.displayingTips)
				{
					Tutorial.instance.MoveToTop();
				}
			}
			yield return new WaitForSeconds(0.25f / Preferences.instance.gameSpeed);
			RunInformation.instance.SaveGame();
		}
	}
	
	public Card SpawnCard(CardData cardData, Vector2 spawnLocation, Transform parent, bool spawnFaceDown = true, bool startFlipping = true, int newSiblingIndex = 0)
	{
		GameObject newCardGO = Instantiate(cardPrefab, spawnLocation, Quaternion.identity, parent);
		if(newSiblingIndex >= 0)	// -1 to ignore
		{
			newCardGO.transform.SetSiblingIndex(newSiblingIndex); // <- This could be a problem in certian places. Use dedicated parents!!!
		}
		Card newCard = newCardGO.GetComponent<Card>();
		newCard.rt.anchoredPosition = spawnLocation;
		newCard.cardData = cardData;
		newCard.back.sprite = V.i.chosenDeckSprite;
		newCard.UpdateGraphics();
		if(spawnFaceDown)
		{
			newCard.ChangeFaceDown(true);
		}
		else
		{
			newCard.ChangeFaceDown(false);
		}
		if(startFlipping)
		{
			newCard.StartFlip();
		}
		return newCard;
	}
	
	public void AddSpecialCardToDeck(string specialCardTag, Vector2 originLocation, bool instant = false)
	{
		CardData newCardData = new CardData(-1, -1, specialCardTag);
		if(instant)
		{
			Deck.instance.drawPile.Add(newCardData);
		}
		else
		{
			Card newCard = SpawnCard(newCardData, originLocation, movingCardsParent, false, true);
			newCard.StartMove(deckLocation, Vector3.zero, false, true, false, true);
		}
		RunInformation.instance.CardAddedToDeck(newCardData);
		
	}
	
	public void StartShuffleDiscardPileIntoDrawPile(float delay = 0f)
	{
		StartCoroutine(ShuffleDiscardPileIntoDrawPile(delay));
	}
	
	public IEnumerator ShuffleDiscardPileIntoDrawPile(float delay = 0f)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			yield return null;
		}
		int cardsInDiscardPile = Deck.instance.discardPile.Count;
		shufflingDiscardPileIntoDrawPile = true;
		SoundManager.instance.PlayCardShuffleSound();
		while(Deck.instance.discardPile.Count > 0)
		{
			GameObject newCardBackOnlyGO = Instantiate(cardBackOnlyPrefab, shufflingCardsParent);
			CardBackOnly newCardBackOnly = newCardBackOnlyGO.GetComponent<CardBackOnly>();
			newCardBackOnly.rt.anchoredPosition = discardPileLocation;
			newCardBackOnly.cardData = Deck.instance.discardPile[Deck.instance.discardPile.Count - 1];
			Deck.instance.discardPile.RemoveAt(Deck.instance.discardPile.Count - 1);
			Deck.instance.UpdateCardsInDiscardPile();
			newCardBackOnly.image.sprite = V.i.chosenDeckSprite;
			newCardBackOnly.StartMove(deckLocation, Vector3.zero, true, false, true);
			yield return new WaitForSeconds(LocalInterface.instance.animationDuration / cardsInDiscardPile);
		}
		shufflingDiscardPileIntoDrawPile = false;
	}
	
	public void SortHandClicked(int sortType)
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage != 1 || sortType != 1)
			{
				MinorNotifications.instance.NewMinorNotification("Please follow the tutorial!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(sortBySuitButton.rt, GameManager.instance.gameplayCanvas));
				return;
			}
		}
		if(sortType == 1)
		{
			if(alwaysSortType == 1)
			{
				ChangeAlwaysSortType(0);
			}
			else
			{
				ChangeAlwaysSortType(1);
			}
			if(V.i.isTutorial && Tutorial.instance.currentStage == 1)
			{
				Tutorial.instance.IncrementStage();
			}
		}
		else if(sortType == 2)
		{
			if(alwaysSortType == 2)
			{
				ChangeAlwaysSortType(0);
			}
			else
			{
				ChangeAlwaysSortType(2);
			}
		}
		SortHand(sortType);
		ReorganizeHand();
	}
	
	public void ChangeAlwaysSortType(int sortType)
	{
		alwaysSortType = sortType;
		sortByRankButton.ChangeSpecialState(sortType == 1 ? true : false);
		sortBySuitButton.ChangeSpecialState(sortType == 2 ? true : false);
	}
	
	public void SortHand(int sortType)
	{
		if(sortType != alwaysSortType)
		{
			ChangeAlwaysSortType(0); 
		}
		List<Card> cardsInHand = GetAllCardsInHand(false);
		if(sortType == 1)
		{
			cardsInHand.Sort((x,y) => 
			{
				if(x.cardData.isSpecialCard && y.cardData.isSpecialCard)
				{
					if(x.cardData.specialCardName == y.cardData.specialCardName)
					{
						return x.rt.anchoredPosition.x.CompareTo(y.rt.anchoredPosition.x);
					}
					else
					{
						// return V.i.v.variantSpecialCards[x.cardData.specialCardName].specialCardName.CompareTo(V.i.v.variantSpecialCards[y.cardData.specialCardName].specialCardName);
						string xSpecialCardName = "";
						if(x.cardData.specialCardName.Length >= 18 && x.cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
						{
							xSpecialCardName = "Four Piece Puzzle";
						}
						else
						{
							xSpecialCardName = V.i.v.variantSpecialCards[x.cardData.specialCardName].specialCardName;
						}
						string ySpecialCardName = "";
						if(y.cardData.specialCardName.Length >= 18 && y.cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
						{
							ySpecialCardName = "Four Piece Puzzle";
						}
						else
						{
							ySpecialCardName = V.i.v.variantSpecialCards[y.cardData.specialCardName].specialCardName;
						}
						return xSpecialCardName.CompareTo(ySpecialCardName);
					}
				}
				if(x.cardData.isSpecialCard != y.cardData.isSpecialCard)
				{
					if(Preferences.instance.specialCardsSortToLeftOfHand)
					{
						return y.cardData.isSpecialCard.CompareTo(x.cardData.isSpecialCard);
					}
					else
					{
						return x.cardData.isSpecialCard.CompareTo(y.cardData.isSpecialCard);
					}
				}
				int rankComparison = x.cardData.rank - y.cardData.rank;
				if(rankComparison != 0)
				{
					return rankComparison;
				}
				else
				{
					if(x.cardData.suit == y.cardData.suit)
					{
						if (Mathf.Approximately(x.cardData.multiplier, y.cardData.multiplier))
						{
							if (Mathf.Approximately(x.cardData.baseValue, y.cardData.baseValue))
							{
                                return x.rt.anchoredPosition.x.CompareTo(y.rt.anchoredPosition.x);
                            }
							return x.cardData.baseValue.CompareTo(y.cardData.baseValue);
                        }
						else
						{ 
							return x.cardData.multiplier.CompareTo(y.cardData.multiplier);
						}
					}
					else
					{
						return LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[x.cardData.suit]] - LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[y.cardData.suit]];
					}
				}
			});
		}
		else
		{
			cardsInHand.Sort((x,y) => 
			{
				if(x.cardData.isSpecialCard && y.cardData.isSpecialCard)
				{
					if(x.cardData.specialCardName == y.cardData.specialCardName)
					{
						return x.rt.anchoredPosition.x.CompareTo(y.rt.anchoredPosition.x);
					}
					else
					{
						// return V.i.v.variantSpecialCards[x.cardData.specialCardName].specialCardName.CompareTo(V.i.v.variantSpecialCards[y.cardData.specialCardName].specialCardName);
						string xSpecialCardName = "";
						if(x.cardData.specialCardName.Length >= 18 && x.cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
						{
							xSpecialCardName = "Four Piece Puzzle";
						}
						else
						{
							xSpecialCardName = V.i.v.variantSpecialCards[x.cardData.specialCardName].specialCardName;
						}
						string ySpecialCardName = "";
						if(y.cardData.specialCardName.Length >= 18 && y.cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
						{
							ySpecialCardName = "Four Piece Puzzle";
						}
						else
						{
							ySpecialCardName = V.i.v.variantSpecialCards[y.cardData.specialCardName].specialCardName;
						}
						return xSpecialCardName.CompareTo(ySpecialCardName);
					}
				}
				if(x.cardData.isSpecialCard != y.cardData.isSpecialCard)
				{
					if(Preferences.instance.specialCardsSortToLeftOfHand)
					{
						return y.cardData.isSpecialCard.CompareTo(x.cardData.isSpecialCard);
					}
					else
					{
						return x.cardData.isSpecialCard.CompareTo(y.cardData.isSpecialCard);
					}
				}
				int suitComparison = LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[x.cardData.suit]] - LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[y.cardData.suit]];
				if(suitComparison != 0)
				{
					return suitComparison;
				}
				else
				{
					if(x.cardData.rank == y.cardData.rank)
					{
                        if (Mathf.Approximately(x.cardData.multiplier, y.cardData.multiplier))
                        {
                            if (Mathf.Approximately(x.cardData.baseValue, y.cardData.baseValue))
                            {
                                return x.rt.anchoredPosition.x.CompareTo(y.rt.anchoredPosition.x);
                            }
                            return x.cardData.baseValue.CompareTo(y.cardData.baseValue);
                        }
                        else
                        {
                            return x.cardData.multiplier.CompareTo(y.cardData.multiplier);
                        }
                    }
					else
					{
						return x.cardData.rank - y.cardData.rank;
					}
				}
			});
		}
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			cardsInHand[i].transform.SetSiblingIndex(i);
		}
	}
	
	public List<Card> GetAllCardsInHand(bool includeLooseCards = true)
	{
		List<Card> cardsInHand = new List<Card>();
		for(int i = 0; i < cardParent.childCount; i++)
		{
			if(cardParent.GetChild(i).GetComponent<Card>() != null)
			{
				cardsInHand.Add(cardParent.GetChild(i).GetComponent<Card>());
			}
		}
		if(includeLooseCards)
		{
			for(int i = 0; i < looseCardParent.childCount; i++)
			{
				if(looseCardParent.GetChild(i).GetComponent<Card>() != null)
				{
					cardsInHand.Add(looseCardParent.GetChild(i).GetComponent<Card>());
				}
			}
		}
		return cardsInHand;
	}
	
	public void ReorganizeHand()
	{
		if(alwaysSortType != 0)
		{
			SortHand(alwaysSortType);
		}
		float distanceBetweenCards = maxDistanceBetweenCards;
		int visualCardsInHand = cardParent.childCount;
		if(siblingIndexOfLooseCard >= 0)
		{
			visualCardsInHand++;
		}
		if(visualCardsInHand > 8)
		{
			distanceBetweenCards = handAreaWidth / cardParent.childCount;
		}
		for(int i = 0; i < visualCardsInHand; i++)
		{
			if(i != siblingIndexOfLooseCard)
			{
				float xDestination = (visualCardsInHand - 1) * (distanceBetweenCards / 2f) - (visualCardsInHand - i - 1) * distanceBetweenCards; // -200 to 200
				float yDestination = handParabola.Evaluate((xDestination + (handAreaWidth / 2)) / handAreaWidth) * 30 - 100;
				Vector2 destination = new Vector2(xDestination, yDestination);
				Vector3 destinationRotation = new Vector3(0, 0, handRotationParabola.Evaluate((xDestination + (handAreaWidth / 2)) / handAreaWidth) * 30f);
				cardParent.GetChild((i < siblingIndexOfLooseCard || siblingIndexOfLooseCard < 0) ? i : i - 1).GetComponent<Card>().StartMove(destination, destinationRotation);
			}
		}
		selectedCards.Clear();
		SelectedCardsUpdated();
	}
	
	public int GetNumberOfCardsOfTypeInList(List<Card> cards, bool special)
	{
		int cardsOfType = 0;
		for(int i = 0; i < cards.Count; i++)
		{
			if(cards[i].cardData.isSpecialCard == special)
			{
				cardsOfType++;
			}
		}
		return cardsOfType;
	}
	
	public void SelectAllClicked()
	{
		List<Card> cardsInHand = GetAllCardsInHand();
		int cardsSelected = 0;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(!selectedCards.Contains(cardsInHand[i]))
			{
				selectedCards.Add(cardsInHand[i]);
				cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition + Vector2.up * distanceDifferenceOfSelectedCard;
				cardsSelected++;
			}
		}
		if(cardsSelected > 0)
		{
			SoundManager.instance.PlayCardPickupSound();
		}
		else
		{
			for(int i = 0; i < cardsInHand.Count; i++)
			{
				if(selectedCards.Contains(cardsInHand[i]))
				{
					selectedCards.Remove(cardsInHand[i]);
					cardsInHand[i].rt.anchoredPosition = cardsInHand[i].rt.anchoredPosition - Vector2.up * distanceDifferenceOfSelectedCard;
					cardsSelected++;
				}
			}
			if(cardsSelected > 0)
			{
				SoundManager.instance.PlayCardDropSound();
			}
		}
		SelectedCardsUpdated();
		if(ControllerSelection.instance.usingController && ControllerSelection.instance.currentlySelectedObject != null && ControllerSelection.instance.currentlySelectedObject.isCard && ControllerSelection.instance.currentlySelectedObject.card.dropZonePlacedIn == null)
		{
			ControllerSelection.instance.RepositionControllerSelectionRT(ControllerSelection.instance.currentlySelectedObject, cardsControllerSelectionGroup);
		}
	}
	
	public void RecallClicked()
	{
		for(int i = PlayArea.instance.standardDropZones.Length - 1; i >= 0; i--)
		{
			if(PlayArea.instance.standardDropZones[i].cardPlaced && !PlayArea.instance.standardDropZones[i].locked)
			{
				PlayArea.instance.standardDropZones[i].placedCard.dropZonePlacedIn = null;
				PlayArea.instance.standardDropZones[i].placedCard.RevertToOriginalImage();
				PlayArea.instance.standardDropZones[i].placedCard.transform.SetParent(cardParent);
				PlayArea.instance.standardDropZones[i].placedCard.transform.SetSiblingIndex(0);
				PlayArea.instance.standardDropZones[i].CardRemoved();
			}
		}
		for(int i = PlayArea.instance.specialCardDropZones.Length - 1; i >= 0; i--)
		{
			if(PlayArea.instance.specialCardDropZones[i].cardPlaced && !PlayArea.instance.specialCardDropZones[i].locked)
			{
				PlayArea.instance.specialCardDropZones[i].placedCard.dropZonePlacedIn = null;
				PlayArea.instance.specialCardDropZones[i].placedCard.RevertToOriginalImage();
				PlayArea.instance.specialCardDropZones[i].placedCard.transform.SetParent(cardParent);
				PlayArea.instance.specialCardDropZones[i].placedCard.transform.SetSiblingIndex(0);
				PlayArea.instance.specialCardDropZones[i].CardRemoved();
			}
		}
		PlayArea.instance.HandUpdated();
		SoundManager.instance.PlayCardSlideSound();
		recallButton.ChangeButtonEnabled(false);
		ReorganizeHand();
	}
	
	public void PlaySelectedClicked()
	{
		if(PlayArea.instance.locked || selectedCards.Count == 0)
		{
			return;
		}
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage == 3)
			{
				if(selectedCards.Count == 4)
				{
					int selectedTwos = 0;
					int selectedQueens = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 0)
						{
							selectedTwos++;
						}
						else if(selectedCards[i].cardData.rank == 10)
						{
							selectedQueens++;
						}
					}
					if(selectedTwos == 2 && selectedQueens == 2)
					{
						Tutorial.instance.IncrementStage();
					}
					else
					{
						MinorNotifications.instance.NewMinorNotification("Select your two queens and two aces!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(playSelectedButton.rt, GameManager.instance.gameplayCanvas));
						return;
					}
				}
				else
				{
					MinorNotifications.instance.NewMinorNotification("Select your two queens and two aces!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(playSelectedButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
			}
			else if(Tutorial.instance.currentStage == 11)
			{
				if(selectedCards.Count == 5)
				{
					int selectedSixes = 0;
					int selectedSevens = 0;
					int selectedEights = 0;
					int selectedNines = 0;
					int selectedTens = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 4)
						{
							selectedSixes++;
						}
						else if(selectedCards[i].cardData.rank == 5)
						{
							selectedSevens++;
						}
						else if(selectedCards[i].cardData.rank == 6)
						{
							selectedEights++;
						}
						else if(selectedCards[i].cardData.rank == 7)
						{
							selectedNines++;
						}
						else if(selectedCards[i].cardData.rank == 8)
						{
							selectedTens++;
						}
					}
					if(selectedSixes == 1 && selectedSevens == 1 && selectedEights == 1 && selectedNines == 1 && selectedTens == 1)
					{
						Tutorial.instance.IncrementStage();
					}
					else
					{
						MinorNotifications.instance.NewMinorNotification("Select your straight!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(playSelectedButton.rt, GameManager.instance.gameplayCanvas));
						return;
					}
				}
				else
				{
					MinorNotifications.instance.NewMinorNotification("Select your straight!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(playSelectedButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
			}
			else if(Tutorial.instance.currentStage == 20)
			{
				if(selectedCards.Count == 4)
				{
					int selectedNines = 0;
					int selectedTens = 0;
					int selectedJacks = 0;
					int selectedQueens = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 7)
						{
							selectedNines++;
						}
						else if(selectedCards[i].cardData.rank == 8)
						{
							selectedTens++;
						}
						else if(selectedCards[i].cardData.rank == 9)
						{
							selectedJacks++;
						}
						else if(selectedCards[i].cardData.rank == 10)
						{
							selectedQueens++;
						}
					}
					if(selectedNines == 1 && selectedTens == 1 && selectedJacks == 1 && selectedQueens == 1)
					{
						Tutorial.instance.IncrementStage();
					}
					else
					{
						MinorNotifications.instance.NewMinorNotification("Select your straight!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(playSelectedButton.rt, GameManager.instance.gameplayCanvas));
						return;
					}
				}
			}
			else
			{
				if(Tutorial.instance.currentStage < 3)
				{
					MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(playSelectedButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
			}
		}
		int numberOfStandardCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, false);
		int numberOfSpecialCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, true);
		int numberOfEmptyStandardDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.standardDropZones).Count;
		int numberOfEmptySpecialDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.specialCardDropZones).Count;
		if(!V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect && Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones") <= 0)
		{
			if(numberOfStandardCardsSelected <= numberOfEmptyStandardDropZones && numberOfSpecialCardsSelected <= numberOfEmptySpecialDropZones)
			{
				
			}
			else
			{
				return;
			}
		}
		else
		{
			if(numberOfStandardCardsSelected <= numberOfEmptyStandardDropZones && numberOfStandardCardsSelected + numberOfSpecialCardsSelected <= numberOfEmptyStandardDropZones + numberOfEmptySpecialDropZones)
			{
				
			}
			else
			{
				return;
			}
		}
		List<DropZone> emptyStandardDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.standardDropZones);
		List<DropZone> emptySpecialDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.specialCardDropZones);
		selectedCards.Sort((x, y) =>
		{
			return x.rt.anchoredPosition.x.CompareTo(y.rt.anchoredPosition.x);
		});
		int standardCardIndex = 0;
		int specialCardIndex = 0;
		for(int i = 0; i < selectedCards.Count; i++)
		{
			DropZone newDropZoneAtEnd = null;
			if(selectedCards[i].cardData.isSpecialCard && specialCardIndex < emptySpecialDropZones.Count)
			{
				// selectedCards[i].transform.SetParent(emptySpecialDropZones[specialCardIndex].transform);
				// destination = LocalInterface.GetCanvasPositionOfRectTransform(emptySpecialDropZones[specialCardIndex].rt);
				// newParentAtEnd = emptySpecialDropZones[specialCardIndex].transform;
				newDropZoneAtEnd = emptySpecialDropZones[specialCardIndex];
				selectedCards[i].dropZonePlacedIn = emptySpecialDropZones[specialCardIndex];
				emptySpecialDropZones[specialCardIndex].CardPlaced(selectedCards[i]);
				selectedCards[i].UpdateToDropZoneImage();
				specialCardIndex++;
			}
			else
			{
				// selectedCards[i].transform.SetParent(emptyStandardDropZones[standardCardIndex].transform);
				// destination = LocalInterface.GetCanvasPositionOfRectTransform(emptyStandardDropZones[standardCardIndex].rt);
				// newParentAtEnd = emptyStandardDropZones[standardCardIndex].transform;
				newDropZoneAtEnd = emptyStandardDropZones[standardCardIndex];
				selectedCards[i].dropZonePlacedIn = emptyStandardDropZones[standardCardIndex];
				emptyStandardDropZones[standardCardIndex].CardPlaced(selectedCards[i]);
				if(selectedCards[i].cardData.isSpecialCard)
				{
					selectedCards[i].UpdateToDropZoneImage();
				}
				standardCardIndex++;
			}
			selectedCards[i].transform.SetParent(movingCardsParent);
			selectedCards[i].StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(newDropZoneAtEnd.rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, newDropZoneAtEnd);
			//public void StartMove(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd = true, bool destroyAtEnd = false, bool discardAtEnd = false, bool addToDrawPileAtEnd = false, RectTransform newParentAtEnd = null)
		}
		selectedCards.Clear();
		SelectedCardsUpdated();
		PlayArea.instance.HandUpdated();
		ReorganizeHand();
		SoundManager.instance.PlayCardDropSound();
	}
	
	public void DiscardClicked()
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage == 9)
			{
				if(selectedCards.Count == 3)
				{
					int selectedThrees = 0;
					int selectedKings = 0;
					int selectedAces = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 1)
						{
							selectedThrees++;
						}
						else if(selectedCards[i].cardData.rank == 11)
						{
							selectedKings++;
						}
						else if(selectedCards[i].cardData.rank == 12)
						{
							selectedAces++;
						}
					}
					if(selectedThrees == 1 && selectedKings == 1 && selectedAces == 1)
					{
						// Tutorial.instance.IncrementStage();
						// will be handled by the draw coroutine
					}
					else
					{
						MinorNotifications.instance.NewMinorNotification("Please select your 3, A and K", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(discardButton.rt, GameManager.instance.gameplayCanvas));
						return;
					}
				}
			}
			else
			{
				MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(discardButton.rt, GameManager.instance.gameplayCanvas));
				return;
			}
		}
		SoundManager.instance.PlayCardSlideSound();
		Stats.instance.DiscardUsed(selectedCards.Count);
		RunInformation.instance.cardsDiscarded += selectedCards.Count;
		DiscardCards(selectedCards, true);
		selectedCards.Clear();
		SelectedCardsUpdated();
		GameManager.instance.DiscardUsed();
		StartDrawCards();
		
	}
	
	public void DiscardCards(List<Card> cardsToDiscard, bool optedToDiscard)
	{
		// Debug.Log($"DiscardCards start cardsToDiscard.Count={cardsToDiscard.Count}");
		int numberOfAces = 0;
		int[] ranksDiscarded = new int[13];
		List<Vector2> locationsOfAces = new List<Vector2>();
		// Debug.Log($"DiscardCards called with cardsToDiscard.Count={cardsToDiscard.Count}");
		for(int i = cardsToDiscard.Count - 1; i >= 0; i--)
		{
			cardsControllerSelectionGroup.RemoveControllerSelectableObjectFromGroup(cardsToDiscard[i].controllerSelectableObject);
			// Debug.Log($"i={i}, cardsToDiscard[i].name={cardsToDiscard[i].name}");
			if(optedToDiscard && numberOfAces < 2 && !cardsToDiscard[i].cardData.isSpecialCard && (cardsToDiscard[i].cardData.rank == 12 || Baubles.instance.GetImpactInt("AllCardsAreAces") > 0) && Baubles.instance.GetImpactInt("DiscardAcesForMoney") > 0)
			{
				numberOfAces++;
				locationsOfAces.Add(cardsToDiscard[i].rt.anchoredPosition);
				// Debug.Log($"Incremented numberOfAces={numberOfAces}");
			}
			if(optedToDiscard && (!LocalInterface.instance.IsBaubleUnlocked("DiscardTriplesForMushrooms") || Baubles.instance.GetImpactInt("DiscardTriplesForMushrooms") > 0) && !cardsToDiscard[i].cardData.isSpecialCard)
			{
				ranksDiscarded[cardsToDiscard[i].cardData.rank]++;
			}
			cardsToDiscard[i].transform.SetParent(discardedCardsParent);
			cardsToDiscard[i].StartMove(discardPileLocation, Vector3.zero, false, true, true);
			if(!cardsToDiscard[i].faceDown)
			{
				cardsToDiscard[i].StartFlip();
			}
			Deck.instance.cardsInHand.Remove(cardsToDiscard[i]);
		}
		ControllerSelection.instance.MoveSelectionIfCurrentlySelectedObjectIsNoLongerInCurrentControllerSelectionGroups();
		if(optedToDiscard && numberOfAces >= 2)
		{
			Baubles.instance.Notify("DiscardAcesForMoney");
			for(int j = 0; j < 2; j++)
			{
				GameObject newChipGO = Instantiate(GameManager.instance.chipPrefab, GameManager.instance.chipsParent);
				Chip newChip = newChipGO.GetComponent<Chip>();
				newChip.rt.anchoredPosition = locationsOfAces[j];
				newChip.StartMove();
				// Debug.Log($"Just sent new chip j={j}");
			}
		}
		if(optedToDiscard && !LocalInterface.instance.IsBaubleUnlocked("DiscardTriplesForMushrooms") || Baubles.instance.GetImpactInt("DiscardTriplesForMushrooms") > 0)
		{
			for(int i = 0; i < 13; i++)
			{
				if(ranksDiscarded[i] >= 3)
				{
					if(!LocalInterface.instance.IsBaubleUnlocked("DiscardTriplesForMushrooms"))
					{
						Stats.instance.ThreeOfAKindDiscarded();
					}
					else
					{
						Card newCard = SpawnCard(new CardData(-1, -1, "IncreaseValue"), deckLocation, cardParent);
						Deck.instance.cardsInHand.Add(newCard);
						ReorganizeHand();
						cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
						Baubles.instance.Notify("DiscardTriplesForMushrooms", 0, SoundManager.instance.PlayFriendlyFrogSound);
					}
				}
			}
		}
	}
	
	public void DiscardEntireHand()
	{
		List<Card> cardsInHandToDiscard = Deck.instance.cardsInHand;
		DiscardCards(cardsInHandToDiscard, false);
	}
	
	public int GetMaxCardsDiscardedAtOnce()
	{
		return V.i.v.variantSpecialOptions["MaxCardsDiscarded"].impact + Baubles.instance.GetImpactInt("AllowDiscardingMore");
	}
	
	public void SelectedCardsUpdated()
	{
		if(GameManager.instance.discardsRemaining > 0)
		{
			if(selectedCards.Count > 0 && selectedCards.Count <= GetMaxCardsDiscardedAtOnce() && !PlayArea.instance.locked)
			{
				discardButton.ChangeButtonEnabled(true);
			}
			else if(selectedCards.Count <= 0 || selectedCards.Count > GetMaxCardsDiscardedAtOnce() || PlayArea.instance.locked)
			{
				discardButton.ChangeButtonEnabled(false);
			}
		}
		for(int i = 0; i < Deck.instance.cardsInHand.Count; i++)
		{
			if(selectedCards.Contains(Deck.instance.cardsInHand[i]))
			{
				Deck.instance.cardsInHand[i].selectionGlowRT.gameObject.SetActive(true);
			}
			else
			{
				Deck.instance.cardsInHand[i].selectionGlowRT.gameObject.SetActive(false);
			}
		}
		int numberOfStandardCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, false);
		int numberOfSpecialCardsSelected = GetNumberOfCardsOfTypeInList(selectedCards, true);
		int numberOfEmptyStandardDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.standardDropZones).Count;
		int numberOfEmptySpecialDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.specialCardDropZones).Count;
		if(selectedCards.Count > 0 && !PlayArea.instance.locked)
		{
			if(!V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect && Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones") <= 0)
			{
				if(numberOfStandardCardsSelected <= numberOfEmptyStandardDropZones && numberOfSpecialCardsSelected <= numberOfEmptySpecialDropZones)
				{
					playSelectedButton.ChangeButtonEnabled(true);
				}
				else
				{
					playSelectedButton.ChangeButtonEnabled(false);
				}
			}
			else
			{
				if(numberOfStandardCardsSelected <= numberOfEmptyStandardDropZones && numberOfStandardCardsSelected + numberOfSpecialCardsSelected <= numberOfEmptyStandardDropZones + numberOfEmptySpecialDropZones)
				{
					playSelectedButton.ChangeButtonEnabled(true);
				}
				else
				{
					playSelectedButton.ChangeButtonEnabled(false);
				}
			}
		}
		else
		{
			playSelectedButton.ChangeButtonEnabled(false);
		}
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage == 2)
			{
				if(selectedCards.Count == 4)
				{
					int selectedTwos = 0;
					int selectedQueens = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 0)
						{
							selectedTwos++;
						}
						else if(selectedCards[i].cardData.rank == 10)
						{
							selectedQueens++;
						}
					}
					if(selectedTwos == 2 && selectedQueens == 2)
					{
						Tutorial.instance.IncrementStage();
					}
				}
			}
			else if(Tutorial.instance.currentStage == 8)
			{
				if(selectedCards.Count == 3)
				{
					int selectedThrees = 0;
					int selectedKings = 0;
					int selectedAces = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 1)
						{
							selectedThrees++;
						}
						else if(selectedCards[i].cardData.rank == 11)
						{
							selectedKings++;
						}
						else if(selectedCards[i].cardData.rank == 12)
						{
							selectedAces++;
						}
					}
					if(selectedThrees == 1 && selectedKings == 1 && selectedAces == 1)
					{
						Tutorial.instance.IncrementStage();
					}
				}
			}
			else if(Tutorial.instance.currentStage == 10)
			{
				if(selectedCards.Count == 5)
				{
					int selectedSixes = 0;
					int selectedSevens = 0;
					int selectedEights = 0;
					int selectedNines = 0;
					int selectedTens = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 4)
						{
							selectedSixes++;
						}
						else if(selectedCards[i].cardData.rank == 5)
						{
							selectedSevens++;
						}
						else if(selectedCards[i].cardData.rank == 6)
						{
							selectedEights++;
						}
						else if(selectedCards[i].cardData.rank == 7)
						{
							selectedNines++;
						}
						else if(selectedCards[i].cardData.rank == 8)
						{
							selectedTens++;
						}
					}
					if(selectedSixes == 1 && selectedSevens == 1 && selectedEights == 1 && selectedNines == 1 && selectedTens == 1)
					{
						Tutorial.instance.IncrementStage();
					}
				}
			}
			else if(Tutorial.instance.currentStage == 19)
			{
				if(selectedCards.Count == 4)
				{
					int selectedNines = 0;
					int selectedTens = 0;
					int selectedJacks = 0;
					int selectedQueens = 0;
					for(int i = 0; i < selectedCards.Count; i++)
					{
						if(selectedCards[i].cardData.rank == 7)
						{
							selectedNines++;
						}
						else if(selectedCards[i].cardData.rank == 8)
						{
							selectedTens++;
						}
						else if(selectedCards[i].cardData.rank == 9)
						{
							selectedJacks++;
						}
						else if(selectedCards[i].cardData.rank == 10)
						{
							selectedQueens++;
						}
					}
					if(selectedNines == 1 && selectedTens == 1 && selectedJacks == 1 && selectedQueens == 1)
					{
						Tutorial.instance.IncrementStage();
					}
				}
			}
		}
	}
}
