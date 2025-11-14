using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;
	
	public AudioClip mainMenuMusic;
	public AudioClip[] gameplayMusic;
	private int[] songOrder;
	private int curSongIndex;
	
	public IEnumerator fadeCoroutine;
	public bool fading;
	
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
		if(!musicSource.isPlaying && Preferences.instance.musicOn)
		{
			switch(LocalInterface.instance.GetCurrentSceneName())
			{
				case "GameplayScene":
					musicSource.clip = gameplayMusic[songOrder[curSongIndex]];
					curSongIndex++;
					if(curSongIndex >= songOrder.Length)
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
		musicSource.volume = Preferences.instance.musicVolume;
		if(!Preferences.instance.musicOn)
		{
			musicSource.Stop();
		}
/* 		else if(Preferences.instance.muteMusicWhenMenuOpen && Preferences.instance.menuOpen)
		{
			StartFade(0);
		} */
	}
	
	public void MenuClosed()
	{
		if(Preferences.instance.musicOn && musicSource.volume < Preferences.instance.musicVolume)
		{
			StartFade(Preferences.instance.musicVolume);
		}
	}
	
	public void StartFade(float destinationVolume)
	{
		if(fading)
		{
			StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = FadeCoroutine(destinationVolume);
		StartCoroutine(fadeCoroutine);
	}
	
	public IEnumerator FadeCoroutine(float destinationVolume)
	{
		fading = true;
		float originVolume = musicSource.volume;
		float t = 0;
		float fadeTime = 1f;
		while(t < fadeTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime, 0, fadeTime);
			musicSource.volume = Mathf.Lerp(originVolume, destinationVolume, t / fadeTime);
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
        if(!hasFocus && Preferences.instance.musicOn && Preferences.instance.muteOnFocusLost)
		{
			StartFade(0);
		}
		if(hasFocus && Preferences.instance.musicOn && musicSource.volume < Preferences.instance.musicVolume)
		{
			if(!(Preferences.instance.muteMusicWhenMenuOpen && Preferences.instance.menuOpen))
			{
				StartFade(Preferences.instance.musicVolume);
			}
		}
    }
}
