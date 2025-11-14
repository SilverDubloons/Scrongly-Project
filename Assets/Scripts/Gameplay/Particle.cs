using UnityEngine;

public class Particle : MonoBehaviour
{
    public RectTransform rt;
	private float timeAlive = 0;
	private const float timeUntilDeath = 1f;
	
	void Update()
	{
		timeAlive += Time.deltaTime;
		rt.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeAlive / timeUntilDeath);
		if(timeAlive > timeUntilDeath)
		{
			// Destroy(this.gameObject);
			rt.SetParent(GameManager.instance.spareChipParticlesParent);
			timeAlive = 0;
			rt.localScale = Vector3.one;
			this.gameObject.SetActive(false);
		}
	}
}
