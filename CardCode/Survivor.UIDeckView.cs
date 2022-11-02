// Survivor.UIDeckView
using System;
using System.Collections.Generic;
using DG.Tweening;
using MWUtil;
using Survivor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDeckView : MonoBehaviour
{
	[SerializeField]
	public CanvasGroup rootCanvasGroup;

	[SerializeField]
	private TextMeshProUGUI titleText;

	[SerializeField]
	private TextMeshProUGUI subTitleText;

	[SerializeField]
	private GameObject exitPanel;

	[SerializeField]
	private Image exitIcon;

	[SerializeField]
	private UIButton confirmBtn;

	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	public UICoinView coinView;

	[SerializeField]
	public UIPointDownUpHandler hideBtn;

	[SerializeField]
	public TextMeshProUGUI hideBtnText;

	[SerializeField]
	private UIDeckScrollItem deckScrollItem;

	private List<UIDeckItem> usingCardItemsOfDeck = new List<UIDeckItem>();

	private UICardItem.OnItemPointCallback onDeckCardItemDown;

	private UICardItem.OnItemPointCallback onDeckCardItemClick;

	private UICardItem.OnItemPointCallback onCardEnter;

	private UICardItem.OnItemPointCallback onCardExit;

	public void Init(EventTriggerListener.PointerDelegate onExitBtnClick, Action onConfirmClick, UICardItem.OnItemPointCallback onCardEnter, UICardItem.OnItemPointCallback onCardExit, UICardItem.OnItemPointCallback onDeckCardItemDown, UICardItem.OnItemPointCallback onDeckCardItemClick, Action onDeckScrollDrag)
	{
		EventTriggerListener.Get(exitPanel).onPointerClick = onExitBtnClick;
		confirmBtn.SetCallback(onConfirmClick);
		this.onCardEnter = onCardEnter;
		this.onCardExit = onCardExit;
		this.onDeckCardItemDown = onDeckCardItemDown;
		this.onDeckCardItemClick = onDeckCardItemClick;
		deckScrollItem.SetDragCallback(onDeckScrollDrag);
	}

	public void End()
	{
		RecycleDeckCards();
	}

	private UIDeckItem GetItemFormDeckCardPool()
	{
		UIDeckItem poolObject = deckScrollItem.deckCardPool.GetPoolObject<UIDeckItem>();
		usingCardItemsOfDeck.Add(poolObject);
		poolObject.cardItem.SetCallbacks(onCardEnter, onCardExit, onDeckCardItemClick, onDeckCardItemDown);
		poolObject.gameObject.SetActive(value: true);
		return poolObject;
	}

	private void RecycleDeckCards()
	{
		deckScrollItem.deckCardPool.RecycleAllPoolObjects();
		foreach (UIDeckItem item in usingCardItemsOfDeck)
		{
			item.cardItem.FreeCard();
		}
		usingCardItemsOfDeck.Clear();
	}

	public void UpdateDeckView(UIDeckMode mode, List<Card> deckCards, CardNumberEnvironment env = null, UnlockedInfo unlockedInfo = null, ArenaInfo arenaInfo = null, float priceOff = 1f)
	{
		RecycleDeckCards();
		if (deckCards == null)
		{
			return;
		}
		bool flag = arenaInfo != null;
		int num = 0;
		if (flag && arenaInfo != null)
		{
			num = arenaInfo.coinAmount;
		}
		foreach (Card deckCard in deckCards)
		{
			if (deckCard == null)
			{
				continue;
			}
			UIDeckItem itemFormDeckCardPool = GetItemFormDeckCardPool();
			bool isDark = false;
			itemFormDeckCardPool.priceObj.SetActive(value: false);
			if (flag && arenaInfo != null)
			{
				int num2 = 0;
				int num3 = 0;
				switch (mode)
				{
				case UIDeckMode.UpgradeEquipment:
					num2 = deckCard.GetPriceOfUpgradeEquipment(arenaInfo, priceOff);
					num3 = deckCard.GetPriceOfUpgradeEquipment(arenaInfo);
					break;
				case UIDeckMode.UpgradeIncrease:
					num2 = deckCard.GetPriceOfUpgradeIncrease(arenaInfo, priceOff);
					num3 = deckCard.GetPriceOfUpgradeIncrease(arenaInfo);
					break;
				}
				if (num3 == num2 || priceOff == 1f)
				{
					itemFormDeckCardPool.priceNum.text = num2.ToString();
				}
				else
				{
					itemFormDeckCardPool.priceNum.text = string.Format(ConstValue.DISCOUNT_PRICE_STR, num3, num2);
				}
				if (num >= num2)
				{
					itemFormDeckCardPool.priceNum.color = Color.white;
				}
				else
				{
					itemFormDeckCardPool.priceNum.color = Color.red;
				}
				itemFormDeckCardPool.priceObj.SetActive(value: true);
			}
			itemFormDeckCardPool.lockImg.enabled = false;
			if (deckCard.IsLocked(unlockedInfo))
			{
				itemFormDeckCardPool.lockImg.enabled = true;
				isDark = true;
			}
			UICardItem cardItem = itemFormDeckCardPool.cardItem;
			cardItem.SetCard(deckCard);
			cardItem.ShowCard();
			if (env == null)
			{
				cardItem.UpdateCardVisual(deckCard, isDark);
			}
			else
			{
				cardItem.UpdateCardVisual(deckCard, isDark, isSecHand: false, env.arenaInfo, env.srcChara);
				cardItem.UpdateMainNumberByEnvironment(deckCard, env);
			}
			cardItem.HideLightBorder();
		}
	}

	public void ShowExitIcon()
	{
		exitIcon.enabled = true;
	}

	public void HideExitBtn()
	{
		exitIcon.enabled = false;
	}

	public void SetConfirmBtnText(string text)
	{
		confirmBtn.SetText(text);
	}

	public void ShowConfirmBtn()
	{
		confirmBtn.gameObject.SetActive(value: true);
	}

	public void HideConfirmBtn()
	{
		confirmBtn.gameObject.SetActive(value: false);
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

	public void SetTitle(string titleStr)
	{
		titleText.text = titleStr;
	}

	public void SetSubTitle(string subTextStr)
	{
		subTitleText.text = subTextStr;
	}

	public void SetScrollSensitivity(float value)
	{
		scrollRect.scrollSensitivity = value;
	}

	public void Hiding(float duration = 0.25f)
	{
		DOTween.Kill(rootCanvasGroup);
		rootCanvasGroup.DOFade(0f, duration);
	}

	public void ResumeHiding(float duration = 0.25f)
	{
		DOTween.Kill(rootCanvasGroup);
		rootCanvasGroup.DOFade(1f, duration);
	}
}
