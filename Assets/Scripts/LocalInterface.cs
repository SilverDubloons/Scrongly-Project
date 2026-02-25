using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

// C:\Users\jsilv\AppData\LocalLow\SilverDubloons\ScronglyEnhanced
// D:\Unity Projects\ScronglyEnhanced\Assets\Text
// D:\Unity Projects\ScronglyEnhanced\Assets\Scripts
// D:\Unity Projects\ScronglyEnhanced\Assets\Images

// run_app_build "D:\Steamworks\steamworks_sdk_162\sdk\tools\ContentBuilder\scripts\simple_app_build.vdf"

public class LocalInterface : MonoBehaviour
{
	public AnimationCurve animationCurve;
	public float animationDuration;
	public Vector2 referenceResolution;
	public Color[] suitColors;
	public Rarity[] rarities;
	public List<BossTier> bossTiers = new List<BossTier>();
	public Dictionary<string, BossLevel> bossLevels = new Dictionary<string, BossLevel>();
	public Dictionary<string, Rarity> rarityDictionary = new Dictionary<string, Rarity>();
	public string[] handNames;
	public string[] numbers;
	public float epsilon;
	public Color defaultTextColor;
	public Color baubleNameColor;
	public Color specialCardNameColor;
	public Color zodiacNameColor;
	public Color pointsColor;
	public Color multiplierColor;
	public Color handNameColor;
	public Color transparentColor;
	public Color opaqueColor;
	public Color negativeZodiacColor;
	public Color deckNameColor;
	public Color greenXColor;
	public Color redXColor;
	public int[] suitOrder; // 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
	public int[] suitOrderIndices; // 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
	public string[] suitNames;
	public Dictionary<string, int> suitOrderDictionary = new Dictionary<string, int>();
	public Dictionary<string, int> baseSuitOrderDictionary = new Dictionary<string, int>();
	public Dictionary<int, string> suitNamesByOrderDictionary = new Dictionary<int, string>();
	public Vector2 cardSize;
	public int smallFontSize;
	public int largeFontSize;
	public double minimumRoundThresholdDifference;
	public string gameName;
	public string localFilesDirectory;
	public GameObject[] persistentObjects;
	public string variantFileVersion;
	public GameObject cardPrefab;
	public Canvas mainMenuCanvas;
	public Canvas persistentCanvas;
	public Sprite circleFourPixel;
	public Sprite circleEightPixel;
	public Sprite[] puzzlePieceSprites; // 0 = top left, 1 = top right, 2 = bottom left, 3 = bottom right
	
	public Variant baseVariant;
	
	public DeckPicker deckPicker;
	public VariantsMenu variantsMenu;
	public Preferences preferences;
	public MusicManager musicManager;
	public MovingObjects movingObjects;
	public SoundManager soundManager;
	public OptionsDialog optionsDialog;
	public Tooltip tooltip;
	public MinorNotifications minorNotifications;
	public TransitionStinger transitionStinger;
	public V v;
	public Decks decks;
	public VariantExplainer variantExplainer;
	public Stats stats;
	public DifficultySelector difficultySelector;
	public UnlocksMenu unlocksMenu;
	public DailyMenu dailyMenu;
	public StatsMenu statsMenu;
	public ControllerSelection controllerSelection;
	public OnScreenKeyboard onScreenKeyboard;
	public MainMenu mainMenu;
	
	public string unlockedBaublesFileName;
	public string unlockedBaublesFileVersion;
	public List<string> unlockedBaubles = new List<String>();
	public string saveGameFileVersion;
	public string standardGameSaveFileName;
	public string dailyGameSaveFileName;
	public string customGameSaveFileName;
	public string completedStandardGamesFolderName;
	public string completedDailyGamesFolderName;
	public string completedCustomGamesFolderName;
	public string errorLogFileName;
	
	public string unlockedSpecialCardsFileName;
	public string unlockedSpecialCardsFileVersion;
	public List<string> unlockedSpecialCards = new List<String>();
	
	public InputGlyphSet[] inputGlyphSets; // 0 = steam deck, 1 = xbox, 2 = playstation
	public TMP_SpriteAsset[] inputGlyphSpriteAssets;
	
	[DllImport("__Internal")]
    private static extern void JS_FileSystem_Sync();
	
	[System.Serializable]
	public struct InputGlyphSet
	{
		public Sprite bottomButtonActive;
		public Sprite bottomButtonDisabled;
		public Sprite rightButtonActive;
		public Sprite rightButtonDisabled;
		public Sprite leftButtonActive;
		public Sprite leftButtonDisabled;
		public Sprite topButtonActive;
		public Sprite topButtonDisabled;
		public Sprite l1ButtonActive;
		public Sprite l1ButtonDisabled;
		public Sprite l2ButtonActive;
		public Sprite l2ButtonDisabled;
		public Sprite r1ButtonActive;
		public Sprite r1ButtonDisabled;
		public Sprite r2ButtonActive;
		public Sprite r2ButtonDisabled;
		public Sprite selectButtonActive;
		public Sprite selectButtonDisabled;
		public Sprite startButtonActive;
		public Sprite startButtonDisabled;
		public Sprite leftStickButtonActive;
		public Sprite leftStickButtonDisabled;
		public Sprite rightStickButtonActive;
		public Sprite rightStickButtonDisabled;
		public Sprite DPadUpActive;
		public Sprite DPadUpDisabled;
		public Sprite DPadLeftActive;
		public Sprite DPadLeftDisabled;
		public Sprite DPadRightActive;
		public Sprite DPadRightDisabled;
		public Sprite DPadDownActive;
		public Sprite DPadDownDisabled;
		public Sprite RightStickVerticalActive;
		public Sprite RightStickVerticalDisabled;
		public Sprite RightStickHorizontalActive;
		public Sprite RightStickHorizontalDisabled;
	}
	
