using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Promotion : MonoBehaviour
{
    public RectTransform rt;
	public RectTransform promotionImageRT;
	public Image promotionImage;
	
	public Vector2 imageOrigin;
	public Vector2 imageFirstStop;
	public Vector2 imageDestination;
	public Vector3 imageRotationOrigin;
	public Vector3 imageRotationDestination;
	public Sprite demotionSprite;
	
	public void StartPromotion(Card cardToPromote, int rankChange)
	{
		rt.anchoredPosition = Vector3.zero;
		if(rankChange <= 0)
		{
			promotionImage.sprite = demotionSprite;
		}
		StartCoroutine(PromotionCoroutine(cardToPromote, rankChange));
	}
	
	public IEnumerator PromotionCoroutine(Card cardToPromote, int rankChange)
	{
		float t = 0;
		while(t < LocalInterface.instance.animationDuration / 2)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			promotionImageRT.anchoredPosition = Vector2.Lerp(imageOrigin, imageFirstStop, t / (LocalInterface.instance.animationDuration / 2));
			promotionImage.color = Color.Lerp(LocalInterface.instance.transparentColor, LocalInterface.instance.opaqueColor, t * 2 / (LocalInterface.instance.animationDuration / 2));
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 4)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			promotionImageRT.localEulerAngles = Vector3.Lerp(imageRotationOrigin, imageRotationDestination, t / (LocalInterface.instance.animationDuration / 8));
			yield return null;
		}
		int newRank = cardToPromote.cardData.rank + rankChange;
		while(newRank > 12)
		{
		 	newRank -= 13;
		}
		while(newRank < 0)
		{
			newRank += 13;
		}
		cardToPromote.ChangeRank(newRank);
		if(rankChange > 0)
		{
			SoundManager.instance.PlayPromotionSound();
		}
		else
		{
			SoundManager.instance.PlayDemotionSound();
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			promotionImageRT.localEulerAngles = Vector3.Lerp(imageRotationDestination, imageRotationOrigin, t / (LocalInterface.instance.animationDuration / 8));
			yield return null;
		}
		while(t < LocalInterface.instance.animationDuration / 4)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			yield return null;
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 2)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			promotionImageRT.anchoredPosition = Vector2.Lerp(imageFirstStop, imageDestination, t / (LocalInterface.instance.animationDuration / 2));
			promotionImage.color = Color.Lerp(LocalInterface.instance.opaqueColor, LocalInterface.instance.transparentColor, (t - LocalInterface.instance.animationDuration / 4) / (LocalInterface.instance.animationDuration / 2));
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
