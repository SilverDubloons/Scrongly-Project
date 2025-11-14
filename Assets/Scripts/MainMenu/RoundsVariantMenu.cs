using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Variant;
using System;

public class RoundsVariantMenu : MonoBehaviour
{
    public ButtonPlus defaultButton;
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
	public Slider difficultySlider;
	public Label difficultyLabel;
	public Toggle customToggle;
	public RectTransform roundOptionsParent;
	public RectTransform roundOptionsContentRectTransform;
	public ControllerSelectionGroup controllerSelectionGroup;
	public Scrollbar verticalScrollbar;
	
	public GameObject roundOptionsPrefab;
	
	public double reasonableRoundScoreMaximum;
	public int roundOptionsWide;
	public float distanceBetweenRoundOptions;
	public Vector2 roundOptionsSize;
	
	public bool roundHasChanged;
	public List<RoundOptions> roundOptions = new List<RoundOptions>();
	
	public static RoundsVariantMenu instance;
	public BossPicker bossPicker;
	public double[,] difficultyLevelRounds = new double[6,50];
	
	public void SetupInstance(bool firstSceneChange)
	{
		bossPicker.SetupInstance(firstSceneChange);
		instance = this;
	}
	
	public void SetRoundsVariantsMenuButtons(bool enabledState)
	{
/* 		if(roundHasChanged)
		{	
			confirmButton.ChangeButtonEnabled(enabledState);
		}
		else
		{
			confirmButton.ChangeButtonEnabled(false);
		} */
		SetRoundsSetupCompleteStatus(enabledState);
		confirmButton.ChangeButtonEnabled(enabledState);
		defaultButton.ChangeButtonEnabled(enabledState);
		cancelButton.ChangeButtonEnabled(enabledState);
		if(customToggle.isOn)
		{
			difficultySlider.interactable = false;
		}
		else
		{
			difficultySlider.interactable = enabledState;
		}
		for(int i = 0; i < roundOptions.Count; i++)
		{
			if(customToggle.isOn)
			{
				roundOptions[i].scoreThresholdInputField.interactable = enabledState;
			}
			else
			{
				roundOptions[i].scoreThresholdInputField.interactable = false;
			}
			roundOptions[i].bossToggle.interactable = enabledState;
		}
		customToggle.interactable = enabledState;
	}
	