	public Sprite GetHotkeySpriteForAction(string actionName, bool active)
	{
		switch(actionName)
		{
			case "SouthButton":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].bottomButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].bottomButtonDisabled;
				}
			case "EastButton":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].rightButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].rightButtonDisabled;
				}
			case "WestButton":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].leftButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].leftButtonDisabled;
				}
			case "NorthButton":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].topButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].topButtonDisabled;
				}
			case "L1Button":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].l1ButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].l1ButtonDisabled;
				}
			case "L2Button":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].l2ButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].l2ButtonDisabled;
				}
			case "R1Button":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].r1ButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].r1ButtonDisabled;
				}
			case "R2Button":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].r2ButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].r2ButtonDisabled;
				}
			case "SelectButton":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].selectButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].selectButtonDisabled;
				}
			case "StartButton":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].startButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].startButtonDisabled;
				}
			case "LeftStickPress":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].leftStickButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].leftStickButtonDisabled;
				}
			case "RightStickPress":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].rightStickButtonActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].rightStickButtonDisabled;
				}
			case "DPadUp":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadUpActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadUpDisabled;
				}
			case "DPadLeft":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadLeftActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadLeftDisabled;
				}
			case "DPadRight":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadRightActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadRightDisabled;
				}
			case "DPadDown":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadDownActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].DPadDownDisabled;
				}
			case "RightStickVertical":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].RightStickVerticalActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].RightStickVerticalDisabled;
				}
			case "RightStickHorizontal":
				if(active)
				{
					return inputGlyphSets[Preferences.instance.glyphSet].RightStickHorizontalActive;
				}
				else
				{
					return inputGlyphSets[Preferences.instance.glyphSet].RightStickHorizontalDisabled;
				}
		}
		LocalInterface.instance.DisplayError($"GetHotkeySpriteForAction failed with actionName={actionName} and active={active}");
		return null;
	}
	
	[System.Serializable]
	public struct Rarity
	{
		public string rarityName;
		public Color rarityColor;
		public int rarityInt;
	}
	
	public class BossTier
	{
		public List<BossLevel> bossLevels = new List<BossLevel>();
	}
	
	public struct BossLevel
	{
		public string tag;
		public string description;
		public int tier;
		public BossLevel(string tag, string description, int tier)
		{
			this.tag = tag;
			this.description = description;
			this.tier = tier;
		}
	}
	
