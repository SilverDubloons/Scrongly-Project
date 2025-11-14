using UnityEngine;

public class ItemEarnedNotifications : MonoBehaviour
{
    public RectTransform itemEarnedNotifierParent;
	
	public GameObject itemEarnedNotifierPrefab;
	
	public static ItemEarnedNotifications instance;
	
	public void SetupInstance()
	{
		instance = this;
	}
	// ItemEarnedNotifications.instance.Notify("New deck unlocked!", decks[deckName].howToUnlock, "UnlockedDeck", deckName, decks[deckName].cardBack);
	public void Notify(string title, string description, string type, string tag, Sprite itemSprite = null)
	{
		GameObject newItemEarnedNotifierGO = Instantiate(itemEarnedNotifierPrefab, itemEarnedNotifierParent);
		ItemEarnedNotifier newItemEarnedNotifier = newItemEarnedNotifierGO.GetComponent<ItemEarnedNotifier>();
		newItemEarnedNotifier.SetupItemEarned(title, description, type, tag, itemSprite);
	}
}
