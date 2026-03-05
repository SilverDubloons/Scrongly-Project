using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
	public AudioSource trackMenu;
	public AudioSource trackGame;
	public AudioSource trackBoss;
	public AudioSource trackShop;
	public MusicState currentMusicState = MusicState.None;
	
	public AudioClip mainMenuMusic;
	public AudioClip[] gameplayMusic;
	private int[] songOrder;
	private int curSongIndex;
	
	public IEnumerator fadeCoroutine;
	public bool fading;
	private IEnumerator crossFadeCoroutine;
    private bool crossFading;
	
	public static MusicManager instance;
	
	public void SetupInstance()
	{
		instance = this;
		ShuffleSongOrder();
	}
	
	void Update()
	{
		ManageMusic();
	}

	private void ManageMusic()
	{
		if (!Preferences.instance.musicOn)
		{
			return;
		}
		if (Preferences.instance.musicOption == MusicOption.Classic)
		{
			if (!musicSource.isPlaying)
			{
				switch (LocalInterface.instance.GetCurrentSceneName())
				{
					case "GameplayScene":
						musicSource.clip = gameplayMusic[songOrder[curSongIndex]];
						curSongIndex++;
						if (curSongIndex >= songOrder.Length)
						{
							curSongIndex = 0;
						}
						break;
					case "MainMenuScene":
						musicSource.clip = mainMenuMusic;
						break;
				}
				musicSource.Play();
			}
		}
		if (Preferences.instance.musicOption == MusicOption.Scrongly)
		{
			if (!trackMenu.isPlaying && !trackGame.isPlaying && !trackBoss.isPlaying && !trackShop.isPlaying)
			{   // to make sure they're all in sync
                // Debug.Log("Looping Scrongly Music");
                StartScronglyMusic();
            }
		}
	}
	public void StartScronglyMusic()
	{
        // Debug.Log("Starting Scrongly Music");
        trackMenu.Play();
		trackGame.Play();
		trackBoss.Play();
		trackShop.Play();
    }
	public void InitializeScronglyMusic(MusicState initialMusicState)
	{
		// Debug.Log($"InitializeScronglyMusic to {initialMusicState}");
        trackMenu.volume = 0f;
        trackGame.volume = 0f;
        trackBoss.volume = 0f;
        trackShop.volume = 0f;
		switch (initialMusicState)
		{ 
			case MusicState.Menu:
				trackMenu.volume = Preferences.instance.musicVolume;
			break;
            case MusicState.Game:
                trackGame.volume = Preferences.instance.musicVolume;
            break;
            case MusicState.Boss:
                trackBoss.volume = Preferences.instance.musicVolume;
            break;
            case MusicState.Shop:
                trackShop.volume = Preferences.instance.musicVolume;
            break;
        }
		currentMusicState = initialMusicState;
    }
	public void StopClassicMusic()
	{
        musicSource.Stop();
    }
	public void StopScronglyMusic()
	{
		// Debug.Log("Stopping Scrongly Music");
		trackMenu.Stop();
		trackGame.Stop();
		trackBoss.Stop();
		trackShop.Stop();
	}
	private void ShuffleSongOrder()
	{
		songOrder = new int[gameplayMusic.Length];
		for(int i = 0; i < gameplayMusic.Length; i++)
		{
			songOrder[i] = i;
		}
		for(int i = 0; i <gameplayMusic.Length; i++)
		{
			int r = UnityEngine.Random.Range(0, i+1);
			int temp = songOrder[i];
			songOrder[i] = songOrder[r];
			songOrder[r] = temp;
		}
	}
	
	public void MusicOptionsUpdated()
	{
        /* 		bool dontFade = false;
                if(musicSource.volume < LocalInterface.instance.epsilon)
                {
                    dontFade = true;
                } */
        // Debug.Log("MusicOptionsUpdated");
        musicSource.volume = Preferences.instance.musicVolume;
        if (!Preferences.instance.musicOn)
		{
			musicSource.Stop();
			StopScronglyMusic();
        }
		if(Preferences.instance.musicOption == MusicOption.Scrongly)
		{
			if (Preferences.instance.musicOn)
			{
				InitializeScronglyMusic(GetWhatCurrentMusicStateShouldBe());
			}
			else
			{
				StopScronglyMusic();
			}
		}
/* 		else if(Preferences.instance.muteMusicWhenMenuOpen && Preferences.instance.menuOpen)
		{
			StartFade(0);
		} */
	}
	
	public void MenuClosed()
	{
		if (!Preferences.instance.musicOn)
		{
			return;
		}
		switch(Preferences.instance.musicOption)
		{
			case MusicOption.Classic:
				if(musicSource.volume < Preferences.instance.musicVolume)
				{
					StartFade(Preferences.instance.musicVolume, musicSource);
				}
			break;
			case MusicOption.Scrongly:
                if (GetTrackForMusicState(currentMusicState).volume < Preferences.instance.musicVolume)
                {
                    StartFade(Preferences.instance.musicVolume, GetTrackForMusicState(currentMusicState));
                }
            break;
		}
	}
	public void StartFadeCurrentMusic(float destinationVolume)
	{
		switch (Preferences.instance.musicOption)
		{
			case MusicOption.Classic:
				StartFade(destinationVolume, musicSource);
			break;
			case MusicOption.Scrongly:
				if (currentMusicState != MusicState.None)
				{
					StartFade(destinationVolume, GetTrackForMusicState(currentMusicState));
				}
            break;
        }
	}
	public void SetCurrentMusicToVolume(float newVolume)
	{
		switch (Preferences.instance.musicOption)
		{
			case MusicOption.Classic:
				musicSource.volume = newVolume;
			break;
			case MusicOption.Scrongly:
				GetTrackForMusicState(currentMusicState).volume = newVolume;
			break;
		}
    }
	public MusicState GetWhatCurrentMusicStateShouldBe()
	{
        switch (LocalInterface.instance.GetCurrentSceneName())
        {
            case "GameplayScene":
				if (GameManager.instance != null && GameManager.instance.IsThisABossRound())
				{ 
					return MusicState.Boss;
                }
				if (Shop.instance != null && Shop.instance.inShop)
				{ 
					return MusicState.Shop;
                }
				return MusicState.Game;
            case "MainMenuScene":
				return MusicState.Menu;
			default:
				Debug.LogError("Unrecognized scene name for music state: " + LocalInterface.instance.GetCurrentSceneName());
				return MusicState.Menu;
        }
    }
	public void UpdateScronglyMusic()
	{ 
		if(Preferences.instance.musicOption != MusicOption.Scrongly)
		{
			return;
		}
		if(!Preferences.instance.musicOn)
		{
			return;
		}
		if(!trackBoss.isPlaying || !trackGame.isPlaying || !trackMenu.isPlaying || !trackShop.isPlaying)
		{
			StartScronglyMusic();
		}
		MusicState oldMusicState = currentMusicState;
        currentMusicState = GetWhatCurrentMusicStateShouldBe();
		if(oldMusicState == MusicState.None)
		{
			InitializeScronglyMusic(currentMusicState);
			return;
		}
		if (currentMusicState == oldMusicState)
		{
			return;
		}
		// Debug.Log($"Changing music state from {oldMusicState} to {currentMusicState}");
        if (crossFading)
		{
			StopCoroutine(crossFadeCoroutine);
            trackMenu.volume = 0f;
            trackGame.volume = 0f;
            trackBoss.volume = 0f;
            trackShop.volume = 0f;
        }
		crossFadeCoroutine = CrossFadeToMusicState(oldMusicState, currentMusicState);
		StartCoroutine(crossFadeCoroutine);
    }
	public IEnumerator CrossFadeToMusicState(MusicState oldMusicState, MusicState newMusicState)
	{
		crossFading = true;
		float oldStateOriginVolume = GetTrackForMusicState(oldMusicState).volume;
		float newStateOriginVolume = GetTrackForMusicState(newMusicState).volume;
		float oldStateDestinationVolume = 0f;
		float newStateDestinationVolume = Preferences.instance.musicVolume;
        float t = 0;
		float crossFadeTime = 1f;
		while (t < crossFadeTime)
		{ 
			t = Mathf.Clamp(t + Time.deltaTime, 0, crossFadeTime);
			float normalizedTime = t / crossFadeTime;
            GetTrackForMusicState(oldMusicState).volume = Mathf.Lerp(oldStateOriginVolume, oldStateDestinationVolume, normalizedTime);
			GetTrackForMusicState(newMusicState).volume = Mathf.Lerp(newStateOriginVolume, newStateDestinationVolume, normalizedTime);
            yield return null;
        }
		crossFading = false;
    }
    public void StopCurrentMusic()
	{
        switch (Preferences.instance.musicOption)
        {
            case MusicOption.Classic:
				musicSource.Stop();
                break;
            case MusicOption.Scrongly:
				StopScronglyMusic();
            break;
        }
    }
	public void StartFade(float destinationVolume, AudioSource sourceToFade)
	{
		if(fading)
		{
			StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = FadeCoroutine(destinationVolume, sourceToFade);
		StartCoroutine(fadeCoroutine);
	}
	
	public IEnumerator FadeCoroutine(float destinationVolume, AudioSource sourceToFade)
	{
		fading = true;
		float originVolume = sourceToFade.volume;
		// Debug.Log($"Fading from {originVolume} to {destinationVolume} for {sourceToFade.name}");
        float t = 0;
		float fadeTime = 1f;
		while(t < fadeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime, 0, fadeTime);
            sourceToFade.volume = Mathf.Lerp(originVolume, destinationVolume, t / fadeTime);
			yield return null;
		}
		fading = false;
	}

	void OnApplicationFocus(bool hasFocus)
	{
		if (Preferences.instance == null) // for starting the game
		{
			return;
		}
		switch (Preferences.instance.musicOption)
		{ 
			case MusicOption.Classic:
                if (!hasFocus && Preferences.instance.musicOn && Preferences.instance.muteOnFocusLost)
				{
					StartFade(0, musicSource);
				}
				if (hasFocus && Preferences.instance.musicOn && musicSource.volume < Preferences.instance.musicVolume)
				{
					if (!(Preferences.instance.muteMusicWhenMenuOpen && Preferences.instance.menuOpen))
					{
						StartFade(Preferences.instance.musicVolume, musicSource);
					}
				}
				break;
			case MusicOption.Scrongly:
                if (!hasFocus && Preferences.instance.musicOn && Preferences.instance.muteOnFocusLost)
                {
                    StartFade(0, GetTrackForMusicState(currentMusicState));
                }
                if (hasFocus && Preferences.instance.musicOn && GetTrackForMusicState(currentMusicState).volume < Preferences.instance.musicVolume)
                {
                    if (!(Preferences.instance.muteMusicWhenMenuOpen && Preferences.instance.menuOpen))
                    {
                        StartFade(Preferences.instance.musicVolume, GetTrackForMusicState(currentMusicState));
                    }
                }
                break;
        }
    }
	public AudioSource GetTrackForMusicState(MusicState state)
	{
		switch(state)
		{
			case MusicState.Menu:
				return trackMenu;
			case MusicState.Game:
				return trackGame;
			case MusicState.Boss:
				return trackBoss;
			case MusicState.Shop:
				return trackShop;
			default:
				Debug.LogWarning("MusicManager GetTrackForMusicState returning null");
				return null;
		}
    }
}
public enum MusicState 
{ 
	Menu,
	Game,
	Boss,
	Shop,
	None
}