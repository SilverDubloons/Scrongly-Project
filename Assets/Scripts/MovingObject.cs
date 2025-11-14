using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MovingObject : MonoBehaviour
{
	public string referenceName;
	public RectTransform rt;
	[System.Serializable]
	public class Location
	{
		public string locationName;
		public Vector2 locationVector2;
		public UnityEvent startEvent;
		public UnityEvent finishEvent;
	}
	public Location[] locations;
	private Dictionary<string, Location> locationsDictionary = new Dictionary<string, Location>();
	private IEnumerator MoveCoroutine;
	private bool moving;
	
	public void SetupLocationsDictionary()
	{
		for(int i = 0; i < locations.Length; i++)
		{
			locationsDictionary.Add(locations[i].locationName, locations[i]);
		}
	}
	
	public void StartMove(string destinationName, float delay = 0f, float speedFactor = 1f)
	{
		if(moving)
		{
			StopCoroutine(MoveCoroutine);
		}
		MoveCoroutine = MoveObject(locationsDictionary[destinationName], delay, speedFactor);
		StartCoroutine(MoveCoroutine);
	}
	
	public void TeleportTo(string destinationName)
	{
		if(moving)
		{
			StopCoroutine(MoveCoroutine);
		}
		rt.anchoredPosition = locationsDictionary[destinationName].locationVector2;
	}
	
	public IEnumerator MoveObject(Location destination, float delay = 0f, float speedFactor = 1f)
	{
		moving = true;
		while(delay > 0f)
		{
			delay -= Time.deltaTime * Preferences.instance.gameSpeed * speedFactor;
			yield return null;
		}
		destination.startEvent.Invoke();
		Vector2 origin = rt.anchoredPosition;
		float t = 0;
		while(t < LocalInterface.instance.animationDuration)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed * speedFactor;
			rt.anchoredPosition = Vector2.Lerp(origin, destination.locationVector2, LocalInterface.instance.animationCurve.Evaluate(t / LocalInterface.instance.animationDuration));
			if(ControllerSelection.instance.usingController && referenceName == "DeckPreview")
			{
				if(ControllerSelection.instance.currentlySelectedObject != null)
				{
					ControllerSelection.instance.RepositionControllerSelectionRT(ControllerSelection.instance.currentlySelectedObject, HandArea.instance.cardsControllerSelectionGroup);
					if(ControllerSelection.instance.currentlySelectedObject.isCard)
					{
						ControllerSelection.instance.currentlySelectedObject.card.OnPointerEnter(new PointerEventData(EventSystem.current));
					}
				}
			}
			yield return null;
		}
		rt.anchoredPosition = destination.locationVector2;
		destination.finishEvent.Invoke();
		moving = false;
	}
	
	public string GetCurrentLocation()
	{
		foreach(KeyValuePair<string, Location> entry in locationsDictionary)
		{
			//if(entry.Value.locationVector2 == rt.anchoredPosition)
			if(Mathf.Abs(entry.Value.locationVector2.x - rt.anchoredPosition.x) < 0.1f && Mathf.Abs(entry.Value.locationVector2.y - rt.anchoredPosition.y) < 0.1f)
			{
				return entry.Key;
			}
		}
		LocalInterface.instance.DisplayError($"Could not get current location of moving object with referenceName {referenceName}");
		return null;
	}
	
	public void DisplayLocationDictionary()
	{
		Debug.Log($"Displaying location dictionary for {referenceName}");
		foreach(KeyValuePair<string, Location> entry in locationsDictionary)
		{
			Debug.Log($"Key={entry.Key}, locationVector2={entry.Value.locationVector2}, locationName={entry.Value.locationName}");
		}
	}
}
