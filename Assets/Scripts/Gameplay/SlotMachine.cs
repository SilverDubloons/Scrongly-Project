using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using static Deck;

public class SlotMachine : MonoBehaviour
{
    public RectTransform slotMachineImageRT;
	public RectTransform slotMachineImageSingleRT;
	public RectTransform cardsParentRT;
	public RectTransform[] columns;
	public HandScoring[] handScorings; // rows
	public GameObject visibilityObject;
	public ButtonPlus leverButton;
	public RectTransform leverRT;
	public RectTransform leverHandleRT;
	public RectTransform resultsPanelBackdrop;
	public Label pointsLabel;
	public Label lowerEndLabel;
	public Label upperEndLabel;
	public Label totalChipsEarnedLabel;
	public ButtonPlus resultsButton;
	public RectTransform chipParent;
	public Chip currentChip;
	public RectTransform currentScoreFill;
	public AudioSource spinningSource;
	
	public float slotSpeed;
	public AnimationCurve spinCurve;
	public AnimationCurve windDownCurve;
	public Vector2 leverTopPosition;
	public Vector2 leverBottomPosition;
	public Vector3 leverTopRotation;
	public Vector3 leverBottomRotation;
	public double lowestPointThreshold;
	public double pointThresholdFactor;
	
	List<Card>[] columnCards = new List<Card>[7];
	List<CardData> cardDatas = new List<CardData>();
	
	public bool spinning;
	public bool[] columnSpinning = new bool[7];
	public bool[] columnWindingDown = new bool[7];
	public float[] columnSpinTime = new float[7];
	public IEnumerator spinCoroutine;
	public int currentWidth;
	public int currentHeight;
	public int spinsRemaining;
	public bool waitingForReturnClick;
	public const float currentScoreFillSize = 244f;
	public int chipsEarned;
	
	public static SlotMachine instance;
	
	public void SetupInstance()
	{
		instance = this;
		visibilityObject.SetActive(false);
	}
	
/* 	void Update()
	{
		if(Input.GetKeyDown(KeyCode.O))
		{
			SetupSlotMachine(7, 7);
		}
		if(Input.GetKeyDown(KeyCode.P))
		{
			StartSpin();
		}
	} */
	
	public void SetInteractability(bool enabledState)
	{
		leverButton.ChangeButtonEnabled(enabledState);
	}
	
