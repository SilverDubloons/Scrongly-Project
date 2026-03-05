using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
public class DescriptionResolver
{
    private static Regex placeholderRegex = new Regex(@"\[(.*?)\]", RegexOptions.Compiled);

    private Dictionary<string, Func<string>> resolvers;

    public DescriptionResolver()
    {
        resolvers = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "CurrentHandSize", () => GameManager.instance.GetMaxHandSize().ToString(CultureInfo.InvariantCulture) },
			{ "CardsNeededToMakeAStraight", () => HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight().ToString(CultureInfo.InvariantCulture) },
			{ "PointsColor", () => LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.pointsColor) },
			{ "MultColor", () => LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.multiplierColor) },
			{ "HandNameColor", () => LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.handNameColor) },
            { "CardsNeededToMakeAFlush", () => HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush().ToString(CultureInfo.InvariantCulture) },
            { "MaxStraightGap", () => GameManager.instance.GetMaxStraightGap().ToString(CultureInfo.InvariantCulture) },
            { "DiscardsPerRound", () => GameManager.instance.GetDiscardsPerRound().ToString(CultureInfo.InvariantCulture) },
            { "HandsUntilFatiguePerRound", () => GameManager.instance.GetHandsUntilFatiguePerRound().ToString(CultureInfo.InvariantCulture) },
            { "ChipThresholdsPerRound", () => GameManager.instance.GetChipThresholdsPerRound().ToString(CultureInfo.InvariantCulture) },
            { "MultToMonarchImpact", () => Baubles.instance.GetImpactInt("MultToMonarch", false, true).ToString(CultureInfo.InvariantCulture) },
            { "PointsToNumberedCardsImpact", () => Baubles.instance.GetImpactInt("PointsToNumberedCards", false, true).ToString(CultureInfo.InvariantCulture) },
            { "PointsToNumberedCardsImpactMainMenu", () => LocalInterface.instance.ConvertDoubleToString(VariantsMenu.instance.loadedVariant.variantBaubles["PointsToNumberedCards"].impact1) } ,
            { "AcesStraightsPoints", () => V.i.v.variantBaubles["AcesStraights"].impact1.ToString(CultureInfo.InvariantCulture) },
            { "AcesStraightsMult", () => V.i.v.variantBaubles["AcesStraights"].impact2.ToString(CultureInfo.InvariantCulture) },
            { "AcesStraightsImpact1", () => Baubles.instance.GetImpactInt("AcesStraights", false, true).ToString(CultureInfo.InvariantCulture) },
            { "AcesStraightsImpact2", () => Baubles.instance.GetImpactInt("AcesStraights", true, true).ToString(CultureInfo.InvariantCulture) },
            { "MultFromRainbowCardsImpact", () => Baubles.instance.GetImpactInt("MultFromRainbowCards", false, true).ToString(CultureInfo.InvariantCulture) },
            { "MultFromKingsImpact", () => Baubles.instance.GetImpactInt("MultFromKings", false, true).ToString(CultureInfo.InvariantCulture) },
            { "IncreaseItemsForSaleImpact", () => Shop.instance.GetNumberOfEachItemOnSale(true).ToString(CultureInfo.InvariantCulture) },
            { "IncreaseValueSpecialCardImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("IncreaseValueSpecialCardImpact") },
            { "IncreaseMultSpecialCardImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("IncreaseMultSpecialCardImpact") },
            { "MultiplyPointsAndMultSpecialCardImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("MultiplyPointsAndMultSpecialCardImpact") },
            { "PromotionImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("PromotionImpact") },
            { "DemotionImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("DemotionImpact") },
            { "CurrentRerollBaseCost", () => Shop.instance.GetCurrentRerollBaseCost().ToString(CultureInfo.InvariantCulture) },
            { "CardsNeededToMakeAStraightFlush", () => HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush().ToString(CultureInfo.InvariantCulture) },
            { "flushZodiacBaubleSuitOrders0", () => LocalInterface.instance.suitNames[GameManager.instance.flushZodiacBaubleSuitOrders[0]] },
            { "flushZodiacBaubleSuitOrders1", () => LocalInterface.instance.suitNames[GameManager.instance.flushZodiacBaubleSuitOrders[1]] },
            { "flushZodiacBaubleSuitOrders2", () => LocalInterface.instance.suitNames[GameManager.instance.flushZodiacBaubleSuitOrders[2]] }
        };
		for (int i = 0; i <= 17; i++)
		{
			string handId = $"Hand{i:D2}Mult";
			resolvers.Add($"{handId}ImpactMainMenu", () => LocalInterface.instance.ConvertDoubleToString(VariantsMenu.instance.loadedVariant.variantBaubles[handId].impact1));
			resolvers.Add($"{handId}Impact", () => LocalInterface.instance.ConvertDoubleToString(V.i.v.variantBaubles[handId].impact1));
		}
    }

    public string Resolve(string template)
    {
        return placeholderRegex.Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            if (resolvers.TryGetValue(key, out var resolver))
            {
                return resolver.Invoke();
            }
            return $"[Unknown:{key}]";
        });
    }

    public void RegisterTag(string tag, Func<string> resolver)
    {
        resolvers[tag] = resolver;
    }
}