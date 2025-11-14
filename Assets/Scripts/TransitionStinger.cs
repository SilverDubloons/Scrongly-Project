using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TransitionStinger : MonoBehaviour
{
    public bool sceneLoaded;
	public bool switchingScenes;
	public RectTransform stingerRT;
	public GameObject diamondObject;
	
	public Vector2 largeSize;
	public float transitionTime;
	
	public static TransitionStinger instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void StartStinger(string sceneToLoad)
	{
		ControllerSelection.instance.currentControllerSelectionGroups.Clear();
		StartCoroutine(StingerAnimation(sceneToLoad));
	}
	
	public IEnumerator StingerAnimation(string sceneToLoad)
	{
		diamondObject.SetActive(true);
		sceneLoaded = false;
		float t = 0;
		stingerRT.gameObject.SetActive(true);
		while(t < transitionTime)
		{
			t += Time.deltaTime;
			stingerRT.sizeDelta = Vector2.Lerp(Vector2.zero, largeSize, t / transitionTime);
			yield return null;
		}
		switchingScenes = true;
		SceneManager.LoadScene(sceneToLoad);
		while(!sceneLoaded)
		{
			yield return null;
		}
		Preferences.instance.CloseMenu();
		LocalInterface.instance.SceneChanged(sceneToLoad, false);
		switchingScenes = false;
		t = 0;
		while(t < transitionTime)
		{
			t += Time.deltaTime;
			stingerRT.sizeDelta = Vector2.Lerp(largeSize, Vector2.zero, t / transitionTime);
			yield return null;
		}
		stingerRT.gameObject.SetActive(true);
		diamondObject.SetActive(false);
	}
}
