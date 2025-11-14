using UnityEngine;
using System;
using System.Collections;
using static GameManager;

public class ScorePlate : MonoBehaviour
{
	public RectTransform rt;
    public RectTransform pointsRT;
    public RectTransform pointsToAddRT;
    public RectTransform multRT;
    public RectTransform multToAddRT;
	public Label pointsLabel;
	public Label pointsToAddLabel;
	public Label multLabel;
	public Label multToAddLabel;
	public DropZone dropZone;
	public RectTransform xImage;
	
	public double points;
	public double pointsToAdd;
	public double mult;
	public double multToAdd;
	public bool additiveMushroomUsed;
	public bool multiplicitiveMushroomUsed;
	public double multiplicitiveMushroomFactor;
	
	public Vector2 plateStartingSize;
	private Vector2 pointsDesiredSize;
	private Vector2 pointsToAddDesiredSize;
	private Vector2 multDesiredSize;
	private Vector2 multToAddDesiredSize;
	public Vector3 plateOriginRotation;
	public Vector3 plateDestinationRotation;
	public Vector3 originScale;
	public Vector3 destinationScale;
	
	private const float platesYDifference = 22f;
	private const float platesXSizeToAdd = 6f;
	
	public void StartScorePlate(int cardsPlayed, HandScoring handScoring = null, Card handScoringCard = null)
	{
		rt.SetSiblingIndex(rt.parent.childCount - 1);
		// Debug.Log($"{name} starting, points={points}, pointsToAdd={pointsToAdd}, mult={mult}, multToAdd=multToAdd");
		int positionIndex = 0;
		if(Math.Abs(points) < 0.1d && Math.Abs(pointsToAdd) < 0.1d)
		{
			pointsRT.gameObject.SetActive(false);
			pointsToAddRT.gameObject.SetActive(false);
		}
		else
		{
			pointsRT.gameObject.SetActive(true);
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(points));
			pointsDesiredSize = new Vector2(pointsLabel.GetPreferredWidth() + platesXSizeToAdd, pointsRT.sizeDelta.y);
			pointsLabel.gameObject.SetActive(false);
			pointsRT.anchoredPosition = new Vector2(0, positionIndex * platesYDifference);
			positionIndex++;
			if(Math.Abs(pointsToAdd) > 0.1d)
			{
				pointsToAddRT.gameObject.SetActive(true);
				pointsToAddLabel.ChangeText($"+{LocalInterface.instance.ConvertDoubleToString(pointsToAdd)}");
				pointsToAddDesiredSize = new Vector2(pointsToAddLabel.GetPreferredWidth() + platesXSizeToAdd, pointsToAddRT.sizeDelta.y);
				pointsToAddLabel.gameObject.SetActive(false);
				pointsToAddRT.anchoredPosition = new Vector2(0, positionIndex * platesYDifference);
				positionIndex++;
			}
			else
			{
				pointsToAddRT.gameObject.SetActive(false);
			}
		}
		if(Math.Abs(mult) < 0.1d && Math.Abs(multToAdd) < 0.1d)
		{
			multRT.gameObject.SetActive(false);
			multToAddRT.gameObject.SetActive(false);
		}
		else
		{
			multRT.gameObject.SetActive(true);
			multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(mult));
			multDesiredSize = new Vector2(multLabel.GetPreferredWidth() + platesXSizeToAdd, multRT.sizeDelta.y);
			multLabel.gameObject.SetActive(false);
			multRT.anchoredPosition = new Vector2(0, positionIndex * platesYDifference);
			positionIndex++;
			if(Math.Abs(multToAdd) > 0.1d)
			{
				multToAddRT.gameObject.SetActive(true);
				multToAddLabel.ChangeText($"+{LocalInterface.instance.ConvertDoubleToString(multToAdd)}");
				multToAddDesiredSize = new Vector2(multToAddLabel.GetPreferredWidth() + platesXSizeToAdd, multToAddRT.sizeDelta.y);
				multToAddLabel.gameObject.SetActive(false);
				multToAddRT.anchoredPosition = new Vector2(0, positionIndex * platesYDifference);
			}
			else
			{
				multToAddRT.gameObject.SetActive(false);
			}
		}
		StartCoroutine(ScorePlateCoroutine(cardsPlayed, positionIndex, handScoring, handScoringCard));
	}
	
	public IEnumerator ScorePlateCoroutine(int cardsPlayed, int platesToShow, HandScoring handScoring = null, Card handScoringCard = null)
	{
		bool applyPointsAndMult = true;
		if(handScoring == null && GameManager.instance.IsBossTagActive("SuitPairGrantsNoValue"))
		{
			if(dropZone.cardPlaced && !dropZone.placedCard.cardData.isSpecialCard && (dropZone.placedCard.cardData.suit == GameManager.instance.GetCurrentBossRoundRandom1() || dropZone.placedCard.cardData.suit == GameManager.instance.GetCurrentBossRoundRandom2()))
			{
				applyPointsAndMult = false;
			}
		}
		float t = 0;
		while(t < LocalInterface.instance.animationDuration * 3 / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			if(pointsRT.gameObject.activeSelf)
			{
				pointsRT.sizeDelta = Vector2.Lerp(plateStartingSize, pointsDesiredSize, t / (LocalInterface.instance.animationDuration * 3 / 8));
			}
			if(pointsToAddRT.gameObject.activeSelf)
			{
				pointsToAddRT.sizeDelta = Vector2.Lerp(plateStartingSize, pointsToAddDesiredSize, t / (LocalInterface.instance.animationDuration * 3 / 8));
			}
			if(multRT.gameObject.activeSelf)
			{
				multRT.sizeDelta = Vector2.Lerp(plateStartingSize, multDesiredSize, t / (LocalInterface.instance.animationDuration * 3 / 8));
			}
			if(multToAddRT.gameObject.activeSelf)
			{
				multToAddRT.sizeDelta = Vector2.Lerp(plateStartingSize, multToAddDesiredSize, t / (LocalInterface.instance.animationDuration * 3 / 8));
			}
			yield return null;
		}
		if(pointsRT.gameObject.activeSelf)
		{
			pointsLabel.gameObject.SetActive(true);
		}
		if(pointsToAddRT.gameObject.activeSelf)
		{
			pointsToAddLabel.gameObject.SetActive(true);
		}
		if(multRT.gameObject.activeSelf)
		{
			multLabel.gameObject.SetActive(true);
		}
		if(multToAddRT.gameObject.activeSelf)
		{
			multToAddLabel.gameObject.SetActive(true);
		}
		yield return new WaitForSeconds((LocalInterface.instance.animationDuration / 2 / Preferences.instance.gameSpeed + LocalInterface.instance.animationDuration / 4 * cardsPlayed) / Preferences.instance.gameSpeed);
		// Debug.Log($"{this.name}, multiplicitiveMushroomUsed={multiplicitiveMushroomUsed}, multiplicitiveMushroomFactor={multiplicitiveMushroomFactor}, points={points}, pointsToAdd={pointsToAdd}, mult={mult}, multToAdd={multToAdd}");
		if(pointsRT.gameObject.activeSelf)
		{
			if(multiplicitiveMushroomUsed)
			{
				points = (points + pointsToAdd) * multiplicitiveMushroomFactor;
			}
			else
			{
				points = points + pointsToAdd;
			}
			// Debug.Log($"points={points}");
			pointsToAdd = 0;
			pointsLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(points));
			pointsDesiredSize = new Vector2(pointsLabel.GetPreferredWidth() + platesXSizeToAdd, pointsRT.sizeDelta.y);
			pointsRT.sizeDelta = pointsDesiredSize;
			if(pointsToAddRT.gameObject.activeSelf)
			{
				pointsToAddRT.gameObject.SetActive(false);
			}
		}
		if(multRT.gameObject.activeSelf)
		{
			if(multiplicitiveMushroomUsed)
			{
				mult = (mult + multToAdd) * multiplicitiveMushroomFactor;
			}
			else
			{
				mult = mult + multToAdd;
			}
			multToAdd = 0;
			multLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(mult));
			multDesiredSize = new Vector2(multLabel.GetPreferredWidth() + platesXSizeToAdd, multRT.sizeDelta.y);
			multRT.sizeDelta = multDesiredSize;
			if(multToAddRT.gameObject.activeSelf)
			{
				multToAddRT.gameObject.SetActive(false);
			}
		}
		if(Math.Abs(points) > 0.1d && applyPointsAndMult)
		{
			if(handScoring == null)
			{
				PlayArea.instance.AddPoints(points);
			}
			else
			{
				handScoring.AddPoints(points);
			}
/* 			if(dropZone.cardPlaced && !dropZone.placedCard.cardData.isSpecialCard)
			{
				
			} */
		}
		if(Math.Abs(mult) > 0.1d && applyPointsAndMult)
		{
			if(handScoring == null)
			{
				PlayArea.instance.AddMult(mult);
			}
			else
			{
				handScoring.AddMult(mult);
			}
		}
		if(handScoringCard == null)
		{
			dropZone.placedCard.cardData.baseValue = (float)points;
			dropZone.placedCard.cardData.multiplier = (float)mult;
			if(!dropZone.placedCard.cardData.isSpecialCard && dropZone.placedCard.cardData.rank == 5)
			{
				Stats.instance.SevenScored();
			}
		}
		else
		{
			handScoringCard.cardData.baseValue = (float)points;
			handScoringCard.cardData.multiplier = (float)mult;
		}
		if(handScoring == null)
		{
			PlayArea.instance.AddToPointsAndMultGainedFromCards(points, mult);
		}
		else
		{
			handScoring.AddToPointsAndMultGainedFromCards(points, mult);
		}
		if(Math.Abs(points) > 0.1d || Math.Abs(mult) > 0.1d)
		{
			if(!applyPointsAndMult)
			{
				SoundManager.instance.PlayCardDidNotGivePointsSound();
				StartCoroutine(ReceiveNoValueCoroutine(platesToShow));
			}
			else if(multiplicitiveMushroomUsed)
			{
				SoundManager.instance.PlayCardUpdradeSound(true);
			}
			else if(additiveMushroomUsed)
			{
				SoundManager.instance.PlayCardUpdradeSound(false);
			}
			else if((((handScoringCard == null && dropZone.placedCard.cardData.rank == 12) || (handScoringCard != null && handScoringCard.cardData.rank == 12)) || Baubles.instance.GetImpactInt("AllCardsAreAces") > 0) && Baubles.instance.GetImpactInt("AcesStraights") > 0 && ((handScoring == null && PlayArea.instance.currentHandsContained[4]) || (handScoring != null && handScoring.currentHandsContained[4])))
			{
				SoundManager.instance.PlayPanFluteSound();
			}
			else
			{
				SoundManager.instance.PlayPianoSound();
			}
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			if(pointsRT.gameObject.activeSelf)
			{
				pointsRT.localEulerAngles = Vector3.Lerp(plateOriginRotation, plateDestinationRotation, t / (LocalInterface.instance.animationDuration / 8));
				if(multiplicitiveMushroomUsed)
				{
					pointsRT.localScale = Vector3.Lerp(originScale, destinationScale, t / (LocalInterface.instance.animationDuration / 8));
				}
			}
			if(multRT.gameObject.activeSelf)
			{
				multRT.localEulerAngles = Vector3.Lerp(plateOriginRotation, plateDestinationRotation, t / (LocalInterface.instance.animationDuration / 8));
				if(multiplicitiveMushroomUsed)
				{
					multRT.localScale = Vector3.Lerp(originScale, destinationScale, t / (LocalInterface.instance.animationDuration / 8));
				}
			}
			yield return null;
		}
		pointsRT.localScale = originScale;
		multRT.localScale = originScale;
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			if(pointsRT.gameObject.activeSelf)
			{
				pointsRT.localEulerAngles = Vector3.Lerp(plateDestinationRotation, plateOriginRotation, t / (LocalInterface.instance.animationDuration / 8));
				if(multiplicitiveMushroomUsed)
				{
					pointsRT.localScale = Vector3.Lerp(destinationScale, originScale, t / (LocalInterface.instance.animationDuration / 8));
				}
			}
			if(multRT.gameObject.activeSelf)
			{
				multRT.localEulerAngles = Vector3.Lerp(plateDestinationRotation, plateOriginRotation, t / (LocalInterface.instance.animationDuration / 8));
				if(multiplicitiveMushroomUsed)
				{
					multRT.localScale = Vector3.Lerp(destinationScale, originScale, t / (LocalInterface.instance.animationDuration / 8));
				}
			}
			yield return null;
		}
		pointsRT.localEulerAngles = plateOriginRotation;
		multRT.localEulerAngles = plateOriginRotation;
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / Preferences.instance.gameSpeed);
		if(pointsRT.gameObject.activeSelf)
		{
			pointsLabel.gameObject.SetActive(false);
		}
		if(multRT.gameObject.activeSelf)
		{
			multLabel.gameObject.SetActive(false);
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration * 3 / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			if(pointsRT.gameObject.activeSelf)
			{
				pointsRT.sizeDelta = Vector2.Lerp(pointsDesiredSize, plateStartingSize, t / (LocalInterface.instance.animationDuration * 3 / 8));
			}
			if(multRT.gameObject.activeSelf)
			{
				multRT.sizeDelta = Vector2.Lerp(multDesiredSize, plateStartingSize, t / (LocalInterface.instance.animationDuration * 3 / 8));
			}
			yield return null;
		}
		pointsRT.gameObject.SetActive(false);
		multRT.gameObject.SetActive(false);
		if(handScoring != null)
		{
			ResetScorePlate();
			if(handScoringCard != null)
			{
				handScoringCard.scorePlate = null;
			}
			rt.SetParent(GameManager.instance.spareScorePlateParent);
			this.gameObject.SetActive(false);
		}
