using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static Variant;
using UnityEngine.Events;

public class LoadVariantMenu : MonoBehaviour
{
    public ButtonPlus deleteButton;
    public ButtonPlus cancelButton;
    public ButtonPlus loadButton;
	public RectTransform contentRT;
	public ControllerSelectionGroup controllerSelectionGroup;
	public UnityEngine.UI.Scrollbar verticalScrollbar;
	
	public GameObject variantSimplePrefab;
	
	public float variantSimpleGap;
	public Vector2 variantSimpleSize;
	
	public List<VariantSimple> variantSimples;
	
	public static LoadVariantMenu instance;
	public VariantSimple selectedVariantSimple;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		if(selectedVariantSimple == null)
		{
			deleteButton.ChangeButtonEnabled(false);
			loadButton.ChangeButtonEnabled(false);
		}
		else
		{
			deleteButton.ChangeButtonEnabled(enabledState);
			loadButton.ChangeButtonEnabled(enabledState);
		}
		cancelButton.ChangeButtonEnabled(enabledState);
		for(int i = 0; i < variantSimples.Count; i++)
		{
			variantSimples[i].variantDescriptionButton.ChangeButtonEnabled(enabledState);
			variantSimples[i].variantButton.ChangeButtonEnabled(enabledState);
		}
	}
	
	public void SpawnVariantSimples()
	{
		for(int i = variantSimples.Count - 1; i >= 0; i--)
		{
			if(controllerSelectionGroup.controllerSelectableObjects.Contains(variantSimples[i].variantButtonControllerSelectableObject))
			{
				controllerSelectionGroup.controllerSelectableObjects.Remove(variantSimples[i].variantButtonControllerSelectableObject);
			}
			if(controllerSelectionGroup.controllerSelectableObjects.Contains(variantSimples[i].variantDescriptionButtonControllerSelectableObject))
			{
				controllerSelectionGroup.controllerSelectableObjects.Remove(variantSimples[i].variantDescriptionButtonControllerSelectableObject);
			}
			Destroy(variantSimples[i].gameObject);
		}
		variantSimples.Clear();
		selectedVariantSimple = null;
		string variantsDirectory = $"{LocalInterface.instance.localFilesDirectory}Variants/";
		DirectoryInfo variantsDirectoryInfo = new DirectoryInfo(variantsDirectory);
		FileInfo[] variantsFileInfo = variantsDirectoryInfo.GetFiles();
		foreach(FileInfo file in variantsFileInfo)
		{
			string variantName = file.Name.Substring(0, file.Name.Length - 4);
			string variantText = VariantsMenu.instance.GetVariantFile(variantName);
			string[] variantLines = variantText.Split('\n');
			string[] variantBasics = variantLines[0].Split('%');
			string variantDescription = variantBasics[2];
			Color variantSpriteColor = LocalInterface.instance.ParseColor(variantBasics[3]);
			string variantSpriteCategory = variantBasics[4];
			int variantSpriteIndex = int.Parse(variantBasics[5]);
			GameObject newVariantSimpleGO = Instantiate(variantSimplePrefab, contentRT);
			newVariantSimpleGO.name = variantName;
			VariantSimple newVariantSimple = newVariantSimpleGO.GetComponent<VariantSimple>();
			newVariantSimple.variantText = variantText;
			switch(variantSpriteCategory)
			{
				case "Variant":
					newVariantSimple.variantIcon.sprite = VariantsMenu.instance.variantImages[variantSpriteIndex];
					break;
				case "Bauble":
					newVariantSimple.variantIcon.sprite = VariantsMenu.instance.baubleImages[variantSpriteIndex];
					break;
				case "SpecialCard":
					newVariantSimple.variantIcon.sprite = VariantsMenu.instance.specialCardImages[variantSpriteIndex];
					break;
			}
			newVariantSimple.variantIcon.color = variantSpriteColor;
			newVariantSimple.variantNameLabel.ChangeText(variantName);
			newVariantSimple.variantDescriptionButton.ChangeButtonText(variantDescription);
			newVariantSimple.variant = new Variant(variantText);
			variantSimples.Add(newVariantSimple);
			controllerSelectionGroup.controllerSelectableObjects.Add(newVariantSimple.variantButtonControllerSelectableObject);
			controllerSelectionGroup.controllerSelectableObjects.Add(newVariantSimple.variantDescriptionButtonControllerSelectableObject);
			newVariantSimple.variantButtonControllerSelectableObject.scrollViewContentRT = contentRT;
			newVariantSimple.variantDescriptionButtonControllerSelectableObject.scrollViewContentRT = contentRT;
			newVariantSimple.variantButtonControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
			newVariantSimple.variantDescriptionButtonControllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
		}
		variantSimples.Sort((x, y) =>
		{
			return x.variantNameLabel.GetText().CompareTo(y.variantNameLabel.GetText());
		});
		for(int i = 0; i < variantSimples.Count; i++)
		{
			variantSimples[i].rt.anchoredPosition = new Vector2(variantSimpleGap, -variantSimpleGap - i * (variantSimpleGap + variantSimpleSize.y));
			variantSimples[i].variantButtonControllerSelectableObject.positionInScrollView = variantSimples[i].rt.anchoredPosition.y - 10f;
			variantSimples[i].variantDescriptionButtonControllerSelectableObject.positionInScrollView = variantSimples[i].rt.anchoredPosition.y - 10f;
		}
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, variantSimpleGap + variantSimples.Count * (variantSimpleGap + variantSimpleSize.y));
	}
	
	public void CancelButtonClicked()
	{
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["LoadVariantMenu"].StartMove("OffScreen");
	}
	
	public void DeleteButtonClicked()
	{
		OptionsDialog.instance.SetupDialog($"Are you sure you want to delete {selectedVariantSimple.variant.variantName}?", new string[1]{"Yes"}, new ThemeManager.UIElementType[1]{ThemeManager.UIElementType.warningButtonActive}, new UnityAction[1]{DeleteConfirmed});
	}
	
	public void DeleteConfirmed()
	{
		LocalInterface.instance.DeleteFile($"Variants/{selectedVariantSimple.variant.variantName}");
		SpawnVariantSimples();
		OptionsDialog.instance.SetVisibility(false);
	}
	
	public void LoadButtonClicked()
	{
		VariantsMenu.instance.loadedVariant = new Variant(selectedVariantSimple.variantText);
		VariantsMenu.instance.loadedVariantBeforeChanges = new Variant(selectedVariantSimple.variantText);
		VariantsMenu.instance.loadedVariantSimple.UpdateVariantSimpleForVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		MovingObjects.instance.mo["LoadVariantMenu"].StartMove("OffScreen");
	}
	
	public void VariantSimpleClicked(VariantSimple variantSimple)
	{
		if(selectedVariantSimple != null)
		{
			selectedVariantSimple.variantButton.ChangeSpecialState(false);
		}
		variantSimple.variantButton.ChangeSpecialState(true);
		selectedVariantSimple = variantSimple;
		SetInteractability(true);
	}
	
	public void VariantSimpleDoubleClicked(VariantSimple variantSimple)
	{
		if(selectedVariantSimple != null)
		{
			selectedVariantSimple.variantButton.ChangeSpecialState(false);
		}
		variantSimple.variantButton.ChangeSpecialState(true);
		selectedVariantSimple = variantSimple;
		SetInteractability(true);
		LoadButtonClicked();
	}
}
