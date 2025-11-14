using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static Deck;
using System;

public class HandScoring : MonoBehaviour
{
	public RectTransform rt; // useful in slot machine
	public Label pointsLabel;
	public Label multLabel;
	public Label handNameLabel;
	
	public double currentPoints;
	public double currentMult;
	public int currentHandTier;
	public bool[] currentHandsContained;
	public List<Card> currentScoredCards;
	public List<Card> currentCards = new List<Card>();
	public List<CardData> currentCardDatas = new List<CardData>();
	public double pointsGainedFromCards;
	public double multGainedFromCards;
	
	public string currentStateOfScoringRoutine;
	public IEnumerator scoringCoroutine;
	
	void Start()
	{
		currentStateOfScoringRoutine = "NotScoring";
	}
	
	public void HandUpdated(List<Card> cards)
	{
		List<CardData> cardDatas = GetCardDatasFromCards(cards);
		currentCards = new List<Card>(cards);
		HandUpdated(cardDatas);
	}
	
    public void HandUpdated(List<CardData> cardDatas)
	{
		currentCardDatas = new List<CardData>(cardDatas);
		List<CardData> standardCards = GetCardsOfTypeFromList(cardDatas, true);
		List<CardData> specialCards = GetCardsOfTypeFromList(cardDatas, false);
		currentScoredCards.Clear();
		if(standardCards.Count > 0)
		{
			HandEvaluation.instance.EvaluateHand(standardCards, false, this);
		}
		else
		{
			if(handNameLabel != null)
			{
				handNameLabel.ChangeText(string.Empty);
			}
			if(pointsLabel != null)
			{
				pointsLabel.ChangeText("Points");
			}
			if(multLabel != null)
			{
				multLabel.ChangeText("Mult");
			}
			currentPoints = 0;
			currentMult = 0;
			currentHandTier = -1;
			currentHandsContained = new bool[18];
		}
	}
	
