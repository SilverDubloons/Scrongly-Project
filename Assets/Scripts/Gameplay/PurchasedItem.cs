using UnityEngine;
using UnityEngine.UI;
using static Deck;
using System.Collections;

public class PurchasedItem : MonoBehaviour
{
	public RectTransform rt;
	public Image image;
	
	public void StartMoving(Vector2 origin, Vector2 destination/* , bool flip = false, CardData cardDataToAdd = null */)
	{
		StartCoroutine(MoveItem(origin, destination/* , cardDataToAdd */));
/* 		if(flip)
		{
			StartCoroutine(FlipItem());
		} */
	}
	
	public IEnumerator MoveItem(Vector2 origin, Vector2 destination)
	{
		float t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.anchoredPosition = Vector2.Lerp(origin, destination, LocalInterface.instance.animationCurve.Evaluate(t / LocalInterface.instance.animationDuration));
			yield return null;
		}
/* 		if(cardDataToAdd != null)
		{
			Deck.instance.AddCardToDrawPile(cardDataToAdd);
		} */
		Destroy(this.gameObject);
	}
	
/* 	public IEnumerator FlipItem()
	{
		Vector3 originalScale = rt.localScale;
		Vector3 destinationScale = rt.localScale;
		destinationScale.x = 0;
		float t = 0;
		float flipTime = LocalInterface.instance.animationDuration / 3f;
		while(t < flipTime)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.localScale = Vector3.Lerp(originalScale, destinationScale, t / flipTime);
			yield return null;
		}
		image.sprite = V.i.chosenDeckSprite;
		t = 0;
		while(t < flipTime)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.localScale = Vector3.Lerp(destinationScale, originalScale, t / flipTime);
			yield return null;
		}
	} */
}
