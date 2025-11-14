using UnityEngine;
using UnityEngine.UI;
using static ThemeManager;

public class ThemedUIElement : MonoBehaviour
{
    public UIElementType elementType;
	public Image image;
	public Label label;
	
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
        if(image != null)
		{
			image.color = ThemeManager.instance.GetColorFromCurrentTheme(elementType);
		}
		if(label != null)
		{
			label.ChangeColor(ThemeManager.instance.GetColorFromCurrentTheme(elementType), ThemeManager.instance.GetColorFromCurrentTheme(UIElementType.shadow));
		}
    }
}