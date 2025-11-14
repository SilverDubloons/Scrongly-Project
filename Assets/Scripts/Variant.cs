using UnityEngine;
using System.Collections.Generic;
using System;

public class Variant
{
	public string variantName;
	public string variantDescription;
	public Sprite variantSprite;
	public Color variantSpriteColor;
	public string variantSpriteCategory;
	public int variantSpriteIndex;
	public List<VariantCard> startingDeck = new List<VariantCard>();
	public int numberOfRandomStandardCardsToAddToDeck;
	public bool includeRainbowInRandomCards;
	public int numberOfRandomSpecialCardsToAddToDeck;
	public bool considerRarity;
	public Dictionary<string, VariantSpecialOption> variantSpecialOptions = new Dictionary<string, VariantSpecialOption>();
	public Dictionary<string, VariantBauble> variantBaubles = new Dictionary<string, VariantBauble>();
	public Dictionary<int, VariantRound> variantRounds = new Dictionary<int, VariantRound>();
	public Dictionary<string, VariantSpecialCard> variantSpecialCards = new Dictionary<string, VariantSpecialCard>();
	
    public struct VariantSpecialOption
	{
		public string tag;
		public string label;
		public string description;
		public string type;
		public Vector2Int range;
		public bool inEffect;
		public int impact;
		public VariantSpecialOption(string tag, string label, string description, string type, Vector2Int range, bool inEffect, int impact)
		{
			this.tag = tag;
			this.label = label;
			this.description = description;
			this.type = type;
			this.range = range;
			this.inEffect = inEffect;
			this.impact = impact;
		}
		public string ToCommaSeparatedString()
		{
			return $"{tag},{label},{description},{type},{range},{inEffect},{impact}";
		}
		public string ToTextString()
		{
			return $"{tag}#{inEffect}#{impact}#{description}";
		}
		public VariantSpecialOption(string[] textParts)
		{
			this.tag = textParts[0];
			this.inEffect = bool.Parse(textParts[1]);
			this.impact = int.Parse(textParts[2]);
			this.label = LocalInterface.instance.baseVariant.variantSpecialOptions[this.tag].label;
			this.description = LocalInterface.instance.baseVariant.variantSpecialOptions[this.tag].description;
			this.type = LocalInterface.instance.baseVariant.variantSpecialOptions[this.tag].type;
			this.range = LocalInterface.instance.baseVariant.variantSpecialOptions[this.tag].range;
		}
		public VariantSpecialOption(VariantSpecialOption variantSpecialOptionToCopy)
		{
			this.tag = variantSpecialOptionToCopy.tag;
			this.label = variantSpecialOptionToCopy.label;
			this.description = variantSpecialOptionToCopy.description;
			this.type = variantSpecialOptionToCopy.type;
			this.range = variantSpecialOptionToCopy.range;
			this.inEffect = variantSpecialOptionToCopy.inEffect;
			this.impact = variantSpecialOptionToCopy.impact;
		}
	}
	