	public void SetupSlotMachine(int width, int height, int spins)
	{
		CardValuesTooltip.instance.HideTooltip();
		MovingObjects.instance.mo["PlayArea"].StartMove("OffScreen");
		MovingObjects.instance.mo["HandArea"].StartMove("OffScreen");
		MovingObjects.instance.mo["BossInformation"].StartMove("OffScreen");
		MovingObjects.instance.mo["CardParent"].StartMove("OffScreen");
		MovingObjects.instance.mo["SlotMachine"].StartMove("OnScreen");
		visibilityObject.SetActive(true);
		resultsPanelBackdrop.gameObject.SetActive(false);
		leverRT.anchoredPosition = new Vector2(24f * width + 122f, 0);
		leverHandleRT.anchoredPosition = leverTopPosition;
		leverHandleRT.localRotation = Quaternion.Euler(leverTopRotation);
		leverButton.ChangeButtonEnabled(false);
		currentWidth = width;
		currentHeight = height;
		SetSpinsRemaining(spins);
		chipsEarned = 0;
		
		for(int i = 0; i < columnCards.Length; i++)
		{
			if(columnCards[i] == null)
			{
				continue;
			}
			for(int j = 0; j < columnCards[i].Count; j++)
			{
				columnCards[i][j].rt.SetParent(GameManager.instance.spareCardParent);
				columnCards[i][j].xImage.gameObject.SetActive(false);
				columnCards[i][j].gameObject.SetActive(false);
			}
			columnCards[i].Clear();
		}
		if(height > 1)
		{
			slotMachineImageRT.gameObject.SetActive(true);
			slotMachineImageRT.sizeDelta = new Vector2(50f + 48f * (width - 1), 50f + 48f * (height - 1));
			slotMachineImageSingleRT.gameObject.SetActive(false);
		}
		else
		{
			slotMachineImageRT.gameObject.SetActive(false);
			slotMachineImageSingleRT.gameObject.SetActive(true);
			slotMachineImageSingleRT.sizeDelta = new Vector2(50f + 48f * (width - 1), 50f);
		}
		for(int i = 0; i < handScorings.Length; i++)
		{
			if(i < height)
			{
				handScorings[i].gameObject.SetActive(true);
				handScorings[i].HandUpdated(new List<CardData>());
				if(i == 0 || i == height - 1)
				{
					if(i == 0 && height == 1)
					{
						handScorings[i].rt.sizeDelta = new Vector2(110f, 50f);
						handScorings[i].rt.anchoredPosition = new Vector2(24f * width + 1f, 0);
					}
					else
					{
						handScorings[i].rt.sizeDelta = new Vector2(110f, 49f);
						if(i == 0)
						{
							handScorings[i].rt.anchoredPosition = new Vector2(24f * width + 1f, 24f * (height - 1) + 0.5f);
						}
						else
						{
							handScorings[i].rt.anchoredPosition = new Vector2(24f * width + 1f, -24f * (height - 1) - 0.5f);
						}
					}
				}
				else
				{
					handScorings[i].rt.sizeDelta = new Vector2(110f, 48f);
					handScorings[i].rt.anchoredPosition = new Vector2(24f * width + 1f, 24f * (height - 1) - 48f * i);
				}
			}
			else
			{
				handScorings[i].gameObject.SetActive(false);
			}
		}
		cardsParentRT.sizeDelta = new Vector2(50f + 48f * (width - 1), 50f + 48f * (height - 1));
		for(int i = 0; i < columns.Length; i++)
		{
			columns[i].sizeDelta = new Vector2(50f, 50f + 48f * (height - 1));
			columns[i].anchoredPosition = new Vector2(-24f * (width - 1) + 48f * i, 0);
		}
		for(int i = 0; i < columnCards.Length; i++)
		{
			columnCards[i] = new List<Card>();
		}
		cardDatas.Clear();
		cardDatas = new List<CardData>(Deck.instance.ShuffleListOfCardDatas(Deck.instance.GetAllCardDatasOfTypeInDeck(false)));
		int cardIndex = 0;
		while(cardDatas.Count > 0 && cardIndex < (width + 1) * height - 1)
		{
			if(GameManager.instance.spareCardParent.childCount > 0)
			{
				GameManager.instance.spareCardParent.GetChild(GameManager.instance.spareCardParent.childCount - 1).gameObject.SetActive(true);
				Card storedCard = GameManager.instance.spareCardParent.GetChild(GameManager.instance.spareCardParent.childCount - 1).GetComponent<Card>();
				storedCard.rt.SetParent(columns[cardIndex % width]);
				columnCards[cardIndex % width].Add(storedCard);
				storedCard.rt.anchoredPosition = new Vector2(0, -24f * (height - 1) + 48f * (columns[cardIndex % width].childCount - 1));
				storedCard.cardData = cardDatas[cardDatas.Count - 1];
				storedCard.UpdateGraphics();
			}
			else
			{
				GameObject newCardGO = Instantiate(LocalInterface.instance.cardPrefab, columns[cardIndex % width]);
				Card newCard = newCardGO.GetComponent<Card>();
				newCard.rt.SetParent(columns[cardIndex % width]);
				columnCards[cardIndex % width].Add(newCard);
				newCard.rt.anchoredPosition = new Vector2(0, -24f * (height - 1) + 48f * (columns[cardIndex % width].childCount - 1));
				newCard.cardData = cardDatas[cardDatas.Count - 1];
				newCard.UpdateGraphics();
				// Debug.Log($"cardIndex={cardIndex} cardIndex % width={cardIndex % width}");
			}
			cardDatas.RemoveAt(cardDatas.Count - 1);
			cardIndex++;
		}
	}
	
