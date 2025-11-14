using UnityEngine;
using UnityEngine.UI;
using static Variant;
using System;

public class BaubleExplainer : MonoBehaviour
{
	public RectTransform rt;
    public Image baubleImage;
	public Image baubleRarityImage;
	public GameObject notInShopIndicator;
	public Label startingQuantityLabel;
	public Label baseCostLabel;	// doubles as mult label for zodiacs
	public Label costStepLabel;	// doubles as cost label for zodiacs
	public Label pointsLabel;
	public TooltipObject tooltipObject;
	
	public void SetupBaubleExplainer(Vector2 location, string tag)
	{
		var resolver = new DescriptionResolver();
		string input = string.Empty;
		if(LocalInterface.instance.GetCurrentSceneName() == "MainMenuScene")
		{
			input = V.i.v.variantBaubles[tag].menuDescription;
		}
		else
		{
			input = V.i.v.variantBaubles[tag].inGameDescription;
		}
		string output = resolver.Resolve(input);
		if(LocalInterface.instance.GetCurrentSceneName() == "MainMenuScene" && V.i.v.variantBaubles[tag].category == "Zodiac")
		{
			output = String.Format(output, LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[tag].impact1), LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[tag].impact2));
		}
		if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene" && V.i.v.variantBaubles[tag].category == "Zodiac")
		{
			output = String.Format(output, LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[tag].impact1), LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[tag].impact2), LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetHandPoints(LocalInterface.instance.GetHandTierFromZodiacTag(tag))), LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetHandMult(LocalInterface.instance.GetHandTierFromZodiacTag(tag))));
		}
		tooltipObject.mainText = output;
		tooltipObject.title = V.i.v.variantBaubles[tag].baubleName;
		if(V.i.v.variantBaubles[tag].category == "Zodiac")
		{
			tooltipObject.titleColor = ThemeManager.UIElementType.Zodiac;
		}
		else
		{
			tooltipObject.titleColor = ThemeManager.UIElementType.BaubleName;
		}
		tooltipObject.subtitle = V.i.v.variantBaubles[tag].category;
		// tooltipObject.subtitleColor = LocalInterface.instance.rarityDictionary[V.i.v.variantBaubles[tag].category].rarityColor;
		tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantBaubles[tag].category);
		rt.anchoredPosition = location;
		baubleImage.sprite = V.i.v.variantBaubles[tag].sprite;
		if(V.i.v.variantBaubles[tag].category == "Zodiac")
		{
			baubleImage.color = LocalInterface.instance.rarityDictionary["Zodiac"].rarityColor;
			pointsLabel.gameObject.gameObject.SetActive(true);
			startingQuantityLabel.gameObject.SetActive(false);
			baubleRarityImage.gameObject.SetActive(false);
			notInShopIndicator.SetActive(false);
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[tag].impact1));
			baseCostLabel.gameObject.SetActive(true);
			baseCostLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[tag].impact2));
			baseCostLabel.ChangeColor(LocalInterface.instance.multiplierColor);
			costStepLabel.gameObject.SetActive(true);
			costStepLabel.ChangeText($"${V.i.v.variantBaubles[tag].baseCost}");
		}
		else
		{
			baubleImage.color = Color.white;
			pointsLabel.gameObject.SetActive(false);
			if(V.i.v.variantBaubles[tag].availableInStore)
			{
				notInShopIndicator.SetActive(false);
				baubleRarityImage.gameObject.SetActive(true);
				baubleRarityImage.color = LocalInterface.instance.rarityDictionary[V.i.v.variantBaubles[tag].category].rarityColor;
				baseCostLabel.gameObject.SetActive(true);
				baseCostLabel.ChangeText($"${V.i.v.variantBaubles[tag].baseCost.ToString()}");
				baseCostLabel.ChangeColor(Color.white);
				if(V.i.v.variantBaubles[tag].costStep == 0 && LocalInterface.instance.baseVariant.variantBaubles[tag].costStep == 0)
				{
					costStepLabel.gameObject.SetActive(false);
				}
				else
				{
					costStepLabel.gameObject.SetActive(true);
					costStepLabel.ChangeText($"+{V.i.v.variantBaubles[tag].costStep.ToString()}");
				}
			}
			else
			{
				notInShopIndicator.SetActive(true);
				baubleRarityImage.gameObject.SetActive(false);
				baseCostLabel.gameObject.SetActive(false);
				costStepLabel.gameObject.SetActive(false);
			}
			if(V.i.v.variantBaubles[tag].startingQuantity == 0)
			{
				startingQuantityLabel.gameObject.SetActive(false);
			}
			else
			{
				startingQuantityLabel.gameObject.SetActive(true);
				startingQuantityLabel.ChangeText(V.i.v.variantBaubles[tag].startingQuantity.ToString());
			}
			
		}
	}
}
