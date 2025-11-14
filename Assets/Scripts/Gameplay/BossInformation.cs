using UnityEngine;
using static GameManager;

public class BossInformation : MonoBehaviour
{
    public Label label;
    public GameObject visualObject;
	public GameObject[] warningSymbols;
	public BaubleCycler[] baubleCyclers;
	
	public static BossInformation instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	
	public void UpdateBossInformation()
	{
		if(GameManager.instance.IsThisABossRound())
		{
			visualObject.SetActive(true);
			BossRound currentBossRound = GameManager.instance.GetCurrentBossRound();
			label.ChangeFontSize(16);
			label.ChangeText(currentBossRound.description);
			if(label.GetPreferredHeight() > 36f)
			{
				label.ChangeFontSize(8);
			}
			if(Baubles.instance.disabledBaubles.Count > 0)
			{
				for(int i = 0; i < warningSymbols.Length; i++)
				{
					warningSymbols[i].SetActive(false);
				}
				for(int i = 0; i < baubleCyclers.Length; i++)
				{
					baubleCyclers[i].gameObject.SetActive(true);
					baubleCyclers[i].SetupBaubleCycler(Baubles.instance.disabledBaubles);
				}
			}
			else
			{
				for(int i = 0; i < warningSymbols.Length; i++)
				{
					warningSymbols[i].SetActive(true);
				}
				for(int i = 0; i < baubleCyclers.Length; i++)
				{
					baubleCyclers[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			visualObject.SetActive(false);
		}
	}
}
