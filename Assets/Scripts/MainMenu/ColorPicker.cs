using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPicker : MonoBehaviour
{
   public ButtonPlus cancelButton;
   public ButtonPlus confirmButton;
   public Slider R_Slider;
   public Slider G_Slider;
   public Slider B_Slider;
   public RawImage R_Image;
   public RawImage G_Image;
   public RawImage B_Image;
   public Image referenceImage;
   public int textureWidth;
   
   public static ColorPicker instance;
   
   void Awake()
   {
	   instance = this;
   }
   
   public void SetInteractability(bool enabledState)
   {
	   cancelButton.ChangeButtonEnabled(enabledState);
	   confirmButton.ChangeButtonEnabled(enabledState);
	   R_Slider.interactable = enabledState;
	   G_Slider.interactable = enabledState;
	   B_Slider.interactable = enabledState;
   }
   
   public void SetColorPicker(Sprite imageSprite, Color imageColor, UnityAction cancelAction, UnityAction confirmAction)
   {
	   referenceImage.sprite = imageSprite;
	   R_Slider.value = imageColor.r;
	   G_Slider.value = imageColor.g;
	   B_Slider.value = imageColor.b;
	   SliderUpdated();
	   cancelButton.ChangeButtonEvent(cancelAction);
	   confirmButton.ChangeButtonEvent(confirmAction);
   }
   
   public void SetColorPicker(Sprite imageSprite, Color imageColor)
   {
	   referenceImage.sprite = imageSprite;
	   R_Slider.value = imageColor.r;
	   G_Slider.value = imageColor.g;
	   B_Slider.value = imageColor.b;
	   SliderUpdated();
   }
   
   public void SliderUpdated()
   {
	   Color currentColor = new Color(R_Slider.value, G_Slider.value, B_Slider.value);
	   referenceImage.color = currentColor;
	   UpdateSliderGradient(R_Image, 
            new Color(0f, G_Slider.value, B_Slider.value), 
            new Color(1f, G_Slider.value, B_Slider.value));

        UpdateSliderGradient(G_Image, 
            new Color(R_Slider.value, 0f, B_Slider.value), 
            new Color(R_Slider.value, 1f, B_Slider.value));

        UpdateSliderGradient(B_Image, 
            new Color(R_Slider.value, G_Slider.value, 0f), 
            new Color(R_Slider.value, G_Slider.value, 1f));
   }
   
	private void UpdateSliderGradient(RawImage backgroundImage, Color startColor, Color endColor)
	{
        Gradient gradient = new Gradient();
        gradient.SetKeys
		(
            new GradientColorKey[] { new GradientColorKey(startColor, 0f), new GradientColorKey(endColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );

        Texture2D texture = new Texture2D(textureWidth, 1);
        for (int x = 0; x < textureWidth; x++)
        {
            float t = x / (float)(textureWidth - 1);
            Color color = gradient.Evaluate(t);
            texture.SetPixel(x, 0, color);
        }
        texture.Apply();
		
        backgroundImage.texture = texture;
	}
	
	public Color GetColor()
	{
		return referenceImage.color;
	}
}