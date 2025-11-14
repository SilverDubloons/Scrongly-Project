using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControllerSelectableObject : MonoBehaviour
{
	public Color borderColor;
	public RectTransform rt;
    public bool isButton;
	public ButtonPlus buttonPlus;
	public bool buttonUsesImage;
	public RectTransform buttonImageRT;
	public Vector2 buttonImagePositionWithoutHotkey;
	public Vector2 buttonImagePositionWithHotkey;
	
	public bool isToggle;
	public Toggle toggle;
	
	public bool isSlider;
	public Slider slider;
	
	public bool isDropdown;
	public TMP_Dropdown dropdown;
	public Scrollbar dropdownScrollbar;
	
	public bool hasTooltip;
	public TooltipObject tooltipObject;
	
	public bool isElementInScrollView;
	public float positionInScrollView;
	public RectTransform scrollViewContentRT;
	public bool isScrollView;
	public Scrollbar scrollViewHorizontalScrollbar;
	public Scrollbar scrollViewVerticalScrollbar;
	
	public bool isInputField;
	public TMP_InputField inputField;
	public bool inputCanBeEmpty;
	public bool inputString;
	public string regexValidationString;
	public bool inputInt;
	public int minInputInt;
	public int maxInputInt;
	public bool inputFloat;
	public float minInputFloat;
	public float maxInputFloat;
	public bool inputDouble;
	public double minInputDouble;
	public double maxInputDouble;
	// public bool numberCanBeNegative;
	
	public bool isSpecial;
	public string specialTag;
	
	public bool isCard;
	public Card card;
	
	public bool isShopItem;
	public ShopItem shopItem;
	
	public bool isSlideOut;
	public SlideOut slideOut;
	
	public bool isHandInfoHand;
	public bool isHandInfoIndividual;
	public bool isHandInfoMinimum;
	public HandInfo handInfo;
	
	public bool unselectable;
	
	public bool hasHotkey;
	public string hotkeyActionName;
	public bool hotkeyBottom; // if true, hotkey image will be on the bottom of the button, otherwise will be on the right
	public bool changeFontSizeWithHotkey;
	public int fontSizeWithoutHotkey; 
	public int fontSizeWithHotkey;
	public Image hotkeyImage;
}
