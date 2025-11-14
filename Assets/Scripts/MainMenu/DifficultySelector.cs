using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
	public GameObject visibilityObject;
    public VariantSimple variantSimple;
	public Slider difficultySlider;
	
	[TextArea]
	public string[] difficultyVariantStrings;
	
	public static DifficultySelector instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		difficultySlider.enabled = enabledState;
		variantSimple.SetVariantButton(enabledState);
	}
	
	public void DisableVisibilityIfNoDifficultyBeaten()
	{
		int highestDifficultyBeaten = Stats.instance.GetHighestDifficultyCompleted();
		if(highestDifficultyBeaten < 0)
		{
			visibilityObject.SetActive(false);
		}
	}
	
	public void SceneLoadSetup()
	{
		// variantSimple.variantButton.ChangeButtonEnabled(false);
		int highestDifficultyBeaten = Stats.instance.GetHighestDifficultyCompleted();
		if(highestDifficultyBeaten < 0)
		{
			visibilityObject.SetActive(false);
		}
		else
		{
			int lastSelectedDifficulty = Preferences.instance.lastSelectedDifficulty;
			difficultySlider.maxValue = highestDifficultyBeaten + 1;
			if(lastSelectedDifficulty < 0)
			{
				lastSelectedDifficulty = 0;
			}
			if(lastSelectedDifficulty > 9)
			{
				lastSelectedDifficulty  = 9;
			}
			difficultySlider.value = lastSelectedDifficulty;
			variantSimple.UpdateVariantSimpleForVariant(new Variant(difficultyVariantStrings[lastSelectedDifficulty]));
		}
	}
	
	public int GetDifficulty()
	{
		if(!visibilityObject.activeSelf)
		{
			return 0;
		}
		return Mathf.RoundToInt(difficultySlider.value);
	}
	
	public void DifficultySliderUpdated()
	{
		variantSimple.UpdateVariantSimpleForVariant(new Variant(difficultyVariantStrings[Mathf.RoundToInt(difficultySlider.value)]));
	}
}
