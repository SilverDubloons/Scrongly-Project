using UnityEngine;
using UnityEngine.UI;
using static Variant;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class Deck : MonoBehaviour
{
    public List<CardData> drawPile = new List<CardData>();
	public List<Card> cardsInHand = new List<Card>();
	public List<CardData> discardPile = new List<CardData>();
	
	public Label drawPileLabel;
	public Label discardPileLabel;
	public Image drawPileCardBack;
	public Image drawPileBackdrop;
	public Image discardPileCardBack;
	
	public Color drawPileBackdropBaseColor;
	public Color drawPileBackdropDarkColor;
	
	public IEnumerator drawPileWarningCoroutine;
	public bool drawPileWarningRunning;
	
	public static Deck instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public class CardData
	{
		public int rank;	// 0 - 12, duece through ace
		public int suit;	// 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
		public float baseValue;
		public float multiplier;
		public bool isSpecialCard;
		public string specialCardName; // if empty, is standard card
		public int deckViewerRow;
		public int deckViewerRowIndex;
		public CardData(int r, int s, string scn = "")
		{
			rank = r;
			suit = s;
			specialCardName = scn;
			if(scn == "")
			{
				isSpecialCard = false;
			}
			else
			{
				isSpecialCard = true;
			}
			baseValue = GetBaseValueByRank(r);
			deckViewerRow = -1;
			deckViewerRowIndex = -1;
		}
		public string ConvertToText()
		{
			if(isSpecialCard)
			{
				return specialCardName;
			}
			else
			{
				return $"{LocalInterface.instance.ConvertRankAndSuitToString(rank, suit)}|{baseValue.ToString()}|{multiplier.ToString()}";
			}
		}
		public CardData(string cardDataText)
		{
			if(cardDataText.Contains('|')) // standard
			{
				isSpecialCard = false;
				string[] cardDataData = cardDataText.Split('|');
				string rankAndSuit = cardDataData[0];
				switch(rankAndSuit[0])
				{
					case '2':
						rank = 0;
					break;
					case '3':
						rank = 1;
					break;
					case '4':
						rank = 2;
					break;
					case '5':
						rank = 3;
					break;
					case '6':
						rank = 4;
					break;
					case '7':
						rank = 5;
					break;
					case '8':
						rank = 6;
					break;
					case '9':
						rank = 7;
					break;
					case 'T':
						rank = 8;
					break;
					case 'J':
						rank = 9;
					break;
					case 'Q':
						rank = 10;
					break;
					case 'K':
						rank = 11;
					break;
					case 'A':
						rank = 12;
					break;
					default:
						LocalInterface.instance.DisplayError($"Failed to parse rank of cardDataText={cardDataText}");
					break;
				}
				switch(rankAndSuit[1])
				{
					case 's':
						suit = 0;
					break;
					case 'c':
						suit = 1;
					break;
					case 'h':
						suit = 2;
					break;
					case 'd':
						suit = 3;
					break;
					case 'r':
						suit = 4;
					break;
					default:
						LocalInterface.instance.DisplayError($"Failed to parse suit of cardDataText={cardDataText}");
					break;
				}
				baseValue = float.Parse(cardDataData[1]);
				multiplier = float.Parse(cardDataData[2]);
				deckViewerRow = -1;
				deckViewerRowIndex = -1;
			}
			else
			{
				rank = -1;
				suit = -1;
				specialCardName = cardDataText;
				isSpecialCard = true;
				deckViewerRow = -1;
				deckViewerRowIndex = -1;
			}
		}
	}
	
	public List<CardData> GetListOfCardDatasFromText(string cardsText)
	{
		List<CardData> cardDatasToReturn = new List<CardData>();
		string[] cardsData = cardsText.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < cardsData.Length; i++)
		{
			cardDatasToReturn.Add(new CardData(cardsData[i]));
		}
		return cardDatasToReturn;
	}
	
	public static float GetBaseValueByRank(int r)
	{
		if(r < 8)
		{
			return r + 2;
		}
		else if(r < 12)
		{
			return 10;
		}
		else if(r == 12)
		{
			return 15;
		}
		else
		{
			LocalInterface.instance.DisplayError("GetBaseValueByRank called with value > 12");
			return 0;
		}
	}
	
	public void CreateDeck(List<VariantCard> startingDeck)
	{
		for(int i = 0; i < startingDeck.Count; i++)
		{
			if(startingDeck[i].isSpecial)
			{
				drawPile.Add(new CardData(-1, -1, startingDeck[i].specialCardTag));
			}
			else
			{
				if(V.i.chosenDeck != "Screpublic" || V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect == true || (startingDeck[i].rank < 9 || startingDeck[i].rank > 11))
				{
					drawPile.Add(new CardData(startingDeck[i].rank, startingDeck[i].suit));
				}
			}
		}
		if(V.i.chosenDeck == "Schromatic" && V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect == false)
		{
			for(int i = 0; i < 13; i++)
			{
				drawPile.Add(new CardData(i, 4));
			}
		}
		for(int i = 0; i < V.i.v.numberOfRandomStandardCardsToAddToDeck; i++)
		{
			if(V.i.v.includeRainbowInRandomCards)
			{
				int r = RNG.instance.starting.Range(0, 65);
				while(V.i.chosenDeck == "Screpublic" && V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect == false && r % 13 >= 9 && r % 13 <= 11)
				{
					r = RNG.instance.starting.Range(0, 65);
				}
				drawPile.Add(new CardData(r % 13, r / 13));
			}
			else
			{
				int r = RNG.instance.starting.Range(0, 52);
				while(V.i.chosenDeck == "Screpublic" && V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect == false && r % 13 >= 9 && r % 13 <= 11)
				{
					r = RNG.instance.starting.Range(0, 52);
				}
				drawPile.Add(new CardData(r % 13, r / 13));
			}
		}
		if(V.i.v.numberOfRandomSpecialCardsToAddToDeck > 1)
		{
			if(V.i.v.considerRarity)
			{
				List<string> availableCommonSpecialCards = new List<string>();
				List<string> availableUnommonSpecialCards = new List<string>();
				List<string> availableRareSpecialCards = new List<string>();
				List<string> availableLegendarySpecialCards = new List<string>();
				foreach(KeyValuePair<string, VariantSpecialCard> entry in V.i.v.variantSpecialCards)
				{
					if(entry.Value.inShop && (!entry.Value.mustBeUnlocked || LocalInterface.instance.IsBaubleUnlocked(entry.Key)))
					{
						switch(entry.Value.category)
						{
							case "Common":
								availableCommonSpecialCards.Add(entry.Key);
							break;
							case "Uncommon":
								availableUnommonSpecialCards.Add(entry.Key);
							break;
							case "Rare":
								availableRareSpecialCards.Add(entry.Key);
							break;
							case "Legendary":
								availableLegendarySpecialCards.Add(entry.Key);
							break;
						}
					}
				}
				for(int i = 0; i < V.i.v.numberOfRandomSpecialCardsToAddToDeck; i++)
				{
					int commonSpecialCardWeight = V.i.v.variantSpecialOptions["CommonSpecialCardWeight"].impact;
					if(availableCommonSpecialCards.Count <= 0)
					{
						commonSpecialCardWeight = 0;
					}
					int uncommonSpecialCardWeight = V.i.v.variantSpecialOptions["UncommonSpecialCardWeight"].impact;
					if(availableUnommonSpecialCards.Count <= 0)
					{
						uncommonSpecialCardWeight = 0;
					}
					int rareSpecialCardWeight = V.i.v.variantSpecialOptions["RareSpecialCardWeight"].impact;
					if(availableRareSpecialCards.Count <= 0)
					{
						rareSpecialCardWeight = 0;
					}
					int legendarySpecialCardWeight = V.i.v.variantSpecialOptions["LegendarySpecialCardWeight"].impact;
					if(availableLegendarySpecialCards.Count <= 0)
					{
						legendarySpecialCardWeight = 0;
					}
					int r = RNG.instance.starting.Range(0, commonSpecialCardWeight + uncommonSpecialCardWeight + rareSpecialCardWeight + legendarySpecialCardWeight);
					string specialCardTag = string.Empty;
					if(r < commonSpecialCardWeight)
					{
						int rb = RNG.instance.starting.Range(0, availableCommonSpecialCards.Count);
						specialCardTag = availableCommonSpecialCards[rb];
					}
					else if(r < commonSpecialCardWeight + uncommonSpecialCardWeight)
					{
						int rb = RNG.instance.starting.Range(0, availableUnommonSpecialCards.Count);
						specialCardTag = availableUnommonSpecialCards[rb];
					}
					else if(r < commonSpecialCardWeight + uncommonSpecialCardWeight + rareSpecialCardWeight)
					{
						int rb = RNG.instance.starting.Range(0, availableRareSpecialCards.Count);
						specialCardTag = availableRareSpecialCards[rb];
					}
					else
					{
						int rb = RNG.instance.starting.Range(0, availableLegendarySpecialCards.Count);
						specialCardTag = availableLegendarySpecialCards[rb];
					}
					drawPile.Add(new CardData(-1, -1, specialCardTag));
				}
			}
			else
			{
				List<string> availableSpecialCards = new List<string>();
				foreach(KeyValuePair<string, VariantSpecialCard> entry in V.i.v.variantSpecialCards)
				{
					if(entry.Value.inShop && entry.Value.category != "Special" && (!entry.Value.mustBeUnlocked || LocalInterface.instance.IsBaubleUnlocked(entry.Key)))
					{
						availableSpecialCards.Add(entry.Key);
					}
				}
				for(int i = 0; i < V.i.v.numberOfRandomSpecialCardsToAddToDeck; i++)
				{
					drawPile.Add(new CardData(-1, -1, availableSpecialCards[RNG.instance.starting.Range(0, availableSpecialCards.Count)]));
				}
			}
		}
		drawPileCardBack.sprite = V.i.chosenDeckSprite;
		discardPileCardBack.sprite = V.i.chosenDeckSprite;
		UpdateCardsInDrawPile();
		UpdateCardsInDiscardPile();
	}
	
	public void StackDeck(string[] cardsInOrder)
	{
		for(int i = 0; i < cardsInOrder.Length; i++)
		{
			Vector2Int rankSuit = LocalInterface.instance.ParseCardString(cardsInOrder[i]);
			int foundCardIndex = -1;
			for(int j = 0; j < drawPile.Count; j++)
			{
				if(drawPile[j].rank == rankSuit.x && drawPile[j].suit == rankSuit.y)
				{
					foundCardIndex = j;
					break;
				}
			}
			CardData tempCard = drawPile[drawPile.Count - 1 - i];
			drawPile[drawPile.Count - 1 - i] = drawPile[foundCardIndex];
			drawPile[foundCardIndex] = tempCard;
		}
	}
	
	public void ShuffleDrawPile()
	{
		for(int i = 0; i < drawPile.Count; i++)
		{
			int r = RNG.instance.shuffle.Range(0, i + 1);
			CardData temp = drawPile[i];
			drawPile[i] = drawPile[r];
			drawPile[r] = temp;
		}
		if(V.i.isTutorial)
		{
			switch(GameManager.instance.currentRound)
			{
				case 0:
					string[] firstRoundStackedDeck = {"Qh", "9h", "2s", "Qd", "6s", "Ac", "2c", /* < starting hand */
					"7d", "Kd", "8c", "3h", /* < first drawn cards */
					"Ts", "3d", "Ah"};
					StackDeck(firstRoundStackedDeck);
				break;
				case 1:
					string[] secondRoundStackedDeck = {"Qs", "4d", "9c", "2s", "Th", "Jc", "7h"};
					StackDeck(secondRoundStackedDeck);
				break;
			}
		}
	}
	
	public List<CardData> ShuffleListOfCardDatas(List<CardData> cardsToShuffle)
	{
		List<CardData> randomizedList = new List<CardData>(cardsToShuffle);
		for(int i = 0; i < randomizedList.Count; i++)
		{
			int r = RNG.instance.shuffle.Range(0, i + 1);
			CardData temp = randomizedList[i];
			randomizedList[i] = randomizedList[r];
			randomizedList[r] = temp;
		}
		return randomizedList;
	}
	
	public void UpdateCardsInDrawPile()
	{
		int cardsInDrawPile = drawPile.Count;
		if(HandArea.instance.topDeckCard != null)
		{
			cardsInDrawPile++;
			// Debug.Log($"drawPile.Count={drawPile.Count}, cardsInDrawPile={cardsInDrawPile} topDeckCard={HandArea.instance.topDeckCard.name}");
		}
		else
		{
			// Debug.Log($"drawPile.Count={drawPile.Count}, cardsInDrawPile={cardsInDrawPile} topDeckCard=null");
		}
		drawPileLabel.ChangeText(cardsInDrawPile.ToString());
		drawPileCardBack.gameObject.SetActive(drawPile.Count > 0 ? true : false);
		if(DeckPreview.instance.displayingDrawPile)
		{
			DeckPreview.instance.PopulateDeckPreview(drawPile);
		}
		// Debug.Log($"drawPileWarningRunning={drawPileWarningRunning}, cardsInDrawPile={cardsInDrawPile}, (cardsInDrawPile + discardPile.Count + cardsInHand.Count) * 0.33f = {(cardsInDrawPile + discardPile.Count + cardsInHand.Count) * 0.33f}");
		if(drawPileWarningRunning)
		{
			if(cardsInDrawPile >= (cardsInDrawPile + discardPile.Count + cardsInHand.Count) * 0.33f)
			{
				StopCoroutine(drawPileWarningCoroutine);
				drawPileBackdrop.color = drawPileBackdropBaseColor;
				drawPileWarningRunning = false;
			}
		}
		else if(cardsInDrawPile < (cardsInDrawPile + discardPile.Count + cardsInHand.Count) * 0.33f)
		{
			drawPileWarningCoroutine = DrawPileWarningCoroutine();
			StartCoroutine(drawPileWarningCoroutine);
		}
	}
	
	public IEnumerator DrawPileWarningCoroutine()
	{
		drawPileWarningRunning = true;
		float colorChangeTime = 1f;
		float t = 0;
		while(t < colorChangeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime, 0, colorChangeTime);
			drawPileBackdrop.color = Color.Lerp(drawPileBackdropBaseColor, drawPileBackdropDarkColor, LocalInterface.instance.animationCurve.Evaluate(t / colorChangeTime));
			yield return null;
		}
		t = 0;
		while(t < colorChangeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime, 0, colorChangeTime);
			drawPileBackdrop.color = Color.Lerp(drawPileBackdropDarkColor, drawPileBackdropBaseColor, LocalInterface.instance.animationCurve.Evaluate(t / colorChangeTime));
			yield return null;
		}
		drawPileWarningCoroutine = DrawPileWarningCoroutine();
		StartCoroutine(drawPileWarningCoroutine);
	}
	