	public void SetupRoundsVariantMenu(Variant variant)
	{
		foreach(KeyValuePair<int, VariantRound> entry in variant.variantRounds)
		{
			if(entry.Value.roundNumber < 30)
			{
				GameObject newRoundOptionsGO = Instantiate(roundOptionsPrefab, roundOptionsParent);
				newRoundOptionsGO.name = $"Round {entry.Value.roundNumber}";
				RoundOptions newRoundOptions = newRoundOptionsGO.GetComponent<RoundOptions>();
				roundOptions.Add(newRoundOptions);
				newRoundOptions.roundLabel.ChangeText((entry.Value.roundNumber + 1).ToString());
				if(entry.Value.bossType != string.Empty)
				{
					newRoundOptions.bossToggle.isOn = true;
					newRoundOptions.pickButton.ChangeButtonEnabled(true);
					if(entry.Value.bossType.Length >= 6)
					{
						if(entry.Value.bossType.Substring(0, 6) == "Random")	// RandomTier00-01
						{
							int lowerEnd = int.Parse(entry.Value.bossType.Substring(10, 2));
							if(entry.Value.bossType.Length >= 13)
							{
								int upperEnd = int.Parse(entry.Value.bossType.Substring(13, 2));
								newRoundOptions.bossLabel.ChangeText($"Random Tier {lowerEnd + 1}-{upperEnd + 1} Boss");
							}
							else
							{
								newRoundOptions.bossLabel.ChangeText($"Random Tier {lowerEnd + 1} Boss");
							}
						}
						else
						{
							newRoundOptions.bossLabel.ChangeText($"{BossPicker.instance.bossRounds[entry.Value.bossType].description}");
						}
					}
					else
					{
						newRoundOptions.bossLabel.ChangeText($"{BossPicker.instance.bossRounds[entry.Value.bossType].description}");
					}
				}
				newRoundOptions.roundNumber = entry.Value.roundNumber;
				newRoundOptions.scoreThresholdInputField.text = entry.Value.scoreNeeded.ToString();
				newRoundOptions.bossToggleControllerSelectableObject.scrollViewContentRT = roundOptionsParent;
				newRoundOptions.pickButtonControllerSelectableObject.scrollViewContentRT = roundOptionsParent;
				newRoundOptions.scoreThresholdInputFieldControllerSelectableObject.scrollViewContentRT = roundOptionsParent;
				newRoundOptions.bossToggleControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newRoundOptions.pickButtonControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newRoundOptions.scoreThresholdInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				controllerSelectionGroup.controllerSelectableObjects.Add(newRoundOptions.bossToggleControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newRoundOptions.pickButtonControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newRoundOptions.scoreThresholdInputFieldControllerSelectableObject);
				
			}
		}
		roundOptions.Sort((x, y) =>
		{
			return x.roundNumber.CompareTo(y.roundNumber);
		});
		for(int i = 0; i < roundOptions.Count; i++)
		{
			roundOptions[i].rt.anchoredPosition = new Vector2(distanceBetweenRoundOptions + (i % roundOptionsWide) * (distanceBetweenRoundOptions + roundOptionsSize.x), -distanceBetweenRoundOptions - (i / roundOptionsWide) * (distanceBetweenRoundOptions + roundOptionsSize.y));
			roundOptions[i].bossToggleControllerSelectableObject.positionInScrollView = roundOptions[i].rt.anchoredPosition.y;
			roundOptions[i].pickButtonControllerSelectableObject.positionInScrollView = roundOptions[i].rt.anchoredPosition.y;
			roundOptions[i].scoreThresholdInputFieldControllerSelectableObject.positionInScrollView = roundOptions[i].rt.anchoredPosition.y;
		}
		roundOptionsContentRectTransform.sizeDelta = new Vector2(roundOptionsContentRectTransform.sizeDelta.x, distanceBetweenRoundOptions + (roundOptions.Count / 3) * (distanceBetweenRoundOptions + roundOptionsSize.y));
	}
	
/* 	public void RoundHasChanged()
	{
		if(roundHasChanged)
		{
			return;
		}
		roundHasChanged = true;
		confirmButton.ChangeButtonEnabled(true);
	} */
	
/* 	public void ResetRoundsChangedStatus()
	{
		for(int i = 0; i < roundOptions.Count; i++)
		{
			roundOptions[i].roundHasChanged = false;
		}
	} */
	
	public void CustomToggleChanged()
	{
		difficultySlider.interactable = !customToggle.isOn;
		int difficulty = Mathf.RoundToInt(difficultySlider.value);
		for(int i = 0; i < roundOptions.Count; i++)
		{
			if(customToggle.isOn)
			{
				roundOptions[i].scoreThresholdInputField.interactable = true;
			}
			else
			{
				roundOptions[i].scoreThresholdInputField.interactable = false;
				roundOptions[i].UpdateRoundScore(difficultyLevelRounds[difficulty, i]);
			}
		}
	}
	
	public void DifficultySliderChanged()
	{
		int difficulty = Mathf.RoundToInt(difficultySlider.value);
		difficultyLabel.ChangeText($"Difficulty {difficulty + 1}");
		for(int i = 0; i < roundOptions.Count; i++)
		{
			roundOptions[i].UpdateRoundScore(difficultyLevelRounds[difficulty, i]);
		}
	}
	
