using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Variant;
using static GameManager;

public class GameStart : MonoBehaviour
{
    public RNG rng;
	public Deck deck;
	public PlayArea playArea;
	public HandArea handArea;
	public Baubles baubles;
	public GameManager gameManager;
	public MovingObjects movingObjects;
	public HandsInformation handsInformation;
	public BaublesInformation baublesInformation;
	public RoundsInformation roundsInformation;
	public PurchasedItems purchasedItems;
	public ScoreVial scoreVial;
	public RunInformation runInformation;
	public DeckPreview deckPreview;
	public Shop shop;
	public BackgroundManager backgroundManager;
	public BossInformation bossInformation;
	public ItemEarnedNotifications itemEarnedNotifications;
	public CardValuesTooltip cardValuesTooltip;
	public DeckViewer deckViewer;
	public RunStatsPanel runStatsPanel;
	public Tutorial tutorial;
	public SlotMachine slotMachine;
	
	void Start()
	{
		rng.SetupInstance();
		deck.SetupInstance();
		playArea.SetupInstance();
		handArea.SetupInstance();
		baubles.SetupInstance();
		gameManager.SetupInstance();
		movingObjects.SetupInstance();
		handsInformation.SetupInstance();
		baublesInformation.SetupInstance();
		roundsInformation.SetupInstance();
		purchasedItems.SetupInstance();
		scoreVial.SetupInstance();
		runInformation.SetupInstance();
		deckPreview.SetupInstance();
		shop.SetupInstance();
		backgroundManager.SetupInstance();
		bossInformation.SetupInstance();
		itemEarnedNotifications.SetupInstance();
		cardValuesTooltip.SetupInstance();
		runStatsPanel.SetupInstance();
		tutorial.SetupInstance();
		slotMachine.SetupInstance();
		
		cardValuesTooltip.HideTooltip();
		baublesInformation.baublesInformationObject.SetActive(false);
		if(V.i.currentDifficulty == 0 && !V.i.isDailyGame && !V.i.isCustomGame)
		{
			runStatsPanel.variantSimple.gameObject.SetActive(false);
		}
		else
		{
			runStatsPanel.variantSimple.UpdateVariantSimpleForVariant(V.i.v);
		}
		runStatsPanel.SetVisibility(false);
		
		
		
		if(V.i.loadingGame)
		{
			runInformation.LoadGame(V.i.loadingGameInformation);
			GameManager.instance.SetupDeckBonuses();
			Deck.instance.drawPileCardBack.sprite = V.i.chosenDeckSprite;
			Deck.instance.discardPileCardBack.sprite = V.i.chosenDeckSprite;
			bossInformation.UpdateBossInformation();
			if(GameManager.instance.IsThisABossRound())
			{
				BackgroundManager.instance.SwitchToBossBackground();
			}
		}
		else
		{
			GameManager.instance.SetupDeckBonuses();
			rng.shuffle.ChangeSeed(V.i.seed);
			rng.shop.ChangeSeed(V.i.seed);
			rng.starting.ChangeSeed(V.i.seed);
			rng.hands.ChangeSeed(V.i.seed);
			rng.misc.ChangeSeed(V.i.seed);
			GameManager.instance.flushZodiacBaubleSuitOrders = LocalInterface.instance.GetRandomizedArrayOfInts(4, 4, 3);
			// Debug.Log($"New game with seed={V.i.seed}");
			// gameManager.ResetDiscards();
			// gameManager.ResetHandsUntilFatigue();
			if(V.i.chosenDeck == "Swiney" && !V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
			{
				gameManager.SetCurrency(V.i.v.variantSpecialOptions["StartingChips"].impact + 10);
				Baubles.instance.BaublePurchased("IncreaseInterestMax", Vector2.zero, true, false);
			}
			else
			{
				gameManager.SetCurrency(V.i.v.variantSpecialOptions["StartingChips"].impact);
			}
			if(V.i.chosenDeck == "Scastronomy" && !V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
			{
				Baubles.instance.BaublePurchased("IncreasePlayableStandardCardCount", Vector2.zero, true, false);
				Baubles.instance.BaublePurchased("IncreasePlayableStandardCardCount", Vector2.zero, true, false);
			}
			if(V.i.chosenDeck == "Sgambler" && !V.i.v.variantSpecialOptions["IgnoreDeckBonus"].inEffect)
			{
				Baubles.instance.BaublePurchased("ChipMagnet", Vector2.zero, true, false);
			}
			foreach(KeyValuePair<int, VariantRound> entry in V.i.v.variantRounds)
			{
				if(entry.Value.bossType != "")
				{
					string bossTag = entry.Value.bossType;
					if(entry.Value.bossType.Substring(0, 6) == "Random")
					{
						int tier = -1;
						int lowerEnd = int.Parse(entry.Value.bossType.Substring(10, 2));
						if(entry.Value.bossType.Length >= 13)
						{
							int upperEnd = int.Parse(entry.Value.bossType.Substring(13, 2));
							tier = rng.starting.Range(lowerEnd, upperEnd + 1);
							
						}
						else
						{
							tier = lowerEnd;
						}
						int randomBoss = rng.starting.Range(0, LocalInterface.instance.bossTiers[tier].bossLevels.Count);
						bossTag = LocalInterface.instance.bossTiers[tier].bossLevels[randomBoss].tag;
					}
					string tag = LocalInterface.instance.bossLevels[bossTag].tag;
					string desc = LocalInterface.instance.bossLevels[bossTag].description;
					int random1 = -1;
					int random2 = -1;
					switch(tag)
					{
						case "SuitDestroyed":
							random1 = rng.starting.Range(0, 4);
							desc = $"{LocalInterface.instance.suitNames[random1]}s are Destroyed after Scoring";
							break;
						case "SuitPairGrantsNoValue":
							random1 = rng.starting.Range(0, 4);
							while(random2 == -1 || random2 == random1)
							{
								random2 = rng.starting.Range(0, 4);
							}
							desc = $"{LocalInterface.instance.suitNames[random1]}s and {LocalInterface.instance.suitNames[random2]}s Provide no Value or Mulitplier";
							break;
						case "HandQuartered":
							random1 = rng.starting.Range(0, 3);
							switch(random1)
							{
								case 0:
									desc = "Hands Containing Three of a Kind have their Multiplier Quartered";
									break;
								case 1:
									desc = "Hands Containing a Straight have their Multiplier Quartered";
									break;
								case 2:
									desc = "Hands Containing a Flush have their Multiplier Quartered";
									break;
							}
							break;
					}
					gameManager.bossRounds.Add(entry.Key, new BossRound(tag, desc, random1, random2));
				}
			}
			foreach(KeyValuePair<string, VariantBauble> entry in V.i.v.variantBaubles)
			{
				for(int i = 0; i < entry.Value.startingQuantity; i++)
				{
					Baubles.instance.BaublePurchased(entry.Key, Vector2.zero, true, false, false);
				}
			}
			gameManager.SetCurrentRound(V.i.v.variantSpecialOptions["StartingRound"].impact - 1);
			if(baubles.GetImpactInt("AddZodiacSpecialCardsEachRound") > 0)
			{
				for(int i = 0; i < baubles.GetImpactInt("AddZodiacSpecialCardsEachRound"); i++)
				{
					handArea.AddSpecialCardToDeck("EarnZodiac", Vector2.zero, true);
				}
			}
			deck.CreateDeck(V.i.v.startingDeck);
			// deck.ShuffleDrawPile();
			scoreVial.SetupChipThresholdsForNewRound();
			runInformation.SetupRunInformationNewGame();
			scoreVial.SetScoreToZero();
			playArea.HandlePreplacedCards();
		}
		shop.AdjustVariantDependantBaubles(); // this used to be just for non-loaded games, I can't remember why? It seems fine and important in loaded games, otherwise you can make your shop as big as you want
		deckViewer.SetupInstance();
		handsInformation.SetupHandInfos();
		shop.PopulateAvailableBaubles();
		shop.PopulateAvailableSpecialCards();
		deckPreview.RearangeSuits();
		playArea.ResizePlayArea();
		handsInformation.OrganizeByPlayableCards();
		handsInformation.UpdateAllHandInfos();
		playArea.HandUpdated();
		gameManager.SetVisibilityOfCheatOptions(Preferences.instance.cheatsOn);
		MovingObjects.instance.mo["DrawPile"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["DiscardPile"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["HandArea"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["PlayArea"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["DiscardsRemaining"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["HandsUntilFatigue"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["Currency"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["RoundInformation"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["HandsInformation"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["BaublesInformation"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["ScoreVial"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["Shop"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["BossInformation"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["MenuButtonBackdrop"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["CardParent"].TeleportTo("OffScreen");
		MovingObjects.instance.mo["SlotMachine"].TeleportTo("OffScreen");
		if(V.i.isTutorial)
		{
			tutorial.SetupTutorial();
		}
		else
		{
			Destroy(tutorial.gameObject);
		}
		StartCoroutine(StartGameCoroutine());
	}
	
	public IEnumerator StartGameCoroutine()
	{
		yield return new WaitForSeconds(0.25f);
		TransitionStinger.instance.sceneLoaded = true;
		yield return new WaitForSeconds(0.25f / Preferences.instance.gameSpeed);
		MovingObjects.instance.mo["DrawPile"].StartMove("OnScreen");
		MovingObjects.instance.mo["DiscardPile"].StartMove("OnScreen");
		MovingObjects.instance.mo["DiscardsRemaining"].StartMove("OnScreen");
		MovingObjects.instance.mo["HandsUntilFatigue"].StartMove("OnScreen");
		MovingObjects.instance.mo["Currency"].StartMove("OnScreen");
		MovingObjects.instance.mo["RoundInformation"].StartMove("OnScreen");
		MovingObjects.instance.mo["HandsInformation"].StartMove("OnScreen");
		MovingObjects.instance.mo["BaublesInformation"].StartMove("OnScreen");
		MovingObjects.instance.mo["ScoreVial"].StartMove("OnScreen");
		MovingObjects.instance.mo["MenuButtonBackdrop"].StartMove("OnScreen");
		MovingObjects.instance.mo["CardParent"].StartMove("OnScreen");
		if(V.i.v.variantSpecialOptions["StartInShop"].inEffect && !V.i.loadingGame)
		{
			Shop.instance.OpenShop(false);
		}
		else
		{
			MovingObjects.instance.mo["HandArea"].StartMove("OnScreen");
			MovingObjects.instance.mo["PlayArea"].StartMove("OnScreen");
			MovingObjects.instance.mo["BossInformation"].StartMove("OnScreen");
			yield return new WaitForSeconds(0.75f / Preferences.instance.gameSpeed);
			
			if(!V.i.loadingGame)
			{
				HandArea.instance.StartDrawCards(0, true);
			}
		}
	}
}
