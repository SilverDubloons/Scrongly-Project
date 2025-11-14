using UnityEngine;
using System.Collections;

public class Chip : MonoBehaviour
{
	public RectTransform rt;
	public const float timeBetweenParticles = 0.015f;
	
    public void StartMove()
	{
		this.transform.SetParent(GameManager.instance.chipsParent);
		StartCoroutine(Move());
	}
	
	public IEnumerator Move()
	{
		float t = 0;
		Vector2 origin = rt.anchoredPosition;
		float timeSinceLastParticleSpawn = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.anchoredPosition = Vector2.Lerp(origin, GameManager.instance.chipDestination, LocalInterface.instance.animationCurve.Evaluate(t / LocalInterface.instance.animationDuration));
			timeSinceLastParticleSpawn += Time.deltaTime;
			if(timeSinceLastParticleSpawn > timeBetweenParticles)
			{
				timeSinceLastParticleSpawn -= timeBetweenParticles;
				if(GameManager.instance.spareChipParticlesParent.childCount > 0)
				{
					Particle storedParticle = GameManager.instance.spareChipParticlesParent.GetChild(GameManager.instance.spareChipParticlesParent.childCount - 1).GetComponent<Particle>();
					storedParticle.gameObject.SetActive(true);
					storedParticle.rt.SetParent(GameManager.instance.particlesParent);
					storedParticle.rt.anchoredPosition = rt.anchoredPosition;
				}
				else
				{
					GameObject newParticleGO = Instantiate(GameManager.instance.particlePrefab, GameManager.instance.particlesParent);
					Particle newParticle = newParticleGO.GetComponent<Particle>();
					newParticle.rt.anchoredPosition = rt.anchoredPosition;
				}
			}
			yield return null;
		}
		SoundManager.instance.PlayChipSound();
		GameManager.instance.AddCurrency();
		rt.SetParent(GameManager.instance.spareChipsParent);
		this.gameObject.SetActive(false);
		/* if(GameManager.instance.spareChipsParent.childCount < 10)
		{
			rt.SetParent(GameManager.instance.spareChipsParent);
			this.gameObject.SetActive(false);
		}
		else
		{
			Destroy(this.gameObject);
		} */
	}
}