	public struct VariantBauble
	{
		public string tag;
		public string baubleName;
		public string menuDescription;
		public string inGameDescription;
		public int max;
		public int baseCost;
		public int costStep;
		public double impact1;
		public double impact2;
		public string category;
		public Sprite sprite;
		public bool availableInStore;
		public bool mustBeUnlocked;
		public int startingQuantity;
		public string howToUnlock;
		public string[] extraDescriptions;
		public VariantBauble(string tag, string baubleName, string menuDescription, string inGameDescription, int max, int baseCost, int costStep, double impact1, double impact2, string category, Sprite sprite, bool availableInStore, bool mustBeUnlocked, int startingQuantity, string howToUnlock, string[] extraDescriptions)
		{
			this.tag = tag;
			this.baubleName = baubleName;
			this.menuDescription = menuDescription;
			this.inGameDescription = inGameDescription;
			this.max = max;
			this.baseCost = baseCost;
			this.costStep = costStep;
			this.impact1 = impact1;
			this.impact2 = impact2;
			this.category = category;
			this.sprite = sprite;
			this.availableInStore = availableInStore;
			this.mustBeUnlocked = mustBeUnlocked;
			this.startingQuantity = startingQuantity;
			this.howToUnlock = howToUnlock;
			this.extraDescriptions = extraDescriptions;
		}
		public string ToCommaSeparatedString()
		{
			return $"{tag},{baubleName},{menuDescription},{inGameDescription},{max},{baseCost},{costStep},{impact1},{impact2},{category},{sprite},{availableInStore},{startingQuantity}";
		}
		public string ToTextString()
		{
			return $"{tag}#{baseCost}#{costStep}#{category}#{availableInStore}#{startingQuantity}#{impact1}#{impact2}";
		}
		public VariantBauble(string[] textParts)
		{
			this.tag = textParts[0];
			this.baseCost = int.Parse(textParts[1]);
			this.costStep = int.Parse(textParts[2]);
			this.category = textParts[3];
			this.availableInStore = bool.Parse(textParts[4]);
			this.startingQuantity = int.Parse(textParts[5]);
			if(textParts.Length > 6)
			{
				this.impact1 = double.Parse(textParts[6]);
				this.impact2 = double.Parse(textParts[7]);
			}
			else
			{
				this.impact1 = LocalInterface.instance.baseVariant.variantBaubles[this.tag].impact1;
				this.impact2 = LocalInterface.instance.baseVariant.variantBaubles[this.tag].impact2;
			}
			this.baubleName = LocalInterface.instance.baseVariant.variantBaubles[this.tag].baubleName;
			this.menuDescription = LocalInterface.instance.baseVariant.variantBaubles[this.tag].menuDescription;
			this.inGameDescription = LocalInterface.instance.baseVariant.variantBaubles[this.tag].inGameDescription;
			this.max = LocalInterface.instance.baseVariant.variantBaubles[this.tag].max;
			
			this.sprite = LocalInterface.instance.baseVariant.variantBaubles[this.tag].sprite;
			this.mustBeUnlocked = LocalInterface.instance.baseVariant.variantBaubles[this.tag].mustBeUnlocked;
			this.howToUnlock = LocalInterface.instance.baseVariant.variantBaubles[this.tag].howToUnlock;
			this.extraDescriptions = LocalInterface.instance.baseVariant.variantBaubles[this.tag].extraDescriptions;
		}
		public VariantBauble(VariantBauble baubleToCopy)
		{
			this.tag = baubleToCopy.tag;
			this.baubleName = baubleToCopy.baubleName;
			this.menuDescription = baubleToCopy.menuDescription;
			this.inGameDescription = baubleToCopy.inGameDescription;
			this.max = baubleToCopy.max;
			this.baseCost = baubleToCopy.baseCost;
			this.costStep = baubleToCopy.costStep;
			this.impact1 = baubleToCopy.impact1;
			this.impact2 = baubleToCopy.impact2;
			this.category = baubleToCopy.category;
			this.sprite = baubleToCopy.sprite;
			this.availableInStore = baubleToCopy.availableInStore;
			this.mustBeUnlocked = baubleToCopy.mustBeUnlocked;
			this.startingQuantity = baubleToCopy.startingQuantity;
			this.howToUnlock = baubleToCopy.howToUnlock;
			this.extraDescriptions = baubleToCopy.extraDescriptions;
		}
	}
	
	public struct VariantCard
	{
		public int rank;
		public int suit;
		public bool isSpecial;
		public string specialCardTag;
		public VariantCard(int rank, int suit, bool isSpecial = false, string specialCardTag = "")
		{
			this.rank = rank;
			this.suit = suit;
			this.isSpecial = isSpecial;
			this.specialCardTag = specialCardTag;
		}
		public string ToCardString()
		{
			if(isSpecial)
			{
				return specialCardTag;
			}
			else
			{
				return LocalInterface.instance.ConvertRankAndSuitToString(rank, suit);
			}
		}
	}
	
