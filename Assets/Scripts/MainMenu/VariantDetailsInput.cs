using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class VariantDetailsInput : MonoBehaviour
{
	public ButtonPlus cancelButton;
	public ButtonPlus confirmButton;
	public TMP_InputField nameInput;
	public TMP_InputField descriptionInput;
	public ButtonPlus spriteButton;
	public Image spriteButtonImage;
	public string spriteCategory;
	public int spriteIndex;
	public string oldVariantName;
	public bool makingNewVariant;
	
	public static VariantDetailsInput instance;
	
	void Awake()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		cancelButton.ChangeButtonEnabled(enabledState);
		confirmButton.ChangeButtonEnabled(enabledState);
		nameInput.interactable = enabledState;
		descriptionInput.interactable = enabledState;
		spriteButton.ChangeButtonEnabled(enabledState);
	}
	
	public void SetDetailsToVariant(Variant variant)
	{
		nameInput.text = variant.variantName;
		descriptionInput.text = variant.variantDescription;
		// spriteButtonImage.sprite = variant.variantSprite;
		spriteButtonImage.sprite = VariantsMenu.instance.GetVariantSprite(variant.variantSpriteCategory, variant.variantSpriteIndex);
		spriteButtonImage.color = variant.variantSpriteColor;
		spriteCategory = variant.variantSpriteCategory;
		spriteIndex = variant.variantSpriteIndex;
	}
	
	public void SetDetailsToNewVariant()
	{
		nameInput.text = string.Empty;
		descriptionInput.text = string.Empty;
		spriteButtonImage.sprite = VariantsMenu.instance.variantImages[17];
		spriteButtonImage.color = Color.red;
		spriteCategory = "Variant";
		spriteIndex = 17;
	}
	
    public void SpriteButtonClicked()
	{
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["SpritePicker"].StartMove("OnScreen");
		SpritePicker.instance.referenceImage.sprite = spriteButtonImage.sprite;
		SpritePicker.instance.referenceImage.color = spriteButtonImage.color;
	}
	
	public void CancelButtonClicked()
	{
		SetDetailsToVariant(VariantsMenu.instance.loadedVariant);
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
	}
	
	public void ConfirmButtonClicked()
	{
		nameInput.text = nameInput.text.Trim();
		string existingFileText = VariantsMenu.instance.GetVariantFile(nameInput.text);
		if(existingFileText == null)
		{
			if(makingNewVariant)
			{
				VariantsMenu.instance.loadedVariant = new Variant(LocalInterface.instance.baseVariant);
			}
			UpdateLoadedVariant();
			MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OffScreen");
			MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
			MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
			MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
			VariantsMenu.instance.SaveOverClicked();
			if(oldVariantName != string.Empty)
			{
				VariantsMenu.instance.DeleteVariant(oldVariantName);
			}
			
		}
		else
		{
			OptionsDialog.instance.SetupDialog($"Would you Like to Save Over {nameInput.text}?", new string[1]{"Yes"}, new ThemeManager.UIElementType[1]{ThemeManager.UIElementType.warningButtonActive}, new UnityAction[1]{SaveOverClicked});
		}
	}
	
	public void UpdateLoadedVariant()
	{
		VariantsMenu.instance.loadedVariant.variantSprite = spriteButtonImage.sprite;
		VariantsMenu.instance.loadedVariant.variantSpriteColor = spriteButtonImage.color;
		VariantsMenu.instance.loadedVariant.variantName = nameInput.text;
		VariantsMenu.instance.loadedVariant.variantDescription = descriptionInput.text;
		VariantsMenu.instance.loadedVariant.variantSpriteCategory = spriteCategory;
		VariantsMenu.instance.loadedVariant.variantSpriteIndex = spriteIndex;
		VariantsMenu.instance.loadedVariantSimple.UpdateVariantSimpleForVariant(VariantsMenu.instance.loadedVariant);
	}
	
	public void SaveOverClicked()
	{
		if(makingNewVariant)
		{
			VariantsMenu.instance.loadedVariant = new Variant(LocalInterface.instance.baseVariant);
		}
		UpdateLoadedVariant();
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OffScreen");
		MovingObjects.instance.mo["VariantsMenu"].StartMove("OnScreen");
		MovingObjects.instance.mo["SeedInput"].StartMove("OnScreen");
		MovingObjects.instance.mo["DeckPicker"].StartMove("OnScreenVariant");
		VariantsMenu.instance.SaveOverClicked();
		if(oldVariantName != string.Empty)
		{
			if(oldVariantName != nameInput.text)
			{
				VariantsMenu.instance.DeleteVariant(oldVariantName);
			}
		}
	}
}
