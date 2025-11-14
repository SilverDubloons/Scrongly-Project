using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static LocalInterface;

public class BossPicker : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;
	public Label tierRangeLabel;
	public GameObject visibleObject;
	public Label roundLabel;
	public ButtonPlus randomTierButton;
	public RectTransform bossButtonParent;
	public RectTransform bossButtonContentRectTransform;
	public GameObject bossPickerButtonPrefab;
	public ControllerSelectionGroup controllerSelectionGroup;
	public Scrollbar verticalScrollbar;
	
	public TextAsset bossRoundsText;
	public Dictionary<string, BossRound> bossRounds = new Dictionary<string, BossRound>();
	public List<BossPickerButton> bossPickerButtons;
	public float bossPickerButtonHeight;
	public float distanceBetweenBossPickerButtons;
	public int currentRound;
	
	public static BossPicker instance;
	
	public void SetupInstance(bool firstSceneChange)
	{
		instance = this;
		CreateBossButtonsFromText(firstSceneChange);
		visibleObject.SetActive(false);
	}
	
	public struct BossRound
	{
		public string tag;
		public string description;
		public int tier;
		public BossRound(string tag, string description, int tier)
		{
			this.tag = tag;
			this.description = description;
			this.tier = tier;
		}
	}
	
	public void SliderUpdated()
	{
		int lesser = Mathf.Min(Mathf.RoundToInt(slider1.value), Mathf.RoundToInt(slider2.value));
		int greater = Mathf.Max(Mathf.RoundToInt(slider1.value), Mathf.RoundToInt(slider2.value));
		if(lesser == greater)
		{
			tierRangeLabel.ChangeText((lesser + 1).ToString());
			randomTierButton.ChangeButtonText($"Random Tier {(lesser + 1).ToString()}");
		}
		else
		{
			tierRangeLabel.ChangeText($"{(lesser + 1).ToString()}-{(greater + 1).ToString()}");
			randomTierButton.ChangeButtonText($"Random Tier {(lesser + 1).ToString()}-{(greater + 1).ToString()}");
		}
		int buttonIndex = 0;
		for(int i = 0; i < bossPickerButtons.Count; i++)
		{
			if(bossPickerButtons[i].tier >= lesser && bossPickerButtons[i].tier <= greater)
			{
				bossPickerButtons[i].gameObject.SetActive(true);
				bossPickerButtons[i].rt.anchoredPosition = new Vector2(0, -distanceBetweenBossPickerButtons - (distanceBetweenBossPickerButtons + bossPickerButtonHeight) * buttonIndex);
				buttonIndex++;
				bossPickerButtons[i].controllerSelectableObject.positionInScrollView = bossPickerButtons[i].rt.anchoredPosition.y;
			}
			else
			{
				bossPickerButtons[i].gameObject.SetActive(false);
			}
		}
		bossButtonContentRectTransform.sizeDelta = new Vector2(bossButtonContentRectTransform.sizeDelta.x, distanceBetweenBossPickerButtons + (distanceBetweenBossPickerButtons + bossPickerButtonHeight) * buttonIndex);
	}
	
	public void CreateBossButtonsFromText(bool firstSceneChange)
	{
		string[] rows = bossRoundsText.text.Split('\n');
		int highestTier = -1;
		for(int i = 1; i < rows.Length; i++)
		{
			string[] columns = rows[i].Split(',');
			int tier = int.Parse(columns[2]);
			if(tier > highestTier)
			{
				highestTier = tier;
			}
		}
		for(int i = 0; i <= highestTier; i++)
		{
			LocalInterface.instance.bossTiers.Add(new BossTier());
		}
		for(int i = 1; i < rows.Length; i++)
		{
			string[] columns = rows[i].Split(',');
			string tag = columns[0];
			string description = columns[1].Replace("COMMA", ",");
			int tier = int.Parse(columns[2]);
			// Debug.Log($"i={i}, tag={tag}, description={description}");
			if(firstSceneChange)
			{
				LocalInterface.instance.bossTiers[tier].bossLevels.Add(new BossLevel(tag, description, tier));
				LocalInterface.instance.bossLevels.Add(tag, new BossLevel(tag, description, tier));
			}
			bossRounds.Add(tag, new BossRound(tag, description, tier));
			GameObject newBossPickerButtonGO = Instantiate(bossPickerButtonPrefab, bossButtonParent);
			BossPickerButton newBossPickerButton = newBossPickerButtonGO.GetComponent<BossPickerButton>();
			newBossPickerButton.buttonPlus.ChangeButtonText(description);
			newBossPickerButton.bossTag = tag;
			newBossPickerButton.tier = tier;
			bossPickerButtons.Add(newBossPickerButton);
			controllerSelectionGroup.controllerSelectableObjects.Add(newBossPickerButton.controllerSelectableObject);
			newBossPickerButton.controllerSelectableObject.scrollViewContentRT = bossButtonParent;
			newBossPickerButton.controllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
		}
		bossPickerButtons.Sort((x, y) =>
		{
			return x.tier.CompareTo(y.tier);
		});
	}
	
	public void RandomTierButtonClicked()
	{
		int slider1Value = Mathf.RoundToInt(slider1.value);
		int slider2Value = Mathf.RoundToInt(slider2.value);
		int lesser = Mathf.Min(slider1Value, slider2Value);
		int greater = Mathf.Max(slider1Value, slider2Value);
		if(slider1Value == slider2Value)
		{
			RoundsVariantMenu.instance.roundOptions[currentRound].UpdateBossTag($"RandomTier0{slider1Value.ToString()}");
		}
		else
		{
			RoundsVariantMenu.instance.roundOptions[currentRound].UpdateBossTag($"RandomTier0{lesser.ToString()}-0{greater.ToString()}");
		}
		visibleObject.SetActive(false);
		controllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public void BossPickerButtonClicked(string tag)
	{
		RoundsVariantMenu.instance.roundOptions[currentRound].UpdateBossTag(tag);
		visibleObject.SetActive(false);
		controllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public void CancelButtonClicked()
	{
		visibleObject.SetActive(false);
		controllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public void OpenBossPicker(int round, string bossTag)
	{
		currentRound = round;
		visibleObject.SetActive(true);
		roundLabel.ChangeText($"Round {round + 1} Boss");
		if(bossTag == string.Empty)
		{
			slider1.value = 0;
			slider2.value = slider2.maxValue;
		}
		else
		{
			if(bossTag.Length >= 6)
			{
				if(bossTag.Substring(0, 6) == "Random")	// RandomTier00-01
				{
					int lowerEnd = int.Parse(bossTag.Substring(10, 2));
					if(bossTag.Length >= 13)
					{
						int upperEnd = int.Parse(bossTag.Substring(13, 2));
						slider1.value = lowerEnd;
						slider2.value = upperEnd;
					}
					else
					{
						slider1.value = lowerEnd;
						slider2.value = lowerEnd;
					}
				}
				else
				{
					slider1.value = bossRounds[bossTag].tier;
					slider2.value = bossRounds[bossTag].tier;
				}
			}
			else
			{
				slider1.value = bossRounds[bossTag].tier;
				slider2.value = bossRounds[bossTag].tier;
			}
		}
		SliderUpdated();
		controllerSelectionGroup.AddToCurrentGroups();
	}
}