	public struct VariantSpecialCard
	{
		public string tag;
		public string specialCardName;
		public string description;
		public string category;
		public Sprite sprite;
		public Sprite playedSprite;
		public int cost;
		public double impact;
		public bool inShop;
		public bool mustBeUnlocked;
		public string howToUnlock;
		public VariantSpecialCard(string tag, string specialCardName, string description, string category, Sprite sprite, Sprite playedSprite, int cost, double impact, bool inShop, bool mustBeUnlocked, string howToUnlock)
		{
			this.tag = tag;
			this.specialCardName = specialCardName;
			this.description = description;
			this.category = category;
			this.sprite = sprite;
			this.playedSprite = playedSprite;
			this.cost = cost;
			this.impact = impact;
			this.inShop = inShop;
			this.mustBeUnlocked = mustBeUnlocked;
			this.howToUnlock = howToUnlock;
		}
		public string ToCommaSeparatedString()
		{
			return $"{tag},{specialCardName},{description},{category},{sprite},{playedSprite},{cost},{impact},{inShop}";
		}
		public string ToTextString()
		{
			return $"{tag}#{category}#{cost}#{impact}#{inShop}";
		}
		public VariantSpecialCard(string[] textParts)
		{
			this.tag = textParts[0];
			this.category = textParts[1];
			this.cost = int.Parse(textParts[2]);
			this.impact = double.Parse(textParts[3]);
			this.inShop = bool.Parse(textParts[4]);
			this.specialCardName = LocalInterface.instance.baseVariant.variantSpecialCards[this.tag].specialCardName;
			this.description = LocalInterface.instance.baseVariant.variantSpecialCards[this.tag].description;
			this.sprite = LocalInterface.instance.baseVariant.variantSpecialCards[this.tag].sprite;
			this.playedSprite = LocalInterface.instance.baseVariant.variantSpecialCards[this.tag].playedSprite;
			this.mustBeUnlocked = LocalInterface.instance.baseVariant.variantSpecialCards[this.tag].mustBeUnlocked;
			this.howToUnlock = LocalInterface.instance.baseVariant.variantSpecialCards[this.tag].howToUnlock;
		}
		public VariantSpecialCard(VariantSpecialCard specialCardToCopy)
		{
			this.tag = specialCardToCopy.tag;
			this.specialCardName = specialCardToCopy.specialCardName;
			this.description = specialCardToCopy.description;
			this.category = specialCardToCopy.category;
			this.sprite = specialCardToCopy.sprite;
			this.playedSprite = specialCardToCopy.playedSprite;
			this.cost = specialCardToCopy.cost;
			this.impact = specialCardToCopy.impact;
			this.inShop = specialCardToCopy.inShop;
			this.mustBeUnlocked = specialCardToCopy.mustBeUnlocked;
			this.howToUnlock = specialCardToCopy.howToUnlock;
		}
	}
	
	public struct VariantRound
	{
		public int roundNumber; // 0 - 49
		public double scoreNeeded;
		public string bossType; // "" = no boss, "RandomTier00", "RandomTier01", "DestroyAllPlayedCards", etc
		public VariantRound(int roundNumber, double scoreNeeded, string bossType)
		{
			this.roundNumber = roundNumber;
			this.scoreNeeded = scoreNeeded;
			this.bossType = bossType;
		}
		public string ToCommaSeparatedString()
		{
			return $"{roundNumber},{scoreNeeded},{bossType}";
		}
		public string ToTextString()
		{
			return $"{roundNumber}#{scoreNeeded}#{bossType}";
		}
		public VariantRound(string[] textParts)
		{
			this.roundNumber = int.Parse(textParts[0]);
			this.scoreNeeded = double.Parse(textParts[1]);
			this.bossType = textParts[2];
		}
		public VariantRound(VariantRound roundToCopy)
		{
			this.roundNumber = roundToCopy.roundNumber;
			this.scoreNeeded = roundToCopy.scoreNeeded;
			this.bossType = roundToCopy.bossType;
		}
	}
	
