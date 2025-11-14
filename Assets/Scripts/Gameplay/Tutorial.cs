using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Tutorial : MonoBehaviour
{
	public TutorialStage[] tutorialStages;
	public string[] tutorialStageTextController;
	public string[] tutorialStageShadowTextController;
	
	public RectTransform dialogRT;
	public Label dialogLabel;
	public ButtonPlus dialogButton;
	public ButtonPlus leftButton;
	public ButtonPlus rightButton;
	public GameObject snakesButtonObject;
	public GameObject straightZodiacButtonObject;
	public GameObject visibilityObject;
	public ControllerSelectionGroup controllerSelectionGroup;
	public ControllerSelectableObject dialogButtonControllerSelectableObject;
	
	public IEnumerator moveCoroutine;
	public bool moving;
	public int currentStage;
	public bool tutorialFinished;
	public bool displayingTips;
	
	public static Tutorial instance;
	
	[System.Serializable]
	public class TutorialStage
	{
		public GameObject objectToEnable;
		public string dialogString;
		public bool stageHasButton;
		public string buttonText;
		public Vector2 dialogPosition;
		public Vector2 dialogSize;
		[SerializeField]
		public UnityEvent onStartEvent;
	}
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void SetupTutorial()
	{
		for(int i = 0; i < tutorialStages.Length; i++)
		{
			if(tutorialStages[i].objectToEnable != null)
			{
				tutorialStages[i].objectToEnable.SetActive(false);
			}
		}
		tutorialFinished = false;
		displayingTips = false;
		leftButton.gameObject.SetActive(false);
		rightButton.gameObject.SetActive(false);
		SetStage(0, false);
		controllerSelectionGroup.AddToCurrentGroups();
	}
	
	public void IncrementStage(bool disableOldStage = true)
	{
		SetStage(currentStage + 1, disableOldStage);
	}
	
	public void SetStage(int newStage, bool disableOldStage = true)
	{
		dialogLabel.ChangeText(tutorialStages[newStage].dialogString);
		if(!tutorialFinished)
		{
			if(disableOldStage)
			{
				if(tutorialStages[currentStage].objectToEnable != null)
				{
					tutorialStages[currentStage].objectToEnable.SetActive(false);
				}
			}
			currentStage = newStage;
			tutorialStages[currentStage].onStartEvent.Invoke();
			if(tutorialStages[currentStage].objectToEnable != null)
			{
				tutorialStages[currentStage].objectToEnable.SetActive(true);
			}
			if(tutorialStages[currentStage].stageHasButton)
			{
				dialogButton.gameObject.SetActive(true);
				dialogButton.ChangeButtonText(tutorialStages[currentStage].buttonText);
				dialogLabel.rt.offsetMin = new Vector2(5f, 34f);
				if(ControllerSelection.instance.usingController)
				{
					ControllerSelection.instance.MoveSelectionToObject(dialogButtonControllerSelectableObject, controllerSelectionGroup);
				}
			}
			else
			{
				dialogButton.gameObject.SetActive(false);
				dialogLabel.rt.offsetMin = new Vector2(5f, 5f);
				StartCoroutine(MoveControllerToBestFitNextFrame());
			}
			if(moving)
			{
				StopCoroutine(moveCoroutine);
			}
			moveCoroutine = Move(tutorialStages[currentStage].dialogPosition, tutorialStages[currentStage].dialogSize);
			StartCoroutine(moveCoroutine);
		}
		currentStage = newStage;
		if(ControllerSelection.instance.usingController)
		{
			ChangeToControllerText();
		}
	}
	
	public IEnumerator MoveControllerToBestFitNextFrame()
	{
		yield return null;
		ControllerSelection.instance.MoveSelectionToBestFit();
	}
	
	public IEnumerator Move(Vector2 destinationPosition, Vector2 destinationSize)
	{
		moving = true;
		Vector2 originPosition = dialogRT.anchoredPosition;
		Vector2 originSize = dialogRT.sizeDelta;
		float moveTime = 1f;
		float t = 0;
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
			dialogRT.anchoredPosition = Vector2.Lerp(originPosition, destinationPosition, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			dialogRT.sizeDelta = Vector2.Lerp(originSize, destinationSize, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			if(ControllerSelection.instance.usingController && ControllerSelection.instance.currentlySelectedObject == dialogButtonControllerSelectableObject)
			{
				ControllerSelection.instance.RepositionControllerSelectionRT(dialogButtonControllerSelectableObject, controllerSelectionGroup);
			}
			yield return null;
		}
		moving = false;
	}
	
	public void DialogButtonClicked()
	{
		if(tutorialFinished)
		{
			displayingTips = false;
			visibilityObject.SetActive(false);
			controllerSelectionGroup.RemoveFromCurrentGroups();
		}
		else
		{
			IncrementStage();
		}
	}
	
	public void SetTutorialItemsToFree()
	{
		Shop.instance.SetItemInCategoryToCost("DecreaseCardsNeededForStraight", "Bauble", 0);
		Shop.instance.SetItemInCategoryToCost("Hand04Power", "Zodiac", 0);
	}
	
	public void EndMainTutorial()
	{
		tutorialFinished = true;
		displayingTips = true;
		leftButton.gameObject.SetActive(true);
		rightButton.gameObject.SetActive(true);
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = Move(tutorialStages[currentStage].dialogPosition, tutorialStages[currentStage].dialogSize);
		StartCoroutine(moveCoroutine);
		dialogButton.gameObject.SetActive(true);
		dialogButton.ChangeButtonText(tutorialStages[currentStage].buttonText);
		dialogLabel.rt.offsetMin = new Vector2(5f, 34f);
	}
	
	public void LeftClicked()
	{
		if(currentStage <= 25)
		{
			SetStage(currentStage = tutorialStages.Length - 1);
		}
		else
		{
			SetStage(currentStage - 1);
		}
	}
	
	public void RightClicked()
	{
		if(currentStage >= tutorialStages.Length - 1)
		{
			SetStage(25);
		}
		else
		{
			SetStage(currentStage + 1);
		}
	}
	
	public void MoveToShopSide()
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = Move(new Vector2(228f, 38.5f), new Vector2(174f, 272f));
		StartCoroutine(moveCoroutine);
	}
	
	public void MoveToTop()
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = Move(new Vector2(0f, 125f), new Vector2(400f, 100f));
		StartCoroutine(moveCoroutine);
	}
	
	public void ChangeToControllerText()
	{
		dialogLabel.ChangeSpriteAsset(LocalInterface.instance.inputGlyphSpriteAssets[Preferences.instance.glyphSet]);
		dialogLabel.ChangeText(tutorialStageTextController[currentStage], tutorialStageShadowTextController[currentStage]);
	}
	
	public void ChangeToRegularText()
	{
		dialogLabel.ChangeSpriteAsset(LocalInterface.instance.inputGlyphSpriteAssets[Preferences.instance.glyphSet]);
		dialogLabel.ChangeText(tutorialStages[currentStage].dialogString);
	}
}
