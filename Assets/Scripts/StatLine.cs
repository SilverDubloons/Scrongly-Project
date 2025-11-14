using UnityEngine;

public class StatLine : MonoBehaviour
{
	public RectTransform rt;
	
    public Label statNameLabel;
    public Label statValueLabel;
    public RectTransform separator;
	public const float totalWidth = 390f;
	
	public void SetupStatLine(string statName, string statValue)
	{
		statNameLabel.ChangeText(statName);
		statValueLabel.ChangeText(statValue);
		
		float statNameWidth = statNameLabel.GetPreferredWidth();
		float statValueWidth = statValueLabel.GetPreferredWidth();
		
		separator.sizeDelta = new Vector2(totalWidth - statNameWidth - statValueWidth, separator.sizeDelta.y);
		separator.anchoredPosition = new Vector2(statNameWidth + 5f, separator.anchoredPosition.y);
	}
}
