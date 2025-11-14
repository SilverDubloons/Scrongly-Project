using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class HandInfo : MonoBehaviour
{
	public RectTransform rt;
	public Image handNameBackdrop;
	public GameObject minimumObject;
	public RectTransform handNameRT;
	public Label handNameLabel;
    public Label individualPointsLabel;
    public Label individualMultLabel;
    public Label minimumPointsLabel;
    public Label minimumMultLabel;
	public ControllerSelectableObject handNameControllerSelectableObject;
	public ControllerSelectableObject individualControllerSelectableObject;
	public ControllerSelectableObject minimumControllerSelectableObject;
	
	public int handNumber;
	public string formattedHandIntString;
	
	public List<int> guaranteedHandsContained = new List<int>();
	public List<int> guaranteedHandsThisHandIsIn = new List<int>();
	
	public void UpdateHandInfo()
	{
		double updatedIndividualPoints = Baubles.instance.GetHandPoints(handNumber, formattedHandIntString);
		string updatedIndividualPointsString = LocalInterface.instance.ConvertDoubleToString(updatedIndividualPoints);
		if(updatedIndividualPointsString.Length > 4)
		{
			individualPointsLabel.ChangeFontSize(LocalInterface.instance.smallFontSize);
		}
		else
		{
			individualPointsLabel.ChangeFontSize(LocalInterface.instance.largeFontSize);
		}
		individualPointsLabel.ChangeText(updatedIndividualPointsString);
		
		double updatedIndividualMult = Baubles.instance.GetHandMult(handNumber, formattedHandIntString);
		string updatedIndividualMultString = LocalInterface.instance.ConvertDoubleToString(updatedIndividualMult);
		if(updatedIndividualMultString.Length > 4)
		{
			individualMultLabel.ChangeFontSize(LocalInterface.instance.smallFontSize);
		}
		else
		{
			individualMultLabel.ChangeFontSize(LocalInterface.instance.largeFontSize);
		}
		individualMultLabel.ChangeText(updatedIndividualMultString);
		
		double updatedMinimumPoints = 0d;
		double updatedMinimumMult = 0d;
		for(int i = 0; i < guaranteedHandsContained.Count; i++)
		{
			updatedMinimumPoints += Baubles.instance.GetHandPoints(guaranteedHandsContained[i]);
			updatedMinimumMult += Baubles.instance.GetHandMult(guaranteedHandsContained[i]);
		}
		if(!guaranteedHandsContained.Contains(4) && (GameManager.instance.handMinimumNumberOfCards[handNumber] - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight())
		{
			updatedMinimumPoints += Baubles.instance.GetHandPoints(4);
			updatedMinimumMult += Baubles.instance.GetHandMult(4);
		}
		if(!guaranteedHandsContained.Contains(5) && (GameManager.instance.handMinimumNumberOfCards[handNumber] - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush())
		{
			updatedMinimumPoints += Baubles.instance.GetHandPoints(5);
			updatedMinimumMult += Baubles.instance.GetHandMult(5);
		}
		if(!guaranteedHandsContained.Contains(8) && (GameManager.instance.handMinimumNumberOfCards[handNumber] - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush())
		{
			updatedMinimumPoints += Baubles.instance.GetHandPoints(8);
			updatedMinimumMult += Baubles.instance.GetHandMult(8);
		}
		string updatedMinimumPointsString = LocalInterface.instance.ConvertDoubleToString(updatedMinimumPoints);
		if(updatedMinimumPointsString.Length > 4)
		{
			minimumPointsLabel.ChangeFontSize(LocalInterface.instance.smallFontSize);
		}
		else
		{
			minimumPointsLabel.ChangeFontSize(LocalInterface.instance.largeFontSize);
		}
		minimumPointsLabel.ChangeText(updatedMinimumPointsString);
		string updatedMinimumMultString = LocalInterface.instance.ConvertDoubleToString(updatedMinimumMult);
		if(updatedMinimumMultString.Length > 4)
		{
			minimumMultLabel.ChangeFontSize(LocalInterface.instance.smallFontSize);
		}
		else
		{
			minimumMultLabel.ChangeFontSize(LocalInterface.instance.largeFontSize);
		}
		minimumMultLabel.ChangeText(updatedMinimumMultString);
	}
	
	public void OnMouseEnterIndividual()
    {
		List<int> handsToHighlight = new List<int>(guaranteedHandsThisHandIsIn);
		if(handNumber == 4)
		{
			for(int i = 5; i <= 17; i++)
			{
				if(i == 5)
				{
					if(!handsToHighlight.Contains(i) && (HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush() - 1) / 4 + 1 >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight())
					{
						handsToHighlight.Add(i);
					}
				}
				else if(i == 8)
				{
					if(!handsToHighlight.Contains(i) && (HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush() - 1) / 4 + 1 >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight())
					{
						handsToHighlight.Add(i);
					}
				}
				else if(!handsToHighlight.Contains(i) && (GameManager.instance.handMinimumNumberOfCards[i] - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight())
				{
					handsToHighlight.Add(i);
				}
			}
		}
		else if(handNumber == 5)
		{
			for(int i = 6; i <= 17; i++)
			{
				if(i == 8)
				{
					if(!handsToHighlight.Contains(i) && (HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush() - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush())
					{
						handsToHighlight.Add(i);
					}
				}
				else if(!handsToHighlight.Contains(i) && (GameManager.instance.handMinimumNumberOfCards[i] - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush())
				{
					handsToHighlight.Add(i);
				}
			}
		}
		else if(handNumber == 8)
		{
			for(int i = 9; i <= 17; i++)
			{
				if(!handsToHighlight.Contains(i) && (GameManager.instance.handMinimumNumberOfCards[i] - 1) / 4 + 1  >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush())
				{
					handsToHighlight.Add(i);
				}
			}
		}
		HandsInformation.instance.HighlightPointsAndMult(false, handsToHighlight);
	}
	
	public void OnMouseEnterMinimum()
    {
		List<int> handsToHighlight = new List<int>(guaranteedHandsContained);
		int cardsNeededToMakeHand = GameManager.instance.handMinimumNumberOfCards[handNumber];
		if(handNumber == 4)
		{
			cardsNeededToMakeHand = HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight();
		}
		else if(handNumber == 5)
		{
			cardsNeededToMakeHand = HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush();
		}
		else if(handNumber == 8)
		{
			cardsNeededToMakeHand = HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush();
		}
		if(!handsToHighlight.Contains(4) && (cardsNeededToMakeHand - 1) / 4 + 1 >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight())
		{
			handsToHighlight.Add(4);
		}
		if(!handsToHighlight.Contains(5) && (cardsNeededToMakeHand - 1) / 4 + 1 >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush())
		{
			handsToHighlight.Add(5);
		}
		if(!handsToHighlight.Contains(8) && (cardsNeededToMakeHand - 1) / 4 + 1 >= HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush())
		{
			handsToHighlight.Add(8);
		}
		HandsInformation.instance.HighlightPointsAndMult(true, handsToHighlight);
	}
	
	public void OnMouseEnterHandName(bool useController = false)
	{
		// Debug.Log($"OnMouseEnterHandName of handNumber={handNumber}");
		if(Preferences.instance.showHandTooltips)
		{
			HandInfoTooltip.instance.SetupTooltip(handNumber, useController, handNameRT);
		}
	}
	
	public void OnMouseExitHandName()
	{
		// Debug.Log($"OnMouseExitHandName of handNumber={handNumber}");
		List<RaycastResult> results = new List<RaycastResult>();
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = Input.mousePosition;
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach(RaycastResult result in results)
		{
			if (result.gameObject != null)
			{
				if(result.gameObject == HandInfoTooltip.instance.backdropObject)
				{
					return;
				}
			}
		}
		Tooltip.instance.DisableTooltip();
	}
	
	public void OnMouseExit()
	{
		HandsInformation.instance.ResetHighlights();
	}
	
	public void ChangeInfoColor(bool individual, Color newColor) // false implies minimum
	{
		if(individual)
		{
			individualPointsLabel.ChangeColor(newColor);
			individualMultLabel.ChangeColor(newColor);
		}
		else
		{
			minimumPointsLabel.ChangeColor(newColor);
			minimumMultLabel.ChangeColor(newColor);
		}
	}
}
