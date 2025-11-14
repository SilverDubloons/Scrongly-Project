using UnityEngine;
using System.Collections.Generic;
using System;

public class HandsInformation : MonoBehaviour
{
	public RectTransform handsInfoBackdropRT;
	public SlideOut slideOut;
	public GameObject tabObject;
	public GameObject backdropObject;
	public Label handsLabel;
	
	public HandInfoTooltip handInfoTooltip;
	
    public GameObject handInfoPrefab;
	public TextAsset handsWithinText;
	public Color defaultInfoTextColor;
	public Color highlightedInfoTextColor;
	public Color containedInfoTextColor;
	public Color handContainedBackdropColor;
	public Color playedHandsBackdropColor;
	public Color unplayedHandsBackdropColor;
	
	public List<HandInfo> handInfos = new List<HandInfo>();
	
	public static HandsInformation instance;
	
	public void SetupInstance()
	{
		instance = this;
		handInfoTooltip.SetupInstance();
		// SetupHandInfos();
		handInfoTooltip.transform.SetSiblingIndex(handInfoTooltip.transform.parent.childCount - 1);
	}
	
	public void SetupHandInfos()
	{
		string[] handsWithinRows = handsWithinText.text.Split('\n');
		for(int i = 0; i < GameManager.instance.handNames.Length; i++)
		{
			GameObject newHandInfoGO = Instantiate(handInfoPrefab, handsInfoBackdropRT);
			newHandInfoGO.name = GameManager.instance.handNames[i];
			HandInfo newHandInfo = newHandInfoGO.GetComponent<HandInfo>();
			handInfos.Add(newHandInfo);
			newHandInfo.handNameLabel.ChangeText(GameManager.instance.handNames[i]);
			newHandInfo.handNumber = i;
			string formattedHandIntString = string.Empty;
			if(i < 10)
			{
				formattedHandIntString = $"0{i.ToString()}";
			}
			else
			{
				formattedHandIntString = i.ToString();
			}
			if(i < 10 || V.i.v.variantSpecialOptions["AllBaublesZodiacsUnlocked"].inEffect)
			{
				newHandInfo.handNameBackdrop.color = playedHandsBackdropColor;
			}
			else
			{
				newHandInfo.handNameBackdrop.color = unplayedHandsBackdropColor;
			}
			newHandInfo.formattedHandIntString = formattedHandIntString;
			string firstRowContent = handsWithinRows[i * 2 + 1].Trim();
			string[] handsContainedColumns = firstRowContent.Split(',', StringSplitOptions.RemoveEmptyEntries);
			for(int j = 0; j < handsContainedColumns.Length; j++)
			{
				// Debug.Log($"i={i}, j={j}, handsContainedColumns[j]={handsContainedColumns[j]}l");
				newHandInfo.guaranteedHandsContained.Add(int.Parse(handsContainedColumns[j]));
			}
			string secondRowContent = handsWithinRows[i * 2 + 2].Trim();
			string[] handsWithinColumns = secondRowContent.Split(',', StringSplitOptions.RemoveEmptyEntries);
			for(int j = 0; j < handsWithinColumns.Length; j++)
			{
				newHandInfo.guaranteedHandsThisHandIsIn.Add(int.Parse(handsWithinColumns[j]));
			}
			slideOut.contentControllerSelectionGroup.controllerSelectableObjects.Add(newHandInfo.handNameControllerSelectableObject);
			slideOut.contentControllerSelectionGroup.controllerSelectableObjects.Add(newHandInfo.individualControllerSelectableObject);
			slideOut.contentControllerSelectionGroup.controllerSelectableObjects.Add(newHandInfo.minimumControllerSelectableObject);
		}
	}
	
	public void OrganizeByPlayableCards()
	{
		int playableCards = GameManager.instance.GetMaxPlayableStandardCards();
		int playableHandIndex = 0;
		for(int i = 0; i < handInfos.Count; i++)
		{
			int minimumCardsRequired = GameManager.instance.handMinimumNumberOfCards[i];
			if(i < 8 && !handInfos[i].minimumObject.activeSelf)
			{
				handInfos[i].minimumObject.SetActive(true);
			}
			switch(i)
			{
				case 4:
					minimumCardsRequired = HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight();
					if(minimumCardsRequired <= 1)
					{
						for(int j = 0; j < 4; j++)
						{
							handInfos[j].minimumObject.SetActive(false);
						}
					}
				break;
				case 5:
					minimumCardsRequired = HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush();
					if(minimumCardsRequired <= 1)
					{
						for(int j = 0; j < 5; j++)
						{
							handInfos[j].minimumObject.SetActive(false);
						}
					}
				break;
				case 8:
					minimumCardsRequired = HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush();
					if(minimumCardsRequired <= 1)
					{
						for(int j = 0; j < 8; j++)
						{
							handInfos[j].minimumObject.SetActive(false);
						}
					}
				break;
			}
			if(playableCards >= minimumCardsRequired)
			{
				handInfos[i].gameObject.SetActive(true);
				handInfos[i].rt.anchoredPosition = new Vector2(2f, -20f - 18f * playableHandIndex);
				playableHandIndex++;
			}
			else
			{
				handInfos[i].gameObject.SetActive(false);
			}
		}
		handsInfoBackdropRT.sizeDelta = new Vector2(handsInfoBackdropRT.sizeDelta.x, playableHandIndex * 18f + 22f);
		handsInfoBackdropRT.anchoredPosition = new Vector2(handsInfoBackdropRT.anchoredPosition.x, Mathf.Lerp(0, -71, handsInfoBackdropRT.sizeDelta.y / LocalInterface.instance.referenceResolution.y));
	}
	
	public void HighlightPointsAndMult(bool individual, List<int> handsToHighlight)
	{
		handInfos[handsToHighlight[0]].ChangeInfoColor(!individual, highlightedInfoTextColor);
		for(int i = 0; i < handsToHighlight.Count; i++)
		{
			handInfos[handsToHighlight[i]].ChangeInfoColor(individual, containedInfoTextColor);
		}
	}
	
	public void ResetHighlights()
	{
		for(int i = 0; i < handInfos.Count; i++)
		{
			handInfos[i].ChangeInfoColor(true, defaultInfoTextColor);
			handInfos[i].ChangeInfoColor(false, defaultInfoTextColor);
		}
	}
	
	public void UpdateAllHandInfos()
	{
		for(int i = 0; i < handInfos.Count; i++)
		{
			handInfos[i].UpdateHandInfo();
		}
	}
	
	public void HandUpdated(bool[] handsContained)
	{
		for(int i = 0; i < handsContained.Length; i++)
		{
			if(handsContained[i])
			{
				handInfos[i].handNameBackdrop.color = handContainedBackdropColor;
			}
			else
			{
				if(i <= 8 || RunInformation.instance.handsPlayed[i] > 0 || V.i.v.variantSpecialOptions["AllBaublesZodiacsUnlocked"].inEffect)
				{
					handInfos[i].handNameBackdrop.color = playedHandsBackdropColor;
				}
				else
				{
					handInfos[i].handNameBackdrop.color = unplayedHandsBackdropColor;
				}
			}
		}
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
        defaultInfoTextColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.handInfoDefaultText);
		highlightedInfoTextColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.handInfoHighlightedText);
		containedInfoTextColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.handInfoContainedText);
        handContainedBackdropColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.handInfoBackdropInHand);
        playedHandsBackdropColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.handInfoBackdropStandard);
        unplayedHandsBackdropColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.handInfoBackdropUnplayed);
	}
}    