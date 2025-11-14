using UnityEngine;
using System.Collections;

public class VialTop : MonoBehaviour
{
    public RectTransform rt;
	
	public Vector2 origin;
	public Vector2 destination;
	
	public float moveTime;
	public float returnTime;
	public float rotationSpeed;
	
	public void StartBurst()
	{
		StartCoroutine(BurstCoroutine());
	}
	
	public IEnumerator BurstCoroutine()
	{
		SoundManager.instance.PlayVialLidPoppingOffSound();
		float t = 0;
		float currentRotationSpeed = UnityEngine.Random.Range(0.5f, 1.5f) * rotationSpeed;
		float currentMoveTime = UnityEngine.Random.Range(0.5f, 1.5f) * moveTime;
		while(t < currentMoveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, currentMoveTime);
			rt.anchoredPosition = Vector2.Lerp(origin, destination, t / currentMoveTime);
			rt.localEulerAngles = new Vector3(rt.localEulerAngles.x, rt.localEulerAngles.y, rt.localEulerAngles.z + currentRotationSpeed * Time.deltaTime);
			yield return null;
		}
		rt.anchoredPosition = destination;
		rt.localEulerAngles = Vector3.zero;
	}
	
	public void StartReturn(float delay = 0)
	{
		if(rt.anchoredPosition == origin)
		{
			return;
		}
		StartCoroutine(ReturnCoroutine(delay));
	}
	
	public IEnumerator ReturnCoroutine(float delay = 0)
	{
		float t = 0;
		Vector2 startPosition = rt.anchoredPosition;
		while(t < delay)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, delay);
			yield return null;
		}
		t = 0;
		while(t < returnTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, returnTime);
			rt.anchoredPosition = Vector2.Lerp(startPosition, origin, t / returnTime);
			yield return null;
		}
		rt.anchoredPosition = origin;
		SoundManager.instance.PlayVialLidReturningSound();
	}
}
