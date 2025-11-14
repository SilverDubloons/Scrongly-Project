using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicMarker : MonoBehaviour
{
	public RectTransform rt;
	public RectTransform markerRT;
	public Image markerImage;
	
	public Vector2 markerOrigin;
	public Vector2 markerFirstStop;
	public Vector2 markerDestination;
	public Vector3 markerOriginRotation;
	public Vector3 markerFirstStopRotation;
	public Vector3 markerSecondStopRotation;
	public Color transparentColor;
	public Color opaqueColor;
	
    public void StartMarker(Card cardToChange, int newRank)
	{
		rt.anchoredPosition = Vector2.zero;
		StartCoroutine(MarkerCoroutine(cardToChange, newRank));
	}
	
	public IEnumerator MarkerCoroutine(Card cardToChange, int newRank)
	{
		float t = 0;
		while(t < LocalInterface.instance.animationDuration / 2)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			markerImage.color = Color.Lerp(transparentColor, opaqueColor, (t * 2) / (LocalInterface.instance.animationDuration / 2));
			markerRT.anchoredPosition = Vector2.Lerp(markerOrigin, markerFirstStop, t / (LocalInterface.instance.animationDuration / 2));
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			markerRT.localEulerAngles = Vector3.Slerp(markerOriginRotation, markerFirstStopRotation, t / (LocalInterface.instance.animationDuration / 8));
			yield return null;
		}
		cardToChange.ChangeRank(newRank);
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 4)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			markerRT.localEulerAngles = Vector3.Slerp(markerFirstStopRotation, markerSecondStopRotation, t / (LocalInterface.instance.animationDuration / 4));
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 4)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			markerRT.localEulerAngles = Vector3.Slerp(markerSecondStopRotation, markerFirstStopRotation, t / (LocalInterface.instance.animationDuration / 4));
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			markerRT.localEulerAngles = Vector3.Slerp(markerFirstStopRotation, markerOriginRotation, t / (LocalInterface.instance.animationDuration / 8));
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 2)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			markerImage.color = Color.Lerp(opaqueColor, transparentColor, ((t * 2) - LocalInterface.instance.animationDuration / 4) / (LocalInterface.instance.animationDuration / 2));
			markerRT.anchoredPosition = Vector2.Lerp(markerFirstStop, markerDestination, t / (LocalInterface.instance.animationDuration / 2));
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