	public Variant(string variantName, string variantDescription, List<VariantCard> startingDeck, int numberOfRandomStandardCardsToAddToDeck, bool includeRainbowInRandomCards, int numberOfRandomSpecialCardsToAddToDeck, bool considerRarity, Dictionary<string, VariantSpecialOption> variantSpecialOptions, Dictionary<string, VariantBauble> variantBaubles, Dictionary<int, VariantRound> variantRounds, Dictionary<string, VariantSpecialCard> variantSpecialCards, string variantSpriteCategory, int variantSpriteIndex, Color variantSpriteColor)
	{
		this.variantName = variantName;
		this.variantDescription = variantDescription;
		this.startingDeck = startingDeck;
		this.numberOfRandomStandardCardsToAddToDeck = numberOfRandomStandardCardsToAddToDeck;
		this.includeRainbowInRandomCards = includeRainbowInRandomCards;
		this.numberOfRandomSpecialCardsToAddToDeck = numberOfRandomSpecialCardsToAddToDeck;
		this.considerRarity = considerRarity;
		this.variantSpecialOptions = variantSpecialOptions;
		this.variantBaubles = variantBaubles;
		this.variantRounds = variantRounds;
		this.variantSpecialCards = variantSpecialCards;
		this.variantSpriteCategory = variantSpriteCategory;
		this.variantSpriteIndex = variantSpriteIndex;
		this.variantSpriteColor = variantSpriteColor;
	}
	
	public Variant(Variant variantToCopy)
	{
		this.variantName = variantToCopy.variantName;
		this.variantDescription = variantToCopy.variantDescription;
		for(int i = 0; i < variantToCopy.startingDeck.Count; i++)
		{
			startingDeck.Add(new VariantCard(variantToCopy.startingDeck[i].rank, variantToCopy.startingDeck[i].suit, variantToCopy.startingDeck[i].isSpecial, variantToCopy.startingDeck[i].specialCardTag));
		}
		this.numberOfRandomStandardCardsToAddToDeck = variantToCopy.numberOfRandomStandardCardsToAddToDeck;
		this.includeRainbowInRandomCards = variantToCopy.includeRainbowInRandomCards;
		this.numberOfRandomSpecialCardsToAddToDeck = variantToCopy.numberOfRandomSpecialCardsToAddToDeck;
		this.considerRarity = variantToCopy.considerRarity;
		foreach(KeyValuePair<string, VariantSpecialOption> entry in variantToCopy.variantSpecialOptions)
		{
			variantSpecialOptions.Add(entry.Key, new VariantSpecialOption(entry.Value.tag, entry.Value.label, entry.Value.description, entry.Value.type, entry.Value.range, entry.Value.inEffect, entry.Value.impact));
		}
		foreach(KeyValuePair<string, VariantBauble> entry in variantToCopy.variantBaubles)
		{
			variantBaubles.Add(entry.Key, new VariantBauble(entry.Value.tag, entry.Value.baubleName, entry.Value.menuDescription, entry.Value.inGameDescription, entry.Value.max, entry.Value.baseCost, entry.Value.costStep, entry.Value.impact1, entry.Value.impact2, entry.Value.category, entry.Value.sprite, entry.Value.availableInStore, entry.Value.mustBeUnlocked, entry.Value.startingQuantity, entry.Value.howToUnlock, entry.Value.extraDescriptions));
		}
		foreach(KeyValuePair<int, VariantRound> entry in variantToCopy.variantRounds)
		{
			variantRounds.Add(entry.Key, new VariantRound(entry.Value.roundNumber, entry.Value.scoreNeeded, entry.Value.bossType));
		}
		foreach(KeyValuePair<string, VariantSpecialCard> entry in variantToCopy.variantSpecialCards)
		{
			variantSpecialCards.Add(entry.Key, new VariantSpecialCard(entry.Value.tag, entry.Value.specialCardName, entry.Value.description, entry.Value.category, entry.Value.sprite, entry.Value.playedSprite, entry.Value.cost, entry.Value.impact, entry.Value.inShop, entry.Value.mustBeUnlocked, entry.Value.howToUnlock));
		}
		this.variantSpriteCategory = variantToCopy.variantSpriteCategory;
		this.variantSpriteIndex = variantToCopy.variantSpriteIndex;
		this.variantSpriteColor = variantToCopy.variantSpriteColor;
	}
	
