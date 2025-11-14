using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour/* , IPointerEnterHandler */, IPointerExitHandler
{
	public GameObject tooltipObject;
	public GameObject currentObject;
	public RectTransform tooltipRT;
	public RectTransform titleBackdrop;
	public RectTransform subtitleBackdrop;
	public RectTransform mainTextBackdrop;
	public Image titleImage;
	public Image subtitleImage;
	public Image mainTextImage;
	public Label titleLabel;
	public Label subtitleLabel;
	public Label mainTextLabel;
	public const float tooltipDistanceFromMouse = 5f;
	public const float tooltipMaxSizeX = 140f;
	public float tooltipYSizeIncrease;
	public float tooltipXSizeIncrease;
	
	public GameObject progressBarObject;
	public Label progressBarLabel;
	public RectTransform progressBarMask; // width of 136 = full
	
	// public bool mouseOverTooltip;
	
	public static Tooltip instance;
	
	public void SetupInstance()
	{
		instance = this;
		tooltipObject.SetActive(false);
	}
	
	public void ShowTooltip(GameObject currentObject, string mainText, string title = "", ThemeManager.UIElementType titleType = ThemeManager.UIElementType.text, string subtitle = "", ThemeManager.UIElementType subtitleType = ThemeManager.UIElementType.text, bool showProgressBar = false, int progressLowEnd = 0, int progressHighEnd = 0, bool useObjectPosition = false)
	{
		useObjectPosition = true;
		tooltipObject.SetActive(true);
		this.currentObject = currentObject;
		tooltipRT.sizeDelta = new Vector2(tooltipMaxSizeX, 900f);
		titleBackdrop.sizeDelta = new Vector2(tooltipMaxSizeX - tooltipXSizeIncrease, 900f);
		subtitleBackdrop.sizeDelta = new Vector2(tooltipMaxSizeX - tooltipXSizeIncrease, 900f);
		mainTextBackdrop.sizeDelta = new Vector2(tooltipMaxSizeX - tooltipXSizeIncrease, 900f);
		float currentHeight = 4f;
		Vector2 titlePrefferedSize = Vector2.zero;
		if(title != "")
		{
			titleBackdrop.gameObject.SetActive(true);
			// titleImage.color = ThemeManager.instance.GetColorFromCurrentTheme(titleType);
			titleLabel.ChangeText(title);
			titlePrefferedSize = titleLabel.GetPreferredValuesString(tooltipMaxSizeX - tooltipXSizeIncrease);
			titleBackdrop.sizeDelta = new Vector2(titlePrefferedSize.x + 6f, titlePrefferedSize.y + 4f);
			currentHeight += titleBackdrop.sizeDelta.y + 2f;
		}
		else
		{
			titleBackdrop.gameObject.SetActive(false);
		}
		Vector2 subtitlePreferedSize = Vector2.zero;
		if(subtitle != "")
		{
			subtitleBackdrop.gameObject.SetActive(true);
			subtitleImage.color = ThemeManager.instance.GetColorFromCurrentTheme(subtitleType);
			subtitleLabel.ChangeText(subtitle);
			subtitlePreferedSize = subtitleLabel.GetPreferredValuesString(tooltipMaxSizeX - tooltipXSizeIncrease);
			subtitleBackdrop.sizeDelta = new Vector2(subtitlePreferedSize.x + 6f, subtitlePreferedSize.y + 4f);
			subtitleBackdrop.anchoredPosition = new Vector2(subtitleBackdrop.anchoredPosition.x, -currentHeight);
			currentHeight += subtitleBackdrop.sizeDelta.y + 2f;
		}
		else
		{
			subtitleBackdrop.gameObject.SetActive(false);
		}
		if(titleBackdrop.gameObject.activeSelf || subtitleBackdrop.gameObject.activeSelf)
		{
			currentHeight += 2f;
		}
		mainTextLabel.ChangeText(mainText, true);
		mainTextLabel.ForceMeshUpdate();
		Vector2 mainTextPreferedSize = mainTextLabel.GetPreferredValuesString(tooltipMaxSizeX - tooltipXSizeIncrease);
		float minimumSize = 0f;
		if(showProgressBar)
		{
			minimumSize = tooltipMaxSizeX - tooltipXSizeIncrease;
		}
		mainTextBackdrop.sizeDelta = new Vector2(mainTextPreferedSize.x + 6f, mainTextPreferedSize.y + 4f);
		mainTextBackdrop.anchoredPosition = new Vector2(mainTextBackdrop.anchoredPosition.x, -currentHeight);
		currentHeight += mainTextBackdrop.sizeDelta.y + 4f;
		// Debug.Log($"titlePrefferedSize={titlePrefferedSize}, subtitlePreferedSize={subtitlePreferedSize}, mainTextPreferedSize={mainTextPreferedSize}, currentHeight={currentHeight}");
		tooltipRT.sizeDelta = new Vector2(Mathf.Max(titlePrefferedSize.x + tooltipXSizeIncrease, subtitlePreferedSize.x + tooltipXSizeIncrease, mainTextPreferedSize.x + tooltipXSizeIncrease, minimumSize), currentHeight);
		// mainTextBackdrop.sizeDelta = new Vector2(mainTextPreferedSize.x, mainTextPreferedSize.y + 4f);
		float tooltipPosX = 0f;
		float tooltipPosY = 0f;
		if(useObjectPosition)
		{
			RectTransform currentRT = currentObject.GetComponent<RectTransform>();
			Vector3 center = LocalInterface.instance.GetCanvasPositionOfRectTransform(currentRT, LocalInterface.instance.persistentCanvas);
			tooltipPosX = center.x + currentRT.sizeDelta.x / 2 + tooltipRT.sizeDelta.x / 2 + tooltipDistanceFromMouse;
			if(tooltipPosX > LocalInterface.instance.referenceResolution.x / 2 - tooltipRT.sizeDelta.x / 2)
			{
				tooltipPosX = center.x - currentRT.sizeDelta.x / 2 - tooltipRT.sizeDelta.x / 2 - tooltipDistanceFromMouse;
			}
			tooltipPosY = center.y;
			// Debug.Log($"currentRT.gameObject.name={currentRT.gameObject.name}, center={center}, tooltipPosX={tooltipPosX}, tooltipPosY={tooltipPosY}");
		}
		else
		{
			Vector2 mousePos = LocalInterface.instance.GetMousePosition();
			tooltipPosX = mousePos.x + tooltipRT.sizeDelta.x / 2 + tooltipDistanceFromMouse;
			if(tooltipPosX > LocalInterface.instance.referenceResolution.x / 2 - tooltipRT.sizeDelta.x / 2)
			{
				tooltipPosX = mousePos.x - tooltipRT.sizeDelta.x / 2 - tooltipDistanceFromMouse;
			}
			tooltipPosY = mousePos.y;
		}
		if(tooltipPosY > LocalInterface.instance.referenceResolution.y / 2 - tooltipRT.sizeDelta.y / 2)
		{
			tooltipPosY = LocalInterface.instance.referenceResolution.y / 2 - tooltipRT.sizeDelta.y / 2;
		}
		if(tooltipPosY < -LocalInterface.instance.referenceResolution.y / 2 + tooltipRT.sizeDelta.y / 2 + (showProgressBar ? 24 : 0))
		{
			tooltipPosY = -LocalInterface.instance.referenceResolution.y / 2 + tooltipRT.sizeDelta.y / 2 + (showProgressBar ? 24 : 0);
		}
		tooltipRT.anchoredPosition = new Vector2(tooltipPosX, tooltipPosY);
		if(showProgressBar)
		{
			progressBarObject.SetActive(true);
			progressBarLabel.ChangeText($"{progressLowEnd}/{progressHighEnd}");
			progressBarMask.sizeDelta = new Vector2((float)progressLowEnd / (float)progressHighEnd * tooltipRT.sizeDelta.x - 4f, progressBarMask.sizeDelta.y);
		}
		else
		{
			progressBarObject.SetActive(false);
		}
	}
	
	public void DisableTooltip()
	{
		currentObject = null;
		// mouseOverTooltip = false;
		tooltipObject.SetActive(false);
	}
	
/* 	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		mouseOverTooltip = true;
		// DisableTooltip();
	} */
	
	public void OnPointerExit(PointerEventData pointerEventData)
	{
		// mouseOverTooltip = false;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach(RaycastResult result in results)
		{
			if (result.gameObject != null)
			{
				if(result.gameObject == currentObject)
				{
					return;
				}
			}
		}
		if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene")
		{
			if(BaublesInformation.instance.slideOut.mouseOver)
			{
				List<RaycastResult> results2 = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerEventData, results2);
				bool mouseOverBaubles = false;
				foreach (RaycastResult result in results2)
				{
					if(result.gameObject != null)
					{
						if(result.gameObject == BaublesInformation.instance.tabObject || result.gameObject == BaublesInformation.instance.backdropObject)
						{
							mouseOverBaubles = true;
							break;
						}
					}
				}
				if(!mouseOverBaubles)
				{
					SoundManager.instance.PlaySlideOutSound(true);
					BaublesInformation.instance.slideOut.mouseOver = false;
				}
			}
			if(HandsInformation.instance.slideOut.mouseOver)
			{
				List<RaycastResult> results2 = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerEventData, results2);
				bool mouseOverHands = false;
				foreach (RaycastResult result in results2)
				{
					if(result.gameObject != null)
					{
						if(result.gameObject == HandsInformation.instance.tabObject || result.gameObject == HandsInformation.instance.backdropObject)
						{
							mouseOverHands = true;
							break;
						}
					}
				}
				if(!mouseOverHands)
				{
					// Debug.Log("Tooltip disabling HandsInformation.instance.slideOut.mouseOver");
					SoundManager.instance.PlaySlideOutSound(true);
					HandsInformation.instance.slideOut.mouseOver = false;
				}
			}
		}
		DisableTooltip();
	}
}
