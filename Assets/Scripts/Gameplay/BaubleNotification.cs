using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;

public class BaubleNotification : MonoBehaviour
{
    public RectTransform rt;
	public Image image;
	public Label diceLabel;
	public RectTransform diceLabelRT;
	
	public Vector2 origin;
	public Vector2 destination;
	public Vector3 rotationOrigin;
	public Vector3 rotationDestination;
	
	public Vector2 d8LabelLocation;
	public Vector2 d10LabelLocation;
	public Vector2 d12LabelLocation;
	public Vector2 d20LabelLocation;
	
	// public void StartNotification(string baubleTag, float delay = 0, AudioClip audioClipToPlay = null, float volumeFactor = 1f)
	public void StartNotification(string baubleTag, float delay = 0, UnityAction soundFunction = null, int chipsToAdd = 0)
	{
		rt.SetSiblingIndex(0);
		int diceRoll = -1;
		bool maxDiceRoll = false;
		if(baubleTag == "Dice")
		{
			switch(Baubles.instance.GetImpactInt("Dice"))
			{
				case 1:
					diceRoll = RNG.instance.hands.Range(1, 7);
					if(diceRoll == 6)
					{
						maxDiceRoll = true;
					}
					image.sprite = Baubles.instance.diceSprites[diceRoll];
				break;
				case 2:
					diceRoll = RNG.instance.hands.Range(1, 9);
					if(diceRoll == 8)
					{
						maxDiceRoll = true;
					}
					image.sprite = Baubles.instance.diceSprites[7];
					diceLabel.ChangeText(diceRoll.ToString());
					diceLabelRT.anchoredPosition = d8LabelLocation;
				break;
				case 3:
					diceRoll = RNG.instance.hands.Range(1, 11);
					if(diceRoll == 10)
					{
						maxDiceRoll = true;
					}
					image.sprite = Baubles.instance.diceSprites[8];
					diceLabel.ChangeText(diceRoll.ToString());
					diceLabelRT.anchoredPosition = d10LabelLocation;
				break;
				case 4:
					diceRoll = RNG.instance.hands.Range(1, 13);
					if(diceRoll == 12)
					{
						maxDiceRoll = true;
					}
					image.sprite = Baubles.instance.diceSprites[9];
					diceLabel.ChangeText(diceRoll.ToString());
					diceLabelRT.anchoredPosition = d12LabelLocation;
				break;
				case 5:
					diceRoll = RNG.instance.hands.Range(1, 21);
					if(diceRoll == 20)
					{
						maxDiceRoll = true;
					}
					image.sprite = Baubles.instance.diceSprites[10];
					diceLabel.ChangeText(diceRoll.ToString());
					diceLabelRT.anchoredPosition = d20LabelLocation;
				break;
				default:
					Debug.Log($"BaubleNotification dice error, case is {Baubles.instance.GetImpactInt("Dice")}");
				break;
			}
			if(diceRoll >= 10)
			{
				diceLabel.ChangeFontSize(8);
			}
		}
		else
		{
			image.sprite = V.i.v.variantBaubles[baubleTag].sprite;
		}
		// StartCoroutine(NotificationCoroutine(delay, audioClipToPlay, volumeFactor));
		StartCoroutine(NotificationCoroutine(delay, soundFunction, diceRoll, maxDiceRoll, chipsToAdd, baubleTag));
	}
	
