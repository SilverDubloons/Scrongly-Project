using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System;

public class ScoreVial : MonoBehaviour
{
    public RectTransform fill;
    public Image fillImage;
    public Image waveImage0;
    public Image waveImage1;
    public RectTransform waveRT0;
    public RectTransform waveRT1;
	public RectTransform chipThresholdParent;
	public RectTransform currentScoreRT;
	public RectTransform bubbleParent;
	public RectTransform splatParent;
	public RectTransform waveMask;
	public Label currentScoreLabel;
	public VialTop vialTop;
	
	public GameObject chipThresholdPrefab;
	public GameObject bubblePrefab;
	public Color fillColor;
	public Color fillColorTransparent;
	public Vector2 fillEmptySize;
	public Vector2 scoreRTEmptyLocation;
	public Color chipThresholdDefaultColor;
	public Color chipThresholdWarningColor;
	public Sprite[] splatSprites;
	public Sprite[] splatCircleSprites;
	public GameObject splatPrefab;
	public GameObject splatCirclePrefab;
	public Vector2[] splatLocations;
	
	public double currentRoundScore;
	public float currentRoundScoreNormalized;
	public float timeSinceLastBubble;
	
	public const float splatMaxDistance = 50f;
	public const float vialHeight = 260f;
	public List<ChipThreshold> chipThresholds = new List<ChipThreshold>();
	public List<Bubble> bubbles = new List<Bubble>();
	
	public static ScoreVial instance;
	
	public void SetupInstance()
	{
		instance = this;
		fillColorTransparent = fillColor;
		fillColorTransparent.a = 0;
		fillImage.color = fillColor;
		waveImage0.color = fillColor;
		waveImage1.color = fillColor;
	}
	
	public void SetupChipThresholdsForNewRound()
	{
		for(int i = chipThresholds.Count - 1; i >= 0; i--)
		{
			Destroy(chipThresholds[i].gameObject);
		}
		chipThresholds.Clear();
		int numberOfChipThresholds = GameManager.instance.GetChipThresholdsPerRound();
		double roundScoreThreshold = GameManager.instance.GetCurrentRoundScoreThreshold();
		
		for(int i = 0; i < numberOfChipThresholds; i++)
		{
			GameObject newChipThresholdGO = Instantiate(chipThresholdPrefab, chipThresholdParent);
			newChipThresholdGO.name = $"ChipThreshold{i}";
			ChipThreshold newChipThreshold = newChipThresholdGO.GetComponent<ChipThreshold>();
			chipThresholds.Add(newChipThreshold);
			newChipThreshold.handsRemaining = i + 1 + V.i.v.variantSpecialOptions["BonusChipThresholdHands"].impact;
			if(V.i.v.variantSpecialOptions["ChipThresholdMinimumPercent"].impact == 0)
			{
				newChipThreshold.scoreThreshold = (roundScoreThreshold / numberOfChipThresholds) * (i + 1);
			}
			else
			{
				double minThreshold = roundScoreThreshold * V.i.v.variantSpecialOptions["ChipThresholdMinimumPercent"].impact / 100;
				if(numberOfChipThresholds <= 1)
				{
					newChipThreshold.scoreThreshold = minThreshold;
				}
				else
				{
					double thresholdDelta = (roundScoreThreshold - minThreshold) / (numberOfChipThresholds - 1);
					newChipThreshold.scoreThreshold = minThreshold + thresholdDelta * i;
				}
			}
			newChipThreshold.UpdateLabel();
			newChipThreshold.rt.anchoredPosition = new Vector2(-74, (float)(newChipThreshold.scoreThreshold / roundScoreThreshold) * vialHeight);
			if(i == 0)
			{
				newChipThreshold.currentSizeNormalized = 1f;
				newChipThreshold.stayDecompressed = true;
			}
			else
			{
				newChipThreshold.currentSizeNormalized = 0f;
				newChipThreshold.stayDecompressed = false;
				newChipThreshold.label.gameObject.SetActive(false);
			}
			newChipThreshold.UpdateSize();
		}
	}
	
