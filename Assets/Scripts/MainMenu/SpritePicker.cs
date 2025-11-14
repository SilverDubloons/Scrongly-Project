using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpritePicker : MonoBehaviour
{
    public ButtonPlus cancelButton;
	public ButtonPlus colorButton;
	public Image referenceImage;
	public RectTransform contentRT;
	public GameObject spriteButtonPrefab;
	public ControllerSelectionGroup controllerSelectionGroup;
	public Scrollbar verticalScrollbar;
	
	public int spriteButtonsWide;
	public float spriteButtonsGap;
	public Vector2 spriteButtonSize;
	
	public List<SpriteButton> spriteButtons = new List<SpriteButton>();
	
	public string spriteCategory;
	public int spriteIndex;
	
	public static SpritePicker instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetInteractability(bool enabledState)
	{
		cancelButton.ChangeButtonEnabled(enabledState);
		colorButton.ChangeButtonEnabled(enabledState);
		for(int i = 0; i < spriteButtons.Count; i++)
		{
			spriteButtons[i].SetInteractability(enabledState);
		}
	}
	
	public void GenerateButtonsForSpriteArray(Sprite[] spriteArray, string spriteType, bool ignoreEdges = true)
	{
		for(int i = 0; i < spriteArray.Length; i++)
		{
			if(!ignoreEdges || (i % 16 != 0 && i > 16))
			{
				Texture2D texture = spriteArray[i].texture;
				Rect spriteRect = spriteArray[i].textureRect;
				// Color[] pixels = texture.GetPixels();
				Color[] pixels = texture.GetPixels
				(
					(int)spriteRect.x,
					(int)spriteRect.y,
					(int)spriteRect.width,
					(int)spriteRect.height
				);
				// Debug.Log($"{spriteType}, i={i}, spriteArray[i].name={spriteArray[i].name} pixels.Length={pixels.Length}");
				bool isEmpty = true;
				for(int j = 0; j < pixels.Length; j++)
				{
					if (pixels[j].a != 0f) // if any pixel is not transparent
					{
						// Debug.Log($"pixelIndex={pixelIndex} is not transparent, pixel.a={pixel.a}");
						isEmpty = false;
						break;
					}
				}
				if(!isEmpty)
				{
					GameObject newSpriteButtonGO = Instantiate(spriteButtonPrefab, contentRT);
					newSpriteButtonGO.name = spriteArray[i].name;
					SpriteButton newSpriteButton = newSpriteButtonGO.GetComponent<SpriteButton>();
					newSpriteButton.image.sprite = spriteArray[i];
					newSpriteButton.spriteType = spriteType;
					newSpriteButton.spriteIndex = i;
					spriteButtons.Add(newSpriteButton);
				}
			}
		}
	}
	
	public void SetupSpritePicker()
	{
		GenerateButtonsForSpriteArray(VariantsMenu.instance.variantImages, "Variant");
		GenerateButtonsForSpriteArray(VariantsMenu.instance.baubleImages, "Bauble");
		GenerateButtonsForSpriteArray(VariantsMenu.instance.specialCardImages, "SpecialCard", false);
		for(int i = 0; i < spriteButtons.Count; i++)
		{
			spriteButtons[i].rt.anchoredPosition = new Vector2(spriteButtonsGap + (i % spriteButtonsWide) * (spriteButtonsGap + spriteButtonSize.x), -spriteButtonsGap - (i / spriteButtonsWide) * (spriteButtonsGap + spriteButtonSize.y));
			controllerSelectionGroup.controllerSelectableObjects.Add(spriteButtons[i].button.controllerSelectableObject);
			spriteButtons[i].button.controllerSelectableObject.scrollViewContentRT = contentRT;
			spriteButtons[i].button.controllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
			spriteButtons[i].button.controllerSelectableObject.positionInScrollView = spriteButtons[i].rt.anchoredPosition.y - 10f;
		}
		contentRT.sizeDelta = new Vector2(contentRT.sizeDelta.x, spriteButtonsGap + ((spriteButtons.Count + spriteButtonsWide - 1) / spriteButtonsWide) * (spriteButtonsGap + spriteButtonSize.y));
	}
	
	public void CancelButtonClicked()
	{
		referenceImage.sprite = VariantsMenu.instance.loadedVariant.variantSprite;
		referenceImage.color = VariantsMenu.instance.loadedVariant.variantSpriteColor;
		spriteCategory = VariantsMenu.instance.loadedVariant.variantSpriteCategory;
		spriteIndex = VariantsMenu.instance.loadedVariant.variantSpriteIndex;
		MovingObjects.instance.mo["SpritePicker"].StartMove("OffScreen");
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OnScreen");
	}
	
	public void ConfirmButtonClicked()
	{
		VariantDetailsInput.instance.spriteButtonImage.sprite = referenceImage.sprite;
		VariantDetailsInput.instance.spriteButtonImage.color = referenceImage.color;
		VariantDetailsInput.instance.spriteCategory = spriteCategory;
		VariantDetailsInput.instance.spriteIndex = spriteIndex;
		MovingObjects.instance.mo["SpritePicker"].StartMove("OffScreen");
		MovingObjects.instance.mo["VariantDetailsInput"].StartMove("OnScreen");
	}
	
	public void ColorButtonClicked()
	{
		MovingObjects.instance.mo["SpritePicker"].StartMove("OffScreen");
		MovingObjects.instance.mo["ColorPicker"].StartMove("OnScreen");
		ColorPicker.instance.SetColorPicker(referenceImage.sprite, referenceImage.color, ColorPickerCancelClicked, ColorPickerConfirmClicked);
	}
	
	public void ColorPickerConfirmClicked()
	{
		referenceImage.color = ColorPicker.instance.GetColor();
		MovingObjects.instance.mo["SpritePicker"].StartMove("OnScreen");
		MovingObjects.instance.mo["ColorPicker"].StartMove("OffScreen");
	}
	
	public void ColorPickerCancelClicked()
	{
		ColorPicker.instance.SetColorPicker(referenceImage.sprite, referenceImage.color);
		MovingObjects.instance.mo["SpritePicker"].StartMove("OnScreen");
		MovingObjects.instance.mo["ColorPicker"].StartMove("OffScreen");
	}
}
