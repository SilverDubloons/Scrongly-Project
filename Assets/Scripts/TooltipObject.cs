using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TooltipObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string mainText;
	public string title;
	public ThemeManager.UIElementType titleColor;
	public string subtitle;
	public ThemeManager.UIElementType subtitleColor;
	
	public bool isCommonTooltip;
	public bool isBauble;
	public bool isSpecialCard;
	public bool isZodiac;
	public bool isSpecial;
	public bool ignoreTooltip;
	
	public string specialTag;
	
	public bool hasProgressBar;
	public string progressBarTag;
	
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
		
		// Debug.Log($"TooltipObject OnPointerEnter on {this.gameObject.name} with parent {this.GetComponent<Transform>().parent.gameObject.name}");
		DisplayTooltip(false);
	}
	
	public void DisplayTooltip(bool useController)
	{
		if(!Preferences.instance.showCommonTooltips && isCommonTooltip)
		{
			return;
		}
		if((isBauble && !Preferences.instance.showBaubleTooltips) || (isSpecialCard && !Preferences.instance.showSpecialCardTooltips) || (isZodiac && !Preferences.instance.showZodiacTooltips) || ignoreTooltip)
		{
			return;
		}
		if(isSpecial)
		{
			switch(specialTag)
			{
				case "DiscardButton":
					if(!(HandArea.instance.selectedCards.Count > HandArea.instance.GetMaxCardsDiscardedAtOnce()))
					{
						return;
					}
					mainText = $"You may only discard {HandArea.instance.GetMaxCardsDiscardedAtOnce().ToString()} cards at once";
				break;
				case "HandsUntilFatigue":
					if(!(GameManager.instance.IsPlayerFatigued()))
					{
						return;
					}
				break;
				case "Interest":
					if(Shop.instance.currentInterestChips.Count <= 0)
					{
						return;
					}
					mainText = $"You earn 1 chip in interest for every {V.i.v.variantSpecialOptions["ChipToInterestRatio"].impact} chips you have when you leave the shop, up to a maximum of {Shop.instance.GetMaxInterest()}";
				break;
				case "Layaway":
					if(Shop.instance.layawayItem != null)
					{
						return;
					}
				break;
				case "TargetFramerate":
					if(Preferences.instance.framerateDropdown.interactable)
					{
						return;
					}
				break;
				case "DailyGameButton":
					if(Stats.instance.GetStatInt("standardRunsPlayed") > 0)
					{
						return;
					}
				break;
				case "CustomGameButton":
					if(Stats.instance.GetStatInt("dailyRunsPlayed") > 0)
					{
						return;
					}
				break;
			}
		}
		if(Tooltip.instance.currentObject != this.gameObject || useController)
		{
			if(hasProgressBar)
			{
				switch(progressBarTag)
				{
					case "IncreaseChipThresholds":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("chipThresholdsCleared"), 150, useController);
					break;
					case "AllCardsAreFaceCards":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("faceCardsScoredWithBauble"), 50, useController);
					break;
					case "AllCardsAreNumberedCards":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("numberedCardsScoredWithBauble"), 300, useController);
					break;
					case "AllCardsAreAces":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("acesScoredWithBauble"), 50, useController);
					break;
					case "SlotMachine":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("sevensScored"), 50, useController);
					break;
					case "DiscardTriplesForMushrooms":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("threeOfAKindsDiscarded"), 10, useController);
					break;
					case "IncreaseMushroomPowerTriples":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("mushroomsPlayed"), 15, useController);
					break;
					case "GetMushroomsFromHands":
						Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, true, Stats.instance.GetStatInt("fiveOfAKindsPlayed"), 20, useController);
					break;
				}
			}
			else
			{
				// Debug.Log($"TooltipObject DisplayTooltip on {this.gameObject.name} with parent {this.GetComponent<Transform>().parent.gameObject.name}");
				Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, false, 0, 0, useController);
			}
		}
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach(RaycastResult result in results)
		{
			if (result.gameObject != null)
			{
				if(result.gameObject == Tooltip.instance.tooltipObject)
				{
					return;
				}
			}
		}
		DisableTooltip();
	}
	
	public void DisableTooltip()
	{
		Tooltip.instance.DisableTooltip();
	}
	
	public void RepositionTooltip(bool useController = false)
	{
		Tooltip.instance.ShowTooltip(this.gameObject, mainText, title, titleColor, subtitle, subtitleColor, false, 0, 0, useController);
	}
}
