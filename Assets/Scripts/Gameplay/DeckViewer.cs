using UnityEngine;
using UnityEngine.UI;
using static Deck;
using System.Collections.Generic;
using System.Collections;

public class DeckViewer : MonoBehaviour
{
    public Image[] suitImages;
	public RectTransform[] suitBackdropRTs;
	public RectTransform interactionBlocker;
	public RectTransform cardParent;
	public Label[] rankLabels;
	public Label[] suitLabels;
	public Label acesLabel;
	public Label faceCardsLabel;
	public Label numberedCardsLabel;
	public Label specialCardsLabel;
	public Label deckNameLabel;
	public Label deckDescriptionLabel;
	public Label totalCardsLabel;
	public Image deckImage;
	public ButtonPlus backButton;
	public ButtonPlus drawPileButton;
	public ButtonPlus fullDeckButton;
	public ButtonPlus discardedButton;
	public GameObject visibilityObject;
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public Vector2 cardAreaSize;
	public Vector2 optimalCardDistanceDelta;
	public const int maxNumberOfCardsPerRow = 20;
	public const float maxWidth = 466f;
	public const float idealDistanceBetweenItems = 8f;
	
	public List<Card> cards = new List<Card>();
	public List<Card> availableCards = new List<Card>();
	// public List<ControllerSelectionGroup> previousControllerSelectionGroups = new List<ControllerSelectionGroup>();
	
	public static DeckViewer instance;
	
	public void SetupInstance()
	{
		instance = this;
		UpdateSuitColorsAndPositions();
		deckNameLabel.ChangeText(V.i.chosenDeck);
		if(V.i.chosenDeck.Length > 6)
		{
			deckNameLabel.ChangeFontSize(8);
		}
		deckDescriptionLabel.ChangeText(V.i.chosenDeckDescription);
		deckImage.sprite = V.i.chosenDeckSprite;
		visibilityObject.SetActive(false);
	}
	
	public void SetInteractability(bool enabledState)
	{
		backButton.ChangeButtonEnabled(enabledState);
		drawPileButton.ChangeButtonEnabled(enabledState);
		fullDeckButton.ChangeButtonEnabled(enabledState);
		discardedButton.ChangeButtonEnabled(enabledState);
	}
	
	public void OpenDeckViewer(int origin) // 1 = draw pile, 2 = discard pile
	{
		// previousControllerSelectionGroups = new List<ControllerSelectionGroup>(ControllerSelection.instance.currentControllerSelectionGroups);
		visibilityObject.SetActive(true);
		MovingObjects.instance.mo["DeckViewer"].StartMove("OnScreen");
		switch(origin)
		{
			case 1:
				ChangeViewedCards(true, false, false, true);
				drawPileButton.ChangeSpecialState(true);
				fullDeckButton.ChangeSpecialState(false);
				discardedButton.ChangeSpecialState(false);
			break;
			case 2:
				ChangeViewedCards(false, false, true, false);
				drawPileButton.ChangeSpecialState(false);
				fullDeckButton.ChangeSpecialState(false);
				discardedButton.ChangeSpecialState(true);
			break;
		}
		SetInteractability(false);
		interactionBlocker.sizeDelta = new Vector2(640f, 720f);
	}
	
	public void FinishOpeningDeckViewer()
	{
		SetInteractability(true);
	}
	
	public void DrawPileClicked()
	{
		ChangeViewedCards(true, false, false, true);
		drawPileButton.ChangeSpecialState(true);
		fullDeckButton.ChangeSpecialState(false);
		discardedButton.ChangeSpecialState(false);
	}
	
	public void FullDeckClicked()
	{
		ChangeViewedCards(true, true, true, true);
		drawPileButton.ChangeSpecialState(false);
		fullDeckButton.ChangeSpecialState(true);
		discardedButton.ChangeSpecialState(false);
	}
	
	public void DiscardedClicked()
	{
		ChangeViewedCards(false, false, true, false);
		drawPileButton.ChangeSpecialState(false);
		fullDeckButton.ChangeSpecialState(false);
		discardedButton.ChangeSpecialState(true);
	}
	
	public void CloseDeckViewer()
	{
		SetInteractability(false);
		MovingObjects.instance.mo["DeckViewer"].StartMove("OffScreen");
		interactionBlocker.sizeDelta = new Vector2(640f, 360f);
	}
	
	public void DisableDeckViewer()
	{
		visibilityObject.SetActive(false);
	}
	
