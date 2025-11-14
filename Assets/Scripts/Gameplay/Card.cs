using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Deck;
using System;

public class Card : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rt;
    public RectTransform selectionGlowRT;
	public Image rankImage;
	public RectTransform rankImageRT;
	public Image bigSuitImage;
	public RectTransform bigSuitImageRT;
	public Image detailImage;
	public RectTransform detailImageRT;
	public Image extraImage;
	public Image front;
	public Image back;
	public Image xImage;
	public TooltipObject tooltipObject;
	public ScorePlate scorePlate;
	public ControllerSelectableObject controllerSelectableObject;
	
	public bool canMove;
	public bool faceDown;
	public bool isDeckviewerClone;
	public int originalSiblingIndex;
	public DropZone dropZonePlacedIn;
	
	private IEnumerator moveCoroutine;
	public bool moving;
	private IEnumerator flipCoroutine;
	public bool flipping;
	public CardData cardData;
	
	public void StartMove(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd = true, bool destroyAtEnd = false, bool discardAtEnd = false, bool addToDrawPileAtEnd = false, DropZone newDropZoneAtEnd = null)
	{
		if(moving)
		{
			StopCoroutine(moveCoroutine);
		}
		moveCoroutine = MoveCard(destination, destinationRotation, canMoveAtEnd, destroyAtEnd, discardAtEnd, addToDrawPileAtEnd, newDropZoneAtEnd);
		StartCoroutine(moveCoroutine);
	}
	
	void OnDestroy()
	{
		if(HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Contains(controllerSelectableObject))
		{
			HandArea.instance.cardsControllerSelectionGroup.controllerSelectableObjects.Remove(controllerSelectableObject);
		}
	}
	
	public IEnumerator MoveCard(Vector2 destination, Vector3 destinationRotation, bool canMoveAtEnd = true, bool destroyAtEnd = false, bool discardAtEnd = false, bool addToDrawPileAtEnd = false, DropZone newDropZoneAtEnd = null)
	{
		canMove = false;
		moving = true;
		Quaternion originalRotationQ = rt.localRotation;
		Quaternion destinationRotationQ = Quaternion.Euler(destinationRotation);
		Vector2 originalPosition = rt.anchoredPosition;
		float t = 0;
		float moveTime = LocalInterface.instance.animationDuration / 4f;
		while(t < moveTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, moveTime);
			rt.localRotation = Quaternion.Lerp(originalRotationQ, destinationRotationQ, t / moveTime);
			rt.anchoredPosition = Vector2.Lerp(originalPosition, destination, t / moveTime);
			if(ControllerSelection.instance.usingController && ControllerSelection.instance.currentlySelectedObject == controllerSelectableObject)
			{
				ControllerSelection.instance.RepositionControllerSelectionRT(controllerSelectableObject, HandArea.instance.cardsControllerSelectionGroup);
				OnPointerEnter(new PointerEventData(EventSystem.current));
				if(cardData.isSpecialCard && Preferences.instance.showSpecialCardTooltips)
				{
					tooltipObject.DisplayTooltip(true);
				}
			}
			yield return null;
		}
		rt.localRotation = destinationRotationQ;
		rt.anchoredPosition = destination;
		moving = false;
		ChangeCanMove(canMoveAtEnd);
		if(discardAtEnd)
		{
			Deck.instance.discardPile.Add(cardData);
			Deck.instance.UpdateCardsInDiscardPile();
			Deck.instance.cardsInHand.Remove(this);
		}
		if(addToDrawPileAtEnd)
		{
			Deck.instance.AddCardToDrawPile(cardData);
		}
		if(newDropZoneAtEnd != null)
		{
			rt.SetParent(newDropZoneAtEnd.rt);
			newDropZoneAtEnd.CardPlaced(this);
		}
		if(destroyAtEnd)
		{
			Destroy(this.gameObject);
		}
	}
	
	public void ChangeFaceDown(bool isFaceDown)
	{
		faceDown = isFaceDown;
		front.gameObject.SetActive(!faceDown);
		back.gameObject.SetActive(faceDown);
	}
	
	public void StartFlip(bool allowMovementAfterwards = false)
	{
		if(flipping)
		{
			StopCoroutine(flipCoroutine);
		}
		flipCoroutine = Flip(allowMovementAfterwards);
		StartCoroutine(flipCoroutine);
	}
	
	public IEnumerator Flip(bool allowMovementAfterwards = false)
	{
		flipping = true;
		ChangeCanMove(false);
		Vector3 originalScale = rt.localScale;
		Vector3 destinationScale = rt.localScale;
		destinationScale.x = 0;
		float flipTime = LocalInterface.instance.animationDuration / 8f;
		float t = 0;
		while(t < flipTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, flipTime);
			rt.localScale = Vector3.Lerp(originalScale, destinationScale, t / flipTime);
			yield return null;
		}
		ChangeFaceDown(!faceDown);
		t = 0;
		while(t < flipTime)
		{
			t = Mathf.Clamp(t + Time.deltaTime * Preferences.instance.gameSpeed, 0, flipTime);
			rt.localScale = Vector3.Lerp(destinationScale, originalScale, t / flipTime);
			yield return null;
		}
		rt.localScale = Vector3.one;
		if(allowMovementAfterwards)
		{
			ChangeCanMove(true);
		}
		flipping = false;
	}
	
	public void ChangeCanMove(bool canCardMove)
	{
		if(!canCardMove)
		{
			rt.localScale = Vector3.one;
		}
		canMove = canCardMove;
	}
	
	public void OnBeginDrag(PointerEventData pointerEventData)
    {
		if(isDeckviewerClone || !canMove || PlayArea.instance.locked)
		{
			return;
		}
		if(dropZonePlacedIn != null)
		{
			dropZonePlacedIn.CardRemoved();
			PlayArea.instance.HandUpdated();
			dropZonePlacedIn = null;
			if(cardData.isSpecialCard)
			{
				RevertToOriginalImage();
			}
		}
		SoundManager.instance.PlayCardPickupSound();
		rt.rotation = Quaternion.identity;
		originalSiblingIndex = transform.GetSiblingIndex();
		transform.SetParent(HandArea.instance.looseCardParent);
	}
	
	public void OnDrag(PointerEventData pointerEventData)
    {
		if(isDeckviewerClone || !canMove || PlayArea.instance.locked)
		{
			return;
		}
		// Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * LocalInterface.instance.referenceResolution.x - LocalInterface.instance.referenceResolution.x / 2, (Input.mousePosition.y / Screen.height) * LocalInterface.instance.referenceResolution.y - LocalInterface.instance.referenceResolution.y / 2);
		Vector2 mousePos = LocalInterface.instance.GetMousePosition();
		rt.anchoredPosition = mousePos;
		rt.rotation = Quaternion.identity;
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach (RaycastResult result in results)
		{
			if(result.gameObject != null)
			{
				if(result.gameObject == HandArea.instance.handAreaCardZone)
				{
					int desiredIndex = 0;
					for(int i = 0; i < HandArea.instance.cardParent.childCount; i++)
					{
						if(rt.anchoredPosition.x < HandArea.instance.cardParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x)
						{
							desiredIndex = i;
							break;
						}
						if(i == HandArea.instance.cardParent.childCount - 1)
						{
							desiredIndex = i + 1;
						}
					}
					HandArea.instance.siblingIndexOfLooseCard = desiredIndex;
					HandArea.instance.ReorganizeHand();
					return;
				}
			}
		}
		HandArea.instance.siblingIndexOfLooseCard = -1;
		HandArea.instance.ReorganizeHand();
	}
	
	public void OnEndDrag(PointerEventData pointerEventData)
	{
		if(isDeckviewerClone || !canMove || PlayArea.instance.locked)
		{
			return;
		}
		SoundManager.instance.PlayCardDropSound();
		// Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) * LocalInterface.instance.referenceResolution.x - LocalInterface.instance.referenceResolution.x / 2, (Input.mousePosition.y / Screen.height) * LocalInterface.instance.referenceResolution.y - LocalInterface.instance.referenceResolution.y / 2);
		// Vector2 mousePos = LocalInterface.instance.GetMousePosition();
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, results);
		foreach (RaycastResult result in results)
		{
			if (result.gameObject != null)
			{
				if(result.gameObject == HandArea.instance.handAreaCardZone)
				{
					int desiredIndex = 0;
					for(int i = 0; i < HandArea.instance.cardParent.childCount; i++)
					{
						if(rt.anchoredPosition.x < HandArea.instance.cardParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x)
						{
							desiredIndex = i;
							break;
						}
						if(i == HandArea.instance.cardParent.childCount - 1)
						{
							desiredIndex = i + 1;
						}
					}
					HandArea.instance.siblingIndexOfLooseCard = desiredIndex;
					transform.SetParent(HandArea.instance.cardParent);
					transform.SetSiblingIndex(desiredIndex);
					HandArea.instance.siblingIndexOfLooseCard = -1;
					if(desiredIndex != originalSiblingIndex)
					{
						HandArea.instance.ChangeAlwaysSortType(0);
					}
					HandArea.instance.ReorganizeHand();
					return;
				}
				DropZone dropZone;
				if(result.gameObject.transform.parent.TryGetComponent(out dropZone))
				{
					if(!dropZone.locked && !PlayArea.instance.locked)
					{
						if(!dropZone.cardPlaced || dropZone.cardPlaced && dropZone.placedCard.canMove)
						{
							if((!cardData.isSpecialCard && dropZone.specialCardsOnly) || (cardData.isSpecialCard && !dropZone.specialCardsOnly && Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones") == 0 && !V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect))
							{
								if(cardData.isSpecialCard)
								{
									Debug.Log("Can't place special cards into standard drop zones without the AllowSpecialCardsInStandardDropZones bauble, or the special option SpecialCardsAllowedInStandardSlots enabled");
									Debug.Log($"cardData.isSpecialCard={cardData.isSpecialCard}, dropZone.specialCardsOnly={dropZone.specialCardsOnly}, Baubles.instance.GetImpactInt(AllowSpecialCardsInStandardDropZones)={Baubles.instance.GetImpactInt("AllowSpecialCardsInStandardDropZones")}, V.i.v.variantSpecialOptions[SpecialCardsAllowedInStandardSlots].inEffect={V.i.v.variantSpecialOptions["SpecialCardsAllowedInStandardSlots"].inEffect}");
								}
								else
								{
									Debug.Log("Standard cards can't go into special card drop zones");
								}
							}
							else
							{
								dropZonePlacedIn = dropZone;
								if(dropZone.cardPlaced)
								{
									dropZone.placedCard.dropZonePlacedIn = null;
									dropZone.placedCard.transform.SetParent(HandArea.instance.cardParent);
									dropZone.placedCard.transform.SetSiblingIndex(0);
									if(dropZone.placedCard.cardData.isSpecialCard)
									{
										dropZone.placedCard.RevertToOriginalImage();
									}
									HandArea.instance.ReorganizeHand();
								}
								transform.SetParent(dropZone.transform);
								rt.anchoredPosition = Vector2.zero;
								dropZone.CardPlaced(this);
								if(cardData.isSpecialCard)
								{
									UpdateToDropZoneImage();
								}
								PlayArea.instance.HandUpdated();
								return;
							}
						}
					}
				}
			}
		}
		transform.SetParent(HandArea.instance.cardParent);
		transform.SetSiblingIndex(originalSiblingIndex);
		HandArea.instance.ReorganizeHand();
	}
	
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		CheckCheatInput();
		if(this == HandArea.instance.topDeckCard)
		{
			if(!DeckViewer.instance.visibilityObject.activeSelf)
			{
				DeckViewer.instance.OpenDeckViewer(1);
			}
		}
		if(isDeckviewerClone || !canMove || PlayArea.instance.locked)
		{
			return;
		}
		if(transform.parent == HandArea.instance.cardParent && canMove)
		{
			if(HandArea.instance.selectedCards.Contains(this))
			{
				HandArea.instance.selectedCards.Remove(this);
				HandArea.instance.SelectedCardsUpdated();
				rt.anchoredPosition = rt.anchoredPosition - Vector2.up * HandArea.instance.distanceDifferenceOfSelectedCard;
				SoundManager.instance.PlayCardDropSound();
			}
			else
			{
				HandArea.instance.selectedCards.Add(this);
				HandArea.instance.SelectedCardsUpdated();
				rt.anchoredPosition = rt.anchoredPosition + Vector2.up * HandArea.instance.distanceDifferenceOfSelectedCard;
				SoundManager.instance.PlayCardPickupSound();
			}
		}
		OnPointerEnter(pointerEventData);
	}
	
	public void RevertToOriginalImage()
	{
		if(!cardData.isSpecialCard)
		{
			return;
		}
		if(cardData.specialCardName.Length >= 18 && cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
		{
			return;
		}
		detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].sprite;
		detailImageRT.sizeDelta = new Vector2(V.i.v.variantSpecialCards[cardData.specialCardName].sprite.rect.width, V.i.v.variantSpecialCards[cardData.specialCardName].sprite.rect.height);
	}
	
	public void UpdateToDropZoneImage()
	{
		if(!cardData.isSpecialCard)
		{
			return;
		}
		if(cardData.specialCardName.Length >= 18 && cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
		{
			return;
		}
		if(cardData.specialCardName == "ConvertCard")
		{
			/* if(!dropZonePlacedIn.specialCardsOnly && dropZonePlacedIn.dropZoneNumber != 0 && dropZonePlacedIn.dropZoneNumber != GameManager.instance.GetMaxPlayableStandardCards() - 1)
			{
				if(PlayArea.instance.standardDropZones[dropZonePlacedIn.dropZoneNumber - 1].cardPlaced && PlayArea.instance.standardDropZones[dropZonePlacedIn.dropZoneNumber + 1].cardPlaced)
				{
					if(!PlayArea.instance.standardDropZones[dropZonePlacedIn.dropZoneNumber - 1].placedCard.cardData.isSpecialCard && !PlayArea.instance.standardDropZones[dropZonePlacedIn.dropZoneNumber + 1].placedCard.cardData.isSpecialCard)
					{
						detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite;
					}
					else
					{
						detailImage.sprite = PlayArea.instance.magicMirrorMisplacedSprite;
					}
				}
				else
				{
					detailImage.sprite = PlayArea.instance.magicMirrorMisplacedSprite;
				}
			}
			else
			{
				detailImage.sprite = PlayArea.instance.magicMirrorMisplacedSprite;
			} */
			if(dropZonePlacedIn.dropZoneNumber != 0)
			{
				if(dropZonePlacedIn.specialCardsOnly)
				{
					if(PlayArea.instance.specialCardDropZones[dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
					{
						detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite;
					}
					else
					{
						detailImage.sprite = PlayArea.instance.magicMirrorMisplacedSprite;
					}
				}
				else
				{
					if(PlayArea.instance.standardDropZones[dropZonePlacedIn.dropZoneNumber - 1].cardPlaced)
					{
						detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite;
					}
					else
					{
						detailImage.sprite = PlayArea.instance.magicMirrorMisplacedSprite;
					}
				}
			}
			else
			{
				detailImage.sprite = PlayArea.instance.magicMirrorMisplacedSprite;
			}
		}
		else
		{
			detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite;
		}
		detailImageRT.sizeDelta = new Vector2(detailImage.sprite.rect.width, detailImage.sprite.rect.height);
	}
	
	public void UpdateGraphics()
	{
		bool inGameplayScene = LocalInterface.instance.GetCurrentSceneName() == "GameplayScene";
		if(cardData.isSpecialCard)
		{
			if(cardData.specialCardName.Length >= 18 && cardData.specialCardName.Substring(0, 18) == "ZodiacsFromFlushes")
			{
				string flushesData = cardData.specialCardName.Replace("ZodiacsFromFlushes", string.Empty);
				string[] flushesDataSplit = flushesData.Split('¤', StringSplitOptions.RemoveEmptyEntries);
				List<int> includedFlushes = new List<int>();
				for(int i = 0; i < flushesDataSplit.Length; i++)
				{
					includedFlushes.Add(int.Parse(flushesDataSplit[i]));
				}
				if(includedFlushes.Contains(GameManager.instance.flushZodiacBaubleSuitOrders[0]))
				{
					rankImage.gameObject.SetActive(true);
					rankImage.sprite = LocalInterface.instance.puzzlePieceSprites[0];
					rankImageRT.sizeDelta = new Vector2(LocalInterface.instance.puzzlePieceSprites[0].rect.width, LocalInterface.instance.puzzlePieceSprites[0].rect.height);
					rankImage.color = LocalInterface.instance.suitColors[GameManager.instance.flushZodiacBaubleSuitOrders[0]];
					rankImageRT.anchoredPosition = new Vector2(-8.5f, 6f);
				}
				else
				{
					rankImage.gameObject.SetActive(false);
				}
				
				if(includedFlushes.Contains(GameManager.instance.flushZodiacBaubleSuitOrders[1]))
				{
					detailImage.gameObject.SetActive(true);
					detailImage.sprite = LocalInterface.instance.puzzlePieceSprites[1];
					detailImageRT.sizeDelta = new Vector2(LocalInterface.instance.puzzlePieceSprites[1].rect.width, LocalInterface.instance.puzzlePieceSprites[1].rect.height);
					detailImage.color = LocalInterface.instance.suitColors[GameManager.instance.flushZodiacBaubleSuitOrders[1]];
					detailImageRT.anchoredPosition = new Vector2(6f, 8.5f);
				}
				else
				{
					detailImage.gameObject.SetActive(false);
				}
				
				if(includedFlushes.Contains(GameManager.instance.flushZodiacBaubleSuitOrders[2]))
				{
					bigSuitImage.gameObject.SetActive(true);
					bigSuitImage.sprite = LocalInterface.instance.puzzlePieceSprites[2];
					bigSuitImageRT.sizeDelta = new Vector2(LocalInterface.instance.puzzlePieceSprites[2].rect.width, LocalInterface.instance.puzzlePieceSprites[2].rect.height);
					bigSuitImage.color = LocalInterface.instance.suitColors[GameManager.instance.flushZodiacBaubleSuitOrders[2]];
					bigSuitImageRT.anchoredPosition = new Vector2(-6f, -8.5f);
				}
				else
				{
					bigSuitImage.gameObject.SetActive(false);
				}
				
				if(includedFlushes.Contains(GameManager.instance.flushZodiacBaubleSuitOrders[3]))
				{
					extraImage.gameObject.SetActive(true);
					extraImage.sprite = LocalInterface.instance.puzzlePieceSprites[3];
					extraImage.rectTransform.sizeDelta = new Vector2(LocalInterface.instance.puzzlePieceSprites[3].rect.width, LocalInterface.instance.puzzlePieceSprites[3].rect.height);
					extraImage.color = LocalInterface.instance.suitColors[GameManager.instance.flushZodiacBaubleSuitOrders[3]];
					extraImage.rectTransform.anchoredPosition = new Vector2(8.5f, -6f);
				}
				else
				{
					extraImage.gameObject.SetActive(false);
				}
				name = cardData.specialCardName;
				tooltipObject.gameObject.SetActive(true);
				tooltipObject.mainText = $"Gain {includedFlushes.Count} Zodiac{(includedFlushes.Count > 1? "s" : "")} for your played hand";
				tooltipObject.title = $"{LocalInterface.instance.numbers[includedFlushes.Count].Substring(0, 1).ToUpper()}{LocalInterface.instance.numbers[includedFlushes.Count].Substring(1)} Piece Puzzle";
				tooltipObject.titleColor = ThemeManager.UIElementType.CardName;
				tooltipObject.subtitle = "Special";
				tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity("Special");
				return;
			}
			detailImage.gameObject.SetActive(true);
			rankImage.gameObject.SetActive(false);
			bigSuitImage.gameObject.SetActive(false);
			extraImage.gameObject.SetActive(false);
			detailImageRT.anchoredPosition = Vector2.zero;
			detailImage.color = Color.white;
			if(dropZonePlacedIn == null)
			{
				detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].sprite;
				detailImageRT.sizeDelta = new Vector2(V.i.v.variantSpecialCards[cardData.specialCardName].sprite.rect.width, V.i.v.variantSpecialCards[cardData.specialCardName].sprite.rect.height);
			}
			else
			{
				detailImage.sprite = V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite;
				detailImageRT.sizeDelta = new Vector2(V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite.rect.width, V.i.v.variantSpecialCards[cardData.specialCardName].playedSprite.rect.height);
			}
			name = cardData.specialCardName;
			tooltipObject.gameObject.SetActive(true);
			var resolver = new DescriptionResolver();
			string input = V.i.v.variantSpecialCards[cardData.specialCardName].description;
			string output = resolver.Resolve(input);
			tooltipObject.mainText = output;
			tooltipObject.title = V.i.v.variantSpecialCards[cardData.specialCardName].specialCardName;
			tooltipObject.titleColor = ThemeManager.UIElementType.CardName;
			tooltipObject.subtitle = V.i.v.variantSpecialCards[cardData.specialCardName].category;
			tooltipObject.subtitleColor = ThemeManager.instance.GetElementTypeForRarity(V.i.v.variantSpecialCards[cardData.specialCardName].category);
			return;
		}
		tooltipObject.gameObject.SetActive(false);
		rankImage.gameObject.SetActive(true);
		detailImage.gameObject.SetActive(true);
		rankImageRT.anchoredPosition = new Vector2(-12f, 10f);
		bigSuitImage.gameObject.SetActive(true);
		bigSuitImageRT.anchoredPosition = new Vector2(-12f, -5f);
		detailImageRT.anchoredPosition = new Vector2(6f, 0);
		extraImage.gameObject.SetActive(false);
		name = LocalInterface.instance.ConvertRankAndSuitToString(cardData.rank, cardData.suit);
		if(cardData.suit < 4)
		{
			rankImage.color = LocalInterface.instance.suitColors[cardData.suit];
			bigSuitImage.color = LocalInterface.instance.suitColors[cardData.suit];
			if(cardData.rank <= 8 || cardData.rank == 12)
			{
				detailImage.color = LocalInterface.instance.suitColors[cardData.suit];
			}
			else
			{
				detailImage.color = Color.white;
			}
			if(inGameplayScene)
			{
				rankImage.sprite = HandArea.instance.rankSprites[cardData.rank];
			}
			else
			{
				rankImage.sprite = DeckVariantMenu.instance.rankSprites[cardData.rank];
			}
		}
		else
		{
			rankImage.color = Color.white;
			bigSuitImage.color = Color.white;
			detailImage.color = Color.white;
			if(inGameplayScene)
			{
				rankImage.sprite = HandArea.instance.rankSprites[cardData.rank + 13];
			}
			else
			{
				rankImage.sprite = DeckVariantMenu.instance.rankSprites[cardData.rank + 13];
			}
		}
		if(inGameplayScene)
		{
			rankImageRT.sizeDelta = new Vector2(HandArea.instance.rankSprites[cardData.rank].rect.width, HandArea.instance.rankSprites[cardData.rank].rect.height);
			bigSuitImage.sprite = HandArea.instance.bigSuitSprites[cardData.suit];
			bigSuitImageRT.sizeDelta = new Vector2(HandArea.instance.bigSuitSprites[cardData.suit].rect.width, HandArea.instance.bigSuitSprites[cardData.suit].rect.height);
			int cardNumber = cardData.suit * 13 + cardData.rank;
			detailImage.sprite = HandArea.instance.cardDetails[cardNumber];
			detailImageRT.sizeDelta = new Vector2(HandArea.instance.cardDetails[cardNumber].rect.width, HandArea.instance.cardDetails[cardNumber].rect.height);
		}
		else
		{
			rankImageRT.sizeDelta = new Vector2(DeckVariantMenu.instance.rankSprites[cardData.rank].rect.width, DeckVariantMenu.instance.rankSprites[cardData.rank].rect.height);
			bigSuitImage.sprite = DeckVariantMenu.instance.suitSprites[cardData.suit];
			bigSuitImageRT.sizeDelta = new Vector2(DeckVariantMenu.instance.suitSprites[cardData.suit].rect.width, DeckVariantMenu.instance.suitSprites[cardData.suit].rect.height);
			int cardNumber = cardData.suit * 13 + cardData.rank;
			detailImage.sprite = DeckVariantMenu.instance.cardDetails[cardNumber];
			detailImageRT.sizeDelta = new Vector2(DeckVariantMenu.instance.cardDetails[cardNumber].rect.width, DeckVariantMenu.instance.cardDetails[cardNumber].rect.height);
		}
	}
	
	public void ChangeSuit(int newSuit)
	{
		cardData.suit = newSuit;
		UpdateGraphics();
	}
	
	public void ChangeRank(int newRank)
	{
		float rankValueChange = GetBaseValueByRank(newRank) - GetBaseValueByRank(cardData.rank);
		cardData.baseValue += rankValueChange;
		cardData.rank = newRank;
		UpdateGraphics();
	}
	
	public void CheckCheatInput()
	{
		if(Preferences.instance.cheatsOn)
		{
			if(Input.GetKey(KeyCode.S))
			{
				ChangeSuit(0);
			}
			if(Input.GetKey(KeyCode.C))
			{
				ChangeSuit(1);
			}
			if(Input.GetKey(KeyCode.H))
			{
				ChangeSuit(2);
			}
			if(Input.GetKey(KeyCode.D))
			{
				ChangeSuit(3);
			}
			if(Input.GetKey(KeyCode.R))
			{
				ChangeSuit(4);
			}
			if(Input.GetKey(KeyCode.Alpha2))
			{
				ChangeRank(0);
			}
			if(Input.GetKey(KeyCode.Alpha3))
			{
				ChangeRank(1);
			}
			if(Input.GetKey(KeyCode.Alpha4))
			{
				ChangeRank(2);
			}
			if(Input.GetKey(KeyCode.Alpha5))
			{
				ChangeRank(3);
			}
			if(Input.GetKey(KeyCode.Alpha6))
			{
				ChangeRank(4);
			}
			if(Input.GetKey(KeyCode.Alpha7))
			{
				ChangeRank(5);
			}
			if(Input.GetKey(KeyCode.Alpha8))
			{
				ChangeRank(6);
			}
			if(Input.GetKey(KeyCode.Alpha9))
			{
				ChangeRank(7);
			}
			if(Input.GetKey(KeyCode.Alpha0))
			{
				ChangeRank(8);
			}
			if(Input.GetKey(KeyCode.J))
			{
				ChangeRank(9);
			}
			if(Input.GetKey(KeyCode.Q))
			{
				ChangeRank(10);
			}
			if(Input.GetKey(KeyCode.K))
			{
				ChangeRank(11);
			}
			if(Input.GetKey(KeyCode.A))
			{
				ChangeRank(12);
			}
		}
	}
	
	public void OnPointerEnter(PointerEventData pointerEventData)
    {
		if(isDeckviewerClone)
		{
			originalSiblingIndex = rt.GetSiblingIndex();
			rt.SetSiblingIndex(rt.parent.childCount - 1);
		}
		if(cardData.isSpecialCard)
		{
			return;
		}
		if(Math.Abs(cardData.multiplier) < 0.1d && Math.Abs(cardData.baseValue - Deck.GetBaseValueByRank(cardData.rank)) < 0.1d && Preferences.instance.onlyShowModifiedCardValues)
		{
			return;
		}
		if(LocalInterface.instance.GetCurrentSceneName() != "GameplayScene")
		{
			return;
		}
		CardValuesTooltip.instance.DisplayTooltip(this);
	}
	
	public void OnPointerExit(PointerEventData pointerEventData)
    {
		if(isDeckviewerClone)
		{
			rt.SetSiblingIndex(originalSiblingIndex);
		}
		if(cardData.isSpecialCard)
		{
			return;
		}
		if(LocalInterface.instance.GetCurrentSceneName() != "GameplayScene")
		{
			return;
		}
		CardValuesTooltip.instance.HideTooltip();
	}
/* 	
	public void SetRaycastability(bool newRaycastabilityState)
	{
		rankImage.raycastTarget = newRaycastabilityState;
	} */
	
/* 	void Update()
	{
		if(selectionGlowRT.gameObject.activeSelf)
		{
			selectionGlowRT.localEulerAngles += new Vector3(0, 0, 60f * Time.deltaTime);
		}
	} */
}