/* 	void Update()
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			for(int i = 0; i < bossTiers.Count; i++)
			{
				for(int j = 0; j < bossTiers[i].bossLevels.Count; j++)
				{
					Debug.Log($"bossTiers[{i}].bossLevels[{j}]: tag={bossTiers[i].bossLevels[j].tag}, description={bossTiers[i].bossLevels[j].description}");
				}
			}
		}
	} */
	
	public string ConvertIntToRarity(int rarityInt)
	{
		switch(rarityInt)
		{
			case 0:
				return "Common";
			case 1:
				return "Uncommon";
			case 2:
				return "Rare";
			case 3:
				return "Legendary";
			case 4:
				return "Special";
			case 5:
				return "Zodiac";
			default:
				return "Rarity Undefined";
		}
	}
	
    public static LocalInterface instance;
	void Awake()
	{
		if(SoundManager.instance == null)	// only will run once when game starts
		{
			instance = this;
			#if UNITY_WEBGL && !UNITY_EDITOR
			if(!Directory.Exists($"/idbfs/{gameName}"))
			{
				Directory.CreateDirectory($"/idbfs/{gameName}");
			}
			localFilesDirectory = "/idbfs/{gameName}/";
			#else
				localFilesDirectory = $"{Application.persistentDataPath}/";
			#endif
			if(!Directory.Exists($"{localFilesDirectory}Variants/"))
			{
				Directory.CreateDirectory($"{localFilesDirectory}Variants/");
			}
			if(!Directory.Exists($"{localFilesDirectory}{completedStandardGamesFolderName}/"))
			{
				Directory.CreateDirectory($"{localFilesDirectory}{completedStandardGamesFolderName}/");
			}
			if(!Directory.Exists($"{localFilesDirectory}{completedDailyGamesFolderName}/"))
			{
				Directory.CreateDirectory($"{localFilesDirectory}{completedDailyGamesFolderName}/");
			}
			if(!Directory.Exists($"{localFilesDirectory}{completedCustomGamesFolderName}/"))
			{
				Directory.CreateDirectory($"{localFilesDirectory}{completedCustomGamesFolderName}/");
			}
			transitionStinger.SetupInstance();
			soundManager.SetupInstance();
			preferences.SetupInstance();
			musicManager.SetupInstance();
			optionsDialog.SetupInstance();
			tooltip.SetupInstance();
			minorNotifications.SetupInstance();
			v.SetupInstance();
			decks.SetupInstance();
			variantExplainer.SetupInstance();
			stats.SetupInstance();
			controllerSelection.SetupInstance();
			onScreenKeyboard.SetupInstance();
			
			decks.LoadDecksFromSpreadsheet();
			variantExplainer.Close();
			
			preferences.LoadPreferencesFromFile();
			stats.CheckForStatsFile();
			
			for(int i = 0; i < persistentObjects.Length; i++)
			{
				DontDestroyOnLoad(persistentObjects[i]);
			}
			for(int i = 0; i < rarities.Length; i++)
			{
				rarityDictionary.Add(rarities[i].rarityName, rarities[i]);
			}
			
			SceneChanged("MainMenuScene", true);
		}
		else
		{
			for(int i = persistentObjects.Length - 1; i >= 0; i--)
			{
				Destroy(persistentObjects[i].gameObject);
			}
		}
	}
	
	public void RedefineSuitOrders()
	{
		suitOrderDictionary.Clear();
		suitNamesByOrderDictionary.Clear();
		baseSuitOrderDictionary.Clear();
		for(int i = 0; i < suitOrder.Length; i++)
		{
			suitOrderDictionary.Add(suitNames[i], suitOrder[i]);
			suitNamesByOrderDictionary.Add(suitOrder[i], suitNames[i]);
			baseSuitOrderDictionary.Add(suitNames[i], i); 
		}
	}
	
	void Start()
	{
		// SceneChanged("MainMenuScene");
	}
	
	public string GetCurrentSceneName()
	{
		Scene currentScene = SceneManager.GetActiveScene();
		string sceneName = currentScene.name;
		return sceneName;	// GameplayScene || MainMenuScene
	}
	
	public void FileUpdated()
	{
		#if UNITY_WEBGL && !UNITY_EDITOR
			JS_FileSystem_Sync();
		#endif
	}
	
	public void BlueSkyClicked()
	{
		Application.OpenURL("https://bsky.app/profile/silverdubloons.bsky.social");
	}
	
	public void TwitterClicked()
	{
		Application.OpenURL("https://twitter.com/SilverDubloons");
	}
	
	public void KoFiClicked()
	{
		Application.OpenURL("https://ko-fi.com/silverdubloons");
	}
	
	public void DiscordClicked()
	{
		Application.OpenURL("https://discord.gg/TdJJBgbWTf");
	}
	
	public string ConvertFloatToString(float f)
	{
		string prefix = "";
		if(f < 0)
		{
			prefix = "-";
		}
		f = Mathf.Abs(f);
		string suffix = "";
		string formattedNumber = "";
		if(f >= 1000000000000000)
		{
			suffix = "e" + (Mathf.Floor(Mathf.Log10(f) / 3) * 3).ToString();
			float exponentNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3));
			if(exponentNumber > 100)
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0");
			}
			else if(exponentNumber > 10)
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0.#");
			}
			else
			{
				formattedNumber = (f / Mathf.Pow(10, Mathf.Floor(Mathf.Log10(f) / 3) * 3)).ToString("0.##");
			}
		}
		else if(f >= 100000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0");
			suffix = "T";
		}
		else if(f >= 10000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0.#");
			suffix = "T";
		}
		else if(f >= 1000000000000)
		{
			formattedNumber = (f/1000000000000f).ToString("0.##");
			suffix = "T";
		}
		else if(f >= 100000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0");
			suffix = "B";
		}
		else if(f >= 10000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0.#");
			suffix = "B";
		}
		else if(f >= 1000000000)
		{
			formattedNumber = (f/1000000000f).ToString("0.##");
			suffix = "B";
		}
		else if(f >= 100000000)
		{
			formattedNumber = (f/1000000f).ToString("0");
			suffix = "M";
		}
		else if(f >= 10000000)
		{
			formattedNumber = (f/1000000f).ToString("0.#");
			suffix = "M";
		}
		else if(f >= 1000000)
		{
			formattedNumber = (f/1000000f).ToString("0.##");
			suffix = "M";
		}
		else if(f >= 100000)
		{
			formattedNumber = (f/1000f).ToString("0");
			suffix = "K";
		}
		else if(f > 100)
		{
			formattedNumber = f.ToString("0");
		}
		else if(f > 10)
		{
			formattedNumber = f.ToString("0.#");
		}
		else if(f > 1)
		{
			formattedNumber = f.ToString("0.##");
		}
		else
		{
			formattedNumber = f.ToString("0.###");
		}
		return prefix + formattedNumber + "" + suffix; // at < e100 longest string output is 7 digits ex 3.14e18
	}
	
	public string ConvertDoubleToString(double d)
	{
		string prefix = "";
		if(d < 0)
		{
			prefix = "-";
		}
		d = Math.Abs(d);
		string suffix = "";
		string formattedNumber = "";
		if(d >= 1000000000000000)
		{
			suffix = "e" + (Math.Floor(Math.Log10(d) / 3) * 3).ToString();
			double exponentNumber = (d / Math.Pow(10, Math.Floor(Math.Log10(d) / 3) * 3));
			if(exponentNumber > 100)
			{
				formattedNumber = (d / Math.Pow(10, Math.Floor(Math.Log10(d) / 3) * 3)).ToString("0");
			}
			else if(exponentNumber > 10)
			{
				formattedNumber = (d / Math.Pow(10, Math.Floor(Math.Log10(d) / 3) * 3)).ToString("0.#");
			}
			else
			{
				formattedNumber = (d / Math.Pow(10, Math.Floor(Math.Log10(d) / 3) * 3)).ToString("0.##");
			}
		}
		else if(d >= 100000000000000)
		{
			formattedNumber = (d/1000000000000d).ToString("0");
			suffix = "T";
		}
		else if(d >= 10000000000000)
		{
			formattedNumber = (d/1000000000000d).ToString("0.#");
			suffix = "T";
		}
		else if(d >= 1000000000000)
		{
			formattedNumber = (d/1000000000000d).ToString("0.##");
			suffix = "T";
		}
		else if(d >= 100000000000)
		{
			formattedNumber = (d/1000000000d).ToString("0");
			suffix = "B";
		}
		else if(d >= 10000000000)
		{
			formattedNumber = (d/1000000000d).ToString("0.#");
			suffix = "B";
		}
		else if(d >= 1000000000)
		{
			formattedNumber = (d/1000000000d).ToString("0.##");
			suffix = "B";
		}
		else if(d >= 100000000)
		{
			formattedNumber = (d/1000000d).ToString("0");
			suffix = "M";
		}
		else if(d >= 10000000)
		{
			formattedNumber = (d/1000000d).ToString("0.#");
			suffix = "M";
		}
		else if(d >= 1000000)
		{
			formattedNumber = (d/1000000d).ToString("0.##");
			suffix = "M";
		}
		else if(d >= 100000)
		{
			formattedNumber = (d/1000d).ToString("0");
			suffix = "K";
		}
		else if(d > 100)
		{
			formattedNumber = d.ToString("0");
		}
		else if(d > 10)
		{
			formattedNumber = d.ToString("0.#");
		}
		else if(d > 1)
		{
			formattedNumber = d.ToString("0.##");
		}
		else
		{
			formattedNumber = d.ToString("0.###");
		}
		return prefix + formattedNumber + "" + suffix; // at < e100 longest string output is 7 digits ex 3.14e18
	}
	
	public string ConvertRankAndSuitToString(int rank, int suit)
	{
		string cardString = string.Empty;
		switch(rank)
		{
			case 0:
				cardString += "2";
				break;
			case 1:
				cardString += "3";
				break;
			case 2:
				cardString += "4";
				break;
			case 3:
				cardString += "5";
				break;
			case 4:
				cardString += "6";
				break;
			case 5:
				cardString += "7";
				break;
			case 6:
				cardString += "8";
				break;
			case 7:
				cardString += "9";
				break;
			case 8:
				cardString += "T";
				break;
			case 9:
				cardString += "J";
				break;
			case 10:
				cardString += "Q";
				break;
			case 11:
				cardString += "K";
				break;
			case 12:
				cardString += "A";
				break;
			default:
				cardString += "?";
				break;
		}
		switch(suit)
		{
			case 0:
				cardString += "s";
				break;
			case 1:
				cardString += "c";
				break;
			case 2:
				cardString += "h";
				break;
			case 3:
				cardString += "d";
				break;
			case 4:
				cardString += "r";
				break;
			default:
				cardString += "?";
				break;
		}
		return cardString;
	}
	// LocalInterface.instance.DisplayError("");
	public void DisplayError(string errorMessage)
	{
		Debug.LogError(errorMessage);
		OptionsDialog.instance.SetupDialog(errorMessage, new string[1]{"Drat"}, new ThemeManager.UIElementType[1]{ThemeManager.UIElementType.standardButtonActive}, new UnityAction[1]{OptionsDialog.instance.CancelClicked}, false);
		string errorFileText = GetFileText(errorLogFileName);
		if(errorFileText == null)
		{
			errorFileText = $"{DateTime.Now.ToString()}:{errorMessage}";
		}
		else
		{
			errorFileText += $"\n{DateTime.Now.ToString()}:{errorMessage}";
		}
		SetFileText(errorLogFileName, errorFileText);
	}
	
	public Rect GetGameViewportRect()
	{
		float targetAspect = 16f / 9f;
		float windowAspect = (float)Screen.width / Screen.height;
		float scaleHeight = windowAspect / targetAspect;
		Rect viewportRect = new Rect();

		if (scaleHeight < 1.0f)
		{
			// Window is taller than 16:9: We have horizontal bars
			float barHeight = (Screen.height - (Screen.height * scaleHeight)) / 2.0f;
			viewportRect = new Rect(0, barHeight, Screen.width, Screen.height * scaleHeight);
		}
		else
		{
			// Window is wider than 16:9: We have vertical bars (or none in windowed mode)
			float scaleWidth = 1.0f / scaleHeight;
			float barWidth = (Screen.width - (Screen.width * scaleWidth)) / 2.0f;
			viewportRect = new Rect(barWidth, 0, Screen.width * scaleWidth, Screen.height);
		}

		return viewportRect;
	}
	
	public Vector2 GetMousePosition()
	{
		Rect gameViewport = GetGameViewportRect();
		if (!gameViewport.Contains(Input.mousePosition))
		{
			return new Vector2(-9001, -9001);
		}
		float normalizedX = (Input.mousePosition.x - gameViewport.x) / gameViewport.width;
		float normalizedY = (Input.mousePosition.y - gameViewport.y) / gameViewport.height;
		Vector2 mousePosInCanvas = new Vector2(
			normalizedX * referenceResolution.x - referenceResolution.x / 2,
			normalizedY * referenceResolution.y - referenceResolution.y / 2);

		return mousePosInCanvas;
		// Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * referenceResolution.x - referenceResolution.x / 2,((Input.mousePosition.y / Screen.height)) * referenceResolution.y - referenceResolution.y / 2);
		// return mousePos;
	}
	
	public Vector2 GetNormalizedMousePosition() // from -1 to 1
	{
		Vector2 mousePos = GetMousePosition();
		mousePos.x = mousePos.x / (referenceResolution.x / 2f);
		mousePos.y = mousePos.y / (referenceResolution.y / 2f);
		return mousePos;
	}
	
	public string[] GetFileTextLines(string localFilePath)
	{
		string filePath = $"{localFilesDirectory}{localFilePath}.txt";
		// Debug.Log($"getting file text lines at {filePath}");
		if(File.Exists(filePath))
		{
			using(StreamReader reader = new StreamReader(filePath))
			{
				string fileData = reader.ReadToEnd();
				string[] lines = fileData.Split('\n');
				return lines;
			}
		}
		return null;
	}
	
	public string GetFileText(string localFilePath)
	{
		// Debug.Log(localFilesDirectory);
		string filePath = $"{localFilesDirectory}{localFilePath}.txt";
		// Debug.Log($"GetFileText localFilePath={localFilePath}, filePath={filePath}");
		if(File.Exists(filePath))
		{
			using(StreamReader reader = new StreamReader(filePath))
			{
				string fileData = reader.ReadToEnd();
				return fileData;
			}
		}
		return null;
	}
	
	public void SetFileText(string localFilePath, string content, bool trim = true)
	{
		string filePath = $"{localFilesDirectory}{localFilePath}.txt";
		if(trim)
		{
			File.WriteAllText(filePath, content.Trim());
		}
		else
		{
			File.WriteAllText(filePath, content);
		}
		FileUpdated();
	}
	
	public void DeleteFile(string localFilePath)
	{
		string filePath = $"{localFilesDirectory}{localFilePath}.txt";
		if(File.Exists(filePath))
		{
			File.Delete(filePath);
		}
		else
		{
			DisplayError($"DeleteFile called with localFilePath={localFilePath} and file was not found");
		}
	}
	
	public bool DoesFileExist(string localFilePath)
	{
		string filePath = $"{localFilesDirectory}{localFilePath}.txt";
		if(File.Exists(filePath))
		{
			return true;
		}
		return false;
	}
	
	public bool Approximately(double a, double b)
	{
		if(Math.Abs(a - b) < epsilon)
		{
			return true;
		}
		return false;
	}
	
	public Color ParseColor(string colorString)
	{
		try
		{
			string cleaned = colorString.Substring(5, colorString.Length - 6);
			string[] components = cleaned.Split(',');
			
			float r = float.Parse(components[0].Trim());
			float g = float.Parse(components[1].Trim());
			float b = float.Parse(components[2].Trim());
			float a = float.Parse(components[3].Trim());
			
			return new Color(r, g, b, a);
		}
		catch (Exception exception)
		{
			LocalInterface.instance.DisplayError($"Error parsing color string '{colorString}': {exception.Message}");
			return Color.magenta;
		}
	}
	
	public int ConvertRankCharToInt(char rankChar)
	{
		switch(rankChar)
		{
			case '2':
				return 0;
			case '3':
				return 1;
			case '4':
				return 2;
			case '5':
				return 3;
			case '6':
				return 4;
			case '7':
				return 5;
			case '8':
				return 6;
			case '9':
				return 7;
			case 'T':
			case 't':
				return 8;
			case 'J':
			case 'j':
				return 9;
			case 'Q':
			case 'q':
				return 10;
			case 'K':
			case 'k':
				return 11;
			case 'A':
			case 'a':
				return 12;
		}
		DisplayError($"ConvertRankCharToInt called with rankChar={rankChar} and no matching case was found");
		return -1;
	}
	
	public int ConvertSuitCharToInt(char suitChar)
	{
		switch(suitChar)
		{// 0 = spade, 1 = club, 2 = heart, 3 = diamond, 4 = rainbow
			case 'S':
			case 's':
				return 0;
			case 'C':
			case 'c':
				return 1;
			case 'H':
			case 'h':
				return 2;
			case 'D':
			case 'd':
				return 3;
			case 'R':
			case 'r':
				return 4;
		}
		DisplayError($"ConvertSuitCharToInt called with suitChar={suitChar} and no matching case was found");
		return -1;
	}
	
	public string ColorToHexadecimal(Color color)
	{
		byte r = (byte)(Mathf.Clamp01(color.r) * 255);
		byte g = (byte)(Mathf.Clamp01(color.g) * 255);
		byte b = (byte)(Mathf.Clamp01(color.b) * 255);
		byte a = (byte)(Mathf.Clamp01(color.a) * 255);
		if(a == 255)
		{
			return $"<color=#{r:X2}{g:X2}{b:X2}>";
		}
		else
		{
			return $"<color=#{r:X2}{g:X2}{b:X2}{a:X2}>";
		}
	}
	
	public String GetZodiacBaubleStringFromHandTier(int handTier)
	{
		string baubleString = string.Empty;
		if(handTier < 10)
		{
			baubleString = $"Hand0{handTier.ToString()}Power";
		}
		else
		{
			baubleString = $"Hand{handTier.ToString()}Power";
		}
		return baubleString;
	}
	
	public String GetMultiplierBaubleStringFromHandTier(int handTier)
	{
		string baubleString = string.Empty;
		if(handTier < 10)
		{
			baubleString = $"Hand0{handTier.ToString()}Mult";
		}
		else
		{
			baubleString = $"Hand{handTier.ToString()}Mult";
		}
		return baubleString;
	}
	
	public int GetHandTierFromZodiacTag(string zodiacTag)
	{
		string intString = zodiacTag.Substring(4, 2);
		return int.Parse(intString);
	}
	
