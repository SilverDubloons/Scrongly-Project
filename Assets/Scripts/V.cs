using UnityEngine;
using System;

public class V : MonoBehaviour
{
    public Variant v;
	public static V i;
	public string chosenDeck;
	public string chosenDeckDescription;
	public Sprite chosenDeckSprite;
	public int seed;
	public bool loadingGame;
	public string loadingGameInformation;
	public DateTime dateTimeStarted;
	public bool isDailyGame;
	public bool isCustomGame;
	public bool isTutorial;
	public int currentDifficulty;
	
	public void SetupInstance()
	{
		i = this;
	}
}
