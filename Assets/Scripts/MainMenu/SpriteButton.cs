using UnityEngine;
using UnityEngine.UI;

public class SpriteButton : MonoBehaviour
{
	public RectTransform rt;
    public ButtonPlus button;
	public Image image;
	public string spriteType;
	public int spriteIndex;
	
	public void SetInteractability(bool enabledState)
	{
		button.ChangeButtonEnabled(enabledState);
	}
	
	public void ButtonClicked()
	{
		SpritePicker.instance.referenceImage.sprite = image.sprite;
		SpritePicker.instance.spriteCategory = spriteType;
		SpritePicker.instance.spriteIndex = spriteIndex;
	}
}
