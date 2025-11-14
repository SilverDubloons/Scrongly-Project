using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DescriptionResolver
{
    private static Regex placeholderRegex = new Regex(@"\[(.*?)\]", RegexOptions.Compiled);

    private Dictionary<string, Func<string>> resolvers;

    public DescriptionResolver()
    {
        resolvers = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "CurrentHandSize", () => GameManager.instance.GetMaxHandSize().ToString() },
			{ "CardsNeededToMakeAStraight", () => HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraight().ToString() },
			{ "PointsColor", () => LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.pointsColor) },
			{ "MultColor", () => LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.multiplierColor) },
			{ "HandNameColor", () => LocalInterface.instance.ColorToHexadecimal(LocalInterface.instance.handNameColor) },
            { "CardsNeededToMakeAFlush", () => HandEvaluation.instance.GetNumberOfCardsNeededToMakeAFlush().ToString() },
            { "MaxStraightGap", () => GameManager.instance.GetMaxStraightGap().ToString() },
            { "DiscardsPerRound", () => GameManager.instance.GetDiscardsPerRound().ToString() },
            { "HandsUntilFatiguePerRound", () => GameManager.instance.GetHandsUntilFatiguePerRound().ToString() },
            { "ChipThresholdsPerRound", () => GameManager.instance.GetChipThresholdsPerRound().ToString() },
            { "MultToMonarchImpact", () => Baubles.instance.GetImpactInt("MultToMonarch", false, true).ToString() },
            { "PointsToNumberedCardsImpact", () => Baubles.instance.GetImpactInt("PointsToNumberedCards", false, true).ToString() },
            { "PointsToNumberedCardsImpactMainMenu", () => LocalInterface.instance.ConvertDoubleToString(VariantsMenu.instance.loadedVariant.variantBaubles["PointsToNumberedCards"].impact1) } ,
            { "AcesStraightsPoints", () => V.i.v.variantBaubles["AcesStraights"].impact1.ToString() },
            { "AcesStraightsMult", () => V.i.v.variantBaubles["AcesStraights"].impact2.ToString() },
            { "AcesStraightsImpact1", () => Baubles.instance.GetImpactInt("AcesStraights", false, true).ToString() },
            { "AcesStraightsImpact2", () => Baubles.instance.GetImpactInt("AcesStraights", true, true).ToString() },
            { "MultFromRainbowCardsImpact", () => Baubles.instance.GetImpactInt("MultFromRainbowCards", false, true).ToString() },
            { "MultFromKingsImpact", () => Baubles.instance.GetImpactInt("MultFromKings", false, true).ToString() },
            { "IncreaseItemsForSaleImpact", () => Shop.instance.GetNumberOfEachItemOnSale(true).ToString() },
            { "IncreaseValueSpecialCardImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("IncreaseValueSpecialCardImpact") },
            { "IncreaseMultSpecialCardImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("IncreaseMultSpecialCardImpact") },
            { "MultiplyPointsAndMultSpecialCardImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("MultiplyPointsAndMultSpecialCardImpact") },
            { "PromotionImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("PromotionImpact") },
            { "DemotionImpact", () => LocalInterface.instance.ResolveSpecialCardDescription("DemotionImpact") },
            { "CurrentRerollBaseCost", () => Shop.instance.GetCurrentRerollBaseCost().ToString() },
            { "CardsNeededToMakeAStraightFlush", () => HandEvaluation.instance.GetNumberOfCardsNeededToMakeAStraightFlush().ToString() },
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