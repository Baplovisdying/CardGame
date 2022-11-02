// Survivor.UIDeckEquipController
using System;
using System.Collections;
using System.Collections.Generic;
using Survivor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDeckEquipController : MonoBehaviour
{
	public delegate void OnEnd();

	[SerializeField]
	private GameObject viewPrefab;

	private AudioController audioCtrl;

	private UIRootController uiRootCtrl;

	private UIKeywordController uiKeywordCtrl;

	private UIDragCardController uiDragCardCtrl;

	public UIDeckEquipView view;

	private GamePlayController gamePlayCtrl;

	private bool isPlaying;

	private IUIEquippable currentSelectedCardItem;

	private UIEquipTempInfo tempInfo;

	private UIDeckEquipMode mode;

	private bool isHasAnyControl;

	private bool isWarningExit;

	private UIDeckEquipHotkeyController hotkeyCtrl = new UIDeckEquipHotkeyController();

	public void Init(UIRootController uiRootCtrl)
	{
		audioCtrl = MainController.instance.audioCtrl;
		this.uiRootCtrl = uiRootCtrl;
		uiKeywordCtrl = uiRootCtrl.uiKeywordCtrl;
		uiDragCardCtrl = uiRootCtrl.uiDragCardCtrl;
	}

	public void InitOnGamePlay(GamePlayController gamePlayCtrl)
	{
		this.gamePlayCtrl = gamePlayCtrl;
	}

	public void FreeOnGamePlay()
	{
		gamePlayCtrl = null;
	}

	private void TestShowEquipMode(OnEnd onEnd)
	{
		StopAllCoroutines();
		StartCoroutine(PlayModeProcess(UIDeckEquipMode.Equip, onEnd));
	}

	public IEnumerator PlayEquipMode()
	{
		yield return StartCoroutine(PlayModeProcess(UIDeckEquipMode.Equip, null));
	}

	private void TestShowNewTrinketMode(List<CardData> newCardDatas, OnEnd onEnd)
	{
		List<Card> list = new List<Card>();
		for (int i = 0; i < newCardDatas.Count; i++)
		{
			list.Add(new Card(newCardDatas[i]));
		}
		StartCoroutine(PlayModeProcess(UIDeckEquipMode.NewTrinket, onEnd, list));
	}

	public void ShowNewEquipMode(List<CardData> newCardDatas, OnEnd onEnd)
	{
		List<Card> list = new List<Card>();
		for (int i = 0; i < newCardDatas.Count; i++)
		{
			list.Add(new Card(newCardDatas[i]));
		}
		StartCoroutine(PlayModeProcess(UIDeckEquipMode.NewEquip, onEnd, list));
	}

	public void ShowLostItemPackMode(List<Card> lostItems, OnEnd onEnd)
	{
		StartCoroutine(PlayModeProcess(UIDeckEquipMode.LostItems, onEnd, lostItems));
	}

	public IEnumerator PlayLostItemPackMode(List<Card> lostItems)
	{
		yield return PlayModeProcess(UIDeckEquipMode.LostItems, null, lostItems);
	}

	public void ShowSellCardsMode(OnEnd onEnd)
	{
		StopAllCoroutines();
		StartCoroutine(PlayModeProcess(UIDeckEquipMode.SellCards, onEnd));
	}

	public IEnumerator PlayGetItemOnEndFightMode(List<Card> rewards, OnEnd onEnd)
	{
		yield return PlayModeProcess(UIDeckEquipMode.GetItemOnEndFight, onEnd, rewards, AudioSnapshot.EndFightReward);
	}

	public IEnumerator PlayGetMissionReward(List<Card> rewards, OnEnd onEnd)
	{
		yield return PlayModeProcess(UIDeckEquipMode.MissionReward, onEnd, rewards, AudioSnapshot.EndFightReward);
	}

	public IEnumerator PlayGetTrinketRewardsOnComplateStage(List<CardData> rewardDatas, OnEnd onEnd)
	{
		bool flag = false;
		List<Card> list = new List<Card>();
		for (int i = 0; i < rewardDatas.Count; i++)
		{
			if (!(rewardDatas[i] == null))
			{
				list.Add(new Card(rewardDatas[i]));
				flag = true;
			}
		}
		if (flag)
		{
			yield return PlayModeProcess(UIDeckEquipMode.NewTrinket, onEnd, list, AudioSnapshot.EndFightReward);
			yield return DoUpdateAfterSwapTrinket();
		}
	}

	public IEnumerator PlayGameEventForTrinket(List<CardData> newCardDatas)
	{
		List<Card> list = new List<Card>();
		for (int i = 0; i < newCardDatas.Count; i++)
		{
			if (!(newCardDatas[i] == null))
			{
				list.Add(new Card(newCardDatas[i]));
			}
		}
		yield return PlayModeProcess(UIDeckEquipMode.NewTrinket, null, list);
		yield return DoUpdateAfterSwapTrinket();
	}

	private IEnumerator DoUpdateAfterSwapTrinket()
	{
		CharacterBase theHero = gamePlayCtrl.charaCtrl.GetHero();
		if (theHero == null)
		{
			Debug.LogWarning("[UIDeckEquipCtrl] DoUpdateAfterSwapTrinket fail, theHero is null");
			yield break;
		}
		gamePlayCtrl.ResetHeroCharacterFlags(theHero);
		uiRootCtrl.uiPlayerInfoCtrl.UpdateHpBar(theHero);
		gamePlayCtrl.arenaInfo.ResetArenaGlobalInfo(theHero);
		List<Card> list = gamePlayCtrl.cardCtrl.ResetItemPack(theHero);
		if (list != null && list.Count > 0)
		{
			yield return uiRootCtrl.uiDeckEquipCtrl.PlayLostItemPackMode(list);
			gamePlayCtrl.uiGamePlayCtrl.UpdateCoineAmount(gamePlayCtrl.arenaInfo.coinAmount);
			uiRootCtrl.uiPlayerInfoCtrl.UpdateEquipments(theHero);
			uiRootCtrl.uiPlayerInfoCtrl.UpdateItemPack(gamePlayCtrl.cardCtrl.itemPack);
			theHero.UpdateEquipmentsViews();
		}
	}

	public IEnumerator PlayGameEventForEquipments(List<CardData> newCardDatas)
	{
		List<Card> list = new List<Card>();
		for (int i = 0; i < newCardDatas.Count; i++)
		{
			if (!(newCardDatas[i] == null))
			{
				list.Add(new Card(newCardDatas[i]));
			}
		}
		yield return PlayModeProcess(UIDeckEquipMode.NewEquip, null, list);
	}

	private IEnumerator PlayModeProcess(UIDeckEquipMode mode, OnEnd onEnd, List<Card> newCards = null, AudioSnapshot snapshot = AudioSnapshot.UI)
	{
		MainController instance = MainController.instance;
		AudioSnapshot lastSnapshot = audioCtrl.currentSnapshot;
		audioCtrl.TransitionToSnapshot(snapshot);
		this.mode = mode;
		isWarningExit = false;
		isHasAnyControl = false;
		currentSelectedCardItem = null;
		tempInfo = CreateTempInfo(newCards);
		CreateView();
		view.SetDeckNumber(gamePlayCtrl.cardCtrl.deckCardCount);
		switch (mode)
		{
		case UIDeckEquipMode.Equip:
			view.SetEquipMode();
			break;
		case UIDeckEquipMode.NewEquip:
			isWarningExit = true;
			view.SetNewEquipMode();
			break;
		case UIDeckEquipMode.NewTrinket:
			isWarningExit = true;
			view.SetNewTrinketMode();
			break;
		case UIDeckEquipMode.GetItemOnEndFight:
			view.SetGetItemsOnEndFightMode();
			break;
		case UIDeckEquipMode.MissionReward:
			view.SetMissionRewardMode();
			break;
		case UIDeckEquipMode.SellCards:
			view.SetSellCardsMode();
			break;
		case UIDeckEquipMode.LostItems:
			isWarningExit = true;
			view.SetLostItemsMode();
			break;
		}
		view.UpdateEquipmentSlots(instance.optionInfo.isShowColorBlind);
		view.itemPackView.EnableAreaCollider(OnItemPackAreaEnter, OnItemPackAreaExit);
		UpdateCoinNumber();
		UpdateHpBar(isImmediate: true);
		UpdateView();
		HideSwappablePanel();
		HideUseItemBtn();
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Enter);
		float duration = 0.5f;
		view.Show(duration);
		view.DisableInteractable();
		hotkeyCtrl.InitHotkeyReceiver(this, "UIDeckEquip", instance.hotkeyDB, instance.navigationInvoker);
		hotkeyCtrl.UpdateHotkeysVisible(instance.optionInfo.isShowHotkey);
		yield return new WaitForSeconds(duration);
		view.EnableInteractable();
		isPlaying = true;
		while (isPlaying)
		{
			yield return null;
		}
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Exit);
		view.itemPackView.DisableAreaCollider();
		view.DisableInteractable();
		hotkeyCtrl.FreeHotkeyReceiver();
		view.Hide(duration);
		view.End();
		audioCtrl.TransitionToSnapshot(lastSnapshot);
		SaveTempInfoToPlayerInfo(tempInfo);
		currentSelectedCardItem = null;
		tempInfo = null;
		onEnd?.Invoke();
	}

	private void UpdateView()
	{
		view.UpdateTrinkets(tempInfo.trinkets);
		view.UpdateEquipments(tempInfo.leftCard, HasDisableUnequip(tempInfo.leftCard), tempInfo.rightCard, HasDisableUnequip(tempInfo.rightCard));
		view.UpdateRecycleArea(tempInfo.newCards);
		view.UpdateItemPackView(tempInfo.itemPack);
		view.SetSellCoinNumber(GetSellCoinNumber(tempInfo.newCards));
	}

	private UIEquipTempInfo CreateTempInfo(List<Card> newCards = null)
	{
		if (mode == UIDeckEquipMode.NewTrinket)
		{
			List<Card> list = new List<Card>();
			int trinketLimit = MainController.instance.unlockedInfo.trinketLimit;
			CardData[] trinketCardDatas = gamePlayCtrl.arenaInfo.trinketCardDatas;
			for (int i = 0; i < trinketCardDatas.Length && i < trinketLimit; i++)
			{
				CardData cardData = trinketCardDatas[i];
				Card item = null;
				if (cardData != null)
				{
					item = new Card(cardData);
				}
				list.Add(item);
			}
			return new UIEquipTempInfo(list, newCards);
		}
		Card leftCard = null;
		Card rightCard = null;
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		if (hero != null)
		{
			CharacterEquipment equipment = hero.GetEquipment(CardAttackColor.Left);
			CharacterEquipment equipment2 = hero.GetEquipment(CardAttackColor.Right);
			leftCard = equipment?.card;
			rightCard = equipment2?.card;
		}
		CardController cardCtrl = gamePlayCtrl.cardCtrl;
		List<Card> list2 = new List<Card>();
		if (cardCtrl != null)
		{
			for (int j = 0; j < cardCtrl.itemPack.Count; j++)
			{
				ItemPackInfo itemPackInfo = cardCtrl.itemPack[j];
				list2.Add(itemPackInfo.card);
			}
		}
		if (newCards == null)
		{
			newCards = new List<Card>();
		}
		return new UIEquipTempInfo(leftCard, rightCard, list2, newCards);
	}

	private void SaveTempInfoToPlayerInfo(UIEquipTempInfo info)
	{
		if (mode == UIDeckEquipMode.NewTrinket)
		{
			gamePlayCtrl.arenaInfo.UpdateTrinkets(info.trinkets);
			return;
		}
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		CardController cardCtrl = gamePlayCtrl.cardCtrl;
		CharacterEquipment equipment = hero.GetEquipment(CardAttackColor.Left);
		CharacterEquipment equipment2 = hero.GetEquipment(CardAttackColor.Right);
		equipment.SetCard(info.leftCard);
		equipment2.SetCard(info.rightCard);
		for (int i = 0; i < cardCtrl.itemPack.Count; i++)
		{
			ItemPackInfo itemPackInfo = cardCtrl.itemPack[i];
			itemPackInfo.card = null;
			if (i < info.itemPack.Count)
			{
				itemPackInfo.card = info.itemPack[i];
			}
		}
		info.coin = GetSellCoinNumber(info.newCards);
		int num = gamePlayCtrl.arenaInfo.AddCoin(info.coin);
		if (num > 0)
		{
			PlayAddCoinEffect(num);
			UpdateCoinNumber();
		}
	}

	private void CreateView()
	{
		if (!(view != null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(viewPrefab, base.transform);
			view = gameObject.GetComponent<UIDeckEquipView>();
			view.Init(OnExitBtnClick, OnDeckBtnClick, OnCardEnter, OnCardExit);
			view.InitLeftEquip(OnLeftPanelClick, OnLeftEquipPanelDrop, OnCardEnter, OnCardExit, OnLeftEquipCardDown, OnLeftEquipBeginDrag, OnLeftEquipEndDrag);
			view.InitRightEquip(OnRightPanelClick, OnRightEquipPanelDrop, OnCardEnter, OnCardExit, OnRightEquipCardDown, OnRightEquipBeginDrag, OnRightEquipEndDrag);
			view.InitItemPack(OnItemPackBorderClick, OnItemPackBorderDrop, OnItemPackEnter, OnItemPackExit, OnItemPackDown, OnItemPackBeginDrag, OnItemPackEndDrag);
			view.InitRecycleItems(OnRecyclePanelClick, OnRecyclePanelDrop, OnRecycleItemDown, OnRecycleItemBeginDrag, OnRecycleItemEndDrag);
			view.useItemBtn.Init(OnUseItemBtnClick);
			view.InitTrinketItems(OnTrinketClick, OnTrinketDrop, OnCardEnter, OnCardExit, OnTrinketDown, OnTrinketBeginDrag, OnTrinketEndDrag);
		}
	}

	private int GetTrinketItemIndex(UIDeckEquipItem item)
	{
		for (int i = 0; i < view.trinketItems.Count; i++)
		{
			if (item == view.trinketItems[i])
			{
				return i;
			}
		}
		return -1;
	}

	private int GetTrinketItemIndex(UICardItem cardItem)
	{
		for (int i = 0; i < view.trinketItems.Count; i++)
		{
			if (cardItem == view.trinketItems[i].cardItem)
			{
				return i;
			}
		}
		return -1;
	}

	private int GetTrinketItemIndex(Card card)
	{
		for (int i = 0; i < view.trinketItems.Count; i++)
		{
			if (!(view.trinketItems[i].cardItem == null) && card == view.trinketItems[i].cardItem.GetCard())
			{
				return i;
			}
		}
		return -1;
	}

	private UIEquipSlotType GetTrinketSlotType(int index)
	{
		switch (index)
		{
		case 0:
			return UIEquipSlotType.Trinket00;
		case 1:
			return UIEquipSlotType.Trinket01;
		case 2:
			return UIEquipSlotType.Trinket02;
		default:
			Debug.LogError("[UIEquipCtrl] Fail, GetTrinketSlotType no find index: " + index);
			return UIEquipSlotType.Trinket00;
		}
	}

	private void OnTrinketClick(UIDeckEquipItem equipItem, PointerEventData eventData)
	{
		int trinketItemIndex = GetTrinketItemIndex(equipItem);
		if (trinketItemIndex >= 0)
		{
			UIEquipSlotType trinketSlotType = GetTrinketSlotType(trinketItemIndex);
			DoSwapCardItem(trinketSlotType, equipItem.cardItem);
		}
	}

	private void OnTrinketDrop(UIDeckEquipItem equipItem, PointerEventData eventData)
	{
		int trinketItemIndex = GetTrinketItemIndex(equipItem);
		if (trinketItemIndex >= 0)
		{
			UIEquipSlotType trinketSlotType = GetTrinketSlotType(trinketItemIndex);
			DoSwapCardItem(trinketSlotType, equipItem.cardItem);
		}
	}

	private void OnTrinketDown(UICardItem cardItem, PointerEventData eventData)
	{
		int trinketItemIndex = GetTrinketItemIndex(cardItem);
		if (trinketItemIndex >= 0)
		{
			UIEquipSlotType trinketSlotType = GetTrinketSlotType(trinketItemIndex);
			DoItemCardDown(trinketSlotType, cardItem);
		}
	}

	private void OnTrinketBeginDrag(UICardItem cardItem, PointerEventData eventData)
	{
		int trinketItemIndex = GetTrinketItemIndex(cardItem);
		if (trinketItemIndex >= 0)
		{
			UIEquipSlotType trinketSlotType = GetTrinketSlotType(trinketItemIndex);
			DoBeginDrag(trinketSlotType, cardItem);
		}
	}

	private void OnTrinketEndDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoEndDrag();
	}

	private void OnLeftEquipCardDown(UICardItem cardItem, PointerEventData eventData)
	{
		DoItemCardDown(UIEquipSlotType.Left, cardItem);
	}

	private void OnLeftEquipBeginDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoBeginDrag(UIEquipSlotType.Left, cardItem);
	}

	private void OnLeftEquipEndDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoEndDrag();
	}

	private void OnLeftEquipDrop(UICardItem cardItem, PointerEventData eventData)
	{
		DoSwapCardItem(UIEquipSlotType.Left, cardItem);
	}

	private void OnLeftEquipPanelDrop(UIDeckEquipItem equipItem, PointerEventData eventData)
	{
		DoSwapCardItem(UIEquipSlotType.Left, equipItem.cardItem);
	}

	private void OnRightEquipCardDown(UICardItem cardItem, PointerEventData eventData)
	{
		DoItemCardDown(UIEquipSlotType.Right, cardItem);
	}

	private void OnRightEquipBeginDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoBeginDrag(UIEquipSlotType.Right, cardItem);
	}

	private void OnRightEquipEndDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoEndDrag();
	}

	private void OnRightEquipDrop(UICardItem cardItem, PointerEventData eventData)
	{
		DoSwapCardItem(UIEquipSlotType.Right, cardItem);
	}

	private void OnRightEquipPanelDrop(UIDeckEquipItem equipItem, PointerEventData eventData)
	{
		DoSwapCardItem(UIEquipSlotType.Right, equipItem.cardItem);
	}

	private void OnRecycleItemDown(UICardItem cardItem, PointerEventData eventData)
	{
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
		uiKeywordCtrl.Hide();
		if (currentSelectedCardItem == cardItem)
		{
			DeselectCardItem(cardItem);
			HideSwappablePanel();
			HideUseItemBtn();
		}
		else
		{
			DeselectCardItem(currentSelectedCardItem);
			HideUseItemBtn();
			SelectCardItem(cardItem);
			UpdateSwappablePanel(cardItem);
			TryShowUseItemBtn(cardItem);
		}
	}

	private void OnRecycleItemBeginDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoBeginDrag(UIEquipSlotType.Recycle, cardItem);
	}

	private void OnRecycleItemEndDrag(UICardItem cardItem, PointerEventData eventData)
	{
		DoEndDrag();
	}

	private void OnRecyclePanelDrop()
	{
		DoSwapCardItem(UIEquipSlotType.Recycle, null);
	}

	private UIEquipSlotType GetItemPackSlotType(int index)
	{
		switch (index)
		{
		case 0:
			return UIEquipSlotType.ItemPack00;
		case 1:
			return UIEquipSlotType.ItemPack01;
		case 2:
			return UIEquipSlotType.ItemPack02;
		default:
			Debug.LogError("[UIEquipCtrl] Fail, GetItemPackSlotType no find index: " + index);
			return UIEquipSlotType.ItemPack00;
		}
	}

	private void OnItemPackDown(UISmallCard smallCard, PointerEventData eventData)
	{
		int index = view.itemPackView.GetIndex(smallCard.GetCard());
		if (index >= 0)
		{
			UIEquipSlotType itemPackSlotType = GetItemPackSlotType(index);
			DoItemCardDown(itemPackSlotType, smallCard);
		}
	}

	private void OnItemPackBeginDrag(UISmallCard smallCard, PointerEventData eventData)
	{
		int index = view.itemPackView.GetIndex(smallCard.GetCard());
		if (index >= 0)
		{
			UIEquipSlotType itemPackSlotType = GetItemPackSlotType(index);
			DoBeginDrag(itemPackSlotType, smallCard);
			uiDragCardCtrl.ChangeToSmallCard();
		}
	}

	private void OnItemPackEndDrag(UISmallCard smallItem, PointerEventData eventData)
	{
		DoEndDrag();
	}

	private void OnItemPackAreaEnter(GameObject obj)
	{
		uiDragCardCtrl.ChangeToSmallCard();
	}

	private void OnItemPackAreaExit(GameObject obj)
	{
		uiDragCardCtrl.ResumeGeneralCardItem();
	}

	private void OnItemPackBorderDrop(UISmallItem smallItem, PointerEventData eventData)
	{
		int index = view.itemPackView.GetIndex(smallItem);
		if (index >= 0)
		{
			UIEquipSlotType itemPackSlotType = GetItemPackSlotType(index);
			DoSwapCardItem(itemPackSlotType, smallItem.smallCard);
		}
	}

	private void OnItemPackBorderClick(UISmallItem smallItem, PointerEventData eventData)
	{
		int index = view.itemPackView.GetIndex(smallItem);
		if (index >= 0)
		{
			UIEquipSlotType itemPackSlotType = GetItemPackSlotType(index);
			DoSwapCardItem(itemPackSlotType, smallItem.smallCard);
		}
	}

	private void OnItemPackEnter(UISmallCard smallCard, PointerEventData eventData)
	{
		if (!(smallCard == null) && currentSelectedCardItem == null)
		{
			uiKeywordCtrl.ShowCard(smallCard);
		}
	}

	private void OnItemPackExit(UISmallCard smallCard, PointerEventData eventData)
	{
		uiKeywordCtrl.Hide();
	}

	private void OnCardEnter(UICardItem cardItem, PointerEventData eventData)
	{
		if (!(cardItem == null) && currentSelectedCardItem == null)
		{
			uiKeywordCtrl.ShowCard(cardItem);
		}
	}

	private void OnCardExit(UICardItem cardItem, PointerEventData eventData)
	{
		uiKeywordCtrl.Hide();
	}

	public void OnDeckBtnClick()
	{
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
		view.DisableInteractable();
		uiRootCtrl.uiDeckCtrl.ShowOwnCards(delegate
		{
			view.EnableInteractable();
		});
	}

	private void OnExitBtnClick()
	{
		DoExit();
	}

	public void DoExit()
	{
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
		if (!isWarningExit)
		{
			isPlaying = false;
			return;
		}
		if (isHasAnyControl)
		{
			isPlaying = false;
			return;
		}
		view.DisableInteractable();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UINoti_ExitDeckEquip);
		uiRootCtrl.uiNotificationCtrl.ShowPreferNo(locString, delegate(bool isConfirm)
		{
			view.EnableInteractable();
			if (isConfirm)
			{
				isPlaying = false;
			}
		});
	}

	private void OnLeftPanelClick(UIDeckEquipItem equipItem, PointerEventData eventData)
	{
		DoSwapCardItem(UIEquipSlotType.Left, equipItem.cardItem);
	}

	private void OnRightPanelClick(UIDeckEquipItem equipItem, PointerEventData eventData)
	{
		DoSwapCardItem(UIEquipSlotType.Right, equipItem.cardItem);
	}

	private void OnRecyclePanelClick()
	{
		DoSwapCardItem(UIEquipSlotType.Recycle, null);
	}

	private void DoItemCardDown(UIEquipSlotType dstSlot, IUIEquippable dstCardItem)
	{
		uiKeywordCtrl.Hide();
		if (currentSelectedCardItem == null)
		{
			DoSelect(dstSlot, dstCardItem);
			return;
		}
		IUIEquippable srcCardItem = currentSelectedCardItem;
		Card card = srcCardItem.GetCard();
		UIEquipSlotType srcSlot = GetSrcSlot(card);
		if (CanSwap(srcSlot, srcCardItem, dstSlot, dstCardItem))
		{
			WarningDisableUnequip(srcCardItem, delegate
			{
				DoSwapCurrentSelectedCard(srcSlot, srcCardItem, dstSlot);
				HideSwappablePanel();
				HideUseItemBtn();
			});
		}
		else
		{
			DoSelect(dstSlot, dstCardItem);
		}
	}

	private void DoSelect(UIEquipSlotType dstSlot, IUIEquippable dstCardItem)
	{
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
		if (currentSelectedCardItem == dstCardItem)
		{
			DeselectCardItem(dstCardItem);
			HideSwappablePanel();
			HideUseItemBtn();
			return;
		}
		DeselectCardItem(currentSelectedCardItem);
		if (HasDisableUnequip(dstCardItem) && IsSlotInTwoHands(dstSlot))
		{
			ShowDisableUnequip(dstCardItem);
			HideSwappablePanel();
			HideUseItemBtn();
		}
		else
		{
			SelectCardItem(dstCardItem);
			UpdateSwappablePanel(dstCardItem);
			TryShowUseItemBtn(dstCardItem);
		}
	}

	private void DoBeginDrag(UIEquipSlotType slot, IUIEquippable cardItem)
	{
		if (cardItem == null)
		{
			return;
		}
		HideUseItemBtn();
		Card card = cardItem.GetCard();
		if (card != null)
		{
			if (HasDisableUnequip(cardItem) && (slot == UIEquipSlotType.Left || slot == UIEquipSlotType.Right))
			{
				ShowDisableUnequip(cardItem);
				return;
			}
			audioCtrl.PlaySFX(GameSFX.UICard_BeginDrag);
			uiDragCardCtrl.ShowDragingCard(card);
			currentSelectedCardItem = cardItem;
			currentSelectedCardItem.HideCard();
			UpdateSwappablePanel(cardItem);
		}
	}

	private void DoEndDrag()
	{
		uiDragCardCtrl.StopDragingCard();
		uiDragCardCtrl.HideDragingCard();
		if (currentSelectedCardItem != null)
		{
			audioCtrl.PlaySFX(GameSFX.UICard_EndDrag);
			currentSelectedCardItem.ShowCard();
			currentSelectedCardItem.HideLightBorder();
			currentSelectedCardItem = null;
			HideSwappablePanel();
			HideUseItemBtn();
		}
	}

	private void DoSwapCardItem(UIEquipSlotType dstSlot, IUIEquippable dstCardItem)
	{
		uiDragCardCtrl.StopDragingCard();
		uiDragCardCtrl.HideDragingCard();
		if (currentSelectedCardItem == null)
		{
			return;
		}
		IUIEquippable srcCardItem = currentSelectedCardItem;
		Card card = srcCardItem.GetCard();
		UIEquipSlotType srcSlot = GetSrcSlot(card);
		if (CanSwap(srcSlot, srcCardItem, dstSlot, dstCardItem))
		{
			WarningDisableUnequip(srcCardItem, delegate
			{
				DoSwapCurrentSelectedCard(srcSlot, srcCardItem, dstSlot);
				HideSwappablePanel();
				HideUseItemBtn();
			});
		}
	}

	private bool SelectCardItem(IUIEquippable cardItem)
	{
		if (cardItem == null)
		{
			return false;
		}
		currentSelectedCardItem = cardItem;
		currentSelectedCardItem.ShowLightBorder();
		return true;
	}

	private void DeselectCardItem(IUIEquippable cardItem)
	{
		if (currentSelectedCardItem != null)
		{
			currentSelectedCardItem.HideLightBorder();
			currentSelectedCardItem = null;
		}
	}

	private int GetSellCoinNumber(List<Card> sellCards)
	{
		int num = 0;
		foreach (Card sellCard in sellCards)
		{
			if (sellCard != null)
			{
				int sellPrice = sellCard.GetSellPrice();
				num += sellPrice;
			}
		}
		return num;
	}

	private UIEquipSlotType GetSrcSlot(Card srcCard)
	{
		int index = view.itemPackView.GetIndex(srcCard);
		if (index >= 0)
		{
			return GetItemPackSlotType(index);
		}
		if (view.leftEquipItem.cardItem.GetCard() == srcCard)
		{
			return UIEquipSlotType.Left;
		}
		if (view.rightEquipItem.cardItem.GetCard() == srcCard)
		{
			return UIEquipSlotType.Right;
		}
		int trinketItemIndex = GetTrinketItemIndex(srcCard);
		if (trinketItemIndex >= 0)
		{
			return GetTrinketSlotType(trinketItemIndex);
		}
		return UIEquipSlotType.Recycle;
	}

	private void HideSwappablePanel()
	{
		view.HideSwappableLeft();
		view.HideSwappableRight();
		view.HideSwappableRecycle();
		view.HideSwappableItemPack();
		view.HideSwappableTrinkets();
	}

	private void UpdateSwappablePanel(IUIEquippable dstCardItem)
	{
		HideSwappablePanel();
		if (dstCardItem == null)
		{
			return;
		}
		Card card = dstCardItem.GetCard();
		if (card == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		switch (GetSrcSlot(card))
		{
		case UIEquipSlotType.Recycle:
			flag = true;
			flag2 = true;
			if (!HasDisableUnequip(dstCardItem))
			{
				view.ShowSwappableItemPack();
			}
			if (card.data.IsTrinketCharm())
			{
				view.ShowSwappableTrinkets(1, 2);
			}
			else
			{
				view.ShowSwappableTrinkets(default(int));
			}
			break;
		case UIEquipSlotType.Left:
			flag2 = true;
			view.ShowSwappableRecycle();
			view.ShowSwappableItemPack();
			break;
		case UIEquipSlotType.Right:
			flag = true;
			view.ShowSwappableRecycle();
			view.ShowSwappableItemPack();
			break;
		case UIEquipSlotType.ItemPack00:
			flag = true;
			flag2 = true;
			view.ShowSwappableRecycle();
			view.ShowSwappableItemPack(0);
			break;
		case UIEquipSlotType.ItemPack01:
			flag = true;
			flag2 = true;
			view.ShowSwappableRecycle();
			view.ShowSwappableItemPack(1);
			break;
		case UIEquipSlotType.ItemPack02:
			flag = true;
			flag2 = true;
			view.ShowSwappableRecycle();
			view.ShowSwappableItemPack(2);
			break;
		case UIEquipSlotType.Trinket00:
			view.ShowSwappableRecycle();
			break;
		case UIEquipSlotType.Trinket01:
			view.ShowSwappableRecycle();
			view.ShowSwappableTrinkets(0, 1);
			break;
		case UIEquipSlotType.Trinket02:
			view.ShowSwappableRecycle();
			view.ShowSwappableTrinkets(0, 2);
			break;
		}
		if (card.isRequireTwoHands)
		{
			if (flag && flag2 && !HasDisableUnequip(view.leftEquipItem.cardItem) && !HasDisableUnequip(view.rightEquipItem.cardItem))
			{
				view.ShowSwappableLeft();
				view.ShowSwappableRight();
			}
			return;
		}
		if (flag && !HasDisableUnequip(view.leftEquipItem.cardItem))
		{
			view.ShowSwappableLeft();
		}
		if (flag2 && !HasDisableUnequip(view.rightEquipItem.cardItem))
		{
			view.ShowSwappableRight();
		}
	}

	private void WarningDisableUnequip(IUIEquippable cardItem, Action callback)
	{
		if (!HasDisableUnequip(cardItem))
		{
			callback();
			return;
		}
		view.DisableInteractable();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UINoti_DisableUnequip);
		uiRootCtrl.uiNotificationCtrl.ShowPreferNo(locString, delegate(bool isConfirm)
		{
			view.EnableInteractable();
			if (isConfirm)
			{
				callback();
			}
		});
	}

	private bool CanSwap(UIEquipSlotType srcSlot, IUIEquippable srcCardItem, UIEquipSlotType dstSlot, IUIEquippable dstCardItem)
	{
		if (srcCardItem == null)
		{
			return false;
		}
		if (srcSlot == dstSlot)
		{
			return false;
		}
		Card card = srcCardItem.GetCard();
		if (card.data.cardType == CardType.Trinket)
		{
			if (card.data.IsTrinketCharm())
			{
				if (dstSlot != UIEquipSlotType.Trinket00 && dstSlot != UIEquipSlotType.Recycle)
				{
					return false;
				}
			}
			else if (dstSlot == UIEquipSlotType.Trinket00)
			{
				return false;
			}
		}
		else
		{
			if (HasDisableUnequip(srcCardItem) && !IsSlotInTwoHands(dstSlot))
			{
				return false;
			}
			if (dstCardItem != null && HasDisableUnequip(dstCardItem) && IsSlotInTwoHands(dstSlot))
			{
				ShowDisableUnequip(dstCardItem);
				return false;
			}
			Card card2 = null;
			if (dstCardItem != null)
			{
				card2 = dstCardItem.GetCard();
			}
			if ((card.isRequireTwoHands || (card2 != null && card2.isRequireTwoHands)) && (IsSlotInTwoHands(srcSlot) || IsSlotInTwoHands(dstSlot)))
			{
				bool flag = false;
				flag = ((!card.isRequireTwoHands || !IsSlotInTwoHands(dstSlot)) ? CanEquippable(card, dstSlot) : (CanEquippable(card, UIEquipSlotType.Left) && CanEquippable(card, UIEquipSlotType.Right)));
				bool flag2 = false;
				flag2 = card2 == null || ((!card2.isRequireTwoHands || !IsSlotInTwoHands(srcSlot)) ? CanEquippable(card2, srcSlot) : (CanEquippable(card2, UIEquipSlotType.Left) && CanEquippable(card2, UIEquipSlotType.Right)));
				return flag && flag2;
			}
		}
		return CanEquippable(card, dstSlot);
	}

	private bool CanEquippable(Card srcCard, UIEquipSlotType dstSlot)
	{
		switch (dstSlot)
		{
		case UIEquipSlotType.Left:
			if (tempInfo.leftCard == null)
			{
				return true;
			}
			if (HasDisableUnequip(tempInfo.leftCard))
			{
				ShowDisableUnequip(view.leftEquipItem.cardItem);
				return false;
			}
			break;
		case UIEquipSlotType.Right:
			if (tempInfo.rightCard == null)
			{
				return true;
			}
			if (HasDisableUnequip(tempInfo.rightCard))
			{
				ShowDisableUnequip(view.rightEquipItem.cardItem);
				return false;
			}
			break;
		case UIEquipSlotType.Recycle:
			return true;
		}
		Card cardBy = tempInfo.GetCardBy(dstSlot);
		if (cardBy == null)
		{
			return true;
		}
		return cardBy != srcCard;
	}

	private void DoSwapCurrentSelectedCard(UIEquipSlotType srcSlot, IUIEquippable srcCardItem, UIEquipSlotType dstSlot)
	{
		if (srcCardItem != null)
		{
			Card card = srcCardItem.GetCard();
			List<Card> list = tempInfo.SwapCard(card, srcSlot, dstSlot);
			UpdateView();
			isHasAnyControl = true;
			audioCtrl.PlaySFX(GameSFX.UICard_PlayEnterAni);
			view.PlayEnterAni(list.ToArray());
			currentSelectedCardItem = null;
		}
	}

	private void PlayAddCoinEffect(int addCoinNumber)
	{
		if (!(gamePlayCtrl == null) && !(view == null))
		{
			UIEffectController uiEffectCtrl = uiRootCtrl.uiEffectCtrl;
			if (!(uiEffectCtrl == null))
			{
				Vector3 coinAmountPos = view.coinView.GetCoinAmountPos();
				uiEffectCtrl.ShowAddNumberEffect(coinAmountPos, addCoinNumber);
				audioCtrl.PlaySFX(GameSFX.CoinGet);
			}
		}
	}

	private void UpdateHpBar(bool isImmediate)
	{
		if (!(view == null) && !(gamePlayCtrl == null))
		{
			CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
			if (!(hero == null))
			{
				uiRootCtrl.UpdateHpBar(view.hpBarView, hero, isImmediate);
			}
		}
	}

	private void UpdateCoinNumber()
	{
		if (!(view == null) && !(gamePlayCtrl == null))
		{
			ArenaInfo arenaInfo = gamePlayCtrl.arenaInfo;
			view.coinView.SetCoinNumber(arenaInfo.coinAmount);
		}
	}

	private void TryShowUseItemBtn(IUIEquippable cardItem)
	{
		if (cardItem != null)
		{
			Card card = cardItem.GetCard();
			if (card != null && UsingCardFlags.CanUseItemInNotFighting(card))
			{
				view.useItemBtn.Play(cardItem.GetUseItemPos());
			}
		}
	}

	private void HideUseItemBtn()
	{
		view.useItemBtn.Stop();
	}

	private void OnUseItemBtnClick()
	{
		if (currentSelectedCardItem == null || gamePlayCtrl == null)
		{
			return;
		}
		CardController cardCtrl = gamePlayCtrl.cardCtrl;
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		if (!(hero == null))
		{
			audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
			Card card = currentSelectedCardItem.GetCard();
			int healValue = 0;
			if (UsingCardFlags.UseHealItemOnNotFighting(card, hero, cardCtrl, out healValue))
			{
				tempInfo.RemoveCardFromAllEquipables(card);
				UpdateView();
				isHasAnyControl = true;
				UpdateHpBar(isImmediate: false);
				uiRootCtrl.PlayHealEffByUsePotion(view.hpBarView.GetTextEffectPos(), healValue);
				DeselectCardItem(currentSelectedCardItem);
				HideSwappablePanel();
				HideUseItemBtn();
			}
		}
	}

	private bool HasDisableUnequip(IUIEquippable cardItem)
	{
		if (cardItem == null)
		{
			return false;
		}
		Card card = cardItem.GetCard();
		if (card == null)
		{
			return false;
		}
		return HasDisableUnequip(card);
	}

	private bool HasDisableUnequip(Card card)
	{
		if (card == null)
		{
			return false;
		}
		if (!card.data.HasAnyCardFlag(CardFlagType.DisableUnequip))
		{
			return false;
		}
		return true;
	}

	private bool IsSlotInTwoHands(UIEquipSlotType dstSlot)
	{
		if (dstSlot != 0)
		{
			return dstSlot == UIEquipSlotType.Right;
		}
		return true;
	}

	private void ShowDisableUnequip(IUIEquippable cardItem)
	{
		UIEffectController uiEffectCtrl = uiRootCtrl.uiEffectCtrl;
		Vector3 effTextPos = cardItem.GetEffTextPos();
		uiEffectCtrl.ShowTextEffect(effTextPos, StringDB_General.instance.GetLocString(StringDB_General_Keys.MSG_DisableUnequip));
	}
}
