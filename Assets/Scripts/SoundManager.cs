using UnityEngine;
// using System.Collections;
using System.Collections.Generic;
using System;

public class SoundManager : MonoBehaviour
{
	public AudioClip[] tickSounds;
	public AudioClip[] cardPickupSounds;
	public AudioClip[] cardDropSounds;
	public AudioClip[] cardSlideSounds;
	public AudioClip[] cardShuffleSounds;
	public AudioClip[] chipSounds;
	public AudioClip[] paintSounds;
	public AudioClip[] pianoSounds;
	public AudioClip[] vampireSounds;
	public AudioClip[] panFluteSounds;
	public AudioClip[] violinSounds;
	public AudioClip[] xylophoneSounds;
	public AudioClip[] splatSounds;
	public AudioClip clickSound;
	public AudioClip coinSpinningSound;
	public AudioClip slideOutSound;
	public AudioClip slideOutSoundReversed;
	public AudioClip bottlePopSound;
	public AudioClip dissolveSound;
	public AudioClip promotionSound;
	public AudioClip demotionSound;
	public AudioClip magicMirrorSound;
	public AudioClip zodiacGainedSound;
	public AudioClip destroyQueensForZodiacsSound;
	public AudioClip sandwichMonarchSound;
	public AudioClip foghornSound;
	public AudioClip diceRollSound;
	public AudioClip partyHornSound;
	public AudioClip rainbowPinwheelSound;
	public AudioClip woodenTileSound;
	public AudioClip explosionSound;
	public AudioClip scoreMultipliedSound;
	public AudioClip roundClearedSound;
	public AudioClip cardUpgradedSound;
	public AudioClip cardMultipliedSound;
	public AudioClip vialLidPoppingOffSound;
	public AudioClip vialLidReturningSound;
	public AudioClip waterRushingSound;
	public AudioClip chipThresholdMissedSound;
	public AudioClip itemPurchasedSound;
	public AudioClip shopRerollSound;
	public AudioClip cardDidNotGivePointsSound;
	public AudioClip negativeZodiacSound;
	public AudioClip pointsAndMultHalvedSound;
	public AudioClip chipMagnetSound;
	public AudioClip addPointsAndMultFromCardsAgainSound;
	public AudioClip gameOverSound;
	public AudioClip gameWonSound;
	public AudioClip endlessModeWonSound;
	public AudioClip slotMachineColumnStoppingSound;
	public AudioClip slotMachineChipGainedSound;
	public AudioClip friendlyFrogSound;
	public AudioClip resourcefulRabbitSound;
	public AudioClip zodiacsForFlushesBaubleSound;
	public AudioClip zodiacsForFlushesCardSound;
	public AudioClip zodiacsForContainedHandsSound;
	
	public AudioSource soundSource;
	public AudioSource tickSource;
	public AudioSource upgradeSource;
	public AudioSource slotMachineChipGainedSource;
	
	private List<Action> soundFunctions = new List<Action>();
	
