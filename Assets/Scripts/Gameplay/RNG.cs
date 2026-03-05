using System;
using System.Globalization;
using UnityEngine;

public class RNG : MonoBehaviour
{
    public RandomNumbers shuffle;	// for shuffling deck
    public RandomNumbers shop;		// for determining items available in shop
    public RandomNumbers starting;	// for start of game, such as adding random cards to player deck from variant
    public RandomNumbers hands;		// for random happenings during hand scoring, such as which card is the sandwich monarch
	public RandomNumbers misc;		// for happenings with bosses or baubles
	
	public static RNG instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void LoadCallCountsFromString(string callCountsString)
	{
		string[] callCountsData = callCountsString.Split('%', StringSplitOptions.RemoveEmptyEntries);
		shuffle.RestoreState(V.i.seed, int.Parse(callCountsData[0], CultureInfo.InvariantCulture));
		shop.RestoreState(V.i.seed, int.Parse(callCountsData[1], CultureInfo.InvariantCulture));
		starting.RestoreState(V.i.seed, int.Parse(callCountsData[2], CultureInfo.InvariantCulture));
		hands.RestoreState(V.i.seed, int.Parse(callCountsData[3], CultureInfo.InvariantCulture));
		misc.RestoreState(V.i.seed, int.Parse(callCountsData[4], CultureInfo.InvariantCulture));
	}
}
