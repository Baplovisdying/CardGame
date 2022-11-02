// Survivor.UIDeckController
using System.Collections;
using System.Collections.Generic;
using MWUtil;
using Survivor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDeckController : MonoBehaviour
{
	public delegate void OnEnd(List<Card> selectedCard);

	[SerializeField]
	private CardDatabase cardDB;

	[SerializeField]
	private ItemDatabase itemDB;

	[SerializeField]
	private FightDatabase fightDB;

	[SerializeField]
	private StartClassDatabase classDB;

	[SerializeField]
	private GameObject viewPrefab;

	private AudioController audioCtrl;

	private UIRootController uiRootCtrl;

	private UIKeywordController uiKeywordCtrl;

	private UILevelupCardController uiLvupCardCtrl;

	private UIDeckView view;

	private GamePlayController gamePlayCtrl;

	private bool isPlaying;

	private bool isCanExit = true;

	private bool isEnableHiding;

	private UnlockedInfo unlockedInfoTemp;

	private ArenaInfo arenaInfoTemp;

	private float priceOff = 1f;

	private int requireSelectCount;

	private List<IUIEquippable> currentSelectedCardItems = new List<IUIEquippable>();

	private UIDeckMode mode;

	private UIDeckHotkeyController hotkeyCtrl = new UIDeckHotkeyController();

	public void Init(UIRootController uiRootCtrl)
	{
		audioCtrl = MainController.instance.audioCtrl;
		this.uiRootCtrl = uiRootCtrl;
		uiKeywordCtrl = uiRootCtrl.uiKeywordCtrl;
		uiLvupCardCtrl = uiRootCtrl.uiLvupCardCtrl;
	}

	public void InitOnGamePlay(GamePlayController gamePlayCtrl)
	{
		this.gamePlayCtrl = gamePlayCtrl;
	}

	public void FreeOnGamePlay()
	{
		gamePlayCtrl = null;
	}

	private bool HasGamePlayCtrl()
	{
		if (gamePlayCtrl == null)
		{
			Debug.LogWarning("[UIDeckCtrl] can play, because gamePlayCtrl is null");
			return false;
		}
		return true;
	}

	public void ShowDrawPile(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayDrawPile(onEnd));
		}
	}

	public IEnumerator PlayDrawPile(OnEnd onEnd)
	{
		List<Card> drawPile = gamePlayCtrl.cardCtrl.GetDrawPile();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DrawPile);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DrawPileSub);
		CardNumberEnvironment env = null;
		if (gamePlayCtrl.isInFighting && gamePlayCtrl.charaAct != null)
		{
			ArenaInfo arenaInfo = gamePlayCtrl.charaAct.arenaInfo;
			env = new CardNumberEnvironment(gamePlayCtrl.charaAct.charaBase, null, arenaInfo);
		}
		List<Card> selectedCards = new List<Card>();
		yield return Play(locString, locString2, string.Empty, UIDeckMode.Read, drawPile, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, env);
		onEnd?.Invoke(selectedCards);
	}

	public void ShowDiscardPile(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayDiscardPile(onEnd));
		}
	}

	public IEnumerator PlayDiscardPile(OnEnd onEnd)
	{
		List<Card> discardPile = gamePlayCtrl.cardCtrl.GetDiscardPile();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DiscardPile);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DiscardPileSub);
		CardNumberEnvironment env = null;
		if (gamePlayCtrl.isInFighting && gamePlayCtrl.charaAct != null)
		{
			ArenaInfo arenaInfo = gamePlayCtrl.charaAct.arenaInfo;
			env = new CardNumberEnvironment(gamePlayCtrl.charaAct.charaBase, null, arenaInfo);
		}
		List<Card> selectedCards = new List<Card>();
		yield return Play(locString, locString2, string.Empty, UIDeckMode.Read, discardPile, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, env);
		onEnd?.Invoke(selectedCards);
	}

	public void ShowExhaustPile(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayExhaustPile(onEnd));
		}
	}

	public IEnumerator PlayExhaustPile(OnEnd onEnd)
	{
		List<Card> exhaustPile = gamePlayCtrl.cardCtrl.GetExhaustPile();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_ExhaustPile);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_ExhaustPileSub);
		CardNumberEnvironment env = null;
		if (gamePlayCtrl.isInFighting && gamePlayCtrl.charaAct != null)
		{
			ArenaInfo arenaInfo = gamePlayCtrl.charaAct.arenaInfo;
			env = new CardNumberEnvironment(gamePlayCtrl.charaAct.charaBase, null, arenaInfo);
		}
		List<Card> selectedCards = new List<Card>();
		yield return Play(locString, locString2, string.Empty, UIDeckMode.Read, exhaustPile, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, env);
		onEnd?.Invoke(selectedCards);
	}

	public void ShowFightingCards(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayFightingCards(onEnd));
		}
	}

	public IEnumerator PlayFightingCards(OnEnd onEnd)
	{
		List<Card> totalFightingCards = gamePlayCtrl.cardCtrl.GetTotalFightingCards(isIncludeExhaustPile: false);
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DeckInFighting);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DeckInFightingSub);
		CardNumberEnvironment env = null;
		if (gamePlayCtrl.isInFighting && gamePlayCtrl.charaAct != null)
		{
			ArenaInfo arenaInfo = gamePlayCtrl.charaAct.arenaInfo;
			env = new CardNumberEnvironment(gamePlayCtrl.charaAct.charaBase, null, arenaInfo);
		}
		List<Card> selectedCards = new List<Card>();
		yield return Play(locString, locString2, string.Empty, UIDeckMode.Read, totalFightingCards, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, env);
		onEnd?.Invoke(selectedCards);
	}

	public void ShowOwnCards(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayOwnCards(onEnd));
		}
	}

	public IEnumerator PlayOwnCards(OnEnd onEnd)
	{
		CardNumberEnvironment env = new CardNumberEnvironment(null, null, gamePlayCtrl.arenaInfo);
		List<Card> deckCards = gamePlayCtrl.cardCtrl.GetDeckCards();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_Deck);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_DeckSub);
		List<Card> selectedCards = new List<Card>();
		yield return Play(locString, locString2, string.Empty, UIDeckMode.Read, deckCards, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, env);
		onEnd?.Invoke(selectedCards);
	}

	public void ShowRemoveCard(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayRemoveCard(onEnd));
		}
	}

	private IEnumerator PlayRemoveCard(OnEnd onEnd)
	{
		List<Card> selectedCards = new List<Card>();
		yield return PlayRemoveCard(selectedCards);
		onEnd?.Invoke(selectedCards);
	}

	public IEnumerator PlayRemoveCard(List<Card> selectedCards)
	{
		List<Card> deckCards = gamePlayCtrl.cardCtrl.GetDeckCards();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RemoveCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RemoveCardsSubTitle);
		string locString3 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_ConfirmBtn_Remove);
		yield return Play(locString, locString2, locString3, UIDeckMode.RemoveCard, deckCards, selectedCards);
	}

	public IEnumerator PlayRemoveCardNoExit()
	{
		List<Card> deckCards = gamePlayCtrl.cardCtrl.GetDeckCards();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RemoveCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_RemoveCardsSubTitle);
		string locString3 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_ConfirmBtn_Remove);
		yield return Play(locString, locString2, locString3, UIDeckMode.RemoveCard, deckCards, null, canExit: false);
	}

	public void ShowSwapCard(OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlaySwapCard(onEnd));
		}
	}

	private IEnumerator PlaySwapCard(OnEnd onEnd)
	{
		List<Card> selectedCards = new List<Card>();
		yield return PlaySwapCard(selectedCards);
		onEnd?.Invoke(selectedCards);
	}

	public IEnumerator PlaySwapCard(List<Card> selectedCards)
	{
		List<Card> deckCards = gamePlayCtrl.cardCtrl.GetDeckCards(CardType.UseEquip);
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SwapCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SwapCardsSubTitle);
		string locString3 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_ConfirmBtn_Swap);
		yield return Play(locString, locString2, locString3, UIDeckMode.SwapCard, deckCards, selectedCards);
	}

	public IEnumerator PlaySwapAndLvCard(List<Card> selectedCards)
	{
		List<Card> deckCards = gamePlayCtrl.cardCtrl.GetDeckCards(CardType.UseEquip);
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SwapCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SwapCardsSubTitle);
		string locString3 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_ConfirmBtn_Swap);
		yield return Play(locString, locString2, locString3, UIDeckMode.SwapAndLevelupCard, deckCards, selectedCards);
	}

	public void ShowLevelupCard()
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayLevelupCard(null));
		}
	}

	public IEnumerator PlayLevelupCard(List<Card> selectedCards)
	{
		List<Card> notLevelupCardsFromDeck = gamePlayCtrl.cardCtrl.GetNotLevelupCardsFromDeck();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_LevelupCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_LevelupCardsSubTitle);
		yield return Play(locString, locString2, string.Empty, UIDeckMode.LevelupCard, notLevelupCardsFromDeck, selectedCards);
	}

	public IEnumerator PlayLevelupCardNoExit()
	{
		List<Card> notLevelupCardsFromDeck = gamePlayCtrl.cardCtrl.GetNotLevelupCardsFromDeck();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_LevelupCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_LevelupCardsSubTitle);
		yield return Play(locString, locString2, string.Empty, UIDeckMode.LevelupCard, notLevelupCardsFromDeck, null, canExit: false);
	}

	public IEnumerator PlayCloneCardNoExit()
	{
		List<Card> deckCards = gamePlayCtrl.cardCtrl.GetDeckCards();
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CloneCardsTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CloneCardsSubTitle);
		yield return Play(locString, locString2, string.Empty, UIDeckMode.CloneCard, deckCards, null, canExit: false);
	}

	public IEnumerator PlaySelectCard(List<Card> pile, string subTitle, CardNumberEnvironment env, List<Card> selectedCards)
	{
		if (pile != null && pile.Count > 0)
		{
			string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_SelectCardTitle);
			yield return Play(locString, subTitle, string.Empty, UIDeckMode.SelectCard, pile, selectedCards, canExit: false, isAutoSort: true, isEnableHiding: true, env);
		}
	}

	public void ShowUpgradeEquipemnt(ArenaInfo arenaInfo, float priceOff, OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayUpgradeEquipemnt(arenaInfo, priceOff, onEnd));
		}
	}

	private IEnumerator PlayUpgradeEquipemnt(ArenaInfo arenaInfo, float priceOff, OnEnd onEnd)
	{
		List<Card> selectedCards = new List<Card>();
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		List<Card> itemsFromPlayer = gamePlayCtrl.cardCtrl.GetItemsFromPlayer(hero);
		itemsFromPlayer.RemoveAll((Card o) => !o.data.IsEquipment());
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_UpgradeEquipmentTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_UpgradeEquipmentSubTitle);
		yield return Play(locString, locString2, string.Empty, UIDeckMode.UpgradeEquipment, itemsFromPlayer, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, null, null, arenaInfo, priceOff);
		onEnd?.Invoke(selectedCards);
	}

	public void ShowUpgradeIncrease(ArenaInfo arenaInfo, float priceOff, OnEnd onEnd)
	{
		if (HasGamePlayCtrl())
		{
			StartCoroutine(PlayUpgradeIncrease(arenaInfo, priceOff, onEnd));
		}
	}

	private IEnumerator PlayUpgradeIncrease(ArenaInfo arenaInfo, float priceOff, OnEnd onEnd)
	{
		List<Card> selectedCards = new List<Card>();
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		List<Card> itemsFromPlayer = gamePlayCtrl.cardCtrl.GetItemsFromPlayer(hero);
		itemsFromPlayer.RemoveAll((Card o) => !o.data.HasIncreaseFunction());
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_UpgradeIncreaseTitle);
		string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_UpgradeIncreaseSubTitle);
		yield return Play(locString, locString2, string.Empty, UIDeckMode.UpgradeIncrease, itemsFromPlayer, selectedCards, canExit: true, isAutoSort: true, isEnableHiding: false, null, null, arenaInfo, priceOff);
		onEnd?.Invoke(selectedCards);
	}

	private IEnumerator PlayCollection(string title, List<Card> deck)
	{
		UnlockedInfo unlockedInfo = MainController.instance.unlockedInfo;
		int num = 0;
		bool flag = false;
		foreach (Card item in deck)
		{
			if (item.data.isLock)
			{
				flag = true;
			}
			if (!item.data.isLock || unlockedInfo.IsUnlocked(item.data))
			{
				num++;
			}
		}
		string subTitle = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CollectionSubTotal) + deck.Count;
		if (flag)
		{
			subTitle = $"{StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CollectionSubLock)} {num}/{deck.Count}";
		}
		yield return Play(title, subTitle, string.Empty, UIDeckMode.Read, deck, null, canExit: true, isAutoSort: true, isEnableHiding: false, null, unlockedInfo);
	}

	public void ShowCollectionCard()
	{
		StartCoroutine(PlayCollectionCard());
	}

	public IEnumerator PlayCollectionCard()
	{
		List<Card> list = new List<Card>();
		List<StartClassData> dataByUnlockOrder = classDB.GetDataByUnlockOrder();
		UnlockedInfo unlockedInfo = MainController.instance.unlockedInfo;
		foreach (CardData allData in cardDB.GetAllDatas())
		{
			if (allData.isHideInCollection || allData.isLevelupCard || !IsAddToDeckByCheckStartClass(allData, dataByUnlockOrder, unlockedInfo))
			{
				continue;
			}
			Card card = new Card(allData);
			if (card.attColor != 0)
			{
				if (card.data.attackPower > 0)
				{
					card.SetCardColor(CardAttackColor.Left);
				}
				else
				{
					card.SetCardColor(CardAttackColor.Right);
				}
			}
			list.Add(card);
		}
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CollectionCards);
		yield return PlayCollection(locString, list);
	}

	private bool IsAddToDeckByCheckStartClass(CardData cardData, List<StartClassData> classDatas, UnlockedInfo unlockedInfo)
	{
		if (!cardData.isClassGeneralCard)
		{
			return true;
		}
		foreach (StartClassData classData in classDatas)
		{
			if (unlockedInfo.IsUnlockedStartClass(classData) && !(classData.characterSetting == null) && classData.characterSetting.ContainInDeck(cardData))
			{
				return true;
			}
		}
		return false;
	}

	public void ShowEquipmentCollection()
	{
		StartCoroutine(PlayCollectionEquipemnt());
	}

	public IEnumerator PlayCollectionEquipemnt()
	{
		List<Card> list = new List<Card>();
		foreach (CardData allData in itemDB.GetAllDatas())
		{
			if (allData.IsEquipment() && !allData.isIncreaseCard)
			{
				Card item = new Card(allData);
				list.Add(item);
			}
		}
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CollectionEquipment);
		yield return PlayCollection(locString, list);
	}

	public void ShowItemCollection()
	{
		StartCoroutine(PlayCollectionItem());
	}

	public IEnumerator PlayCollectionItem()
	{
		List<Card> list = new List<Card>();
		foreach (CardData allData in itemDB.GetAllDatas(CardRare.Normal, CardRare.Rare, CardRare.SuperRare))
		{
			if (allData.cardType == CardType.Item)
			{
				Card item = new Card(allData);
				list.Add(item);
			}
		}
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CollectionItem);
		yield return PlayCollection(locString, list);
	}

	public void ShowTrinketCollection()
	{
		StartCoroutine(PlayCollectionTrinket());
	}

	public IEnumerator PlayCollectionTrinket()
	{
		_ = MainController.instance.unlockedInfo;
		List<Card> list = new List<Card>();
		foreach (CardData allData in itemDB.GetAllDatas())
		{
			if (allData.IsTrinket())
			{
				Card item = new Card(allData);
				list.Add(item);
			}
		}
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CollectionTrinket);
		yield return PlayCollection(locString, list);
	}

	public IEnumerator Play(string title, string subTitle, string confirmTxt, UIDeckMode mode, List<Card> deckCards, List<Card> selectedCards, bool canExit = true, bool isAutoSort = true, bool isEnableHiding = false, CardNumberEnvironment env = null, UnlockedInfo unlockedInfo = null, ArenaInfo arenaInfo = null, float priceOff = 1f)
	{
		if (!canExit && (deckCards == null || deckCards.Count <= 0))
		{
			Debug.LogWarning("[UIDeckCtrl] Play fail, no return and no deck: " + title);
			yield break;
		}
		MainController instance = MainController.instance;
		AudioSnapshot lastSnapshot = audioCtrl.currentSnapshot;
		audioCtrl.TransitionToSnapshot(AudioSnapshot.UI);
		CreateView();
		this.mode = mode;
		requireSelectCount = 1;
		currentSelectedCardItems.Clear();
		isCanExit = canExit;
		unlockedInfoTemp = unlockedInfo;
		arenaInfoTemp = arenaInfo;
		this.priceOff = priceOff;
		this.isEnableHiding = isEnableHiding;
		view.hideBtnText.text = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIDeck_CheckFight);
		if (isEnableHiding)
		{
			view.hideBtn.gameObject.SetActive(value: true);
		}
		else
		{
			view.hideBtn.gameObject.SetActive(value: false);
		}
		OptionInfo optionInfo = MainController.instance.optionInfo;
		if (optionInfo != null)
		{
			view.SetScrollSensitivity(optionInfo.scrollSensitivity);
		}
		if (isAutoSort)
		{
			deckCards.Sort(Card.CompareInDeck);
		}
		view.coinView.Hide(0f);
		if (arenaInfo != null)
		{
			view.coinView.SetCoinNumber(arenaInfo.coinAmount);
			view.coinView.Show(0f);
		}
		view.UpdateDeckView(mode, deckCards, env, unlockedInfo, arenaInfo, priceOff);
		view.HideConfirmBtn();
		if (!isCanExit)
		{
			view.HideExitBtn();
		}
		else
		{
			view.ShowExitIcon();
		}
		view.SetTitle(title);
		view.SetSubTitle(subTitle);
		if (string.IsNullOrEmpty(confirmTxt))
		{
			confirmTxt = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UI_Confirm);
		}
		view.SetConfirmBtnText(confirmTxt);
		float duration = 0.5f;
		view.Show(duration);
		view.rootCanvasGroup.DisableInteractable();
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Enter);
		hotkeyCtrl.InitHotkeyReceiver(this, "UIDeck", instance.hotkeyDB, instance.navigationInvoker);
		hotkeyCtrl.UpdateHotkeysVisible(instance.optionInfo.isShowHotkey);
		yield return new WaitForSeconds(duration);
		view.rootCanvasGroup.EnableInteractable();
		isPlaying = true;
		while (isPlaying)
		{
			yield return null;
		}
		view.rootCanvasGroup.DisableInteractable();
		audioCtrl.PlaySFX(GameSFX.UIGeneral_Exit);
		hotkeyCtrl.FreeHotkeyReceiver();
		if (selectedCards == null)
		{
			selectedCards = new List<Card>();
		}
		foreach (IUIEquippable currentSelectedCardItem in currentSelectedCardItems)
		{
			Card card = currentSelectedCardItem.GetCard();
			if (card != null)
			{
				selectedCards.Add(card);
			}
		}
		currentSelectedCardItems.Clear();
		yield return DoFinishFunction(selectedCards);
		unlockedInfoTemp = null;
		arenaInfoTemp = null;
		this.priceOff = 1f;
		view.Hide(duration);
		view.End();
		audioCtrl.TransitionToSnapshot(lastSnapshot);
	}

	private IEnumerator DoFinishFunction(List<Card> selectedCards)
	{
		foreach (Card card in selectedCards)
		{
			switch (mode)
			{
			case UIDeckMode.RemoveCard:
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.RemoveCard, card);
				gamePlayCtrl.cardCtrl.RemoveCardsFromDeck(card);
				break;
			case UIDeckMode.SwapCard:
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.SwapCard, card);
				card.SwapCardColor();
				break;
			case UIDeckMode.SwapAndLevelupCard:
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.SwapAndLevelupCard, card);
				card.SwapCardColor();
				if (card.CanLevelup())
				{
					card.LevelupCard();
				}
				break;
			case UIDeckMode.LevelupCard:
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.LevelupCard, card);
				card.LevelupCard();
				break;
			case UIDeckMode.CloneCard:
			{
				Card card4 = new Card(card);
				gamePlayCtrl.cardCtrl.AddCardsToDeck(card4);
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.GetNewCard, card4);
				break;
			}
			case UIDeckMode.UpgradeEquipment:
			{
				Card card3 = new Card(card);
				card.UpgradeEquipment();
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.Upgrade, card3, card);
				break;
			}
			case UIDeckMode.UpgradeIncrease:
			{
				Card card2 = new Card(card);
				card.UpgradeIncrease();
				yield return uiRootCtrl.uiPlayCardGotoCtrl.Play(UIPlayCardGotoMode.Upgrade, card2, card);
				break;
			}
			}
		}
	}

	private void CreateView()
	{
		if (!(view != null))
		{
			GameObject gameObject = Object.Instantiate(viewPrefab, base.transform);
			view = gameObject.GetComponent<UIDeckView>();
			view.Init(OnExitBtnClick, OnConfirmBtnClick, OnCardEnter, OnCardExit, OnDeckCardItemDown, OnDeckCardItemClick, OnDeckScrollDrag);
			view.hideBtn.SetCallbacks(OnHideBtnDown, OnHideBtnUp);
		}
	}

	private void OnHideBtnUp(GameObject obj)
	{
		if (isEnableHiding && !(view == null))
		{
			view.ResumeHiding();
		}
	}

	private void OnHideBtnDown(GameObject obj)
	{
		if (isEnableHiding && !(view == null))
		{
			view.Hiding();
		}
	}

	private void OnExitBtnClick(GameObject go, PointerEventData eventData)
	{
		DoExit();
	}

	public void DoExit()
	{
		if (isCanExit)
		{
			currentSelectedCardItems.Clear();
			isPlaying = false;
		}
	}

	private void OnCardEnter(UICardItem cardItem, PointerEventData eventData)
	{
		if (!(cardItem == null) && (unlockedInfoTemp == null || !ShowLockInfo(cardItem, unlockedInfoTemp)))
		{
			uiKeywordCtrl.ShowCard(cardItem);
		}
	}

	private bool ShowLockInfo(UICardItem cardItem, UnlockedInfo unlockedInfo)
	{
		if (cardItem == null)
		{
			return false;
		}
		if (unlockedInfo == null)
		{
			return false;
		}
		Card card = cardItem.GetCard();
		if (card == null)
		{
			return false;
		}
		CardData data = card.data;
		if (data == null)
		{
			return false;
		}
		if (!data.isLock)
		{
			return false;
		}
		if (unlockedInfo.IsUnlocked(data))
		{
			return false;
		}
		FightSetting dataByUnlockCardData = fightDB.GetDataByUnlockCardData(data);
		if (dataByUnlockCardData == null)
		{
			if (data._isLockForDemo)
			{
				Vector3 keywordPos = cardItem.GetKeywordPos();
				string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIKeyword_UnlockRuleTitle);
				string locString2 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIKeyword_DemoLockDesc);
				uiKeywordCtrl.Show(keywordPos, locString, locString2);
				return true;
			}
			Debug.LogWarning("[UIDeckCtrl] ShowLockInfo fail, has no any unlockFight found: " + data.name);
			return false;
		}
		Vector3 keywordPos2 = cardItem.GetKeywordPos();
		string locString3 = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UIKeyword_UnlockRuleTitle);
		string unlockReason = dataByUnlockCardData.GetUnlockReason();
		uiKeywordCtrl.Show(keywordPos2, locString3, unlockReason);
		return true;
	}

	private void OnCardExit(UICardItem cardItem, PointerEventData eventData)
	{
		uiKeywordCtrl.Hide();
	}

	private void OnConfirmBtnClick()
	{
		UIDeckMode uIDeckMode = mode;
		if ((uint)(uIDeckMode - 1) > 3u && uIDeckMode != UIDeckMode.CloneCard)
		{
			Debug.LogError("[UIDeckCtrl] OnConfirmBtnClick mode fail: " + mode);
		}
		else if (requireSelectCount == currentSelectedCardItems.Count)
		{
			audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
			isPlaying = false;
		}
	}

	private void OnDeckCardItemDown(UICardItem cardItem, PointerEventData eventData)
	{
		UIDeckMode uIDeckMode = mode;
		if ((uint)(uIDeckMode - 1) > 3u && uIDeckMode != UIDeckMode.CloneCard)
		{
			return;
		}
		if (requireSelectCount == 1)
		{
			audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
			for (int num = currentSelectedCardItems.Count - 1; num >= 0; num--)
			{
				IUIEquippable selectedCardItem = currentSelectedCardItems[num];
				DeselectCardItem(selectedCardItem);
			}
			SelectCardItem(cardItem);
		}
		if (requireSelectCount != currentSelectedCardItems.Count)
		{
			view.HideConfirmBtn();
		}
		else
		{
			view.ShowConfirmBtn();
		}
	}

	private bool SelectCardItem(IUIEquippable selectedCardItem)
	{
		if (selectedCardItem == null)
		{
			return false;
		}
		if (currentSelectedCardItems.Contains(selectedCardItem))
		{
			return false;
		}
		currentSelectedCardItems.Add(selectedCardItem);
		selectedCardItem.ShowLightBorder();
		return true;
	}

	private void DeselectCardItem(IUIEquippable selectedCardItem)
	{
		if (selectedCardItem != null && currentSelectedCardItems.Contains(selectedCardItem))
		{
			selectedCardItem.HideLightBorder();
			currentSelectedCardItems.Remove(selectedCardItem);
		}
	}

	private void OnDeckCardItemClick(UICardItem cardItem, PointerEventData eventData)
	{
		switch (mode)
		{
		case UIDeckMode.Read:
		{
			Card card = cardItem.GetCard();
			if (card != null && !(card.data == null) && card.CanLevelup() && !card.IsLocked(unlockedInfoTemp))
			{
				StartCoroutine(DoReadLevelupCard(card));
			}
			break;
		}
		case UIDeckMode.LevelupCard:
			audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
			StartCoroutine(DoExecuteLevelupCard(cardItem));
			break;
		case UIDeckMode.UpgradeEquipment:
			if (IsCanPriceForUpgradeEquipment(cardItem))
			{
				audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
				StartCoroutine(DoExecuteUpgradeEquipment(cardItem));
			}
			else
			{
				Vector3 position2 = cardItem.transform.position;
				uiRootCtrl.uiEffectCtrl.ShowTextEffect(position2, StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UI_NotEnoughMoney));
				audioCtrl.PlaySFX(GameSFX.UIGeneral_Error);
			}
			break;
		case UIDeckMode.UpgradeIncrease:
			if (IsCanPriceForUpgradeIncrease(cardItem))
			{
				audioCtrl.PlaySFX(GameSFX.UIGeneral_Click);
				StartCoroutine(DoExecuteUpgradeIncrease(cardItem));
			}
			else
			{
				Vector3 position = cardItem.transform.position;
				uiRootCtrl.uiEffectCtrl.ShowTextEffect(position, StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UI_NotEnoughMoney));
				audioCtrl.PlaySFX(GameSFX.UIGeneral_Error);
			}
			break;
		}
	}

	private bool IsCanPriceForUpgradeEquipment(UICardItem cardItem)
	{
		if (arenaInfoTemp == null)
		{
			return false;
		}
		Card card = cardItem.GetCard();
		if (card == null || card.data == null)
		{
			return false;
		}
		return arenaInfoTemp.coinAmount >= card.GetPriceOfUpgradeEquipment(arenaInfoTemp, priceOff);
	}

	private bool IsCanPriceForUpgradeIncrease(UICardItem cardItem)
	{
		if (arenaInfoTemp == null)
		{
			return false;
		}
		Card card = cardItem.GetCard();
		if (card == null || card.data == null)
		{
			return false;
		}
		return arenaInfoTemp.coinAmount >= card.GetPriceOfUpgradeIncrease(arenaInfoTemp, priceOff);
	}

	private IEnumerator DoReadLevelupCard(Card card)
	{
		view.rootCanvasGroup.DisableInteractable();
		yield return uiLvupCardCtrl.PlayLevelupCard(card, UILevelupCardController.UILevelupMode.ReadLevelup);
		view.rootCanvasGroup.EnableInteractable();
	}

	private IEnumerator DoExecuteLevelupCard(UICardItem cardItem)
	{
		Card card = cardItem.GetCard();
		if (card != null && !(card.data == null))
		{
			view.rootCanvasGroup.DisableInteractable();
			yield return uiLvupCardCtrl.PlayLevelupCard(card, UILevelupCardController.UILevelupMode.LevelupCard);
			if (uiLvupCardCtrl.GetResult())
			{
				currentSelectedCardItems.Clear();
				currentSelectedCardItems.Add(cardItem);
				isPlaying = false;
			}
			else
			{
				view.rootCanvasGroup.EnableInteractable();
			}
		}
	}

	private IEnumerator DoExecuteUpgradeEquipment(UICardItem cardItem)
	{
		Card card = cardItem.GetCard();
		if (card != null && !(card.data == null))
		{
			view.rootCanvasGroup.DisableInteractable();
			yield return uiLvupCardCtrl.PlayUpgradeEquipement(card, arenaInfoTemp, priceOff);
			if (uiLvupCardCtrl.GetResult())
			{
				currentSelectedCardItems.Clear();
				currentSelectedCardItems.Add(cardItem);
				isPlaying = false;
			}
			else
			{
				view.rootCanvasGroup.EnableInteractable();
			}
		}
	}

	private IEnumerator DoExecuteUpgradeIncrease(UICardItem cardItem)
	{
		Card card = cardItem.GetCard();
		if (card != null && !(card.data == null))
		{
			view.rootCanvasGroup.DisableInteractable();
			yield return uiLvupCardCtrl.PlayUpgradeIncrease(card, arenaInfoTemp, priceOff);
			if (uiLvupCardCtrl.GetResult())
			{
				currentSelectedCardItems.Clear();
				currentSelectedCardItems.Add(cardItem);
				isPlaying = false;
			}
			else
			{
				view.rootCanvasGroup.EnableInteractable();
			}
		}
	}

	private void OnDeckScrollDrag()
	{
		uiKeywordCtrl.Hide();
	}
}
