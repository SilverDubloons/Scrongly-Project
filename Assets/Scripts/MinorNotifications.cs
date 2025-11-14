using UnityEngine;
using System.Collections.Generic;

// MinorNotifications.instance.NewMinorNotification("Issue with GetSeed", LocalInterface.instance.GetMousePosition());

public class MinorNotifications : MonoBehaviour
{
	public GameObject minorNotificationPrefab;
	public Transform minorNotificationParent;
    public float minorNotificationYSizeIncrease;
    public float minorNotificationXSizeIncrease;
	public Color defaultNotificationColor;
	
	public static MinorNotifications instance;
	
	public List<MinorNotification> minorNotifications;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void NewMinorNotification(string text, Vector2 position, Vector2 controllerPosition, float maxWidth = 100f, float delay = 2f, float riseSpeed = 10f, float fadeTime = 2f, Color? color = null)
	{
		if(ControllerSelection.instance.usingController)
		{
			position = controllerPosition;
		}
		if(minorNotifications.Count > 0)
		{
			minorNotifications[minorNotifications.Count - 1].gameObject.SetActive(true);
			minorNotifications[minorNotifications.Count - 1].StartNotification(text, position, maxWidth, delay, riseSpeed, fadeTime, color);
			minorNotifications.RemoveAt(minorNotifications.Count - 1);
		}
		else
		{
			GameObject newMinorNotificationGO = Instantiate(minorNotificationPrefab, minorNotificationParent);
			MinorNotification newMinorNotification = newMinorNotificationGO.GetComponent<MinorNotification>();
			newMinorNotification.StartNotification(text, position, maxWidth, delay, riseSpeed, fadeTime, color);
		}
	}
}
