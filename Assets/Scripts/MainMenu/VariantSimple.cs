using UnityEngine;
using UnityEngine.UI;

public class VariantSimple : MonoBehaviour
{
    public RectTransform rt;
	public Image variantIcon;
	public Image border;
	public Image interior;
	public Label variantNameLabel;
	public ButtonPlus variantDescriptionButton;
	public ButtonPlus variantButton;
	public ControllerSelectableObject variantButtonControllerSelectableObject;
	public ControllerSelectableObject variantDescriptionButtonControllerSelectableObject;
	
	public string variantText;
	public Variant variant;
	
	void Start()
	{
		ThemeManager.instance.OnThemeChanged += ApplyTheme;
        ApplyTheme();
	}
	
	void OnDestroy() 
	{
        if(ThemeManager.instance != null)
		{
            ThemeManager.instance.OnThemeChanged -= ApplyTheme;
		}
    }
	
	public void ApplyTheme()
	{
		if(border != null)
		{
			border.color = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.variantSimpleBorder);
		}
		if(interior != null)
		{
			interior.color = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.variantSimpleInterior);
		}
	}
	
	public void SetVariantButton(bool enabledState)
	{
		variantDescriptionButton.ChangeButtonEnabled(enabledState);
	}
	
	public void UpdateVariantSimpleForVariant(Variant variant)
	{
		if(variant.variantSprite != null)
		{
			variantIcon.sprite = variant.variantSprite;
		}
		else
		{
			variantIcon.sprite = VariantsMenu.instance.GetVariantSprite(variant.variantSpriteCategory, variant.variantSpriteIndex);
		}
		variantIcon.color = variant.variantSpriteColor;
		variantNameLabel.ChangeText(variant.variantName);
		variantDescriptionButton.ChangeButtonText(variant.variantDescription);
		this.variant = variant;
	}
	
	public void VariantSimpleClicked()
	{
		if(LoadVariantMenu.instance != null)
		{
			LoadVariantMenu.instance.VariantSimpleClicked(this);
		}
	}
	
	public void VariantSimpleDoubleClicked()
	{
		if(LocalInterface.instance.GetCurrentSceneName() == "MainMenuScene")
		{
			if(LoadVariantMenu.instance != null)
			{
				if(MovingObjects.instance.mo["LoadVariantMenu"].GetCurrentLocation() == "OnScreen")
				{
					LoadVariantMenu.instance.VariantSimpleDoubleClicked(this);
				}
			}
		}
	}
	
	public void VariantDescriptionButtonClicked()
	{
		if(LocalInterface.instance.GetCurrentSceneName() == "MainMenuScene")
		{
			if(MovingObjects.instance.mo["LoadVariantMenu"].GetCurrentLocation() == "OnScreen")
			{
				VariantSimpleClicked();
			}
			V.i.v = variant;
		}
		VariantExplainer.instance.DisplayVariant(variant.ConvertToText());
		// VariantExplainer.instance.DisplayVariant(variantText);
	}
}
