// Survivor.UICardView
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MWUtil;
using Survivor;
using UnityEngine;

public class UICardView : MonoBehaviour
{
	[SerializeField]
	private RectTransform handCardsPanelTrans;

	[SerializeField]
	private CanvasGroup handCardsPanel;

	[SerializeField]
	private ObjectPool cardPool;

	private UICardItem.OnItemPointCallback onPointEnter;

	private UICardItem.OnItemPointCallback onPointExit;

	private UICardItem.OnItemPointCallback onPointClick;

	private UICardItem.OnItemPointCallback onPointDown;

	private UICardItem.OnItemPointCallback onPointUp;

	private UICardItem.OnItemPointCallback onDrag;

	private UICardItem.OnItemPointCallback onInitDrag;

	private UICardItem.OnItemPointCallback onBeginDrag;

	private UICardItem.OnItemPointCallback onEndDrag;

	private UICardItem.OnItemPointCallback onDrop;

	private float handCardPanelOriginY;

	[SerializeField]
	private float handCardPanelWidth = 1320f;

	[SerializeField]
	private float sideOffsetX = 100f;

	[SerializeField]
	private float cardItemWidth = 205f;

	[SerializeField]
	private float selectedOffsetX = 20f;

	public List<UICardItem> usingCardItems { get; private set; }

	private void Awake()
	{
		usingCardItems = new List<UICardItem>();
		handCardPanelOriginY = handCardsPanelTrans.anchoredPosition.y;
	}

	public void Init()
	{
		EnableHandPanelInteractable();
	}

	public void SetPointerCallbacks(UICardItem.OnItemPointCallback onPointEnter, UICardItem.OnItemPointCallback onPointExit, UICardItem.OnItemPointCallback onPointClick, UICardItem.OnItemPointCallback onPointDown, UICardItem.OnItemPointCallback onPointUp)
	{
		this.onPointEnter = onPointEnter;
		this.onPointExit = onPointExit;
		this.onPointClick = onPointClick;
		this.onPointDown = onPointDown;
		this.onPointUp = onPointUp;
	}

	public void SetDragCallbacks(UICardItem.OnItemPointCallback onDrag, UICardItem.OnItemPointCallback onInitDrag, UICardItem.OnItemPointCallback onBeginDrag, UICardItem.OnItemPointCallback onEndDrag, UICardItem.OnItemPointCallback onDrop)
	{
		this.onDrag = onDrag;
		this.onInitDrag = onInitDrag;
		this.onBeginDrag = onBeginDrag;
		this.onEndDrag = onEndDrag;
		this.onDrop = onDrop;
	}

	public void SetPlayerRoundStyle(float duration = 0.35f)
	{
		handCardsPanelTrans.DOAnchorPosY(handCardPanelOriginY, duration).SetEase(Ease.OutBack);
	}

	public void SetNotPlayerRoundStyle(float duration = 0.35f)
	{
		handCardsPanelTrans.DOAnchorPosY(handCardPanelOriginY - 70f, duration).SetEase(Ease.InOutBack);
	}

	public UICardItem GetNewUICardItem()
	{
		UICardItem poolObject = cardPool.GetPoolObject<UICardItem>();
		poolObject.gameObject.SetActive(value: true);
		poolObject.transform.SetAsLastSibling();
		poolObject.SetCallbacks(onPointEnter, onPointExit, onPointClick, onPointDown, onPointUp);
		poolObject.SetDragCallbacks(onDrag, onInitDrag, onBeginDrag, onEndDrag, onDrop);
		poolObject.SetDeselect(immediate: true);
		poolObject.HideLightBorder();
		poolObject.EnableInteractable();
		usingCardItems.Add(poolObject);
		return poolObject;
	}