	public void SetupDifficulties()
	{
		for(int r = 0; r < 6; r++)
		{
			difficultyLevelRounds[r, 0] = 300d;
			difficultyLevelRounds[r, 1] = 500d;
			difficultyLevelRounds[r, 2] = 750d;
			if(r == 0)
			{
				for(int i = 3; i < 50; i++)
				{
					difficultyLevelRounds[r, i] = VariantsMenu.instance.baseVariant.variantRounds[i].scoreNeeded;
				}
			}
			else
			{
				switch(r)
				{
					case 1:
					difficultyLevelRounds[r, 29] = 20000000d;
					break;
					case 2:
					difficultyLevelRounds[r, 29] = 50000000d;
					break;
					case 3:
					difficultyLevelRounds[r, 29] = 100000000d;
					break;
					case 4:
					difficultyLevelRounds[r, 29] = 250000000d;
					break;
					case 5:
					difficultyLevelRounds[r, 29] = 1000000000d;
					break;
				}
				double factor = Math.Pow(difficultyLevelRounds[r, 29] / difficultyLevelRounds[r, 2], 1 / 27f);
				for(int i = 3; i < 29; i++)
				{
					difficultyLevelRounds[r, i] = difficultyLevelRounds[r, i - 1] * factor;
				}
				for(int i = 3; i < 29; i++)
				{
					int numberOfDigits = (int)Math.Floor(Math.Log10(difficultyLevelRounds[r, i])) + 1;
					int roundingNumber = (int)Math.Round(Math.Pow(10, (numberOfDigits - 3)));
					if(roundingNumber == 10)
					{
						roundingNumber = 50;
					}
					else if(roundingNumber == 100)
					{
						roundingNumber = 500;
					}
					difficultyLevelRounds[r, i] = Math.Round(difficultyLevelRounds[r, i] / roundingNumber) * roundingNumber;
				}
				for(int i = 30; i < 35; i++)
				{
					difficultyLevelRounds[r, i] = Math.Min(Math.Round(difficultyLevelRounds[r, i - 1] * 2d), reasonableRoundScoreMaximum);
				}
				for(int i = 35; i < 40; i++)
				{
					difficultyLevelRounds[r, i] = Math.Min(Math.Round(difficultyLevelRounds[r, i - 1] * 5d), reasonableRoundScoreMaximum);
				}
				for(int i = 40; i < 45; i++)
				{
					difficultyLevelRounds[r, i] = Math.Min(Math.Round(difficultyLevelRounds[r, i - 1] * 10d), reasonableRoundScoreMaximum);
				}
				for(int i = 45; i < 50; i++)
				{
					difficultyLevelRounds[r, i] = Math.Min(Math.Round(difficultyLevelRounds[r, i - 1] * 100d), reasonableRoundScoreMaximum);
				}
			}
		}
	}
	
	public void SetRoundOptionsToVariant(Variant variant)
	{
		double round9Value = variant.variantRounds[9].scoreNeeded;
		int difficultyLevel = -1;
		bool isDifficultyLevel = false;
		for(int i = 0; i < 6; i++)
		{
			if(Math.Abs(difficultyLevelRounds[i, 9] - round9Value) <= LocalInterface.instance.epsilon)
			{
				difficultyLevel = i;
				isDifficultyLevel = true;
				break;
			}
		}
		foreach(KeyValuePair<int, VariantRound> entry in variant.variantRounds)
		{
			if(entry.Value.roundNumber < 30)
			{
				if(entry.Value.bossType != string.Empty)
				{
					roundOptions[entry.Value.roundNumber].bossToggle.isOn = true;
					roundOptions[entry.Value.roundNumber].pickButton.ChangeButtonEnabled(true);
					if(entry.Value.bossType.Length >= 6)
					{
						if(entry.Value.bossType.Substring(0, 6) == "Random")	// RandomTier00-01
						{
							int lowerEnd = int.Parse(entry.Value.bossType.Substring(10, 2));
							if(entry.Value.bossType.Length >= 13)
							{
								int upperEnd = int.Parse(entry.Value.bossType.Substring(13, 2));
								roundOptions[entry.Value.roundNumber].bossLabel.ChangeText($"Random Tier {lowerEnd + 1}-{upperEnd + 1} Boss");
							}
							else
							{
								roundOptions[entry.Value.roundNumber].bossLabel.ChangeText($"Random Tier {lowerEnd + 1} Boss");
							}
						}
						else
						{
							roundOptions[entry.Value.roundNumber].bossLabel.ChangeText($"{BossPicker.instance.bossRounds[entry.Value.bossType].description}");
						}
					}
					else
					{
						roundOptions[entry.Value.roundNumber].bossLabel.ChangeText($"{BossPicker.instance.bossRounds[entry.Value.bossType].description}");
					}
				}
				else
				{
					roundOptions[entry.Value.roundNumber].bossToggle.isOn = false;
				}
				if(Math.Abs(difficultyLevelRounds[difficultyLevel, entry.Value.roundNumber] - entry.Value.scoreNeeded) > LocalInterface.instance.epsilon)
				{
					isDifficultyLevel = false;
				}
				roundOptions[entry.Value.roundNumber].roundNumber = entry.Value.roundNumber;
				roundOptions[entry.Value.roundNumber].scoreThresholdInputField.text = entry.Value.scoreNeeded.ToString();
			}
		}
		if(isDifficultyLevel)
		{
			difficultySlider.value = difficultyLevel;
			difficultyLabel.ChangeText($"Difficulty {difficultyLevel + 1}");
			customToggle.isOn = false;
		}
		else
		{
			difficultySlider.value = 0;
			difficultyLabel.ChangeText($"Custom");
			customToggle.isOn = true;
		}
	}
	
