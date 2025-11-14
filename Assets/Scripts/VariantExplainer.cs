using UnityEngine;
using System.Collections.Generic;
using System;
using static Deck;

public class VariantExplainer : MonoBehaviour
{
	public RectTransform rt;
	public GameObject visibilityObject;
    public Label titleLabel;
    public Label descriptionLabel;
    public Label specialOptionsLabel;
    public Label roundsLabel;
	public RectTransform content;
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public GameObject cardExplainerPrefab;
	public GameObject baubleExplainerPrefab;
	public GameObject specialCardExplainerPrefab;
	
	public List<CardExplainer> cardExplainers = new List<CardExplainer>();
	public List<BaubleExplainer> baubleExplainers = new List<BaubleExplainer>();
	public List<SpecialCardExplainer> specialCardExplainers = new List<SpecialCardExplainer>();
	
	public const int itemsWide = 5;
	
	public static VariantExplainer instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void DisplayVariant(string variantString)
	{
		// Debug.Log(variantString);
		visibilityObject.SetActive(true);
		string[] variantLines = variantString.Split('\n');
		string[] variantBasics = variantLines[0].Replace("basicInfo=", string.Empty).Split('%');
		titleLabel.ChangeText(variantBasics[1]);
		descriptionLabel.ChangeText(variantBasics[2]);
		
		string specialOptionsString = string.Empty;
		// Debug.Log($"variantBasics[6]={variantBasics[6]}, variantBasics[8]={variantBasics[8]}");
		if(variantBasics[6] == "" || int.Parse(variantBasics[6]) != 0)
		{
			specialOptionsString += $"Add {variantBasics[6]} random standard cards to deck";
			if(bool.Parse(variantBasics[7]))
			{
				specialOptionsString += ". They can be rainbow";
			}
			specialOptionsString += "\n";
		}
		if(variantBasics[8] == "" || int.Parse(variantBasics[8]) != 0)
		{
			specialOptionsString += $"Add {variantBasics[8]} random special cards to deck";
			if(bool.Parse(variantBasics[9]))
			{
				specialOptionsString += ", considering rarity";
			}
			specialOptionsString += "\n";
		}
		float yPos = -70f;
		// Debug.Log($"yPos={yPos}");
		if(variantLines[2].Length > 0)
		{
			string[] variantSpecialOptions = variantLines[2].Replace("variantSpecialOptions=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
			for(int i = 0; i < variantSpecialOptions.Length; i++)
			{
				string[] specialOptionData = variantSpecialOptions[i].Split('#');
				if(specialOptionData.Length > 3)
				{
					if(specialOptionData[3].Contains("{0}"))
					{
						specialOptionsString += string.Format(specialOptionData[3], specialOptionData[2]);
					}
					else
					{
						specialOptionsString += specialOptionData[3];
					}
				}
				else
				{
					specialOptionsString += specialOptionData[2];
				}
				if(i < variantSpecialOptions.Length - 1)
				{
					specialOptionsString += "\n";
				}
			}
		}
		// Debug.Log($"specialOptionsString={specialOptionsString}");
		if(specialOptionsString != string.Empty)
		{
			specialOptionsLabel.gameObject.SetActive(true);
			specialOptionsLabel.ChangeText(specialOptionsString);
			float specialOptionsLabelHeight = specialOptionsLabel.GetPreferredHeight();
			yPos -= specialOptionsLabelHeight;
			// Debug.Log($"yPos={yPos} specialOptionsLabelHeight={specialOptionsLabelHeight}");
		}
		else
		{
			specialOptionsLabel.gameObject.SetActive(false);
		}
		
		string[] variantCards = variantLines[1].Replace("startingDeck=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		int[,] suitRankCards = new int[5, 13];
		Dictionary<string, int> specialCards = new Dictionary<string, int>();
		for(int i = 0; i < variantCards.Length; i++)
		{
			if(variantCards[i].Length == 2)
			{
				int rank = -1;
				int suit = -1;
				switch(variantCards[i][0])
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
						LocalInterface.instance.DisplayError($"Failed to parse rank of variantCards={variantCards[i]}");
					break;
				}
				switch(variantCards[i][1])
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
						LocalInterface.instance.DisplayError($"Failed to parse suit of variantCards={variantCards[i]}");
					break;
				}
				suitRankCards[suit, rank]++;
			}
			else // is special
			{
				if(specialCards.ContainsKey(variantCards[i]))
				{
					specialCards[variantCards[i]]++;
				}
				else
				{
					specialCards.Add(variantCards[i], 1);
				}
			}
		}
		int cardIndex = 0;
		for(int i = 0; i < suitRankCards.GetLength(0); i++)
		{
			for(int j = 0; j < suitRankCards.GetLength(1); j++)
			{
				if(i < 4)
				{
					if(suitRankCards[i, j] != 1)
					{
						CreateCardExplainer(yPos, cardIndex, suitRankCards[i, j], new CardData(j, i));
						cardIndex++;
					}
				}
				else
				{
					if(suitRankCards[i, j] != 0)
					{
						CreateCardExplainer(yPos, cardIndex, suitRankCards[i, j], new CardData(j, i));
						cardIndex++;
					}
				}
			}
		}
		foreach(KeyValuePair<string, int> entry in specialCards)
		{
			CreateCardExplainer(yPos, cardIndex, entry.Value, new CardData(-1, -1, entry.Key));
			cardIndex++;
		}
		for(int i = cardIndex; i < cardExplainers.Count; i++)
		{
			cardExplainers[i].gameObject.SetActive(false);
		}
		float cardsHeight = ((cardIndex + (cardIndex > 0 ? itemsWide - 1 : 0)) / itemsWide) * 49f;
		yPos -= cardsHeight;
		// Debug.Log($"yPos={yPos} cardsHeight={cardsHeight}");
		string[] variantBaubles = variantLines[3].Replace("variantBaubles=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < variantBaubles.Length; i++)
		{
			string[] baubleData = variantBaubles[i].Split('#', StringSplitOptions.RemoveEmptyEntries);
			CreateBaubleExplainer(yPos, i, baubleData[0]);
		}
		float baublesHeight = ((variantBaubles.Length + (variantBaubles.Length > 0 ? itemsWide - 1 : 0)) / itemsWide) * 49f;
		yPos -= baublesHeight;
		// Debug.Log($"yPos={yPos} baublesHeight={baublesHeight}");
		for(int i = variantBaubles.Length; i < baubleExplainers.Count; i++)
		{
			baubleExplainers[i].gameObject.SetActive(false);
		}
		string[] variantRounds = variantLines[4].Replace("variantRounds=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		List<VariantRoundForSorting> variantRoundsForSorting = new List<VariantRoundForSorting>();
		for(int i = 0; i < variantRounds.Length; i++)
		{
			string[] roundData = variantRounds[i].Split('#', StringSplitOptions.RemoveEmptyEntries);
			if(int.Parse(roundData[0]) > 29)
			{
				continue;
			}
			if(roundData.Length > 2)
			{
				variantRoundsForSorting.Add(new VariantRoundForSorting(int.Parse(roundData[0]), double.Parse(roundData[1]), roundData[2]));
			}
			else
			{
				variantRoundsForSorting.Add(new VariantRoundForSorting(int.Parse(roundData[0]), double.Parse(roundData[1]), string.Empty));
			}
		}
		variantRoundsForSorting.Sort((x, y) =>
		{
			return x.roundNumber - y.roundNumber;
		});
		if(variantRounds.Length > 0)
		{
			roundsLabel.gameObject.SetActive(true);
			roundsLabel.rt.anchoredPosition = new Vector2(roundsLabel.rt.anchoredPosition.x, yPos);
			string variantRoundText = string.Empty;
			// for(int i = 0; i < variantRounds.Length; i++)
			for(int i = 0; i < variantRoundsForSorting.Count; i++)
			{
				// string[] roundData = variantRounds[i].Split('#', StringSplitOptions.RemoveEmptyEntries);
/* 				if(int.Parse(roundData[0]) > 29)
				{
					continue;
				} */
				// variantRoundText += $"<color=red>{(int.Parse(roundData[0]) + 1)}</color> {LocalInterface.instance.ConvertDoubleToString(double.Parse(roundData[1]))}";
				variantRoundText += $"<color=red>{(variantRoundsForSorting[i].roundNumber + 1)}</color> {LocalInterface.instance.ConvertDoubleToString(variantRoundsForSorting[i].roundScore)}";
				// if(roundData.Length > 2)
				if(variantRoundsForSorting[i].bossType != string.Empty)
				{
					/* if(roundData[2] != string.Empty)
					{ */
						// if(roundData[2].Length >= 6)
						if(variantRoundsForSorting[i].bossType.Length >= 6)
						{
							// if(roundData[2].Substring(0, 6) == "Random")	// RandomTier00-01
							if(variantRoundsForSorting[i].bossType.Substring(0, 6) == "Random")	// RandomTier00-01
							{
								// int lowerEnd = int.Parse(roundData[2].Substring(10, 2));
								int lowerEnd = int.Parse(variantRoundsForSorting[i].bossType.Substring(10, 2));
								// if(roundData[2].Length >= 13)
								if(variantRoundsForSorting[i].bossType.Length >= 13)
								{
									// int upperEnd = int.Parse(roundData[2].Substring(13, 2));
									int upperEnd = int.Parse(variantRoundsForSorting[i].bossType.Substring(13, 2));
									variantRoundText += $"\n<color=red>Random Tier {lowerEnd + 1}-{upperEnd + 1} Boss</color>";
								}
								else
								{
									variantRoundText += $"\n<color=red>Random Tier {lowerEnd + 1} Boss</color>";
								}
							}
							else
							{
								// variantRoundText += $"\n<color=red>{LocalInterface.instance.bossLevels[roundData[2]].description}</color>";
								variantRoundText += $"\n<color=red>{LocalInterface.instance.bossLevels[variantRoundsForSorting[i].bossType].description}</color>";
							}
						}
						else
						{
							// variantRoundText += $"\n<color=red>{LocalInterface.instance.bossLevels[roundData[2]].description}</color>";
							variantRoundText += $"\n<color=red>{LocalInterface.instance.bossLevels[variantRoundsForSorting[i].bossType].description}</color>";
						}
					// }
				}
				if(i < variantRounds.Length - 1)
				{
					variantRoundText += "\n";
				}
			}
			roundsLabel.ChangeText(variantRoundText, true);
			float roundsLabelHeight = roundsLabel.GetPreferredHeight() + 6f;
			yPos -= roundsLabelHeight;
			// Debug.Log($"yPos={yPos} roundsLabelHeight={roundsLabelHeight}");
		}
		else
		{
			roundsLabel.gameObject.SetActive(false);
		}
		string[] variantSpecialCards = variantLines[5].Replace("variantSpecialCards=", string.Empty).Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < variantSpecialCards.Length; i++)
		{
			string[] specialCardData = variantSpecialCards[i].Split('#');
			CreateSpecialCardExplainer(yPos, i,  specialCardData[0]);
		}
		float specialCardsHeight = ((variantSpecialCards.Length + (variantSpecialCards.Length > 0 ? itemsWide - 1 : 0)) / itemsWide) * 49f;
		yPos -= specialCardsHeight;
		// Debug.Log($"yPos={yPos} specialCardsHeight={specialCardsHeight}");
		content.sizeDelta = new Vector2(content.sizeDelta.x, -yPos);
		controllerSelectionGroup.AddToCurrentGroups();
	}
	
	public void CreateCardExplainer(float yPos, int cardIndex, int quantity, CardData cardData)
	{
		if(cardExplainers.Count > cardIndex)
		{
			cardExplainers[cardIndex].gameObject.SetActive(true);
			cardExplainers[cardIndex].SetupCardExplainer(new Vector2(27f + 49f * (cardIndex % itemsWide), yPos - 22f - 49f * (cardIndex / itemsWide)), cardData, quantity);
		}
		else
		{
			GameObject newCardExplainerGO = Instantiate(cardExplainerPrefab, content);
			CardExplainer newCardExplainer = newCardExplainerGO.GetComponent<CardExplainer>();
			cardExplainers.Add(newCardExplainer);
			newCardExplainer.SetupCardExplainer(new Vector2(27f + 49f * (cardIndex % itemsWide), yPos - 22f - 49f * (cardIndex / itemsWide)), cardData, quantity);
		}
	}
	
	public void CreateBaubleExplainer(float yPos, int baubleIndex, string tag)
	{
		if(baubleExplainers.Count > baubleIndex)
		{
			baubleExplainers[baubleIndex].gameObject.SetActive(true);
			baubleExplainers[baubleIndex].SetupBaubleExplainer(new Vector2(27f + 49f * (baubleIndex % itemsWide), yPos - 22f - 49f * (baubleIndex / itemsWide)), tag);
		}
		else
		{
			GameObject newBaubleExplainerGO = Instantiate(baubleExplainerPrefab, content);
			BaubleExplainer newBaubleExplainer = newBaubleExplainerGO.GetComponent<BaubleExplainer>();
			baubleExplainers.Add(newBaubleExplainer);
			newBaubleExplainer.SetupBaubleExplainer(new Vector2(27f + 49f * (baubleIndex % itemsWide), yPos - 22f - 49f * (baubleIndex / itemsWide)), tag);
		}
	}
	
	public void CreateSpecialCardExplainer(float yPos, int specialCardIndex, string tag)
	{
		if(specialCardExplainers.Count > specialCardIndex)
		{
			specialCardExplainers[specialCardIndex].gameObject.SetActive(true);
			specialCardExplainers[specialCardIndex].SetupSpecialCardExplainer(new Vector2(27f + 49f * (specialCardIndex % itemsWide), yPos - 22f - 49f * (specialCardIndex / itemsWide)), tag);
		}
		else
		{
			GameObject newSpecialCardExplainerGO = Instantiate(specialCardExplainerPrefab, content);
			SpecialCardExplainer newSpecialCardExplainer = newSpecialCardExplainerGO.GetComponent<SpecialCardExplainer>();
			specialCardExplainers.Add(newSpecialCardExplainer);
			newSpecialCardExplainer.SetupSpecialCardExplainer(new Vector2(27f + 49f * (specialCardIndex % itemsWide), yPos - 22f - 49f * (specialCardIndex / itemsWide)), tag);
		}
	}
	
	public void Close()
	{
		visibilityObject.SetActive(false);
		controllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public class VariantRoundForSorting
	{
		public int roundNumber;
		public double roundScore;
		public string bossType;
		public VariantRoundForSorting(int roundNumber, double roundScore, string bossType)
		{
			this.roundNumber = roundNumber;
			this.roundScore = roundScore;
			this.bossType = bossType;
		}
	}
}
