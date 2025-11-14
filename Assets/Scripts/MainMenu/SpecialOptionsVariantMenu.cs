using UnityEngine;
using System.Collections.Generic;
using static Variant;
using UnityEngine.UI;

public class SpecialOptionsVariantMenu : MonoBehaviour
{
    public ButtonPlus defaultButton;
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
	
	public ControllerSelectionGroup controllerSelectionGroup;
	
	public Transform specialOptionsVariantParent;
	public RectTransform specialOptionsVariantContentRectTransform;
	public Scrollbar verticalScrollbar;
	public GameObject specialOptionTogglePrefab;
	public GameObject specialOptionSliderPrefab;
	public GameObject specialOptionInputFieldPrefab;
	
	public int specialOptionsVariantWide;
	public float distanceBetweenSpecialOptionsVariants;
	public Vector2 specialOptionToggleSize;
	public Vector2 specialOptionSliderSize;
	public Vector2 specialOptionInputFieldSize;
	
	// public bool optionHasChanged;
	
	public static SpecialOptionsVariantMenu instance;
/* 	public List<SpecialOptionToggle> specialOptionToggles = new List<SpecialOptionToggle>();
	public List<SpecialOptionSlider> specialOptionSliders = new List<SpecialOptionSlider>();
	public List<SpecialOptionInputField> specialOptionInputFields = new List<SpecialOptionInputField>(); */
	public Dictionary<string, SpecialOptionToggle> specialOptionToggles = new Dictionary<string, SpecialOptionToggle>();
	public Dictionary<string, SpecialOptionSlider> specialOptionSliders = new Dictionary<string, SpecialOptionSlider>();
	public Dictionary<string, SpecialOptionInputField> specialOptionInputFields = new Dictionary<string, SpecialOptionInputField>();
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetSpecialOptionsVariantMenuButtons(bool enabledState)
	{
		confirmButton.ChangeButtonEnabled(enabledState);
		defaultButton.ChangeButtonEnabled(enabledState);
		cancelButton.ChangeButtonEnabled(enabledState);	
		foreach(KeyValuePair<string, SpecialOptionToggle> entry in specialOptionToggles)
		{
			entry.Value.SetInteractability(enabledState);
		}
		foreach(KeyValuePair<string, SpecialOptionSlider> entry in specialOptionSliders)
		{
			entry.Value.SetInteractability(enabledState);
		}
		foreach(KeyValuePair<string, SpecialOptionInputField> entry in specialOptionInputFields)
		{
			entry.Value.SetInteractability(enabledState);
		}
	}
	
	public struct SpecialOptionOrderHelper
	{
		public RectTransform rt;
		public string stringToOrderBy;
		public Vector2 prefabSize;
		public SpecialOptionOrderHelper(RectTransform rt, string stringToOrderBy, Vector2 prefabSize)
		{
			this.rt = rt;
			this.stringToOrderBy = stringToOrderBy;
			this.prefabSize = prefabSize;
		}
	}
	
	public struct SpecialOptionColumn
	{
		public float columnSize;
		public int columnInt;
		public SpecialOptionColumn(float columnSize, int columnInt)
		{
			this.columnSize = columnSize;
			this.columnInt = columnInt;
		}
	}
	
