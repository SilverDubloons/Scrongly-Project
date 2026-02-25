using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;

public class Preferences : MonoBehaviour
{
	public GameObject menuVisualObject;
	public GameObject mainMenuObject;
	public GameObject gameplayMenuObject;
	public GameObject audioMenuObject;
	public GameObject videoMenuObject;
	public GameObject controllerMenuObject;
	public GameObject tooltipOptionsMenuObject;
	public GameObject videoOptionsButtonObject;
	public GameObject exitGameButtonObject;
	public GameObject replayTutorialButtonObject;
	public GameObject returnToMenuButtonObject;
	// audio options
	public Toggle soundToggle;
	public Slider soundVolumeSlider;
	public Toggle musicToggle;
	public Slider musicVolumeSlider;
	public Toggle muteOnFocusLostToggle;
	public Toggle muteMusicWhenMenuOpenToggle;
	// gameplay options
	public Slider gameSpeedSlider;
	public Toggle onlyShowModifiedCardValuesToggle;
	public Toggle cheatsOnToggle;
	public Toggle classicThemeToggle;
	public Toggle darkThemeToggle;
    public Slider dragThresholdSlider;
    // video options
    public Toggle borderlessToggle;
	public Toggle fullScreenToggle;
	public Toggle windowedToggle;
	public TMP_Dropdown resolutionDropdown;
	public Toggle vSyncToggle;
	public TMP_Dropdown framerateDropdown;
	public Toggle showFramerateToggle;
	// controller options
	public Slider controllerDeadzoneSlider;
	public Toggle steamDeckToggle;
	public Toggle xBoxToggle;
	public Toggle playstationToggle;
	// toggle options
	public Toggle showCommonTooltipsToggle;
	public Toggle showBaubleTooltipsToggle;
	public Toggle showSpecialCardTooltipsToggle;
	public Toggle showZodiacTooltipsToggle;
	public Toggle showHandTooltipsToggle;
	
 	public ControllerSelectionGroup mainOptionsMenuControllerSelectionGroup; // add the others and remove them all in close
 	public ControllerSelectionGroup audioMenuControllerSelectionGroup;
 	public ControllerSelectionGroup controllerMenuControllerSelectionGroup;
 	public ControllerSelectionGroup gameplayMenuControllerSelectionGroup;
 	public ControllerSelectionGroup tooltipMenuControllerSelectionGroup;
 	public ControllerSelectionGroup videoMenuControllerSelectionGroup;
/*	public ControllerSelectionGroup audioOptionsMenuControllerSelectionGroup;
	public ControllerSelectionGroup gameplayOptionsMenuControllerSelectionGroup;
	public ControllerSelectionGroup vOptionsMenuControllerSelectionGroup; */
	
	public Label framerateLabel;
	public GameObject framerateLabelObject;
	
	private Resolution[] resolutions;
	List<Resolution> uniqueResolutions = new List<Resolution>();
	
    public bool soundOn;
	public bool musicOn;
	public float soundVolume;
	public float musicVolume;
	public bool muteOnFocusLost;
	public float maxTimeBetweenDoubleClicks;
	public bool cheatsOn;
	public bool rotatingBackground;
	public string lastSelectedVariant;
	public float gameSpeed;
	public bool specialCardsSortToLeftOfHand;
	public bool disableExcessScoringAnimation;
	public bool muteMusicWhenMenuOpen;
	public bool showCommonTooltips;
	public bool onlyShowModifiedCardValues;
	public int lastSelectedDifficulty;
	public int displayType; // 0 = borderless, 1 = full screen, 2 = windowed
	public int resolutionX;
	public int resolutionY;
	public bool vSyncOn;
	public int targetFrameRate;
	public bool showFramerate;
	public int glyphSet;
	public float controllerDeadzone;
	public bool showBaubleTooltips;
	public bool showSpecialCardTooltips;
	public bool showZodiacTooltips;
	public bool showHandTooltips;
	public int currentTheme;
	public bool animateLockButton;
	public int  dragThreshold; // 5 to 100, default 20. Higher means more likely to register drags, lower means more likely 
	
	public string preferencesFileName;
	public string preferencesFileVersion;
	
	private bool settingUpUI;
	public bool menuOpen = false;
	
	private float timeSinceLastFramerateUpdate;
	private int framesSinceLastFramerateUpdate;
	public float timeBetweenFramerateUpdates;
	
