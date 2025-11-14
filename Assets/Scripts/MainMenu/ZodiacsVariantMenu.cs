using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Variant;

public class ZodiacsVariantMenu : MonoBehaviour
{
	public ButtonPlus defaultButton;
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
    public RectTransform zodiacVariantOptionsParent;
	public RectTransform zodiacVariantOptionsContentRectTransform;
	public GameObject zodiacVariantOptionsPrefab;
	public ControllerSelectionGroup controllerSelectionGroup;
	public Scrollbar verticalScrollbar;
	
    public int reasonableMaximumZodiacCost;
	public double reasonableMaximumZodiacImpact;
	public int zodiacVariantOptionsWide;
	public float distanceBetweenZodiacVariantOptions;
	public Vector2 zodiacVariantOptionsSize;
	
	public List<ZodiacVariantOptions> zodiacVariantOptions = new List<ZodiacVariantOptions>();
	
	public static ZodiacsVariantMenu instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		for(int i = 0; i < zodiacVariantOptions.Count; i++)
		{
			zodiacVariantOptions[i].SetInteractability(enabledState);
		}
		confirmButton.ChangeButtonEnabled(enabledState);
		defaultButton.ChangeButtonEnabled(enabledState);
		cancelButton.ChangeButtonEnabled(enabledState);	
	}
	
	public void SetupZodiacsVariantMenu(Variant variant)
	{
		foreach(KeyValuePair<string, VariantBauble> entry in variant.variantBaubles)
		{
			if(entry.Value.category == "Zodiac")
			{
				GameObject newZodiacVariantOptionsGO = Instantiate(zodiacVariantOptionsPrefab, zodiacVariantOptionsParent);
				newZodiacVariantOptionsGO.name = entry.Value.tag;
				ZodiacVariantOptions newZodiacVariantOptions = newZodiacVariantOptionsGO.GetComponent<ZodiacVariantOptions>();
				zodiacVariantOptions.Add(newZodiacVariantOptions);
				newZodiacVariantOptions.SetupZodiacVariantOptions(entry.Value.sprite, entry.Value.tag, int.Parse(entry.Value.tag.Substring(4, 2)), entry.Value.menuDescription, entry.Value.baubleName);
				controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacVariantOptions.costInputFieldControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacVariantOptions.pointsInputFieldControllerSelectableObject);
				controllerSelectionGroup.controllerSelectableObjects.Add(newZodiacVariantOptions.multiplierInputFieldControllerSelectableObject);
				newZodiacVariantOptions.costInputFieldControllerSelectableObject.scrollViewContentRT = zodiacVariantOptionsParent;
				newZodiacVariantOptions.pointsInputFieldControllerSelectableObject.scrollViewContentRT = zodiacVariantOptionsParent;
				newZodiacVariantOptions.multiplierInputFieldControllerSelectableObject.scrollViewContentRT = zodiacVariantOptionsParent;
				newZodiacVariantOptions.costInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newZodiacVariantOptions.pointsInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
				newZodiacVariantOptions.multiplierInputFieldControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
			}
		}
		zodiacVariantOptions.Sort((x, y) =>
		{
			return x.handNumber.CompareTo(y.handNumber);
		});
		for(int i = 0;i < zodiacVariantOptions.Count; i++)
		{
			zodiacVariantOptions[i].rt.anchoredPosition = new Vector2(distanceBetweenZodiacVariantOptions + (i % zodiacVariantOptionsWide) * (distanceBetweenZodiacVariantOptions + zodiacVariantOptionsSize.x), -distanceBetweenZodiacVariantOptions - (i / zodiacVariantOptionsWide) * (distanceBetweenZodiacVariantOptions + zodiacVariantOptionsSize.y));
			zodiacVariantOptions[i].costInputFieldControllerSelectableObject.positionInScrollView = zodiacVariantOptions[i].rt.anchoredPosition.y -10f;
			zodiacVariantOptions[i].pointsInputFieldControllerSelectableObject.positionInScrollView = zodiacVariantOptions[i].rt.anchoredPosition.y - 10f;
			zodiacVariantOptions[i].multiplierInputFieldControllerSelectableObject.positionInScrollView = zodiacVariantOptions[i].rt.anchoredPosition.y - 10f;
		}
		zodiacVariantOptionsContentRectTransform.sizeDelta = new Vector2(zodiacVariantOptionsContentRectTransform.sizeDelta.x, distanceBetweenZodiacVariantOptions + (zodiacVariantOptions.Count / zodiacVariantOptionsWide) * (distanceBetweenZodiacVariantOptions + zodiacVariantOptionsSize.y));
	}
	
	public void SetZodiacsVariantMenuToVariant(Variant variant)
	{
		for(int i = 0;i < zodiacVariantOptions.Count; i++)
		{
			zodiacVariantOptions[i].SetZodiacVariantOptions(variant.variantBaubles[zodiacVariantOptions[i].zodiacTag].baseCost, variant.variantBaubles[zodiacVariantOptions[i].zodiacTag].impact1, variant.variantBaubles[zodiacVariantOptions[i].zodiacTag].impact2);
		}
	}
	
	public void DefaultButtonClicked()
	{
		SetZodiacsVariantMenuToVariant(VariantsMenu.instance.baseVariant);
	}
	
	public void CancelButtonClicked()
	{
		SetZodiacsVariantMenuToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["ZodiacsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
	
	public void ConfirmButtonClicked()
	{
		for(int i = 0;i < zodiacVariantOptions.Count; i++)
		{
			VariantBauble tempVariantBauble = VariantsMenu.instance.loadedVariant.variantBaubles[zodiacVariantOptions[i].zodiacTag];
			tempVariantBauble.baseCost = int.Parse(zodiacVariantOptions[i].costInputField.text);
			tempVariantBauble.impact1 = double.Parse(zodiacVariantOptions[i].pointsInputField.text);
			tempVariantBauble.impact2 = double.Parse(zodiacVariantOptions[i].multiplierInputField.text);
			VariantsMenu.instance.loadedVariant.variantBaubles[zodiacVariantOptions[i].zodiacTag] = tempVariantBauble;
		}
		MovingObjects.instance.mo["ZodiacsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
	}
}
