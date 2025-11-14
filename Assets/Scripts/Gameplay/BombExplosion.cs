using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BombExplosion : MonoBehaviour
{
    public RectTransform rt;
	public RectTransform explosionImageRT;
	public Image explosionImage;
	public BombFragment[] bombFragments;
	
	public Vector3 explosionOriginScale;
	public Vector3 explosionDestinationScale;
	
	public int expiredFragments = 0;
	
	public void StartExplosion(Card cardToDestroy = null)
	{
		rt.anchoredPosition = Vector2.zero;
		rt.SetParent(PlayArea.instance.bombExplosionParent);
		SoundManager.instance.PlayExplosionSound();
		StartCoroutine(ExplosionCoroutine(cardToDestroy));
	}
	
	public IEnumerator ExplosionCoroutine(Card cardToDestroy)
	{
		float t = 0;
		while(t < LocalInterface.instance.animationDuration / 3)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			explosionImageRT.localScale = Vector3.Lerp(explosionOriginScale, explosionDestinationScale, t / (LocalInterface.instance.animationDuration / 3));
			yield return null;
		}
		if(cardToDestroy != null)
		{
			for(int i = 0; i < bombFragments.Length; i++)
			{
				bombFragments[i].gameObject.SetActive(true);
				bombFragments[i].StartMovement();
				bombFragments[i].parentBombExplosion = this;
			}
			Deck.instance.cardsInHand.Remove(cardToDestroy);
			cardToDestroy.dropZonePlacedIn.CardRemoved();
			Destroy(cardToDestroy.gameObject);
		}
		t = 0;
		while(t < LocalInterface.instance.animationDuration / 3)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			explosionImageRT.localScale = Vector3.Lerp(explosionDestinationScale, explosionOriginScale, t / (LocalInterface.instance.animationDuration / 3));
			yield return null;
		}
	}
	
	public void FragmentExpired()
	{
		expiredFragments++;
		if(expiredFragments >= 4)
		{
			Destroy(this.gameObject);
		}
	}
}