	// public IEnumerator NotificationCoroutine(float delay = 0, AudioClip audioClipToPlay = null, float volumeFactor = 1f)
	public IEnumerator NotificationCoroutine(float delay = 0, UnityAction soundFunction = null, int diceRoll = -1, bool maxDiceRoll = false, int chipsToAdd = 0, string baubleTag = "")
	{
		rt.anchoredPosition = origin;
		float t = 0;	
		while(t < delay)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			yield return null;
		}
		t = 0;
		while (t < LocalInterface.instance.animationDuration / 4)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.anchoredPosition = Vector2.Lerp(origin, destination, t / (LocalInterface.instance.animationDuration / 4));
			yield return null;
		}
		t = 0;
		while (t < LocalInterface.instance.animationDuration * 5 / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			yield return null;
		}
		t = 0;
		if(LocalInterface.instance.BaubleNameIsHandMult(baubleTag) >= 0)
		{
			MinorNotifications.instance.NewMinorNotification($"x{LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetImpactDouble(baubleTag))}", Baubles.instance.baubleNotificationMinorNotificationLocation, Baubles.instance.baubleNotificationMinorNotificationLocation, 100f, Baubles.instance.baubleNotificationMinorNotificationDelay / Preferences.instance.gameSpeed, 10f, Baubles.instance.baubleNotificationMinorNotificationFadeTime / Preferences.instance.gameSpeed, LocalInterface.instance.multiplierColor);
		}
		else
		{
			switch(baubleTag)
			{
				case "MultFromKings":
					MinorNotifications.instance.NewMinorNotification($"+{LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetImpactDouble(baubleTag) * PlayArea.instance.GetNumberOfCardsOfRank(11))}", Baubles.instance.baubleNotificationMinorNotificationLocation, Baubles.instance.baubleNotificationMinorNotificationLocation, 100f, Baubles.instance.baubleNotificationMinorNotificationDelay / Preferences.instance.gameSpeed, 10f, Baubles.instance.baubleNotificationMinorNotificationFadeTime / Preferences.instance.gameSpeed, LocalInterface.instance.multiplierColor);
				break;
				case "MultFromRainbowCards":
					MinorNotifications.instance.NewMinorNotification($"+{LocalInterface.instance.ConvertDoubleToString(Baubles.instance.GetImpactDouble(baubleTag) * PlayArea.instance.GetNumberOfCardsOfSuit(4, true))}", Baubles.instance.baubleNotificationMinorNotificationLocation, Baubles.instance.baubleNotificationMinorNotificationLocation, 100f, Baubles.instance.baubleNotificationMinorNotificationDelay / Preferences.instance.gameSpeed, 10f, Baubles.instance.baubleNotificationMinorNotificationFadeTime / Preferences.instance.gameSpeed, LocalInterface.instance.multiplierColor);
				break;
				case "AddPointsAndMultFromCardsAgain":
					Vector2 distanceToAdd = Vector2.zero;
					if(Math.Abs(PlayArea.instance.pointsGainedFromCards) > 0.1d)
					{						
						MinorNotifications.instance.NewMinorNotification($"+{LocalInterface.instance.ConvertDoubleToString(PlayArea.instance.pointsGainedFromCards)}", Baubles.instance.baubleNotificationMinorNotificationLocation, Baubles.instance.baubleNotificationMinorNotificationLocation, 100f, Baubles.instance.baubleNotificationMinorNotificationDelay / Preferences.instance.gameSpeed, 10f, Baubles.instance.baubleNotificationMinorNotificationFadeTime / Preferences.instance.gameSpeed, LocalInterface.instance.pointsColor);
							distanceToAdd += new Vector2(0, 18f);
					}
					if(Math.Abs(PlayArea.instance.multGainedFromCards) > 0.1d)
					{
						MinorNotifications.instance.NewMinorNotification($"+{LocalInterface.instance.ConvertDoubleToString(PlayArea.instance.multGainedFromCards)}", Baubles.instance.baubleNotificationMinorNotificationLocation + distanceToAdd,  Baubles.instance.baubleNotificationMinorNotificationLocation + distanceToAdd, 100f, Baubles.instance.baubleNotificationMinorNotificationDelay / Preferences.instance.gameSpeed, 10f, Baubles.instance.baubleNotificationMinorNotificationFadeTime / Preferences.instance.gameSpeed, LocalInterface.instance.multiplierColor);
					}
				break;
			}
		}
		if(soundFunction != null)
		{
			soundFunction.Invoke();
		}
		if(diceRoll > 0)
		{
			if(diceRoll == 1)
			{
				SoundManager.instance.PlayFoghornSound();
			}
			else
			{
				PlayArea.instance.AddMult((double)diceRoll);
				if(maxDiceRoll)
				{
					SoundManager.instance.PlayPartyHornSound();
				}
				else
				{
					SoundManager.instance.PlayDiceRollSound();
				}
				if(diceRoll >= 6)
				{
					if(diceRoll == 20)
					{
						StartCoroutine(SpawnChips(10));
					}
					else
					{
						StartCoroutine(SpawnChips(1));
					}
				}
			}
		}
		if(chipsToAdd > 0)
		{
			StartCoroutine(SpawnChips(chipsToAdd));
		}
		while (t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.localEulerAngles = Vector3.Lerp(rotationOrigin, rotationDestination, t / (LocalInterface.instance.animationDuration / 8));
			yield return null;
		}
		t = 0;
		while (t < LocalInterface.instance.animationDuration / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.localEulerAngles = Vector3.Lerp(rotationDestination, rotationOrigin, t / (LocalInterface.instance.animationDuration / 8));
			yield return null;
		}
		t = 0;
		while (t < LocalInterface.instance.animationDuration * 5 / 8)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			yield return null;
		}
		t = 0;
		while (t < LocalInterface.instance.animationDuration / 4)
		{
			t += Time.deltaTime * Preferences.instance.gameSpeed;
			rt.anchoredPosition = Vector2.Lerp(destination, origin, t / (LocalInterface.instance.animationDuration / 4));
			yield return null;
		}
		Destroy(this.gameObject);
	}
	
	public IEnumerator SpawnChips(int numberOfChipsToSpawn)
	{
		for(int i = 0; i < numberOfChipsToSpawn; i++)
		{
			GameObject newChipGO = Instantiate(GameManager.instance.chipPrefab, GameManager.instance.chipsParent);
			Chip newChip = newChipGO.GetComponent<Chip>();
			newChip.rt.anchoredPosition = LocalInterface.instance.GetRandomPointWithinRadius(destination, 20f);
			newChip.StartMove();
			yield return null;
		}
	}
}
