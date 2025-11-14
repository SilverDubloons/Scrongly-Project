using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RoundOptions : MonoBehaviour
{
    public RectTransform rt;
	public Label roundLabel;
	public Toggle bossToggle;
	public ButtonPlus pickButton;
	public Label bossLabel;
	public ControllerSelectableObject bossToggleControllerSelectableObject;
	public ControllerSelectableObject pickButtonControllerSelectableObject;
	public ControllerSelectableObject scoreThresholdInputFieldControllerSelectableObject;
	
	public int roundNumber;
	public double roundScoreThreshold;
	public TMP_InputField scoreThresholdInputField;
	public bool setupComplete;
	public string bossTag;
	
	public void BossToggleUpdated()
	{
		pickButton.ChangeButtonEnabled(bossToggle.isOn);
		if(bossToggle.isOn)
		{
			int highestBossTier = 5;
			int maxRound = 29;
			float scalingFactor = (float)highestBossTier / (float)maxRound;
			int bossRound = Mathf.RoundToInt((float)roundNumber * scalingFactor);
			UpdateBossTag($"RandomTier0{bossRound.ToString()}");
		}
		else
		{
			UpdateBossTag(string.Empty);
		}
	}
	
	public void UpdateRoundScore(double newScore)
	{
		roundScoreThreshold = newScore;
		if(newScore <= 0)
		{
			scoreThresholdInputField.text = string.Empty;
		}
		else
		{
			scoreThresholdInputField.text = newScore.ToString();
		}
	}
	
	public void UpdateBossTag(string newBossTag)
	{
		bossTag = newBossTag;
		if(bossTag == string.Empty)
		{
			bossLabel.ChangeText("Standard Round");
			bossToggle.isOn = false;
			pickButton.ChangeButtonEnabled(false);
		}
		else
		{
			if(newBossTag.Length >= 6)
			{
				if(newBossTag.Substring(0, 6) == "Random")	// RandomTier00-01
				{
					int lowerEnd = int.Parse(newBossTag.Substring(10, 2));
					if(newBossTag.Length >= 13)
					{
						int upperEnd = int.Parse(newBossTag.Substring(13, 2));
						bossLabel.ChangeText($"Random Tier {lowerEnd + 1}-{upperEnd + 1} Boss");
					}
					else
					{
						bossLabel.ChangeText($"Random Tier {lowerEnd + 1} Boss");
					}
				}
				else
				{
					bossLabel.ChangeText($"{BossPicker.instance.bossRounds[newBossTag].description}");
				}
			}
			else
			{
				bossLabel.ChangeText($"{BossPicker.instance.bossRounds[newBossTag].description}");
			}
			pickButton.ChangeButtonEnabled(true);
			bossToggle.isOn = true;
		}
	}
	
	public void SetButtonState(bool buttonState)
	{
		if(bossTag == string.Empty)
		{
			pickButton.ChangeButtonEnabled(false);
		}
		else
		{
			pickButton.ChangeButtonEnabled(buttonState);
		}
		if(RoundsVariantMenu.instance.customToggle.isOn)
		{
			scoreThresholdInputField.interactable = buttonState;
		}
		else
		{
			scoreThresholdInputField.interactable = false;
		}
	}
	
	public void PickButtonClicked()
	{
		BossPicker.instance.OpenBossPicker(roundNumber, bossTag);
	}
	
	public void ScoreThresholdInputFieldUpdated()
	{
		
	}
	
	public void ScoreThresholdInputFieldFinished()
	{
		if(scoreThresholdInputField.text.Length <= 0)
		{
			roundScoreThreshold = 0;
			return;
		}
		try
		{	
			double roundScoreInput = double.Parse(scoreThresholdInputField.text);
			if(roundScoreInput > RoundsVariantMenu.instance.reasonableRoundScoreMaximum)
			{
				roundScoreThreshold = RoundsVariantMenu.instance.reasonableRoundScoreMaximum;
				scoreThresholdInputField.text = roundScoreThreshold.ToString();
			}
			else
			{
				roundScoreThreshold = roundScoreInput;
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Could not parse \"{scoreThresholdInputField.text}\" to a double. Exception message= {exception.Message}");
			scoreThresholdInputField.text = VariantsMenu.instance.loadedVariant.variantRounds[roundNumber].scoreNeeded.ToString();
		}
	}
}
