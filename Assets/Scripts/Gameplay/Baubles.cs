using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Linq;

public class Baubles : MonoBehaviour
{
	public RectTransform baubleNotificationParent;
	
	public GameObject baubleNotificationPrefab;
	
	public Vector2 baubleNotificationMinorNotificationLocation;
	public float baubleNotificationMinorNotificationDelay;
	public float baubleNotificationMinorNotificationFadeTime;
    
	public Dictionary<string, int> owned = new Dictionary<string, int>();
	public List<string> disabledBaubles = new List<string>();
	
	public Sprite[] diceSprites;
	public string[] diceDescriptions;
	
	public static Baubles instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public int GetImpactInt(string tag, bool impact2 = false, bool ignoreDisabled = false)
	{
		if(disabledBaubles.Contains(tag) && !ignoreDisabled)
		{
			return 0;
		}
		int impact = 0;
		if(V.i.v.variantBaubles.ContainsKey(tag))
		{
			impact = impact2 ?  (int)Math.Round(V.i.v.variantBaubles[tag].impact2) : (int)Math.Round(V.i.v.variantBaubles[tag].impact1);
		}
		else
		{
			LocalInterface.instance.DisplayError($"GetImpactInt called on tag \"{tag}\" which is not in the Variant dictionary");
			return 0;
		}
		int quantityOwned = 0;
		if(owned.ContainsKey(tag))
		{
			quantityOwned = owned[tag];
		}
		return impact * quantityOwned;
	}
	
	public double GetImpactDouble(string tag, bool impact2 = false, bool ignoreDisabled = false)
	{
		if(disabledBaubles.Contains(tag) && !ignoreDisabled)
		{
			return 0;
		}
		double impact = 0;
		if(V.i.v.variantBaubles.ContainsKey(tag))
		{
			impact = impact2 ? V.i.v.variantBaubles[tag].impact2 : V.i.v.variantBaubles[tag].impact1;
		}
		else
		{
			LocalInterface.instance.DisplayError($"GetImpactDouble called on tag \"{tag}\" which is not in the Variant dictionary");
			return 0;
		}
		int quantityOwned = 0;
		if(owned.ContainsKey(tag))
		{
			quantityOwned = owned[tag];
		}
		return impact * quantityOwned;
	}
	
	public double GetImpactFloat(string tag, bool impact2 = false, bool ignoreDisabled = false)
	{
		if(disabledBaubles.Contains(tag) && !ignoreDisabled)
		{
			return 0;
		}
		float impact = 0;
		if(V.i.v.variantBaubles.ContainsKey(tag))
		{
			impact = impact2 ? (float)V.i.v.variantBaubles[tag].impact2 : (float)V.i.v.variantBaubles[tag].impact1;
		}
		else
		{
			LocalInterface.instance.DisplayError($"GetImpactFloat called on tag \"{tag}\" which is not in the Variant dictionary");
			return 0;
		}
		int quantityOwned = 0;
		if(owned.ContainsKey(tag))
		{
			quantityOwned = owned[tag];
		}
		return impact * quantityOwned;
	}
	
	public int GetQuantityOwned(string tag, bool ignoreDisabled = false)
	{
		if(disabledBaubles.Contains(tag) && !ignoreDisabled)
		{
			return 0;
		}
		if(!owned.ContainsKey(tag))
		{
			return 0;
		}
		else
		{
			return owned[tag];
		}
	}
	
	public double GetHandPoints(int handNumber)
	{
		string formattedHandIntString = handNumber.ToString();
		if(handNumber < 10)
		{
			formattedHandIntString = $"0{handNumber.ToString()}";
		}
		return GameManager.instance.handPoints[handNumber] + GetImpactDouble($"Hand{formattedHandIntString}Power");
	}
	
	public double GetHandMult(int handNumber)
	{
		string formattedHandIntString = handNumber.ToString();
		if(handNumber < 10)
		{
			formattedHandIntString = $"0{handNumber.ToString()}";
		}
		return GameManager.instance.handMults[handNumber] + GetImpactDouble($"Hand{formattedHandIntString}Power", true);
	}
	
	public double GetHandPoints(int handNumber, string formattedHandIntString)
	{
		return GameManager.instance.handPoints[handNumber] + GetImpactDouble($"Hand{formattedHandIntString}Power");
	}
	
	public double GetHandMult(int handNumber, string formattedHandIntString)
	{
		return GameManager.instance.handMults[handNumber] + GetImpactDouble($"Hand{formattedHandIntString}Power", true);
	}
	
