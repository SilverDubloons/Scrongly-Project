using UnityEngine;
using System.Collections.Generic;
using static Variant;

public class CheatMenu : MonoBehaviour
{
	public RectTransform content;
	
    public GameObject cheatButtonPrefab;
	public Sprite[] extraButtonSprites;
	
	public List<CheatButton> cheatButtons = new List<CheatButton>();
	
	public const int buttonsWide = 10;
	public const float buttonBufferSize = 5f;
	public const float distanceBetweenButtons = 55f;
	
	public static CheatMenu instance;
	
	void Start()
	{
		SetupCheatMenu();
		instance = this;
	}
	
	public void SetupCheatMenu()
	{
		int buttonIndex = 0;
		foreach(KeyValuePair<string, VariantBauble> entry in V.i.v.variantBaubles)
		{
			GameObject newCheatButtonGO = Instantiate(cheatButtonPrefab, content);
			CheatButton newCheatButton = newCheatButtonGO.GetComponent<CheatButton>();
			cheatButtons.Add(newCheatButton);
			newCheatButton.itemType = "Bauble";
			newCheatButton.variantBauble = entry.Value;
			newCheatButton.rt.anchoredPosition = new Vector2(buttonBufferSize + (buttonIndex % buttonsWide) * distanceBetweenButtons, -buttonBufferSize + (buttonIndex / buttonsWide) * -distanceBetweenButtons);
			newCheatButton.buttonImage.sprite = entry.Value.sprite;
			if(entry.Value.category == "Zodiac")
			{
				newCheatButton.itemType = "Zodiac";
				newCheatButton.buttonImage.color = LocalInterface.instance.rarityDictionary["Zodiac"].rarityColor;
			}
			buttonIndex++;
		}
		foreach(KeyValuePair<string, VariantSpecialCard> entry in V.i.v.variantSpecialCards)
		{
			GameObject newCheatButtonGO = Instantiate(cheatButtonPrefab, content);
			CheatButton newCheatButton = newCheatButtonGO.GetComponent<CheatButton>();
			cheatButtons.Add(newCheatButton);
			newCheatButton.itemType = "SpecialCard";
			newCheatButton.variantSpecialCard = entry.Value;
			newCheatButton.rt.anchoredPosition = new Vector2(buttonBufferSize + (buttonIndex % buttonsWide) * distanceBetweenButtons, -buttonBufferSize + (buttonIndex / buttonsWide) * -distanceBetweenButtons);
			newCheatButton.buttonImage.sprite = entry.Value.sprite;
			buttonIndex++;
		}
		for(int i = 0; i < 5; i++)
		{
			GameObject newCheatButtonGO = Instantiate(cheatButtonPrefab, content);
			CheatButton newCheatButton = newCheatButtonGO.GetComponent<CheatButton>();
			cheatButtons.Add(newCheatButton);
			newCheatButton.itemType = "Currency";
			switch(i)
			{
				case 0:
					newCheatButton.quantity = 1;
				break;
				case 1:
					newCheatButton.quantity = 5;
				break;
				case 2:
					newCheatButton.quantity = 25;
				break;
				case 3:
					newCheatButton.quantity = 100;
				break;
				case 4:
					newCheatButton.quantity = 500;
				break;
			}
			newCheatButton.rt.anchoredPosition = new Vector2(buttonBufferSize + (buttonIndex % buttonsWide) * distanceBetweenButtons, -buttonBufferSize + (buttonIndex / buttonsWide) * -distanceBetweenButtons);
			newCheatButton.buttonImage.sprite = extraButtonSprites[i];
			buttonIndex++;
		}
		content.sizeDelta = new Vector2(content.sizeDelta.x, buttonBufferSize + ((buttonIndex + buttonsWide) / buttonsWide) * distanceBetweenButtons);
	}
	
	public void UpdateInteractabilityOfCheatButtons()
	{
		for(int i = 0; i < cheatButtons.Count; i++)
		{
			cheatButtons[i].SetInteractability();
		}
	}
}
