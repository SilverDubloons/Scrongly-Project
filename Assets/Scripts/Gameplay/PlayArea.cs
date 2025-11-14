using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using static Deck;
using static GameManager;
using static Variant;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class PlayArea : MonoBehaviour, IPointerClickHandler
{
	public RectTransform rt;
	public RectTransform specialDropZonesRT;
	public RectTransform pointsRT;
	public Label handNameLabel;
	public ButtonPlus lockInHandButton;
	public Image lockInHandButtonImage;
	public RectTransform lockInHandButtonImageRT;
	public Label pointsLabel;
	public Label multLabel;
	public RectTransform crosshair;
	public RectTransform snurfGunDart;
	public RectTransform bombExplosionParent;
	public RectTransform flushesContainedRT;
	public Image[] flushesContainedImages;
	
	public Color redXColor;
	public Color greenXColor;
    public DropZone[] standardDropZones;
    public DropZone[] specialCardDropZones;
	
	public Sprite unlockedButtonSprite;
	public Sprite lockedButtonSprite;
	public Sprite magicMirrorMisplacedSprite;
	public GameObject rainbowPaintPrefab;
	public GameObject dissolvePrefab;
	public GameObject magicMarkerPrefab;
	public GameObject promotionPrefab;
	public GameObject magicMirrorPrefab;
	public GameObject bombExplosionPrefab;
	
	public static PlayArea instance;
	public bool locked;
	public double currentPoints;
	public double currentMult;
	public List<Card> currentScoredCards = new List<Card>();
	public int currentHandTier;
	public bool[] currentHandsContained;
	public bool[] currentFlushesContained;
	public double pointsGainedFromCards;
	public double multGainedFromCards;
	
	private const float scorePlateYDifference = 11f;
	
	public string currentStateOfScoringRoutine;
	
	public void SetupInstance()
	{
		instance = this;
		crosshair.gameObject.SetActive(false);
	}
	
	public void SetInteractability(bool enabledState)
	{
		if(!enabledState)
		{
			lockInHandButton.ChangeButtonEnabled(false);
		}
		else
		{
			if(!locked)
			{
				HandUpdated();
			}
		}
	}
	
	public void ResizePlayArea()
	{
		int standard = GameManager.instance.GetMaxPlayableStandardCards();
		int special = GameManager.instance.GetNumberOfSpecialCardOnlyDropZones();
		for(int i = 0; i < 7; i++)
		{
			if(i < standard)
			{
				standardDropZones[i].gameObject.SetActive(true);
			}
			else
			{
				standardDropZones[i].gameObject.SetActive(false);
			}
		}
		for(int i = 0; i < 7; i++)
		{
			if(i < special)
			{
				specialCardDropZones[i].gameObject.SetActive(true);
			}
			else
			{
				specialCardDropZones[i].gameObject.SetActive(false);
			}
		}
		if(standard < 5)
		{
			standard = 5;
		}
		rt.sizeDelta = new Vector2(Mathf.Max(standard, special) * 48 + 64, rt.sizeDelta.y);
		pointsRT.anchoredPosition = new Vector2(10 + (standard - 5) * 24, pointsRT.anchoredPosition.y);
		if(special == 0)
		{
			specialDropZonesRT.gameObject.SetActive(false);
		}
		else
		{
			specialDropZonesRT.gameObject.SetActive(true);
			specialDropZonesRT.sizeDelta = new Vector2(special * 48 + 4, specialDropZonesRT.sizeDelta.y);
		}
	}
	
	public List<DropZone> GetEmptyDropZonesFromArray(DropZone[] dropZoneArray)
	{
		List<DropZone> emptyDropZones = new List<DropZone>();
		for(int i = 0; i < dropZoneArray.Length; i++)
		{
			if(dropZoneArray[i].gameObject.activeSelf && !dropZoneArray[i].cardPlaced)
			{
				emptyDropZones.Add(dropZoneArray[i]);
			}
		}
		return emptyDropZones;
	}
	
	public int GetNumberOfEmptyDropZones(bool standard)
	{
		int numberOfEmptyDropZones = 0;
		if(standard)
		{
			for(int i = 0; i < standardDropZones.Length; i++)
			{
				if(standardDropZones[i].gameObject.activeSelf && !standardDropZones[i].cardPlaced)
				{
					numberOfEmptyDropZones++;
				}
			}
		}
		else
		{
			for(int i = 0; i < specialCardDropZones.Length; i++)
			{
				if(specialCardDropZones[i].gameObject.activeSelf && !specialCardDropZones[i].cardPlaced)
				{
					numberOfEmptyDropZones++;
				}
			}
		}
		return numberOfEmptyDropZones;
	}
	
	public DropZone GetEmptyDropZoneToTheRight(DropZone dropZone)
	{
		if(dropZone.specialCardsOnly)
		{
			for(int i = dropZone.dropZoneNumber; i < GameManager.instance.GetNumberOfSpecialCardOnlyDropZones(); i++)
			{
				if(!specialCardDropZones[i].cardPlaced)
				{
					return specialCardDropZones[i];
				}
			}
			return null;
		}
		else
		{
			for(int i = dropZone.dropZoneNumber; i < GameManager.instance.GetMaxPlayableStandardCards(); i++)
			{
				if(!standardDropZones[i].cardPlaced)
				{
					return standardDropZones[i];
				}
			}
			return null;
		}
	}
	
	public DropZone GetEmptyDropZoneToTheLeft(DropZone dropZone)
	{
		if(dropZone.specialCardsOnly)
		{
			for(int i = dropZone.dropZoneNumber; i >= 0; i--)
			{
				if(!specialCardDropZones[i].cardPlaced)
				{
					return specialCardDropZones[i];
				}
			}
			return null;
		}
		else
		{
			for(int i = dropZone.dropZoneNumber; i >= 0; i--)
			{
				if(!standardDropZones[i].cardPlaced)
				{
					return standardDropZones[i];
				}
			}
			return null;
		}
	}
	
	public bool AreThereAnyPlacedCards()
	{
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				return true;
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				return true;
			}
		}
		return false;
	}
	
	public Card GetFirstStandardCard()
	{
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				return standardDropZones[i].placedCard;
			}
		}
		return null;
	}
	
	public void SetVisibilityOfMushroomMults(bool visible)
	{
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			standardDropZones[i].mushroomMultBackdrop.gameObject.SetActive(visible);
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			specialCardDropZones[i].mushroomMultBackdrop.gameObject.SetActive(visible);
		}
	}
	
	public void HandUpdated()
	{
		List<CardData> standardCards = GetAllStandardCardDatas();
		List<CardData> specialCards = GetAllSpecialCards();
		crosshair.gameObject.SetActive(false);
		SetVisibilityOfMushroomMults(false);
		flushesContainedRT.gameObject.SetActive(false);
		if(standardCards.Count + specialCards.Count > 0)
		{
			if(!locked)
			{
				HandArea.instance.recallButton.ChangeButtonEnabled(true);
			}
			if(standardCards.Count > 0)
			{
				if(!locked)
				{
					lockInHandButton.ChangeButtonEnabled(true);
				}
				if(standardCards.Count == 1 && GetTotalNumberOfCards() == 1 && Baubles.instance.GetImpactInt("DestroySoloStandardCard") > 0 && !locked)
				{
					crosshair.gameObject.SetActive(true);
					Card onlyStandardCard = GetFirstStandardCard();
					onlyStandardCard.dropZonePlacedIn.xImage.gameObject.SetActive(false);
					crosshair.SetParent(onlyStandardCard.dropZonePlacedIn.rt);
					crosshair.SetSiblingIndex(crosshair.parent.childCount - 1);
					crosshair.anchoredPosition = Vector2.zero;
					crosshair.localRotation = Quaternion.identity;
					crosshair.localScale = Vector3.one;
					handNameLabel.ChangeText("Blasted");
					currentPoints = 0;
					currentMult = 0;
					pointsLabel.ChangeText("0");
					multLabel.ChangeText("0");
					currentScoredCards.Clear();
					currentHandTier = -1;
					currentHandsContained = new bool[18];
					HandsInformation.instance.HandUpdated(currentHandsContained);
				}
				else
				{
					HandEvaluation.instance.EvaluateHand(standardCards, false);
				}
			}
			else
			{
				lockInHandButton.ChangeButtonEnabled(false);
				handNameLabel.ChangeText(string.Empty);
				pointsLabel.ChangeText("Points");
				multLabel.ChangeText("Mult");
				currentPoints = 0;
				currentMult = 0;
				currentScoredCards.Clear();
				currentHandTier = -1;
				currentHandsContained = new bool[18];
				HandsInformation.instance.HandUpdated(currentHandsContained);
			}
			if(HandContainsSpecialCardTag("ConvertCard"))
			{
				List<Card> conversionCards = GetAllSpecialCardsWithTag("ConvertCard");
				for(int i = 0; i < conversionCards.Count; i++)
				{
					conversionCards[i].UpdateToDropZoneImage();
				}
			}
		}
		else
		{
			lockInHandButton.ChangeButtonEnabled(false);
			HandArea.instance.recallButton.ChangeButtonEnabled(false);
			handNameLabel.ChangeText(string.Empty);
			pointsLabel.ChangeText("Points");
			multLabel.ChangeText("Mult");
			currentPoints = 0;
			currentMult = 0;
			currentScoredCards.Clear();
			currentHandTier = -1;
			currentHandsContained = new bool[18];
			HandsInformation.instance.HandUpdated(currentHandsContained);
		}
	}
	
	public void HandEvaluated(List<CardData> cardsUsed, bool evaluatingOnlyCardsUsed, bool[] handsContained, bool isRoyalFlush = false)
	{
		/* string cardsUsedString = string.Empty;
		for(int i = 0; i < cardsUsed.Count; i++)
		{
			cardsUsedString += $"cardsUsed[{i}]={cardsUsed[i].ConvertToText()}";
			if(i < cardsUsed.Count - 1)
			{
				cardsUsedString += ",";
			}
		}
		Debug.Log($"HandEvaluated. evaluatingOnlyCardsUsed={evaluatingOnlyCardsUsed}, cardsUsed.Count={cardsUsed.Count}, cardsUsedString={cardsUsedString}"); */
		if(cardsUsed != null)
		{
			if(GetNumberOfStandardCards() > cardsUsed.Count && !evaluatingOnlyCardsUsed && Baubles.instance.GetImpactInt("UseAllCardsInPlayArea") == 0)
			{
				HandEvaluation.instance.EvaluateHand(cardsUsed, true);
				return;
			}
		}
		currentScoredCards.Clear();
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			standardDropZones[i].xImage.gameObject.SetActive(true);
			standardDropZones[i].mushroomMultBackdrop.gameObject.SetActive(false);
			if(Baubles.instance.GetImpactInt("UseAllCardsInPlayArea") > 0)
			{
				standardDropZones[i].xImage.color = greenXColor;
			}
			else
			{
				standardDropZones[i].xImage.color = redXColor;
			}
			standardDropZones[i].xImage.transform.SetSiblingIndex(standardDropZones[i].transform.childCount - 1);
			standardDropZones[i].mushroomMultBackdrop.transform.SetSiblingIndex(standardDropZones[i].transform.childCount - 1);
			if(standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					for(int j = 0; j < cardsUsed.Count; j++)
					{
						if(standardDropZones[i].placedCard.cardData == cardsUsed[j])
						{
							currentScoredCards.Add(standardDropZones[i].placedCard);
							standardDropZones[i].xImage.gameObject.SetActive(false);
							break;
						}
					}
					if(Baubles.instance.GetImpactInt("UseAllCardsInPlayArea") > 0 && !currentScoredCards.Contains(standardDropZones[i].placedCard))
					{
						currentScoredCards.Add(standardDropZones[i].placedCard);
					}
				}
				else
				{
					standardDropZones[i].xImage.gameObject.SetActive(false);
				}
			}
			else
			{
				standardDropZones[i].xImage.gameObject.SetActive(false);
			}
		}
		if(Baubles.instance.GetImpactInt("IncreaseMushroomPowerTriples") > 0 && IsAMushroomPlayed())
		{
			for(int i = 0; i < currentScoredCards.Count; i++)
			{
				int cardsOfThisRank = GetNumberOfCardsOfRank(currentScoredCards[i].cardData.rank, true);
				if(cardsOfThisRank >= 3)
				{	
					int cardPowerFactor = Mathf.Min(Baubles.instance.GetImpactInt("IncreaseMushroomPowerTriples") + 1, cardsOfThisRank - 1);
					currentScoredCards[i].dropZonePlacedIn.UpdateMushroomData(true, cardPowerFactor);
				}
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			specialCardDropZones[i].mushroomMultBackdrop.gameObject.SetActive(false);
		}
		if(Baubles.instance.GetImpactInt("IncreaseMushroomPowerSames") > 0)
		{
			List<Card> increaseValueCards = GetAllSpecialCardsWithTag("IncreaseValue");
			if(increaseValueCards.Count > 1)
			{
				for(int i = 0; i < increaseValueCards.Count; i++)
				{
					increaseValueCards[i].dropZonePlacedIn.UpdateMushroomData(false, increaseValueCards.Count);
				}
			}
			List<Card> increaseMultCards = GetAllSpecialCardsWithTag("IncreaseMult");
			if(increaseMultCards.Count > 1)
			{
				for(int i = 0; i < increaseMultCards.Count; i++)
				{
					increaseMultCards[i].dropZonePlacedIn.UpdateMushroomData(false, increaseMultCards.Count);
				}
			}
			List<Card> doubleValueAndMultCards = GetAllSpecialCardsWithTag("DoubleValueAndMult");
			if(doubleValueAndMultCards.Count > 1)
			{
				for(int i = 0; i < doubleValueAndMultCards.Count; i++)
				{
					doubleValueAndMultCards[i].dropZonePlacedIn.UpdateMushroomData(false, doubleValueAndMultCards.Count);
				}
			}
		}
		currentHandTier = -1;
		for(int i = 17; i >= 0; i--)
		{
			if(handsContained[i])
			{
				currentHandTier = i;
				if(isRoyalFlush && currentHandTier == 8)
				{
					handNameLabel.ChangeText("Royal Flush");
				}
				else
				{
					handNameLabel.ChangeText(GameManager.instance.handNames[i]);
				}
				break;
			}
		}
		currentPoints = 0d;
		currentMult = 0d;
		if(currentHandTier >= 0)
		{
			for(int i = 0; i < handsContained.Length; i++)
			{
				if(handsContained[i])
				{
					string handTierString = LocalInterface.instance.GetZodiacBaubleStringFromHandTier(i);
					currentPoints += GameManager.instance.handPoints[i];
					currentPoints += Baubles.instance.GetImpactDouble(handTierString);
					currentMult += GameManager.instance.handMults[i];
					currentMult += Baubles.instance.GetImpactDouble(handTierString, true);
				}
			}
		}
		pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPoints));
		multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentMult));
		currentHandsContained = handsContained;
		HandsInformation.instance.HandUpdated(handsContained);
		currentFlushesContained = new bool[4];
		int[] cardsOfSuit = new int[5];
		for(int i = 0; i < currentScoredCards.Count; i++)
		{
			if(!currentScoredCards[i].cardData.isSpecialCard)
			{
				cardsOfSuit[currentScoredCards[i].cardData.suit]++;
			}
		}
		for(int i = 0; i < 4; i++)
		{
			currentFlushesContained[i] = cardsOfSuit[i] + cardsOfSuit[4] >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush();
		}
		int zodiacsFromFlushesImpact = Baubles.instance.GetImpactInt("ZodiacsFromFlushes");
		if(zodiacsFromFlushesImpact > 0)
		{
			for(int i = 0; i < 4; i++)
			{
				flushesContainedImages[i].gameObject.SetActive(false);
			}
			int flushesBothIncludedAndInBauble = 0;
			float currentWidth = 2f;
			for(int i = 0; i < zodiacsFromFlushesImpact; i++)
			{
				if(currentFlushesContained[GameManager.instance.flushZodiacBaubleSuitOrders[i]])
				{
					flushesContainedRT.gameObject.SetActive(true);
					flushesContainedImages[GameManager.instance.flushZodiacBaubleSuitOrders[i]].gameObject.SetActive(true);
					flushesContainedImages[GameManager.instance.flushZodiacBaubleSuitOrders[i]].color = LocalInterface.instance.suitColors[GameManager.instance.flushZodiacBaubleSuitOrders[i]];
					flushesContainedImages[GameManager.instance.flushZodiacBaubleSuitOrders[i]].rectTransform.anchoredPosition = new Vector2(currentWidth, flushesContainedImages[GameManager.instance.flushZodiacBaubleSuitOrders[i]].rectTransform.anchoredPosition.y);
					
					if(GameManager.instance.flushZodiacBaubleSuitOrders[i] == 1)
					{
						currentWidth += 2f;
					}
					currentWidth += 13f;
					flushesBothIncludedAndInBauble++;
				}
			}
			if(flushesBothIncludedAndInBauble > 0)
			{
				// currentWidth += 2f;
				flushesContainedRT.sizeDelta = new Vector2(currentWidth, flushesContainedRT.sizeDelta.y);
			}
		}
	}
	
	public void AddPoints(double pointsToAdd)
	{
		currentPoints += pointsToAdd;
		pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPoints));
		pointsLabel.StartExpandRetract();
	}
	
	public void MultiplyPoints(double multToMultiplyBy)
	{
		currentPoints = currentPoints * multToMultiplyBy;
		pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPoints));
		pointsLabel.StartExpandRetract();
	}
	
	public void AddMult(double multToAdd)
	{
		currentMult += multToAdd;
		multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentMult));
		multLabel.StartExpandRetract();
	}
	
	public void MultiplyMult(double multToMultiplyBy)
	{
		currentMult = currentMult * multToMultiplyBy;
		multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentMult));
		multLabel.StartExpandRetract();
	}
	
	public int GetNumberOfStandardCards()
	{
		int numberOfStandardCards = 0;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					numberOfStandardCards++;
				}
			}
		}
		return numberOfStandardCards;
	}
	
	public int GetTotalNumberOfCards()
	{
		int numberOfCards = 0;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				numberOfCards++;
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				numberOfCards++;
			}
		}
		return numberOfCards;
	}
	
	public List<CardData> GetAllStandardCardDatas()
	{
		List<CardData> standardCards = new List<CardData>();
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					standardCards.Add(standardDropZones[i].placedCard.cardData);
				}
			}
		}
		return standardCards;
	}
	
	public List<Card> GetAllStandardCards()
	{
		List<Card> standardCards = new List<Card>();
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					standardCards.Add(standardDropZones[i].placedCard);
				}
			}
		}
		return standardCards;
	}
	
	public List<CardData> GetAllSpecialCards()
	{
		List<CardData> specialCards = new List<CardData>();
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					specialCards.Add(standardDropZones[i].placedCard.cardData);
				}
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				specialCards.Add(specialCardDropZones[i].placedCard.cardData);
			}
		}
		return specialCards;
	}
	
	public List<Card> GetAllScoredFaceCards()
	{
		List<Card> scoredFaceCards = new List<Card>();
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(!standardDropZones[i].placedCard.cardData.isSpecialCard && (standardDropZones[i].placedCard.cardData.rank >=9 && standardDropZones[i].placedCard.cardData.rank <=11) || (Baubles.instance.GetImpactInt("AllCardsAreFaceCards") > 0))
				{
					if(currentScoredCards.Contains(standardDropZones[i].placedCard))
					{
						scoredFaceCards.Add(standardDropZones[i].placedCard);
					}
				}
			}
		}
		return scoredFaceCards;
	}
	
	public int GetNumberOfCardsOfSuit(int suitToCompare, bool scoredCardsOnly)
	{
		int cardsOfSuit = 0;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(standardDropZones[i].placedCard.cardData.suit == suitToCompare)
				{
					if(!scoredCardsOnly || currentScoredCards.Contains(standardDropZones[i].placedCard))
					{
						cardsOfSuit++;
					}
				}
			}
		}
		return cardsOfSuit;
	}
	
	public int GetNumberOfCardsOfRank(int rankToCompare, bool onlyScoredCards = true)
	{
		int cardsOfRank = 0;
		if(onlyScoredCards)
		{
			for(int i = 0; i < currentScoredCards.Count; i++)
			{
				if(currentScoredCards[i].cardData.rank == rankToCompare)
				{
					cardsOfRank++;
				}
			}
		}
		else
		{
			for(int i = 0; i < standardDropZones.Length; i++)
			{
				if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
				{
					if(standardDropZones[i].placedCard.cardData.rank == rankToCompare)
					{
						cardsOfRank++;
					}
				}
			}
		}
		return cardsOfRank;
	}
	
	public Card GetFirstInstanceOfSpecialCardInDropZoneArray(DropZone[] dropZoneArray, string specialCardTag)
	{
		for(int i = 0; i < dropZoneArray.Length; i++)
		{
			if(dropZoneArray[i].gameObject.activeSelf && dropZoneArray[i].cardPlaced)
			{
				if(dropZoneArray[i].placedCard.cardData.specialCardName == specialCardTag)
				{
					return dropZoneArray[i].placedCard;
				}
			}
		}
		return null;
	}
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		HandArea.instance.PlaySelectedClicked();
	}
	
	public void LockButtonClicked()
	{
		if(V.i.isTutorial && !Tutorial.instance.tutorialFinished)
		{
			if(Tutorial.instance.currentStage == 4)
			{
				int playedTwos = 0;
				int playedQueens = 0;
				int playedCards = 0;
				for(int i = 0; i < standardDropZones.Length; i++)
				{
					if(standardDropZones[i].cardPlaced)
					{
						if(standardDropZones[i].placedCard.cardData.rank == 0)
						{
							playedTwos++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 10)
						{
							playedQueens++;
						}
						playedCards++;
					}
				}
				if(playedTwos == 2 && playedQueens == 2 && playedCards == 4)
				{
					Tutorial.instance.IncrementStage();
				}
				else
				{
					MinorNotifications.instance.NewMinorNotification("Play just your two pair, queens and aces!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(lockInHandButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
			}
			else if(Tutorial.instance.currentStage == 12)
			{
				int playedSixes = 0;
				int playedSevens = 0;
				int playedEights = 0;
				int playedNines = 0;
				int playedTens = 0;
				int playedCards = 0;
				for(int i = 0; i < standardDropZones.Length; i++)
				{
					if(standardDropZones[i].cardPlaced)
					{
						if(standardDropZones[i].placedCard.cardData.rank == 4)
						{
							playedSixes++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 5)
						{
							playedSevens++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 6)
						{
							playedEights++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 7)
						{
							playedNines++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 8)
						{
							playedTens++;
						}
						playedCards++;
					}
				}
				if(playedSixes == 1 && playedSevens == 1 && playedEights == 1 && playedNines == 1 && playedTens == 1 && playedCards == 5)
				{
					Tutorial.instance.IncrementStage();
				}
				else
				{
					MinorNotifications.instance.NewMinorNotification("Play your straight, 6-7-8-9-10", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(lockInHandButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
			}
			else if(Tutorial.instance.currentStage == 21)
			{
				int playedNines = 0;
				int playedTens = 0;
				int playedJacks = 0;
				int playedQueens = 0;
				int playedCards = 0;
				for(int i = 0; i < standardDropZones.Length; i++)
				{
					if(standardDropZones[i].cardPlaced)
					{
						if(standardDropZones[i].placedCard.cardData.rank == 7)
						{
							playedNines++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 8)
						{
							playedTens++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 9)
						{
							playedJacks++;
						}
						else if(standardDropZones[i].placedCard.cardData.rank == 10)
						{
							playedQueens++;
						}
						playedCards++;
					}
				}
				if(playedNines == 1 && playedTens == 1 && playedJacks == 1 && playedQueens == 1 && playedCards == 4)
				{
					Tutorial.instance.IncrementStage();
				}
				else
				{
					MinorNotifications.instance.NewMinorNotification("Play your straight, 9-10-J-Q", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(lockInHandButton.rt, GameManager.instance.gameplayCanvas));
					return;
				}
			}
			else
			{
				MinorNotifications.instance.NewMinorNotification("Hold your horses!", LocalInterface.instance.GetMousePosition(), LocalInterface.instance.GetCanvasPositionOfRectTransform(lockInHandButton.rt, GameManager.instance.gameplayCanvas));
				return;
			}
		}
		locked = true;
		lockInHandButton.ChangeButtonEnabled(false);
		HandArea.instance.recallButton.ChangeButtonEnabled(false);
		lockInHandButtonImage.sprite = lockedButtonSprite;
		lockInHandButtonImageRT.sizeDelta = new Vector2(lockedButtonSprite.rect.width, lockedButtonSprite.rect.height);
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				standardDropZones[i].placedCard.canMove = false;
			}
		}
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				specialCardDropZones[i].placedCard.canMove = false;
			}
		}
		if(currentHandTier >= 0)
		{
			RunInformation.instance.HandPlayed(currentHandTier, currentHandsContained, GameManager.instance.IsPlayerFatigued());
			/* RunInformation.instance.handsPlayed[currentHandTier]++;
			if(RunInformation.instance.handsPlayed[currentHandTier] == 1 && currentHandTier > 8)
			{
				HandsInformation.instance.HandUpdated(currentHandsContained);
			} */
		}
		
		StartCoroutine(ScoreHand());
	}
	
	public IEnumerator ScoreHand()
	{
		currentStateOfScoringRoutine = "Start";
		int totalNumberOfCards = GetTotalNumberOfCards();
		if(totalNumberOfCards == 1 && Baubles.instance.GetImpactInt("DestroySoloStandardCard") > 0)
		{
			StartCoroutine(ShootSnurfGun());
			yield break;
		}
		if(HandContainsSpecialCardTag("ConvertCard"))
		{
			currentStateOfScoringRoutine = "ConvertingCards";
			StartCoroutine(PerformCardConversion());
			while(currentStateOfScoringRoutine == "ConvertingCards")
			{
				yield return null;
			}
		}
		if(GameManager.instance.IsBossTagActive("NegativeZodiac"))
		{
			Baubles.instance.NegativeZodiacGained(LocalInterface.instance.GetZodiacBaubleStringFromHandTier(currentHandTier), new Vector2(0, 133f));
			HandUpdated();
			SoundManager.instance.PlayNegativeZodiacSound();
			yield return new WaitForSeconds(0.75f / Preferences.instance.gameSpeed);
		}
		if(HandContainsSpecialCardTag("RainbowPaint"))
		{
			currentStateOfScoringRoutine = "PaintingCardsRainbow";
			StartCoroutine(PaintCardsRainbow());
			while(currentStateOfScoringRoutine == "PaintingCardsRainbow")
			{
				yield return null;
			}
		}
		if(HandContainsSpecialCardTag("ChangeAllRanks"))
		{
			currentStateOfScoringRoutine = "ChangingAllRanks";
			StartCoroutine(ChangeAllStandardCardRanks());
			while(currentStateOfScoringRoutine == "ChangingAllRanks")
			{
				yield return null;
			}
		}
		if(HandContainsSpecialCardTag("Promotion") || HandContainsSpecialCardTag("Demotion"))
		{
			currentStateOfScoringRoutine = "PromotingCards";
			StartCoroutine(PerformPromotionOrDemotion());
			while(currentStateOfScoringRoutine == "PromotingCards")
			{
				yield return null;
			}
		}
		// old spot of convert card
		if(HandContainsSpecialCardTag("EarnZodiac"))
		{
			currentStateOfScoringRoutine = "EarningZodiacs";
			StartCoroutine(GainZodiacs());
			while(currentStateOfScoringRoutine == "EarningZodiacs")
			{
				yield return null;
			}
		}
		if(HandContainsSpecialCardTag("ZodiacsFromFlushes"))
		{
			currentStateOfScoringRoutine = "EarningZodiacsFromFlushes";
			StartCoroutine(GainZodiacsFromFlushes());
			while(currentStateOfScoringRoutine == "EarningZodiacsFromFlushes")
			{
				yield return null;
			}
		}
		if(HandContainsSpecialCardTag("ZodiacContainedHands"))
		{
			currentStateOfScoringRoutine = "EarningZodiacsFromContainedHands";
			StartCoroutine(GainZodiacsFromContainedHands());
			while(currentStateOfScoringRoutine == "EarningZodiacsFromContainedHands")
			{
				yield return null;
			}
		}
		else
		{
			if(!LocalInterface.instance.IsSpecialCardUnlocked("ZodiacContainedHands") && !V.i.isDailyGame && !V.i.isCustomGame)
			{
				if(currentHandsContained[6] && currentHandsContained[8])
				{
					LocalInterface.instance.UnlockSpecialCard("ZodiacContainedHands");
				}
			}
		}
		if(Baubles.instance.GetImpactInt("DestroyQueensForZodiacs") > 0)
		{
			List<Card> allStandardCards = GetAllStandardCards();
			for(int i = 0; i < allStandardCards.Count; i++)
			{
				if(allStandardCards[i].cardData.rank == 10)
				{
					currentStateOfScoringRoutine = "GettingZodiacsFromQueens";
					StartCoroutine(GainZodiacsFromQueens(allStandardCards));
					while(currentStateOfScoringRoutine == "GettingZodiacsFromQueens")
					{
						yield return null;
					}
					break;
				}
			}
		}
		if(HandContainsSpecialCardTag("SlotMachine"))
		{
			List<Card> slotMachineCards = GetAllSpecialCardsWithTag("SlotMachine");
			int numberOfSevens = GetNumberOfCardsOfRank(5);
			SlotMachine.instance.SetupSlotMachine(GameManager.instance.GetMaxPlayableStandardCards(true), Mathf.Max(numberOfSevens, 1), slotMachineCards.Count);
			currentStateOfScoringRoutine = "RunningSlotMachine";
			while(currentStateOfScoringRoutine == "RunningSlotMachine")
			{
				yield return null;
			}
		}
		if(GameManager.instance.IsBossTagActive("BaseScoreMultHalved"))
		{
			MultiplyPoints(0.5d);
			MultiplyMult(0.5d);
			SoundManager.instance.PlayPointsAndMultHalvedSound();
			yield return new WaitForSeconds(0.75f / Preferences.instance.gameSpeed);
		}
		if(GameManager.instance.IsBossTagActive("HandQuartered"))
		{
			switch(GameManager.instance.GetCurrentBossRoundRandom1()) // 0 = 3oak, 1 = straight, 2 = flush
			{
				case 0:
					if(currentHandsContained[3])
					{
						MultiplyMult(0.25d);
						SoundManager.instance.PlayPointsAndMultHalvedSound();
						yield return new WaitForSeconds(0.75f / Preferences.instance.gameSpeed);
					}
				break;
				case 1:
					if(currentHandsContained[4])
					{
						MultiplyMult(0.25d);
						SoundManager.instance.PlayPointsAndMultHalvedSound();
						yield return new WaitForSeconds(0.75f / Preferences.instance.gameSpeed);
					}
				break;
				case 2:
					if(currentHandsContained[5])
					{
						MultiplyMult(0.25d);
						SoundManager.instance.PlayPointsAndMultHalvedSound();
						yield return new WaitForSeconds(0.75f / Preferences.instance.gameSpeed);
					}
				break;
			}
		}
		if(Baubles.instance.GetImpactInt("GetMushroomsFromHands") > 0)
		{
			int getMushroomsFromHands = Baubles.instance.GetImpactInt("GetMushroomsFromHands");
			if(currentHandsContained[17] && getMushroomsFromHands >= 3)
			{
				Baubles.instance.Notify("GetMushroomsFromHands", 0, SoundManager.instance.PlayResourcefulRabbitSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				Card newCard = HandArea.instance.SpawnCard(new CardData(-1, -1, "DoubleValueAndMult"), HandArea.instance.deckLocation, HandArea.instance.cardParent);
				Deck.instance.cardsInHand.Add(newCard);
				HandArea.instance.ReorganizeHand();
				HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
			}
			else if(currentHandsContained[13] && getMushroomsFromHands >= 2)
			{
				Baubles.instance.Notify("GetMushroomsFromHands", 0, SoundManager.instance.PlayResourcefulRabbitSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				Card newCard = HandArea.instance.SpawnCard(new CardData(-1, -1, "IncreaseMult"), HandArea.instance.deckLocation, HandArea.instance.cardParent);
				Deck.instance.cardsInHand.Add(newCard);
				HandArea.instance.ReorganizeHand();
				HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
			}
			else if(currentHandsContained[9])
			{
				Baubles.instance.Notify("GetMushroomsFromHands", 0, SoundManager.instance.PlayResourcefulRabbitSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				Card newCard = HandArea.instance.SpawnCard(new CardData(-1, -1, "IncreaseValue"), HandArea.instance.deckLocation, HandArea.instance.cardParent);
				Deck.instance.cardsInHand.Add(newCard);
				HandArea.instance.ReorganizeHand();
				HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
			}
		}
		int zodiacsFromFlushesImpact = Baubles.instance.GetImpactInt("ZodiacsFromFlushes");
		if(zodiacsFromFlushesImpact > 0)
		{
			int flushZodiacs = 0;
			string flushesContained = "";
			for(int i = 0; i < zodiacsFromFlushesImpact; i++)
			{
				if(currentFlushesContained[GameManager.instance.flushZodiacBaubleSuitOrders[i]])
				{
					flushZodiacs++;
					flushesContained += $"{GameManager.instance.flushZodiacBaubleSuitOrders[i]}¤";
				}
			}
			if(flushZodiacs > 0)
			{
				Baubles.instance.Notify("ZodiacsFromFlushes", 0, SoundManager.instance.PlayZodiacsForFlushesBaubleSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				flushesContained = flushesContained.Substring(0, flushesContained.Length - 1);
				Card newCard = HandArea.instance.SpawnCard(new CardData(-1, -1, $"ZodiacsFromFlushes{flushesContained}"), HandArea.instance.deckLocation, HandArea.instance.cardParent);
				Deck.instance.cardsInHand.Add(newCard);
				HandArea.instance.ReorganizeHand();
				HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Add(newCard.controllerSelectableObject);
			}
			flushesContainedRT.gameObject.SetActive(false);
		}
		else
		{
			if(!LocalInterface.instance.IsBaubleUnlocked("ZodiacsFromFlushes") && !V.i.isDailyGame && !V.i.isCustomGame)
			{
				if(LocalInterface.instance.GetNumberOfMatchingBoolsInArray(currentFlushesContained, true) >= 4)
				{
					LocalInterface.instance.UnlockBauble("ZodiacsFromFlushes");
					ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["ZodiacsFromFlushes"].howToUnlock, "UnlockedBauble", "ZodiacsFromFlushes", V.i.v.variantBaubles["ZodiacsFromFlushes"].sprite);
				}
			}
		}
		SetVisibilityOfMushroomMults(false);
		currentStateOfScoringRoutine = "ScoringCards";
		StartCoroutine(GainScoreAndMultFromCards());
		while(currentStateOfScoringRoutine == "ScoringCards")
		{
			yield return null;
		}
		if(Baubles.instance.GetImpactInt("AddPointsAndMultFromCardsAgain") > 0)
		{
			if(Math.Abs(pointsGainedFromCards) > 0.1d || Math.Abs(multGainedFromCards) > 0.1d)
			{
				Baubles.instance.Notify("AddPointsAndMultFromCardsAgain", 0, SoundManager.instance.PlayAddPointsAndMultFromCardsAgainSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				if(Math.Abs(pointsGainedFromCards) > 0.1d)
				{
					AddPoints(pointsGainedFromCards);
				}
				if(Math.Abs(multGainedFromCards) > 0.1d)
				{
					AddMult(multGainedFromCards);
				}
				yield return new WaitForSeconds (0.625f / Preferences.instance.gameSpeed);
			}
		}
		if(Baubles.instance.GetImpactInt("Dice") > 0)
		{
			Baubles.instance.Notify("Dice");
			yield return new WaitForSeconds (1.5f / Preferences.instance.gameSpeed);
		}
		if(Baubles.instance.GetImpactInt("MultFromRainbowCards") > 0)
		{
			int numberOfRainbowCards = GetNumberOfCardsOfSuit(4, true);
			if(numberOfRainbowCards > 0)
			{
				Baubles.instance.Notify("MultFromRainbowCards", 0, SoundManager.instance.PlayRainbowPinwheelSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				AddMult(numberOfRainbowCards * Baubles.instance.GetImpactDouble("MultFromRainbowCards"));
				yield return new WaitForSeconds (0.625f / Preferences.instance.gameSpeed);
			}
		}
		if(Baubles.instance.GetImpactInt("MultFromKings") > 0)
		{
			int numberOfKings = GetNumberOfCardsOfRank(11);
			if(numberOfKings > 0)
			{
				Baubles.instance.Notify("MultFromKings", 0, SoundManager.instance.PlayWoodenTileSound);
				yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
				AddMult(numberOfKings * Baubles.instance.GetImpactDouble("MultFromKings"));
				yield return new WaitForSeconds (0.625f / Preferences.instance.gameSpeed);
			}
		}
		// four colored eye masks, rework? Too similar to the hand multis
		// idea: gain the points and mult from cards again
		for(int i = 0; i <= 17; i++)
		{
			if(currentHandsContained[i])
			{
				string currentMultString = LocalInterface.instance.GetMultiplierBaubleStringFromHandTier(i);
				if(Baubles.instance.GetImpactInt(currentMultString) > 0)
				{
					Baubles.instance.Notify(currentMultString, 0, SoundManager.instance.PlayViolinSound);
					yield return new WaitForSeconds (0.875f / Preferences.instance.gameSpeed);
					MultiplyMult(Baubles.instance.GetImpactDouble(currentMultString));
					yield return new WaitForSeconds (0.625f / Preferences.instance.gameSpeed);
				}
			}
		}
		
		// All card converting stuff should be above this line
		RunInformation.instance.standardCardDatasInLastHand.Clear();
		List<Card> standardCardsForRunInformation = GetAllStandardCards();
		for(int i = 0; i < standardCardsForRunInformation.Count; i++)
		{
			RunInformation.instance.standardCardDatasInLastHand.Add(standardCardsForRunInformation[i].cardData);
		}
		
		if(HandContainsSpecialCardTag("Bomb") || V.i.v.variantSpecialOptions["DestroyAllPlayedHands"].inEffect || GameManager.instance.IsBossTagActive("DestroyAllPlayedCards"))
		{
			currentStateOfScoringRoutine = "DestroyingAllCards";
			StartCoroutine(DestroyAllRemainingCards());
			while(currentStateOfScoringRoutine == "DestroyingAllCards")
			{
				yield return null;
			}
		}
		if(Baubles.instance.GetImpactInt("DestroyQueensForZodiacs") > 0)
		{
			List<Card> allStandardCards = GetAllStandardCards();
			for(int i = 0; i < allStandardCards.Count; i++)
			{
				if(allStandardCards[i].cardData.rank == 10)
				{
					currentStateOfScoringRoutine = "DestroyingQueens";
					StartCoroutine(DestroyAllCardsOfRank(allStandardCards, new List<int>{10}));
					while(currentStateOfScoringRoutine == "DestroyingQueens")
					{
						yield return null;
					}
					break;
				}
			}
		}
		if(GameManager.instance.IsBossTagActive("SuitDestroyed"))
		{
			List<Card> allStandardCards = GetAllStandardCards();
			BossRound currentBossRound = GameManager.instance.GetCurrentBossRound();
			for(int i = 0; i < allStandardCards.Count; i++)
			{
				if(allStandardCards[i].cardData.suit == currentBossRound.random1)
				{
					currentStateOfScoringRoutine = "DestroyingCardsOfSuit";
					StartCoroutine(DestroyAllCardsOfSuit(allStandardCards, new List<int>{currentBossRound.random1}));
					while(currentStateOfScoringRoutine == "DestroyingCardsOfSuit")
					{
						yield return null;
					}
					break;
				}
			}
		}
		if(GameManager.instance.IsBossTagActive("DestroyFaceCards"))
		{
			List<Card> allStandardCards = GetAllStandardCards();
			for(int i = 0; i < allStandardCards.Count; i++)
			{
				if(allStandardCards[i].cardData.rank >= 9 && allStandardCards[i].cardData.rank <= 11)
				{
					currentStateOfScoringRoutine = "DestroyingCardsOfRank";
					StartCoroutine(DestroyAllCardsOfRank(allStandardCards, new List<int>{9, 10, 11}));
					while(currentStateOfScoringRoutine == "DestroyingCardsOfRank")
					{
						yield return null;
					}
					break;
				}
			}
		}
		yield return new WaitForSeconds (0.5f / Preferences.instance.gameSpeed);
		MultiplyPoints(currentMult);
		RunInformation.instance.HandFinished(currentPoints, currentHandTier);
		multLabel.StartExpandRetract();
		SoundManager.instance.PlayScoreMultipliedSound();
		yield return new WaitForSeconds (0.5f / Preferences.instance.gameSpeed);
		currentStateOfScoringRoutine = "UpdatingScoreVial";
		ScoreVial.instance.AddScore(currentPoints);
		while(currentStateOfScoringRoutine == "UpdatingScoreVial")
		{
			yield return null;
		}
		EndScoringHand();
	}
	
	public IEnumerator DestroyAllCardsOfRank(List<Card> allStandardCards, List<int> ranksToDestroy)
	{
		for(int i = 0; i < allStandardCards.Count; i++)
		{
			if(ranksToDestroy.Contains(allStandardCards[i].cardData.rank))
			{
				GameObject newBombExplosionGO = Instantiate(bombExplosionPrefab, allStandardCards[i].dropZonePlacedIn.rt);
				BombExplosion newBombExplosion = newBombExplosionGO.GetComponent<BombExplosion>();
				newBombExplosion.StartExplosion(allStandardCards[i]);
				yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
			}
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		currentStateOfScoringRoutine = "DoneDestroyingCardsOfRank";
	}
	
	public IEnumerator DestroyAllCardsOfSuit(List<Card> allStandardCards, List<int> suitsToDestroy)
	{
		for(int i = 0; i < allStandardCards.Count; i++)
		{
			if(suitsToDestroy.Contains(allStandardCards[i].cardData.suit))
			{
				GameObject newBombExplosionGO = Instantiate(bombExplosionPrefab, allStandardCards[i].dropZonePlacedIn.rt);
				BombExplosion newBombExplosion = newBombExplosionGO.GetComponent<BombExplosion>();
				newBombExplosion.StartExplosion(allStandardCards[i]);
				yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
			}
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		currentStateOfScoringRoutine = "DoneDestroyingCardsOfSuit";
	}
	
	public IEnumerator DestroyAllRemainingCards()
	{
		int maxStandardCards = GameManager.instance.GetMaxPlayableStandardCards();
		int standardBombDropZoneNumberUpper = maxStandardCards / 2;
		int standardBombDropZoneNumberLower = maxStandardCards / 2;
		Card standardDropZoneBombCard = GetFirstInstanceOfSpecialCardInDropZoneArray(standardDropZones, "Bomb");
		if(standardDropZoneBombCard != null)
		{
			standardBombDropZoneNumberUpper = standardDropZoneBombCard.dropZonePlacedIn.dropZoneNumber;
			standardBombDropZoneNumberLower = standardDropZoneBombCard.dropZonePlacedIn.dropZoneNumber;
		}
		bool doneDestroyingCardsInStandardDropZonesUpper = false;
		bool doneDestroyingCardsInStandardDropZonesLower = false;
		bool doneDestroyingCardsInSpecialDropZonesUpper = true;
		bool doneDestroyingCardsInSpecialDropZonesLower = true;
		int maxSpecialCards = GameManager.instance.GetNumberOfSpecialCardOnlyDropZones();
		int specialBombDropZoneNumberUpper = maxSpecialCards / 2;
		int specialBombDropZoneNumberLower = maxSpecialCards / 2;
		if(maxSpecialCards > 0)
		{
			Card specialDropZoneBombCard = GetFirstInstanceOfSpecialCardInDropZoneArray(specialCardDropZones, "Bomb");
			if(specialDropZoneBombCard != null)
			{
				specialBombDropZoneNumberUpper = specialDropZoneBombCard.dropZonePlacedIn.dropZoneNumber;
				specialBombDropZoneNumberLower = specialDropZoneBombCard.dropZonePlacedIn.dropZoneNumber;
			}
			doneDestroyingCardsInSpecialDropZonesUpper = false;
			doneDestroyingCardsInSpecialDropZonesLower = false;
		}
		while(!doneDestroyingCardsInStandardDropZonesUpper || !doneDestroyingCardsInStandardDropZonesLower || !doneDestroyingCardsInSpecialDropZonesUpper || !doneDestroyingCardsInSpecialDropZonesLower)
		{
			if(standardBombDropZoneNumberLower >= 0)
			{
				GameObject newBombExplosionGO = Instantiate(bombExplosionPrefab, standardDropZones[standardBombDropZoneNumberLower].rt);
				BombExplosion newBombExplosion = newBombExplosionGO.GetComponent<BombExplosion>();
				newBombExplosion.StartExplosion(standardDropZones[standardBombDropZoneNumberLower].placedCard);
			}
			else
			{
				doneDestroyingCardsInStandardDropZonesLower = true;
			}
			if(standardBombDropZoneNumberUpper <= maxStandardCards - 1)
			{
				if(standardBombDropZoneNumberLower != standardBombDropZoneNumberUpper)
				{
					GameObject newBombExplosionGO = Instantiate(bombExplosionPrefab, standardDropZones[standardBombDropZoneNumberUpper].rt);
					BombExplosion newBombExplosion = newBombExplosionGO.GetComponent<BombExplosion>();
					newBombExplosion.StartExplosion(standardDropZones[standardBombDropZoneNumberUpper].placedCard);
				}
			}
			else
			{
				doneDestroyingCardsInStandardDropZonesUpper = true;
			}
			standardBombDropZoneNumberLower--;
			standardBombDropZoneNumberUpper++;
			if(maxSpecialCards > 0)
			{
				if(specialBombDropZoneNumberLower >= 0)
				{
					GameObject newBombExplosionGO = Instantiate(bombExplosionPrefab, specialCardDropZones[specialBombDropZoneNumberLower].rt);
					BombExplosion newBombExplosion = newBombExplosionGO.GetComponent<BombExplosion>();
					newBombExplosion.StartExplosion(specialCardDropZones[specialBombDropZoneNumberLower].placedCard);
				}
				else
				{
					doneDestroyingCardsInSpecialDropZonesLower = true;
				}
				if(specialBombDropZoneNumberUpper <= maxSpecialCards - 1)
				{
					if(specialBombDropZoneNumberLower != specialBombDropZoneNumberUpper)
					{
						GameObject newBombExplosionGO = Instantiate(bombExplosionPrefab, specialCardDropZones[specialBombDropZoneNumberUpper].rt);
						BombExplosion newBombExplosion = newBombExplosionGO.GetComponent<BombExplosion>();
						newBombExplosion.StartExplosion(specialCardDropZones[specialBombDropZoneNumberUpper].placedCard);
					}
				}
				else
				{
					doneDestroyingCardsInSpecialDropZonesUpper = true;
				}
				specialBombDropZoneNumberLower--;
				specialBombDropZoneNumberUpper++;
			}
			yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
		}
		
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		currentStateOfScoringRoutine = "DoneDestroyingCards";
	}
	
	public IEnumerator GainScoreAndMultFromCards()
	{
		currentScoredCards.Sort((x, y) =>
		{
			return x.dropZonePlacedIn.dropZoneNumber.CompareTo(y.dropZonePlacedIn.dropZoneNumber);
		});
		bool lastHigh = false;
		Card monarchCard = null;
		List<Card> addToPointsCards = GetAllSpecialCardsWithTag("IncreaseValue");
		List<Card> addToMultCards = GetAllSpecialCardsWithTag("IncreaseMult");
		List<Card> doublePointsAndMultCards = GetAllSpecialCardsWithTag("DoubleValueAndMult");
		if(!LocalInterface.instance.IsBaubleUnlocked("IncreaseMushroomPowerTriples"))
		{
			int mushroomsPlayed = addToPointsCards.Count + addToMultCards.Count + doublePointsAndMultCards.Count;
			if(mushroomsPlayed > 0)
			{
				Stats.instance.MushroomsPlayed(mushroomsPlayed);
			}
		}
		if(!LocalInterface.instance.IsBaubleUnlocked("IncreaseMushroomPowerSames") && !V.i.isDailyGame && !V.i.isCustomGame)
		{
			if(addToPointsCards.Count >= 10 || addToMultCards.Count >= 10 || doublePointsAndMultCards.Count >= 10)
			{
				LocalInterface.instance.UnlockBauble("IncreaseMushroomPowerSames");
				ItemEarnedNotifications.instance.Notify("New Bauble unlocked!", V.i.v.variantBaubles["IncreaseMushroomPowerSames"].howToUnlock, "UnlockedBauble", "IncreaseMushroomPowerSames", V.i.v.variantBaubles["IncreaseMushroomPowerSames"].sprite);
			}
		}
		if(Baubles.instance.GetImpactInt("MultToMonarch") > 0)
		{
			List<Card> scoredFaceCards = GetAllScoredFaceCards();
			if(scoredFaceCards.Count > 0)
			{
				monarchCard = scoredFaceCards[RNG.instance.hands.Range(0, scoredFaceCards.Count)];
				monarchCard.dropZonePlacedIn.scorePlate.multToAdd += Baubles.instance.GetImpactDouble("MultToMonarch") * scoredFaceCards.Count;
			}
		}
		for(int i = 0; i < currentScoredCards.Count; i++)
		{
			if(currentScoredCards[i] == monarchCard)
			{
				Baubles.instance.Notify("MultToMonarch", LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed * currentScoredCards.Count, SoundManager.instance.PlaySandwichMonarchSound);
				Stats.instance.FaceCardScoredWithBauble();
			}
			if(Baubles.instance.GetImpactInt("PointsToNumberedCards") > 0)
			{
				if(currentScoredCards[i].cardData.rank <= 8 || Baubles.instance.GetImpactInt("AllCardsAreNumberedCards") > 0)
				{
					currentScoredCards[i].dropZonePlacedIn.scorePlate.pointsToAdd += Baubles.instance.GetImpactDouble("PointsToNumberedCards");
					 Baubles.instance.Notify("PointsToNumberedCards", LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed * currentScoredCards.Count, SoundManager.instance.PlayVampireSound);
					 Stats.instance.NumberedCardScoredWithBauble();
				}
			}
			if(Baubles.instance.GetImpactInt("AcesStraights") > 0 && currentHandsContained[4])
			{
				if(currentScoredCards[i].cardData.rank == 12 || Baubles.instance.GetImpactInt("AllCardsAreAces") > 0)
				{
					currentScoredCards[i].dropZonePlacedIn.scorePlate.pointsToAdd += Baubles.instance.GetImpactDouble("AcesStraights");
					currentScoredCards[i].dropZonePlacedIn.scorePlate.multToAdd += Baubles.instance.GetImpactDouble("AcesStraights", true);
					// Baubles.instance.Notify("AcesStraights", LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed * currentScoredCards.Count, SoundManager.instance.PlayPanFluteSound);
					Baubles.instance.Notify("AcesStraights", LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed * currentScoredCards.Count);
					Stats.instance.AceScoredWithBauble();
				}
			}
			currentScoredCards[i].dropZonePlacedIn.scorePlate.points = currentScoredCards[i].cardData.baseValue;
			currentScoredCards[i].dropZonePlacedIn.scorePlate.mult = currentScoredCards[i].cardData.multiplier;
			int cardsOfThisRank = GetNumberOfCardsOfRank(currentScoredCards[i].cardData.rank, true);
			int increaseMushroomPowerTriples = Baubles.instance.GetImpactInt("IncreaseMushroomPowerTriples");
			int cardPowerFactor = 1;
			if(increaseMushroomPowerTriples > 0 && cardsOfThisRank >= 3)
			{
				cardPowerFactor = Mathf.Min(increaseMushroomPowerTriples + 1, cardsOfThisRank - 1);
			}
			if(addToPointsCards.Count > 0)
			{
				int mushroomPowerFactor = 1;
				if(Baubles.instance.GetImpactInt("IncreaseMushroomPowerSames") > 0)
				{
					mushroomPowerFactor = addToPointsCards.Count;
				}
				currentScoredCards[i].dropZonePlacedIn.scorePlate.pointsToAdd += addToPointsCards.Count * V.i.v.variantSpecialCards["IncreaseValue"].impact * mushroomPowerFactor * cardPowerFactor;
				currentScoredCards[i].dropZonePlacedIn.scorePlate.additiveMushroomUsed = true;
			}
			if(addToMultCards.Count > 0)
			{
				int mushroomPowerFactor = 1;
				if(Baubles.instance.GetImpactInt("IncreaseMushroomPowerSames") > 0)
				{
					mushroomPowerFactor = addToMultCards.Count;
				}
				currentScoredCards[i].dropZonePlacedIn.scorePlate.multToAdd += addToMultCards.Count * V.i.v.variantSpecialCards["IncreaseMult"].impact * mushroomPowerFactor * cardPowerFactor;
				currentScoredCards[i].dropZonePlacedIn.scorePlate.additiveMushroomUsed = true;
			}
			if(doublePointsAndMultCards.Count > 0)
			{
				int mushroomPowerFactor = 1;
				if(Baubles.instance.GetImpactInt("IncreaseMushroomPowerSames") > 0)
				{
					mushroomPowerFactor = doublePointsAndMultCards.Count;
				}
				currentScoredCards[i].dropZonePlacedIn.scorePlate.multiplicitiveMushroomFactor = Math.Pow(V.i.v.variantSpecialCards["DoubleValueAndMult"].impact * mushroomPowerFactor * cardPowerFactor, doublePointsAndMultCards.Count);
				currentScoredCards[i].dropZonePlacedIn.scorePlate.multiplicitiveMushroomUsed = true;
			}
			currentScoredCards[i].dropZonePlacedIn.scorePlate.StartScorePlate(currentScoredCards.Count);
			if(currentScoredCards[i].dropZonePlacedIn.dropZoneNumber > 0 && i > 0 && standardDropZones[currentScoredCards[i].dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
			{
				if(currentScoredCards[i - 1] == standardDropZones[currentScoredCards[i].dropZonePlacedIn.dropZoneNumber - 1].placedCard)
				{
					currentScoredCards[i].dropZonePlacedIn.scorePlate.rt.anchoredPosition = new Vector2(0, (lastHigh ? -scorePlateYDifference : scorePlateYDifference));
					lastHigh = !lastHigh;
					yield return new WaitForSeconds (LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
					continue;
				}
			}
			if(currentScoredCards[i].dropZonePlacedIn.dropZoneNumber < GameManager.instance.GetMaxPlayableStandardCards() - 1 && i < currentScoredCards.Count - 1 && standardDropZones[currentScoredCards[i].dropZonePlacedIn.dropZoneNumber + 1].cardPlaced)
			{
				if(currentScoredCards[i + 1] == standardDropZones[currentScoredCards[i].dropZonePlacedIn.dropZoneNumber + 1].placedCard)
				{
					currentScoredCards[i].dropZonePlacedIn.scorePlate.rt.anchoredPosition = new Vector2(0, (lastHigh ? -scorePlateYDifference : scorePlateYDifference));
					lastHigh = !lastHigh;
					yield return new WaitForSeconds (LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
					continue;
				}
			}
			currentScoredCards[i].dropZonePlacedIn.scorePlate.rt.anchoredPosition = Vector2.zero;
			lastHigh = false;
			yield return new WaitForSeconds (LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
		}
		yield return new WaitForSeconds (LocalInterface.instance.animationDuration * 3 / Preferences.instance.gameSpeed);
		yield return null;
		currentStateOfScoringRoutine = "DoneScoring";
	}
	
	public IEnumerator GainZodiacsFromQueens(List<Card> allStandardCards)
	{
		string baubleString = LocalInterface.instance.GetZodiacBaubleStringFromHandTier(currentHandTier);
		Baubles.instance.Notify("DestroyQueensForZodiacs");
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		SoundManager.instance.PlayDestroyQueensForZodiacsSound();
		for(int i = 0; i < allStandardCards.Count; i++)
		{
			if(allStandardCards[i].cardData.rank == 10)
			{
				// RunInformation.instance.zodiacsEarned[currentHandTier]++;
				Baubles.instance.ZodiacPurchased(baubleString, LocalInterface.instance.GetCanvasPositionOfRectTransform(allStandardCards[i].rt, GameManager.instance.gameplayCanvas) + new Vector2(0, 25f), true, false);
				Baubles.instance.ZodiacPurchased(baubleString, LocalInterface.instance.GetCanvasPositionOfRectTransform(allStandardCards[i].rt, GameManager.instance.gameplayCanvas) + new Vector2(0, -25f), true, false);
			}
		}
		Baubles.instance.HandInfosChanged();
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DoneGainingZodiacsFromQueens";
		// StartCoroutine(ScoreHand());
	}
	
	public IEnumerator GainZodiacs()
	{
		List<Card> zodiacCards = GetAllSpecialCardsWithTag("EarnZodiac");
		string baubleString = LocalInterface.instance.GetZodiacBaubleStringFromHandTier(currentHandTier);
		SoundManager.instance.PlayZodiacGainedSound();
		for(int i = 0; i < zodiacCards.Count; i++)
		{
			// RunInformation.instance.zodiacsEarned[currentHandTier]++;
			Baubles.instance.ZodiacPurchased(baubleString, LocalInterface.instance.GetCanvasPositionOfRectTransform(zodiacCards[i].rt, GameManager.instance.gameplayCanvas), true, false);
		}
		Baubles.instance.HandInfosChanged();
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		for(int i = 0; i < zodiacCards.Count; i++)
		{
			DissolveCard(zodiacCards[i]);
		}
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DoneGainingZodiacs";
		// StartCoroutine(ScoreHand());
	}
	
	public IEnumerator GainZodiacsFromFlushes()
	{
		List<Card> zodiacCards = GetAllSpecialCardsWithTag("ZodiacsFromFlushes");
		string baubleString = LocalInterface.instance.GetZodiacBaubleStringFromHandTier(currentHandTier);
		SoundManager.instance.PlayZodiacsForFlushesCardSound();
		for(int i = 0; i < zodiacCards.Count; i++)
		{
			string flushesContainedString = zodiacCards[i].cardData.specialCardName.Replace("ZodiacsFromFlushes", string.Empty);
			string[] flushesContainedArray = flushesContainedString.Split('¤', StringSplitOptions.RemoveEmptyEntries);
			int numberOfZodiacs = flushesContainedArray.Length;
			for(int j = 0; j < numberOfZodiacs; j++)
			{
				Vector2 cardLocation = LocalInterface.instance.GetCanvasPositionOfRectTransform(zodiacCards[i].rt, GameManager.instance.gameplayCanvas);
				Baubles.instance.ZodiacPurchased(baubleString, new Vector2(cardLocation.x, cardLocation.y + (numberOfZodiacs - 1) * 25f - j * 50f), true, false);
			}
		}
		Baubles.instance.HandInfosChanged();
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		for(int i = 0; i < zodiacCards.Count; i++)
		{
			DissolveCard(zodiacCards[i]);
		}
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DoneGainingZodiacsFromFlushes";
		// StartCoroutine(ScoreHand());
	}
	
	public IEnumerator GainZodiacsFromContainedHands()
	{
		List<Card> zodiacCards = GetAllSpecialCardsWithTag("ZodiacContainedHands");
		// string baubleString = LocalInterface.instance.GetZodiacBaubleStringFromHandTier(currentHandTier);
		SoundManager.instance.PlayZodiacsForContainedHandsSound();
		List<string> zodiacsToGain = new List<string>();
		for(int i = 0; i < currentHandsContained.Length; i++)
		{
			if(currentHandsContained[i])
			{
				zodiacsToGain.Add(LocalInterface.instance.GetZodiacBaubleStringFromHandTier(i));
			}
		}
		for(int i = 0; i < zodiacCards.Count; i++)
		{
			Vector2 cardLocation = LocalInterface.instance.GetCanvasPositionOfRectTransform(zodiacCards[i].rt, GameManager.instance.gameplayCanvas);
			for(int j = 0; j < zodiacsToGain.Count; j++)
			{
				Baubles.instance.ZodiacPurchased(zodiacsToGain[j], new Vector2(cardLocation.x, cardLocation.y + (zodiacsToGain.Count - 1) * 25f - j * 50f), true, false);
			}
		}
		Baubles.instance.HandInfosChanged();
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		for(int i = 0; i < zodiacCards.Count; i++)
		{
			DissolveCard(zodiacCards[i]);
		}
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DoneGainingZodiacsFromContainedHands";
		// StartCoroutine(ScoreHand());
	}

	public IEnumerator PerformCardConversion()
	{
		List<Card> conversionCards = GetAllSpecialCardsWithTag("ConvertCard");
		conversionCards.Sort((x, y) =>
		{
			if(x.dropZonePlacedIn.specialCardsOnly != y.dropZonePlacedIn.specialCardsOnly)
			{
				return x.dropZonePlacedIn.specialCardsOnly.CompareTo(y.dropZonePlacedIn.specialCardsOnly);
			}
			return x.dropZonePlacedIn.dropZoneNumber.CompareTo(y.dropZonePlacedIn.dropZoneNumber);
		});
		for(int i = 0; i < conversionCards.Count; i++)
		{
			if(conversionCards[i].dropZonePlacedIn.dropZoneNumber != 0)
			{
				if(conversionCards[i].dropZonePlacedIn.specialCardsOnly)
				{
					if(specialCardDropZones[conversionCards[i].dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
					{
						GameObject newMagicMirrorGO = Instantiate(magicMirrorPrefab, conversionCards[i].rt);
						MagicMirror newMagicMirror = newMagicMirrorGO.GetComponent<MagicMirror>();
						newMagicMirror.StartConversion(specialCardDropZones[conversionCards[i].dropZonePlacedIn.dropZoneNumber - 1].placedCard, conversionCards[i]);
						yield return new WaitForSeconds(1.25f / Preferences.instance.gameSpeed);
					}
				}
				else
				{
					if(standardDropZones[conversionCards[i].dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
					{
						GameObject newMagicMirrorGO = Instantiate(magicMirrorPrefab, conversionCards[i].rt);
						MagicMirror newMagicMirror = newMagicMirrorGO.GetComponent<MagicMirror>();
						newMagicMirror.StartConversion(standardDropZones[conversionCards[i].dropZonePlacedIn.dropZoneNumber - 1].placedCard, conversionCards[i]);
						yield return new WaitForSeconds(1.25f / Preferences.instance.gameSpeed);
					}
				}
			}
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration * 2 / Preferences.instance.gameSpeed);
/* 		for(int i = 0; i < conversionCards.Count; i++)
		{
			DissolveCard(conversionCards[i]);
		} */
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DoneConverting";
	}
	
	public IEnumerator PerformPromotionOrDemotion()
	{
		List<Card> standardCards = GetAllStandardCards();
		List<Card> promotionCards = GetAllSpecialCardsWithTag("Promotion");
		List<Card> demotionCards = GetAllSpecialCardsWithTag("Demotion");
		int rankChange = promotionCards.Count - demotionCards.Count;
		for(int i = 0; i < standardCards.Count; i++)
		{
			GameObject newPromotionGO = Instantiate(promotionPrefab, standardCards[i].rt);
			Promotion newPromotion = newPromotionGO.GetComponent<Promotion>();
			newPromotion.StartPromotion(standardCards[i], rankChange);
			yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration * 2 / Preferences.instance.gameSpeed);
		for(int i = 0; i < promotionCards.Count; i++)
		{
			DissolveCard(promotionCards[i]);
		}
		for(int i = 0; i < demotionCards.Count; i++)
		{
			DissolveCard(demotionCards[i]);
		}
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DonePromoting";
		// StartCoroutine(ScoreHand());
	}
	
	public IEnumerator ChangeAllStandardCardRanks()
	{
		List<Card> standardCards = GetAllStandardCards();
		int randomRank = RNG.instance.hands.Range(0, 13);
		for(int i = 0; i < standardCards.Count; i++)
		{
			GameObject newMagicMarkerGO = Instantiate(magicMarkerPrefab, standardCards[i].rt);
			MagicMarker newMagicMarker = newMagicMarkerGO.GetComponent<MagicMarker>();
			newMagicMarker.StartMarker(standardCards[i], randomRank);
			yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration * 2 / Preferences.instance.gameSpeed);
		List<Card> magicMarkerCards = GetAllSpecialCardsWithTag("ChangeAllRanks");
		for(int i = 0; i < magicMarkerCards.Count; i++)
		{
			DissolveCard(magicMarkerCards[i]);
		}
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DoneChangingRanks";
		// StartCoroutine(ScoreHand());
	}
	
	public IEnumerator PaintCardsRainbow()
	{
		List<Card> standardCards = GetAllStandardCards();
		for(int i = 0; i < standardCards.Count; i++)
		{
			if(standardCards[i].cardData.suit < 4)
			{
				GameObject newRainbowPaintGO = Instantiate(rainbowPaintPrefab, standardCards[i].rt);
				RainbowPaint newRainbowPaint = newRainbowPaintGO.GetComponent<RainbowPaint>();
				newRainbowPaint.StartPaint(standardCards[i]);
				yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4 / Preferences.instance.gameSpeed);
			}
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration * 2 / Preferences.instance.gameSpeed);
		List<Card> rainbowPaintCards = GetAllSpecialCardsWithTag("RainbowPaint");
		for(int i = 0; i < rainbowPaintCards.Count; i++)
		{
			DissolveCard(rainbowPaintCards[i]);
		}
		HandUpdated();
		yield return null;
		currentStateOfScoringRoutine = "DonePaintingCards";
		// StartCoroutine(ScoreHand());
	}
	
	public void DissolveCard(Card cardToDissolve)
	{
		Deck.instance.cardsInHand.Remove(cardToDissolve);
		if(cardToDissolve.dropZonePlacedIn != null)
		{
			cardToDissolve.dropZonePlacedIn.CardRemoved();
		}
		GameObject newDissolveGO = Instantiate(dissolvePrefab, cardToDissolve.rt);
		Dissolve newDissolve = newDissolveGO.GetComponent<Dissolve>();
		newDissolve.StartDissolve(cardToDissolve.rt);
	}
	
	public bool HandContainsSpecialCardTag(string tag)
	{
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				// if(specialCardDropZones[i].placedCard.cardData.isSpecialCard && specialCardDropZones[i].placedCard.cardData.specialCardName == tag)
				if(specialCardDropZones[i].placedCard.cardData.isSpecialCard && specialCardDropZones[i].placedCard.cardData.specialCardName.Length >= tag.Length && specialCardDropZones[i].placedCard.cardData.specialCardName.Substring(0, tag.Length) == tag)
				{
					return true;
				}
			}
		}
		if(V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect || Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones") > 0)
		{
			for(int i = 0; i < standardDropZones.Length; i++)
			{
				if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
				{
					if(standardDropZones[i].placedCard.cardData.isSpecialCard && standardDropZones[i].placedCard.cardData.specialCardName.Length >= tag.Length && standardDropZones[i].placedCard.cardData.specialCardName.Substring(0, tag.Length) == tag)
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	
	public List<Card> GetAllSpecialCardsWithTag(string tag)
	{
		List<Card> specialCardsWithTag = new List<Card>();
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				if(specialCardDropZones[i].placedCard.cardData.isSpecialCard && specialCardDropZones[i].placedCard.cardData.specialCardName.Length >= tag.Length && specialCardDropZones[i].placedCard.cardData.specialCardName.Substring(0, tag.Length) == tag)
				{
					specialCardsWithTag.Add(specialCardDropZones[i].placedCard);
				}
			}
		}
		if(V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect || Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones") > 0)
		{
			for(int i = 0; i < standardDropZones.Length; i++)
			{
				if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
				{
					if(standardDropZones[i].placedCard.cardData.isSpecialCard && standardDropZones[i].placedCard.cardData.specialCardName.Length >= tag.Length && standardDropZones[i].placedCard.cardData.specialCardName.Substring(0, tag.Length) == tag)
					{
						specialCardsWithTag.Add(standardDropZones[i].placedCard);
					}
				}
			}
		}
		return specialCardsWithTag;
	}
	
	public bool IsAMushroomPlayed()
	{
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				if(specialCardDropZones[i].placedCard.cardData.isSpecialCard && (specialCardDropZones[i].placedCard.cardData.specialCardName == "IncreaseValue" || specialCardDropZones[i].placedCard.cardData.specialCardName == "IncreaseMult" || specialCardDropZones[i].placedCard.cardData.specialCardName == "DoubleValueAndMult"))
				{
					return true;
				}
			}
		}
		if(V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect || Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones") > 0)
		{
			for(int i = 0; i < standardDropZones.Length; i++)
			{
				if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
				{
					if(standardDropZones[i].placedCard.cardData.isSpecialCard && (standardDropZones[i].placedCard.cardData.specialCardName == "IncreaseValue" || standardDropZones[i].placedCard.cardData.specialCardName == "IncreaseMult" || standardDropZones[i].placedCard.cardData.specialCardName == "DoubleValueAndMult"))
					{
						return true;
					}
				}
			}
		}
		return false;
	}
	
	public IEnumerator ShootSnurfGun()
	{
		crosshair.SetParent(rt);
		crosshair.gameObject.SetActive(false);
		SoundManager.instance.PlayBottlePopSound();
		Card onlyStandardCard = GetFirstStandardCard();
		onlyStandardCard.rt.SetParent(HandArea.instance.discardedCardsParent);
		while(snurfGunDart.anchoredPosition.x < 338f)
		{
			snurfGunDart.anchoredPosition = new Vector2(snurfGunDart.anchoredPosition.x + (640f * Time.deltaTime * Preferences.instance.gameSpeed), snurfGunDart.anchoredPosition.y);
			if(snurfGunDart.anchoredPosition.x > onlyStandardCard.rt.anchoredPosition.x - 40)
			{
				onlyStandardCard.rt.anchoredPosition = new Vector2(snurfGunDart.anchoredPosition.x + 40, onlyStandardCard.rt.anchoredPosition.y);
			}
			yield return null;
		}
		snurfGunDart.anchoredPosition = new Vector2(-338f, snurfGunDart.anchoredPosition.y);
		Deck.instance.cardsInHand.Remove(onlyStandardCard);
		Destroy(onlyStandardCard.gameObject);
		EndScoringHand();
	}
	
	public void AddToPointsAndMultGainedFromCards(double pointsFromCards, double multFromCards)
	{
		pointsGainedFromCards += pointsFromCards;
		multGainedFromCards += multFromCards;
	}
	
	public void EndScoringHand()
	{
		locked = false;
		lockInHandButtonImage.sprite = unlockedButtonSprite;
		CheckForScrepublicDeckUnlock();
		lockInHandButtonImageRT.sizeDelta = new Vector2(unlockedButtonSprite.rect.width, unlockedButtonSprite.rect.height);
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf && standardDropZones[i].cardPlaced)
			{
				if(standardDropZones[i].placedCard.cardData.isSpecialCard)
				{
					DissolveCard(standardDropZones[i].placedCard);
				}
				else
				{
					standardDropZones[i].placedCard.rt.SetParent(HandArea.instance.discardedCardsParent);
					standardDropZones[i].placedCard.StartMove(HandArea.instance.discardPileLocation, Vector3.zero, false, true, true);
					standardDropZones[i].placedCard.StartFlip();
					Deck.instance.cardsInHand.Remove(standardDropZones[i].placedCard);
					standardDropZones[i].CardRemoved();
					SoundManager.instance.PlayCardSlideSound();
				}
			}
			standardDropZones[i].scorePlate.ResetScorePlate();
		}
		pointsGainedFromCards = 0;
		multGainedFromCards = 0;
		for(int i = 0; i < specialCardDropZones.Length; i++)
		{
			if(specialCardDropZones[i].gameObject.activeSelf && specialCardDropZones[i].cardPlaced)
			{
				DissolveCard(specialCardDropZones[i].placedCard);
			}
		}
		HandUpdated();
		if(ScoreVial.instance.currentRoundScore >= GameManager.instance.GetCurrentRoundScoreThreshold() - 0.1d)
		{
			HandArea.instance.DiscardEntireHand();
			if(HandArea.instance.topDeckCard != null)
			{
				HandArea.instance.topDeckCard.rt.SetParent(HandArea.instance.discardedCardsParent);
				HandArea.instance.topDeckCard.StartMove(HandArea.instance.discardPileLocation, Vector3.zero, false, true, true);
				HandArea.instance.topDeckCard.StartFlip();
				SoundManager.instance.PlayCardSlideSound();
			}
			// HandArea.instance.ShuffleDiscardPileIntoDrawPile(0.5f / Preferences.instance.gameSpeed);
			if(GameManager.instance.currentRound == 29)
			{
				RunInformation.instance.SaveGame();
				Stats.instance.RunWon();
				SoundManager.instance.PlayGameWonSound();
				RunStatsPanel.instance.UpdateFromSaveFile(RunInformation.instance.CreateSaveGameString());
				RunStatsPanel.instance.SetAvailabilityOfButtons(true, true);
				RunStatsPanel.instance.runStatsPanelIsOnScreen = true;
				MovingObjects.instance.mo["RunStatsPanel"].StartMove("OnScreen");
				MovingObjects.instance.mo["HandArea"].StartMove("OffScreen");
				MovingObjects.instance.mo["CardParent"].StartMove("OffScreen");
				MovingObjects.instance.mo["PlayArea"].StartMove("OffScreen");
				MovingObjects.instance.mo["BossInformation"].StartMove("OffScreen");
				RunStatsPanel.instance.UpdateTitleWon();
			}
			else if(GameManager.instance.currentRound == 49)
			{
				RunInformation.instance.SaveGame();
				SoundManager.instance.PlayEndlessModeWonSound();
				RunStatsPanel.instance.UpdateFromSaveFile(RunInformation.instance.CreateSaveGameString());
				RunStatsPanel.instance.SetAvailabilityOfButtons(true, false);
				RunStatsPanel.instance.runStatsPanelIsOnScreen = true;
				MovingObjects.instance.mo["RunStatsPanel"].StartMove("OnScreen");
				MovingObjects.instance.mo["HandArea"].StartMove("OffScreen");
				MovingObjects.instance.mo["CardParent"].StartMove("OffScreen");
				MovingObjects.instance.mo["PlayArea"].StartMove("OffScreen");
				MovingObjects.instance.mo["BossInformation"].StartMove("OffScreen");
				RunStatsPanel.instance.UpdateTitleForWonEndless();
				LocalInterface.instance.MoveSaveGameToFinishedGame(V.i.isDailyGame, V.i.isCustomGame, V.i.dateTimeStarted);
			}
			else
			{
				SoundManager.instance.PlayRoundClearedSound();
				StartCoroutine(MoveToShopCoroutine());
			}
		}
		else
		{
			if(GameManager.instance.IsPlayerFatigued())
			{
				HandArea.instance.DiscardEntireHand();
			}
			else
			{
				GameManager.instance.HandUntilFatigueUsed();
			}
			HandArea.instance.StartDrawCards(0.5f / Preferences.instance.gameSpeed);
			HandlePreplacedCards();
		}
	}
	
	public void CheckForScrepublicDeckUnlock()
	{
		if(!Decks.instance.decks["Screpublic"].unlocked && !V.i.isDailyGame && !V.i.isCustomGame)
		{
			List<CardData> cardsDatasInDeck = Deck.instance.GetAllCardDatasInDeck();
			for(int i = 0; i < cardsDatasInDeck.Count; i++)
			{
				if(cardsDatasInDeck[i].rank >= 9 && cardsDatasInDeck[i].rank <= 11)
				{
					return;
				}
			}
			Decks.instance.UnlockDeck("Screpublic");
		}
	}
	
	public IEnumerator MoveToShopCoroutine()
	{
		yield return new WaitForSeconds(0.5f / Preferences.instance.gameSpeed);
		HandArea.instance.StartShuffleDiscardPileIntoDrawPile();
		Shop.instance.OpenShop();
		ScoreVial.instance.StartDrainVial();
		ScoreVial.instance.vialTop.StartReturn(1f);
		yield return new WaitForSeconds(0.5f / Preferences.instance.gameSpeed);
		HandUpdated();
	}
	
	public void HandlePreplacedCards()
	{
		int preplacedCards = (V.i.v.variantSpecialOptions["PreplacedCard"].inEffect ? 1 : 0) + (GameManager.instance.IsBossTagActive("PreplacedCard") ? 1 : 0);
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].gameObject.activeSelf)
			{
				standardDropZones[i].locked = false;
			}
		}
		for(int i = 0; i < preplacedCards; i++)
		{
			if(!standardDropZones[i * 2 + 1].gameObject.activeSelf)
			{
				break;
			}
			float specialCardChance = 0.15f;
			float r = RNG.instance.hands.Range(0, 1f);
			CardData newCardData;
			if(r <= specialCardChance)
			{
				KeyValuePair<string, VariantSpecialCard> randomVariantSpecialCard = V.i.v.variantSpecialCards.ElementAt(RNG.instance.hands.Range(0, V.i.v.variantSpecialCards.Count));
				newCardData = new CardData(-1, -1, randomVariantSpecialCard.Key);
			}
			else
			{
				newCardData = new CardData(RNG.instance.hands.Range(0, 13), RNG.instance.hands.Range(0, 5));
			}
			Card preplacedCard = HandArea.instance.SpawnCard(newCardData, Vector2.zero, standardDropZones[i * 2 + 1].rt, false, false);
			preplacedCard.rt.anchoredPosition = Vector2.zero;
			preplacedCard.dropZonePlacedIn = standardDropZones[i * 2 + 1];
			standardDropZones[i * 2 + 1].locked = true;
			standardDropZones[i * 2 + 1].CardPlaced(preplacedCard);
			preplacedCard.UpdateToDropZoneImage();
		}
	}
	
	public string ConvertPreplacedCardsToString()
	{
		string preplacedCardsString = string.Empty;
		for(int i = 0; i < standardDropZones.Length; i++)
		{
			if(standardDropZones[i].locked && standardDropZones[i].cardPlaced)
			{
				preplacedCardsString += standardDropZones[i].placedCard.cardData.ConvertToText() + $"@{i}%";
			}
		}
		if(preplacedCardsString.Length > 0)
		{
			return preplacedCardsString.Substring(0, preplacedCardsString.Length - 1);
		}
		return "none";
	}
	
	public void ConvertStringToPreplacedCards(string preplacedCardsString)
	{
		string[] preplacedCardsData = preplacedCardsString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < preplacedCardsData.Length; i++)
		{
			string[] preplacedCardData = preplacedCardsData[i].Split('@', StringSplitOptions.RemoveEmptyEntries);
			int dropZoneInt = int.Parse(preplacedCardData[1]);
			Card preplacedCard = HandArea.instance.SpawnCard(new CardData(preplacedCardData[0]), Vector2.zero, standardDropZones[dropZoneInt].rt, false, false);
			preplacedCard.rt.anchoredPosition = Vector2.zero;
			preplacedCard.dropZonePlacedIn = standardDropZones[dropZoneInt];
			standardDropZones[dropZoneInt].locked = true;
			standardDropZones[dropZoneInt].CardPlaced(preplacedCard);
			preplacedCard.UpdateToDropZoneImage();
		}
	}
}
