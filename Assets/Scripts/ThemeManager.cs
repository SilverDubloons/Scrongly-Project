using UnityEngine;

public class ThemeManager : MonoBehaviour
{
	public static ThemeManager instance;
	public ColorTheme[] colorThemes;
	public event System.Action OnThemeChanged;
	
	void Awake()
	{
		if(instance != null)
		{
			return;
		}
		instance = this;
	}
	
	public enum UIElementType {backdrop, darkBackdrop, standardButtonActive, altButtonActive, backButtonActive, warningButtonActive, buttonSpecialState, buttonMouseOver, buttonDisabled, variantSimpleBorder, variantSimpleInterior, scrollView, scrollbarHandle, text, shadow, inputFieldBackdrop, inputFieldText, inputFieldPlaceholder, variantMenuBackdrop, toggleCheckmark, sliderBackdrop, sliderKnob, variantObjectBackdrop, tooltipBackdrop, tooltipBorder, tooltipInterior, bubble, vialFill, chipThreshold, currentScore, deckIconLockedColor, deckIconUnlockedColor, deckIconSelectedColor, handInfoBackdropStandard, handInfoBackdropUnplayed, handInfoBackdropInHand, points, mult, itemEarnedNotifierBackdrop, minorNotification, chipParticle, runStatsPanelA, runStatsPanelB, runStatsPanelC, explainerText, statLine, playAreaMain, playAreaSpecialCards, layaway, informationTabs, individualMinimumBackdrops, deckPreview, handInfoTooltip, deckViewerMain, deckViewerMiddle, deckViewerDark, interactionBlocker, chipThresholdWarning, handInfoDefaultText, handInfoHighlightedText, handInfoContainedText, gainedValuesScorePlate, shopBuyButton, progressBarBackground, progressBarTop, VariantSimpleSelected, Common, Uncommon, Rare, Legendary, Zodiac, Special, NegativeZodiac, BaubleName, CardName, ZodiacName, DeckName}
	