	public string GetPrintedVariantString()
	{
		string printedVariant = $"variantName={variantName}\nvariantDescription={variantName}\n\n***Baubles***\n";
		foreach(KeyValuePair<string, VariantBauble> entry in variantBaubles)
		{
			printedVariant += $"{entry.Value.ToCommaSeparatedString()}\n";
		}
		printedVariant += "\n***SpecialOptions***\n";
		foreach(KeyValuePair<string, VariantSpecialOption> entry in variantSpecialOptions)
		{
			printedVariant += $"{entry.Value.ToCommaSeparatedString()}\n";
		}
		printedVariant += $"\n***StartingDeck***\nnumberOfRandomStandardCardsToAddToDeck={numberOfRandomStandardCardsToAddToDeck},includeRainbowInRandomCards={includeRainbowInRandomCards},numberOfRandomSpecialCardsToAddToDeck={numberOfRandomSpecialCardsToAddToDeck},considerRarity={considerRarity}\n";
		for(int i = 0; i < startingDeck.Count; i++)
		{
			printedVariant += $"{startingDeck[i].ToCardString()},";
		}
		printedVariant += "\n***VariantRounds***\n";
		foreach(KeyValuePair<int, VariantRound> entry in variantRounds)
		{
			printedVariant += $"{entry.Value.ToCommaSeparatedString()}\n";
		}
		printedVariant += "\n***VariantSpecialCards***\n";
		foreach(KeyValuePair<string, VariantSpecialCard> entry in variantSpecialCards)
		{
			printedVariant += $"{entry.Value.ToCommaSeparatedString()}\n";
		}
		return printedVariant.Trim();
	}
	
