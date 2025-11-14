using UnityEngine;
using System;

public class CardValuesTooltip : MonoBehaviour
{
    public RectTransform rt;
	public GameObject visualObject;
	public RectTransform pointsBackdrop;
	public RectTransform multBackdrop;
	public Label pointsLabel;
	public Label multLabel;
	
	public const float extraWidth = 10f;
	
	public static CardValuesTooltip instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void DisplayTooltip(Card card)
	{
		visualObject.SetActive(true);
		float yPos = 0;
		if(Math.Abs(card.cardData.baseValue) > 0.1d)
		{
			pointsBackdrop.gameObject.SetActive(true);
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(card.cardData.baseValue));
			pointsBackdrop.sizeDelta = new Vector2(pointsLabel.GetPreferredWidth() + extraWidth, pointsBackdrop.sizeDelta.y);
			yPos += 24f;
		}
		else
		{
			pointsBackdrop.gameObject.SetActive(false);
		}
		if(Math.Abs(card.cardData.multiplier) > 0.1d)
		{
			multBackdrop.gameObject.SetActive(true);
			multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(card.cardData.multiplier));
			multBackdrop.sizeDelta = new Vector2(multLabel.GetPreferredWidth() + extraWidth, multBackdrop.sizeDelta.y);
			multBackdrop.anchoredPosition = new Vector2(multBackdrop.anchoredPosition.x, yPos);
			yPos += 24f;
		}
		else
		{
			multBackdrop.gameObject.SetActive(false);
		}
		rt.anchoredPosition = LocalInterface.instance.GetCanvasPositionOfRectTransform(card.rt, GameManager.instance.gameplayCanvas) + new Vector2(0, 50f);
		/* rt.anchoredPosition = card.rt.anchoredPosition + new Vector2(0, 50f);
		if(card.isDeckviewerClone)
		{
			rt.anchoredPosition += new Vector2(-73f, 14f);
		}
		if(rt.anchoredPosition.y + yPos > LocalInterface.instance.referenceResolution.y / 2)
		{
			rt.anchoredPosition = card.rt.anchoredPosition + new Vector2(0, -6f);
			if(card.isDeckviewerClone)
			{
				rt.anchoredPosition += new Vector2(-73f, -14f);
			}
		} */
		// check for y excess
	}
	
	public void HideTooltip()
	{
		visualObject.SetActive(false);
	}
}
