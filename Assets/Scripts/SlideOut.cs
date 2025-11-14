using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SlideOut : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
	public float moveSpeed;
	public Vector2 startPosition;
	public Vector2 endPosition;
	public bool mouseOver = false;
	private float moveState = 0;
	public bool slideEnabled;
	public bool isInfoPanel;
	public ControllerSelectionGroup hotkeyControllerSelectionGroup;
	public ControllerSelectionGroup contentControllerSelectionGroup;
	
	public void SetInteractability(bool enabledState)
	{
		slideEnabled = enabledState;
	}
	
	void Update()
	{
		if(mouseOver && slideEnabled && moveState < 1f)
		{
			moveState += moveSpeed * Time.deltaTime * Preferences.instance.gameSpeed;
			if(moveState > 1f)
			{
				moveState = 1f;
			}
			rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveState);
			if(ControllerSelection.instance.usingController)
			{
				ControllerSelection.instance.RepositionControllerSelectionRT(ControllerSelection.instance.currentlySelectedObject, contentControllerSelectionGroup);
			}
		}
		if((!mouseOver || !slideEnabled) && moveState > 0)
		{
			moveState -= moveSpeed * Time.deltaTime * Preferences.instance.gameSpeed;
			if(moveState < 0)
			{
				moveState = 0;
			}
			rt.anchoredPosition = Vector2.Lerp(startPosition, endPosition, moveState);
			if(ControllerSelection.instance.usingController)
			{
				ControllerSelection.instance.RepositionControllerSelectionRT(ControllerSelection.instance.currentlySelectedObject, contentControllerSelectionGroup);
			}
		}
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		// Debug.Log($"{this.gameObject.name} OnPointerEnter");
		if(slideEnabled && moveState < 0.99f)
		{
			SoundManager.instance.PlaySlideOutSound();
		}
		if(isInfoPanel)
		{
			int highestChildIndex = -1;
			int roundIndex = GameManager.instance.roundsInformationPanel.GetSiblingIndex();
			if(roundIndex > highestChildIndex)
			{
				highestChildIndex = roundIndex;
			}
			int handsIndex = GameManager.instance.handsInformationPanel.GetSiblingIndex();
			if(handsIndex > highestChildIndex)
			{
				highestChildIndex = handsIndex;
			}
			int baublesIndex = GameManager.instance.baublesInformationPanel.GetSiblingIndex();
			if(baublesIndex > highestChildIndex)
			{
				highestChildIndex = baublesIndex;
			}
			rt.SetSiblingIndex(highestChildIndex);
		}
		// transform.SetSiblingIndex(transform.parent.childCount - 1);
		mouseOver = true;
		contentControllerSelectionGroup.AddToCurrentGroups();
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		// Debug.Log($"{this.gameObject.name} OnPointerExit");
		if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene")
		{
			if(this == BaublesInformation.instance.slideOut || this == HandsInformation.instance.slideOut)
			{
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerEventData, results);
				foreach (RaycastResult result in results)
				{
					if(result.gameObject != null)
					{
						if(result.gameObject == Tooltip.instance.tooltipObject || result.gameObject == HandInfoTooltip.instance.backdropObject)
						{
							return;
						}
					}
				}
			}
		}
		if(slideEnabled)
		{
			if(this == HandsInformation.instance.slideOut)
			{
				HandInfoTooltip.instance.DisableTooltip();
			}
			SoundManager.instance.PlaySlideOutSound(true);
		}
		mouseOver = false;
		contentControllerSelectionGroup.RemoveFromCurrentGroups();
	}
}