	/*
	 0 = variantFileVersion
	 1 = variantName
	 2 = variantDescription
	 3 = variantSpriteColor
	 4 = variantSpriteCategory
	 5 = variantSpriteIndex
	 6 = numberOfRandomStandardCardsToAddToDeck
	 7 = includeRainbowInRandomCards
	 8 = numberOfRandomSpecialCardsToAddToDeck
	 9 = considerRarity
	*/
	public string ConvertToText()
	{
		string variantText = $"basicInfo={LocalInterface.instance.variantFileVersion}%{variantName}%{variantDescription}%{variantSpriteColor}%{variantSpriteCategory}%{variantSpriteIndex}%{numberOfRandomStandardCardsToAddToDeck}%{includeRainbowInRandomCards}%{numberOfRandomSpecialCardsToAddToDeck}%{considerRarity}\nstartingDeck=";
		bool addedLatestElement = false;
		for(int i = 0; i < startingDeck.Count; i++)
		{
			variantText += startingDeck[i].ToCardString() + "%";
			addedLatestElement = true;
		}
		if(addedLatestElement)
		{
			variantText = variantText.Substring(0, variantText.Length - 1);
			addedLatestElement = false;
		}
		variantText += "\nvariantSpecialOptions=";
		foreach(KeyValuePair<string, VariantSpecialOption> entry in variantSpecialOptions)
		{
			if(entry.Value.inEffect != LocalInterface.instance.baseVariant.variantSpecialOptions[entry.Key].inEffect || entry.Value.impact != LocalInterface.instance.baseVariant.variantSpecialOptions[entry.Key].impact)
			{
				variantText += entry.Value.ToTextString() + "%";
				addedLatestElement = true;
			}
		}
		if(addedLatestElement)
		{
			variantText = variantText.Substring(0, variantText.Length - 1);
			addedLatestElement = false;
		}
		variantText += "\nvariantBaubles=";
		foreach(KeyValuePair<string, VariantBauble> entry in variantBaubles)
		{
			if(entry.Value.baseCost != LocalInterface.instance.baseVariant.variantBaubles[entry.Key].baseCost || entry.Value.costStep != LocalInterface.instance.baseVariant.variantBaubles[entry.Key].costStep || entry.Value.category != LocalInterface.instance.baseVariant.variantBaubles[entry.Key].category || entry.Value.availableInStore != LocalInterface.instance.baseVariant.variantBaubles[entry.Key].availableInStore || entry.Value.startingQuantity != LocalInterface.instance.baseVariant.variantBaubles[entry.Key].startingQuantity || Math.Abs(entry.Value.impact1 - LocalInterface.instance.baseVariant.variantBaubles[entry.Key].impact1) > LocalInterface.instance.epsilon || Math.Abs(entry.Value.impact2 - LocalInterface.instance.baseVariant.variantBaubles[entry.Key].impact2) > LocalInterface.instance.epsilon)
			{
				variantText += entry.Value.ToTextString() + "%";
				addedLatestElement = true;
			}
		}
		if(addedLatestElement)
		{
			variantText = variantText.Substring(0, variantText.Length - 1);
			addedLatestElement = false;
		}
		variantText += "\nvariantRounds=";
		foreach(KeyValuePair<int, VariantRound> entry in variantRounds)
		{
			// if(entry.Value.roundNumber != LocalInterface.instance.baseVariant.variantRounds[entry.Key].roundNumber || Math.Abs(entry.Value.scoreNeeded - LocalInterface.instance.baseVariant.variantRounds[entry.Key].scoreNeeded) > LocalInterface.instance.minimumRoundThresholdDifference || entry.Value.bossType != LocalInterface.instance.baseVariant.variantRounds[entry.Key].bossType)
			if(entry.Value.roundNumber != LocalInterface.instance.baseVariant.variantRounds[entry.Key].roundNumber || !LocalInterface.LogApproximatelyEqual(entry.Value.scoreNeeded, LocalInterface.instance.baseVariant.variantRounds[entry.Key].scoreNeeded) || entry.Value.bossType != LocalInterface.instance.baseVariant.variantRounds[entry.Key].bossType)
			{
				variantText += entry.Value.ToTextString() + "%";
				addedLatestElement = true;
			}
		}
		if(addedLatestElement)
		{
			variantText = variantText.Substring(0, variantText.Length - 1);
			addedLatestElement = false;
		}
		variantText += "\nvariantSpecialCards=";
		foreach(KeyValuePair<string, VariantSpecialCard> entry in variantSpecialCards)
		{
			if(entry.Value.category != LocalInterface.instance.baseVariant.variantSpecialCards[entry.Key].category || entry.Value.cost != LocalInterface.instance.baseVariant.variantSpecialCards[entry.Key].cost || Math.Abs(entry.Value.impact - LocalInterface.instance.baseVariant.variantSpecialCards[entry.Key].impact) > LocalInterface.instance.epsilon || entry.Value.inShop != LocalInterface.instance.baseVariant.variantSpecialCards[entry.Key].inShop)
			{
				variantText += entry.Value.ToTextString() + "%";
				addedLatestElement = true;
			}
		}
		if(addedLatestElement)
		{
			variantText = variantText.Substring(0, variantText.Length - 1);
			addedLatestElement = false;
		}
		return variantText;
	}
	