	public void UpdateSuitColorsAndPositions()
	{
		for(int i = 0; i < 4; i++)
		{
			suitBackdropRTs[i].anchoredPosition = new Vector2(2 + 22 * LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[i]], suitBackdropRTs[i].anchoredPosition.y);
			suitImages[i].color = LocalInterface.instance.suitColors[i];
		}
	}
	
	public void ChangeViewedCards(bool drawPile, bool inHand, bool discarded, bool topDeckCard)
	{
		List<CardData> cardsToShow = new List<CardData>();
		if(drawPile)
		{
			cardsToShow.AddRange(Deck.instance.drawPile);
		}
		if(discarded)
		{
			cardsToShow.AddRange(Deck.instance.discardPile);
		}
		if(inHand)
		{
			for(int i = 0; i < Deck.instance.cardsInHand.Count; i++)
			{
				cardsToShow.Add(Deck.instance.cardsInHand[i].cardData);
			}
		}
		if(topDeckCard)
		{
			if(HandArea.instance.topDeckCard != null)
			{
				cardsToShow.Add(HandArea.instance.topDeckCard.cardData);
			}
		}
		cardsToShow.Sort((x,y) => 
		{
			if(x.isSpecialCard && y.isSpecialCard)
			{
				string xSpecialCardName = "";
				if(x.specialCardName.Length >= 18 && x.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
				{
					xSpecialCardName = "Four Piece Puzzle";
				}
				else
				{
					xSpecialCardName = V.i.v.variantSpecialCards[x.specialCardName].specialCardName;
				}
				string ySpecialCardName = "";
				if(y.specialCardName.Length >= 18 && y.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
				{
					ySpecialCardName = "Four Piece Puzzle";
				}
				else
				{
					ySpecialCardName = V.i.v.variantSpecialCards[y.specialCardName].specialCardName;
				}
				return xSpecialCardName.CompareTo(ySpecialCardName);
			}
			if(x.isSpecialCard && !y.isSpecialCard)
			{
				return 1;
			}
			if(!x.isSpecialCard && y.isSpecialCard)
			{
				return -1;
			}
			int suitComparison = LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[x.suit]] - LocalInterface.instance.suitOrderDictionary[LocalInterface.instance.suitNames[y.suit]];
			if(suitComparison != 0)
			{
				return suitComparison;
			}
			else
			{
				return x.rank - y.rank;
			}
		});
		for(int i = 0; i < cards.Count; i++)
		{
			if(!availableCards.Contains(cards[i]))
			{
				availableCards.Add(cards[i]);
			}
		}
		// for(int i = cardsToShow.Count; i < cards.Count; i++)
		for(int i = 0; i < cards.Count; i++)
		{
			cards[i].gameObject.SetActive(false);
		}
		int[] suitCards = new int[5];
		int[] rankCards = new int[13];
		int numberedCards = 0;
		int faceCards = 0;
		int aceCards = 0;
		int specialCards = 0;
		for(int i = 0; i < cardsToShow.Count; i++)
		{
			if(cardsToShow[i].isSpecialCard)
			{
				specialCards++;
			}
			else
			{
				rankCards[cardsToShow[i].rank]++;
				suitCards[cardsToShow[i].suit]++;
				if(cardsToShow[i].rank == 12 || Baubles.instance.GetImpactInt("AllCardsAreAces") > 0)
				{
					aceCards++;
				}
				if((cardsToShow[i].rank >= 9 && cardsToShow[i].rank <= 11) || Baubles.instance.GetImpactInt("AllCardsAreFaceCards") > 0)
				{
					faceCards++;
				}
				if(cardsToShow[i].rank < 9 || Baubles.instance.GetImpactInt("AllCardsAreNumberedCards") > 0)
				{
					numberedCards++;
				}
			}
		}
		UpdateLabel(acesLabel, rankCards[12]);
		UpdateLabel(faceCardsLabel, faceCards);
		UpdateLabel(numberedCardsLabel, numberedCards);
		UpdateLabel(specialCardsLabel, specialCards);
		totalCardsLabel.ChangeText(cardsToShow.Count.ToString());
		if(cardsToShow.Count >= 1000)
		{
			totalCardsLabel.ChangeFontSize(8);
		}
		else
		{
			totalCardsLabel.ChangeFontSize(16);
		}
		for(int i = 0; i < 13; i++)
		{
			UpdateLabel(rankLabels[i], rankCards[i]);
		}
		int[] typeRows = new int[6];
		// List<int> typeRows = new List<int>;
		int numberOfRows = 0;
		for(int i = 0; i < 5; i++)
		{
			if(suitCards[i] > 0)
			{
				numberOfRows++;
				typeRows[i]++;
			}
			if(i < 4)
			{
				UpdateLabel(suitLabels[i], suitCards[i]);
			}
			else
			{
				suitLabels[i].ChangeText(suitCards[i].ToString());
				suitBackdropRTs[i].sizeDelta = new Vector2(Mathf.Max(suitLabels[i].GetPreferredWidth() + 4f, 20f), suitBackdropRTs[i].sizeDelta.y);
			}
		}
		if(specialCards > 0)
		{
			numberOfRows++;
			typeRows[5]++;
		}
		bool addedNewRow = true;
		while(numberOfRows < 6 && addedNewRow)
		{
			addedNewRow = false;
			if(specialCards > typeRows[5] * maxNumberOfCardsPerRow)
			{
				typeRows[5]++;
				numberOfRows++;
				addedNewRow = true;
			}
			for(int i = 4; i >= 0; i--)
			{
				if(suitCards[i] > typeRows[i] * maxNumberOfCardsPerRow && numberOfRows < 6)
				{
					typeRows[i]++;
					numberOfRows++;
					addedNewRow = true;
				}
			}
		}
		StartCoroutine(SpawnCards(cardsToShow, typeRows, numberOfRows, suitCards, specialCards));
	}
	
	public IEnumerator SpawnCards(List<CardData> cardsToShow, int[] typeRows, int numberOfRows, int[] suitCards, int specialCards)
	{
		int currentRow = 0;
		CardData lastCardData = null;
		int[] cardsInEachRow = new int[numberOfRows];
		int currentCategoryAmount = 0;
		int currentCategoryRow = 0;
		for(int i = 0; i < cardsToShow.Count; i++)
		{
			if(lastCardData != null && (lastCardData.isSpecialCard != cardsToShow[i].isSpecialCard || lastCardData.suit != cardsToShow[i].suit))
			{
				currentCategoryAmount = 0;
				currentCategoryRow = 0;
				currentRow++;
			}
			if(!cardsToShow[i].isSpecialCard)
			{
				if(typeRows[cardsToShow[i].suit] > 1)
				{
					if(currentCategoryAmount > suitCards[cardsToShow[i].suit] / typeRows[cardsToShow[i].suit] + currentCategoryRow * (suitCards[cardsToShow[i].suit] / typeRows[cardsToShow[i].suit]))
					{
						currentRow++;
						currentCategoryRow++;
						currentCategoryAmount = 0;
					}
				}
			}
			else if(typeRows[5] > 1)
			{
				if(currentCategoryAmount > specialCards / typeRows[5])// + currentCategoryRow * specialCards / typeRows[5])
				{
					currentRow++;
					currentCategoryRow++;
					currentCategoryAmount = 0;
				}
			}
			cardsToShow[i].deckViewerRow = currentRow;
			cardsToShow[i].deckViewerRowIndex = currentCategoryAmount;
			// Debug.Log($"i={i}, currentRow={currentRow}, currentCategoryRow={currentCategoryRow}, numberOfRows={numberOfRows}");
			cardsInEachRow[currentRow]++;
			lastCardData = cardsToShow[i];
			currentCategoryAmount++;
		}
		for(int i = 0; i < cardsToShow.Count; i++)
		{
			float squeezeDistance =  (maxWidth - LocalInterface.instance.cardSize.x) / (cardsInEachRow[cardsToShow[i].deckViewerRow] - 1);
			float xDelta = Mathf.Min(LocalInterface.instance.cardSize.x + idealDistanceBetweenItems, squeezeDistance);
			if(availableCards.Count > 0)
			{
				availableCards[availableCards.Count - 1].gameObject.SetActive(true);
				availableCards[availableCards.Count - 1].cardData = cardsToShow[i];
				availableCards[availableCards.Count - 1].UpdateGraphics();
				availableCards[availableCards.Count - 1].rt.anchoredPosition = new Vector2((cardsInEachRow[cardsToShow[i].deckViewerRow] - 1) * xDelta / 2 - (cardsInEachRow[cardsToShow[i].deckViewerRow] - cardsToShow[i].deckViewerRowIndex - 1) * xDelta, 26f * (numberOfRows - 1) - cardsToShow[i].deckViewerRow * 52f);
				// availableCards[availableCards.Count - 1].rt.anchoredPosition = new Vector2((cardsInEachRow[cardsToShow[i].deckViewerRow] - 1) * -26f + 52 * cardsToShow[i].deckViewerRowIndex, -26f * numberOfRows + cardsToShow[i].deckViewerRow * 52f);
				availableCards[availableCards.Count - 1].rt.SetSiblingIndex(availableCards[availableCards.Count - 1].rt.parent.childCount - 1);
				availableCards.RemoveAt(availableCards.Count - 1);
			}
			else
			{
				Card newCard = HandArea.instance.SpawnCard(cardsToShow[i], new Vector2((cardsInEachRow[cardsToShow[i].deckViewerRow] - 1) * xDelta / 2 - (cardsInEachRow[cardsToShow[i].deckViewerRow] - cardsToShow[i].deckViewerRowIndex - 1) * xDelta, 26f * (numberOfRows - 1) - cardsToShow[i].deckViewerRow * 52f), cardParent, false, false);
				controllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
				// Card newCard = HandArea.instance.SpawnCard(cardsToShow[i], new Vector2((cardsInEachRow[cardsToShow[i].deckViewerRow] - 1) * -26f + 52 * cardsToShow[i].deckViewerRowIndex, -26f * numberOfRows + cardsToShow[i].deckViewerRow * 52f), cardParent, false, false);
				newCard.isDeckviewerClone = true;
				newCard.rt.SetSiblingIndex(newCard.rt.parent.childCount - 1);
				cards.Add(newCard);
			}
		}
		yield return null;
	}
	
	public void UpdateLabel(Label label, int newInt)
	{
		label.ChangeText(newInt.ToString());
		if(newInt >= 10)
		{
			label.ChangeFontSize(8);
		}
		else
		{
			label.ChangeFontSize(16);
		}
	}
}
