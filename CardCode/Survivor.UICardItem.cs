// Survivor.UICardItem
using System.Collections.Generic;
using DG.Tweening;
using MWUtil;
using Survivor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UICardItem : MonoBehaviour, IUIEquippable, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
	public delegate void OnItemPointCallback(UICardItem cardItem, PointerEventData eventData);

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private UICardItemDestroyAni cardDestroyAni;

	[SerializeField]
	private RectTransform cardTrans;

	[SerializeField]
	private SpriteAtlas cardBgAtlas;

	[SerializeField]
	private Image cardBg;

	[SerializeField]
	private UICardItemBorder border;

	[SerializeField]
	private Image increaseOn;

	[SerializeField]
	private Image cardImg;

	[SerializeField]
	private Image handSideLeftImg;

	[SerializeField]
	private Image handSideRightImg;

	[SerializeField]
	private Image colorBlindImg;

	[SerializeField]
	private Image rareImg;

	[SerializeField]
	private Image keywordBg;

	[SerializeField]
	private GameObject pointPanel;

	[SerializeField]
	private TextMeshProUGUI pointNum;

	[SerializeField]
	private TextMeshProUGUI titleTxt;

	[SerializeField]
	private TextMeshProUGUI typeTxt;

	[SerializeField]
	private GameObject increasePanel;

	[SerializeField]
	private TextMeshProUGUI increaseNum;

	[SerializeField]
	private GameObject otherPanel;

	[SerializeField]
	private Image secCardIconImg;

	[SerializeField]
	private GameObject[] generalObjs;

	[SerializeField]
	private TextMeshProUGUI mainNum;

	[SerializeField]
	private Image mainNumIcon;

	[SerializeField]
	private GameObject[] equipExtendObjs;

	[SerializeField]
	private TextMeshProUGUI equipNum;

	[SerializeField]
	private Image equipNumIcon;

	[SerializeField]
	public RectTransform keywordPosTrans;

	[SerializeField]
	public RectTransform keywordNegaPosTrans;

	[SerializeField]
	private UIHotkeyText hotkeyText;

	[SerializeField]
	private bool isDarkMode;

	[SerializeField]
	private Image[] darkImages;

	[SerializeField]
	private TextMeshProUGUI[] darkTexts;

	private bool isSelected;

	private OnItemPointCallback onPointEnter;

	private OnItemPointCallback onPointExit;

	private OnItemPointCallback onPointClick;

	private OnItemPointCallback onPointDown;

	private OnItemPointCallback onPointUp;

	private OnItemPointCallback onDrag;

	private OnItemPointCallback onInitDrag;

	private OnItemPointCallback onBeginDrag;

	private OnItemPointCallback onEndDrag;

	private OnItemPointCallback onDrop;

	private Card _card;

	public Vector3 useItemPosOffset = new Vector3(0f, 1.5f, 0f);

	private void Awake()
	{
		border.enabled = false;
		increaseOn.enabled = false;
		canvasGroup.alpha = 1f;
		HideHotkey();
	}

	public void ClearAllCallbacks()
	{
		onPointEnter = null;
		onPointExit = null;
		onPointClick = null;
		onPointDown = null;
		onPointUp = null;
		onDrag = null;
		onInitDrag = null;
		onBeginDrag = null;
		onEndDrag = null;
		onDrop = null;
	}

	public void SetCallbacks(OnItemPointCallback onPointEnter = null, OnItemPointCallback onPointExit = null, OnItemPointCallback onPointClick = null, OnItemPointCallback onPointDown = null, OnItemPointCallback onPointUp = null)
	{
		this.onPointEnter = onPointEnter;
		this.onPointExit = onPointExit;
		this.onPointClick = onPointClick;
		this.onPointDown = onPointDown;
		this.onPointUp = onPointUp;
	}

	public void SetDragCallbacks(OnItemPointCallback onDrag = null, OnItemPointCallback onInitDrag = null, OnItemPointCallback onBeginDrag = null, OnItemPointCallback onEndDrag = null, OnItemPointCallback onDrop = null)
	{
		this.onDrag = onDrag;
		this.onInitDrag = onInitDrag;
		this.onBeginDrag = onBeginDrag;
		this.onEndDrag = onEndDrag;
		this.onDrop = onDrop;
		if (onDrag != null || onInitDrag != null || onBeginDrag != null || onEndDrag != null)
		{
			EventTriggerDragListener orAddComponent = base.gameObject.GetOrAddComponent<EventTriggerDragListener>();
			orAddComponent.onDrag = OnDrag;
			orAddComponent.onInitDrag = OnInitializePotentialDrag;
			orAddComponent.onBeginDrag = OnBeginDrag;
			orAddComponent.onEndDrag = OnEndDrag;
		}
		if (this.onDrop != null)
		{
			base.gameObject.GetOrAddComponent<EventTriggerDropListener>().onDrop = OnDrop;
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && onPointClick != null)
		{
			onPointClick(this, eventData);
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && onPointDown != null)
		{
			onPointDown(this, eventData);
		}
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && onPointUp != null)
		{
			onPointUp(this, eventData);
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		if (onPointExit != null)
		{
			onPointExit(this, eventData);
		}
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
	{
		if (onPointEnter != null)
		{
			onPointEnter(this, eventData);
		}
	}

	private void OnDrag(PointerEventData eventData)
	{
		if (onDrag != null)
		{
			onDrag(this, eventData);
		}
	}

	private void OnInitializePotentialDrag(PointerEventData eventData)
	{
		if (onInitDrag != null)
		{
			onInitDrag(this, eventData);
		}
	}

	private void OnBeginDrag(PointerEventData eventData)
	{
		if (onBeginDrag != null)
		{
			onBeginDrag(this, eventData);
		}
	}

	private void OnEndDrag(PointerEventData eventData)
	{
		if (onEndDrag != null)
		{
			onEndDrag(this, eventData);
		}
	}

	private void OnDrop(PointerEventData eventData)
	{
		if (onDrop != null)
		{
			onDrop(this, eventData);
		}
	}

	public void SetCard(Card card)
	{
		_card = card;
		base.name = card.title;
		SetView(card.data, card.GetTotalCardFlags());
		DisableKeywordBg();
	}

	public void SetView(CardData cardData, List<CardFlag> totalCardFlags = null)
	{
		SetCardBg(cardData.cardType, cardData.cardRare);
		SetCardImage(cardData.cardImage);
		switch (cardData.actMethodPointVisual)
		{
		default:
			HideMainNumIcon();
			break;
		case CardActMethodPointVisual.Attack:
			SetMainIcon(UICardMainIconType.Attack);
			break;
		case CardActMethodPointVisual.Defend:
			SetMainIcon(UICardMainIconType.Defend);
			break;
		case CardActMethodPointVisual.Hemorrhage:
		case CardActMethodPointVisual.OriginAttack:
			SetMainIcon(UICardMainIconType.Hemorrhage);
			break;
		}
		if (totalCardFlags == null)
		{
			ResetOtherPanel(cardData.cardFlags);
		}
		else
		{
			ResetOtherPanel(totalCardFlags);
		}
		ResetViewObjectsActive(cardData.IsEquipment(), cardData.IsShowIncrease());
		cardDestroyAni.Disable();
	}

	public void EnableKeywordBg()
	{
		keywordBg.enabled = true;
	}

	public void DisableKeywordBg()
	{
		keywordBg.enabled = false;
	}

	private void ResetViewObjectsActive(bool isEquip, bool isShowIncrease)
	{
		GameObject[] array = generalObjs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!isEquip);
		}
		array = equipExtendObjs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(isEquip);
		}
		increasePanel.SetActive(isShowIncrease);
	}

	public void ResetOtherPanel(List<CardFlag> totalCardFlags)
	{
		bool flag = false;
		secCardIconImg.enabled = false;
		Sprite sprite = null;
		using (List<CardFlag>.Enumerator enumerator = totalCardFlags.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current.flag)
				{
				case CardFlagType.FirstCard:
				case CardFlagType.FirstCardDestroy:
					flag = true;
					sprite = cardBgAtlas.GetSprite(UICardMainIconType.FirstCard.ToString());
					break;
				case CardFlagType.Retain:
					flag = true;
					sprite = cardBgAtlas.GetSprite(UICardMainIconType.Retain.ToString());
					break;
				case CardFlagType.NoCostActOnHit:
					flag = true;
					sprite = cardBgAtlas.GetSprite(UICardMainIconType.NoCostActOnHit.ToString());
					break;
				}
				if (flag)
				{
					secCardIconImg.sprite = sprite;
					secCardIconImg.enabled = true;
					break;
				}
			}
		}
		otherPanel.SetActive(flag);
	}

	private void SetCardBg(CardType cardType, CardRare cardRare)
	{
		UICardItemBorderType uICardItemBorderType = UICardItemBorderType.UICardBorder;
		rareImg.enabled = true;
		UICardItemBgType uICardItemBgType;
		switch (cardType)
		{
		default:
			uICardItemBgType = UICardItemBgType.UICard_Action;
			break;
		case CardType.GeneralMove:
			uICardItemBgType = UICardItemBgType.UICard_GeneralMove;
			break;
		case CardType.Equipment:
			uICardItemBgType = UICardItemBgType.UICard_Equip;
			uICardItemBorderType = UICardItemBorderType.UICardBorder_Item;
			break;
		case CardType.Item:
		case CardType.Trap:
			uICardItemBgType = UICardItemBgType.UICard_Item;
			uICardItemBorderType = UICardItemBorderType.UICardBorder_Item;
			break;
		case CardType.Power:
			uICardItemBgType = UICardItemBgType.UICard_Power;
			break;
		case CardType.Status:
			uICardItemBgType = UICardItemBgType.UICard_Status;
			uICardItemBorderType = UICardItemBorderType.UICardBorder_Status;
			rareImg.enabled = false;
			break;
		case CardType.Trinket:
			uICardItemBgType = ((cardRare != CardRare.SuperRare) ? UICardItemBgType.UICard_Trinket : UICardItemBgType.UICard_Trinket_Charm);
			rareImg.enabled = false;
			uICardItemBorderType = UICardItemBorderType.UICardBorder_Trinket;
			break;
		}
		cardBg.sprite = cardBgAtlas.GetSprite(uICardItemBgType.ToString());
		keywordBg.sprite = cardBgAtlas.GetSprite(uICardItemBorderType.ToString());
		border.sprite = cardBgAtlas.GetSprite(uICardItemBorderType.ToString());
	}

	private void SetMainIcon(UICardMainIconType iconType)
	{
		Sprite sprite = cardBgAtlas.GetSprite(iconType.ToString());
		mainNumIcon.sprite = sprite;
		equipNumIcon.sprite = sprite;
		mainNumIcon.enabled = true;
		equipNumIcon.enabled = true;
		equipNum.enabled = true;
	}

	private void HideMainNumIcon()
	{
		mainNumIcon.enabled = false;
		equipNumIcon.enabled = false;
		equipNum.enabled = false;
	}

	public void UpdateCardVisual(Card card, bool isDark = false, bool isSecHand = false, ArenaInfo arenaInfo = null, CharacterBase character = null)
	{
		CardData data = card.data;
		isDarkMode = isDark;
		titleTxt.text = data.GetTitle();
		typeTxt.text = data.GetCardType();
		bool isShowColorBlind = MainController.instance.optionInfo.isShowColorBlind;
		ResetSideImage(data, card.attColor, isShowColorBlind);
		UpdateColor(data, card.attColor, isDark || isSecHand);
		UpdateEnergy(card, arenaInfo, character);
		UpdateMainNumber(card, isDark);
		UpdateEquipNumber(card);
		UpdateIncreaseNumber(card);
	}

	public void UpdateColor(CardData cardData, CardAttackColor attColor, bool isDark)
	{
		SetDefaultColor();
		CardType cardType = cardData.cardType;
		if ((uint)cardType <= 2u)
		{
			switch (attColor)
			{
			default:
				cardBg.color = GameColorData.instance.COLOR_CARD_DEF;
				break;
			case CardAttackColor.Left:
			{
				Color cOLOR_CARD_LEFT = GameColorData.instance.COLOR_CARD_LEFT;
				cardBg.color = cOLOR_CARD_LEFT;
				handSideLeftImg.color = cOLOR_CARD_LEFT;
				colorBlindImg.color = cOLOR_CARD_LEFT;
				increaseOn.color = GameColorData.instance.COLOR_INCREASE_LEFT;
				break;
			}
			case CardAttackColor.Right:
			{
				Color cOLOR_CARD_RIGHT = GameColorData.instance.COLOR_CARD_RIGHT;
				cardBg.color = cOLOR_CARD_RIGHT;
				handSideRightImg.color = cOLOR_CARD_RIGHT;
				colorBlindImg.color = cOLOR_CARD_RIGHT;
				increaseOn.color = GameColorData.instance.COLOR_INCREASE_RIGHT;
				break;
			}
			}
		}
		else
		{
			cardBg.color = Color.white;
		}
		SetTitleColor(cardData);
		SetRareColor(cardData.cardRare, attColor);
		if (isDark)
		{
			SetDarkColor();
		}
	}

	private void SetTitleColor(CardData cardData)
	{
		if (cardData.isLevelupCard || cardData.isIncreaseCard)
		{
			titleTxt.color = GameColorData.instance.COLOR_NUMBER_UP;
		}
	}

	private void SetRareColor(CardRare cardRare, CardAttackColor attColor)
	{
		switch (cardRare)
		{
		case CardRare.Rare:
			rareImg.color = GameColorData.instance.COLOR_CARD_RARE_RARE;
			return;
		case CardRare.SuperRare:
			rareImg.color = GameColorData.instance.COLOR_CARD_RARE_SUPER;
			return;
		}
		switch (attColor)
		{
		case CardAttackColor.None:
			rareImg.color = GameColorData.instance.COLOR_CARD_RARE_DEF;
			break;
		case CardAttackColor.Left:
			rareImg.color = GameColorData.instance.COLOR_CARD_RARE_DEF_LEFT;
			break;
		case CardAttackColor.Right:
			rareImg.color = GameColorData.instance.COLOR_CARD_RARE_DEF_RIGHT;
			break;
		}
	}

	private void ResetSideImage(CardData cardData, CardAttackColor attackColor, bool isShowColorBlindIcon)
	{
		handSideLeftImg.enabled = false;
		handSideRightImg.enabled = false;
		colorBlindImg.enabled = false;
		if (cardData.cardType == CardType.UseEquip)
		{
			colorBlindImg.enabled = isShowColorBlindIcon;
			switch (attackColor)
			{
			case CardAttackColor.Left:
				handSideLeftImg.enabled = true;
				colorBlindImg.transform.localScale = new Vector3(1f, 1f, 1f);
				break;
			case CardAttackColor.Right:
				handSideRightImg.enabled = true;
				colorBlindImg.transform.localScale = new Vector3(-1f, 1f, 1f);
				break;
			}
		}
	}

	private void SetDarkColor()
	{
		Image[] array = darkImages;
		foreach (Image obj in array)
		{
			obj.color = GetDarkColor(obj.color);
		}
		TextMeshProUGUI[] array2 = darkTexts;
		foreach (TextMeshProUGUI obj2 in array2)
		{
			obj2.color = GetDarkColor(obj2.color);
		}
	}

	private void SetDefaultColor()
	{
		Image[] array = darkImages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = Color.white;
		}
		TextMeshProUGUI[] array2 = darkTexts;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].color = Color.white;
		}
	}

	public Card FreeCard()
	{
		Card card = _card;
		_card = null;
		return card;
	}

	public Card GetCard()
	{
		return _card;
	}

	public void ResetAnchoredPostion()
	{
		cardTrans.anchoredPosition = Vector2.zero;
	}

	public void UpdateEnergy(Card card, ArenaInfo arenaInfo = null, CharacterBase srcChara = null)
	{
		if (card == null || card.data == null || card.data.isHidePoint)
		{
			HideEnergyNumber();
			return;
		}
		int originEnergy = card.GetOriginEnergy();
		int adjustedEnergy = card.GetAdjustedEnergy(arenaInfo, srcChara);
		SetEnergyNumber(originEnergy, adjustedEnergy, isDarkMode);
	}

	public void UpdateEnergyByOriginCard(Card card, Card originCard)
	{
		if (card == null || card.data == null || card.data.isHidePoint)
		{
			HideEnergyNumber();
			return;
		}
		int originEnergy = originCard.GetOriginEnergy();
		int originEnergy2 = card.GetOriginEnergy();
		SetEnergyNumber(originEnergy, originEnergy2, isDarkMode);
	}

	public void SetEnergyNumber(int originNum, int adjustNum, bool isDark)
	{
		Color color = Color.white;
		if (adjustNum > originNum)
		{
			color = GameColorData.instance.COLOR_NUMBER_DOWN;
		}
		else if (adjustNum < originNum)
		{
			color = GameColorData.instance.COLOR_NUMBER_UP;
		}
		if (isDark)
		{
			color = GetDarkColor(color);
		}
		pointPanel.SetActive(value: true);
		pointNum.text = adjustNum.ToString();
		pointNum.color = color;
	}

	private void HideEnergyNumber()
	{
		pointPanel.SetActive(value: false);
	}

	private void UpdateMainNumber(Card card, bool isDark)
	{
		if (!card.IsShowMainNumber())
		{
			HideMainNumber();
			return;
		}
		int originNumber = card.GetOriginNumber();
		int adjustNumber = card.GetAdjustNumber();
		Color numberColor = GetNumberColor(card, originNumber, adjustNumber, isDark);
		int multiAttackCount = card.GetMultiAttackCount();
		SetMainNumber(mainNum, adjustNumber, numberColor, multiAttackCount);
		ShowMainNumber();
	}

	public void UpdateMainNumberByEnvironment(Card card, CardNumberEnvironment env)
	{
		if (!card.IsShowMainNumber())
		{
			HideMainNumber();
			return;
		}
		int originNumber = card.GetOriginNumber();
		int adjustNumberByEnvironment = card.GetAdjustNumberByEnvironment(env);
		Color numberColor = GetNumberColor(env, card, originNumber, adjustNumberByEnvironment, isDarkMode);
		int multiAttackCount = env.GetMultiAttackCount(card);
		SetMainNumber(mainNum, adjustNumberByEnvironment, numberColor, multiAttackCount);
		ShowMainNumber();
	}

	public static void SetMainNumber(TextMeshProUGUI number, int attPoint, Color color, int multiply)
	{
		number.text = attPoint.ToString();
		if (multiply > 1)
		{
			number.text = $"{attPoint}<size=30>x{multiply}</size>";
		}
		else
		{
			number.text = $"{attPoint}";
		}
		number.color = color;
	}

	public int UpdateMainNumberByOriginCard(Card card, Card originCard)
	{
		if (card == null || card.data == null || card.data.actionMethod == null || card.data.IsEquipment())
		{
			HideMainNumber();
			return -1;
		}
		int num = 0;
		int origin = 0;
		switch (card.data.actMethodPointVisual)
		{
		case CardActMethodPointVisual.None:
			HideMainNumber();
			return -1;
		case CardActMethodPointVisual.Attack:
		case CardActMethodPointVisual.Hemorrhage:
		case CardActMethodPointVisual.OriginAttack:
			origin = originCard.GetOriginAttack();
			num = card.GetOriginAttack();
			break;
		case CardActMethodPointVisual.Defend:
			origin = originCard.GetOriginDefend();
			num = card.GetOriginDefend();
			break;
		case CardActMethodPointVisual.Move:
			origin = originCard.GetOriginMove();
			num = card.GetOriginMove();
			break;
		}
		Color numberColor = GetNumberColor(card, origin, num, isDarkMode);
		int multiAttackCount = card.GetMultiAttackCount();
		ShowMainNumber();
		SetMainNumber(mainNum, num, numberColor, multiAttackCount);
		return num;
	}

	private void ShowMainNumber()
	{
		mainNum.enabled = true;
	}

	private void HideMainNumber()
	{
		mainNum.enabled = false;
	}

	private void UpdateEquipNumber(Card card)
	{
		if (card.data.IsEquipment())
		{
			SetEquipNumber(equipNum, card, isDarkMode);
		}
	}

	public static void SetEquipNumber(TextMeshProUGUI equipNum, Card card, bool isDarkMode)
	{
		int adjust = 0;
		int origin = 0;
		switch (card.data.actMethodPointVisual)
		{
		case CardActMethodPointVisual.Attack:
			origin = card.GetOriginAttack();
			adjust = card.GetAdjustedAttack();
			break;
		case CardActMethodPointVisual.Defend:
			origin = card.GetOriginDefend();
			adjust = card.GetAdjustedDefend();
			break;
		}
		if (card.IsNeedReload())
		{
			equipNum.text = ConstValue.UICARDITEM_RELOAD_TEXT;
			equipNum.color = GameColorData.instance.COLOR_NUMBER_RELOAD;
		}
		else
		{
			Color numberColor = GetNumberColor(origin, adjust, isDarkMode);
			equipNum.text = adjust.ToString();
			equipNum.color = numberColor;
		}
	}

	public static Color GetNumberColor(CardNumberEnvironment env, Card card, int origin, int adjust, bool isDark)
	{
		if (env.IsNeedReload(card))
		{
			return GameColorData.instance.COLOR_NUMBER_RELOAD;
		}
		return GetNumberColor(card, origin, adjust, isDark);
	}

	public static Color GetNumberColor(Card card, int origin, int adjust, bool isDark)
	{
		CardActMethodPointVisual actMethodPointVisual = card.data.actMethodPointVisual;
		if ((uint)(actMethodPointVisual - 4) <= 1u)
		{
			return GameColorData.instance.COLOR_NUMBER_HEMORRHAGE;
		}
		return GetNumberColor(origin, adjust, isDark);
	}

	public static Color GetNumberColor(int origin, int adjust, bool isDark)
	{
		Color color = Color.white;
		if (adjust > origin)
		{
			color = GameColorData.instance.COLOR_NUMBER_UP;
		}
		else if (adjust < origin)
		{
			color = GameColorData.instance.COLOR_NUMBER_DOWN;
		}
		if (isDark)
		{
			color = GetDarkColor(color);
		}
		return color;
	}

	private static Color GetDarkColor(Color originColor)
	{
		originColor *= 0.5f;
		originColor.a = 1f;
		return originColor;
	}

	public void UpdateEquipNumberForLevelup(Card card, Card originCard)
	{
		if (card.data.IsEquipment())
		{
			int adjust = 0;
			int origin = 0;
			switch (card.data.actMethodPointVisual)
			{
			case CardActMethodPointVisual.Attack:
				origin = originCard.GetOriginAttack();
				adjust = card.GetAdjustedAttack();
				break;
			case CardActMethodPointVisual.Defend:
				origin = originCard.GetOriginDefend();
				adjust = card.GetAdjustedDefend();
				break;
			}
			Color numberColor = GetNumberColor(origin, adjust, isDarkMode);
			equipNum.text = adjust.ToString();
			equipNum.color = numberColor;
		}
	}

	private void UpdateIncreaseNumber(Card card)
	{
		int originIncrease = card.GetOriginIncrease();
		int adjustedIncrease = card.GetAdjustedIncrease();
		Color numberColor = GetNumberColor(originIncrease, adjustedIncrease, isDarkMode);
		increaseNum.text = adjustedIncrease.ToString();
		increaseNum.color = numberColor;
	}

	public void UpdateIncreaseNumberForLevelup(Card card, Card originCard)
	{
		int originIncrease = originCard.GetOriginIncrease();
		int adjustedIncrease = card.GetAdjustedIncrease();
		Color numberColor = GetNumberColor(originIncrease, adjustedIncrease, isDarkMode);
		increaseNum.text = adjustedIncrease.ToString();
		increaseNum.color = numberColor;
	}

	public void UpdateIncreaseOnByEnvironment(Card card, CharacterBase srcChara, ArenaInfo arenaInfo)
	{
		if (card == null || srcChara == null || arenaInfo == null)
		{
			increaseOn.enabled = false;
			return;
		}
		CharacterEquipment usingEquipment = srcChara.GetUsingEquipment(card);
		if (usingEquipment == null || usingEquipment.card == null)
		{
			increaseOn.enabled = false;
		}
		else if (usingEquipment.card.isUsingIncrease)
		{
			increaseOn.enabled = true;
		}
		else
		{
			increaseOn.enabled = false;
		}
	}

	private void SetCardImage(Sprite sprite)
	{
		if (sprite == null)
		{
			cardImg.enabled = false;
			return;
		}
		cardImg.sprite = sprite;
		cardImg.enabled = true;
		cardImg.SetNativeSize();
	}

	public void SetSelect(float addPosX = 0f, float addPosY = 40f)
	{
		isSelected = true;
		DOTween.Kill(cardTrans);
		cardTrans.DOAnchorPos(new Vector2(addPosX, addPosY), 0.3f).SetEase(Ease.OutQuart);
	}

	public void SetDeselect(bool immediate = false)
	{
		if (immediate)
		{
			isSelected = false;
			DOTween.Kill(cardTrans);
			cardTrans.anchoredPosition = Vector2.zero;
		}
		else if (isSelected)
		{
			isSelected = false;
			DOTween.Kill(cardTrans);
			cardTrans.DOAnchorPos(Vector2.zero, 0.2f).SetEase(Ease.OutQuart);
		}
	}

	public void PlayCardAni(float duration, TweenCallback onAniEnd)
	{
		DOTween.Kill(canvasGroup);
		DOTween.Kill(cardTrans);
		canvasGroup.DOFade(0f, duration);
		cardTrans.DOAnchorPosY(80f, duration).SetRelative(isRelative: true).SetEase(Ease.OutQuart)
			.OnComplete(onAniEnd);
	}

	public void PlayEnterAni(float duration = 0.6f)
	{
		DOTween.Kill(canvasGroup);
		DOTween.Kill(cardTrans);
		canvasGroup.alpha = 0f;
		canvasGroup.DOFade(1f, duration);
		cardTrans.anchoredPosition = Vector2.zero;
		cardTrans.DOAnchorPosY(100f, duration).SetEase(Ease.OutQuart).From();
	}

	public void PlayFadeOutAni(float duration = 0.5f)
	{
		DOTween.Kill(canvasGroup);
		DOTween.Kill(cardTrans);
		canvasGroup.DOFade(0f, duration);
	}

	public void PlayDestroyAni(float duration = 0.5f, TweenCallback onAniEnd = null)
	{
		DOTween.Kill(cardTrans);
		cardTrans.DOAnchorPosY(30f, duration).SetRelative(isRelative: true).SetEase(Ease.OutQuart)
			.OnComplete(onAniEnd);
		cardDestroyAni.Stop();
		cardDestroyAni.Play(duration, delegate
		{
			HideCard();
			cardDestroyAni.Disable();
			if (onAniEnd != null)
			{
				onAniEnd();
			}
		});
	}

	public void ResetDestroyAni()
	{
		cardDestroyAni.Disable();
		cardDestroyAni.Stop();
	}

	public void ShowCard()
	{
		DOTween.Kill(canvasGroup);
		canvasGroup.alpha = 1f;
	}

	public void HideCard()
	{
		DOTween.Kill(canvasGroup);
		canvasGroup.alpha = 0f;
	}

	public void ShowLightBorder()
	{
		border.enabled = true;
	}

	public void HideLightBorder()
	{
		border.enabled = false;
	}

	public void EnableInteractable()
	{
		canvasGroup.EnableInteractable();
	}

	public void DisableInteractable()
	{
		canvasGroup.DisableInteractable();
	}

	public bool GetInteractable()
	{
		return canvasGroup.interactable;
	}

	public void SetRootAnchoredPos(Vector2 pos)
	{
		(base.transform as RectTransform).anchoredPosition = pos;
	}

	public Vector2 GetRootAnchoredPos()
	{
		return (base.transform as RectTransform).anchoredPosition;
	}

	public void PlayRootAnchorPosMove(Vector2 pos, float duration)
	{
		(base.transform as RectTransform).DOAnchorPos(pos, duration).SetEase(Ease.OutQuart);
	}

	public Vector3 GetKeywordPos()
	{
		return keywordPosTrans.position;
	}

	public Vector3 GetKeywordNegaPos()
	{
		return keywordNegaPosTrans.position;
	}

	public Vector3 GetIncreasePos()
	{
		return increasePanel.transform.position;
	}

	private void SetDataOnEditor(CardData cardData)
	{
		Card card = new Card(cardData);
		SetView(cardData);
		HideLightBorder();
		EnableKeywordBg();
		UpdateCardVisual(card);
	}

	Vector3 IUIEquippable.GetUseItemPos()
	{
		return base.transform.position + useItemPosOffset;
	}

	Vector3 IUIEquippable.GetEffTextPos()
	{
		return base.transform.position;
	}

	void IUIEquippable.SetDeselect()
	{
		SetDeselect();
	}

	void IUIEquippable.PlayEnterAni()
	{
		PlayEnterAni();
	}

	public void ShowHotkey()
	{
		hotkeyText.ShowHotkey();
	}

	public void HideHotkey()
	{
		hotkeyText.HideHotkey();
	}

	public void SetHotkey(string text)
	{
		hotkeyText.SetHotkey(text);
	}
}
