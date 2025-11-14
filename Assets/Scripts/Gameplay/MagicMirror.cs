using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagicMirror : MonoBehaviour
{
    public RectTransform rt;
	public Image spellImage;
	
	public void StartConversion(Card cardToCopyFrom, Card cardToPasteTo)
	{
		rt.anchoredPosition = Vector2.zero;
		StartCoroutine(ConversionCoroutine(cardToCopyFrom, cardToPasteTo));
	}
	
	public IEnumerator ConversionCoroutine(Card cardToCopyFrom, Card cardToPasteTo)
	{
		float t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			spellImage.fillAmount = Mathf.Lerp(0, 1, t / LocalInterface.instance.animationDuration);
			yield return null;
		}
		cardToPasteTo.cardData.rank = cardToCopyFrom.cardData.rank;
		cardToPasteTo.cardData.suit = cardToCopyFrom.cardData.suit;
		cardToPasteTo.cardData.baseValue = cardToCopyFrom.cardData.baseValue;
		cardToPasteTo.cardData.multiplier = cardToCopyFrom.cardData.multiplier;
		cardToPasteTo.cardData.isSpecialCard = cardToCopyFrom.cardData.isSpecialCard;
		cardToPasteTo.cardData.specialCardName = cardToCopyFrom.cardData.specialCardName;
		cardToPasteTo.UpdateGraphics();
		SoundManager.instance.PlayMagicMirrorSound();
		if(cardToCopyFrom.cardData.suit == 4)
		{
			RunInformation.instance.CheckForSchromaticUnlock();
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			spellImage.fillAmount = Mathf.Lerp(1, 0, t / LocalInterface.instance.animationDuration);
			yield return null;
		}
		Destroy(this.gameObject);
	}
}