/* 	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			if(HandArea.instance.topDeckCard != null)
			{
				Debug.Log($"drawPile.Count={drawPile.Count} topDeckCard={HandArea.instance.topDeckCard.name}");
			}
			else
			{
				Debug.Log($"drawPile.Count={drawPile.Count} topDeckCard=null");
			}
		}
	} */
	
	public void UpdateCardsInDiscardPile()
	{
		discardPileLabel.ChangeText(discardPile.Count.ToString());
		discardPileCardBack.gameObject.SetActive(discardPile.Count > 0 ? true : false);
		if(DeckPreview.instance.displayingDiscardPile)
		{
			DeckPreview.instance.PopulateDeckPreview(discardPile);
		}
	}
	
	public int GetNumberOfStandardCardsInHand()
	{
		int standardCards = 0;
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(!cardsInHand[i].cardData.isSpecialCard)
			{
				standardCards++;
			}
		}
		return standardCards;
	}
	
	public void AddCardToDrawPile(CardData cardDataToAdd)
	{
		drawPile.Add(cardDataToAdd);
		UpdateCardsInDrawPile();
	}
	
	public List<CardData> GetAllCardDatasInDeck()
	{
		List<CardData> cardsToReturn = new List<CardData>(drawPile);
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			cardsToReturn.Add(cardsInHand[i].cardData);
		}
		for(int i = 0; i < discardPile.Count; i++)
		{
			cardsToReturn.Add(discardPile[i]);
		}
		return cardsToReturn;
	}
	
	public List<CardData> GetAllCardDatasOfTypeInDeck(bool special)
	{
		List<CardData> cardDatasToReturn = new List<CardData>();
		for(int i = 0; i < drawPile.Count; i++)
		{
			if(drawPile[i].isSpecialCard == special)
			{
				cardDatasToReturn.Add(drawPile[i]);
			}
		}
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(cardsInHand[i].cardData.isSpecialCard == special)
			{
				cardDatasToReturn.Add(cardsInHand[i].cardData);
			}
		}
		for(int i = 0; i < discardPile.Count; i++)
		{
			if(discardPile[i].isSpecialCard == special)
			{
				cardDatasToReturn.Add(discardPile[i]);
			}
		}
		return cardDatasToReturn;
	}
	
	void Start()
	{
        ThemeManager.instance.OnThemeChanged += ApplyTheme;
        ApplyTheme();
    }
	
	void OnDestroy() 
	{
        if(ThemeManager.instance != null)
		{
            ThemeManager.instance.OnThemeChanged -= ApplyTheme;
		}
    }
	
	public void ApplyTheme()
	{
        drawPileBackdropBaseColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.backdrop);
		drawPileBackdropDarkColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.darkBackdrop);
    }
	
	public void ResetGraphicsOfAllSpecialCardsWithTag(string tag)
	{
		for(int i = 0; i < cardsInHand.Count; i++)
		{
			if(cardsInHand[i].cardData.isSpecialCard && cardsInHand[i].cardData.specialCardName.Length >= tag.Length && cardsInHand[i].cardData.specialCardName.Substring(0, tag.Length) == tag)
			{
				cardsInHand[i].UpdateGraphics();
			}
		}
	}
}
