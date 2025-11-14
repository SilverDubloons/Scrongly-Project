using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class ControllerSelection : MonoBehaviour
{
	public RectTransform controllerSelectionRT;
	public Image controllerSelectionImage;
	public GameObject visibilityObject;
	
	public Sprite borderFourPixel;
	public Sprite borderEightPixel;
	public Color movingCardColor;
	
	public List<ControllerSelectionGroup> currentControllerSelectionGroups = new List<ControllerSelectionGroup>();
	public ControllerSelectableObject currentlySelectedObject;
	
	public bool usingController;
	
	private InputAction moveAction; // left stick
	private InputAction rightStickAction;
	private InputAction southButtonAction;
	private InputAction eastButtonAction;
	private InputAction northButtonAction;
	private InputAction westButtonAction;
	private InputAction l1ButtonAction;
	private InputAction l2ButtonAction;
	private InputAction r1ButtonAction;
	private InputAction r2ButtonAction;
	private InputAction startButtonAction;
	private InputAction leftStickPressButtonAction;
	private InputAction rightStickPressButtonAction;
	private InputAction dPadUpAction;
	private InputAction dPadLeftAction;
	private InputAction dPadRightAction;
	private InputAction dPadDownAction;
	
	public float timeMoving;
	public int timesMovedWithoutLettingGo;
	public float rightStickTimeMoving;
	public int rightStickTimesMovedWithoutLettingGo;
	private Vector2 lastMousePos;
	public float southButtonTimePressed;
	public float southButtonTimesMovedWithoutLettingGo;
	public bool selectedCardHasBeenMoved;
	public bool alreadyCheckedControllerType;
    
	public static ControllerSelection instance;
	
	public void SetupInstance()
	{
		instance = this;
		moveAction = InputSystem.actions.FindAction("LeftStick");
		rightStickAction = InputSystem.actions.FindAction("RightStick");
		southButtonAction = InputSystem.actions.FindAction("SouthButton");
		eastButtonAction = InputSystem.actions.FindAction("EastButton");
		northButtonAction = InputSystem.actions.FindAction("NorthButton");
		westButtonAction = InputSystem.actions.FindAction("WestButton");
		l1ButtonAction = InputSystem.actions.FindAction("L1Button");
		l2ButtonAction = InputSystem.actions.FindAction("L2Button");
		r1ButtonAction = InputSystem.actions.FindAction("R1Button");
		r2ButtonAction = InputSystem.actions.FindAction("R2Button");
		startButtonAction = InputSystem.actions.FindAction("StartButton");
		leftStickPressButtonAction = InputSystem.actions.FindAction("LeftStickPress");
		rightStickPressButtonAction = InputSystem.actions.FindAction("RightStickPress");
		dPadUpAction = InputSystem.actions.FindAction("DPadUp");
		dPadLeftAction = InputSystem.actions.FindAction("DPadLeft");
		dPadRightAction = InputSystem.actions.FindAction("DPadRight");
		dPadDownAction = InputSystem.actions.FindAction("DPadDown");
		visibilityObject.SetActive(false);
	}
	
	void Update()
	{
		CheckLeftStickInput();
		CheckRightStickInput();
		if(southButtonAction.WasPerformedThisFrame())
		{
			SouthButtonPerformed();
		}
		if(southButtonAction.IsPressed())
		{
			southButtonTimePressed += Time.deltaTime;
			if(currentlySelectedObject != null && currentlySelectedObject.isCard && southButtonTimePressed > 0.2f)
			{
				controllerSelectionImage.color = movingCardColor;
			}
		}
		if(southButtonAction.WasCompletedThisFrame())
		{
			SouthButtonCompleted();
			southButtonTimePressed = 0;
			selectedCardHasBeenMoved = false;
			if(currentlySelectedObject != null && currentlySelectedObject.isCard)
			{
				controllerSelectionImage.color = currentlySelectedObject.borderColor;
			}
		}
		if(eastButtonAction.WasPerformedThisFrame())
		{
			if(currentlySelectedObject != null && currentlySelectedObject.isDropdown && currentlySelectedObject.dropdown.IsExpanded)
			{
				
			}
			else
			{
				HotkeyButtonPerformed("EastButton");
			}
		}
		if(eastButtonAction.WasCompletedThisFrame())
		{
			if(currentlySelectedObject != null && currentlySelectedObject.isDropdown && currentlySelectedObject.dropdown.IsExpanded)
			{
				currentlySelectedObject.dropdown.Hide();
			}
			else
			{
				ControllerSelectableObject eastButtonCompletedObject = HotkeyButtonCompleted("EastButton");
				if(eastButtonCompletedObject == null)
				{
					for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
					{
						for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
						{
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isSlideOut && currentControllerSelectionGroups[i].controllerSelectableObjects[j].slideOut.mouseOver)
							{
								currentControllerSelectionGroups[i].controllerSelectableObjects[j].slideOut.OnPointerExit(new PointerEventData(EventSystem.current));
							}
						}
					}
				}
			}
		}
		if(northButtonAction.WasPerformedThisFrame())
		{
			HotkeyButtonPerformed("NorthButton");
		}
		if(northButtonAction.WasCompletedThisFrame())
		{
			HotkeyButtonCompleted("NorthButton");
		}
		if(westButtonAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject westPerformedObject = HotkeyButtonPerformed("WestButton");
			if(westPerformedObject != null)
			{
				if(westPerformedObject.isSpecial && westPerformedObject.specialTag == "OnScreenKeyboardShift")
				{
					OnScreenKeyboard.instance.ShiftPressedDown();
				}
			}				
		}
		if(westButtonAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject westCompletedObject = HotkeyButtonCompleted("WestButton");
			if(westCompletedObject != null)
			{
				if(westCompletedObject.isSpecial && westCompletedObject.specialTag == "OnScreenKeyboardShift")
				{
					OnScreenKeyboard.instance.ShiftReleased();
				}
			}
		}
		if(l1ButtonAction.WasPerformedThisFrame())
		{
			HotkeyButtonPerformed("L1Button");
		}
		if(l1ButtonAction.WasCompletedThisFrame())
		{
			HotkeyButtonCompleted("L1Button");
		}
		if(l2ButtonAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject l2PerformedObject = HotkeyButtonPerformed("L2Button");
			if(l2PerformedObject != null)
			{
				if(l2PerformedObject.isSpecial && l2PerformedObject.specialTag == "DrawPile")
				{
					DeckPreview.instance.MouseOverDeck(true);
				}
			}
		}
		if(l2ButtonAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject l2CompletedObject = HotkeyButtonCompleted("L2Button");
			if(l2CompletedObject != null)
			{
				if(l2CompletedObject.isSpecial && l2CompletedObject.specialTag == "DrawPile")
				{
					DeckPreview.instance.MouseExited();
				}
			}
		}
		if(r1ButtonAction.WasPerformedThisFrame())
		{
			HotkeyButtonPerformed("R1Button");
		}
		if(r1ButtonAction.WasCompletedThisFrame())
		{
			HotkeyButtonCompleted("R1Button");
		}
		if(r2ButtonAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject r2PerformedPerformedObject = HotkeyButtonPerformed("R2Button");
			if(r2PerformedPerformedObject != null)
			{
				if(r2PerformedPerformedObject.isSpecial && r2PerformedPerformedObject.specialTag == "DiscardPile")
				{
					DeckPreview.instance.MouseOverDeck(false);
				}
			}
		}
		if(r2ButtonAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject r2CompletedObject = HotkeyButtonCompleted("R2Button");
			if(r2CompletedObject != null)
			{
				if(r2CompletedObject.isSpecial && r2CompletedObject.specialTag == "DiscardPile")
				{
					DeckPreview.instance.MouseExited();
				}
			}
		}
		if(leftStickPressButtonAction.WasPerformedThisFrame())
		{
			HotkeyButtonPerformed("LeftStickPress");
		}
		if(leftStickPressButtonAction.WasCompletedThisFrame())
		{
			HotkeyButtonCompleted("LeftStickPress");
		}
		if(rightStickPressButtonAction.WasPerformedThisFrame())
		{
			HotkeyButtonPerformed("RightStickPress");
		}
		if(rightStickPressButtonAction.WasCompletedThisFrame())
		{
			HotkeyButtonCompleted("RightStickPress");
		}
		if(dPadUpAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonPerformed("DPadUp");
		}
		if(dPadUpAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonCompleted("DPadUp");
			if(dPadAction == null)
			{
				
			}
		}
		if(dPadLeftAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonPerformed("DPadLeft");
		}
		if(dPadLeftAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonCompleted("DPadLeft");
		}
		if(dPadRightAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonPerformed("DPadRight");
		}
		if(dPadRightAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonCompleted("DPadRight");
		}
		if(dPadDownAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonPerformed("DPadDown");
		}
		if(dPadDownAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonCompleted("DPadDown");
		}
		if(startButtonAction.WasPerformedThisFrame())
		{
			ControllerSelectableObject dPadAction = HotkeyButtonPerformed("StartButton");
		}
		if(startButtonAction.WasCompletedThisFrame())
		{
			ControllerSelectableObject startCompletedObject = HotkeyButtonCompleted("StartButton");
			if(startCompletedObject == null)
			{
				Preferences.instance.ToggleMenuVisualObject();
			}
		}
		if(usingController)
		{
			Vector2 mousePos = LocalInterface.instance.GetMousePosition();
			if(Mathf.Abs(mousePos.x - lastMousePos.x) > 0.1f || Mathf.Abs(mousePos.y - lastMousePos.y) > 0.1f)
			{
				usingController = false;
				Cursor.visible = true;
				visibilityObject.SetActive(false);
				for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
				{
					for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
					{
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton)
						{
							currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.DisableHotkey();
							if(OnScreenKeyboard.instance.visibilityObject.activeSelf)
							{
								OnScreenKeyboard.instance.CancelClicked(true);
							}
						}
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage != null)
						{
							currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage.gameObject.SetActive(false);
						}
					}
				}
				if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene" && V.i.isTutorial && (!Tutorial.instance.tutorialFinished || Tutorial.instance.displayingTips))
				{
					Tutorial.instance.ChangeToRegularText();
				}
			}
			lastMousePos = mousePos;
		}
	}
	
	public void CheckLeftStickInput()
	{
		Vector2 moveValue = moveAction.ReadValue<Vector2>();
		if(Mathf.Abs(moveValue.x) < Preferences.instance.controllerDeadzone && Mathf.Abs(moveValue.y) < Preferences.instance.controllerDeadzone)
		{
			timeMoving = 0f;
			timesMovedWithoutLettingGo = 0;
		}
		else
		{
			if(!usingController)
			{
				ActivateControllerInput();
				return;
			}
			if(currentControllerSelectionGroups.Count <= 0)
			{
				return;
			}
			if(currentlySelectedObject == null)
			{
				MoveSelectionToBestFit();
				return;
			}
			if(currentlySelectedObject.isDropdown && currentlySelectedObject.dropdown.IsExpanded)
			{
				ScrollToSelectedItem();
				return;
			}
			if(timeMoving == 0 || (timesMovedWithoutLettingGo == 1 && timeMoving >= 0.3f) || (timesMovedWithoutLettingGo > 1 && timeMoving >= 0.3f + timesMovedWithoutLettingGo * 0.1f))
			{
				timesMovedWithoutLettingGo++;
				if(currentlySelectedObject.isSlider && Mathf.Abs(moveValue.x) > Mathf.Abs(moveValue.y))
				{
					if(currentlySelectedObject.slider.wholeNumbers)
					{
						currentlySelectedObject.slider.value = Mathf.Clamp(currentlySelectedObject.slider.value + (moveValue.x > 0 ? 1 : -1), currentlySelectedObject.slider.minValue, currentlySelectedObject.slider.maxValue);
					}
					else
					{
						float change = (currentlySelectedObject.slider.minValue + currentlySelectedObject.slider.maxValue) / 20f * (moveValue.x > 0 ? 1 : -1);
						currentlySelectedObject.slider.value = Mathf.Clamp(currentlySelectedObject.slider.value + change, currentlySelectedObject.slider.minValue, currentlySelectedObject.slider.maxValue);
					}
				}
				else if(currentlySelectedObject.isCard && southButtonTimePressed > 0.2f)
				{
					if(Mathf.Abs(moveValue.x) > Mathf.Abs(moveValue.y))
					{
						if(currentlySelectedObject.card.dropZonePlacedIn == null)
						{
							if(moveValue.x > 0)
							{
								if(currentlySelectedObject.card.rt.GetSiblingIndex() < currentlySelectedObject.card.rt.parent.childCount - 1)
								{
									HandArea.instance.ChangeAlwaysSortType(0);
									currentlySelectedObject.card.rt.SetSiblingIndex(currentlySelectedObject.card.rt.GetSiblingIndex() + 1);
									HandArea.instance.ReorganizeHand();
									selectedCardHasBeenMoved = true;
								}
								else
								{
									IndicateCantMove();
								}
							}
							else if(moveValue.x < 0)
							{
								if(currentlySelectedObject.card.rt.GetSiblingIndex() > 0)
								{
									HandArea.instance.ChangeAlwaysSortType(0);
									currentlySelectedObject.card.rt.SetSiblingIndex(currentlySelectedObject.card.rt.GetSiblingIndex() - 1);
									HandArea.instance.ReorganizeHand();
									selectedCardHasBeenMoved = true;
								}
								else
								{
									IndicateCantMove();
								}
							}
						}
						else if(!PlayArea.instance.locked)
						{
							if(moveValue.x > 0)
							{
								DropZone dropZoneToTheRight = PlayArea.instance.GetEmptyDropZoneToTheRight(currentlySelectedObject.card.dropZonePlacedIn);
								if(dropZoneToTheRight == null)
								{
									IndicateCantMove();
								}
								else
								{
									currentlySelectedObject.card.dropZonePlacedIn.CardRemoved();
									currentlySelectedObject.card.dropZonePlacedIn = dropZoneToTheRight;
									dropZoneToTheRight.CardPlaced(currentlySelectedObject.card);
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.movingCardsParent);
									currentlySelectedObject.card.StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(dropZoneToTheRight.rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, dropZoneToTheRight);
									SoundManager.instance.PlayCardDropSound();
									PlayArea.instance.HandUpdated();
									selectedCardHasBeenMoved = true;
								}
							}
							else
							{
								DropZone dropZoneToTheLeft = PlayArea.instance.GetEmptyDropZoneToTheLeft(currentlySelectedObject.card.dropZonePlacedIn);
								if(dropZoneToTheLeft == null)
								{
									IndicateCantMove();
								}
								else
								{
									currentlySelectedObject.card.dropZonePlacedIn.CardRemoved();
									currentlySelectedObject.card.dropZonePlacedIn = dropZoneToTheLeft;
									dropZoneToTheLeft.CardPlaced(currentlySelectedObject.card);
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.movingCardsParent);
									currentlySelectedObject.card.StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(dropZoneToTheLeft.rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, dropZoneToTheLeft);
									SoundManager.instance.PlayCardDropSound();
									PlayArea.instance.HandUpdated();
									selectedCardHasBeenMoved = true;
								}
							}
						}
						else
						{
							IndicateCantMove();
						}
					}
					else
					{
						if(currentlySelectedObject.card.dropZonePlacedIn == null) // in hand
						{
							if(moveValue.y > 0)
							{
								if(PlayArea.instance.GetNumberOfEmptyDropZones(true) > 0)
								{
									List<DropZone> emptyStandardDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.standardDropZones);
									currentlySelectedObject.card.dropZonePlacedIn = emptyStandardDropZones[0];
									emptyStandardDropZones[0].CardPlaced(currentlySelectedObject.card);
									if(currentlySelectedObject.card.cardData.isSpecialCard)
									{
										currentlySelectedObject.card.UpdateToDropZoneImage();
									}
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.movingCardsParent);
									currentlySelectedObject.card.StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(emptyStandardDropZones[0].rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, emptyStandardDropZones[0]);
									HandArea.instance.selectedCards.Clear();
									HandArea.instance.SelectedCardsUpdated();
									PlayArea.instance.HandUpdated();
									HandArea.instance.ReorganizeHand();
									SoundManager.instance.PlayCardDropSound();
									selectedCardHasBeenMoved = true;
								}
								else if(PlayArea.instance.GetNumberOfEmptyDropZones(false) > 0 && currentlySelectedObject.card.cardData.isSpecialCard)
								{
									List<DropZone> emptySpecialDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.specialCardDropZones);
									currentlySelectedObject.card.dropZonePlacedIn = emptySpecialDropZones[0];
									emptySpecialDropZones[0].CardPlaced(currentlySelectedObject.card);
									currentlySelectedObject.card.UpdateToDropZoneImage();
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.movingCardsParent);
									currentlySelectedObject.card.StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(emptySpecialDropZones[0].rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, emptySpecialDropZones[0]);
									HandArea.instance.selectedCards.Clear();
									HandArea.instance.SelectedCardsUpdated();
									PlayArea.instance.HandUpdated();
									HandArea.instance.ReorganizeHand();
									SoundManager.instance.PlayCardDropSound();
									selectedCardHasBeenMoved = true;
								}
								else
								{
									IndicateCantMove();
								}
							}
							else
							{
								IndicateCantMove();
							}
						}
						else	// in drop zone
						{
							if(moveValue.y > 0)
							{
								if(!currentlySelectedObject.card.dropZonePlacedIn.specialCardsOnly && PlayArea.instance.GetNumberOfEmptyDropZones(false) > 0 && currentlySelectedObject.card.cardData.isSpecialCard)
								{
									List<DropZone> emptySpecialDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.specialCardDropZones);
									currentlySelectedObject.card.dropZonePlacedIn.CardRemoved();
									currentlySelectedObject.card.dropZonePlacedIn = emptySpecialDropZones[0];
									emptySpecialDropZones[0].CardPlaced(currentlySelectedObject.card);
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.movingCardsParent);
									currentlySelectedObject.card.StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(emptySpecialDropZones[0].rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, emptySpecialDropZones[0]);
									PlayArea.instance.HandUpdated();
									SoundManager.instance.PlayCardDropSound();
									selectedCardHasBeenMoved = true;
								}
								else
								{
									IndicateCantMove();
								}
							}
							else	// moving down
							{
								if(currentlySelectedObject.card.dropZonePlacedIn.specialCardsOnly && PlayArea.instance.GetNumberOfEmptyDropZones(true) > 0)
								{
									List<DropZone> emptyStandardDropZones = PlayArea.instance.GetEmptyDropZonesFromArray(PlayArea.instance.standardDropZones);
									currentlySelectedObject.card.dropZonePlacedIn.CardRemoved();
									currentlySelectedObject.card.dropZonePlacedIn = emptyStandardDropZones[0];
									emptyStandardDropZones[0].CardPlaced(currentlySelectedObject.card);
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.movingCardsParent);
									currentlySelectedObject.card.StartMove(LocalInterface.instance.GetCanvasPositionOfRectTransform(emptyStandardDropZones[0].rt, GameManager.instance.gameplayCanvas), Vector3.zero, true, false, false, false, emptyStandardDropZones[0]);
									PlayArea.instance.HandUpdated();
									SoundManager.instance.PlayCardDropSound();
									selectedCardHasBeenMoved = true;
								}
								else if((currentlySelectedObject.card.dropZonePlacedIn.specialCardsOnly && PlayArea.instance.GetNumberOfEmptyDropZones(true) <= 0) || !currentlySelectedObject.card.dropZonePlacedIn.specialCardsOnly)
								{
									currentlySelectedObject.card.dropZonePlacedIn.CardRemoved();
									currentlySelectedObject.card.dropZonePlacedIn = null;
									currentlySelectedObject.card.RevertToOriginalImage();
									currentlySelectedObject.card.transform.SetParent(HandArea.instance.cardParent);
									currentlySelectedObject.card.transform.SetSiblingIndex(0);
									PlayArea.instance.HandUpdated();
									SoundManager.instance.PlayCardSlideSound();
									HandArea.instance.ReorganizeHand();
									HandArea.instance.recallButton.ChangeButtonEnabled(PlayArea.instance.AreThereAnyPlacedCards());
									selectedCardHasBeenMoved = true;
								}
								else
								{
									IndicateCantMove();
								}
							}
						}
					}
				}
				else if(currentlySelectedObject.isCard && Mathf.Abs(moveValue.x) > Mathf.Abs(moveValue.y) && currentlySelectedObject.card.rt.parent == HandArea.instance.cardParent && (currentlySelectedObject.card.rt.GetSiblingIndex() != 0 || moveValue.x > 0) && (currentlySelectedObject.card.rt.GetSiblingIndex() < currentlySelectedObject.card.rt.parent.childCount - 1 || moveValue.x < 0))
				{
					if(moveValue.x > 0)
					{
						MoveSelectionToObject(currentlySelectedObject.card.rt.parent.GetChild(currentlySelectedObject.card.rt.GetSiblingIndex() + 1).GetComponent<ControllerSelectableObject>(), HandArea.instance.cardsControllerSelectionGroup);
					}
					else
					{
						MoveSelectionToObject(currentlySelectedObject.card.rt.parent.GetChild(currentlySelectedObject.card.rt.GetSiblingIndex() - 1).GetComponent<ControllerSelectableObject>(), HandArea.instance.cardsControllerSelectionGroup);
					}
				}
				else
				{
					ControllerSelectableObject bestObject = null;
					ControllerSelectionGroup bestGroup = null;
					float highestScore = float.MinValue;
					ControllerSelectionGroup highestAvailabilityPriorityGroup = GetControllerSelectionGroupWithHighestAvailabilityPriority();
					int highestAvailabilityPriority = int.MinValue;
					if(highestAvailabilityPriorityGroup != null)
					{
						highestAvailabilityPriority = highestAvailabilityPriorityGroup.availabilityPriority;
					}
					for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
					{
						if(currentControllerSelectionGroups[i].availabilityPriority < highestAvailabilityPriority)
						{
							continue;
						}
						for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
						{
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j] == null)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].unselectable)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j] == currentlySelectedObject)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.GetButtonEnabled())
							{
								continue;
							}
							if(!currentControllerSelectionGroups[i].controllerSelectableObjects[j].gameObject.activeSelf || !currentControllerSelectionGroups[i].controllerSelectableObjects[j].gameObject.activeInHierarchy)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isToggle && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].toggle.interactable)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isSlider && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].slider.interactable)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isDropdown && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].dropdown.interactable)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isScrollView)
							{
								continue;
							}
							if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isInputField && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].inputField.interactable)
							{
								continue;
							}
							Vector2 directionToCandidate = LocalInterface.instance.GetCanvasPositionOfRectTransform(currentControllerSelectionGroups[i].controllerSelectableObjects[j].rt, currentControllerSelectionGroups[i].canvas) - LocalInterface.instance.GetCanvasPositionOfRectTransform(currentlySelectedObject.rt, currentControllerSelectionGroups[i].canvas);
							float distance = directionToCandidate.magnitude;
							Vector2 normalizedDirectionToCandidate = directionToCandidate.normalized;
							float alignmentScore = Vector2.Dot(moveValue, normalizedDirectionToCandidate);
							float finalScore = alignmentScore - (distance * 0.0003f);
							if(finalScore > highestScore)
							{
								highestScore = finalScore;
								bestObject = currentControllerSelectionGroups[i].controllerSelectableObjects[j];
								bestGroup = currentControllerSelectionGroups[i];
							}
						}
					}
					if(bestObject == null || bestGroup == null)
					{
						// play a little "nuh uh" sound and retrigger the movement animation right where it is
					}
					else
					{
						MoveSelectionToObject(bestObject, bestGroup);
					}
				}
			}
			timeMoving += Time.deltaTime;
		}
	}
	
	public ControllerSelectableObject FindClosestActiveObject(Vector2 origin)
	{
		ControllerSelectableObject bestObject = null;
		ControllerSelectionGroup bestGroup = null;
		float closestDistance = float.MaxValue;
		ControllerSelectionGroup highestAvailabilityPriorityGroup = GetControllerSelectionGroupWithHighestAvailabilityPriority();
		int highestAvailabilityPriority = int.MinValue;
		if(highestAvailabilityPriorityGroup != null)
		{
			highestAvailabilityPriority = highestAvailabilityPriorityGroup.availabilityPriority;
		}
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			if(currentControllerSelectionGroups[i].availabilityPriority < highestAvailabilityPriority)
			{
				continue;
			}
			for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
			{
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j] == null)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].unselectable)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j] == currentlySelectedObject)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.GetButtonEnabled())
				{
					continue;
				}
				if(!currentControllerSelectionGroups[i].controllerSelectableObjects[j].gameObject.activeSelf || !currentControllerSelectionGroups[i].controllerSelectableObjects[j].gameObject.activeInHierarchy)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isToggle && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].toggle.interactable)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isSlider && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].slider.interactable)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isDropdown && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].dropdown.interactable)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isScrollView)
				{
					continue;
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isInputField && !currentControllerSelectionGroups[i].controllerSelectableObjects[j].inputField.interactable)
				{
					continue;
				}
				Vector2 directionToCandidate = LocalInterface.instance.GetCanvasPositionOfRectTransform(currentControllerSelectionGroups[i].controllerSelectableObjects[j].rt, currentControllerSelectionGroups[i].canvas) - origin;
				float distance = directionToCandidate.magnitude;
				if(distance < closestDistance)
				{
					closestDistance = distance;
					bestObject = currentControllerSelectionGroups[i].controllerSelectableObjects[j];
					bestGroup = currentControllerSelectionGroups[i];
				}
			}
		}
		return bestObject;
	}
	
	public void IndicateCantMove()
	{
		// jiggle
	}
	
	public void CheckRightStickInput()
	{
		Vector2 rightStickValue = rightStickAction.ReadValue<Vector2>();
		if(Mathf.Abs(rightStickValue.x) < Preferences.instance.controllerDeadzone && Mathf.Abs(rightStickValue.y) < Preferences.instance.controllerDeadzone)
		{
			// do nothing
			rightStickTimesMovedWithoutLettingGo = 0;
			rightStickTimeMoving = 0;
		}
		else
		{
			if(!usingController)
			{
				ActivateControllerInput();
				return;
			}
			ControllerSelectionGroup highestAvailabilityPriorityGroup = GetControllerSelectionGroupWithHighestAvailabilityPriority();
			int highestAvailabilityPriority = int.MinValue;
			if(highestAvailabilityPriorityGroup != null)
			{
				highestAvailabilityPriority = highestAvailabilityPriorityGroup.availabilityPriority;
			}
			for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
			{
				if(currentControllerSelectionGroups[i].availabilityPriority < highestAvailabilityPriority)
				{
					continue;
				}
				for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
				{
					if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isScrollView)
					{
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].scrollViewHorizontalScrollbar != null && Mathf.Abs(rightStickValue.x) > Mathf.Abs(rightStickValue.y))
						{
							
						}
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].scrollViewVerticalScrollbar != null && Mathf.Abs(rightStickValue.x) <= Mathf.Abs(rightStickValue.y))
						{
							currentControllerSelectionGroups[i].controllerSelectableObjects[j].scrollViewVerticalScrollbar.value = Mathf.Clamp(currentControllerSelectionGroups[i].controllerSelectableObjects[j].scrollViewVerticalScrollbar.value + rightStickValue.y * Time.deltaTime, 0, 1f);
							if(currentlySelectedObject != null && currentlySelectedObject.isElementInScrollView && currentlySelectedObject.scrollViewVerticalScrollbar != null)
							{
								MoveSelectionToBestFit(true);
							}
						}
					}
				}
			}
			if(OnScreenKeyboard.instance.visibilityObject.activeSelf)
			{
				if(rightStickTimeMoving == 0 || (rightStickTimesMovedWithoutLettingGo == 1 && rightStickTimeMoving >= 0.3f) || (rightStickTimesMovedWithoutLettingGo > 1 && rightStickTimeMoving >= 0.3f + rightStickTimesMovedWithoutLettingGo * 0.1f))
				{
					if(Mathf.Abs(rightStickValue.x) > Mathf.Abs(rightStickValue.y))
					{
						rightStickTimesMovedWithoutLettingGo++;
						if(rightStickValue.x > 0)
						{
							OnScreenKeyboard.instance.MoveCaretRight();
						}
						else
						{
							OnScreenKeyboard.instance.MoveCaretLeft();
						}
					}
				}
			}
			rightStickTimeMoving += Time.deltaTime;
		}
	}
	
	public void ScrollToSelectedItem()
	{
		GameObject selected = EventSystem.current.currentSelectedGameObject;
		int selectedIndex = selected.transform.GetSiblingIndex();
		if(currentlySelectedObject.dropdownScrollbar == null)
		{
			currentlySelectedObject.dropdownScrollbar = currentlySelectedObject.dropdown.transform.Find("Dropdown List").GetComponentInChildren<Scrollbar>(true);
		}
		int totalItems = currentlySelectedObject.dropdown.options.Count;
		float normalizedPosition = 1f - ((float)(selectedIndex - 1) / (totalItems - 1));
		currentlySelectedObject.dropdownScrollbar.value = normalizedPosition;
	}
	
	public void SouthButtonPerformed()
	{
		if(!usingController)
		{
			ActivateControllerInput();
			return;
		}
		if(currentControllerSelectionGroups.Count <= 0 || currentlySelectedObject == null)
		{
			return;
		}
		if(currentlySelectedObject.gameObject.activeInHierarchy)
		{
			if(currentlySelectedObject.isButton && currentlySelectedObject.buttonPlus.GetButtonEnabled())
			{
				currentlySelectedObject.buttonPlus.mouseOverButton = true;
				currentlySelectedObject.buttonPlus.StartClickingButton();
			}
			if(currentlySelectedObject.isInputField && currentlySelectedObject.inputField.interactable)
			{
				OnScreenKeyboard.instance.SetupOnScreenKeyboard(currentlySelectedObject.inputField, currentlySelectedObject);
			}
		}
	}
	
	public void SouthButtonCompleted()
	{
		if(!usingController)
		{
			ActivateControllerInput();
			return;
		}
		if(currentControllerSelectionGroups.Count <= 0 || currentlySelectedObject == null)
		{
			return;
		}
		if(currentlySelectedObject.isButton)
		{
			currentlySelectedObject.buttonPlus.ExectuteButtonPress();
		}
		else if(currentlySelectedObject.isToggle)
		{
			currentlySelectedObject.toggle.isOn = !currentlySelectedObject.toggle.isOn;
		}
		else if(currentlySelectedObject.isDropdown)
		{
			if(currentlySelectedObject.dropdown.IsExpanded)
			{
				// currentlySelectedObject.dropdown.Hide();
			}
			else
			{
				currentlySelectedObject.dropdown.Show();
				ScrollToSelectedItem();
			}
		}
		if(currentlySelectedObject != null && currentlySelectedObject.isCard && !selectedCardHasBeenMoved && currentlySelectedObject.card.rt.parent == HandArea.instance.cardParent)
		{
			currentlySelectedObject.card.OnPointerClick(new PointerEventData(EventSystem.current));
			RepositionControllerSelectionRT(currentlySelectedObject, HandArea.instance.cardsControllerSelectionGroup);
		}
	}
	
	public ControllerSelectableObject HotkeyButtonPerformed(string hotkeyActionName) // returns null if none found
	{
		if(!usingController)
		{
			ActivateControllerInput();
			return null;
		}
		ControllerSelectionGroup highestAvailabilityPriorityControllerSelectionGroup = GetControllerSelectionGroupWithHighestAvailabilityPriority();
		int highestAvailabilityPriority = int.MinValue;
		if(highestAvailabilityPriorityControllerSelectionGroup != null)
		{
			highestAvailabilityPriority = highestAvailabilityPriorityControllerSelectionGroup.availabilityPriority;
		}
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			if(currentControllerSelectionGroups[i].availabilityPriority < highestAvailabilityPriority)
			{
				continue;
			}
			for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
			{
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hasHotkey && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyActionName == hotkeyActionName)
				{
					if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton && currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.GetButtonEnabled() && currentControllerSelectionGroups[i].controllerSelectableObjects[j].gameObject.activeInHierarchy)
					{
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isSpecial && (currentControllerSelectionGroups[i].controllerSelectableObjects[j].specialTag == "DrawPile" || currentControllerSelectionGroups[i].controllerSelectableObjects[j].specialTag == "DiscardPile"))
						{
							return currentControllerSelectionGroups[i].controllerSelectableObjects[j];
						}
						currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.mouseOverButton = true;
						currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.StartClickingButton();
						return currentControllerSelectionGroups[i].controllerSelectableObjects[j];
					}
				}
			}
		}
		return null;
	}
	
	public ControllerSelectableObject HotkeyButtonCompleted(string hotkeyActionName)
	{
		ControllerSelectionGroup highestAvailabilityPriorityControllerSelectionGroup = GetControllerSelectionGroupWithHighestAvailabilityPriority();
		int highestAvailabilityPriority = int.MinValue;
		if(highestAvailabilityPriorityControllerSelectionGroup != null)
		{
			highestAvailabilityPriority = highestAvailabilityPriorityControllerSelectionGroup.availabilityPriority;
		}
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			if(currentControllerSelectionGroups[i].availabilityPriority < highestAvailabilityPriority && !currentControllerSelectionGroups[i].ignoreAvailablilityPriority)
			{
				continue;
			}
			for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
			{
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hasHotkey && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyActionName == hotkeyActionName)
				{
					if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton && currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.GetButtonEnabled() && currentControllerSelectionGroups[i].controllerSelectableObjects[j].gameObject.activeInHierarchy)
					{
						ControllerSelectableObject objectToReturn = currentControllerSelectionGroups[i].controllerSelectableObjects[j];
						currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.ExectuteButtonPress();
						return objectToReturn;
					}
					if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isSlideOut && currentControllerSelectionGroups[i].controllerSelectableObjects[j].slideOut.rt.GetChild(0).gameObject.activeInHierarchy)
					{
						List<SlideOut> slideOutsToDisable = new List<SlideOut>();
						List<SlideOut> slideOutsToEnable = new List<SlideOut>();
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].slideOut.mouseOver)
						{
							slideOutsToDisable.Add(currentControllerSelectionGroups[i].controllerSelectableObjects[j].slideOut);
						}
						else
						{
							slideOutsToEnable.Add(currentControllerSelectionGroups[i].controllerSelectableObjects[j].slideOut);
						}
						for(int k = 0; k < currentControllerSelectionGroups.Count; k++)
						{
							for(int l = 0; l < currentControllerSelectionGroups[k].controllerSelectableObjects.Count; l++)
							{
								if(currentControllerSelectionGroups[k] != null && currentControllerSelectionGroups[k].controllerSelectableObjects[l] != null && currentControllerSelectionGroups[k].controllerSelectableObjects[l].isSlideOut && currentControllerSelectionGroups[k].controllerSelectableObjects[l].slideOut.mouseOver && currentControllerSelectionGroups[k].controllerSelectableObjects[l] != currentControllerSelectionGroups[i].controllerSelectableObjects[j])
								{
									slideOutsToDisable.Add(currentControllerSelectionGroups[k].controllerSelectableObjects[l].slideOut);
								}
							}
						}
						for(int k = 0; k < slideOutsToDisable.Count; k++)
						{
							slideOutsToDisable[k].OnPointerExit(new PointerEventData(EventSystem.current));
						}
						for(int k = 0; k < slideOutsToEnable.Count; k++)
						{
							slideOutsToEnable[k].OnPointerEnter(new PointerEventData(EventSystem.current));
						}
						return currentControllerSelectionGroups[i].controllerSelectableObjects[j];
					}
				}
			}
		}
		return null;
	}

	public void ActivateControllerInput()
	{
		if(currentControllerSelectionGroups.Count <= 0)
		{
			return;
		}
		usingController = true;
		Cursor.visible = false;
		visibilityObject.SetActive(true);
		MoveSelectionToBestFit();
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
			{
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hasHotkey)
				{
					currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.UpdateHotkey();
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hasHotkey && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage != null)
				{
					currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage.gameObject.SetActive(true);
					currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage.sprite = LocalInterface.instance.GetHotkeySpriteForAction(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyActionName, true);
				}
			}
		}
		if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene" && V.i.isTutorial && (!Tutorial.instance.tutorialFinished || Tutorial.instance.displayingTips))
		{
			Tutorial.instance.ChangeToControllerText();
		}
	}
	
	public void MoveSelectionIfCurrentlySelectedObjectIsNoLongerInCurrentControllerSelectionGroups()
	{
		if(!usingController)
		{
			return;
		}
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
			{
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j] == currentlySelectedObject)
				{
					return;
				}
			}
		}
		MoveSelectionToBestFit();
	}
	
	public void MoveSelectionToBestFit(bool ignoreLastSelectedObject = false)
	{
		if(currentControllerSelectionGroups.Count <= 0)
		{
			return;
		}
		if(!usingController)
		{
			return;
		}
		currentControllerSelectionGroups.Sort((x, y) =>
		{
			int availabilityPriorityComparison = y.availabilityPriority - x.availabilityPriority;
			if(availabilityPriorityComparison != 0)
			{
				return availabilityPriorityComparison;
			}
			return y.priority.CompareTo(x.priority);
		});
		// Debug.Log($"MoveSelectionToBestFit best group name= {currentControllerSelectionGroups[0].gameObject.name}");
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			if(!currentControllerSelectionGroups[i].neverAutoSelect)
			{
				if(currentControllerSelectionGroups[i].lastSelectedObject != null && currentControllerSelectionGroups[i].lastSelectedObject.gameObject.activeInHierarchy && !ignoreLastSelectedObject)
				{
					if((currentControllerSelectionGroups[i].lastSelectedObject.isButton && currentControllerSelectionGroups[i].lastSelectedObject.buttonPlus.GetButtonEnabled()) || (currentControllerSelectionGroups[i].lastSelectedObject.isToggle && currentControllerSelectionGroups[i].lastSelectedObject.toggle.interactable) || (currentControllerSelectionGroups[i].lastSelectedObject.isSlider && currentControllerSelectionGroups[i].lastSelectedObject.slider.interactable) || (currentControllerSelectionGroups[i].lastSelectedObject.isDropdown && currentControllerSelectionGroups[i].lastSelectedObject.dropdown.interactable) || (currentControllerSelectionGroups[i].lastSelectedObject.isInputField && currentControllerSelectionGroups[i].lastSelectedObject.inputField.interactable))
					{
						MoveSelectionToObject(currentControllerSelectionGroups[i].lastSelectedObject, currentControllerSelectionGroups[i]);
						return;
					}
					else if(currentControllerSelectionGroups[i].defaultObject != null && currentControllerSelectionGroups[i].defaultObject.gameObject.activeInHierarchy)
					{
						MoveSelectionToObject(currentControllerSelectionGroups[i].defaultObject, currentControllerSelectionGroups[i]);
						return;
					}
					
				}
				else if(currentControllerSelectionGroups[i].defaultObject != null && currentControllerSelectionGroups[i].defaultObject.gameObject.activeInHierarchy)
				{
					MoveSelectionToObject(currentControllerSelectionGroups[i].defaultObject, currentControllerSelectionGroups[i]);
					return;
				}
				if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene" && currentControllerSelectionGroups[i] == HandArea.instance.cardsControllerSelectionGroup)
				{
					List<Card> cardsInHandAndControllerSelectionGroup = new List<Card>();
					for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
					{
						if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isCard && currentControllerSelectionGroups[i].controllerSelectableObjects[j].card.dropZonePlacedIn == null)
						{
							cardsInHandAndControllerSelectionGroup.Add(currentControllerSelectionGroups[i].controllerSelectableObjects[i].card);
						}
					}
					if(cardsInHandAndControllerSelectionGroup.Count > 0)
					{
						cardsInHandAndControllerSelectionGroup.Sort((a, b) =>
						{
							float aDistance = Vector2.Distance(LocalInterface.instance.GetCanvasPositionOfRectTransform(a.rt, GameManager.instance.gameplayCanvas), LocalInterface.instance.GetCanvasPositionOfRectTransform(controllerSelectionRT, LocalInterface.instance.persistentCanvas));
							float bDistance = Vector2.Distance(LocalInterface.instance.GetCanvasPositionOfRectTransform(b.rt, GameManager.instance.gameplayCanvas), LocalInterface.instance.GetCanvasPositionOfRectTransform(controllerSelectionRT, LocalInterface.instance.persistentCanvas));
							return aDistance.CompareTo(bDistance);
						});
						MoveSelectionToObject(cardsInHandAndControllerSelectionGroup[i].controllerSelectableObject, HandArea.instance.cardsControllerSelectionGroup);
						return;
					}
				}
			}
		}
		// Debug.Log("DeactivateSelection");
		DeactivateSelection();
	}
	
	public void DeactivateSelection()
	{
		if(currentlySelectedObject != null)
		{
			if(currentlySelectedObject.hasTooltip)
			{
				currentlySelectedObject.tooltipObject.DisableTooltip();
			}
			if(currentlySelectedObject.isCard && currentlySelectedObject.card.cardData.isSpecialCard)
			{
				currentlySelectedObject.card.tooltipObject.DisableTooltip();
			}
		}
		
		currentlySelectedObject = null;
		visibilityObject.SetActive(false);
	}
	
	public void MoveSelectionToObject(ControllerSelectableObject newObject, ControllerSelectionGroup newGroup)
	{
		if(currentlySelectedObject != null)
		{
			if(currentlySelectedObject.isButton && currentlySelectedObject.buttonPlus.holdingDown)
			{
				currentlySelectedObject.buttonPlus.MouseExit();
			}
			if(currentlySelectedObject.hasTooltip)
			{
				currentlySelectedObject.tooltipObject.DisableTooltip();
			}
			if(currentlySelectedObject.isCard)
			{
				currentlySelectedObject.card.OnPointerExit(new PointerEventData(EventSystem.current));
				if(currentlySelectedObject.card.cardData.isSpecialCard)
				{
					currentlySelectedObject.card.tooltipObject.DisableTooltip();
				}
			}
			if(currentlySelectedObject.isHandInfoHand)
			{
				// currentlySelectedObject.handInfo.OnMouseExitHandName();
				HandInfoTooltip.instance.DisableTooltip();
			}
			if(currentlySelectedObject.isHandInfoIndividual || currentlySelectedObject.isHandInfoMinimum)
			{
				currentlySelectedObject.handInfo.OnMouseExit();
			}
		}
		if(!usingController)
		{
			return;
		}
		visibilityObject.SetActive(true);
		controllerSelectionImage.color = newObject.borderColor;
		currentlySelectedObject = newObject;
		newGroup.lastSelectedObject = currentlySelectedObject;
		if(!alreadyCheckedControllerType)
		{
			Preferences.instance.glyphSet = GetControllerType();
			Preferences.instance.SetUIToCurrentOptions();
			Preferences.instance.SetPreferencesFileToCurrentSettings();
			alreadyCheckedControllerType = true;
		}
		if(newObject.isElementInScrollView)
		{
			if(newObject.scrollViewVerticalScrollbar != null && newObject.scrollViewContentRT != null)
			{
				float fullSize = newObject.scrollViewContentRT.sizeDelta.y;
				float position = Mathf.Abs(newObject.positionInScrollView);
				float positionNormalized = Mathf.Clamp(1f - (position + 30f) / fullSize, 0, 1f);
				float positionAlter = (positionNormalized - 0.5f) / 7.5f;
				// Debug.Log($"fullSize={fullSize}, position={position}");
				newObject.scrollViewVerticalScrollbar.value = Mathf.Clamp(positionNormalized + positionAlter, 0, 1);
			}
		}
		RepositionControllerSelectionRT(newObject, newGroup);
		if(newObject.isCard && newObject.card.cardData.isSpecialCard && Preferences.instance.showSpecialCardTooltips)
		{
			newObject.card.tooltipObject.DisplayTooltip(true);
		}
		if(newObject.hasTooltip)
		{
			if(newObject.isShopItem)
			{
				if(newObject.shopItem.itemType == "Bauble" && Preferences.instance.showBaubleTooltips)
				{
					newObject.tooltipObject.DisplayTooltip(true);
				}
				else if(newObject.shopItem.itemType == "Card" && Preferences.instance.showSpecialCardTooltips)
				{
					newObject.shopItem.card.tooltipObject.DisplayTooltip(true);
				}
				else if(newObject.shopItem.itemType == "Zodiac" && Preferences.instance.showZodiacTooltips)
				{
					newObject.tooltipObject.DisplayTooltip(true);
				}
			}
			else if(newObject.tooltipObject.isBauble && Preferences.instance.showBaubleTooltips)
			{
				newObject.tooltipObject.DisplayTooltip(true);
			}
			else if(newObject.tooltipObject.isSpecialCard && Preferences.instance.showSpecialCardTooltips)
			{
				newObject.card.tooltipObject.DisplayTooltip(true);
			}
			else if(newObject.tooltipObject.isZodiac && Preferences.instance.showZodiacTooltips)
			{
				newObject.tooltipObject.DisplayTooltip(true);
			}
			else
			{
				newObject.tooltipObject.DisplayTooltip(true);
			}
		}
		if(newObject.isCard)
		{
			newObject.card.OnPointerEnter(new PointerEventData(EventSystem.current));
		}
		if(newObject.isHandInfoHand && Preferences.instance.showHandTooltips)
		{
			newObject.handInfo.OnMouseEnterHandName(true);
		}
		if(newObject.isHandInfoIndividual) 
		{
			newObject.handInfo.OnMouseEnterIndividual();
		}
		if(newObject.isHandInfoMinimum)
		{
			newObject.handInfo.OnMouseEnterMinimum();
		}
	}
	
	public void RepositionControllerSelectionRT(ControllerSelectableObject newObject, ControllerSelectionGroup newGroup)
	{
		if(newObject == null || newGroup == null)
		{
			return;
		}
		controllerSelectionRT.anchoredPosition = LocalInterface.instance.GetCanvasPositionOfRectTransform(newObject.rt, newGroup.canvas);
		controllerSelectionRT.sizeDelta = newObject.rt.rect.size;
		if(newObject.isButton)
		{
			if(newObject.buttonPlus.buttonImage.sprite == LocalInterface.instance.circleFourPixel)
			{
				controllerSelectionImage.sprite = borderFourPixel;
				controllerSelectionRT.anchoredPosition += new Vector2(0, -2);
				controllerSelectionRT.sizeDelta += new Vector2(0, 4);
			}
			else if(newObject.buttonPlus.buttonImage.sprite == LocalInterface.instance.circleEightPixel)
			{
				controllerSelectionImage.sprite = borderEightPixel;
				controllerSelectionRT.anchoredPosition += new Vector2(0, -2);
				controllerSelectionRT.sizeDelta += new Vector2(0, 4);
			}
			else
			{
				controllerSelectionImage.sprite = borderFourPixel;
			}
		}
		else
		{
			controllerSelectionImage.sprite = borderFourPixel;
		}
		if(newObject.isCard)
		{
			controllerSelectionRT.localRotation = newObject.card.rt.localRotation;
		}
		else
		{
			controllerSelectionRT.localRotation = Quaternion.identity;
		}
		if(newObject.isCard && newObject.card.cardData.isSpecialCard && Preferences.instance.showSpecialCardTooltips)
		{
			newObject.card.tooltipObject.DisplayTooltip(true);
		}
		if(newObject.hasTooltip)
		{
			if(newObject.isShopItem)
			{
				if(newObject.shopItem.itemType == "Bauble" && Preferences.instance.showBaubleTooltips)
				{
					newObject.tooltipObject.DisplayTooltip(true);
				}
				else if(newObject.shopItem.itemType == "Card" && Preferences.instance.showSpecialCardTooltips)
				{
					newObject.shopItem.card.tooltipObject.DisplayTooltip(true);
				}
				else if(newObject.shopItem.itemType == "Zodiac" && Preferences.instance.showZodiacTooltips)
				{
					newObject.tooltipObject.DisplayTooltip(true);
				}
			}
			else if(newObject.tooltipObject.isBauble && Preferences.instance.showBaubleTooltips)
			{
				newObject.tooltipObject.DisplayTooltip(true);
			}
			else if(newObject.tooltipObject.isSpecialCard && Preferences.instance.showSpecialCardTooltips)
			{
				newObject.card.tooltipObject.DisplayTooltip(true);
			}
			else if(newObject.tooltipObject.isZodiac && Preferences.instance.showZodiacTooltips)
			{
				newObject.tooltipObject.DisplayTooltip(true);
			}
			else
			{
				newObject.tooltipObject.DisplayTooltip(true);
			}
		}
	}
	
	public void AddControllerSelectionGroup(ControllerSelectionGroup newControllerSelectionGroup)
	{
		ControllerSelectionGroup oldHighestAvailablityPriorityGroup = GetControllerSelectionGroupWithHighestAvailabilityPriority();
		int oldHighestAvailablityPriority = int.MinValue;
		if(oldHighestAvailablityPriorityGroup != null)
		{
			oldHighestAvailablityPriority = oldHighestAvailablityPriorityGroup.availabilityPriority;
		}
		if(!currentControllerSelectionGroups.Contains(newControllerSelectionGroup))
		{
			currentControllerSelectionGroups.Add(newControllerSelectionGroup);
		}
		if(usingController)
		{
			if(currentlySelectedObject == null)
			{
				MoveSelectionToBestFit();
			}
			if(newControllerSelectionGroup.availabilityPriority > oldHighestAvailablityPriority)
			{
				MoveSelectionToBestFit();
			}
			for(int i = 0; i < newControllerSelectionGroup.controllerSelectableObjects.Count; i++)
			{
				if(newControllerSelectionGroup.controllerSelectableObjects[i].isButton && newControllerSelectionGroup.controllerSelectableObjects[i].hasHotkey && newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage == null)
				{
					newControllerSelectionGroup.controllerSelectableObjects[i].buttonPlus.UpdateHotkey();
				}
				if(newControllerSelectionGroup.controllerSelectableObjects[i].hasHotkey && newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage != null)
				{
					newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage.gameObject.SetActive(true);
					newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage.sprite = LocalInterface.instance.GetHotkeySpriteForAction(newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyActionName, true);
				}
			}
		}
		else
		{
			for(int i = 0; i < newControllerSelectionGroup.controllerSelectableObjects.Count; i++)
			{
				if(newControllerSelectionGroup.controllerSelectableObjects[i].isButton && newControllerSelectionGroup.controllerSelectableObjects[i].hasHotkey && newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage == null)
				{
					newControllerSelectionGroup.controllerSelectableObjects[i].buttonPlus.DisableHotkey();
				}
				if(newControllerSelectionGroup.controllerSelectableObjects[i].hasHotkey && newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage != null)
				{
					newControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage.gameObject.SetActive(false);
				}
			}
		}
	}
	
	public ControllerSelectionGroup GetControllerSelectionGroupWithHighestAvailabilityPriority()
	{
		ControllerSelectionGroup highestAvailabilityPriorityControllerSelectionGroup = null;
		int highestAvailabilityPriority = int.MinValue;
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			if(currentControllerSelectionGroups[i].availabilityPriority > highestAvailabilityPriority)
			{
				highestAvailabilityPriority = currentControllerSelectionGroups[i].availabilityPriority;
				highestAvailabilityPriorityControllerSelectionGroup = currentControllerSelectionGroups[i];
			}
		}
		return highestAvailabilityPriorityControllerSelectionGroup;
	}
	
	public void RemoveControllerSelectionGroup(ControllerSelectionGroup removingControllerSelectionGroup)
	{
		for(int i = 0; i < removingControllerSelectionGroup.controllerSelectableObjects.Count; i++)
		{
			if(removingControllerSelectionGroup.controllerSelectableObjects[i] == currentlySelectedObject)
			{
				DeactivateSelection();
			}
			if(removingControllerSelectionGroup.controllerSelectableObjects[i].hasHotkey && removingControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage != null)
			{
				removingControllerSelectionGroup.controllerSelectableObjects[i].hotkeyImage.sprite = LocalInterface.instance.GetHotkeySpriteForAction(removingControllerSelectionGroup.controllerSelectableObjects[i].hotkeyActionName, true);
			}
		}
		if(currentControllerSelectionGroups.Contains(removingControllerSelectionGroup))
		{
			currentControllerSelectionGroups.Remove(removingControllerSelectionGroup);
		}
		if(currentlySelectedObject == null && usingController)
		{
			MoveSelectionToBestFit();
		}
	}
	
	public void UpdateHotkeys()
	{
		if(!usingController)
		{
			return;
		}
		for(int i = 0; i < currentControllerSelectionGroups.Count; i++)
		{
			for(int j = 0; j < currentControllerSelectionGroups[i].controllerSelectableObjects.Count; j++)
			{
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].isButton && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hasHotkey && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage == null)
				{
					currentControllerSelectionGroups[i].controllerSelectableObjects[j].buttonPlus.UpdateHotkey();
				}
				if(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hasHotkey && currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage != null)
				{
					currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage.gameObject.SetActive(true);
					currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyImage.sprite = LocalInterface.instance.GetHotkeySpriteForAction(currentControllerSelectionGroups[i].controllerSelectableObjects[j].hotkeyActionName, true);
				}
			}
		}
	}
	
	public int GetControllerType()
	{
		string[] joystickNames = Input.GetJoystickNames();
		if(joystickNames.Length > 0)
		{
			string joystickName = joystickNames[0];
			if(joystickName.ToLower().Contains("xbox"))
			{
				return 1;
			}
			else if(joystickName.ToLower().Contains("playstation"))
			{
				return 2;
			}
			else
			{
				return 0;
			}
		}
		return -1;
	}
}
