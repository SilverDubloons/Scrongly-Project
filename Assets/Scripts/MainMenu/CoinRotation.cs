using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinRotation : MonoBehaviour, IPointerClickHandler
{
    public RectTransform rt;
	public Transform baseCanvas;
	public AnimationCurve rotationCurve;
	public float spinTime;
	public int offsetPixelsForOneSpin;
	private Vector2 relativePosition;
	private bool spinning;
	
	public void SetRelativePosition()
	{
		Transform oldParent = transform.parent;
		transform.SetParent(baseCanvas);
		relativePosition = rt.anchoredPosition + new Vector2(-LocalInterface.instance.referenceResolution.x / 2, LocalInterface.instance.referenceResolution.y / 2);
		transform.SetParent(oldParent);
	}

    public void OnPointerClick(PointerEventData eventData)
    {
		if(!spinning)
		{
			SetRelativePosition();
			Vector2 mousePos = LocalInterface.instance.GetMousePosition();
			Vector2 clickOffset = mousePos - relativePosition;
			// Debug.Log($"relativePosition = {relativePosition.ToString()} mousePos = {mousePos.ToString()} clickOffset = {clickOffset.ToString()}");
			Vector3 endRotation = new Vector3(Mathf.Round(clickOffset.y / offsetPixelsForOneSpin) * 360f, Mathf.Round(clickOffset.x / offsetPixelsForOneSpin) * 360f, 0);
			if(endRotation != Vector3.zero)
			{
				SoundManager.instance.PlaySound(SoundManager.instance.coinSpinningSound);
				StartCoroutine(RotateCoin(endRotation));
			}
		}
    }
	
	public IEnumerator RotateCoin(Vector3 endRotation)
	{
		spinning = true;
		float t = 0;
		Vector3 startRotation = Vector3.zero;
		while(t < spinTime)
		{
			t += Time.deltaTime;
			rt.eulerAngles = Vector3.Lerp(startRotation, endRotation, rotationCurve.Evaluate(t / spinTime));
			yield return null;
		}
		rt.eulerAngles = Vector3.zero;
		spinning = false;
	}
}