	public Color GetColorFromCurrentTheme(UIElementType elementType) 
	{
		ColorTheme theme = colorThemes[Preferences.instance.currentTheme];
        return elementType switch 
		{
            UIElementType.backdrop => theme.backdrop,
            UIElementType.darkBackdrop => theme.darkBackdrop,
            UIElementType.standardButtonActive => theme.standardButtonActive,
            UIElementType.altButtonActive => theme.altButtonActive,
            UIElementType.backButtonActive => theme.backButtonActive,
            UIElementType.warningButtonActive => theme.warningButtonActive,
            UIElementType.buttonSpecialState => theme.buttonSpecialState,
            UIElementType.buttonMouseOver => theme.buttonMouseOver,
            UIElementType.buttonDisabled => theme.buttonDisabled,
            UIElementType.variantSimpleBorder => theme.variantSimpleBorder,
            UIElementType.variantSimpleInterior => theme.variantSimpleInterior,
            UIElementType.scrollView => theme.scrollView,
            UIElementType.scrollbarHandle => theme.scrollbarHandle,
            UIElementType.text => theme.text,
            UIElementType.shadow => theme.shadow,
            UIElementType.inputFieldBackdrop => theme.inputFieldBackdrop,
            UIElementType.inputFieldText => theme.inputFieldText,
            UIElementType.inputFieldPlaceholder => theme.inputFieldPlaceholder,
            UIElementType.variantMenuBackdrop => theme.variantMenuBackdrop,
            UIElementType.toggleCheckmark => theme.toggleCheckmark,
            UIElementType.sliderBackdrop => theme.sliderBackdrop,
            UIElementType.sliderKnob => theme.sliderKnob,
            UIElementType.variantObjectBackdrop => theme.variantObjectBackdrop,
            UIElementType.tooltipBackdrop => theme.tooltipBackdrop,
            UIElementType.tooltipBorder => theme.tooltipBorder,
            UIElementType.tooltipInterior => theme.tooltipInterior,
            UIElementType.bubble => theme.bubble,
            UIElementType.vialFill => theme.vialFill,
            UIElementType.chipThreshold => theme.chipThreshold,
            UIElementType.currentScore => theme.currentScore,
            UIElementType.deckIconLockedColor => theme.deckIconLockedColor,
            UIElementType.deckIconUnlockedColor => theme.deckIconUnlockedColor,
            UIElementType.deckIconSelectedColor => theme.deckIconSelectedColor,
            UIElementType.handInfoBackdropStandard => theme.handInfoBackdropStandard,
            UIElementType.handInfoBackdropUnplayed => theme.handInfoBackdropUnplayed,
            UIElementType.handInfoBackdropInHand => theme.handInfoBackdropInHand,
            UIElementType.points => theme.points,
            UIElementType.mult => theme.mult,
            UIElementType.itemEarnedNotifierBackdrop => theme.itemEarnedNotifierBackdrop,
            UIElementType.minorNotification => theme.minorNotification,
            UIElementType.chipParticle => theme.chipParticle,
            UIElementType.runStatsPanelA => theme.runStatsPanelA,
            UIElementType.runStatsPanelB => theme.runStatsPanelB,
            UIElementType.runStatsPanelC => theme.runStatsPanelC,
            UIElementType.explainerText => theme.explainerText,
            UIElementType.statLine => theme.statLine,
            UIElementType.playAreaMain => theme.playAreaMain,
            UIElementType.playAreaSpecialCards => theme.playAreaSpecialCards,
            UIElementType.layaway => theme.layaway,
            UIElementType.informationTabs => theme.informationTabs,
            UIElementType.individualMinimumBackdrops => theme.individualMinimumBackdrops,
            UIElementType.deckPreview => theme.deckPreview,
            UIElementType.handInfoTooltip => theme.handInfoTooltip,
            UIElementType.deckViewerMain => theme.deckViewerMain,
            UIElementType.deckViewerMiddle => theme.deckViewerMiddle,
            UIElementType.deckViewerDark => theme.deckViewerDark,
            UIElementType.interactionBlocker => theme.interactionBlocker,
            UIElementType.chipThresholdWarning => theme.chipThresholdWarning,
            UIElementType.handInfoDefaultText => theme.handInfoDefaultText,
            UIElementType.handInfoHighlightedText => theme.handInfoHighlightedText,
            UIElementType.handInfoContainedText => theme.handInfoContainedText,
            UIElementType.gainedValuesScorePlate => theme.gainedValuesScorePlate,
            UIElementType.shopBuyButton => theme.shopBuyButton,
            UIElementType.progressBarBackground => theme.progressBarBackground,
            UIElementType.progressBarTop => theme.progressBarTop,
            UIElementType.VariantSimpleSelected => theme.VariantSimpleSelected,
            UIElementType.Common => theme.Common,
            UIElementType.Uncommon => theme.Uncommon,
            UIElementType.Rare => theme.Rare,
            UIElementType.Legendary => theme.Legendary,
            UIElementType.Zodiac => theme.Zodiac,
            UIElementType.Special => theme.Special,
            UIElementType.NegativeZodiac => theme.NegativeZodiac,
            UIElementType.BaubleName => theme.BaubleName,
            UIElementType.CardName => theme.CardName,
            UIElementType.ZodiacName => theme.ZodiacName,
            UIElementType.DeckName => theme.DeckName,
            _ => Color.white
        };
    }
	
	public Color GetTransparentColorFromCurrentTheme(UIElementType elementType) 
	{
		Color color = GetColorFromCurrentTheme(elementType);
		color.a = 0;
		return color;
	}
	 