	public void HandEvaluated(List<CardData> cardsUsed, bool evaluatingOnlyCardsUsed, bool[] handsContained, bool isRoyalFlush = false)
	{
		if(cardsUsed != null)
		{
			if(GetCardsOfTypeFromList(currentCardDatas, true).Count > cardsUsed.Count && !evaluatingOnlyCardsUsed && Baubles.instance.GetImpactInt("UseAllCardsInPlayArea") == 0)
			{
				HandEvaluation.instance.EvaluateHand(cardsUsed, true, this);
				return;
			}
		}
		currentScoredCards.Clear();
		for(int i = 0; i < currentCards.Count; i++)
		{
			currentCards[i].xImage.gameObject.SetActive(true);
			if(Baubles.instance.GetImpactInt("UseAllCardsInPlayArea") > 0)
			{
				currentCards[i].xImage.color = LocalInterface.instance.greenXColor;
			}
			else
			{
				currentCards[i].xImage.color = LocalInterface.instance.redXColor;
			}
			if(!currentCards[i].cardData.isSpecialCard)
			{
				for(int j = 0; j < cardsUsed.Count; j++)
				{
					if(currentCards[i].cardData == cardsUsed[j])
					{
						currentScoredCards.Add(currentCards[i]);
						currentCards[i].xImage.gameObject.SetActive(false);
						break;
					}
				}
				if(Baubles.instance.GetImpactInt("UseAllCardsInPlayArea") > 0 && !currentScoredCards.Contains(currentCards[i]))
				{
					currentScoredCards.Add(currentCards[i]);
				}
			}
			else
			{
				currentCards[i].xImage.gameObject.SetActive(false);
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
					if(handNameLabel != null)
					{
						handNameLabel.ChangeText("Royal Flush");
					}
				}
				else
				{
					if(handNameLabel != null)
					{
						handNameLabel.ChangeText(GameManager.instance.handNames[i]);
					}
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
		if(pointsLabel != null)
		{
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPoints));
		}
		if(multLabel != null)
		{
			multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentMult));
		}
		currentHandsContained = handsContained;
	}
	
	public List<CardData> GetCardDatasFromCards(List<Card> cards)
	{
		List<CardData> cardDatas = new List<CardData>();
		for(int i = 0; i < cards.Count; i++)
		{
			cardDatas.Add(cards[i].cardData);
		}
		return cardDatas;
	}
	
	public List<CardData> GetCardsOfTypeFromList(List<CardData> cardDatas, bool standard) // false returns specialCards
	{
		List<CardData> cardDatasOfType = new List<CardData>();
		for(int i = 0; i < cardDatas.Count; i++)
		{
			if(cardDatas[i].isSpecialCard != standard)
			{
				cardDatasOfType.Add(cardDatas[i]);
			}
		}
		return cardDatasOfType;
	}
	
	public void AddPoints(double pointsToAdd)
	{
		currentPoints += pointsToAdd;
		if(pointsLabel != null)
		{
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPoints));
			pointsLabel.StartExpandRetract();
		}
	}
	
	public void MultiplyPoints(double multToMultiplyBy)
	{
		currentPoints = currentPoints * multToMultiplyBy;
		if(pointsLabel != null)
		{
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPoints));
			pointsLabel.StartExpandRetract();
		}
	}
	
	public void AddMult(double multToAdd)
	{
		currentMult += multToAdd;
		if(multLabel != null)
		{
			multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentMult));
			multLabel.StartExpandRetract();
		}
	}
	
	public void MultiplyMult(double multToMultiplyBy)
	{
		currentMult = currentMult * multToMultiplyBy;
		if(multLabel != null)
		{
			multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentMult));
			multLabel.StartExpandRetract();
		}
	}
	
	public void AddToPointsAndMultGainedFromCards(double pointsFromCards, double multFromCards)
	{
		pointsGainedFromCards += pointsFromCards;
		multGainedFromCards += multFromCards;
	}
	
	public void StartScoringHand()
	{
		if(currentStateOfScoringRoutine != "NotScoring")
		{
			StopCoroutine(scoringCoroutine);
			LocalInterface.instance.DisplayError($"StartScoringHand was called while currentStateOfScoringRoutine={currentStateOfScoringRoutine} on {this.name}");
		}
		scoringCoroutine = ScoreHand(false);
		StartCoroutine(scoringCoroutine);
	}
	
	public IEnumerator ScoreHand(bool useBaubles = false)
	{
		currentStateOfScoringRoutine = "Start";
		// we don't do special cards or boss levels, which saves a lot!
		currentStateOfScoringRoutine = "ScoringCards";
		StartCoroutine(GainScoreAndMultFromCards());
		while(currentStateOfScoringRoutine == "ScoringCards")
		{
			yield return null;
		}
		if(useBaubles)
		{
			if(Baubles.instance.GetImpactInt("AddPointsAndMultFromCardsAgain") > 0)
			{
				if(Math.Abs(pointsGainedFromCards) > 0.1d || Math.Abs(multGainedFromCards) > 0.1d)
				{
					// Baubles.instance.Notify("AddPointsAndMultFromCardsAgain", 0, SoundManager.instance.PlayAddPointsAndMultFromCardsAgainSound);
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
			for(int i = 0; i < currentHandsContained.Length; i++)
			{
				
			}
		}
		yield return new WaitForSeconds (0.5f / Preferences.instance.gameSpeed);
		MultiplyPoints(currentMult);
		multLabel.StartExpandRetract();
		SoundManager.instance.PlayScoreMultipliedSound();
		yield return new WaitForSeconds (0.25f / Preferences.instance.gameSpeed);
		currentStateOfScoringRoutine = "NotScoring";
	}
	
	public IEnumerator GainScoreAndMultFromCards(bool useBaubles = false)
	{
		currentScoredCards.Sort((a, b) =>
		{
			return LocalInterface.instance.GetCanvasPositionOfRectTransform(a.rt, GameManager.instance.gameplayCanvas).x.CompareTo(LocalInterface.instance.GetCanvasPositionOfRectTransform(b.rt, GameManager.instance.gameplayCanvas).x);
		});
		for(int i = 0; i < currentScoredCards.Count; i++)
		{
			if(currentScoredCards[i].scorePlate == null)
			{
				if(GameManager.instance.spareScorePlateParent.childCount > 0)
				{
					GameManager.instance.spareScorePlateParent.GetChild(GameManager.instance.spareScorePlateParent.childCount - 1).gameObject.SetActive(true);
					ScorePlate storedScorePlate = GameManager.instance.spareScorePlateParent.GetChild(GameManager.instance.spareScorePlateParent.childCount - 1).GetComponent<ScorePlate>();
					storedScorePlate.rt.SetParent(currentScoredCards[i].rt);
					storedScorePlate.rt.anchoredPosition = new Vector2(0, -10f);
					currentScoredCards[i].scorePlate = storedScorePlate;
				}
				else
				{
					GameObject newScorePlateGO = Instantiate(GameManager.instance.scorePlatePrefab, currentScoredCards[i].rt);
					ScorePlate newScorePlate = newScorePlateGO.GetComponent<ScorePlate>();
					newScorePlate.rt.SetParent(currentScoredCards[i].rt);
					newScorePlate.rt.anchoredPosition = new Vector2(0, -10f);
					currentScoredCards[i].scorePlate = newScorePlate;
				}
			}
			else
			{
				currentScoredCards[i].scorePlate.gameObject.SetActive(true);
			}
		}
		if(useBaubles)
		{
			Card monarchCard = null;
			if(Baubles.instance.GetImpactInt("MultToMonarch") > 0)
			{
				List<Card> scoredFaceCards = GetAllScoredFaceCards();
				if(scoredFaceCards.Count > 0)
				{
					monarchCard = scoredFaceCards[RNG.instance.hands.Range(0, scoredFaceCards.Count)];
					monarchCard.scorePlate.multToAdd += Baubles.instance.GetImpactDouble("MultToMonarch") * scoredFaceCards.Count;
				}
			}
			// bool lastHigh = false;
			for(int i = 0; i < currentScoredCards.Count; i++)
			{
				if(currentScoredCards[i] == monarchCard)
				{
					Baubles.instance.Notify("MultToMonarch", 0.25f / Preferences.instance.gameSpeed * currentScoredCards.Count, SoundManager.instance.PlaySandwichMonarchSound);
				}
				if(Baubles.instance.GetImpactInt("PointsToNumberedCards") > 0)
				{
					if(currentScoredCards[i].cardData.rank <= 8 || Baubles.instance.GetImpactInt("AllCardsAreNumberedCards") > 0)
					{
						currentScoredCards[i].scorePlate.pointsToAdd += Baubles.instance.GetImpactDouble("PointsToNumberedCards");
						Baubles.instance.Notify("PointsToNumberedCards", 0.25f / Preferences.instance.gameSpeed * currentScoredCards.Count, SoundManager.instance.PlayVampireSound);
					}
				}
				if(Baubles.instance.GetImpactInt("AcesStraights") > 0 && currentHandsContained[4])
				{
					if(currentScoredCards[i].cardData.rank == 12 || Baubles.instance.GetImpactInt("AllCardsAreAces") > 0)
					{
						currentScoredCards[i].scorePlate.pointsToAdd += Baubles.instance.GetImpactDouble("AcesStraights");
						currentScoredCards[i].scorePlate.multToAdd += Baubles.instance.GetImpactDouble("AcesStraights", true);
						Baubles.instance.Notify("AcesStraights", 0.25f / Preferences.instance.gameSpeed * currentScoredCards.Count);
					}
				}
			}
		}
		for(int i = 0; i < currentScoredCards.Count; i++)
		{
			currentScoredCards[i].scorePlate.points = currentScoredCards[i].cardData.baseValue;
			currentScoredCards[i].scorePlate.mult = currentScoredCards[i].cardData.multiplier;
			currentScoredCards[i].scorePlate.StartScorePlate(currentScoredCards.Count, this, currentScoredCards[i]);
			yield return new WaitForSeconds (0.25f / Preferences.instance.gameSpeed);
		}
		yield return new WaitForSeconds (3f / Preferences.instance.gameSpeed);
		yield return null;
		currentStateOfScoringRoutine = "DoneGainingPointsAndMultFromCards";
		/* for(int i = 0; i < currentScoredCards.Count; i++)
		{
			if(currentScoredCards[i] == monarchCard)
			{
				Baubles.instance.Notify("MultToMonarch", 0.25f / Preferences.instance.gameSpeed * currentScoredCards.Count, SoundManager.instance.PlaySandwichMonarchSound);
				// Stats.instance.FaceCardScoredWithBauble(); // probably don't do this? it's not the same as playing hands
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
			if(addToPointsCards.Count > 0)
			{
				currentScoredCards[i].dropZonePlacedIn.scorePlate.pointsToAdd += addToPointsCards.Count * V.i.v.variantSpecialCards["IncreaseValue"].impact;
				currentScoredCards[i].dropZonePlacedIn.scorePlate.additiveMushroomUsed = true;
			}
			if(addToMultCards.Count > 0)
			{
				currentScoredCards[i].dropZonePlacedIn.scorePlate.multToAdd += addToMultCards.Count * V.i.v.variantSpecialCards["IncreaseMult"].impact;
				currentScoredCards[i].dropZonePlacedIn.scorePlate.additiveMushroomUsed = true;
			}
			if(doublePointsAndMultCards.Count > 0)
			{
				currentScoredCards[i].dropZonePlacedIn.scorePlate.multiplicitiveMushroomFactor = Math.Pow(V.i.v.variantSpecialCards["DoubleValueAndMult"].impact, doublePointsAndMultCards.Count);
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
		 */
	}
	
	public List<Card> GetAllScoredFaceCards()
	{
		List<Card> scoredFaceCards = new List<Card>();
		for(int i = 0; i < currentScoredCards.Count; i++)
		{
			if(!currentScoredCards[i].cardData.isSpecialCard && (currentScoredCards[i].cardData.rank >= 9 && currentScoredCards[i].cardData.rank <= 11) || (Baubles.instance.GetImpactInt("AllCardsAreFaceCards") > 0))
			{
				scoredFaceCards.Add(currentScoredCards[i]);
			}
		}
		return scoredFaceCards;
	}
}