/* 		points = 0;
		pointsToAdd = 0;
		mult = 0;
		multToAdd = 0; */
	}
	
	public IEnumerator ReceiveNoValueCoroutine(int platesToShow)
	{
		xImage.gameObject.SetActive(true);
		xImage.anchoredPosition = new Vector2(0, (platesToShow - 1) * 11f);
		float t = 0;
		float xGrowTime = 0.2f;
		while(t < xGrowTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, xGrowTime);
			xImage.localScale = Vector2.Lerp(Vector2.zero, Vector2.one, t / xGrowTime);
			yield return null;
		}
		yield return new WaitForSeconds(0.1f / Preferences.instance.gameSpeed);
		float xWagTime = 0.1f;
		t = 0;
		while(t < xWagTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, xWagTime);
			xImage.localEulerAngles = Vector3.Lerp(plateOriginRotation, plateDestinationRotation, t / xWagTime);
			yield return null;
		}
		t = 0;
		while(t < xWagTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, xWagTime);
			xImage.localEulerAngles = Vector3.Lerp(plateDestinationRotation, plateOriginRotation, t / xWagTime);
			yield return null;
		}
		t = 0;
		while(t < xWagTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, xWagTime);
			xImage.localEulerAngles = Vector3.Lerp(plateOriginRotation, -plateDestinationRotation, t / xWagTime);
			yield return null;
		}
		t = 0;
		while(t < xWagTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, xWagTime);
			xImage.localEulerAngles = Vector3.Lerp(-plateDestinationRotation, plateOriginRotation, t / xWagTime);
			yield return null;
		}
		yield return new WaitForSeconds(0.1f / Preferences.instance.gameSpeed);
		t = 0;
		while(t < xGrowTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, xGrowTime);
			xImage.localScale = Vector2.Lerp(Vector2.one, Vector2.zero, t / xGrowTime);
			yield return null;
		}
		xImage.gameObject.SetActive(false);
	}
	
	public void ResetScorePlate()
	{
		points = 0;
		pointsToAdd = 0;
		mult = 0;
		multToAdd = 0;
		additiveMushroomUsed = false;
		multiplicitiveMushroomUsed = false;
		multiplicitiveMushroomFactor = 0;
	}
}
