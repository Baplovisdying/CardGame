// Survivor.UIDeckEquipView
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MWUtil;
using Survivor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDeckEquipView : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup rootCanvasGroup;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private TextMeshProUGUI subTitleText;

	[SerializeField]
	public UICoinView coinView;

	[SerializeField]
	public UIHpBarView hpBarView;

	[SerializeField]
	private UIButton exitBtn;

	[SerializeField]
	private UIButton deckBtn;

	[SerializeField]
	public UIHotkeyText deckHotkey;

	[SerializeField]
	public GameObject equipmentsPanelObj;

	[SerializeField]
	public UIDeckEquipItem leftEquipItem;

	[SerializeField]
	public UIDeckEquipItem rightEquipItem;

	[SerializeField]
	public UIItemPackView itemPackView;

	[SerializeField]
	public GameObject trinketsPanelObj;

	[SerializeField]
	public List<UIDeckEquipItem> trinketItems;

	[SerializeField]
	public UIUseItemView useItemBtn;

	[SerializeField]
	private GameObject sellPanelObj;

	[SerializeField]
	private TextMeshProUGUI sellCoinNumber;

	[SerializeField]
	private TextMeshProUGUI sellTitle;

	[SerializeField]
	private UIDeckArea recycleArea;

	[SerializeField]
	private TextMeshProUGUI recycleTitleText;

	[SerializeField]
	private ObjectPool recyclePool;

	[SerializeField]
	private Image recycleSwappable;

	private List<UICardItem> usingCardItemOfRecycleArea = new List<UICardItem>();

	private UICardItem.OnItemPointCallback onCardEnter;

	private UICardItem.OnItemPointCallback onCardExit;

	private UICardItem.OnItemPointCallback onRecycleItemDown;

	private UICardItem.OnItemPointCallback onRecycleItemBeginDrag;

	private UICardItem.OnItemPointCallback onRecycleItemEndDrag;

	public void Init(Action onExitBtnClick, Action onDeckBtnClick, UICardItem.OnItemPointCallback onCardEnter, UICardItem.OnItemPointCallback onCardExit)
	{
		exitBtn.SetCallback(onExitBtnClick);
		deckBtn.SetCallback(onDeckBtnClick);
		this.onCardEnter = onCardEnter;
		this.onCardExit = onCardExit;
	}

	public void InitTrinketItems(UIDeckEquipItem.OnBorderPointCallback onPanelClick, UIDeckEquipItem.OnBorderPointCallback onPanelDrop, UICardItem.OnItemPointCallback onCardEnter, UICardItem.OnItemPointCallback onCardExit, UICardItem.OnItemPointCallback onCardDown, UICardItem.OnItemPointCallback onBeginDrag, UICardItem.OnItemPointCallback onEndDrag)
	{
		for (int i = 0; i < trinketItems.Count; i++)
		{
			UIDeckEquipItem uIDeckEquipItem = trinketItems[i];
			if (i == 0)
			{
				uIDeckEquipItem.SetSlotImage(UICardItemSlotType.UISlotTrinketCharm);
				uIDeckEquipItem.SetBgColor(GameColorData.instance.COLOR_SLOT_TRINKET_CHARM);
			}
			else
			{
				uIDeckEquipItem.SetSlotImage(UICardItemSlotType.UISlotTrinketOther);
				uIDeckEquipItem.SetBgColor(GameColorData.instance.COLOR_SLOT_TRINKET);
			}
			uIDeckEquipItem.onClick = onPanelClick;
			uIDeckEquipItem.onDrop = onPanelDrop;
			uIDeckEquipItem.cardItem.SetCallbacks(onCardEnter, onCardExit, null, onCardDown);
			uIDeckEquipItem.cardItem.SetDragCallbacks(null, null, onBeginDrag, onEndDrag);
		}
	}

	public void InitLeftEquip(UIDeckEquipItem.OnBorderPointCallback onPanelClick, UIDeckEquipItem.OnBorderPointCallback onPanelDrop, UICardItem.OnItemPointCallback onCardEnter, UICardItem.OnItemPointCallback onCardExit, UICardItem.OnItemPointCallback onCardDown, UICardItem.OnItemPointCallback onBeginDrag, UICardItem.OnItemPointCallback onEndDrag)
	{
		leftEquipItem.onClick = onPanelClick;
		leftEquipItem.onDrop = onPanelDrop;
		leftEquipItem.SetSlotImage(UICardItemSlotType.UISlotEquip);
		leftEquipItem.SetBgColor(GameColorData.instance.COLOR_CARD_LEFT);
		leftEquipItem.SetSlotScaleX(1);
		leftEquipItem.cardItem.SetCallbacks(onCardEnter, onCardExit, null, onCardDown);
		leftEquipItem.cardItem.SetDragCallbacks(null, null, onBeginDrag, onEndDrag);
	}

	public void InitRightEquip(UIDeckEquipItem.OnBorderPointCallback onPanelClick, UIDeckEquipItem.OnBorderPointCallback onPanelDrop, UICardItem.OnItemPointCallback onCardEnter, UICardItem.OnItemPointCallback onCardExit, UICardItem.OnItemPointCallback onCardDown, UICardItem.OnItemPointCallback onBeginDrag, UICardItem.OnItemPointCallback onEndDrag)
	{
		rightEquipItem.onClick = onPanelClick;
		rightEquipItem.onDrop = onPanelDrop;
		rightEquipItem.SetSlotImage(UICardItemSlotType.UISlotEquip);
		rightEquipItem.SetBgColor(GameColorData.instance.COLOR_CARD_RIGHT);
		rightEquipItem.SetSlotScaleX(-1);
		rightEquipItem.cardItem.SetCallbacks(onCardEnter, onCardExit, null, onCardDown);
		rightEquipItem.cardItem.SetDragCallbacks(null, null, onBeginDrag, onEndDrag);
	}

	public void InitItemPack(UISmallItem.OnBorderPointCallback onItemPackBorderClick, UISmallItem.OnBorderPointCallback onItemPackBorderDrop, UISmallCard.OnItemPointCallback onItemPackEnter, UISmallCard.OnItemPointCallback onItemPackExit, UISmallCard.OnItemPointCallback onItemPackDown, UISmallCard.OnItemPointCallback onBeginDrag, UISmallCard.OnItemPointCallback onEndDrag)
	{
		itemPackView.Init();
		itemPackView.SetBorderCallbacks(onItemPackBorderClick, onItemPackBorderDrop);
		itemPackView.SetCallbacks(onItemPackEnter, onItemPackExit, onItemPackDown);
		itemPackView.SetDragCallbacks(null, null, onBeginDrag, onEndDrag, null);
		itemPackView.SetUseItemPosOffset(new Vector3(0f, 0.8f, 0f));
	}

	public void InitRecycleItems(Action onPanelClick, Action onPanelDrop, UICardItem.OnItemPointCallback onDown, UICardItem.OnItemPointCallback onBeginDrag, UICardItem.OnItemPointCallback onEndDrag)
	{
		recycleArea.onClick = onPanelClick;
		recycleArea.onDrop = onPanelDrop;
		onRecycleItemDown = onDown;
		onRecycleItemBeginDrag = onBeginDrag;
		onRecycleItemEndDrag = onEndDrag;
	}

	public void End()
	{
		leftEquipItem.cardItem.FreeCard();
		rightEquipItem.cardItem.FreeCard();
		itemPackView.FreeCards();
		RecycleAllOfRecyclePool();
	}

	private UICardItem GetItemFromRecyclePool()
	{
		UICardItem poolObject = recyclePool.GetPoolObject<UICardItem>();
		usingCardItemOfRecycleArea.Add(poolObject);
		poolObject.SetCallbacks(onCardEnter, onCardExit, null, onRecycleItemDown);
		poolObject.SetDragCallbacks(null, null, onRecycleItemBeginDrag, onRecycleItemEndDrag);
		poolObject.gameObject.SetActive(value: true);
		poolObject.useItemPosOffset = new Vector3(0f, -1.5f, 0f);
		return poolObject;
	}

	public void RecycleAllOfRecyclePool()
	{
		recyclePool.RecycleAllPoolObjects();
		foreach (UICardItem item in usingCardItemOfRecycleArea)
		{
			item.FreeCard();
		}
		usingCardItemOfRecycleArea.Clear();
	}

	public void UpdateTrinkets(List<Card> cards)
	{
		for (int i = 0; i < trinketItems.Count; i++)
		{
			UIDeckEquipItem uIDeckEquipItem = trinketItems[i];
			uIDeckEquipItem.HideHintBorder();
			uIDeckEquipItem.gameObject.SetActive(value: false);
			UICardItem cardItem = uIDeckEquipItem.cardItem;
			cardItem.FreeCard();
			cardItem.gameObject.SetActive(value: false);
			if (i < cards.Count)
			{
				uIDeckEquipItem.gameObject.SetActive(value: true);
				Card card = cards[i];
				if (card != null)
				{
					cardItem.gameObject.SetActive(value: true);
					cardItem.SetCard(card);
					cardItem.ShowCard();
					cardItem.UpdateCardVisual(card);
					cardItem.HideLightBorder();
					cardItem.EnableInteractable();
				}
			}
		}
	}

	public void UpdateEquipments(Card leftCard, bool isLeftDark, Card rightCard, bool isRightDark)
	{
		SetEquipItem(leftEquipItem, leftCard, isLeftDark);
		if (leftCard == rightCard)
		{
			isRightDark = true;
		}
		SetEquipItem(rightEquipItem, rightCard, isRightDark);
	}

	public void UpdateEquipmentSlots(bool isShowColorBlindIcon)
	{
		leftEquipItem.colorBlindImg.enabled = isShowColorBlindIcon;
		rightEquipItem.colorBlindImg.enabled = isShowColorBlindIcon;
		foreach (UIDeckEquipItem trinketItem in trinketItems)
		{
			trinketItem.colorBlindImg.enabled = false;
		}
	}

	private void SetEquipItem(UIDeckEquipItem equipItem, Card card, bool isDark = false)
	{
		equipItem.HideHintBorder();
		UICardItem cardItem = equipItem.cardItem;
		if (card != null)
		{
			cardItem.gameObject.SetActive(value: true);
			cardItem.SetCard(card);
			cardItem.ShowCard();
			cardItem.UpdateCardVisual(card, isDark);
			cardItem.HideLightBorder();
			cardItem.EnableInteractable();
		}
		else
		{
			cardItem.FreeCard();
			cardItem.gameObject.SetActive(value: false);
		}
	}

	public void UpdateItemPackView(List<Card> cards)
	{
		itemPackView.UpdateItemPackView(cards);
	}

	public void UpdateRecycleArea(List<Card> cards)
	{
		RecycleAllOfRecyclePool();
		if (cards == null)
		{
			return;
		}
		foreach (Card card in cards)
		{
			UICardItem itemFromRecyclePool = GetItemFromRecyclePool();
			itemFromRecyclePool.SetCard(card);
			itemFromRecyclePool.ShowCard();
			itemFromRecyclePool.UpdateCardVisual(card);
			itemFromRecyclePool.HideLightBorder();
		}
	}

	public void PlayEnterAni(params Card[] playCards)
	{
		if (playCards == null || playCards.Length == 0)
		{
			return;
		}
		foreach (Card card in playCards)
		{
			if (card == null)
			{
				continue;
			}
			foreach (IUIEquippable item in FetchCardItems(card))
			{
				item?.PlayEnterAni();
			}
		}
	}

	private List<IUIEquippable> FetchCardItems(Card card)
	{
		List<IUIEquippable> list = new List<IUIEquippable>();
		if (leftEquipItem.cardItem.GetCard() == card)
		{
			list.Add(leftEquipItem.cardItem);
		}
		if (rightEquipItem.cardItem.GetCard() == card)
		{
			list.Add(rightEquipItem.cardItem);
		}
		UISmallItem[] smallItems = itemPackView.smallItems;
		foreach (UISmallItem uISmallItem in smallItems)
		{
			if (uISmallItem.smallCard.GetCard() == card)
			{
				list.Add(uISmallItem.smallCard);
			}
		}
		foreach (UICardItem item in usingCardItemOfRecycleArea)
		{
			if (item.GetCard() == card)
			{
				list.Add(item);
			}
		}
		foreach (UIDeckEquipItem trinketItem in trinketItems)
		{
			if (trinketItem.cardItem.GetCard() == card)
			{
				list.Add(trinketItem.cardItem);
			}
		}
		return list;
	}

	public void Show(float duration)
	{
		DOTween.Kill(rootCanvasGroup);
		base.gameObject.SetActive(value: true);
		rootCanvasGroup.alpha = 0f;
		rootCanvasGroup.DOFade(1f, duration);
	}

	public void Hide(float duration = 0.5f)
	{
		rootCanvasGroup.DOFade(0f, duration).OnComplete(delegate
		{
			base.gameObject.SetActive(value: false);
		});
	}

	public void DisableInteractable()
	{
		rootCanvasGroup.DisableInteractable();
	}

	public void EnableInteractable()
	{
		rootCanvasGroup.EnableInteractable();
	}

	public void SetSellCoinNumber(int number)
	{
		sellCoinNumber.text = number.ToString();
	}

	public void ShowSwappableLeft()
	{
		leftEquipItem.ShowHintBorder();
	}

	public void HideSwappableLeft()
	{
		leftEquipItem.HideHintBorder();
	}

	public void ShowSwappableRight()
	{
		rightEquipItem.ShowHintBorder();
	}

	public void HideSwappableRight()
	{
		rightEquipItem.HideHintBorder();
	}

	public void ShowSwappableRecycle()
	{
		recycleSwappable.enabled = true;
	}

	public void HideSwappableRecycle()
	{
		recycleSwappable.enabled = false;
	}

	public void ShowSwappableItemPack(int execludeIdx = -1)
	{
		for (int i = 0; i < itemPackView.smallItems.Length; i++)
		{
			UISmallItem uISmallItem = itemPackView.smallItems[i];
			if (execludeIdx != i)
			{
				uISmallItem.ShowBorder();
			}
		}
	}

	public void HideSwappableItemPack()
	{
		UISmallItem[] smallItems = itemPackView.smallItems;
		for (int i = 0; i < smallItems.Length; i++)
		{
			smallItems[i].HideBorder();
		}
	}

	public void ShowSwappableTrinkets(params int[] execludeIdx)
	{
		for (int i = 0; i < trinketItems.Count; i++)
		{
			if (!execludeIdx.Contains(i))
			{
				trinketItems[i].ShowHintBorder();
			}
		}
	}

	public void HideSwappableTrinkets()
	{
		foreach (UIDeckEquipItem trinketItem in trinketItems)
		{
			trinketItem.HideHintBorder();
		}
	}

	public void SetDeckNumber(int num)
	{
		deckBtn.SetText(num.ToString());
	}

	private void SetGeneralModeActive()
	{
		sellPanelObj.SetActive(value: true);
		equipmentsPanelObj.SetActive(value: true);
		trinketsPanelObj.SetActive(value: false);
		coinView.gameObject.SetActive(value: true);
	}

	public void SetEquipMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_EquipTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_EquipSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RecyclePanelTitle);
		sellTitle.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_AutoRecycle);
		SetGeneralModeActive();
	}

	public void SetNewEquipMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_NewEquipTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_NewEquipSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RecyclePanelTitle);
		sellTitle.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_AutoRecycle);
		SetGeneralModeActive();
	}

	public void SetGetItemsOnEndFightMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_EndFightTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_EndFightSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RecyclePanelTitle);
		sellTitle.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_AutoRecycle);
		SetGeneralModeActive();
	}

	public void SetMissionRewardMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_MissionRewardTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_MissionRewardSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RecyclePanelTitle);
		sellTitle.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_AutoRecycle);
		SetGeneralModeActive();
	}

	public void SetSellCardsMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SellCardsTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SellCardsSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RecyclePanelTitle);
		sellTitle.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_AutoRecycle);
		SetGeneralModeActive();
	}

	public void SetNewTrinketMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_NewTrinketTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_NewTrinketSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_NewTrinketRecyclePanelTitle);
		sellPanelObj.SetActive(value: false);
		equipmentsPanelObj.SetActive(value: false);
		trinketsPanelObj.SetActive(value: true);
		coinView.gameObject.SetActive(value: false);
	}

	public void SetLostItemsMode()
	{
		titleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_LostItemsTitle);
		subTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_LostItemsSubTitle);
		recycleTitleText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RecyclePanelTitle);
		sellTitle.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_AutoRecycle);
		SetGeneralModeActive();
	}
}
