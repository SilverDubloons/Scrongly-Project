using UnityEngine;

public class BossPickerButton : MonoBehaviour
{
	public RectTransform rt;
	public ButtonPlus buttonPlus;
	public int tier;
	public string bossTag;
	public ControllerSelectableObject controllerSelectableObject;
	
    public void ButtonClicked()
	{
		BossPicker.instance.BossPickerButtonClicked(bossTag);
	}
}
