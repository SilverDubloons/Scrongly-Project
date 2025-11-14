using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Splat : MonoBehaviour
{
	public RectTransform rt;
    public Image splatImage;
	public AnimationCurve descendAndExpandAnimationCurve;
	
	public void StartSplat(Vector2 destinationLocation, Sprite splatSprite)
	{
		splatImage.sprite = splatSprite;
		rt.sizeDelta = new Vector2(splatSprite.rect.width, splatSprite.rect.height);
		splatImage.color = ScoreVial.instance.fillColor;
		destinationLocation = new Vector2(destinationLocation.x + UnityEngine.Random.Range(-ScoreVial.splatMaxDistance, ScoreVial.splatMaxDistance), destinationLocation.y + UnityEngine.Random.Range(-ScoreVial.splatMaxDistance, ScoreVial.splatMaxDistance));
		StartCoroutine(SplatCoroutine(destinationLocation));
	}
	
	public IEnumerator SplatCoroutine(Vector2 destinationLocation)
	{
		Vector2 originLocation = new Vector2(destinationLocation.x, destinationLocation.y + 360);
		float t = 0;
		float descendAndExpandTime = 0.4f;
		while(t < descendAndExpandTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, descendAndExpandTime);
			rt.anchoredPosition = Vector2.Lerp(originLocation, destinationLocation, descendAndExpandAnimationCurve.Evaluate(t / descendAndExpandTime));
			rt.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, descendAndExpandAnimationCurve.Evaluate(t / descendAndExpandTime));
			yield return null;
		}
		SoundManager.instance.PlaySplatSound();
		rt.anchoredPosition = destinationLocation;
		rt.localScale = Vector3.one;
		int r = UnityEngine.Random.Range(3, 6);
		for(int i = 0; i < r; i++)
		{
			GameObject newSplatCircleGO = Instantiate(ScoreVial.instance.splatCirclePrefab, rt);
			SplatCircle newSplatCircle = newSplatCircleGO.GetComponent<SplatCircle>();
			float approxRadius = ((rt.sizeDelta.x + rt.sizeDelta.y) / 4f) * UnityEngine.Random.Range(1.25f, 1.4f);
			float minAngle = (360f / r) * i;
			float maxAngle = (360f / r) * i + (360f / r);
			newSplatCircle.StartSplatCircle(LocalInterface.instance.GetRandomPointOnCircle(Vector2.zero, approxRadius, minAngle, maxAngle), ScoreVial.instance.splatCircleSprites[UnityEngine.Random.Range(0, ScoreVial.instance.splatCircleSprites.Length)]);
			yield return null;
		}
		yield return new WaitForSeconds(0.8f / Preferences.instance.gameSpeed);
		Vector2 slideLocation = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - UnityEngine.Random.Range(70f, 130f));
		float descendAndFadeTime = 2f;
		t = 0;
		while (t < descendAndFadeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, descendAndFadeTime);
			splatImage.color = Color.Lerp(ScoreVial.instance.fillColor, ScoreVial.instance.fillColorTransparent, t / descendAndFadeTime);
			rt.anchoredPosition = Vector2.Lerp(destinationLocation, slideLocation, t / descendAndFadeTime);
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
