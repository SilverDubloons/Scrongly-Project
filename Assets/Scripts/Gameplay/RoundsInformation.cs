using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoundsInformation : MonoBehaviour
{
    public Label currentRoundLabel;
	public Label currentRoundThresholdLabel;
	public Label roundNumbersLabel;
	public RectTransform roundNumbersLabelRT;
	public Label roundInfoLabel;
	public RectTransform roundInfoRT;
	public RectTransform roundContentRT;
	public Scrollbar roundInfoScrollbar;
	
	public static RoundsInformation instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void UpdateRoundsInformation(bool includeEndlessMode)
	{
		currentRoundLabel.ChangeText((GameManager.instance.currentRound + 1).ToString());
		currentRoundThresholdLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(GameManager.instance.GetCurrentRoundScoreThreshold()));
		StartCoroutine(UpdateRoundsInformationCoroutine(includeEndlessMode));
	}
	
	public IEnumerator UpdateRoundsInformationCoroutine(bool includeEndlessMode)
	{
		string roundInfo = string.Empty;
		string roundNumbers = string.Empty;
		Vector2 roundNumbersLabelRTOffsetMin = roundNumbersLabelRT.offsetMin;
		roundNumbersLabelRTOffsetMin.y = 0;
		roundNumbersLabelRT.offsetMin = roundNumbersLabelRTOffsetMin;
		float lineSize = 9.28f;
		float lastInfoSize = 0;
		if(GameManager.instance.currentRound <= 29)
		{
			for(int i = GameManager.instance.currentRound; i <= 29; i++)
			{
				roundInfo += $"{LocalInterface.instance.ConvertDoubleToString(V.i.v.variantRounds[i].scoreNeeded)}\n";
				if(GameManager.instance.bossRounds.ContainsKey(i))
				{
					roundInfo += $"<color=red>{GameManager.instance.bossRounds[i].description}</color>\n";
				}
				roundInfoLabel.ChangeText(roundInfo);
				yield return null;
				float infoSize = roundInfoLabel.GetPreferredHeight();
				float infoSizeDifference = infoSize - lastInfoSize;
				int linesAdded = Mathf.RoundToInt(infoSizeDifference / lineSize);
				// Debug.Log($"i={i}, infoSize={infoSize}, infoSizeDifference={infoSizeDifference}, linesAdded={linesAdded}");
				lastInfoSize = infoSize;
				roundNumbers += $"<color=red>{i + 1}</color>";
				for(int j = 0; j < linesAdded; j++)
				{
					roundNumbers += "\n";
				}
			}
			roundInfoLabel.ChangeText(roundInfo, true);
		}
		if(includeEndlessMode)
		{
			for(int i = Mathf.Max(GameManager.instance.currentRound, 30); i <= 49; i++)
			{
				roundInfo += $"{LocalInterface.instance.ConvertDoubleToString(V.i.v.variantRounds[i].scoreNeeded)}\n";
				roundNumbers += $"<color=red>{i + 1}</color>\n";
			}
			roundInfoLabel.ChangeText(roundInfo, true);
		}
		roundNumbersLabel.ChangeText(roundNumbers.Trim(), true);
		yield return null;
		float infoPreferedHeight = roundInfoLabel.GetPreferredHeight();
		float numbersPreferedHeight = roundNumbersLabel.GetPreferredHeight();
		// roundNumbersLabelRT.offsetMin.y = infoPreferedHeight - numbersPreferedHeight;
		roundNumbersLabelRTOffsetMin.y = infoPreferedHeight - numbersPreferedHeight;
		roundNumbersLabelRT.offsetMin = roundNumbersLabelRTOffsetMin;
		roundInfoRT.sizeDelta = new Vector2(roundInfoRT.sizeDelta.x, Mathf.Min(infoPreferedHeight + 18f, LocalInterface.instance.referenceResolution.y));
		roundInfoRT.anchoredPosition = new Vector2(roundInfoRT.anchoredPosition.x, Mathf.Lerp(0, -37f, roundInfoRT.sizeDelta.y / LocalInterface.instance.referenceResolution.y));
		roundContentRT.sizeDelta = new Vector2(roundContentRT.sizeDelta.x, infoPreferedHeight + 6f);
		yield return null;
		roundInfoScrollbar.value = 1f;
	}
}
