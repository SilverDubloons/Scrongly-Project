using UnityEngine;
using UnityEngine.UI;
using static Deck;
using System.Collections.Generic;

public class DeckPreview : MonoBehaviour
{
	public RectTransform rt;
    public Label[] rankQuantityLabels;
	public Label[] suitQuantityLabels;
	public Label[] individualQuantityLabels;
	public Image[] suitImages;
	public Image[] suitBackdropImages;
	public Label specialQuantityLabel;
	public GameObject specialQuantityBackdrop;
	public RectTransform[] suitRects;
	
	public bool displayingDrawPile;
	public bool displayingDiscardPile;
	// public bool previewHasRainbowCards;
	
	public static DeckPreview instance;
	
	public void MouseOverDeck(bool overDrawPile)
	{
		if(overDrawPile)
		{
			displayingDrawPile = true;
			PopulateDeckPreview(Deck.instance.drawPile, true);
		}
		else
		{
			displayingDiscardPile = true;
			PopulateDeckPreview(Deck.instance.discardPile);
		}
		int activeSuits = 0;
		for(int i = 0; i <= 4; i++)
		{
			if(suitRects[i].gameObject.activeSelf)
			{
				activeSuits++;
			}
		}
		MovingObjects.instance.mo["DeckPreview"].StartMove($"OnScreen{activeSuits}Suits", 0, 4f);
		if(!Shop.instance.inShop && !RunStatsPanel.instance.runStatsPanelIsOnScreen && !SlotMachine.instance.visibilityObject.activeSelf)
		{
			MovingObjects.instance.mo["HandArea"].StartMove($"OnScreenDeckPreview{activeSuits}Suits", 0, 4f);
			MovingObjects.instance.mo["CardParent"].StartMove($"OnScreenDeckPreview{activeSuits}Suits", 0, 4f);
			MovingObjects.instance.mo["PlayArea"].StartMove("OffScreen", 0, 4f);
			MovingObjects.instance.mo["BossInformation"].StartMove("OffScreen", 0, 4f);
		}
		SoundManager.instance.PlaySlideOutSound();
	}
	
	public void MouseExited()
	{
		SoundManager.instance.PlaySlideOutSound(true);
		MovingObjects.instance.mo["DeckPreview"].StartMove("OffScreen", 0, 4f);
		if(!Shop.instance.inShop && !RunStatsPanel.instance.runStatsPanelIsOnScreen && !SlotMachine.instance.visibilityObject.activeSelf)
		{
			MovingObjects.instance.mo["HandArea"].StartMove("OnScreenDeckPreview", 0, 4f);
			MovingObjects.instance.mo["CardParent"].StartMove("OnScreen", 0, 4f);
			MovingObjects.instance.mo["PlayArea"].StartMove("OnScreen", 0, 4f);
			MovingObjects.instance.mo["BossInformation"].StartMove("OnScreen", 0, 4f);
		}
		displayingDrawPile = false;
		displayingDiscardPile = false;
	}
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void RearangeSuits()
	{
		int activeIndex = 0;
		for(int i = 0; i <= 4; i++)
		{
			// Debug.Log($"i={i}, LocalInterface.instance.suitOrderIndices[i]={LocalInterface.instance.suitOrderIndices[i]}");
			if(suitRects[LocalInterface.instance.suitOrderIndices[i]].gameObject.activeSelf)
			{
				suitRects[LocalInterface.instance.suitOrderIndices[i]].anchoredPosition = new Vector2(suitRects[LocalInterface.instance.suitOrderIndices[i]].anchoredPosition.x, -50f - 24 * activeIndex);
				activeIndex++;
			}
			if(i < 4)
			{
				suitImages[i].color = LocalInterface.instance.suitColors[i];
				suitBackdropImages[i].color = LocalInterface.instance.suitColors[i];
			}
		}
	}
	
	public void PopulateDeckPreview(List<CardData> cardDatas, bool includeTopDeckCard = false)
	{
		int[,] suitRankCards = new int[5,13];
		int[] suitCards = new int[5];
		int[] rankCards = new int[13];
		int specialCards = 0;
		for(int i = 0; i < cardDatas.Count; i++)
		{
			if(cardDatas[i].isSpecialCard)
			{
				specialCards++;
			}
			else
			{
				suitRankCards[cardDatas[i].suit, cardDatas[i].rank]++;
				suitCards[cardDatas[i].suit]++;
				rankCards[cardDatas[i].rank]++;
			}
		}
		if(includeTopDeckCard)
		{
			if(HandArea.instance.topDeckCard != null)
			{
				if(HandArea.instance.topDeckCard.cardData.isSpecialCard)
				{
					specialCards++;
				}
				else
				{
					suitRankCards[HandArea.instance.topDeckCard.cardData.suit, HandArea.instance.topDeckCard.cardData.rank]++;
					suitCards[HandArea.instance.topDeckCard.cardData.suit]++;
					rankCards[HandArea.instance.topDeckCard.cardData.rank]++;
				}
			}
		}
		for(int i = 0; i < rankQuantityLabels.Length; i++)
		{
			rankQuantityLabels[i].ChangeText(rankCards[i].ToString());
			if(rankCards[i] >= 10)
			{
				rankQuantityLabels[i].ChangeFontSize(8);
			}
			else
			{
				rankQuantityLabels[i].ChangeFontSize(16);
			}
		}
		if(specialCards > 0)
		{
			specialQuantityLabel.ChangeText(specialCards.ToString());
			specialQuantityBackdrop.SetActive(true);
		}
		else
		{
			specialQuantityBackdrop.SetActive(false);
		}
		for(int i = 0; i < suitQuantityLabels.Length; i++)
		{
			string suitQuantityString = suitCards[i].ToString();
			if(i < 4 && suitCards[4] > 0)
			{
				suitQuantityString += $"({(suitCards[i] + suitCards[4]).ToString()})";
			}
			suitQuantityLabels[i].ChangeText(suitQuantityString);
			if((suitCards[i] >= 100 && suitCards[i] + suitCards[4] >= 10) || (suitCards[i] >= 10 && suitCards[i] + suitCards[4] >= 100))
			{
				suitQuantityLabels[i].ChangeFontSize(8);
			}
			else
			{
				suitQuantityLabels[i].ChangeFontSize(16);
			}
		}
		for(int i = 0; i < individualQuantityLabels.Length; i++)
		{
			individualQuantityLabels[i].ChangeText(suitRankCards[i / 13, i % 13].ToString());
			if(suitRankCards[i / 13, i % 13] >= 10)
			{
				individualQuantityLabels[i].ChangeFontSize(8);
			}
			else
			{
				individualQuantityLabels[i].ChangeFontSize(16);
			}
		}
		int activeSuits = 0;
		for(int i = 0; i < suitCards.Length; i++)
		{
			if(suitCards[i] == 0)
			{
				suitRects[i].gameObject.SetActive(false);
			}
			else
			{
				activeSuits++;
				suitRects[i].gameObject.SetActive(true);
			}
		}
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, 53f + 24f * activeSuits);
		RearangeSuits();
		// previewHasRainbowCards = false;
		/* if(suitCards[4] == 0)
		{
			suitRects[4].gameObject.SetActive(false);
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, 149f);
			previewHasRainbowCards = false;
		}
		else
		{
			suitRects[4].gameObject.SetActive(true);
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, 173f);
			previewHasRainbowCards = true;
		} */
	}
}
