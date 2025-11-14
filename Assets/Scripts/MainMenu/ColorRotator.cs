using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorRotator : MonoBehaviour
{
    private float t = 0;
	public float switchTime;
	public Image imageToChange;
	private int stage = 0;
	
	void Update()
	{
		t += Time.deltaTime;
		Color imageColor = imageToChange.color;
		if(t > switchTime)
		{
			t = 0;
			stage++;
			if(stage > 5)
			{
				stage = 0;
			}
		}
		switch(stage)
		{
			case 0:
			imageColor.g += Time.deltaTime / switchTime;
			break;
			case 1:
			imageColor.r -= Time.deltaTime / switchTime;
			break;
			case 2:
			imageColor.b += Time.deltaTime / switchTime;
			break;
			case 3:
			imageColor.g -= Time.deltaTime / switchTime;
			break;
			case 4:
			imageColor.r += Time.deltaTime / switchTime;
			break;
			case 5:
			imageColor.b -= Time.deltaTime / switchTime;
			break;
		}
		imageToChange.color = imageColor;
	}
}