	public static SoundManager instance;
	public void SetupInstance()
	{
		instance = this;
		soundFunctions.Add(PlayCardPickupSound);
		soundFunctions.Add(PlayCardDropSound);
		soundFunctions.Add(PlayCardSlideSound);
		soundFunctions.Add(PlayCardShuffleSound);
		soundFunctions.Add(PlayClickSound);
		soundFunctions.Add(PlayTickSound);
		soundFunctions.Add(PlayChipSound);
		soundFunctions.Add(PlayBottlePopSound);
		soundFunctions.Add(PlayPaintSound);
		soundFunctions.Add(PlayDissolveSound);
		soundFunctions.Add(PlayPromotionSound);
		soundFunctions.Add(PlayDemotionSound);
		soundFunctions.Add(PlayMagicMirrorSound);
		soundFunctions.Add(PlayZodiacGainedSound);
		soundFunctions.Add(PlayDestroyQueensForZodiacsSound);
		soundFunctions.Add(PlayPianoSound);
		soundFunctions.Add(PlaySandwichMonarchSound);
		soundFunctions.Add(PlayVampireSound);
		soundFunctions.Add(PlayFoghornSound);
		soundFunctions.Add(PlayDiceRollSound);
		soundFunctions.Add(PlayPartyHornSound);
		soundFunctions.Add(PlayPanFluteSound);
		soundFunctions.Add(PlayViolinSound);
		soundFunctions.Add(PlayRainbowPinwheelSound);
		soundFunctions.Add(PlayWoodenTileSound);
		soundFunctions.Add(PlayExplosionSound);
		soundFunctions.Add(PlayScoreMultipliedSound);
		soundFunctions.Add(PlayXylophoneSound);
		soundFunctions.Add(PlayRoundClearedSound);
		// soundFunctions.Add(PlayCardUpdradeSound); // this one needs a parameter
		soundFunctions.Add(PlayVialLidPoppingOffSound);
		soundFunctions.Add(PlayVialLidReturningSound);
		soundFunctions.Add(PlayWaterRushingSound);
		soundFunctions.Add(PlaySplatSound);
		soundFunctions.Add(PlayChipThresholdMissedSound);
		soundFunctions.Add(PlayItemPurchasedSound);
		soundFunctions.Add(PlayShopRerollSound);
		soundFunctions.Add(PlayCardDidNotGivePointsSound);
		soundFunctions.Add(PlayNegativeZodiacSound);
		soundFunctions.Add(PlayPointsAndMultHalvedSound);
		soundFunctions.Add(PlayChipMagnetSound);
		soundFunctions.Add(PlayAddPointsAndMultFromCardsAgainSound);
		soundFunctions.Add(PlayGameOverSound);
		soundFunctions.Add(PlayGameWonSound);
		soundFunctions.Add(PlayEndlessModeWonSound);
	}
	
	public void TestSoundClicked()
	{
		soundFunctions[UnityEngine.Random.Range(0, soundFunctions.Count)]?.Invoke();
	}
	
