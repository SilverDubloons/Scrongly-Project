using UnityEngine;
using System.Collections;

public class BombFragment : MonoBehaviour
{
    public RectTransform rt;
	public BombExplosion parentBombExplosion;
	
	public bool leftSide;
	
	private bool alreadyReportedDeath = false;
	
	public void StartMovement()
	{
		StartCoroutine(MovementCoroutine());
	}
	
	public IEnumerator MovementCoroutine()
	{
		float xSpeed = UnityEngine.Random.Range(150f, 250f);
		if(leftSide)
		{
			xSpeed = -xSpeed;
		}
		float ySpeed = UnityEngine.Random.Range(150f, 250f);
		while(true)
		{
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + xSpeed * Time.deltaTime, rt.anchoredPosition.y + ySpeed * Time.deltaTime);
			ySpeed -= 600f * Time.deltaTime;
			if(rt.anchoredPosition.y < -300f && !alreadyReportedDeath)
			{
				alreadyReportedDeath = true;
				parentBombExplosion.FragmentExpired();
				Destroy(this.gameObject);
			}
			yield return null;
		}
	}
}