	public Variant(string variantText)
	{
		string[] variantLines = variantText.Split('\n');
		string[] basicData = variantLines[0].Replace("basicInfo=", string.Empty).Split('%');
		/* for(int i = 0; i < basicData.Length; i++)
		{
			Debug.Log($"basicData[{i}]={basicData[i]}");
		} */
		string variantVersion = basicData[0];
		if(variantVersion != LocalInterface.instance.variantFileVersion)
		{
			LocalInterface.instance.DisplayError($"Unsupported variant file version mismatch. Current version={LocalInterface.instance.variantFileVersion}, this files version={variantVersion}");
			return;
		}
		variantName = basicData[1];
		variantDescription = basicData[2];
		variantSpriteColor = LocalInterface.instance.ParseColor(basicData[3]);
		variantSpriteCategory = basicData[4];
		variantSpriteIndex = int.Parse(basicData[5]);
		numberOfRandomStandardCardsToAddToDeck = int.Parse(basicData[6]);
		includeRainbowInRandomCards = bool.Parse(basicData[7]);
		numberOfRandomSpecialCardsToAddToDeck = int.Parse(basicData[8]);
		considerRarity = bool.Parse(basicData[9]);
		string[] startingDeckData = variantLines[1].Replace("startingDeck=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < startingDeckData.Length; i++)
		{
			if(startingDeckData[i].Length == 2)
			{
				startingDeck.Add(new VariantCard(LocalInterface.instance.ConvertRankCharToInt(startingDeckData[i][0]), LocalInterface.instance.ConvertSuitCharToInt(startingDeckData[i][1])));
			}
			else
			{
				startingDeck.Add(new VariantCard(-1, -1, true, startingDeckData[i]));
			}
		}
		string[] specialOptionsData = variantLines[2].Replace("variantSpecialOptions=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		Dictionary<string, VariantSpecialOption> specialOptionsDataDictionary = new Dictionary<string, VariantSpecialOption>();
		for(int i = 0; i < specialOptionsData.Length; i++)
		{
			string[] specialOptionData = specialOptionsData[i].Split('#');
			specialOptionsDataDictionary.Add(specialOptionData[0], new VariantSpecialOption(specialOptionData));
		}
		foreach(KeyValuePair<string, VariantSpecialOption> entry in LocalInterface.instance.baseVariant.variantSpecialOptions)
		{
			if(!specialOptionsDataDictionary.ContainsKey(entry.Key))
			{
				specialOptionsDataDictionary.Add(entry.Key, new VariantSpecialOption(entry.Value));
			}
		}
		this.variantSpecialOptions = specialOptionsDataDictionary;
		string[] baublesData = variantLines[3].Replace("variantBaubles=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		Dictionary<string, VariantBauble> baublesDataDictionary = new Dictionary<string, VariantBauble>();
		for(int i = 0; i < baublesData.Length; i++)
		{
			// Debug.Log($"baublesData[{i}]={baublesData[i]}");
			string[] baubleData = baublesData[i].Split('#');
			baublesDataDictionary.Add(baubleData[0], new VariantBauble(baubleData));
		}
		foreach(KeyValuePair<string, VariantBauble> entry in LocalInterface.instance.baseVariant.variantBaubles)
		{
			if(!baublesDataDictionary.ContainsKey(entry.Key))
			{
				baublesDataDictionary.Add(entry.Key, new VariantBauble(entry.Value));
			}
		}
		this.variantBaubles = baublesDataDictionary;
		string[] roundsData = variantLines[4].Replace("variantRounds=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		Dictionary<int, VariantRound> roundsDataDictionary = new Dictionary<int, VariantRound>();
		for(int i = 0; i < roundsData.Length; i++)
		{
			string[] roundData = roundsData[i].Split('#');
			roundsDataDictionary.Add(int.Parse(roundData[0]), new VariantRound(roundData));
		}
		foreach(KeyValuePair<int, VariantRound> entry in LocalInterface.instance.baseVariant.variantRounds)
		{
			if(!roundsDataDictionary.ContainsKey(entry.Key))
			{
				roundsDataDictionary.Add(entry.Key, new VariantRound(entry.Value));
			}
		}
		this.variantRounds = roundsDataDictionary;
		string[] specialCardsData = variantLines[5].Replace("variantSpecialCards=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		Dictionary<string, VariantSpecialCard> specialCardsDataDictionary = new Dictionary<string, VariantSpecialCard>();
		for(int i = 0; i < specialCardsData.Length; i++)
		{
			string[] specialCardData = specialCardsData[i].Split('#');
			specialCardsDataDictionary.Add(specialCardData[0], new VariantSpecialCard(specialCardData));
		}
		foreach(KeyValuePair<string, VariantSpecialCard> entry in LocalInterface.instance.baseVariant.variantSpecialCards)
		{
			if(!specialCardsDataDictionary.ContainsKey(entry.Key))
			{
				specialCardsDataDictionary.Add(entry.Key, new VariantSpecialCard(entry.Value));
			}
		}
		this.variantSpecialCards = specialCardsDataDictionary;
	}
}