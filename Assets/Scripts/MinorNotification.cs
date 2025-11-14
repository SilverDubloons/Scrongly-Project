using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinorNotification : MonoBehaviour
{
	public RectTransform rt;
	public Image[] imagesToFade;
	public Label label;
	public Image backdropImage;
	
	public IEnumerator riseCoroutine;
	public IEnumerator fadeCoroutine;
	
    public void StartNotification(string text, Vector2 position, float maxWidth = 100f, float delay = 2f, float riseSpeed = 10f, float fadeTime = 2f, Color? color = null)
	{
		label.ChangeText(text);
		label.ChangeAlpha(1);
		for(int i = 0; i < imagesToFade.Length; i++)
		{
			imagesToFade[i].color = new Color(imagesToFade[i].color.r, imagesToFade[i].color.g, imagesToFade[i].color.b, 1);
		}
		float width = Mathf.Min(maxWidth, label.GetPreferredWidth() + MinorNotifications.instance.minorNotificationXSizeIncrease);
		rt.sizeDelta = new Vector2(width, 640);
		float height = label.GetPreferredHeight() + MinorNotifications.instance.minorNotificationYSizeIncrease;
		rt.sizeDelta = new Vector2(width, height);
		rt.anchoredPosition = position;
		backdropImage.color = color ?? MinorNotifications.instance.defaultNotificationColor;
		if(position.x + width / 2 > LocalInterface.instance.referenceResolution.x / 2)
		{
			rt.anchoredPosition = new Vector2(LocalInterface.instance.referenceResolution.x / 2 - width / 2, rt.anchoredPosition.y);
		}
		else if(position.x - width / 2 < -LocalInterface.instance.referenceResolution.x / 2)
		{
			rt.anchoredPosition = new Vector2(-LocalInterface.instance.referenceResolution.x / 2 + width / 2, rt.anchoredPosition.y);
		}
		if(position.y + height / 2 > LocalInterface.instance.referenceResolution.y / 2)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, LocalInterface.instance.referenceResolution.y / 2 - height / 2);
		}
		else if(position.y - height / 2 < -LocalInterface.instance.referenceResolution.y / 2)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -LocalInterface.instance.referenceResolution.y / 2 + height / 2);
		}
		riseCoroutine = Rise(delay, riseSpeed);
		StartCoroutine(riseCoroutine);
		fadeCoroutine = Fade(delay, fadeTime);
		StartCoroutine(fadeCoroutine);
	}
	
	private IEnumerator Rise(float delay, float speed)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		while(true)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + (speed * Time.deltaTime));
			yield return null;
		}
	}
	
	private IEnumerator Fade(float delay, float fadeTime)
	{
		float t = 0;
		while(t < delay)
		{
			t += Time.deltaTime;
			yield return null;
		}
		t = 0;
		while(t < fadeTime)
		{
			t += Time.deltaTime;
			float newAlpha = 1f - (t / fadeTime);
			for(int i = 0; i < imagesToFade.Length; i++)
			{
				imagesToFade[i].color = new Color(imagesToFade[i].color.r, imagesToFade[i].color.g, imagesToFade[i].color.b, newAlpha);
			}
			label.ChangeAlpha(newAlpha);
			yield return null;
		}
		MinorNotifications.instance.minorNotifications.Add(this);
		StopCoroutine(riseCoroutine);
		this.gameObject.SetActive(false);
	}
}
