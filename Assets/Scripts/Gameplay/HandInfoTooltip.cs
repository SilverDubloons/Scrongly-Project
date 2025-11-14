using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Deck;

public class HandInfoTooltip : MonoBehaviour, IPointerExitHandler
{
	public RectTransform rt;
	public RectTransform handNameBackdropRT;
	public RectTransform handDescriptionBackdropRT;
    public Label handNameLabel;
    public Label handDescriptionLabel;
	public Card[] cards;
	public GameObject backdropObject;
	public int currentHandNumber;
	public const float cardY = -18f;
	
	public const float tooltipDistanceFromMouse = 5f;
	
	public static HandInfoTooltip instance;
	
	public void SetupInstance()
	{
		instance = this;
		backdropObject.SetActive(false);
	}
	
	public void SetupTooltip(int handNumber, bool useController, RectTransform handNameRT)
	{
		useController = true;
		backdropObject.SetActive(true);
		currentHandNumber = handNumber;
		handNameLabel.ChangeText(GameManager.instance.handNames[handNumber]);
		handNameBackdropRT.sizeDelta = new Vector2(handNameLabel.GetPreferredValuesString(900f).x + 6f, handNameBackdropRT.sizeDelta.y);
		// handNameBackdropRT.sizeDelta = new Vector2(handNameLabel.GetPreferredWidth() + 6f, handNameBackdropRT.sizeDelta.y);
		if(handNumber == 4)
		{
			SetupTooltipForStraight(false);
		}
		else if(handNumber == 5)
		{
			SetupTooltipForFlush();
		}
		else if(handNumber == 8)
		{
			SetupTooltipForStraight(true);
		}
		else
		{
			SetupTooltipForStandardHand(handNumber);
		}
		float descriptionOptimalHeight = handDescriptionLabel.GetPreferredHeight();
		handDescriptionLabel.rt.sizeDelta = new Vector2(handDescriptionLabel.rt.sizeDelta.x, descriptionOptimalHeight + 2);
		// handDescriptionBackdropRT.sizeDelta = new Vector2(handDescriptionBackdropRT.sizeDelta.x, descriptionOptimalHeight + 6);
		// handDescriptionBackdropRT.sizeDelta = new Vector2(handDescriptionLabel.GetPreferredValuesString(900f).x + 6f, descriptionOptimalHeight + 6);
		
		// Debug.Log($"handDescriptionBackdropRT.sizeDelta={handDescriptionBackdropRT.sizeDelta}, descriptionOptimalHeight={descriptionOptimalHeight}");
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, descriptionOptimalHeight + 74); // this right here
		// handDescriptionBackdropRT.sizeDelta = new Vector2(handDescriptionLabel.GetPreferredWidth() + 6f, descriptionOptimalHeight + 6);
		handDescriptionBackdropRT.sizeDelta = new Vector2(handDescriptionLabel.GetPreferredValuesString(rt.sizeDelta.x - 12f).x + 6f, descriptionOptimalHeight + 6);
		Vector2 mousePos = LocalInterface.instance.GetMousePosition();
		float tooltipPosX = mousePos.x + rt.sizeDelta.x / 2 + tooltipDistanceFromMouse;
		float tooltipPosY = mousePos.y;
		if(useController)
		{
			tooltipPosX = LocalInterface.instance.GetCanvasPositionOfRectTransform(handNameRT, GameManager.instance.gameplayCanvas).x + 100f + rt.sizeDelta.x / 2 ;
			tooltipPosY = LocalInterface.instance.GetCanvasPositionOfRectTransform(handNameRT, GameManager.instance.gameplayCanvas).y;
		}
		if(tooltipPosX > LocalInterface.instance.referenceResolution.x / 2 - rt.sizeDelta.x / 2)
		{
			tooltipPosX = mousePos.x - rt.sizeDelta.x / 2 - tooltipDistanceFromMouse;
		}
		if(tooltipPosY > LocalInterface.instance.referenceResolution.y / 2 - rt.sizeDelta.y / 2)
		{
			tooltipPosY = LocalInterface.instance.referenceResolution.y / 2 - rt.sizeDelta.y / 2;
		}
		if(tooltipPosY < -LocalInterface.instance.referenceResolution.y / 2 + rt.sizeDelta.y / 2)
		{
			tooltipPosY = -LocalInterface.instance.referenceResolution.y / 2 + rt.sizeDelta.y / 2;
		}
		tooltipPosX = rt.sizeDelta.x / 2 - 124f; // this is just better
		rt.anchoredPosition = new Vector2(tooltipPosX, tooltipPosY);
	}
	
	public void SetupTooltipForStraight(bool straightFlush)
	{
		int cardsNeeded = 0;
		if(straightFlush)
		{
			cardsNeeded =  HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush();
		}
		else
		{
			cardsNeeded =  HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight();
		}
		rt.sizeDelta = new Vector2(Mathf.Max(cardsNeeded * 49f + 5f, 100f), 100f);
		int gap = GameManager.instance.GetMaxStraightGap();
		bool wrap = Baubles.instance.GetImpactInt("StraightsCanWrap") > 0 ? true : false;
		for(int i = 0; i < cards.Length; i++)
		{
			if(i < cardsNeeded)
			{
				cards[i].gameObject.SetActive(true);
				cards[i].rt.anchoredPosition = new Vector2((cardsNeeded - 1) * -24.5f + 49f * i, cardY);
			}
			else
			{
				cards[i].gameObject.SetActive(false);
			}
		}
		int sr0 = UnityEngine.Random.Range(0, 4);
		int sr1 = -1;
		while(sr1 < 0 || sr1 == sr0)
		{
			sr1 = UnityEngine.Random.Range(0, 4);
		}
		int sr2 = -1;
		while(sr2 < 0 || sr2 == sr0 || sr2 == sr1)
		{
			sr2 = UnityEngine.Random.Range(0, 4);
		}
		int sr3 = -1;
		while(sr3 < 0 || sr3 == sr0 || sr3 == sr1 || sr3 == sr2)
		{
			sr3 = UnityEngine.Random.Range(0, 4);
		}
		int r = 0;
		if(wrap)
		{
			r = 11 - gap;
		}
		else
		{
			r = Random.Range(0, 8 - gap * 2);
		}
		cards[0].cardData = new CardData(r, sr0);
		cards[0].UpdateGraphics();
		if(cardsNeeded > 1)
		{
			r = WrapInt(r + gap + 1, 12);
			cards[1].cardData = new CardData(r, straightFlush ? sr0 : sr1);
			cards[1].UpdateGraphics();
			if(cardsNeeded > 2)
			{
				int poolRemaining = 9 - gap * 2;
				int nextGap = UnityEngine.Random.Range(1, Mathf.Min(gap + 2, poolRemaining + 1));
				poolRemaining -= nextGap;
				r = WrapInt(r + nextGap, 12);
				cards[2].cardData = new CardData(r, straightFlush ? sr0 : sr2);
				cards[2].UpdateGraphics();
				if(cardsNeeded > 3)
				{
					nextGap = UnityEngine.Random.Range(1, Mathf.Min(gap + 2, poolRemaining + 1));
					poolRemaining -= nextGap;
					r = WrapInt(r + nextGap, 12);
					cards[3].cardData = new CardData(r, straightFlush ? sr0 : sr3);
					cards[3].UpdateGraphics();
					if(cardsNeeded > 4)
					{
						nextGap = UnityEngine.Random.Range(1, Mathf.Min(gap + 2, poolRemaining + 1));
						poolRemaining -= nextGap;
						r = WrapInt(r + nextGap, 12);
						cards[4].cardData = new CardData(r, sr0);
						cards[4].UpdateGraphics();
					}
				}
			}
		}
		if(cardsNeeded <= 1)
		{
			if(straightFlush)
			{
				handDescriptionLabel.ChangeText("All hands are considered straight flushes");
			}
			else
			{
				handDescriptionLabel.ChangeText("All hands are considered straights");
			}
		}
		else
		{
			string description = "";
			if(straightFlush)
			{
				description = $"At least {LocalInterface.instance.numbers[cardsNeeded]} cards of consecutive rank, all the same suit";
			}
			else
			{
				description = $"At least {LocalInterface.instance.numbers[cardsNeeded]} cards of consecutive rank";
			}
			if(gap > 0)
			{
				description += $", with at most {LocalInterface.instance.numbers[gap]} gap{(gap > 1 ? "s" : "")} between them";
			}
			if(wrap)
			{
				if(straightFlush)
				{
					description += ". Straight flushes may wrap";
				}
				else
				{
					description += ". Straights may wrap";
				}
			}
			handDescriptionLabel.ChangeText(description);
		}
	}
	
	public int WrapInt(int x, int maxValue)
	{
		while(x > maxValue)
		{
			x -= (maxValue + 1);
		}
		return x;
	}
	
	public void SetupTooltipForFlush()
	{
		int cardsNeeded =  HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush();
		rt.sizeDelta = new Vector2(Mathf.Max(cardsNeeded * 49f + 5f, 100f), 100f);
		for(int i = 0; i < cards.Length; i++)
		{
			if(i < cardsNeeded)
			{
				cards[i].gameObject.SetActive(true);
				cards[i].rt.anchoredPosition = new Vector2((cardsNeeded - 1) * -24.5f + 49f * i, cardY);
			}
			else
			{
				cards[i].gameObject.SetActive(false);
			}
		}
		int gap = GameManager.instance.GetMaxStraightGap() + 1;
		int s = UnityEngine.Random.Range(0, 4);
		int r = UnityEngine.Random.Range(0, 7 - gap);
		int poolRemaining = 8 - r;
		cards[0].cardData = new CardData(r, s);
		cards[0].UpdateGraphics();
		if(cardsNeeded > 1)
		{
			r = r + gap + 1;
			cards[1].cardData = new CardData(r, s);
			cards[1].UpdateGraphics();
			if(cardsNeeded > 2)
			{
				int newGap = UnityEngine.Random.Range(1, (poolRemaining > 6 ? 4 : 3));
				poolRemaining -= newGap;
				r += newGap;
				cards[2].cardData = new CardData(r, s);
				cards[2].UpdateGraphics();
				if(cardsNeeded > 3)
				{
					newGap = UnityEngine.Random.Range(1, (poolRemaining > 5 ? 4 : 3));
					poolRemaining -= newGap;
					r += newGap;
					cards[3].cardData = new CardData(r, s);
					cards[3].UpdateGraphics();
					if(cardsNeeded > 4)
					{
						r += UnityEngine.Random.Range(1, poolRemaining);
						cards[4].cardData = new CardData(r, s);
						cards[4].UpdateGraphics();
					}
				}
			}
			handDescriptionLabel.ChangeText($"At least {LocalInterface.instance.numbers[cardsNeeded]} cards of the same suit");
		}
		else
		{
			handDescriptionLabel.ChangeText($"All hands are considered flushes");
		}
		
	}
	
	public void SetupTooltipForStandardHand(int handNumber)
	{
		rt.sizeDelta = new Vector2(Mathf.Max(GameManager.instance.handMinimumNumberOfCards[handNumber] * 49f + 5f, 100f), 100f);
		if(handNumber <= 8 || RunInformation.instance.handsPlayed[handNumber] > 0 || V.i.v.variantSpecialOptions["AllBaublesZodiacsUnlocked"].inEffect)
		{
			handDescriptionLabel.ChangeText(GameManager.instance.handDescriptions[handNumber]);
		}
		else
		{
			handDescriptionLabel.ChangeText($"{GameManager.instance.handDescriptions[handNumber]}\n<color=red>This hand has not been played yet, so any related Baubles and Zodiacs will not appear in the shop</color>", true);
		}
		for(int i = 0; i < cards.Length; i++)
		{
			if(i < GameManager.instance.handMinimumNumberOfCards[handNumber])
			{
				cards[i].gameObject.SetActive(true);
				cards[i].rt.anchoredPosition = new Vector2((GameManager.instance.handMinimumNumberOfCards[handNumber] - 1) * -24.5f + 49f * i, cardY);
			}
			else
			{
				cards[i].gameObject.SetActive(false);
			}
		}
		int sr0 = UnityEngine.Random.Range(0, 4);
		int sr1 = -1;
		while(sr1 < 0 || sr1 == sr0)
		{
			sr1 = UnityEngine.Random.Range(0, 4);
		}
		int sr2 = -1;
		while(sr2 < 0 || sr2 == sr0 || sr2 == sr1)
		{
			sr2 = UnityEngine.Random.Range(0, 4);
		}
		int sr3 = -1;
		while(sr3 < 0 || sr3 == sr0 || sr3 == sr1 || sr3 == sr2)
		{
			sr3 = UnityEngine.Random.Range(0, 4);
		}
		int rr0 = UnityEngine.Random.Range(0, 13);
		int rr1 = -1;
		while(rr1 < 0 || rr1 == rr0)
		{
			rr1 = UnityEngine.Random.Range(0, 13);
		}
		int rr2 = -1;
		while(rr2 < 0 || rr2 == rr0 || rr2 == rr1)
		{
			rr2 = UnityEngine.Random.Range(0, 13);
		}
		cards[0].cardData = new CardData(rr0, sr0);
		cards[0].UpdateGraphics();
		if(handNumber > 0)
		{
			cards[1].cardData = new CardData(rr0, sr1);
			cards[1].UpdateGraphics();
			if(handNumber > 1)
			{
				if(handNumber == 2 || handNumber == 10)
				{
					cards[2].cardData = new CardData(rr1, sr2);
					cards[2].UpdateGraphics();
					cards[3].cardData = new CardData(rr1, sr3);
					cards[3].UpdateGraphics();
					if(handNumber == 10)
					{
						cards[4].cardData = new CardData(rr2, sr0);
						cards[4].UpdateGraphics();
						cards[5].cardData = new CardData(rr2, sr1);
						cards[5].UpdateGraphics();
					}
				}
				else
				{
					cards[2].cardData = new CardData(rr0, sr2);
					cards[2].UpdateGraphics();
					if(handNumber == 6 || handNumber == 11 || handNumber == 14)
					{
						cards[3].cardData = new CardData(rr1, sr3);
						cards[3].UpdateGraphics();
						cards[4].cardData = new CardData(rr1, sr0);
						cards[4].UpdateGraphics();
						if(handNumber == 11)
						{
							cards[5].cardData = new CardData(rr1, sr1);
							cards[5].UpdateGraphics();
						}
						else if(handNumber == 14)
						{
							cards[5].cardData = new CardData(rr2, sr1);
							cards[5].UpdateGraphics();
							cards[6].cardData = new CardData(rr2, sr2);
							cards[6].UpdateGraphics();
						}
					}
					else if(handNumber > 3)
					{
						cards[3].cardData = new CardData(rr0, sr3);
						cards[3].UpdateGraphics();
						if(handNumber == 12 || handNumber == 15)
						{
							cards[4].cardData = new CardData(rr1, sr0);
							cards[4].UpdateGraphics();
							cards[5].cardData = new CardData(rr1, sr1);
							cards[5].UpdateGraphics();
							if(handNumber == 15)
							{
								cards[6].cardData = new CardData(rr1, sr2);
								cards[6].UpdateGraphics();
							}
						}
						else if(handNumber > 7)
						{
							cards[4].cardData = new CardData(rr0, sr0);
							cards[4].UpdateGraphics();
							if(handNumber == 16)
							{
								cards[5].cardData = new CardData(rr1, sr1);
								cards[5].UpdateGraphics();
								cards[6].cardData = new CardData(rr1, sr2);
								cards[6].UpdateGraphics();
							}
							else if(handNumber > 9)
							{
								cards[5].cardData = new CardData(rr0, sr1);
								cards[5].UpdateGraphics();
								if(handNumber == 17)
								{
									cards[6].cardData = new CardData(rr0, sr2);
									cards[6].UpdateGraphics();
								}
							}
						}
					}
				}
			}
		}
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		bool mouseOverHands = false;
		foreach(RaycastResult result in results)
		{
			if(result.gameObject != null)
			{
				if(result.gameObject == HandsInformation.instance.tabObject || result.gameObject == HandsInformation.instance.backdropObject)
				{
					mouseOverHands = true;
					break;
				}
				if(result.gameObject == HandsInformation.instance.handInfos[currentHandNumber].handNameBackdrop.gameObject)
				{
					return;
				}
			}
		}
		if(!mouseOverHands)
		{
			// Debug.Log("OnPointerExit of HandInfoTooltip");
			SoundManager.instance.PlaySlideOutSound(true);
			HandsInformation.instance.slideOut.mouseOver = false;
		}
		DisableTooltip();
	}
	
	public void DisableTooltip()
	{
		backdropObject.SetActive(false);
	}
}