	public void BaublePurchased(string baubleTag, Vector2 origin, bool ignoreSpecialEffects = false, bool countAsGained = true, bool issueEffects = true)
	{
		if(countAsGained)
		{
			Stats.instance.BaubleGained();
		}
		if(owned.ContainsKey(baubleTag))
		{
			owned[baubleTag]++;
		}
		else
		{
			owned.Add(baubleTag, 1);
		}
		BaublesInformation.instance.BaublePurchased(baubleTag, origin);
		PurchasedItems.instance.BaublePurchased(baubleTag, origin);
		if(!ignoreSpecialEffects)
		{
			BaublesInformation.instance.baublesLabel.StartExpandRetract();
		}
		if(issueEffects)
		{
			switch(baubleTag)
			{
				case "IncreaseChipThresholds":
					ScoreVial.instance.SetupChipThresholdsForNewRound();
				break;
				case "IncreaseHandsUntilFatigue":
					GameManager.instance.ResetHandsUntilFatigue();
				break;
				case "IncreaseDiscards":
					GameManager.instance.ResetDiscards();
				break;
				case "IncreasePlayableStandardCardCount":
				case "IncreasePlayableSpecialCardCount":
					PlayArea.instance.ResizePlayArea();
					HandsInformation.instance.OrganizeByPlayableCards();
					HandsInformation.instance.UpdateAllHandInfos();
				break;
				case "DecreaseCardsNeededForStraight":
				case "DecreaseCardsNeededForFlush":
					HandsInformation.instance.OrganizeByPlayableCards();
					HandsInformation.instance.UpdateAllHandInfos();
					if(GetQuantityOwned("DecreaseCardsNeededForStraightFlush", true) < Mathf.Min(GetQuantityOwned("DecreaseCardsNeededForStraight", true), GetQuantityOwned("DecreaseCardsNeededForFlush", true)))
					{
						BaublePurchased("DecreaseCardsNeededForStraightFlush", Vector2.zero, true, true);
						ItemEarnedNotifications.instance.Notify("Special Bauble Earned", "For buying both a Plastic Brick and Snakes bauble", "EarnedBauble", "DecreaseCardsNeededForStraightFlush", V.i.v.variantBaubles["DecreaseCardsNeededForStraightFlush"].sprite);
					}
				break;
				case "DecreaseCardsNeededForStraightFlush":
					HandsInformation.instance.OrganizeByPlayableCards();
					HandsInformation.instance.UpdateAllHandInfos();
				break;
				case "IncreaseItemsForSale":
					if(!disabledBaubles.Contains("IncreaseItemsForSale"))
					{
						Shop.instance.AddItemToEachCategory();
					}
				break;
				case "AllCardsInShopAreRainbow":
					Shop.instance.SwitchAllCardsToRainbow();
				break;
				case "DecreaseRerollCost":
					if(!disabledBaubles.Contains("DecreaseRerollCost"))
					{
						Shop.instance.ChangeCurrentRerollCost(-1);
					}
				break;
			}
		}
	}
	
	public void ZodiacPurchased(string baubleTag, Vector2 origin, bool countAsGained = true, bool updateHandInfos = true)
	{
		if(countAsGained)
		{
			Stats.instance.ZodiacGained();
		}
		if(owned.ContainsKey(baubleTag))
		{
			owned[baubleTag]++;
		}
		else
		{
			owned.Add(baubleTag, 1);
		}
		PurchasedItems.instance.ZodiacPurchased(baubleTag, origin);
		if(updateHandInfos)
		{
			HandInfosChanged();
		}
	}
	
	public void HandInfosChanged()
	{
		HandsInformation.instance.UpdateAllHandInfos();
		HandsInformation.instance.handsLabel.StartExpandRetract();
	}
	
	public void NegativeZodiacGained(string baubleTag, Vector2 origin)
	{
		if(owned.ContainsKey(baubleTag))
		{
			owned[baubleTag]--;
		}
		else
		{
			owned.Add(baubleTag, -1);
		}
		PurchasedItems.instance.ZodiacPurchased(baubleTag, origin, true);
		HandInfosChanged();
	}
	
	//public void Notify(string baubleTag, float delay = 0, AudioClip audioClipToPlay = null, float volumeFactor = 1f)
	public void Notify(string baubleTag, float delay = 0, UnityAction soundFunction = null, int chipsToAdd = 0)
	{
		GameObject newBaubleNotificationGO = Instantiate(baubleNotificationPrefab, baubleNotificationParent);
		BaubleNotification newBaubleNotification = newBaubleNotificationGO.GetComponent<BaubleNotification>();
		// newBaubleNotification.StartNotification(baubleTag, delay, audioClipToPlay, volumeFactor);
		newBaubleNotification.StartNotification(baubleTag, delay, soundFunction, chipsToAdd);
	}
	
