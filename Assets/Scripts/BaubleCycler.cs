using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BaubleCycler : MonoBehaviour
{
    public Image[] images;
	public RectTransform[] rts;
	public bool addToStartingBaubleIndex;
	
	public IEnumerator cycleCoroutine;
	public bool cycling;
	
	public int currentNumberOfBaubles;
	public int currentDisplayedBauble;
	
	
	public void SetupBaubleCycler(List<string> baubleTags)
	{
		currentNumberOfBaubles = baubleTags.Count;
		for(int i = 0; i < images.Length; i++)
		{
			if(i < currentNumberOfBaubles)
			{
				images[i].gameObject.SetActive(true);
				switch(baubleTags[i])
				{
					case "Dice":
						if(Baubles.instance.GetQuantityOwned("Dice", true) <= 1)
						{
							images[i].sprite = V.i.v.variantBaubles[baubleTags[i]].sprite;
						}
						else
						{
							images[i].sprite = Baubles.instance.diceSprites[10 + Baubles.instance.GetQuantityOwned("Dice", true)];
						}
					break;
					default:
						images[i].sprite = V.i.v.variantBaubles[baubleTags[i]].sprite;
					break;
				}
			}
			else
			{
				images[i].gameObject.SetActive(false);
			}
		}
		currentDisplayedBauble = 0;
		if(addToStartingBaubleIndex)
		{
			currentDisplayedBauble += currentNumberOfBaubles / 2;
		}
		if(cycling)
		{
			StopCoroutine(cycleCoroutine);
		}
		if(currentNumberOfBaubles > 1)
		{
			cycleCoroutine = CycleImages();
			StartCoroutine(cycleCoroutine);
		}
	}
	
	public int CycleIndex(int x, int max)
	{
		while(x > max)
		{
			x = x - max - 1;
		}
		return x;
	}
	
	public IEnumerator CycleImages()
	{
		cycling = true;
		for(int i = currentDisplayedBauble; i < currentDisplayedBauble + currentNumberOfBaubles; i++)
		{
			int nextIndex = CycleIndex(i, currentNumberOfBaubles - 1);
			rts[nextIndex].anchoredPosition = new Vector2(48 * (i - currentDisplayedBauble), 0);
		}
		yield return new WaitForSeconds(5f);
		float switchTime = 1f;
		float t = 0;
		while(t < switchTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime, 0, switchTime);
			for(int i = currentDisplayedBauble; i < currentDisplayedBauble + currentNumberOfBaubles; i++)
			{
				int nextIndex = CycleIndex(i, currentNumberOfBaubles - 1);
				rts[nextIndex].anchoredPosition = Vector2.Lerp(new Vector2(48 * (i - currentDisplayedBauble), 0), new Vector2(48 * (i - currentDisplayedBauble) - 48, 0), t / switchTime);
			}
			yield return null;
		}
		currentDisplayedBauble = CycleIndex(currentDisplayedBauble + 1, currentNumberOfBaubles - 1);
		cycling = false;
		StartCoroutine(CycleImages());
	}
}