	public void SetupSpecialOptionsVariantMenu(Variant variant)
	{
		List<SpecialOptionOrderHelper> specialOptionOrderHelpers = new List<SpecialOptionOrderHelper>();
		foreach(KeyValuePair<string, VariantSpecialOption> entry in variant.variantSpecialOptions)
		{
			switch(entry.Value.type)
			{
				case "Toggle":
					GameObject newSpecialOptionToggleGO = Instantiate(specialOptionTogglePrefab, Vector3.zero, Quaternion.identity, specialOptionsVariantParent);
					newSpecialOptionToggleGO.name = entry.Value.tag;
					SpecialOptionToggle newSpecialOptionToggle = newSpecialOptionToggleGO.GetComponent<SpecialOptionToggle>();
					specialOptionToggles.Add(entry.Value.tag, newSpecialOptionToggle);
					newSpecialOptionToggle.SetupSpecialOptionToggle(entry.Value.inEffect, entry.Value.label, entry.Value.tag);
					specialOptionOrderHelpers.Add(new SpecialOptionOrderHelper(newSpecialOptionToggle.rt, entry.Value.label, specialOptionToggleSize));
					controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialOptionToggle.controllerSelectableObject);
					break;
				case "Slider":
					GameObject newSpecialOptionSliderGO = Instantiate(specialOptionSliderPrefab, Vector3.zero, Quaternion.identity, specialOptionsVariantParent);
					newSpecialOptionSliderGO.name = entry.Value.tag;
					SpecialOptionSlider newSpecialOptionSlider = newSpecialOptionSliderGO.GetComponent<SpecialOptionSlider>();
					specialOptionSliders.Add(entry.Value.tag, newSpecialOptionSlider);
					newSpecialOptionSlider.SetupSpecialOptionSlider(entry.Value.range.x, entry.Value.range.y, entry.Value.impact, entry.Value.label, entry.Value.tag);
					specialOptionOrderHelpers.Add(new SpecialOptionOrderHelper(newSpecialOptionSlider.rt, entry.Value.label, specialOptionSliderSize));
					controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialOptionSlider.controllerSelectableObject);
					break;
				case "InputField":
					GameObject newSpecialOptionsInputFieldGO = Instantiate(specialOptionInputFieldPrefab, Vector3.zero, Quaternion.identity, specialOptionsVariantParent);
					newSpecialOptionsInputFieldGO.name = entry.Value.tag;
					SpecialOptionInputField newSpecialOptionInputField = newSpecialOptionsInputFieldGO.GetComponent<SpecialOptionInputField>();
					specialOptionInputFields.Add(entry.Value.tag, newSpecialOptionInputField);
					newSpecialOptionInputField.SetupSpecialOptionInputField(entry.Value.range.x, entry.Value.range.y, entry.Value.impact, entry.Value.label, entry.Value.tag);
					specialOptionOrderHelpers.Add(new SpecialOptionOrderHelper(newSpecialOptionInputField.rt, entry.Value.label, specialOptionInputFieldSize));
					controllerSelectionGroup.controllerSelectableObjects.Add(newSpecialOptionInputField.controllerSelectableObject);
					break;
			}
		}
		specialOptionOrderHelpers.Sort((x, y) =>
		{
			return x.stringToOrderBy.CompareTo(y.stringToOrderBy);
		});
		List<SpecialOptionColumn> specialOptionColumns = new List<SpecialOptionColumn>();
		for(int i = 0; i < specialOptionsVariantWide; i++)
		{
			specialOptionColumns.Add(new SpecialOptionColumn(0, i));
		}
		float largestColumn = 0;
		for(int i = 0; i < specialOptionOrderHelpers.Count; i++)
		{
			specialOptionColumns.Sort((x, y) =>
			{
				if(Mathf.Abs(x.columnSize - y.columnSize) > LocalInterface.instance.epsilon)
				{
					return y.columnSize.CompareTo(x.columnSize);
				}
				return x.columnInt.CompareTo(y.columnInt);
			});
			int column = specialOptionColumns[0].columnInt;
			float yPos = specialOptionColumns[0].columnSize;
			specialOptionOrderHelpers[i].rt.anchoredPosition = new Vector2(distanceBetweenSpecialOptionsVariants + (distanceBetweenSpecialOptionsVariants + specialOptionOrderHelpers[i].prefabSize.x) * column, -distanceBetweenSpecialOptionsVariants + yPos);
			SpecialOptionColumn tempSpecialOptionColumn = specialOptionColumns[0];
			tempSpecialOptionColumn.columnSize -= (specialOptionOrderHelpers[i].prefabSize.y + distanceBetweenSpecialOptionsVariants);
			if(Mathf.Abs(tempSpecialOptionColumn.columnSize) > Mathf.Abs(largestColumn))
			{
				largestColumn = Mathf.Abs(tempSpecialOptionColumn.columnSize);
			}
			specialOptionColumns[0] = tempSpecialOptionColumn;
		}
		specialOptionsVariantContentRectTransform.sizeDelta = new Vector2(specialOptionsVariantContentRectTransform.sizeDelta.x, largestColumn + distanceBetweenSpecialOptionsVariants);
	}
	