	public void HandleDisabledBaubles()
	{
		bool resizePlayArea = false;
		for(int i = 0; i < disabledBaubles.Count; i++)
		{
			BaublesInformation.instance.baubleIcons[disabledBaubles[i]].xObject.SetActive(false);
			if(disabledBaubles[i] == "IncreasePlayableStandardCardCount" || disabledBaubles[i] == "IncreasePlayableSpecialCardCount")
			{
				resizePlayArea = true;
			}
		}
		disabledBaubles.Clear();
		if(resizePlayArea)
		{
			PlayArea.instance.ResizePlayArea();
		}
		int numberOfBaublesDisabled = 0;
		if(GameManager.instance.IsBossTagActive("BaubleDisabled"))
		{
			numberOfBaublesDisabled++;
		}
		if(GameManager.instance.IsBossTagActive("TwoBaublesDisabled"))
		{
			numberOfBaublesDisabled += 2;
		}
		if(GameManager.instance.IsBossTagActive("ThreeBaublesDisabled"))
		{
			numberOfBaublesDisabled += 3;
		}
		if(GameManager.instance.IsBossTagActive("FourBaublesDisabled"))
		{
			numberOfBaublesDisabled += 4;
		}
		if(GameManager.instance.IsBossTagActive("FiveBaublesDisabled"))
		{
			numberOfBaublesDisabled += 5;
		}
		if(numberOfBaublesDisabled > 0)
		{
			resizePlayArea = false;
			// int[] baublesToDisable = LocalInterface.instance.GetRandomizedArrayOfInts(Mathf.Min(numberOfBaublesDisabled, owned.Count), Baubles.instance.owned.Count, 2);
			List<string> ownedBaublesTags = GetListOfOwnedBaublesTags();
			int[] baublesToDisable = LocalInterface.instance.GetRandomizedArrayOfInts(Mathf.Min(numberOfBaublesDisabled, ownedBaublesTags.Count), ownedBaublesTags.Count, 2);
			for(int i = 0; i < baublesToDisable.Length; i++)
			{
				//KeyValuePair<string, VariantSpecialCard> randomVariantSpecialCard = V.i.v.variantSpecialCards.ElementAt(RNG.instance.hands.Range(0, V.i.v.variantSpecialCards.Count));
				if(ownedBaublesTags.Count > i)
				{
					// KeyValuePair<string, int> ownedToDisable = owned.ElementAt(baublesToDisable[i]);
					// string baubleTagToDisable = ownedToDisable.Key;
					string baubleTagToDisable = ownedBaublesTags[baublesToDisable[i]];
					disabledBaubles.Add(baubleTagToDisable);
					BaublesInformation.instance.baubleIcons[baubleTagToDisable].xObject.SetActive(true);
					switch(baubleTagToDisable)
					{
						case "IncreasePlayableStandardCardCount":
						case "IncreasePlayableSpecialCardCount":
							resizePlayArea = true;
						break;
					}
				}
			}
			if(resizePlayArea)
			{
				PlayArea.instance.ResizePlayArea();
			}
		}
	}
	
	public List<string> GetListOfOwnedBaublesTags()
	{
		List<string> ownedBaublesTags = new List<string>();
		foreach(KeyValuePair<string, int> entry in owned)
		{
			if(V.i.v.variantBaubles[entry.Key].category != "Zodiac")
			{
				ownedBaublesTags.Add(entry.Key);
			}
		}
		return ownedBaublesTags;
	}
	
	public string ConvertOwnedBaublesToString()
	{
		string ownedBaublesString = string.Empty;
		foreach(KeyValuePair<string, int> entry in owned)
		{
			ownedBaublesString += $"{entry.Key}|{entry.Value}%";
		}
		if(ownedBaublesString.Length > 0)
		{
			return ownedBaublesString.Substring(0, ownedBaublesString.Length - 1);
		}
		return string.Empty;
	}
	
	public void ConvertStringToOwnedBaubles(string ownedBaublesString)
	{
		string[] ownedBaublesData = ownedBaublesString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < ownedBaublesData.Length; i++)
		{
			string[] ownedBaubleData = ownedBaublesData[i].Split('|', StringSplitOptions.RemoveEmptyEntries);
			// Debug.Log($"i={i}, ownedBaublesData[i]={ownedBaublesData[i]}");
			for(int j = 0; j < int.Parse(ownedBaubleData[1]); j++)
			{
				if(LocalInterface.BaubleNameIsZodiac(ownedBaubleData[0]))
				{
					ZodiacPurchased(ownedBaubleData[0], new Vector2(-400f, 0), false);
				}
				else
				{
					BaublePurchased(ownedBaubleData[0], new Vector2(-400f, 0), true, false, false);
				}
			}
			// owned.Add(ownedBaubleData[0], int.Parse(ownedBaubleData[1]));
		}
	}
	
	public string ConvertDisabledBaublesToString()
	{
		string disabledBaublesString = string.Empty;
		for(int i = 0; i < disabledBaubles.Count; i++)
		{
			disabledBaublesString += disabledBaubles[i];
			if(i < disabledBaubles.Count - 1)
			{
				disabledBaublesString += "%";
			}
		}
		return disabledBaublesString;
	}
	
	public void ConvertStringToDisabledBaubles(string disabledBaublesString)
	{
		string[] disabledBaublesData = disabledBaublesString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < disabledBaublesData.Length; i++)
		{
			disabledBaubles.Add(disabledBaublesData[i]);
			BaublesInformation.instance.baubleIcons[disabledBaublesData[i]].xObject.SetActive(true);
		}
	}
}
