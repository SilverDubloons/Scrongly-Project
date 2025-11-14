using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ChipThreshold : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
	public RectTransform backdrop;
	public Image backdropImage;
	public Image dissolveMaskImage;
	public Label label;
	public Chip chip;
	public GameObject pointerObject;
	
	public Sprite[] dissolveSprites;
	public float dissolveTime;
	public float sizeChangeSpeed;
	public Vector2 smallSize;
	public Vector2 largeSize;
	
	public bool stayDecompressed;
	public double scoreThreshold;
	public int handsRemaining;
	
	public float currentSizeNormalized;
	private bool expanding;
	private bool contracting;
	private IEnumerator currentCoroutine;
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(!expanding && !stayDecompressed)
		{
			if(contracting)
			{
				StopCoroutine(currentCoroutine);
				contracting = false;
			}
			currentCoroutine = Expand();
			SoundManager.instance.PlaySlideOutSound();
			StartCoroutine(currentCoroutine);
		}
	}
	
	public void ForceDecompression()
	{
		if(contracting || expanding)
		{
			StopCoroutine(currentCoroutine);
			contracting = false;
			expanding = false;
		}
		currentCoroutine = Expand();
		StartCoroutine(currentCoroutine);
		stayDecompressed = true;
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(!contracting && !stayDecompressed)
		{
			if(expanding)
			{
				StopCoroutine(currentCoroutine);
				expanding = false;
			}
			currentCoroutine = Contract();
			SoundManager.instance.PlaySlideOutSound(true);
			StartCoroutine(currentCoroutine);
		}
	}
	
	public IEnumerator Expand()
	{
		expanding = true;
		while(currentSizeNormalized < 1f)
		{
			currentSizeNormalized += sizeChangeSpeed * Time.deltaTime * Preferences.instance.gameSpeed;
			UpdateSize();
			yield return null;
		}
		currentSizeNormalized = 1f;
		label.gameObject.SetActive(true);
		UpdateSize();
		expanding = false;
	}
	
	public IEnumerator Contract()
	{
		contracting = true;
		label.gameObject.SetActive(false);
		while(currentSizeNormalized > 0)
		{
			currentSizeNormalized -= sizeChangeSpeed * Time.deltaTime * Preferences.instance.gameSpeed;
			UpdateSize();
			yield return null;
		}
		currentSizeNormalized = 0;
		UpdateSize();
		contracting = false;
	}
	
	public void UpdateSize()
	{
		backdrop.sizeDelta = Vector2.Lerp(smallSize, largeSize, currentSizeNormalized);
	}
	
	public void UpdateLabel()
	{
		if(handsRemaining == 1)
		{
			label.ChangeText($"{handsRemaining} Hand\n{LocalInterface.instance.ConvertDoubleToString(scoreThreshold)}");
		}
		else
		{
			label.ChangeText($"{handsRemaining} Hands\n{LocalInterface.instance.ConvertDoubleToString(scoreThreshold)}");
		}
		if(handsRemaining <= 1)
		{
			backdropImage.color = ScoreVial.instance.chipThresholdWarningColor;
		}
		else
		{
			backdropImage.color = ScoreVial.instance.chipThresholdDefaultColor;
		}
	}
	
	public void ChipThresholdReached()
	{
		chip.StartMove();
		SoundManager.instance.PlayXylophoneSound();
		Destroy(this.gameObject);
	}
	
	public void StartDissolve()
	{
		dissolveMaskImage.enabled = true;
		SoundManager.instance.PlayChipThresholdMissedSound();
		StartCoroutine(Dissolve());
	}
	
	public IEnumerator Dissolve()
	{
		float t = 0;
		while(t < dissolveTime)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			int newIndex = Mathf.RoundToInt(t / dissolveTime * (dissolveSprites.Length - 1));
			newIndex = Mathf.Clamp(newIndex, 0, dissolveSprites.Length - 1);
			dissolveMaskImage.sprite = dissolveSprites[newIndex];
			yield return null;
		}
		ScoreVial.instance.ReplaceMissedChipThreshold(rt.anchoredPosition, scoreThreshold);
		Destroy(this.gameObject);
	}
	
	public void StartMove(Vector2 destination)
	{
		StartCoroutine(Move(destination));
	}
	
	public IEnumerator Move(Vector2 destination)
	{
		float t = 0;
		float moveTime = LocalInterface.instance.animationDuration;
		Vector2 origin = rt.anchoredPosition;
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, 1f);
			rt.anchoredPosition = Vector2.Lerp(origin, destination, LocalInterface.instance.animationCurve.Evaluate(t / moveTime));
			yield return null;
		}
		rt.anchoredPosition = destination;
	}
	
	public string ConvertToText()
	{
		return $"{rt.anchoredPosition.y}|{stayDecompressed}|{scoreThreshold}|{handsRemaining}";
	}
}