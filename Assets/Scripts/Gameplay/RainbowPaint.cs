using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Deck;

public class RainbowPaint : MonoBehaviour
{
	public RectTransform rt;
    public RectTransform paint;
    public RectTransform paintbrush;
	public Image paintbrushImage;
	
	public Vector2 paintOrigin;
	public Vector2 paintFirstStop;
	public Vector2 paintDestination;
	public Vector2 paintbrushOrigin;
	public Vector2 paintbrushDestination;
	public Color paintbrushOriginColor;
	public Color paintbrushDestinationColor;
	
	public void StartPaint(Card cardToPaint)
	{
		rt.anchoredPosition = Vector3.zero;
		SoundManager.instance.PlayPaintSound();
		StartCoroutine(Paint(cardToPaint));
	}
	
	public IEnumerator Paint(Card cardToPaint)
	{
		float t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			paint.anchoredPosition = Vector2.Lerp(paintOrigin, paintFirstStop, t / LocalInterface.instance.animationDuration);
			paintbrush.anchoredPosition = Vector2.Lerp(paintbrushOrigin, paintbrushDestination, t / LocalInterface.instance.animationDuration);
			yield return null;
		}
		paintbrush.anchoredPosition = paintbrushDestination;
		cardToPaint.ChangeSuit(4);
		RunInformation.instance.CheckForSchromaticUnlock();
		t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			paint.anchoredPosition = Vector2.Lerp(paintFirstStop, paintDestination, t / LocalInterface.instance.animationDuration);
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 2)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			paintbrushImage.color = Color.Lerp(paintbrushOriginColor, paintbrushDestinationColor, t / (LocalInterface.instance.animationDuration / 2));
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
