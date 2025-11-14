using UnityEngine;

public class Bubble : MonoBehaviour
{
	public RectTransform rt;
	private float randomEulerAngle;
	private float randomSpeedFactor;
	
    void Start()
    {
        ResetBubble();
    }

    void Update()
    {
        float sinwave = Mathf.Sin(Time.time * 4 + randomEulerAngle) / 45;
		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + sinwave, rt.anchoredPosition.y + (40 + Mathf.Pow(ScoreVial.instance.currentRoundScoreNormalized, 2f) * 120f) * Time.deltaTime * randomSpeedFactor);
		if(rt.anchoredPosition.y > 280f)
		{
			ScoreVial.instance.bubbles.Add(this);
			this.gameObject.SetActive(false);
		}
    }
	
	public void ResetBubble()
	{
		rt.anchoredPosition = new Vector2(UnityEngine.Random.Range(-7f, 7f), 0);
		randomEulerAngle = UnityEngine.Random.Range(0, 360f);
		randomSpeedFactor = UnityEngine.Random.Range(0.8f, 1.2f);
	}
}
