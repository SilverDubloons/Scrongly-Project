using UnityEngine;
using UnityEngine.UI;
using System;

public class BaubleIcon : MonoBehaviour
{
    public RectTransform rt;
	public Image baubleImage;
	public Label quantityLabel;
	public GameObject xObject;
	public TooltipObject tooltipObject;
	public ControllerSelectableObject controllerSelectableObject;
	
	public string baubleTag;
	
	public void UpdateBaubleIcon()
	{
		int quantityOwned = Baubles.instance.GetQuantityOwned(baubleTag, true);
		if(quantityOwned <= 1)
		{
			quantityLabel.ChangeText(string.Empty);
		}
		else
		{
			quantityLabel.ChangeText(quantityOwned.ToString());
		}
		var resolver = new DescriptionResolver();
		// string input = V.i.v.variantBaubles[baubleTag].inGameDescription;
		string input = "";
		if(quantityOwned > 1 && V.i.v.variantBaubles[baubleTag].extraDescriptions.Length > 0)
		{
			input = V.i.v.variantBaubles[baubleTag].extraDescriptions[Mathf.Min(quantityOwned - 2, V.i.v.variantBaubles[baubleTag].extraDescriptions.Length - 1)];
		}
		else
		{
			input = V.i.v.variantBaubles[baubleTag].inGameDescription;
		}
		string output = resolver.Resolve(input);
		// Debug.Log(output); // → "Increases hand size by 1. Current size is 6"
		tooltipObject.mainText = output;
		// tooltipObject.mainText = V.i.v.variantBaubles[baubleTag].inGameDescription;
	}
}
