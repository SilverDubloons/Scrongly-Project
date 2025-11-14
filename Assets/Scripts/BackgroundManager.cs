using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundManager : MonoBehaviour
{
    public RectTransform backgroundRT;
    public Image backgroundImage;
	public float rotationSpeed;
	public GameObject juliaObject;
	
	public static BackgroundManager instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	void Update()
	{
		if(Preferences.instance.rotatingBackground)
		{
			backgroundRT.Rotate(0, 0, rotationSpeed * Time.deltaTime);
		}
	}
	
	public void SwitchToBossBackground()
	{
		juliaObject.SetActive(true);
		StartCoroutine(Fade(LocalInterface.instance.opaqueColor, LocalInterface.instance.transparentColor, false));
	}
	
	public void SwitchToClassicBackground()
	{
		StartCoroutine(Fade(LocalInterface.instance.transparentColor, LocalInterface.instance.opaqueColor, true));
	}
	
	public IEnumerator Fade(Color originColor, Color destinationColor, bool turnOffBossBacground)
	{
		float t = 0;
		float fadeTime = 1f;
		while(t < fadeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime, 0, fadeTime);
			backgroundImage.color = Color.Lerp(originColor, destinationColor, t / fadeTime);
			yield return null;
		}
		if(turnOffBossBacground)
		{
			juliaObject.SetActive(false);
		}
	}
}