/* 	public void OptionHasChanged()
	{
		if(optionHasChanged)
		{
			return;
		}
		optionHasChanged = true;
		confirmButton.ChangeButtonEnabled(true);
	} */
	
/* 	public void ResetSpecialOptionsChangedStatus()
	{
		foreach(KeyValuePair<string, SpecialOptionToggle> entry in specialOptionToggles)
		{
			entry.Value.hasChanged = false;
		}
		foreach(KeyValuePair<string, SpecialOptionSlider> entry in specialOptionSliders)
		{
			entry.Value.hasChanged = false;
		}
		foreach(KeyValuePair<string, SpecialOptionInputField> entry in specialOptionInputFields)
		{
			entry.Value.hasChanged = false;
		}
	} */
	
	public void SetSpecialOptionsToVariant(Variant variant)
	{
		foreach(KeyValuePair<string, SpecialOptionToggle> entry in specialOptionToggles)
		{
			entry.Value.setupComplete = false;
			entry.Value.UpdateToggle(variant.variantSpecialOptions[entry.Value.specialOptionTag].inEffect);
			entry.Value.setupComplete = true;
		}
		foreach(KeyValuePair<string, SpecialOptionSlider> entry in specialOptionSliders)
		{
			entry.Value.setupComplete = false;
			entry.Value.UpdateSlider(variant.variantSpecialOptions[entry.Value.specialOptionTag].impact);
			entry.Value.setupComplete = true;
		}
		foreach(KeyValuePair<string, SpecialOptionInputField> entry in specialOptionInputFields)
		{
			entry.Value.setupComplete = false;
			entry.Value.UpdateInputField(variant.variantSpecialOptions[entry.Value.specialOptionTag].impact);
			entry.Value.setupComplete = true;
		}
	}
	
	public void DefaultButtonClicked()
	{
		SetSpecialOptionsToVariant(VariantsMenu.instance.baseVariant);
	}
	
	public void CancelButtonClicked()
	{
		SetSpecialOptionsToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["SpecialOptionsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		// controllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public void ConfirmButtonClicked()
	{
		foreach(KeyValuePair<string, SpecialOptionToggle> entry in specialOptionToggles)
		{
			VariantSpecialOption tempVariantSpecialOption = VariantsMenu.instance.loadedVariant.variantSpecialOptions[entry.Value.specialOptionTag];
			tempVariantSpecialOption.inEffect = entry.Value.isOn;
			VariantsMenu.instance.loadedVariant.variantSpecialOptions[entry.Value.specialOptionTag] = tempVariantSpecialOption;
		}
		foreach(KeyValuePair<string, SpecialOptionSlider> entry in specialOptionSliders)
		{
			VariantSpecialOption tempVariantSpecialOption = VariantsMenu.instance.loadedVariant.variantSpecialOptions[entry.Value.specialOptionTag];
			tempVariantSpecialOption.impact = entry.Value.val;
			VariantsMenu.instance.loadedVariant.variantSpecialOptions[entry.Value.specialOptionTag] = tempVariantSpecialOption;
		}
		foreach(KeyValuePair<string, SpecialOptionInputField> entry in specialOptionInputFields)
		{
			VariantSpecialOption tempVariantSpecialOption = VariantsMenu.instance.loadedVariant.variantSpecialOptions[entry.Value.specialOptionTag];
			tempVariantSpecialOption.impact = entry.Value.val;
			VariantsMenu.instance.loadedVariant.variantSpecialOptions[entry.Value.specialOptionTag] = tempVariantSpecialOption;
		}
		MovingObjects.instance.mo["SpecialOptionsVariantMenu"].StartMove("OffScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		// controllerSelectionGroup.RemoveFromCurrentGroups();
	}
}