	private float timeSinceLastResolutionChangeCheck;
	public float timeBetweenResolutionChangeChecks = 5f;
	private Vector2Int lastResolution;
	
	public static Preferences instance;
	
	[System.Serializable]
    public struct FramerateOption
    {
        public string label;
        public int framerateInt;
    }
	
	public FramerateOption[] framerateOptions;
	
	public void SetupInstance()
	{
		instance = this;
		#if UNITY_WEBGL && !UNITY_EDITOR
			videoOptionsButtonObject.SetActive(false);
			exitGameButtonObject.SetActive(false);
		#else
			
		#endif
		menuVisualObject.SetActive(false);
		returnToMenuButtonObject.SetActive(false);
		lastResolution = new Vector2Int(-1, -1);
	}
	
	public void ToggleMenuVisualObject()
	{
		if(menuVisualObject.activeSelf)
		{
			CloseMenu();
		}
		else
		{
			OpenMenu();
		}
	}
		
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleMenuVisualObject();
		}
		if(Input.GetKeyDown(KeyCode.J) && Input.GetKeyDown(KeyCode.Q))
		{
			cheatsOnToggle.gameObject.SetActive(true);
		}
		if(showFramerate)
		{
			timeSinceLastFramerateUpdate += Time.unscaledDeltaTime;
			framesSinceLastFramerateUpdate++;
			if(timeSinceLastFramerateUpdate >= timeBetweenFramerateUpdates)
			{
				float fps = framesSinceLastFramerateUpdate / timeSinceLastFramerateUpdate;
				timeSinceLastFramerateUpdate -= timeBetweenFramerateUpdates;
				framesSinceLastFramerateUpdate = 0;
				framerateLabel.ChangeText($"fps: {fps:F1}");
			}
		}
		#if !UNITY_WEBGL
		if(displayType == 2) // windowed
		{
			timeSinceLastResolutionChangeCheck += timeSinceLastFramerateUpdate += Time.unscaledDeltaTime;
			if(timeSinceLastResolutionChangeCheck >= timeBetweenResolutionChangeChecks)
			{
				timeSinceLastResolutionChangeCheck = 0;
				if(Screen.width != lastResolution.x || Screen.height != lastResolution.y)
				{
					resolutionX = Screen.width;
					resolutionY = Screen.height;
					SetPreferencesFileToCurrentSettings();
				}
				lastResolution = new Vector2Int(Screen.width, Screen.height);
			}
		}
		#endif
	}
	
	public void LoadPreferencesFromFile()
	{
		// Debug.Log("LoadPreferencesFromFile called"); 
		string[] preferencesLines = LocalInterface.instance.GetFileTextLines(preferencesFileName);
		if(preferencesLines != null)
		{
			try
			{
				/* for(int i = 0; i < preferencesLines.Length; i++)
				{
					Debug.Log($"preferencesLines[{i}]={preferencesLines[i]}");
				} */
				if(preferencesLines[0] == preferencesFileVersion)
				{
					soundOn = bool.Parse(preferencesLines[1].Replace("soundOn=", string.Empty));
					musicOn = bool.Parse(preferencesLines[2].Replace("musicOn=", string.Empty));
					soundVolume = float.Parse(preferencesLines[3].Replace("soundVolume=", string.Empty));
					musicVolume = float.Parse(preferencesLines[4].Replace("musicVolume=", string.Empty));
					muteOnFocusLost = bool.Parse(preferencesLines[5].Replace("muteOnFocusLost=", string.Empty));
					maxTimeBetweenDoubleClicks = float.Parse(preferencesLines[6].Replace("maxTimeBetweenDoubleClicks=", string.Empty));
					cheatsOn = bool.Parse(preferencesLines[7].Replace("cheatsOn=", string.Empty));
					rotatingBackground = bool.Parse(preferencesLines[8].Replace("rotatingBackground=", string.Empty));
					lastSelectedVariant = preferencesLines[9].Replace("lastSelectedVariant=", string.Empty);
					gameSpeed = float.Parse(preferencesLines[10].Replace("gameSpeed=", string.Empty));
					specialCardsSortToLeftOfHand = bool.Parse(preferencesLines[11].Replace("specialCardsSortToLeftOfHand=", string.Empty));
					disableExcessScoringAnimation = bool.Parse(preferencesLines[12].Replace("disableExcessScoringAnimation=", string.Empty));
					muteMusicWhenMenuOpen = bool.Parse(preferencesLines[13].Replace("muteMusicWhenMenuOpen=", string.Empty));
					showCommonTooltips = bool.Parse(preferencesLines[14].Replace("showCommonTooltips=", string.Empty));
					onlyShowModifiedCardValues = bool.Parse(preferencesLines[15].Replace("onlyShowModifiedCardValues=", string.Empty));
					lastSelectedDifficulty = int.Parse(preferencesLines[16].Replace("lastSelectedDifficulty=", string.Empty));
					displayType = int.Parse(preferencesLines[17].Replace("displayType=", string.Empty));
					resolutionX = int.Parse(preferencesLines[18].Replace("resolutionX=", string.Empty));
					resolutionY = int.Parse(preferencesLines[19].Replace("resolutionY=", string.Empty));
					vSyncOn = bool.Parse(preferencesLines[20].Replace("vSyncOn=", string.Empty));
					targetFrameRate = int.Parse(preferencesLines[21].Replace("targetFrameRate=", string.Empty));
					showFramerate = bool.Parse(preferencesLines[22].Replace("showFramerate=", string.Empty));
					glyphSet = int.Parse(preferencesLines[23].Replace("glyphSet=", string.Empty));
					controllerDeadzone = float.Parse(preferencesLines[24].Replace("controllerDeadzone=", string.Empty));
					showBaubleTooltips = bool.Parse(preferencesLines[25].Replace("showBaubleTooltips=", string.Empty));
					showSpecialCardTooltips = bool.Parse(preferencesLines[26].Replace("showSpecialCardTooltips=", string.Empty));
					showZodiacTooltips = bool.Parse(preferencesLines[27].Replace("showZodiacTooltips=", string.Empty));
					showHandTooltips = bool.Parse(preferencesLines[28].Replace("showHandTooltips=", string.Empty));
					currentTheme = int.Parse(preferencesLines[29].Replace("currentTheme=", string.Empty));
					if (preferencesLines.Length > 30)
					{
						animateLockButton = bool.Parse(preferencesLines[30].Replace("animateLockButton=", string.Empty));
					}
					else
					{
						animateLockButton = true;
						SetPreferencesFileToCurrentSettings();
                    }
					if (preferencesLines.Length > 31)
					{
						dragThreshold = int.Parse(preferencesLines[31].Replace("dragThreshold=", string.Empty));
					}
					else
					{
						dragThreshold = 20;
						SetPreferencesFileToCurrentSettings();
                    }
				}
				else
				{
					LocalInterface.instance.DisplayError($"Unsupported version mismatch in LoadPreferencesFromFile, your version={preferencesLines[0]}, current version={preferencesFileVersion}. Resetting preferences file");
					ResetPreferencesFile();
				}
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Issue in LoadPreferencesFromFile, message={exception.Message}. Resetting preferences file");
				ResetPreferencesFile();
			}
		}
		else
		{
			Debug.Log("preferencesLines were null. Only normal on first run of game");
			ResetPreferencesFile();
		}
		SetUIToCurrentOptions();
		MusicManager.instance.MusicOptionsUpdated();
		PopulateFramerateDropdown();
		UpdateResolutionOptions();
		DisplayOptionsUpdated("LoadPreferencesFromFile");
	}
	
	public void UpdateResolutionOptions()
	{
		// LocalInterface.instance.DisplayError($"UpdateResolutionOptions Start");
		#if !UNITY_WEBGL
		try
		{
			resolutions = Screen.resolutions;
			resolutionDropdown.ClearOptions();

			List<string> options = new List<string>();
			HashSet<string> existingResolutions = new HashSet<string>();
			int currentResolutionIndex = 0;
			bool  foundSavedResolution = false;
			for (int i = 0; i < resolutions.Length; i++)
			{
				string optionString = $"{resolutions[i].width} x {resolutions[i].height}";
				if (existingResolutions.Add(optionString))
				{
					options.Add(optionString);
					uniqueResolutions.Add(resolutions[i]);
					
				}
				if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && !foundSavedResolution)
				{
					currentResolutionIndex = options.Count - 1;
				}
				if(resolutions[i].width == resolutionX && resolutions[i].height == resolutionY)
				{
					foundSavedResolution = true;
					currentResolutionIndex = options.Count - 1;
				}
			}

			resolutionDropdown.AddOptions(options);
			resolutionDropdown.value = currentResolutionIndex;
			resolutionDropdown.RefreshShownValue();
			// LocalInterface.instance.DisplayError($"UpdateResolutionOptions Conclusion. resolutionDropdown.value={resolutionDropdown.value}, uniqueResolutions.Count={uniqueResolutions.Count}");
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Failed to establish acceptable resolutions: {exception.Message}");
		}
		#endif
	}
	
	void PopulateFramerateDropdown()
    {
        framerateDropdown.ClearOptions();
        List<string> options = new List<string>();
		int savedFramerateIndex = 0;
        //foreach (FramerateOption option in framerateOptions)
		for(int i = 0; i < framerateOptions.Length; i++)
        {
            options.Add(framerateOptions[i].label);
			if(framerateOptions[i].framerateInt == targetFrameRate)
			{
				savedFramerateIndex = i;
			}
        }
        framerateDropdown.AddOptions(options);
		framerateDropdown.value = savedFramerateIndex;
		framerateDropdown.RefreshShownValue();
    }
	
	public void ResetPreferencesFile(bool doNotLoadFile = false)
	{
		// Debug.Log("ResetPreferencesFile called"); 
		soundOn = true;
		musicOn = true;
		soundVolume = 0.5f;
		musicVolume = 0.25f;
		muteOnFocusLost = false;
		maxTimeBetweenDoubleClicks = 0.4f;
		cheatsOn = false;
		rotatingBackground = true;
		lastSelectedVariant = string.Empty;
		gameSpeed = 1f;
		specialCardsSortToLeftOfHand = true;
		disableExcessScoringAnimation = false;
		muteMusicWhenMenuOpen = false;
		showCommonTooltips = true;
		onlyShowModifiedCardValues = true;
		lastSelectedDifficulty = 0;
		displayType = 0;
		resolutionX = Screen.currentResolution.width;
		resolutionY = Screen.currentResolution.height;
		vSyncOn = true;
		targetFrameRate = -1;
		showFramerate = false;
		glyphSet = 0;
		controllerDeadzone = 0.2f;
		showBaubleTooltips = true;
		showSpecialCardTooltips = true;
		showZodiacTooltips = true;
		showHandTooltips = true;
		currentTheme = 0;
        animateLockButton = true;
        dragThreshold = 20;
		string[] preferencesLines = LocalInterface.instance.GetFileTextLines(preferencesFileName);
		if(preferencesLines != null && !doNotLoadFile)
		{
			try
			{
				for(int i = 0; i < preferencesLines.Length; i++)
				{
					string[] preferenceLineData = preferencesLines[i].Split('=');
					if(preferenceLineData.Length > 1)
					{
						switch(preferenceLineData[0])
						{
							case "soundOn":
								soundOn = bool.Parse(preferenceLineData[1]);
							break;
							case "musicOn":
								musicOn = bool.Parse(preferenceLineData[1]);
							break;
							case "soundVolume":
								soundVolume = float.Parse(preferenceLineData[1]);
							break;
							case "musicVolume":
								musicVolume = float.Parse(preferenceLineData[1]);
							break;
							case "muteOnFocusLost":
								muteOnFocusLost = bool.Parse(preferenceLineData[1]);
							break;
							case "maxTimeBetweenDoubleClicks":
								maxTimeBetweenDoubleClicks = float.Parse(preferenceLineData[1]);
							break;
							case "cheatsOn":
								cheatsOn = bool.Parse(preferenceLineData[1]);
							break;
							case "rotatingBackground":
								rotatingBackground = bool.Parse(preferenceLineData[1]);
							break;
							case "lastSelectedVariant":
								lastSelectedVariant = preferenceLineData[1];
							break;
							case "gameSpeed":
								gameSpeed = float.Parse(preferenceLineData[1]);
							break;
							case "specialCardsSortToLeftOfHand":
								specialCardsSortToLeftOfHand = bool.Parse(preferenceLineData[1]);
							break;
							case "disableExcessScoringAnimation":
								disableExcessScoringAnimation = bool.Parse(preferenceLineData[1]);
							break;
							case "muteMusicWhenMenuOpen":
								muteMusicWhenMenuOpen = bool.Parse(preferenceLineData[1]);
							break;
							case "showCommonTooltips":
								showCommonTooltips = bool.Parse(preferenceLineData[1]);
							break;
							case "onlyShowModifiedCardValues":
								onlyShowModifiedCardValues = bool.Parse(preferenceLineData[1]);
							break;
							case "lastSelectedDifficulty":
								lastSelectedDifficulty = int.Parse(preferenceLineData[1]);
							break;
							case "displayType":
								displayType = int.Parse(preferenceLineData[1]);
							break;
							case "resolutionX":
								resolutionX = int.Parse(preferenceLineData[1]);
							break;
							case "resolutionY":
								resolutionY = int.Parse(preferenceLineData[1]);
							break;
							case "vSyncOn":
								vSyncOn = bool.Parse(preferenceLineData[1]);
							break;
							case "targetFrameRate":
								targetFrameRate = int.Parse(preferenceLineData[1]);
							break;
							case "showFramerate":
								showFramerate = bool.Parse(preferenceLineData[1]);
							break;
							case "glyphSet":
								glyphSet = int.Parse(preferenceLineData[1]);
							break;
							case "controllerDeadzone":
								controllerDeadzone = float.Parse(preferenceLineData[1]);
							break;
							case "showBaubleTooltips":
								showBaubleTooltips = bool.Parse(preferenceLineData[1]);
							break;
							case "showSpecialCardTooltips":
								showSpecialCardTooltips = bool.Parse(preferenceLineData[1]);
							break;
							case "showZodiacTooltips":
								showZodiacTooltips = bool.Parse(preferenceLineData[1]);
							break;
							case "showHandTooltips":
								showHandTooltips = bool.Parse(preferenceLineData[1]);
							break;
							case "currentTheme":
								currentTheme = int.Parse(preferenceLineData[1]);
							break;
                            case "animateLockButton":
                                animateLockButton = bool.Parse(preferenceLineData[1]);
                            break;
                            case "dragThreshold":
                                dragThreshold = int.Parse(preferenceLineData[1]);
                            break;
                        }
					}
				}
			}
			catch(Exception exception)
			{
				LocalInterface.instance.DisplayError($"Issue in ResetPreferencesFile, message={exception.Message}. Resetting preferences file without loading old one");
				ResetPreferencesFile(true);
			}
		}
		SetPreferencesFileToCurrentSettings();
	}
	
	public void SetPreferencesFileToCurrentSettings()
	{
		string preferencesText = $"{preferencesFileVersion}\nsoundOn={soundOn.ToString()}\nmusicOn={musicOn.ToString()}\nsoundVolume={soundVolume.ToString()}\nmusicVolume={musicVolume.ToString()}\nmuteOnFocusLost={muteOnFocusLost.ToString()}\nmaxTimeBetweenDoubleClicks={maxTimeBetweenDoubleClicks.ToString()}\ncheatsOn={cheatsOn.ToString()}\nrotatingBackground={rotatingBackground.ToString()}\nlastSelectedVariant={lastSelectedVariant}\ngameSpeed={gameSpeed.ToString()}\nspecialCardsSortToLeftOfHand={specialCardsSortToLeftOfHand.ToString()}\ndisableExcessScoringAnimation={disableExcessScoringAnimation.ToString()}\nmuteMusicWhenMenuOpen={muteMusicWhenMenuOpen.ToString()}\nshowCommonTooltips={showCommonTooltips}\nonlyShowModifiedCardValues={onlyShowModifiedCardValues}\nlastSelectedDifficulty={lastSelectedDifficulty}\ndisplayType={displayType}\nresolutionX={resolutionX}\nresolutionY={resolutionY}\nvSyncOn={vSyncOn}\ntargetFrameRate={targetFrameRate}\nshowFramerate={showFramerate}\nglyphSet={glyphSet}\ncontrollerDeadzone={controllerDeadzone}\nshowBaubleTooltips={showBaubleTooltips}\nshowSpecialCardTooltips={showSpecialCardTooltips}\nshowZodiacTooltips={showZodiacTooltips}\nshowHandTooltips={showHandTooltips}\ncurrentTheme={currentTheme}\nanimateLockButton={animateLockButton}\ndragThreshold={dragThreshold}";
		LocalInterface.instance.SetFileText(preferencesFileName, preferencesText);
	}


    public void MenuButtonClicked()
	{
		menuVisualObject.SetActive(true);
	}
	
	public void SetUIToCurrentOptions()
	{
		// Debug.Log("SetUIToCurrentOptions called"); 
		settingUpUI = true;
		// audio
		soundToggle.isOn = soundOn;
		soundVolumeSlider.value = soundVolume;
		musicToggle.isOn = musicOn;
		musicVolumeSlider.value = musicVolume;
		muteOnFocusLostToggle.isOn = muteOnFocusLost;
		muteMusicWhenMenuOpenToggle.isOn = muteMusicWhenMenuOpen;
		// gameplay
		gameSpeedSlider.value = (int)(Mathf.Log(gameSpeed, 2) + 1);
		showCommonTooltipsToggle.isOn = showCommonTooltips;
		onlyShowModifiedCardValuesToggle.isOn = onlyShowModifiedCardValues;
		cheatsOnToggle.isOn = cheatsOn;
		cheatsOnToggle.gameObject.SetActive(cheatsOn);
		dragThresholdSlider.value = dragThreshold;
        if (LocalInterface.instance.GetCurrentSceneName() == "GameplayScene")
		{
			GameManager.instance.SetVisibilityOfCheatOptions(cheatsOn);
		}
		switch(displayType)
		{
			case 0:
				borderlessToggle.isOn = true;
				fullScreenToggle.isOn = false;
				windowedToggle.isOn = false;
			break;
			case 1:
				fullScreenToggle.isOn = true;
				windowedToggle.isOn = false;
				borderlessToggle.isOn = false;
			break;
			case 2:
				windowedToggle.isOn = true;
				fullScreenToggle.isOn = false;
				borderlessToggle.isOn = false;
			break;
		}
		vSyncToggle.isOn = vSyncOn;
		showFramerateToggle.isOn = showFramerate;
		framerateLabelObject.SetActive(showFramerate);
		controllerDeadzoneSlider.value = controllerDeadzone;
		switch(glyphSet)
		{
			case 0:
				steamDeckToggle.isOn = true;
				xBoxToggle.isOn = false;
				playstationToggle.isOn = false;
			break;
			case 1:
				xBoxToggle.isOn = true;
				steamDeckToggle.isOn = false;
				playstationToggle.isOn = false;
			break;
			case 2:
				playstationToggle.isOn = true;
				steamDeckToggle.isOn = false;
				xBoxToggle.isOn = false;
			break;
		}
		showBaubleTooltipsToggle.isOn = showBaubleTooltips;
		showSpecialCardTooltipsToggle.isOn = showSpecialCardTooltips;
		showZodiacTooltipsToggle.isOn = showZodiacTooltips;
		showHandTooltipsToggle.isOn = showHandTooltips;
		switch(currentTheme)
		{
			case 0:
				classicThemeToggle.isOn = true;
				darkThemeToggle.isOn = false;
			break;
			case 1:
				darkThemeToggle.isOn = true;
				classicThemeToggle.isOn = false;
			break;
		}
		// Debug.Log($"SetUIToCurrentOptions, currentTheme={currentTheme}");
		settingUpUI = false;
	}
	
	public void SetOptionsToCurrentUIValues()
	{
		// Debug.Log("SetOptionsToCurrentUIValues called"); 
		if(settingUpUI)
		{
			return;
		}
		// Debug.Log("SetOptionsToCurrentUIValues executing"); 
		// audio
		soundOn = soundToggle.isOn;
		soundVolume = soundVolumeSlider.value;
		musicOn = musicToggle.isOn;
		musicVolume = musicVolumeSlider.value;
		muteOnFocusLost = muteOnFocusLostToggle.isOn;
		if(musicOn)
		{
			if(muteMusicWhenMenuOpenToggle.isOn && !muteMusicWhenMenuOpen)
			{
				MusicManager.instance.StartFade(0);
			}
			else if(!muteMusicWhenMenuOpenToggle.isOn && muteMusicWhenMenuOpen)
			{
				MusicManager.instance.StartFade(musicVolume);
			}
			else if(!muteMusicWhenMenuOpen)
			{
				MusicManager.instance.musicSource.volume = musicVolume;
			}
		}
		else
		{
			MusicManager.instance.musicSource.Stop();
		}
		muteMusicWhenMenuOpen = muteMusicWhenMenuOpenToggle.isOn;
		// gameplay
		gameSpeed = Mathf.Pow(2, gameSpeedSlider.value) / 2;
		showCommonTooltips = showCommonTooltipsToggle.isOn;
		onlyShowModifiedCardValues = onlyShowModifiedCardValuesToggle.isOn;
		cheatsOn = cheatsOnToggle.isOn;
		dragThreshold = (int)dragThresholdSlider.value;
        EventSystem.current.pixelDragThreshold = dragThreshold;
        if (LocalInterface.instance.GetCurrentSceneName() == "GameplayScene")
		{
			GameManager.instance.SetVisibilityOfCheatOptions(cheatsOn);
		}
		controllerDeadzone = controllerDeadzoneSlider.value;
		if(steamDeckToggle.isOn)
		{
			glyphSet = 0;
		}
		else if(xBoxToggle.isOn)
		{
			glyphSet = 1;
		}
		else if(playstationToggle.isOn)
		{
			glyphSet = 2;
		}
		showBaubleTooltips = showBaubleTooltipsToggle.isOn;
		showSpecialCardTooltips = showSpecialCardTooltipsToggle.isOn;
		showZodiacTooltips = showZodiacTooltipsToggle.isOn;
		showHandTooltips = showHandTooltipsToggle.isOn;
		if(classicThemeToggle.isOn)
		{
			currentTheme = 0;
		}
		else if(darkThemeToggle.isOn)
		{
			currentTheme = 1;
		}
		// Debug.Log($"SetOptionsToCurrentUIValues, currentTheme={currentTheme}");
		ControllerSelection.instance.UpdateHotkeys();
	}
	
	public void UpdateTheme()
	{
		// currentTheme = themeNumber;
		if(classicThemeToggle.isOn)
		{
			currentTheme = 0;
		}
		else if(darkThemeToggle.isOn)
		{
			currentTheme = 1;
		}
		// Debug.Log($"UpdateTheme, currentTheme={currentTheme}");
		if(settingUpUI)
		{
			return;
		}
		ThemeManager.instance.ApplyTheme(currentTheme);
	}
	
	public void CloseMenu()
	{
		menuOpen = false;
		menuVisualObject.SetActive(false);
		// MusicManager.instance.MusicOptionsUpdated();
		MusicManager.instance.MenuClosed();
		if(muteMusicWhenMenuOpen)
		{
			MusicManager.instance.StartFade(musicVolume);
		}
		mainOptionsMenuControllerSelectionGroup.RemoveFromCurrentGroups();
		audioMenuControllerSelectionGroup.RemoveFromCurrentGroups();
		controllerMenuControllerSelectionGroup.RemoveFromCurrentGroups();
		gameplayMenuControllerSelectionGroup.RemoveFromCurrentGroups();
		tooltipMenuControllerSelectionGroup.RemoveFromCurrentGroups();
		videoMenuControllerSelectionGroup.RemoveFromCurrentGroups();
	}
	
	public void OpenMenu()
	{
		menuVisualObject.SetActive(true);
		mainMenuObject.SetActive(true);
		gameplayMenuObject.SetActive(false);
		audioMenuObject.SetActive(false);
		videoMenuObject.SetActive(false);
		controllerMenuObject.SetActive(false);
		tooltipOptionsMenuObject.SetActive(false);
		menuOpen = true;
		// MusicManager.instance.MusicOptionsUpdated();
		if(muteMusicWhenMenuOpen)
		{
			MusicManager.instance.StartFade(0);
		}
		mainOptionsMenuControllerSelectionGroup.AddToCurrentGroups();
	}
	
	public void SceneChanged(string newSceneName)
	{
		if(newSceneName == "MainMenuScene")
		{
			returnToMenuButtonObject.SetActive(false);
			replayTutorialButtonObject.SetActive(true);
		}
		else
		{
			returnToMenuButtonObject.SetActive(true);
			replayTutorialButtonObject.SetActive(false);
		}
	}
	
	public void ReturnToMenuClicked()
	{
		string dateTimeSaved = RunInformation.instance.GetSaveParameter("dateTimeSaved");
		if(dateTimeSaved == null)
		{
			ReturnToMenu();
		}
		else
		{
			string timeSinceLastSave = LocalInterface.GetTimeDifferenceString(DateTime.Parse(dateTimeSaved));
			OptionsDialog.instance.SetupDialog($"Are you sure you want to return to the main menu? Last save was {timeSinceLastSave} ago", new string[1]{"Yes"}, new ThemeManager.UIElementType[1]{ThemeManager.UIElementType.standardButtonActive}, new UnityAction[1]{ReturnToMenu});
		}
	}
	
	public void ReturnToMenu()
	{
		TransitionStinger.instance.StartStinger("MainMenuScene");
		OptionsDialog.instance.SetVisibility(false);
		mainMenuObject.SetActive(false);
		gameplayMenuObject.SetActive(false);
		audioMenuObject.SetActive(false);
		videoMenuObject.SetActive(false);
	}
	
	public void ExitGameClicked()
	{
		if(LocalInterface.instance.GetCurrentSceneName() == "GameplayScene")
		{
			string dateTimeSaved = RunInformation.instance.GetSaveParameter("dateTimeSaved");
			if(dateTimeSaved == null)
			{
				ExitGame();
			}
			else
			{
				string timeSinceLastSave = LocalInterface.GetTimeDifferenceString(DateTime.Parse(dateTimeSaved));
				OptionsDialog.instance.SetupDialog($"Are you sure you want to exit the game? Last save was {timeSinceLastSave} ago", new string[1]{"Yes"}, new ThemeManager.UIElementType[1]{ThemeManager.UIElementType.standardButtonActive}, new UnityAction[1]{ExitGame});
			}
		}
		else
		{
			ExitGame();
		}
	}
	
	public void ExitGame()
	{
		Application.Quit();
	}
	
	public void ResolutionDropdownUpdated()
	{
		// LocalInterface.instance.DisplayError($"ResolutionDropdownUpdated Start");
		try
		{
			Resolution selectedResolution = uniqueResolutions[resolutionDropdown.value];
			if(selectedResolution.width != Screen.currentResolution.width || selectedResolution.height != Screen.currentResolution.height)
			{
				Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
				// Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);
				resolutionX = selectedResolution.width;
				resolutionY = selectedResolution.height;
			}
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Failed to properly set resolution: {exception.Message} resolutionDropdown.value={resolutionDropdown.value}, uniqueResolutions.Count={uniqueResolutions.Count}");
		}
	}
	
	public void DisplayOptionsUpdated(string callLocation)
	{
		if(settingUpUI || TransitionStinger.instance.switchingScenes)
		{
			return;
		}
		// LocalInterface.instance.DisplayError($"DisplayOptionsUpdated Start displayType={displayType} callLocation={callLocation}");
		#if !UNITY_WEBGL
		if(borderlessToggle.isOn)
		{
			displayType = 0;
		}
		if(fullScreenToggle.isOn)
		{
			displayType = 1;
		}
		if(windowedToggle.isOn)
		{
			displayType = 2;
		}
		// LocalInterface.instance.DisplayError($"DisplayOptionsUpdated Is not in WEBGL mode displayType={displayType}");
		switch(displayType)
		{
			case 0:
				if(Screen.fullScreenMode != FullScreenMode.FullScreenWindow)
				{
					Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				}
			break;
			case 1:
				if(Screen.fullScreenMode != FullScreenMode.ExclusiveFullScreen)
				{
					Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				}
			break;
			case 2:
				if(Screen.fullScreenMode != FullScreenMode.Windowed)
				{
					Screen.fullScreenMode = FullScreenMode.Windowed;
				}
			break;
		}
		
		if (vSyncToggle.isOn)
		{
            QualitySettings.vSyncCount = 1;
			framerateDropdown.interactable = false;
		}
        else
		{
            QualitySettings.vSyncCount = 0;
			framerateDropdown.interactable = true;
		}
		try
		{
			Application.targetFrameRate = framerateOptions[framerateDropdown.value].framerateInt;
		}
		catch(Exception exception)
		{
			LocalInterface.instance.DisplayError($"Failed to properly set framerate: {exception.Message} framerateDropdown.value={framerateDropdown.value}, framerateOptions.Length={framerateOptions.Length}");
		}
		showFramerate = showFramerateToggle.isOn;
		framerateLabelObject.SetActive(showFramerate);
		#endif
	}
	
	public void ReplayTutorialClicked()
	{
		MainMenu.instance.StartTutorial();
	}
}
