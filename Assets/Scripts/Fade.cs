using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fade : MonoBehaviour
{
    public Image image;
	public float fadeTime;
	public bool fadeAtStart;
	public bool destroyAtEnd;
	public Color startingColor;
	public Color endingColor;
	public AnimationCurve fadeCurve;
	
	private bool fading;
	private IEnumerator fadingCoroutine;
		
	void Start()
	{
		if(fadeAtStart)
		{
			StartFade();
		}
	}
	
	public void StartFade()
	{
		if(fading)
		{
			StopCoroutine(fadingCoroutine);
		}
		fadingCoroutine = FadeCoroutine();
		StartCoroutine(fadingCoroutine);
	}

	public IEnumerator FadeCoroutine()
	{
		fading = true;
		float t = 0;
		while(t < fadeTime)
		{
			t += Time.deltaTime;
			image.color = Color.Lerp(startingColor, endingColor, fadeCurve.Evaluate(t / fadeTime));
			yield return null;
		}
		image.color = endingColor;
		if(destroyAtEnd)
		{
			Destroy(this.gameObject);
		}
	}
}
