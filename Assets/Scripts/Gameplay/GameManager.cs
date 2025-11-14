using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
	public Label discardsRemainingLabel;
	public Label handsUntilFatigueRemainingLabel;
	public Label handsUntilFatigueLabel;
	public Label fatiguedLabel;
	public Label currencyLabel;
	public RectTransform roundsInformationPanel;
	public RectTransform handsInformationPanel;
	public RectTransform baublesInformationPanel;
	public RectTransform chipsParent;
	public Vector2 chipDestination;
	public RectTransform particlesParent;
	public ButtonPlus menuButton;
	public RectTransform spareScorePlateParent;
	public RectTransform spareCardParent;
	public RectTransform spareChipsParent;
	public RectTransform spareChipParticlesParent;
	public GameObject scorePlatePrefab;
	public Canvas gameplayCanvas;
	public Canvas cheatCanvas;
	
	public GameObject cheatButtonBackdrop;
	
	public GameObject particlePrefab;
	public GameObject chipPrefab;
	
	public string[] handNames;
	public string[] handDescriptions;
	public float[] handPoints;
	public float[] handMults;
	public int[] handMinimumNumberOfCards;
	
	public int handsUntilFatigueRemaining;
    public int discardsRemaining;
	public int currency;
	public int currentRound;
	public Dictionary<int, BossRound> bossRounds = new Dictionary<int, BossRound>();
	
	public int bonusHandsUntilFatiguePerRound;	// from deck
	public int bonusDiscardsPerRound;
	public int bonusChipThresholdsPerRound;
	
	public int[] flushZodiacBaubleSuitOrders;
	
	public static GameManager instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public class BossRound
	{
		public string tag;
		public string description;
		public int random1;
		public int random2;
		public BossRound(string tag, string description, int random1 = -1, int random2 = -1)
		{
			this.tag = tag;
			this.description = description;
			this.random1 = random1;
			this.random2 = random2;
		}
	}
	
	public string ConvertBossRoundsToString()
	{
		string bossRoundsString = string.Empty;
		foreach(KeyValuePair<int, BossRound> entry in bossRounds)
		{
			bossRoundsString += $"{entry.Key}|{entry.Value.tag}|{entry.Value.description}|{entry.Value.random1}|{entry.Value.random2}%";
		}
		if(bossRoundsString.Length > 0)
		{
			return bossRoundsString.Substring(0, bossRoundsString.Length - 1);
		}
		return string.Empty;
	}
	
	public void LoadBossRoundsFromString(string bossRoundsString)
	{
		bossRounds.Clear();
		string[] bossRoundsSplit = bossRoundsString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < bossRoundsSplit.Length; i++)
		{
			string[] bossRoundData = bossRoundsSplit[i].Split('|');
			int round = int.Parse(bossRoundData[0]);
			string tag = bossRoundData[1];
			string description = bossRoundData[2];
			int random1 = int.Parse(bossRoundData[3]);
			int random2 = int.Parse(bossRoundData[4]);
			bossRounds.Add(round, new BossRound(tag, description, random1, random2));
		}
	}
	
	public void SetMenuButtonInteractability(bool enabledState)
	{
		menuButton.ChangeButtonEnabled(enabledState);
	}
	
	public void MenuButtonClicked()
	{
		Preferences.instance.OpenMenu();
	}
	
	public void DiscardUsed()
	{
		discardsRemaining--;
		discardsRemainingLabel.ChangeText(discardsRemaining.ToString());
		discardsRemainingLabel.StartExpandRetract();
	}
	
	public void SetDiscards(int dis)
	{
		discardsRemaining = dis;
		discardsRemainingLabel.ChangeText(discardsRemaining.ToString());
		discardsRemainingLabel.StartExpandRetract();
	}
	
	public void ResetDiscards()
	{
		discardsRemaining = V.i.v.variantSpecialOptions["StartingDiscards"].impact + Baubles.instance.GetImpactInt("IncreaseDiscards") + bonusDiscardsPerRound;
		if(IsBossTagActive("LessDiscardsAndHands"))
		{
			discardsRemaining--;
		}
		if(discardsRemaining < 0)
		{
			discardsRemaining = 0;
		}
		if(IsBossTagActive("NoDiscards"))
		{
			discardsRemaining = 0;
		}
		discardsRemainingLabel.ChangeText(discardsRemaining.ToString());
		discardsRemainingLabel.StartExpandRetract();
	}
	
	public int GetDiscardsPerRound()
	{
		return V.i.v.variantSpecialOptions["StartingDiscards"].impact + Baubles.instance.GetImpactInt("IncreaseDiscards") + bonusDiscardsPerRound;
	}
	
	public void HandUntilFatigueUsed()
	{
		handsUntilFatigueRemaining--;
		CheckForFatiguedStatus();
		if(IsPlayerFatigued())
		{
			fatiguedLabel.StartExpandRetract();
		}
		else
		{
			handsUntilFatigueRemainingLabel.StartExpandRetract();
		}
	}
	
	public void SetHandsUntilFatigue(int hof)
	{
		handsUntilFatigueRemaining = hof;
		CheckForFatiguedStatus();
		if(IsPlayerFatigued())
		{
			fatiguedLabel.StartExpandRetract();
		}
		else
		{
			handsUntilFatigueRemainingLabel.StartExpandRetract();
		}
	}
	
	public void ResetHandsUntilFatigue()
	{
		handsUntilFatigueRemaining = V.i.v.variantSpecialOptions["StartingHandsUntilFatigue"].impact + Baubles.instance.GetImpactInt("IncreaseHandsUntilFatigue") + bonusHandsUntilFatiguePerRound;
		if(IsBossTagActive("LessDiscardsAndHands"))
		{
			handsUntilFatigueRemaining--;
		}
		if(handsUntilFatigueRemaining < 0)
		{
			handsUntilFatigueRemaining = 0;
		}
		if(IsBossTagActive("Fatigued"))
		{
			handsUntilFatigueRemaining = 0;
		}
		CheckForFatiguedStatus();
		if(handsUntilFatigueRemaining > 0)
		{
			handsUntilFatigueRemainingLabel.StartExpandRetract();
		}
	}
	
	public int GetHandsUntilFatiguePerRound()
	{
		return V.i.v.variantSpecialOptions["StartingHandsUntilFatigue"].impact + Baubles.instance.GetImpactInt("IncreaseHandsUntilFatigue") + bonusHandsUntilFatiguePerRound;
	}
	
	public bool IsPlayerFatigued()
	{
		if(handsUntilFatigueRemaining <= 0)
		{
			return true;
		}
		return false;
	}
	
	public void CheckForFatiguedStatus()
	{
		if(handsUntilFatigueRemaining <= 0)
		{
			handsUntilFatigueLabel.gameObject.SetActive(false);
			handsUntilFatigueRemainingLabel.gameObject.SetActive(false);
			fatiguedLabel.gameObject.SetActive(true);
		}
		else
		{
			handsUntilFatigueLabel.gameObject.SetActive(true);
			handsUntilFatigueRemainingLabel.gameObject.SetActive(true);
			fatiguedLabel.gameObject.SetActive(false);
			handsUntilFatigueRemainingLabel.ChangeText(handsUntilFatigueRemaining.ToString());
		}
	}
	
	public void AddCurrency(int amount = 1)
	{
		currency += amount;
		if(amount > 0)
		{
			RunInformation.instance.chipsEarned += amount;
			Stats.instance.CurrencyEarned(amount);
		}
		currencyLabel.ChangeText(currency.ToString());
		currencyLabel.StartExpandRetract();
		Shop.instance.CurrencyUpdated();
	}
	
	public void SetCurrency(int amount)
	{
		currency = amount;
		currencyLabel.ChangeText(currency.ToString());
		currencyLabel.StartExpandRetract();
		if(Shop.instance.shopFinishedOpening)
		{
			Shop.instance.CurrencyUpdated();
		}
	}
	
	public void SetCurrentRound(int round)
	{
		bool lastRoundWasBossRound = false;
		if(IsThisABossRound() && currentRound >= 0)
		{
			lastRoundWasBossRound = true;
		}
		currentRound = round;
		if(bossRounds.ContainsKey(round - 1))
		{
			lastRoundWasBossRound = true;
		}
		PerformNewRoundMaintenance(lastRoundWasBossRound);
	}
	
	public void LoadGameToRound(int round)
	{
		currentRound = round;
		RoundsInformation.instance.UpdateRoundsInformation(currentRound >= 29 ? true : false);
	}
	
	public void AdvanceRound()
	{
		bool lastRoundWasBossRound = false;
		if(IsThisABossRound())
		{
			lastRoundWasBossRound = true;
		}
		currentRound++;
		PerformNewRoundMaintenance(lastRoundWasBossRound);
	}
	
	public void PerformNewRoundMaintenance(bool lastRoundWasBossRound)
	{
		RoundsInformation.instance.UpdateRoundsInformation(currentRound >= 29 ? true : false);
		CheckForBackgroundChange(lastRoundWasBossRound);
		if(IsBossTagActive("SmallerPlayArea"))
		{
			PlayArea.instance.ResizePlayArea();
		}
		Baubles.instance.HandleDisabledBaubles();
		ResetDiscards();
		ResetHandsUntilFatigue();
		BossInformation.instance.UpdateBossInformation();
		Shop.instance.HandleDisabledBaubles();
		HandsInformation.instance.OrganizeByPlayableCards();
		HandsInformation.instance.UpdateAllHandInfos();
	}
	
	public void CheckForBackgroundChange(bool lastRoundWasBossRound)
	{
		bool thisIsABossRound = IsThisABossRound();
		if(thisIsABossRound != lastRoundWasBossRound)
		{
			if(lastRoundWasBossRound)
			{
				BackgroundManager.instance.SwitchToClassicBackground();
			}
			else
			{
				BackgroundManager.instance.SwitchToBossBackground();
			}
		}
	}
	
	public bool IsThisABossRound()
	{
		if(!bossRounds.ContainsKey(currentRound))
		{
			return false;
		}
		return true;
	}
	
	public BossRound GetCurrentBossRound()
	{
		if(!bossRounds.ContainsKey(currentRound))
		{
			return null;
		}
		return bossRounds[currentRound];
	}
	
	public int GetCurrentBossRoundRandom1()
	{
		if(!bossRounds.ContainsKey(currentRound))
		{
			return -1;
		}
		return bossRounds[currentRound].random1;
	}
	
	public int GetCurrentBossRoundRandom2()
	{
		if(!bossRounds.ContainsKey(currentRound))
		{
			return -1;
		}
		return bossRounds[currentRound].random2;
	}
	
	public bool IsBossTagActive(string bossTag)
	{
		if(!bossRounds.ContainsKey(currentRound))
		{
			return false;
		}
		if(bossRounds[currentRound].tag == bossTag)
		{
			return true;
		}
		return false;
	}
	
	public int GetMaxPlayableStandardCards(bool ignoreBoss = false)
	{
		int playableStandardCards = 5 + Baubles.instance.GetImpactInt("IncreasePlayableStandardCardCount");
		if(IsBossTagActive("SmallerPlayArea") && !ignoreBoss)
		{
			playableStandardCards--;
		}
		return playableStandardCards;
	}
	
	public int GetNumberOfSpecialCardOnlyDropZones()
	{
		int specialCardDropZones = Baubles.instance.GetImpactInt("IncreasePlayableSpecialCardCount");
		return specialCardDropZones;
	}
	
	public int GetMaxHandSize()
	{
		int handSize = V.i.v.variantSpecialOptions["StartingHandSize"].impact + Baubles.instance.GetImpactInt("IncreaseHandSize");
		if(IsBossTagActive("SmallerHand"))
		{
			handSize--;
		}
		return handSize;
	}
	
	public int GetMaxStraightGap()
	{
		return Baubles.instance.GetImpactInt("IncreaseMaxGapInStraights");
	}
	
	public int GetChipThresholdsPerRound()
	{
		return V.i.v.variantSpecialOptions["ChipThresholdsPerRound"].impact + bonusChipThresholdsPerRound + Baubles.instance.GetImpactInt("IncreaseChipThresholds");
	}
	
	public double GetCurrentRoundScoreThreshold()
	{
		double currentRoundThreshold = V.i.v.variantRounds[currentRound].scoreNeeded;
		if(IsBossTagActive("PointsRequiredDoubled"))
		{
			currentRoundThreshold = currentRoundThreshold * 2;
		}
		if(IsBossTagActive("PointsRequiredTripled"))
		{
			currentRoundThreshold = currentRoundThreshold * 3;
		}
		return currentRoundThreshold;
	}
	
	public void SetVisibilityOfCheatOptions(bool visibilityState)
	{
		cheatCanvas.gameObject.SetActive(visibilityState);
		cheatButtonBackdrop.SetActive(visibilityState);
	}
	
	public void SetupDeckBonuses()
	{
		bonusHandsUntilFatiguePerRound = 0;
		bonusDiscardsPerRound = 0;
		bonusChipThresholdsPerRound = 0;
		if(!V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
		{
			switch(V.i.chosenDeck)
			{
				case "Swirly":
					bonusHandsUntilFatiguePerRound = 1;
					bonusDiscardsPerRound = 1;
				break;
				case "Sleepy":
					bonusHandsUntilFatiguePerRound = -30;
					bonusDiscardsPerRound = 4;
				break;
				case "Sgambler":
					bonusChipThresholdsPerRound = 1;
				break;
			}
		}
	}
}