	public void PlaySound(AudioClip sound, float volumeFactor = 1f)
	{
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			soundSource.PlayOneShot(sound, Preferences.instance.soundVolume * volumeFactor);
		}
	}
	
	public void PlayCardPickupSound()
	{
		PlaySound(cardPickupSounds[UnityEngine.Random.Range(0, cardPickupSounds.Length)], 0.5f);
	}
	
	public void PlayCardDropSound()
	{
		PlaySound(cardDropSounds[UnityEngine.Random.Range(0, cardDropSounds.Length)], 0.5f);
	}
	
	private float lastCardSlideSoundTime;
	
	public void PlayCardSlideSound()
	{
		if(Time.time - lastCardSlideSoundTime > 0.1f)
		{
			PlaySound(cardSlideSounds[UnityEngine.Random.Range(0, cardSlideSounds.Length)], 0.5f);
		}
		lastCardSlideSoundTime = Time.time;
	}
	
	public void PlayCardShuffleSound()
	{
		PlaySound(cardShuffleSounds[UnityEngine.Random.Range(0, cardShuffleSounds.Length)], 0.5f);
	}
	
	public void PlayClickSound()
	{
		PlaySound(clickSound, 0.5f);
	}
	
	public void PlaySlideOutSound(bool reversed = false)
	{
		if(reversed)
		{
			PlaySound(slideOutSound, 0.25f);
		}
		else
		{
			PlaySound(slideOutSoundReversed, 0.25f);
		}
	}
	
	private float lastTickSoundTime = 0;
	private int tickSoundIndex = 0;
	
	public void PlayTickSound()
	{
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			if(Time.time - lastTickSoundTime > 0.2f)
			{
				tickSoundIndex = 0;
			}
			lastTickSoundTime = Time.time;
			tickSource.pitch = 1f + 0.05f * tickSoundIndex;
			tickSource.PlayOneShot(tickSounds[UnityEngine.Random.Range(0,tickSounds.Length)], Preferences.instance.soundVolume * 0.5f);
			tickSoundIndex++;
		}
	}
	
	private float lastChipSoundTime;
	
	public void PlayChipSound()
	{
		if(Time.time - lastChipSoundTime > 0.1f)
		{
			PlaySound(chipSounds[UnityEngine.Random.Range(0, chipSounds.Length)], 0.7f);
		}
		lastChipSoundTime = Time.time;
	}
	
	public void PlayBottlePopSound()
	{
		PlaySound(bottlePopSound);
	}
	
	public void PlayPaintSound()
	{
		PlaySound(paintSounds[UnityEngine.Random.Range(0, paintSounds.Length)], 1f);
	}
	
	private float lastDissolveSoundTime;
	
	public void PlayDissolveSound()
	{
		if(Time.time - lastDissolveSoundTime > 0.1f)
		{
			PlaySound(dissolveSound);
		}
		lastDissolveSoundTime = Time.time;
	}
	
	public void PlayPromotionSound()
	{
		PlaySound(promotionSound);
	}
	
	public void PlayDemotionSound()
	{
		PlaySound(demotionSound);
	}
	
	public void PlayMagicMirrorSound()
	{
		PlaySound(magicMirrorSound);
	}
	
	public void PlayZodiacGainedSound()
	{
		PlaySound(zodiacGainedSound);
	}
	
	public void PlayDestroyQueensForZodiacsSound()
	{
		PlaySound(destroyQueensForZodiacsSound);
	}
	
	private float lastPianoSoundTime;
	private int pianoSoundIndex;
	
	public void PlayPianoSound()
	{
		if(Time.time - lastPianoSoundTime > 2f || pianoSoundIndex >= pianoSounds.Length)
		{
			pianoSoundIndex = 0; 
		}
		lastPianoSoundTime = Time.time;
		PlaySound(pianoSounds[pianoSoundIndex]);
		pianoSoundIndex++;
	}
	
	public void PlaySandwichMonarchSound()
	{
		PlaySound(sandwichMonarchSound, 0.6f);
	}
	
	private float lastVampireSoundTime;
	private int vampireSoundIndex;
	
	public void PlayVampireSound()
	{
		if(Time.time - lastVampireSoundTime > 2f)
		{
			vampireSoundIndex = 0; 
		}
		if(vampireSoundIndex >= vampireSounds.Length)
		{
			vampireSoundIndex = vampireSounds.Length - 1;
		}
		lastVampireSoundTime = Time.time;
		PlaySound(vampireSounds[vampireSoundIndex], 0.7f);
		vampireSoundIndex++;
	}
	
	public void PlayFoghornSound()
	{
		PlaySound(foghornSound);
	}
	
	public void PlayDiceRollSound()
	{
		PlaySound(diceRollSound);
	}
	
	public void PlayPartyHornSound()
	{
		PlaySound(partyHornSound);
	}
	
	private float lastPanFluteSoundTime;
	private int panFluteSoundIndex;
	
	public void PlayPanFluteSound()
	{
		if(Time.time - lastPanFluteSoundTime > 2f || panFluteSoundIndex >= panFluteSounds.Length)
		{
			panFluteSoundIndex = 0; 
		}
		lastPanFluteSoundTime = Time.time;
		PlaySound(panFluteSounds[panFluteSoundIndex], 2f);
		panFluteSoundIndex++;
	}
	
	private float lastViolinSoundTime;
	private int violinSoundIndex;
	
	public void PlayViolinSound()
	{
		if(Time.time - lastViolinSoundTime > 2f || violinSoundIndex >= violinSounds.Length)
		{
			violinSoundIndex = 0; 
		}
		lastViolinSoundTime = Time.time;
		PlaySound(violinSounds[violinSoundIndex]);
		violinSoundIndex++;
	}
	
	public void PlayRainbowPinwheelSound()
	{
		PlaySound(rainbowPinwheelSound);
	}
	
	public void PlayWoodenTileSound()
	{
		PlaySound(woodenTileSound);
	}
	
	private float lastExplosionSoundTime;
	
	public void PlayExplosionSound()
	{
		if(Time.time - lastExplosionSoundTime > 0.1f)
		{
			PlaySound(explosionSound);
		}
		lastExplosionSoundTime = Time.time;
	}
	
	public void PlayScoreMultipliedSound()
	{
		PlaySound(scoreMultipliedSound);
	}
	
	private float lastXylophoneSoundTime;
	private int xylophoneSoundIndex;
	
	public void PlayXylophoneSound()
	{
		if(Time.time - lastXylophoneSoundTime > 2f)
		{
			xylophoneSoundIndex = 0;
		}
		if(xylophoneSoundIndex >= xylophoneSounds.Length)
		{
			return;
		}
		lastXylophoneSoundTime = Time.time;
		PlaySound(xylophoneSounds[xylophoneSoundIndex]);
		xylophoneSoundIndex++;
	}
	
	public void PlayRoundClearedSound()
	{
		PlaySound(roundClearedSound);
	}
	
	private float lastUpgradeSoundTime;
	private int upgradeSoundIndex;
	
	public void PlayCardUpdradeSound(bool multiplied = false)
	{
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			if(Time.time - lastUpgradeSoundTime > 2f)
			{
				upgradeSoundIndex = 0;
			}
			lastUpgradeSoundTime = Time.time;
			upgradeSource.pitch = 1f + 0.05f * upgradeSoundIndex;
			upgradeSource.PlayOneShot((multiplied ? cardMultipliedSound : cardUpgradedSound), Preferences.instance.soundVolume);
			upgradeSoundIndex++;
		}
	}
	
	public void PlayVialLidPoppingOffSound()
	{
		PlaySound(vialLidPoppingOffSound);
	}
	
	public void PlayVialLidReturningSound()
	{
		PlaySound(vialLidReturningSound);
	}
	
	public void PlayWaterRushingSound()
	{
		PlaySound(waterRushingSound);
	}
	
	public void PlaySplatSound()
	{
		PlaySound(splatSounds[UnityEngine.Random.Range(0, splatSounds.Length)]);
	}
	
	public void PlayChipThresholdMissedSound()
	{
		PlaySound(chipThresholdMissedSound);
	}
	
	public void PlayItemPurchasedSound()
	{
		PlaySound(itemPurchasedSound);
	}
	
	public void PlayShopRerollSound()
	{
		PlaySound(shopRerollSound);
	}
	
	public void PlayCardDidNotGivePointsSound()
	{
		PlaySound(cardDidNotGivePointsSound, 0.1f);
	}
	
	public void PlayNegativeZodiacSound()
	{
		PlaySound(negativeZodiacSound, 0.8f);
	}
	
	public void PlayPointsAndMultHalvedSound()
	{
		PlaySound(pointsAndMultHalvedSound, 0.25f);
	}
	
	public void PlayChipMagnetSound()
	{
		PlaySound(chipMagnetSound);
	}
	
	public void PlayAddPointsAndMultFromCardsAgainSound()
	{
		PlaySound(addPointsAndMultFromCardsAgainSound, 0.5f);
	}
	
	public void PlayGameOverSound()
	{
		PlaySound(gameOverSound, 1f);
	}
	
	public void PlayGameWonSound()
	{
		PlaySound(gameWonSound, 1f);
	}
	
	public void PlayEndlessModeWonSound()
	{
		PlaySound(endlessModeWonSound, 0.6f);
	}
	
	private float lastSlotMachineColumnStoppingSoundTime;
	
	public void PlaySlotMachineColumnStoppingSound()
	{
		if(Time.time - lastSlotMachineColumnStoppingSoundTime < 0.1f)
		{
			return;
		}	
		lastSlotMachineColumnStoppingSoundTime = Time.time;
		PlaySound(slotMachineColumnStoppingSound, 1f);
	}
	
	private float lastSlotMachineChipGainedSoundTime;
	private int slotMachineChipGainedSoundIndex;
	
	public void PlaySlotMachineChipGainedSound()
	{
		if(Preferences.instance.soundOn && (Application.isFocused || (!Application.isFocused && !Preferences.instance.muteOnFocusLost)))
		{
			if(Time.time - lastSlotMachineChipGainedSoundTime > 2f)
			{
				slotMachineChipGainedSoundIndex = 0;
			}
			if(Time.time - lastSlotMachineChipGainedSoundTime < 0.1f)
			{
				return;
			}	
			lastSlotMachineChipGainedSoundTime = Time.time;
			slotMachineChipGainedSource.pitch = 1f + 0.05f * slotMachineChipGainedSoundIndex;
			slotMachineChipGainedSource.PlayOneShot(slotMachineChipGainedSound, Preferences.instance.soundVolume * 3f);
			slotMachineChipGainedSoundIndex++;
		}
	}
	
	public void PlayFriendlyFrogSound()
	{
		PlaySound(friendlyFrogSound, 1f);
	}
	
	public void PlayResourcefulRabbitSound()
	{
		PlaySound(resourcefulRabbitSound, 1f);
	}
	
	public void PlayZodiacsForFlushesBaubleSound()
	{
		PlaySound(zodiacsForFlushesBaubleSound, 2.2f);
	}
	
	public void PlayZodiacsForFlushesCardSound()
	{
		PlaySound(zodiacsForFlushesCardSound, 2.2f);
	}
	
	public void PlayZodiacsForContainedHandsSound()
	{
		PlaySound(zodiacsForContainedHandsSound, 1.6f);
	}
}