	public void SetScoreToZero()
	{
		currentRoundScore = 0;
		currentRoundScoreNormalized = 0;
		currentScoreRT.anchoredPosition = new Vector2(currentScoreRT.anchoredPosition.x, 0);
		currentScoreLabel.ChangeText("0");
		currentScoreRT.sizeDelta = new Vector2(currentScoreLabel.GetPreferredWidth() + 6, currentScoreRT.sizeDelta.y);
	}
	
	public void AddScore(double scoreToAdd)
	{
		StartCoroutine(AddScoreCoroutine(scoreToAdd));
	}
	
	public void LoadScore(double scoreToLoad)
	{
		currentRoundScore = scoreToLoad;
		double currentRoundThreshold = GameManager.instance.GetCurrentRoundScoreThreshold();
		currentRoundScoreNormalized = Mathf.Clamp((float)(currentRoundScore / currentRoundThreshold), 0, 1f);
		currentScoreLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentRoundScore));
		currentScoreRT.sizeDelta = new Vector2(currentScoreLabel.GetPreferredWidth() + 6, currentScoreRT.sizeDelta.y);
		currentScoreRT.anchoredPosition = new Vector2(currentScoreRT.anchoredPosition.x, Mathf.Min((float)(currentRoundScore / currentRoundThreshold * vialHeight), vialHeight));
		fill.sizeDelta = new Vector2(fill.sizeDelta.x, Mathf.Min((currentRoundScoreNormalized * (vialHeight + 1)), (vialHeight + 1)));
	}
	
	public void LoadChipThresholds(string chipThresholdsText)
	{
		for(int i = chipThresholds.Count - 1; i >= 0; i--)
		{
			Destroy(chipThresholds[i].gameObject);
		}
		chipThresholds.Clear();
		string[] chipThresholdsData = chipThresholdsText.Split('%', StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < chipThresholdsData.Length; i++)
		{	// return $"{rt.anchoredPosition.y}|{stayDecompressed}|{scoreThreshold}|{handsRemaining}";
			string[] chipThresholdData = chipThresholdsData[i].Split('|', StringSplitOptions.RemoveEmptyEntries);
			GameObject newChipThresholdGO = Instantiate(chipThresholdPrefab, chipThresholdParent);
			newChipThresholdGO.name = $"ChipThreshold{i}";
			ChipThreshold newChipThreshold = newChipThresholdGO.GetComponent<ChipThreshold>();
			chipThresholds.Add(newChipThreshold);
			newChipThreshold.handsRemaining = int.Parse(chipThresholdData[3]);
			newChipThreshold.scoreThreshold = float.Parse(chipThresholdData[2]);
			newChipThreshold.UpdateLabel();
			newChipThreshold.rt.anchoredPosition = new Vector2(-74f, float.Parse(chipThresholdData[0]));
			if(bool.Parse(chipThresholdData[1]))
			{
				newChipThreshold.currentSizeNormalized = 1f;
				newChipThreshold.stayDecompressed = true;
			}
			else
			{
				newChipThreshold.currentSizeNormalized = 0f;
				newChipThreshold.stayDecompressed = false;
				newChipThreshold.label.gameObject.SetActive(false);
			}
			newChipThreshold.UpdateSize();
		}
	}
	
	public IEnumerator AddScoreCoroutine(double scoreToAdd)
	{
		double oldScore = currentRoundScore;
		double newScore = currentRoundScore + scoreToAdd;
		double oldSyncedScore = currentRoundScore;
		double newSyncedScore = newScore;
		double currentSyncedScore = currentRoundScore;
		double currentRoundThreshold = GameManager.instance.GetCurrentRoundScoreThreshold();
 		if(newSyncedScore > currentRoundThreshold)
		{
			newSyncedScore = currentRoundThreshold;
		}
		bool vialOverflow = false;
		if(scoreToAdd >= currentRoundThreshold * 2 && !Preferences.instance.disableExcessScoringAnimation)
		{
			vialOverflow = true;
			SoundManager.instance.PlayWaterRushingSound();
		}
		Vector2 oldScorePosition = currentScoreRT.anchoredPosition;
		Vector2 newScorePosition = new Vector2(currentScoreRT.anchoredPosition.x, Mathf.Min((float)(newScore / currentRoundThreshold * vialHeight), vialHeight));
		Vector2 oldFillSize = fill.sizeDelta;
		Vector2 newFillSize = new Vector2(fill.sizeDelta.x, Mathf.Min((float)(newScore / GameManager.instance.GetCurrentRoundScoreThreshold() * (vialHeight + 1)), (vialHeight + 1)));
		float t = 0;
		float fillTime = LocalInterface.instance.animationDuration / (vialOverflow ? 2 : 1);
		int chipThresholdsCleared = 0;
		while(t < fillTime)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			t = Mathf.Clamp(t, 0, 1f);
			currentRoundScore = LocalInterface.DoubleLerp(oldScore, newScore, (double)LocalInterface.instance.animationCurve.Evaluate(t / fillTime));
			currentRoundScoreNormalized = Mathf.Clamp((float)(currentRoundScore / currentRoundThreshold), 0, 1f);
			fill.sizeDelta = Vector2.Lerp(oldFillSize, newFillSize, LocalInterface.instance.animationCurve.Evaluate(t / fillTime));
			waveMask.offsetMax = new Vector2(0, Mathf.Min((vialHeight - fill.sizeDelta.y + 1f), 3f) );
			currentScoreRT.anchoredPosition = Vector2.Lerp(oldScorePosition, newScorePosition, LocalInterface.instance.animationCurve.Evaluate(t / fillTime));
			currentScoreLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentRoundScore));
			currentScoreRT.sizeDelta = new Vector2(currentScoreLabel.GetPreferredWidth() + 6, currentScoreRT.sizeDelta.y);
			currentSyncedScore = LocalInterface.DoubleLerp(oldSyncedScore, newSyncedScore, (double)LocalInterface.instance.animationCurve.Evaluate(t / fillTime));
			chipThresholdsCleared += UpdateScoreThresholdPositions(currentSyncedScore);
			yield return null;
		}
		currentRoundScore = newScore;
		currentScoreRT.anchoredPosition = newScorePosition;
		currentSyncedScore = currentRoundScore;
		chipThresholdsCleared += UpdateScoreThresholdPositions(currentSyncedScore);
		if(vialOverflow)
		{
			vialTop.StartBurst();
			t = 0;
			float fillToTopTime = 0.05f;
			Vector2 fillOriginSize = fill.sizeDelta;
			Vector2 fillDestinationSize = fill.sizeDelta + new Vector2(0, 11f);
			while(t < fillToTopTime)
			{
				t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, fillToTopTime);
				fill.sizeDelta = Vector2.Lerp(fillOriginSize, fillDestinationSize, t / fillToTopTime);
				yield return null;
			}
			int numberOfSplats = UnityEngine.Random.Range(6, 9);
			int[] randomLocations = LocalInterface.instance.GetRandomizedArrayOfInts(numberOfSplats, numberOfSplats);
			int[] randomSplatSprites = LocalInterface.instance.GetRandomizedArrayOfInts(numberOfSplats, numberOfSplats);
			for(int i = 0; i < randomLocations.Length; i++)
			{
				GameObject newSplatGO = Instantiate(splatPrefab, splatParent);
				Splat newSplat = newSplatGO.GetComponent<Splat>();
				newSplat.StartSplat(splatLocations[randomLocations[i]], splatSprites[randomSplatSprites[i]]);
				yield return new WaitForSeconds(UnityEngine.Random.Range(0.05f, 0.1f));
			}
			yield return new WaitForSeconds(0.4f / Preferences.instance.gameSpeed);
			// vialTop.StartReturn(1f);
		}
		if(chipThresholdsCleared > 1 && Baubles.instance.GetImpactInt("ChipMagnet") > 0)
		{
			int chipsToAdd = chipThresholdsCleared - 1;
			if(chipThresholdsCleared >= GameManager.instance.GetChipThresholdsPerRound())
			{
				chipsToAdd += 2;
				// logic for sgambler deck unlock
				RunInformation.instance.AllChipThresholdsClearedWithMagnet();
			}
			else
			{
				RunInformation.instance.consecutiveHandsWithMagnetClearingAllChipThresholds = 0;
			}
			Baubles.instance.Notify("ChipMagnet", 0, SoundManager.instance.PlayChipMagnetSound, chipsToAdd);
		}
		else
		{
			RunInformation.instance.consecutiveHandsWithMagnetClearingAllChipThresholds = 0;
		}
		yield return new WaitForSeconds(LocalInterface.instance.animationDuration / 4f / Preferences.instance.gameSpeed);
		UpdateChipThresholds();
		PlayArea.instance.currentStateOfScoringRoutine = "DoneUpdatingScoreVial";
	}
	
	public int UpdateScoreThresholdPositions(double currentSyncedScore)
	{
		int chipThresholdsCleared = 0;
		for(int i = 0; i < chipThresholds.Count; i++)
		{
			if(currentSyncedScore >= chipThresholds[i].scoreThreshold)
			{
				ChipThreshold tempThreshold = chipThresholds[i];
				chipThresholds.Remove(tempThreshold);
				tempThreshold.ChipThresholdReached();
				chipThresholdsCleared++;
				i--;	// this could be bad? I was tired when I wrote this 19.Jul.2025
				Stats.instance.ChipThresholdCleared();
			}
			else
			{
				if(currentScoreRT.anchoredPosition.y + 6f > chipThresholds[i].rt.anchoredPosition.y - 10f)
				{
					chipThresholds[i].rt.anchoredPosition = new Vector2(chipThresholds[i].rt.anchoredPosition.x, Mathf.Min(currentScoreRT.anchoredPosition.y + 16f, vialHeight + 2f));
					chipThresholds[i].pointerObject.SetActive(false);
				}
				else
				{
					break;
				}
			}
		}
		return chipThresholdsCleared;
	}
	
	public void UpdateChipThresholds()
	{
		for(int i = chipThresholds.Count - 1; i >= 0; i--)
		{
			chipThresholds[i].handsRemaining--;
			chipThresholds[i].UpdateLabel();
			if(chipThresholds[i].handsRemaining <= 0)
			{
				chipThresholds[i].StartDissolve();
				chipThresholds.Remove(chipThresholds[i]);
			}
			else if(i == 0 && !chipThresholds[i].stayDecompressed)
			{
				chipThresholds[i].ForceDecompression();
			}
		}
	}
	
	public void ReplaceMissedChipThreshold(Vector2 oldPosition, double oldScoreThreshold)
	{
		// StartCoroutine(ReplaceMissedScoreThresholdCoroutine(oldPosition, oldScoreThreshold));
		Vector2 lastPosition = oldPosition;
		double lastScoreThreshold = oldScoreThreshold;
		for(int i = 0; i < chipThresholds.Count; i++)
		{
			Vector2 tempPositon = chipThresholds[i].rt.anchoredPosition;
			double tempScoreThreshold = chipThresholds[i].scoreThreshold;
			chipThresholds[i].StartMove(lastPosition);
			chipThresholds[i].scoreThreshold = lastScoreThreshold;
			chipThresholds[i].UpdateLabel();
			lastPosition = tempPositon;
			lastScoreThreshold = tempScoreThreshold;
			if(i == 0)
			{
				chipThresholds[i].ForceDecompression();
			}
		}
	}
	
