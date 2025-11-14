using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeckVariantCard : MonoBehaviour
{
	public RectTransform rt;
	
    public Image rankImage;
    public Image suitImage;
    public Image detailImage;
	public RectTransform rankImageRT;
    public RectTransform suitImageRT;
    public RectTransform detailImageRT;
    public Label quantityLabel;
	public ButtonPlus minusButton;
	public ButtonPlus plusButton;
	public ControllerSelectableObject minusButtonControllerSelectableObject;
	public ControllerSelectableObject plusButtonControllerSelectableObject;
	
	// public bool hasChanged;
	public int quantity;
	public string specialCardTag;
	// public bool setupComplete;
	
	public void SetupDeckVariantCard(int quantity, string tag = "")
	{
		UpdateQuantity(quantity);
		specialCardTag = tag;
		minusButtonControllerSelectableObject.scrollViewContentRT = DeckVariantMenu.instance.cardParent;
		plusButtonControllerSelectableObject.scrollViewContentRT = DeckVariantMenu.instance.cardParent;
		minusButtonControllerSelectableObject.scrollViewVerticalScrollbar = DeckVariantMenu.instance.verticalScrollbar;
		plusButtonControllerSelectableObject.scrollViewVerticalScrollbar = DeckVariantMenu.instance.verticalScrollbar;
		StartCoroutine(SetPosition());
	}
	
	public IEnumerator SetPosition()
	{
		yield return null;
		minusButtonControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
		plusButtonControllerSelectableObject.positionInScrollView = rt.anchoredPosition.y;
	}
	
	public void SetDeckVariantCardButtons(bool enabledState)
	{
		if(quantity <= 0)
		{
			minusButton.ChangeButtonEnabled(false);
		}
		else
		{
			minusButton.ChangeButtonEnabled(enabledState);
		}
		if(quantity >= DeckVariantMenu.instance.maximumIndividualCardQuantity)
		{
			plusButton.ChangeButtonEnabled(false);
		}
		else
		{
			plusButton.ChangeButtonEnabled(enabledState);
		}
	}
	
	public void UpdateQuantity(int newQuantity)
	{
		quantity = newQuantity;
		if(quantity <= 0 && minusButton.GetButtonEnabled())
		{
			minusButton.ChangeButtonEnabled(false);
		}
		else
		{
			if(!minusButton.GetButtonEnabled())
			{
				minusButton.ChangeButtonEnabled(true);
			}
		}
		if(quantity >= DeckVariantMenu.instance.maximumIndividualCardQuantity && plusButton.GetButtonEnabled())
		{
			plusButton.ChangeButtonEnabled(false);
		}
		else
		{
			if(!plusButton.GetButtonEnabled())
			{
				plusButton.ChangeButtonEnabled(true);
			}
		}
		quantityLabel.ChangeText(quantity.ToString());
	}
	
	public void MinusClicked()
	{
		UpdateQuantity(quantity - 1);
		/* if(!setupComplete)
		{
			return;
		} */
		// DeckVariantMenu.instance.DeckHasChanged();
		// hasChanged = true;
	}
	
	public void PlusClicked()
	{
		UpdateQuantity(quantity + 1);
		/* if(!setupComplete)
		{
			return;
		} */
		// DeckVariantMenu.instance.DeckHasChanged();
		// hasChanged = true;
	}
}