	[System.Serializable]
    public class ColorTheme
	{
		public Color backdrop;		
		public Color darkBackdrop;		
		public Color standardButtonActive;	// also toggle backdrop, dropdown as well
		public Color altButtonActive; // deck picker buttons
		public Color backButtonActive;
		public Color warningButtonActive;
		public Color buttonSpecialState;
		public Color buttonMouseOver;
		public Color buttonDisabled;
		public Color variantSimpleBorder;
		public Color variantSimpleInterior;
		public Color scrollView;
		public Color scrollbarHandle;
		public Color text;
		public Color shadow;
		public Color inputFieldBackdrop;
		public Color inputFieldText;
		public Color inputFieldPlaceholder;
		public Color variantMenuBackdrop; // also sprite picker
		public Color toggleCheckmark;
		public Color sliderBackdrop;
		public Color sliderKnob;
		public Color variantObjectBackdrop; // BaubleVariantOptions, bossPicker, RoundOptions, SpecialCardVariantOptions, SpecialOptionInputField
		public Color tooltipBackdrop;
		public Color tooltipBorder;
		public Color tooltipInterior;
		public Color bubble;
		public Color vialFill;
		public Color chipThreshold;
		public Color currentScore;
		public Color deckIconLockedColor;
		public Color deckIconUnlockedColor;
		public Color deckIconSelectedColor;
		public Color handInfoBackdropStandard;
		public Color handInfoBackdropUnplayed;
		public Color handInfoBackdropInHand;
		public Color points;
		public Color mult;
		public Color itemEarnedNotifierBackdrop;
		public Color minorNotification;
		public Color chipParticle;
		public Color runStatsPanelA; // hands played, cards discarded, baubles gained, zodiacs gained, cards added to deck, chips earned
		public Color runStatsPanelB; // most played hand
		public Color runStatsPanelC; // round reached, score in final hand
		public Color explainerText; // SpecialCardExplainer, cardExplainer
		public Color statLine; // in stats menu
		public Color playAreaMain;
		public Color playAreaSpecialCards;
		public Color layaway;
		public Color informationTabs;
		public Color individualMinimumBackdrops;
		public Color deckPreview;
		public Color handInfoTooltip;
		public Color deckViewerMain;
		public Color deckViewerMiddle;
		public Color deckViewerDark;
		public Color interactionBlocker; // 57 colors!
		public Color chipThresholdWarning;
		public Color defaultHandInfoText;
		public Color highligtedHandInfoText;
		public Color containedHandInfoText;
		public Color handInfoDefaultText;
		public Color handInfoHighlightedText;
		public Color handInfoContainedText;
		public Color gainedValuesScorePlate;
		public Color shopBuyButton;
		public Color progressBarBackground;
		public Color progressBarTop;
		public Color VariantSimpleSelected;
		public Color Common;
		public Color Uncommon;
		public Color Rare;
		public Color Legendary;
		public Color Zodiac;
		public Color Special;
		public Color NegativeZodiac;
		public Color BaubleName;
		public Color CardName;
		public Color ZodiacName;
		public Color DeckName; // for unlocks
	}
	
	public void ApplyTheme(int newThemeIndex)
	{
		if(newThemeIndex <= colorThemes.Length)
		{
			Preferences.instance.currentTheme = newThemeIndex;
		}
		else
		{
			Preferences.instance.currentTheme = 0;
		}
		ColorTheme newTheme = colorThemes[Preferences.instance.currentTheme];
		OnThemeChanged?.Invoke();
    }
	
/* 	void Update()
	{
		if(Input.GetKeyDown(KeyCode.M))
		{
			Preferences.instance.currentTheme++;
			if(Preferences.instance.currentTheme >= colorThemes.Length)
			{
				Preferences.instance.currentTheme = 0;
			}
			ApplyTheme(Preferences.instance.currentTheme);
		}
	} */
	
	public UIElementType GetElementTypeForRarity(string rarity)
	{
		switch(rarity)
		{
			case "Common":
				return UIElementType.Common;
			case "Uncommon":
				return UIElementType.Uncommon;
			case "Rare":
				return UIElementType.Rare;
			case "Legendary":
				return UIElementType.Legendary;
			case "Zodiac":
				return UIElementType.Zodiac;
			case "Special":
				return UIElementType.Special;
		}
		LocalInterface.instance.DisplayError($"GetElementTypeForRarity called for {rarity} which isn't accepted");
		return UIElementType.backdrop;
	}
}