/* 	public IEnumerator ReplaceMissedScoreThresholdCoroutine(Vector oldPosition, double oldScoreThreshold)
	{
		
	} */
	
	void Update()
	{
		if(currentRoundScore < 0.1d)
		{
			return;
		}
		timeSinceLastBubble += Time.deltaTime;
		float timeBetweenBubbles = (1.05f - Mathf.Pow(currentRoundScoreNormalized, 2f));
		while(timeSinceLastBubble > timeBetweenBubbles)
		{
			if(bubbles.Count > 0)
			{
				bubbles[bubbles.Count - 1].gameObject.SetActive(true);
				bubbles[bubbles.Count - 1].ResetBubble();
				bubbles.RemoveAt(bubbles.Count - 1);
			}
			else
			{
				bubbles.Add(Instantiate(bubblePrefab, bubbleParent).GetComponent<Bubble>());
			}
			timeSinceLastBubble -= timeBetweenBubbles;
		}
		float waveSpeed = (5 + Mathf.Pow(currentRoundScoreNormalized, 2f) * 30f) * Time.deltaTime;
		waveRT0.anchoredPosition = new Vector2(waveRT0.anchoredPosition.x + waveSpeed, waveRT0.anchoredPosition.y);
		if(waveRT0.anchoredPosition.x > 17f)
		{
			waveRT0.anchoredPosition = new Vector2(waveRT1.anchoredPosition.x - 24, waveRT0.anchoredPosition.y);
		}
		waveRT1.anchoredPosition = new Vector2(waveRT1.anchoredPosition.x + waveSpeed, waveRT1.anchoredPosition.y);
		if(waveRT1.anchoredPosition.x > 17f)
		{
			waveRT1.anchoredPosition = new Vector2(waveRT0.anchoredPosition.x - 24, waveRT1.anchoredPosition.y);
		}
	}
	
	public void StartDrainVial()
	{
		StartCoroutine(DrainVialCoroutine());
	}
	
	public IEnumerator DrainVialCoroutine()
	{
		Vector2 fillOriginSize = fill.sizeDelta;
		Vector2 currentScoreOriginLocation = currentScoreRT.anchoredPosition;
		float t = 0;
		float drainTime = 1f;
		double originScore = currentRoundScore;
		currentRoundScore = 0;
		while(t < drainTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, drainTime);
			fill.sizeDelta = Vector2.Lerp(fillOriginSize, fillEmptySize, LocalInterface.instance.animationCurve.Evaluate(t / drainTime));
			currentScoreRT.anchoredPosition = Vector2.Lerp(currentScoreOriginLocation, scoreRTEmptyLocation, LocalInterface.instance.animationCurve.Evaluate(t / drainTime));
			double currentDisplayedScore = LocalInterface.DoubleLerp(originScore, 0, LocalInterface.instance.animationCurve.Evaluate(t / drainTime));
			currentScoreLabel.ChangeText(LocalInterface.instance.ConvertDoubleToString(currentDisplayedScore));
			currentScoreRT.sizeDelta = new Vector2(currentScoreLabel.GetPreferredWidth() + 6, currentScoreRT.sizeDelta.y);
			yield return null;
		}
		GameManager.instance.AdvanceRound();
		SetupChipThresholdsForNewRound();
	}
	
	void Start()
	{
        ThemeManager.instance.OnThemeChanged += ApplyTheme;
        ApplyTheme();
    }
	
	void OnDestroy() 
	{
        if(ThemeManager.instance != null)
		{
            ThemeManager.instance.OnThemeChanged -= ApplyTheme;
		}
    }
	
	public void ApplyTheme()
	{
        fillColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.vialFill);
		fillColorTransparent = fillColor;
		fillColorTransparent.a = 0;
        chipThresholdDefaultColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.chipThreshold);
        chipThresholdWarningColor = ThemeManager.instance.GetColorFromCurrentTheme(ThemeManager.UIElementType.chipThresholdWarning);
	}
}