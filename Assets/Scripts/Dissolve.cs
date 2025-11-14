using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dissolve : MonoBehaviour
{
    public RectTransform rt;
	public RectTransform itemToDissolveParent;
	public Image dissolveMaskImage;
	
	public Sprite[] dissolveSprites;
	
	public const float dissolveTime = 1f;
	
	public void StartDissolve(RectTransform objectToDissolve)
	{
		rt.SetParent(objectToDissolve);
		rt.anchoredPosition = Vector2.zero;
		rt.SetParent(objectToDissolve.parent);
		rt.localRotation = objectToDissolve.localRotation;
		objectToDissolve.SetParent(itemToDissolveParent);
		SoundManager.instance.PlayDissolveSound();
		StartCoroutine(DissolveCoroutine());
	}
	
	public IEnumerator DissolveCoroutine()
	{
		float t = 0;
		while(t < dissolveTime)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			int newIndex = Mathf.RoundToInt(t / dissolveTime * (dissolveSprites.Length - 1));
			if(newIndex < 0)
			{
				newIndex = 0;
			}
			if(newIndex >= dissolveSprites.Length)
			{
				newIndex = dissolveSprites.Length - 1;
			}
			dissolveMaskImage.sprite = dissolveSprites[newIndex];
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