/* 	public Vector2 GetCanvasPositionOfRectTransform(RectTransform rectTransform)
	{
		Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
		Vector2 normalizedPosition = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);
		Vector2 canvasPosition = new Vector2((normalizedPosition.x - 0.5f) * referenceResolution.x, (normalizedPosition.y - 0.5f) * referenceResolution.y);
		// Debug.Log($"rectTransform.name={rectTransform.name}, screenPosition={screenPosition}, normalizedPosition={normalizedPosition}, canvasPosition={canvasPosition}");
		return canvasPosition;
	} */
	
	public Vector2 GetCanvasPositionOfRectTransform(RectTransform rectTransform, Canvas canvas)
	{
		// Get world corners of the rect
		Vector3[] worldCorners = new Vector3[4];
		rectTransform.GetWorldCorners(worldCorners);

		// Average the corners to find the world center
		Vector3 worldCenter = (worldCorners[0] + worldCorners[2]) * 0.5f;

		// Convert world position to screen point
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCenter);

		// Convert screen point to local point in the canvas
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvas.worldCamera, out localPoint);

		return localPoint;
	}
	
	public static double DoubleLerp(double a, double b, double t)
	{
		return a + (b - a) * Math.Clamp(t, 0, 1);
	}
	
	public void PopulateUnlockedBaubles()
	{
		unlockedBaubles.Clear();
		string[] unlockedBaublesLines = GetFileTextLines(unlockedBaublesFileName);
		if(unlockedBaublesLines != null)
		{
			if(unlockedBaublesLines[0].Trim() != unlockedBaublesFileVersion)
			{
				DisplayError($"Version mismatch in unlocked Baubles file. File version is {unlockedBaublesLines[0].Trim()}, current version is {unlockedBaublesFileVersion}");
				return;
			}
			if(unlockedBaublesLines.Length > 1)
			{
				for(int i = 1; i < unlockedBaublesLines.Length; i++)
				{
					unlockedBaubles.Add(unlockedBaublesLines[i].Trim());
				}
			}
		}
		else
		{
			string unlockedBaublesFileText = unlockedBaublesFileVersion;
			LocalInterface.instance.SetFileText(unlockedBaublesFileName, unlockedBaublesFileVersion);
		}
	}
	
	public bool IsBaubleUnlocked(string tag)
	{
		if(unlockedBaubles.Contains(tag))
		{
			return true;
		}
		return false;
	}
	
	public void UnlockBauble(string tag)
	{
		if(unlockedBaubles.Contains(tag))
		{
			return;
		}
		unlockedBaubles.Add(tag);
		string unlockedBaublesFileText = GetFileText(unlockedBaublesFileName);
		unlockedBaublesFileText = $"{unlockedBaublesFileText}\n{tag}";
		LocalInterface.instance.SetFileText(unlockedBaublesFileName, unlockedBaublesFileText);
	}
	
	public void PopulateUnlockedSpecialCards()
	{
		unlockedSpecialCards.Clear();
		string[] unlockedSpecialCardLines = LocalInterface.instance.GetFileTextLines(unlockedSpecialCardsFileName);
		if(unlockedSpecialCardLines != null)
		{
			if(unlockedSpecialCardLines[0].Trim() != unlockedSpecialCardsFileVersion)
			{
				DisplayError($"Version mismatch in unlocked Special Cards file. File version is {unlockedSpecialCardLines[0].Trim()}, current version is {unlockedSpecialCardsFileVersion}");
				return;
			}
			if(unlockedSpecialCardLines.Length > 1)
			{
				for(int i = 1; i < unlockedSpecialCardLines.Length; i++)
				{
					unlockedSpecialCards.Add(unlockedSpecialCardLines[i].Trim());
				}
			}
		}
		else
		{
			string unlockedSpecialCardsFileText = unlockedSpecialCardsFileVersion;
			LocalInterface.instance.SetFileText(unlockedSpecialCardsFileName, unlockedSpecialCardsFileVersion);
		}
	}
	
	public void UnlockSpecialCard(string tag)
	{
		if(unlockedSpecialCards.Contains(tag))
		{
			return;
		}
		unlockedSpecialCards.Add(tag);
		string unlockedSpecialCardsFileText = GetFileText(unlockedSpecialCardsFileName);
		unlockedSpecialCardsFileText = $"{unlockedSpecialCardsFileText}\n{tag}";
		LocalInterface.instance.SetFileText(unlockedSpecialCardsFileName, unlockedSpecialCardsFileText);
		ItemEarnedNotifications.instance.Notify("Special Card unlocked!", V.i.v.variantSpecialCards[tag].howToUnlock, "UnlockedSpecialCard", tag, V.i.v.variantSpecialCards[tag].sprite);
	}
	
	public bool IsSpecialCardUnlocked(string tag)
	{
		if(unlockedSpecialCards.Contains(tag))
		{
			return true;
		}
		return false;
	}
	
	public int[] GetRandomizedArrayOfInts(int lengthOfArray, int maxInt, int rngToUse = 0) // 0 = unity, 1 = hands // 2 = misc
	{
		int[] randomizedArray = new int[maxInt];
		for(int i = 0; i < randomizedArray.Length; i++)
		{
			randomizedArray[i] = i;
		}
		for(int  i = 0; i < randomizedArray.Length; i++)
		{
			int r = -1;
			switch(rngToUse)
			{
				case 0:
					r = UnityEngine.Random.Range(0, i + 1);
				break;
				case 1:
					r = RNG.instance.hands.Range(0, i + 1);
				break;
				case 2:
					r = RNG.instance.misc.Range(0, i + 1);
				break;
				case 3:
					r = RNG.instance.starting.Range(0, i + 1);
				break;
			}
			int tempInt = randomizedArray[i];
			randomizedArray[i] = randomizedArray[r];
			randomizedArray[r] = tempInt;
		}
		int[] arrayToReturn = new int[lengthOfArray];
		for(int i = 0; i < arrayToReturn.Length; i++)
		{
			arrayToReturn[i] = randomizedArray[i];
		}
		return arrayToReturn;
	}
	
	public Vector2 GetRandomPointWithinRadius(Vector2 centerPoint, float radius)
	{
		float randomRadius = UnityEngine.Random.Range(0, radius);
		float randomAngle = UnityEngine.Random.Range(0, 360f);
		float radian = randomAngle * Mathf.Deg2Rad;
		float xPos = randomRadius * Mathf.Cos(radian);
		float yPos = randomRadius * Mathf.Sin(radian);
		return new Vector2(centerPoint.x + xPos, centerPoint.y + yPos);
	}
	
	public Vector2 GetRandomPointOnCircle(Vector2 centerPoint, float radius, float minAngle = 0, float maxAngle = 360f)
	{
		float randomAngle = UnityEngine.Random.Range(minAngle, maxAngle);
		float radian = randomAngle * Mathf.Deg2Rad;
		float xPos = radius * Mathf.Cos(radian);
		float yPos = radius * Mathf.Sin(radian);
		return new Vector2(centerPoint.x + xPos, centerPoint.y + yPos);
	}
	
	public string ResolveSpecialCardDescription(string descriptionTag)
	{
		switch(descriptionTag)
		{
			case "IncreaseValueSpecialCardImpact":
				switch (GetCurrentSceneName())
				{
					case "GameplayScene":
						return ConvertDoubleToString(V.i.v.variantSpecialCards["IncreaseValue"].impact);
					case "MainMenuScene":
						return SpecialCardsVariantMenu.instance.specialCardVariantOptionsDictionary["IncreaseValue"].impactInputField.text;
				}
			break;
			case "IncreaseMultSpecialCardImpact":
				switch (GetCurrentSceneName())
				{
					case "GameplayScene":
						return ConvertDoubleToString(V.i.v.variantSpecialCards["IncreaseMult"].impact);
					case "MainMenuScene":
						return SpecialCardsVariantMenu.instance.specialCardVariantOptionsDictionary["IncreaseMult"].impactInputField.text;
				}
			break;
			case "MultiplyPointsAndMultSpecialCardImpact":
				switch (GetCurrentSceneName())
				{
					case "GameplayScene":
						return ConvertDoubleToString(V.i.v.variantSpecialCards["DoubleValueAndMult"].impact);
					case "MainMenuScene":
						return SpecialCardsVariantMenu.instance.specialCardVariantOptionsDictionary["DoubleValueAndMult"].impactInputField.text;
				}
			break;
			case "PromotionImpact":
				switch (GetCurrentSceneName())
				{
					case "GameplayScene":
						return ConvertDoubleToString(V.i.v.variantSpecialCards["Promotion"].impact);
					case "MainMenuScene":
						return SpecialCardsVariantMenu.instance.specialCardVariantOptionsDictionary["Promotion"].impactInputField.text;
				}
			break;
			case "DemotionImpact":
				switch (GetCurrentSceneName())
				{
					case "GameplayScene":
						return ConvertDoubleToString(V.i.v.variantSpecialCards["Demotion"].impact);
					case "MainMenuScene":
						return SpecialCardsVariantMenu.instance.specialCardVariantOptionsDictionary["Demotion"].impactInputField.text;
				}
			break;
		}
		return "[ResolveSpecialCardDescription failed]";
	}
	
	public IEnumerator VariantExplainerSetupCoroutine()
	{
		variantExplainer.rt.anchoredPosition = new Vector2(0, 360);
		yield return null;
		variantExplainer.DisplayVariant(baseVariant.ConvertToText());
		yield return null;
		variantExplainer.rt.anchoredPosition = Vector2.zero;
		variantExplainer.Close();
	}
	
	public void SceneChanged(string newSceneName, bool firstSceneChange = false)
	{
		if(GetCurrentSceneName() == "MainMenuScene")
		{
			deckPicker = GameObject.FindWithTag("DeckPicker").GetComponent<DeckPicker>();
			variantsMenu = GameObject.FindWithTag("VariantsMenu").GetComponent<VariantsMenu>();
			movingObjects = GameObject.FindWithTag("MovingObjects").GetComponent<MovingObjects>();
			difficultySelector = GameObject.FindWithTag("DifficultySelector").GetComponent<DifficultySelector>();
			unlocksMenu = GameObject.FindWithTag("UnlocksMenu").GetComponent<UnlocksMenu>();
			dailyMenu = GameObject.FindWithTag("DailyMenu").GetComponent<DailyMenu>();
			statsMenu = GameObject.FindWithTag("StatsMenu").GetComponent<StatsMenu>();
			mainMenuCanvas = GameObject.FindWithTag("MainMenuCanvas").GetComponent<Canvas>();
			mainMenu = GameObject.FindWithTag("MainMenu").GetComponent<MainMenu>();
			
			movingObjects.SetupInstance();
			difficultySelector.SetupInstance();
			RedefineSuitOrders();
			
			dailyMenu.SetupInstance();
			variantsMenu.SetupInstance(firstSceneChange);
			statsMenu.SetupInstance();
			
			deckPicker.SetupInstance();
			// deckPicker.SetupDecksDictionary();
			// deckPicker.LoadDecks();
			// deckPicker.SetupSelectableDecks();
			deckPicker.StartSettingUpDecks();
			PopulateUnlockedBaubles();
			PopulateUnlockedSpecialCards();
			variantsMenu.LoadBaseVariant();
			
			difficultySelector.SceneLoadSetup();
			
			
			// runStatsPanel.SetVisibility(false);
			unlocksMenu.SetupUnlocksMenu();
			
			StartCoroutine(VariantExplainerSetupCoroutine());
			dailyMenu.SetupDailyMenu();
			statsMenu.SetupStatsMenu();
			
			if(preferences.lastSelectedVariant == string.Empty)
			{
				variantsMenu.SetupMenuForVariant(variantsMenu.baseVariant);
			}
			else
			{
				string lastSelectedVariantString = VariantsMenu.instance.GetVariantFile(preferences.lastSelectedVariant);
				if(lastSelectedVariantString == null)
				{
					variantsMenu.SetupMenuForVariant(variantsMenu.baseVariant);
				}
				else
				{
					variantsMenu.SetupMenuForVariant(new Variant(lastSelectedVariantString));
				}
			}
			variantsMenu.roundsVariantMenu.SetupDifficulties();
			mainMenu.SetupInstance();
		}
		Preferences.instance.SceneChanged(newSceneName);
		persistentCanvas.worldCamera = Camera.main;
		Preferences.instance.UpdateTheme();
		EventSystem.current.pixelDragThreshold = Preferences.instance.dragThreshold;
    }

	public static string GetTimeDifferenceString(DateTime pastTime)
	{
		TimeSpan difference = DateTime.Now - pastTime;
		
		if (difference.TotalSeconds < 60)
		{
			int seconds = (int)difference.TotalSeconds;
			return $"{seconds} second{(seconds != 1 ? "s" : "")}";
		}
		else if (difference.TotalMinutes < 60)
		{
			int minutes = (int)difference.TotalMinutes;
			return $"{minutes} minute{(minutes != 1 ? "s" : "")}";
		}
		else if (difference.TotalHours < 24)
		{
			int hours = (int)difference.TotalHours;
			return $"{hours} hour{(hours != 1 ? "s" : "")}";
		}
		else
		{
			int days = (int)difference.TotalDays;
			return $"{days} day{(days != 1 ? "s" : "")}";
		}
	}
	
	public static bool LogApproximatelyEqual(double a, double b, double epsilon = 1e-5)
	{
		if (a == b) return true;
		double logA = Math.Log(Math.Abs(a));
		double logB = Math.Log(Math.Abs(b));
		return Math.Abs(logA - logB) < epsilon;
	}
	
	public static bool BaubleNameIsZodiac(string input)
    {
        Regex regex = new Regex(@"^Hand[0-9]{2}Power$");
        return regex.IsMatch(input);
    }
	
	public int BaubleNameIsHandMult(string input)
    {
        if (input.Length != 10)
        {
            return -1;
        }
        if (!input.StartsWith("Hand"))
        {
            return -1;
        }

        if (!input.EndsWith("Mult"))
        {
            return -1;
        }
        string numberPart = input.Substring(4, 2);
        if(int.TryParse(numberPart, out int result))
        {
            if (result >= 0 && result <= 99)
            {
                return result;
            }
        }
        return -1;
    }
	
	public void MoveSaveGameToFinishedGame(bool daily, bool custom, DateTime startTime)
	{
		string sourceFilePath = string.Empty;
		string destinationFilePath = string.Empty;
		if(daily)
		{
			sourceFilePath = $"{localFilesDirectory}{dailyGameSaveFileName}.txt";
			destinationFilePath = $"{localFilesDirectory}{completedDailyGamesFolderName}/";
		}
		else if (custom)
		{
			sourceFilePath = $"{localFilesDirectory}{customGameSaveFileName}.txt";
			destinationFilePath = $"{localFilesDirectory}{completedCustomGamesFolderName}/";
		}
		else
		{
			sourceFilePath = $"{localFilesDirectory}{standardGameSaveFileName}.txt";
			destinationFilePath = $"{localFilesDirectory}{completedStandardGamesFolderName}/";
		}
		string destinationFileName = $"{startTime.ToString("s_m_d_M_yyyy")}.txt";
		destinationFilePath += destinationFileName;
		if(!File.Exists(sourceFilePath))
		{
			DisplayError($"Unable to move saved game to long term storage, saved game {sourceFilePath} not found");
			return;
		}
		if(File.Exists(destinationFilePath))
		{
			DisplayError($"Unable to move saved game to long term storage, there is already a file called {destinationFilePath}");
			return;
		}
		File.Move(sourceFilePath, destinationFilePath);
	}
	
	public DateTime GetDateTimeStartedFromSaveFile(string saveFileName)
	{
		
		string[] fileLines = GetFileTextLines($"{saveFileName}");
		for(int i = 0; i < fileLines.Length; i++)
		{
			string[] fileLineInfo = fileLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(fileLineInfo.Length > 1)
			{
				if(fileLineInfo[0] == "dateTimeStarted")
				{
					return DateTime.Parse(fileLineInfo[1]);
				}
			}
		}
		DisplayError("GetDateTimeStartedFromSaveFile called and time started could not be found. Filename={saveFileName}");
		return DateTime.Now;
	}
	
	public DateTime GetDateTimeStartedFromVariantString(string variantString)
	{
		
		string[] variantLines = variantString.Split('\n');
		for(int i = 0; i < variantLines.Length; i++)
		{
			string[] variantLineInfo = variantLines[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
			if(variantLineInfo.Length > 1)
			{
				if(variantLineInfo[0] == "dateTimeStarted")
				{
					return DateTime.Parse(variantLineInfo[1]);
				}
			}
		}
		DisplayError("GetDateTimeStartedFromVariantString called and time started could not be found");
		return DateTime.Now;
	}
	
	public Vector2Int ParseCardString(string cardString) // returns rank, suit
	{
		if(cardString.Length != 2)
		{
			return new Vector2Int(-1, -1);
		}
		int rankInt = ConvertRankCharToInt(cardString[0]);
		int suitInt = ConvertSuitCharToInt(cardString[1]);
		return new Vector2Int(rankInt, suitInt);
	}
	
	public int GetNumberOfMatchingBoolsInArray(bool[] boolArray, bool boolCompare)
	{
		int matchingBools = 0;
		for(int i = 0; i < boolArray.Length; i++)
		{
			if(boolArray[i] == boolCompare)
			{
				matchingBools++;
			}
		}
		return matchingBools;
	}
}