	public void SetRoundsSetupCompleteStatus(bool setupCompleteStatus)
	{
		for(int i = 0; i < roundOptions.Count; i++)
		{
			roundOptions[i].setupComplete = setupCompleteStatus;
		}
	}
	
	public void DefaultButtonClicked()
	{
		/* difficultySlider.value = 0;
		customToggle.isOn = false;
		DifficultySliderChanged();
		for(int i = 0; i < roundOptions.Count; i++)
		{
			roundOptions[i].UpdateBossTag(string.Empty);
		} */
		SetRoundOptionsToVariant(VariantsMenu.instance.baseVariant);
	}
	
	public void CancelButtonClicked()
	{
		SetRoundOptionsToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["RoundsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
	public void ConfirmButtonClicked()
	{
		for(int i = 0; i < roundOptions.Count; i++)
		{
			VariantRound newVariantRound = new VariantRound(i, roundOptions[i].roundScoreThreshold, roundOptions[i].bossTag);
			VariantsMenu.instance.loadedVariant.variantRounds[i] = newVariantRound;
		}
		for(int i = 30; i < 35; i++)
		{
			VariantRound newVariantRound = new VariantRound(i, Math.Min(Math.Round(VariantsMenu.instance.loadedVariant.variantRounds[i - 1].scoreNeeded * 2d), reasonableRoundScoreMaximum), string.Empty);
			VariantsMenu.instance.loadedVariant.variantRounds[i] = newVariantRound;
		}
		for(int i = 35; i < 40; i++)
		{
			VariantRound newVariantRound = new VariantRound(i, Math.Min(Math.Round(VariantsMenu.instance.loadedVariant.variantRounds[i - 1].scoreNeeded * 5d), reasonableRoundScoreMaximum), string.Empty);
			VariantsMenu.instance.loadedVariant.variantRounds[i] = newVariantRound;
		}
		for(int i = 40; i < 45; i++)
		{
			VariantRound newVariantRound = new VariantRound(i, Math.Min(Math.Round(VariantsMenu.instance.loadedVariant.variantRounds[i - 1].scoreNeeded * 10d), reasonableRoundScoreMaximum), string.Empty);
			VariantsMenu.instance.loadedVariant.variantRounds[i] = newVariantRound;
		}
		for(int i = 45; i < 50; i++)
		{
			VariantRound newVariantRound = new VariantRound(i, Math.Min(Math.Round(VariantsMenu.instance.loadedVariant.variantRounds[i - 1].scoreNeeded * 100d), reasonableRoundScoreMaximum), string.Empty);
			VariantsMenu.instance.loadedVariant.variantRounds[i] = newVariantRound;
		}
		MovingObjects.instance.mo["RoundsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
}
