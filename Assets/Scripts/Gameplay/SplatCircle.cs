using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplatCircle : MonoBehaviour
{
    public RectTransform rt;
	public Image splatCircleImage;
	
	public AnimationCurve spatCircleMovementCurve;
	
	public void StartSplatCircle(Vector2 destinationLocation, Sprite splatCircleSprite)
	{
		splatCircleImage.sprite = splatCircleSprite;
		splatCircleImage.color = ScoreVial.instance.fillColor;
		rt.sizeDelta = new Vector2(splatCircleSprite.rect.width, splatCircleSprite.rect.height);
		StartCoroutine(SplatCircleCoroutine(destinationLocation));
	}
	
	public IEnumerator SplatCircleCoroutine(Vector2 destinationLocation)
	{
		float moveTime = 0.8f;
		Vector2 originLocation = rt.anchoredPosition;
		float t = 0;
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
			rt.anchoredPosition = Vector2.Lerp(originLocation, destinationLocation, spatCircleMovementCurve.Evaluate(t / moveTime));
			yield return null;
		}
		float fadeTime = 2f;
		t = 0;
		while(t < fadeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, fadeTime);
			splatCircleImage.color = Color.Lerp(ScoreVial.instance.fillColor, ScoreVial.instance.fillColorTransparent, t / fadeTime);
			yield return null;
		}
	}
}
