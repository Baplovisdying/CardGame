// Survivor.UICardOfTileView
using DG.Tweening;
using MWUtil;
using Survivor;
using UnityEngine;
using UnityEngine.UI;

public class UICardOfTileView : MonoBehaviour
{
	[SerializeField]
	public UICardItem cardItem;

	[SerializeField]
	private Graphic borderCollider;

	[SerializeField]
	private Image borderImg;

	[SerializeField]
	private Image symbolArrow;

	[SerializeField]
	public UIHotkeyText hotkeyText;

	private Vector2 originAnchorPos;

	private void Awake()
	{
		originAnchorPos = ((RectTransform)base.transform).anchoredPosition;
		HideBorder();
		EnableCardItemInteractable();
	}

	public void SetPointerCallbacks(UICardItem.OnItemPointCallback onPointEnter, UICardItem.OnItemPointCallback onPointExit, UICardItem.OnItemPointCallback onPointClick, UICardItem.OnItemPointCallback onPointDown)
	{
		cardItem.SetCallbacks(onPointEnter, onPointExit, onPointClick, onPointDown);
	}

	public void SetDragCallbacks(UICardItem.OnItemPointCallback onDrag, UICardItem.OnItemPointCallback onInitDrag, UICardItem.OnItemPointCallback onBeginDrag, UICardItem.OnItemPointCallback onEndDrag, UICardItem.OnItemPointCallback onDrop)
	{
		cardItem.SetDragCallbacks(onDrag, onInitDrag, onBeginDrag, onEndDrag, onDrop);
	}

	public void SetBorderCallbacks(EventTriggerListener.PointerDelegate onBorderClick, EventTriggerListener.PointerDelegate onBorderEnter, EventTriggerListener.PointerDelegate onBorderExit, EventTriggerListener.PointerDelegate onBorderDrop)
	{
		EventTriggerListener eventTriggerListener = EventTriggerListener.Get(borderCollider.gameObject);
		eventTriggerListener.onPointerClick = onBorderClick;
		eventTriggerListener.onPointerEnter = onBorderEnter;
		eventTriggerListener.onPointerExit = onBorderExit;
		eventTriggerListener.onDrop = onBorderDrop;
	}

	public void Init()
	{
	}

	public void Free()
	{
	}

	public bool IsCardItem(IUIEquippable cardItem)
	{
		return this.cardItem == cardItem as UICardItem;
	}

	public bool IsSameCard(Card card)
	{
		if (card == null)
		{
			return false;
		}
		return cardItem.GetCard() == card;
	}

	public void SetCard(Card card)
	{
		cardItem.SetCard(card);
		cardItem.UpdateCardVisual(card);
	}

	public void Show()
	{
		cardItem.ShowCard();
	}

	public void Hide()
	{
		cardItem.HideCard();
	}

	public Card FreeCard()
	{
		return cardItem.FreeCard();
	}

	public void DeselectCard()
	{
		cardItem.SetDeselect();
	}

	public void HideLightBorder()
	{
		cardItem.HideLightBorder();
	}

	public void ShowBorder()
	{
		borderImg.enabled = true;
		symbolArrow.enabled = true;
	}

	public void HideBorder()
	{
		borderImg.enabled = false;
		symbolArrow.enabled = false;
	}

	public void EnableBorderInteractable()
	{
		borderCollider.enabled = true;
	}

	public void DisableBorderInteractable()
	{
		borderCollider.enabled = false;
	}

	public bool GetBorderInteractable()
	{
		return borderCollider.enabled;
	}

	public void EnableCardItemInteractable()
	{
		cardItem.EnableInteractable();
	}

	public void DisableCardItemInteractable()
	{
		cardItem.DisableInteractable();
	}

	public bool GetCardItemInteractable()
	{
		return cardItem.GetInteractable();
	}

	public void SetPlayerRoundStyle(float duration)
	{
		(base.transform as RectTransform).DOAnchorPos(originAnchorPos, duration).SetEase(Ease.OutBack);
	}

	public void SetNotPlayerRoundStyle(Vector2 plus, float duration)
	{
		(base.transform as RectTransform).DOAnchorPos(originAnchorPos + plus, duration).SetEase(Ease.InOutBack);
	}

	public void PlayCardEnter()
	{
		cardItem.PlayEnterAni();
	}
}
