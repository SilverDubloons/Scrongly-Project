using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MovingObjects : MonoBehaviour
{
    public MovingObject[] movingObjects;
	public Dictionary<string, MovingObject> mo = new Dictionary<string, MovingObject>();
	
	public static MovingObjects instance;
	
	public void SetupInstance()
	{
		instance = this;
		SetupMovingObjects();
	}
	
	private void SetupMovingObjects()
	{
		for(int i = 0; i < movingObjects.Length; i++)
		{
			mo.Add(movingObjects[i].referenceName, movingObjects[i]);
			movingObjects[i].SetupLocationsDictionary();
		}
		Scene currentScene = SceneManager.GetActiveScene();
		string sceneName = currentScene.name;
		if(sceneName == "MainMenuScene")
		{
			mo["MainMenu"].TeleportTo("OffScreen");
			mo["MainMenu"].StartMove("OnScreen");
			mo["PlayMenu"].TeleportTo("OffScreen");
			mo["Version"].TeleportTo("OffScreen");
			mo["Version"].StartMove("OnScreen");
			mo["Title"].TeleportTo("OffScreen");
			mo["Title"].StartMove("OnScreen");
			mo["ExitButton"].TeleportTo("OffScreen");
			mo["ExitButton"].StartMove("OnScreen");
			mo["DeckPicker"].TeleportTo("OffScreen");
			mo["SelfPromotion"].TeleportTo("OffScreen");
			mo["VariantsMenu"].TeleportTo("OffScreen");
			mo["BaubleVariantsMenu"].TeleportTo("OffScreen");
			mo["SelfPromotion"].StartMove("OnScreen");
			mo["SpecialOptionsVariantMenu"].TeleportTo("OffScreen");
			mo["DeckVariantMenu"].TeleportTo("OffScreen");
			mo["RoundsVariantMenu"].TeleportTo("OffScreen");
			mo["SpecialCardsVariantMenu"].TeleportTo("OffScreen");
			mo["ZodiacsVariantMenu"].TeleportTo("OffScreen");
			mo["VariantDetailsInput"].TeleportTo("OffScreen");
			mo["SpritePicker"].TeleportTo("OffScreen");
			mo["ColorPicker"].TeleportTo("OffScreen");
			mo["LoadVariantMenu"].TeleportTo("OffScreen");
			mo["ImportStringDialog"].TeleportTo("OffScreen");
			mo["SeedInput"].TeleportTo("OffScreen");
			mo["DifficultySelector"].TeleportTo("OffScreen");
			mo["UnlocksMenu"].TeleportTo("OffScreen");
			mo["DailyMenu"].TeleportTo("OffScreen");
			mo["StatsMenu"].TeleportTo("OffScreen");
			mo["ExportStringDialog"].TeleportTo("OffScreen");
			/* DisableVisibilityOfChildren(MovingObjects.instance.mo["PlayMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["DeckPicker"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["DifficultySelector"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["DailyMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["UnlocksMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["StatsMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["VariantsMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["SeedInput"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["BaubleVariantsMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["SpecialOptionsVariantMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["DeckVariantMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["RoundsVariantMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["SpecialCardsVariantMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["ZodiacsVariantMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["LoadVariantMenu"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["VariantDetailsInput"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["ImportStringDialog"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["SpritePicker"].rt);
			DisableVisibilityOfChildren(MovingObjects.instance.mo["ColorPicker"].rt); */
		}
		if(sceneName == "GameplayScene")
		{
			
		}
	}
}
