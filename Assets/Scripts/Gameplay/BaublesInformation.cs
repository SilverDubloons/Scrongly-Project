using UnityEngine;
using System.Collections.Generic;

public class BaublesInformation : MonoBehaviour
{
	public RectTransform baubleIconsBackdrop;
	public RectTransform baubleIconsContent;
	public GameObject baublesInformationObject;
	public GameObject tabObject;
	public GameObject backdropObject;
	public Label baublesLabel;
	public UnityEngine.UI.Scrollbar verticalScrollbar;
	
    public GameObject baubleIconPrefab;
	public SlideOut slideOut;
	
	public Dictionary<string, BaubleIcon> baubleIcons = new Dictionary<string, BaubleIcon>();
	
	public static BaublesInformation instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void BaublePurchased(string baubleTag, Vector2 origin)
	{
		bool firstObject = false;
		if(!baublesInformationObject.activeSelf)
		{
			firstObject = true;
			baublesInformationObject.SetActive(true);
		}
		if(baubleIcons.Count < 2)
		{
			firstObject = true;
		}
		if(baubleIcons.ContainsKey(baubleTag))
		{
			if(baubleTag == "Dice")
			{
				int diceAmount = Baubles.instance.GetQuantityOwned("Dice", true);
				baubleIcons[baubleTag].baubleImage.sprite = Baubles.instance.diceSprites[diceAmount + 9];
				baubleIcons[baubleTag].tooltipObject.mainText = Baubles.instance.diceDescriptions[diceAmount - 2];
				switch(diceAmount)
				{
					case 2:
						baubleIcons[baubleTag].tooltipObject.title = "D8";
					break;
					case 3:
						baubleIcons[baubleTag].tooltipObject.title = "D10";
					break;
					case 4:
						baubleIcons[baubleTag].tooltipObject.title = "D12";
					break;
					case 5:
						baubleIcons[baubleTag].tooltipObject.title = "D20";
					break;
					default:
						LocalInterface.instance.DisplayError($"BaublesInformation die upgrade failed. Case {diceAmount}");
					break;
				}
			}
			else
			{
				baubleIcons[baubleTag].UpdateBaubleIcon();
			}
			return;
		}
		GameObject newBaubleIconGO = Instantiate(baubleIconPrefab, baubleIconsContent);
		newBaubleIconGO.name = V.i.v.variantBaubles[baubleTag].baubleName;
		BaubleIcon newBaubleIcon = newBaubleIconGO.GetComponent<BaubleIcon>();
		newBaubleIcon.baubleImage.sprite = V.i.v.variantBaubles[baubleTag].sprite;
		newBaubleIcon.baubleTag = baubleTag;
		newBaubleIcon.tooltipObject.title = V.i.v.variantBaubles[baubleTag].baubleName;
		newBaubleIcon.tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
		newBaubleIcon.tooltipObject.subtitle = LocalInterface.instance.rarityDictionary[V.i.v.variantBaubles[baubleTag].category].rarityName;
		newBaubleIcon.tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantBaubles[baubleTag].category);
		newBaubleIcon.rt.anchoredPosition = new Vector2(5 + (baubleIcons.Count % 2) * 49, -5 - (baubleIcons.Count / 2) * 49);
		baubleIconsContent.sizeDelta = new Vector2(baubleIconsContent.sizeDelta.x, 54 + (baubleIcons.Count / 2) * 49);
		baubleIconsBackdrop.sizeDelta = new Vector2(baubleIconsBackdrop.sizeDelta.x, Mathf.Min(baubleIconsContent.sizeDelta.y + 10, LocalInterface.instance.referenceResolution.y));
		baubleIconsBackdrop.anchoredPosition = new Vector2(baubleIconsBackdrop.anchoredPosition.x, Mathf.Lerp(0, -92, baubleIconsBackdrop.sizeDelta.y / LocalInterface.instance.referenceResolution.y));
		baubleIcons.Add(baubleTag, newBaubleIcon);
		newBaubleIcon.UpdateBaubleIcon();
		slideOut.contentControllerSelectionGroup.controllerSelectableObjects.Add(newBaubleIcon.controllerSelectableObject);
		if(firstObject)
		{
			slideOut.contentControllerSelectionGroup.defaultObject = newBaubleIcon.controllerSelectableObject;
		}
		newBaubleIcon.controllerSelectableObject.scrollViewContentRT = baubleIconsContent;
		newBaubleIcon.controllerSelectableObject.scrollViewVerticalScrollbar = verticalScrollbar;
		newBaubleIcon.controllerSelectableObject.positionInScrollView = newBaubleIcon.rt.anchoredPosition.y - 10f;
	}
	
	public void UpdateSpecialBaubleIconsDescriptions()
	{
		if(baubleIcons.ContainsKey("ZodiacsFromFlushes"))
		{
			baubleIcons["ZodiacsFromFlushes"].UpdateBaubleIcon();
		}
	}
}