	public void StartSpin()
	{
		if(spinning)
		{
			StopCoroutine(spinCoroutine); // this def shouldn't happen
		}
		for(int i = 0; i < columnSpinning.Length; i++)
		{
			columnSpinning[i] = false;
			columnWindingDown[i] = false;
			columnSpinTime[i] = 0f;
		}
		for(int i = 0; i < columnCards.Length; i++)
		{
			for(int j = 0; j < columnCards[i].Count; j++)
			{
				columnCards[i][j].xImage.gameObject.SetActive(false);
			}
		}
		for(int i = 0; i < currentHeight; i++)
		{
			handScorings[i].HandUpdated(new List<CardData>());
		}
		resultsPanelBackdrop.gameObject.SetActive(false);
		spinCoroutine = Spin();
		StartCoroutine(spinCoroutine);
	}
	
	public IEnumerator Spin()
	{
		spinning = true;
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			spinningSource.volume = Preferences.instance.soundVolume;
			spinningSource.Play();
		}
		StartCoroutine(SpinColumn(0));
		float timeBetweenColumns = 0.2f;
		float t = 0;
		for(int i = 1; i < currentWidth; i++)
		{
			while(t < timeBetweenColumns)
			{
				t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, timeBetweenColumns);
				yield return null;
			}
			t = 0;
			StartCoroutine(SpinColumn(i));
		}
		float spinningTime = 3f;
		t = 0;
		while(t < spinningTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, spinningTime);
			yield return null;
		}
		t = 0;
		for(int i = 0; i < currentWidth; i++)
		{
			while(t < timeBetweenColumns)
			{
				t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, timeBetweenColumns);
				yield return null;
			}
			t = 0;
			columnSpinTime[i] = 0f;
			columnWindingDown[i] = true;
		}
		float windDownTime = 1f;
		t = 0;
		while(t < windDownTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, windDownTime);
			yield return null;
		}
		for(int i = 0; i < currentWidth; i++)
		{
			while(t < timeBetweenColumns)
			{
				t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, timeBetweenColumns);
				yield return null;
			}
			t = 0;
			columnSpinning[i] = false;
		}
		float stoppingTime = 1.1f;
		t = 0;
		while(t < stoppingTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, stoppingTime);
			yield return null;
		}
		bool columnHasRoom = ColumnHasRoom();
		// Debug.Log($"columnHasRoom={columnHasRoom}, cardDatas.Count={cardDatas.Count}");
		while(cardDatas.Count > 0 && columnHasRoom)
		{
			int columnWithLeastCards = -1;
			int lowestNumberOfCards = 9001;
			for(int i = 0; i < currentWidth; i++)
			{
				if(columnCards[i].Count < lowestNumberOfCards)
				{
					lowestNumberOfCards = columnCards[i].Count;
					columnWithLeastCards = i;
				}
			}
			if(GameManager.instance.spareCardParent.childCount > 0)
			{
				GameManager.instance.spareCardParent.GetChild(GameManager.instance.spareCardParent.childCount - 1).gameObject.SetActive(true);
				Card storedCard = GameManager.instance.spareCardParent.GetChild(GameManager.instance.spareCardParent.childCount - 1).GetComponent<Card>();
				storedCard.rt.SetParent(columns[columnWithLeastCards]);
				// storedCard.rt.SetSiblingIndex(0);
				columnCards[columnWithLeastCards].Add(storedCard);
				storedCard.rt.anchoredPosition = new Vector2(0, 24f * (currentHeight + 1));
				float newY = storedCard.rt.anchoredPosition.y - 48f * (currentHeight + 1 - columnCards[columnWithLeastCards].Count);
				// Debug.Log($"storedCard.gameObject.name={storedCard.gameObject.name}, storedCard.rt.anchoredPosition.y={storedCard.rt.anchoredPosition.y}, newY={newY}");
				StartCoroutine(MoveCardToPosition(storedCard, new Vector2(0, newY), false, columnWithLeastCards));
				storedCard.cardData = cardDatas[cardDatas.Count - 1];
				storedCard.UpdateGraphics();
			}
			else
			{
				GameObject newCardGO = Instantiate(LocalInterface.instance.cardPrefab, columns[columnWithLeastCards]);
				Card newCard = newCardGO.GetComponent<Card>();
				newCard.rt.SetParent(columns[columnWithLeastCards]);
				// newCard.rt.SetSiblingIndex(0);
				columnCards[columnWithLeastCards].Add(newCard);
				newCard.rt.anchoredPosition = new Vector2(0, 24f * (currentHeight + 1));
				float newY = newCard.rt.anchoredPosition.y - 48f * (currentHeight + 1 - columnCards[columnWithLeastCards].Count);
				// Debug.Log($"newCard.gameObject.name={newCard.gameObject.name}, newCard.rt.anchoredPosition.y={newCard.rt.anchoredPosition.y}, newY={newY}");
				StartCoroutine(MoveCardToPosition(newCard, new Vector2(0, newY), false, columnWithLeastCards));
				newCard.cardData = cardDatas[cardDatas.Count - 1];
				newCard.UpdateGraphics();
			}
			cardDatas.RemoveAt(cardDatas.Count - 1);
			columnHasRoom = ColumnHasRoom();
		}
		float waitingTime = 1.1f;
		t = 0;
		while(t < waitingTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, waitingTime);
			yield return null;
		}
		spinningSource.Stop();
		for(int i = 0; i < currentHeight; i++)
		{
			List<Card> cardsInRow = new List<Card>();
			for(int j = 0; j < currentWidth; j++)
			{
				if(columnCards[j].Count > i)
				{
					cardsInRow.Add(columnCards[j][i]);
				}
			}
			handScorings[currentHeight - i - 1].HandUpdated(cardsInRow);
		}
		for(int i = 0; i < currentHeight; i++)
		{
			handScorings[i].StartScoringHand();
			yield return new WaitForSeconds(0.2f);
		}
		bool stillScoring = true;
		while(stillScoring)
		{
			stillScoring = StillScoringHand();
			yield return null;
		}
		double totalScore = 0;
		for(int i = 0; i < currentHeight; i++)
		{
			totalScore += handScorings[i].currentPoints;
		}
		resultsPanelBackdrop.gameObject.SetActive(true);
		pointsLabel.ChangeText($"{LocalInterface.instance.ConvertDoubleToString(totalScore)} Points");
		lowerEndLabel.ChangeText("0");
		upperEndLabel.ChangeText($"{LocalInterface.instance.ConvertDoubleToString(lowestPointThreshold)}");
		totalChipsEarnedLabel.ChangeText($"Chips Earned: {chipsEarned}");
		if(spinsRemaining > 0)
		{
			resultsPanelBackdrop.sizeDelta = new Vector2(300f, 93f);
			resultsButton.gameObject.SetActive(false);
		}
		else
		{
			resultsPanelBackdrop.sizeDelta = new Vector2(300f, 118f);
			resultsButton.gameObject.SetActive(true);
			resultsButton.ChangeButtonEnabled(false);
		}
		float chipGainingTime = 5f;
		double currentDisplayedScore = 0;
		double currentPointThreshold = lowestPointThreshold;
		double lastPointThreshold = 0;
		t = 0;
		while(t < chipGainingTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, chipGainingTime);
			currentDisplayedScore = LocalInterface.DoubleLerp(0, totalScore, LocalInterface.instance.animationCurve.Evaluate(t / chipGainingTime));
			while(currentDisplayedScore >= currentPointThreshold)
			{
				currentChip.StartMove();
				if(GameManager.instance.spareChipsParent.childCount > 0)
				{
					Chip spareChip = GameManager.instance.spareChipsParent.GetChild(GameManager.instance.spareChipsParent.childCount - 1).GetComponent<Chip>();
					spareChip.gameObject.SetActive(true);
					spareChip.rt.SetParent(chipParent);
					spareChip.rt.anchoredPosition = Vector2.zero;
					currentChip = spareChip;
				}
				else
				{
					GameObject newChipGO = Instantiate(GameManager.instance.chipPrefab, chipParent);
					Chip newChip = newChipGO.GetComponent<Chip>();
					newChip.rt.anchoredPosition = Vector2.zero;
					currentChip = newChip;
				}
				chipsEarned++;
				totalChipsEarnedLabel.ChangeText($"Chips Earned: {chipsEarned}");
				lastPointThreshold = currentPointThreshold;
				currentPointThreshold = currentPointThreshold * pointThresholdFactor;
				lowerEndLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(lastPointThreshold));
				upperEndLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentPointThreshold));
				SoundManager.instance.PlaySlotMachineChipGainedSound();
			}
			currentScoreFill.sizeDelta = new Vector2(Mathf.Lerp(0, currentScoreFillSize, ((float)currentDisplayedScore - (float)lastPointThreshold) / ((float)currentPointThreshold - (float)lastPointThreshold)), currentScoreFill.sizeDelta.y);
			yield return null;
		}
		
		if(spinsRemaining > 0)
		{
			StartCoroutine(MoveLever(leverTopPosition, leverTopRotation, true));
		}
		else
		{
			resultsButton.ChangeButtonEnabled(true);
			waitingForReturnClick = true;
			while(waitingForReturnClick)
			{
				yield return null;
			}
			MovingObjects.instance.mo["PlayArea"].StartMove("OnScreen");
			MovingObjects.instance.mo["HandArea"].StartMove("OnScreen");
			MovingObjects.instance.mo["BossInformation"].StartMove("OnScreen");
			MovingObjects.instance.mo["CardParent"].StartMove("OnScreen");
			MovingObjects.instance.mo["SlotMachine"].StartMove("OffScreen");
			resultsButton.ChangeButtonEnabled(false);
			t = 0;
			float returningTime = 1f;
			while(t < returningTime)
			{
				t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, waitingTime);
				yield return null;
			}
			PlayArea.instance.currentStateOfScoringRoutine = "FinishedSlotMachine";
		}
		spinning = false;
	}
	
	public void ResultsButtonClicked()
	{
		waitingForReturnClick = false;
	}
	
	public bool StillScoringHand()
	{
		for(int i = 0; i < currentHeight; i++)
		{
			if(handScorings[i].currentStateOfScoringRoutine != "NotScoring")
			{
				return true;
			}
		}
		return false;
	}
	
	public bool ColumnHasRoom()
	{
		for(int i = 0; i < currentWidth; i++)
		{
			if(columnCards[i].Count < currentHeight)
			{
				return true;
			}
		}
		return false;
	}
	
	public IEnumerator SpinColumn(int columnNumber)
	{
		columnSpinning[columnNumber] = true;
		while(columnSpinning[columnNumber])
		{
			columnSpinTime[columnNumber] += Time.deltaTime * Preferences.instance.gameSpeed;
			float descentDistance = 0f;
			if(columnWindingDown[columnNumber])
			{
				descentDistance = windDownCurve.Evaluate(Mathf.Clamp(columnSpinTime[columnNumber], 0, 1f)) * Time.deltaTime * slotSpeed * Preferences.instance.gameSpeed;
			}
			else
			{
				descentDistance = spinCurve.Evaluate(Mathf.Clamp(columnSpinTime[columnNumber], 0, 1f)) * Time.deltaTime * slotSpeed * Preferences.instance.gameSpeed;
			}
			for(int i = 0; i < columnCards[columnNumber].Count; i++)
			{
				columnCards[columnNumber][i].rt.anchoredPosition = new Vector2(columnCards[columnNumber][i].rt.anchoredPosition.x, columnCards[columnNumber][i].rt.anchoredPosition.y - descentDistance);
				if(i == columnCards[columnNumber].Count - 1 && columnCards[columnNumber].Count > 0 && columnCards[columnNumber][i].rt.anchoredPosition.y < 24f * (currentHeight - 1) && cardDatas.Count > 0)
				{
					if(GameManager.instance.spareCardParent.childCount > 0)
					{
						GameManager.instance.spareCardParent.GetChild(GameManager.instance.spareCardParent.childCount - 1).gameObject.SetActive(true);
						Card storedCard = GameManager.instance.spareCardParent.GetChild(GameManager.instance.spareCardParent.childCount - 1).GetComponent<Card>();
						storedCard.rt.SetParent(columns[columnNumber]);
						columnCards[columnNumber].Add(storedCard);
						storedCard.rt.anchoredPosition = new Vector2(0, 24f * (currentHeight + 1));
						storedCard.cardData = cardDatas[cardDatas.Count - 1];
						storedCard.xImage.gameObject.SetActive(false);
						storedCard.UpdateGraphics();
					}
					else
					{
						GameObject newCardGO = Instantiate(LocalInterface.instance.cardPrefab, columns[columnNumber]);
						Card newCard = newCardGO.GetComponent<Card>();
						newCard.rt.SetParent(columns[columnNumber]);
						columnCards[columnNumber].Add(newCard);
						newCard.rt.anchoredPosition = new Vector2(0, 24f * (currentHeight + 1));
						newCard.cardData = cardDatas[cardDatas.Count - 1];
						newCard.UpdateGraphics();
					}
					cardDatas.RemoveAt(cardDatas.Count - 1);
				}
				if(i == 0 && columnCards[columnNumber].Count > 0 && columnCards[columnNumber][i].rt.anchoredPosition.y < -24f - 24f * currentHeight)
				{
					columnCards[columnNumber][i].rt.SetParent(GameManager.instance.spareCardParent);
					cardDatas.Add(columnCards[columnNumber][i].cardData);
					// columnCards[columnNumber][i].xImage.SetActive(false);
					columnCards[columnNumber][i].gameObject.SetActive(false);
					columnCards[columnNumber].RemoveAt(i);
					i--;
				}
			}
			yield return null;
		}
		for(int i = 0; i < columnCards[columnNumber].Count; i++)
		{
			StartCoroutine(MoveCardToPosition(columnCards[columnNumber][i], new Vector2(0, -24f * (currentHeight + 1) + 48f * i), i == 0, columnNumber));
		}
 		float positioningTime = 1f;
		float t = 0;
		while(t < positioningTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, positioningTime);
			yield return null;
		}
	}
	
	public IEnumerator MoveCardToPosition(Card card, Vector2 destinationPosition, bool releaseToCardDatasAtEnd, int column)
	{
		float moveTime = 1f;
		float t = 0;
		Vector2 originPosition = card.rt.anchoredPosition;
		// Debug.Log($"Moving Card {card.gameObject.name} from {originPosition} to {destinationPosition}");
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
			// card.rt.anchoredPosition = Vector2.Lerp(originPosition, destinationPosition, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			card.rt.anchoredPosition = Vector2.Lerp(originPosition, destinationPosition, t / moveTime);
			yield return null;
		}
		if(releaseToCardDatasAtEnd)
		{
			cardDatas.Add(card.cardData);
			columnCards[column].RemoveAt(0);
			card.rt.SetParent(GameManager.instance.spareCardParent);
			card.gameObject.SetActive(false);
		}
		SoundManager.instance.PlaySlotMachineColumnStoppingSound();
	}
	
	public void LeverButtonClicked()
	{
		AdjustSpinsRemaining(-1);
		leverButton.ChangeButtonEnabled(false);
		if(ControllerSelection.instance.usingController)
		{
			ControllerSelection.instance.DeactivateSelection();
		}
		StartSpin();
		StartCoroutine(MoveLever(leverBottomPosition, leverBottomRotation, false));
	}
	
	public IEnumerator MoveLever(Vector2 destinationPosition, Vector3 destinationRotation, bool enableButtonAtEnd)
	{
		float moveTime = 1f;
		float t = 0;
		Vector2 originPosition = leverHandleRT.anchoredPosition;
		Quaternion originRotationQ = leverHandleRT.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(destinationRotation);
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
			leverHandleRT.anchoredPosition = Vector2.Lerp(originPosition, destinationPosition, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			leverHandleRT.localRotation = Quaternion.Lerp(originRotationQ, destinationRotationQ, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			yield return null;
		}
		if(enableButtonAtEnd)
		{
			leverButton.ChangeButtonEnabled(true);
			if(ControllerSelection.instance.usingController)
			{
				ControllerSelection.instance.MoveSelectionToBestFit();
			}
		}
	}
	
	public void SetSpinsRemaining(int numberToSet)
	{
		spinsRemaining = numberToSet;
		AdjustSpinsRemaining(0);
	}
	
	public void AdjustSpinsRemaining(int change)
	{
		spinsRemaining = spinsRemaining + change;
		if(spinsRemaining > 0)
		{
			leverButton.ChangeButtonText(spinsRemaining.ToString());
		}
		else
		{
			leverButton.ChangeButtonText(string.Empty);
		}
	}
	
	public void SlotMachineOffScreen()
	{
		visibilityObject.SetActive(false);
	}
}