	public void RecycleAllCardsImmediate()
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			usingCardItem.DisableInteractable();
			usingCardItem.FreeCard();
			usingCardItem.SetDeselect(immediate: true);
		}
		usingCardItems.Clear();
		cardPool.RecycleAllPoolObjects();
	}

	public void RecycleCardImmediate(UICardItem cardItem)
	{
		cardItem.DisableInteractable();
		cardItem.FreeCard();
		cardItem.SetDeselect(immediate: true);
		usingCardItems.Remove(cardItem);
		cardPool.RecyclePoolObject(cardItem.gameObject);
	}

	public void RecycleFromCardPool(UICardItem removeCardItem)
	{
		cardPool.RecyclePoolObject(removeCardItem.gameObject);
	}

	public void SortHandCards(List<UICardItem> newCards = null, float duration = 0.5f)
	{
		if (usingCardItems != null && usingCardItems.Count > 0)
		{
			usingCardItems.Sort((UICardItem x, UICardItem y) => Card.CompareInHandCard(x.GetCard(), y.GetCard()));
			DoSortHandCards(null, newCards, duration);
		}
	}

	public void UpdateHotkeyTexts(HotkeyDatabase hotkeyDB, List<HotkeyData> handHotkeyTexts, bool isForceHide)
	{
		if (usingCardItems == null || usingCardItems.Count <= 0 || handHotkeyTexts == null || handHotkeyTexts.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < usingCardItems.Count; i++)
		{
			UICardItem uICardItem = usingCardItems[i];
			uICardItem.HideHotkey();
			if (!isForceHide && i < handHotkeyTexts.Count)
			{
				string hotkeyHintString = hotkeyDB.GetHotkeyHintString(handHotkeyTexts[i]);
				uICardItem.SetHotkey(hotkeyHintString);
				uICardItem.ShowHotkey();
			}
		}
	}

	public void SortHandCardsForSelected(IUIEquippable selectedItem, float duration = 0.5f)
	{
		UICardItem selectedCard = selectedItem as UICardItem;
		DoSortHandCards(selectedCard, null, duration);
	}

	private void DoSortHandCards(UICardItem selectedCard, List<UICardItem> newCards, float duration)
	{
		if (usingCardItems == null || usingCardItems.Count <= 0)
		{
			return;
		}
		float num = (float)usingCardItems.Count * cardItemWidth;
		float num2 = handCardPanelWidth - sideOffsetX * 2f;
		float num3 = 0f;
		if (num2 < num)
		{
			num = num2;
			if (selectedCard != null && usingCardItems.Contains(selectedCard))
			{
				num3 = selectedOffsetX * 0.5f;
			}
		}
		float num4 = num / (float)usingCardItems.Count;
		bool flag = true;
		float num5 = -1f * num * 0.5f + num4 * 0.5f;
		for (int i = 0; i < usingCardItems.Count; i++)
		{
			UICardItem uICardItem = usingCardItems[i];
			float num6 = num5 + (float)i * num4;
			if (selectedCard != null && uICardItem == selectedCard)
			{
				uICardItem.transform.SetSiblingIndex(usingCardItems.Count);
				flag = false;
			}
			else if (flag)
			{
				uICardItem.transform.SetSiblingIndex(i);
				num6 -= num3;
			}
			else
			{
				uICardItem.transform.SetSiblingIndex(i - 1);
				num6 += num3;
			}
			if (newCards != null && newCards.Contains(uICardItem))
			{
				uICardItem.SetRootAnchoredPos(new Vector2(num6, 0f));
			}
			else
			{
				uICardItem.PlayRootAnchorPosMove(new Vector2(num6, 0f), duration);
			}
		}
	}

	public void ShowSelectedCards(params UICardItem[] cards)
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			if (cards != null && cards.Contains(usingCardItem))
			{
				usingCardItem.SetSelect();
			}
			else
			{
				usingCardItem.SetDeselect();
			}
		}
	}

	public void DeselectAllCards()
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			usingCardItem.SetDeselect();
		}
	}

	public void ShowLightBorderCards(params UICardItem[] cards)
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			if (cards != null && cards.Contains(usingCardItem))
			{
				usingCardItem.ShowLightBorder();
			}
			else
			{
				usingCardItem.HideLightBorder();
			}
		}
	}

	public void HideAllLightBorderCards()
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			usingCardItem.HideLightBorder();
		}
	}

	public void ShowCards()
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			usingCardItem.ShowCard();
		}
	}

	public UICardItem GetFirstUsedCardItem()
	{
		if (usingCardItems.Count <= 0)
		{
			return null;
		}
		return usingCardItems[0];
	}

	public UICardItem GetUICardItemFromUsing(Card card)
	{
		foreach (UICardItem usingCardItem in usingCardItems)
		{
			if (!(usingCardItem == null) && usingCardItem.GetCard() != null && usingCardItem.GetCard() == card)
			{
				return usingCardItem;
			}
		}
		return null;
	}

	public bool Contains(UICardItem cardItem)
	{
		return usingCardItems.Contains(cardItem);
	}

	public void DisableHandPanelInteractable()
	{
		handCardsPanel.DisableInteractable();
	}

	public void EnableHandPanelInteractable()
	{
		handCardsPanel.EnableInteractable();
	}
}
