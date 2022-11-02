// Survivor.CardController
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MWUtil;
using Survivor;
using UnityEngine;

public class CardController : MonoBehaviour, IDebugDrawable
{
	[SerializeField]
	public CardDatabase cardDB;

	[SerializeField]
	public ItemDatabase itemDB;

	[SerializeField]
	public CardData stunCardData;

	[SerializeField]
	public CardData woundCardData;

	[SerializeField]
	public CardData lightingCardData;

	[SerializeField]
	public CardData tiredCardData;

	[SerializeField]
	private List<Card> handCards = new List<Card>();

	[SerializeField]
	private List<Card> drawPile = new List<Card>();

	[SerializeField]
	private List<Card> discardPile = new List<Card>();

	[SerializeField]
	private List<Card> exhaustPile = new List<Card>();

	[SerializeField]
	private List<Card> deckCards = new List<Card>();

	[NonSerialized]
	public List<ItemPackInfo> itemPack = new List<ItemPackInfo>();

	[SerializeField]
	public CardData tuteDefCard;

	private GamePlayController gamePlayCtrl;

	private UIGamePlayController uiGamePlayCtrl;

	public CardData addCardEditor;

	public int itemPackLimit { get; private set; } = 3;


	public int drawPileCount => drawPile.Count;

	public int discardPileCount => discardPile.Count;

	public int exhaustPileCount => exhaustPile.Count;

	public int deckCardCount => deckCards.Count;

	public int handCardCount => handCards.Count;

	string IDebugDrawable.tab => "Card";

	public void TestAddCard()
	{
		Card card = new Card(addCardEditor);
		AddCardInfos addCardInfos = new AddCardInfos(AddCardsToHands(card));
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		StartCoroutine(gamePlayCtrl.PlayAddCardsToHandProcess(addCardInfos, isFull: false, hero));
	}

	public void TestDrawCard(int drawCnt)
	{
		CharacterBase hero = gamePlayCtrl.charaCtrl.GetHero();
		DrawCardsInfo info = DrawCards(hero, drawCnt, 0);
		StartCoroutine(gamePlayCtrl.PlayDrawCardsToHandProcess(info, gamePlayCtrl.charaAct.charaBase));
	}

	private void SortHandCards()
	{
		uiGamePlayCtrl.SortHandCards();
	}

	private void RandomSortDeckCards()
	{
		deckCards.Sort((Card a, Card b) => UnityEngine.Random.Range(-1, 2));
	}

	public void Init(GamePlayController gamePlayCtrl, GameProgress gp, StartCharacterSetting characterSetting, bool isDebugQuickFight)
	{
		this.gamePlayCtrl = gamePlayCtrl;
		uiGamePlayCtrl = gamePlayCtrl.uiGamePlayCtrl;
		if (gp.status == GameProgressStatus.New)
		{
			List<Card> list = characterSetting.GetDeckCards();
			for (int i = 0; i < list.Count; i++)
			{
				AddCardsToDeck(list[i]);
			}
			LoadItemPackFromCharaSetting(characterSetting);
			if (!isDebugQuickFight)
			{
				itemPackLimit = MainController.instance.unlockedInfo.itemPackLimit;
				ResetItemPack(itemPackLimit);
			}
		}
		else
		{
			LoadDeckFromGamePorgress(gp);
			LoadItemPackFromGameProgress(gp);
		}
	}

	private void LoadDeckFromGamePorgress(GameProgress gp)
	{
		List<Card> addCards = gp.GetDeckCards(cardDB);
		AddCardsToDeck(addCards);
	}

	public void SetItemPackLimit(int value)
	{
		itemPackLimit = value;
		if (itemPackLimit <= 0)
		{
			itemPackLimit = 0;
		}
		if (itemPackLimit >= ConstValue.ITEMPACK_MAX)
		{
			itemPackLimit = ConstValue.ITEMPACK_MAX;
		}
	}

	public List<Card> ResetItemPack(CharacterBase theHero)
	{
		int num = itemPackLimit;
		int totalValueOfFlagInfos = theHero.usingCharaFlags.GetTotalValueOfFlagInfos(CharacterFlagType.SubItemPackLimit);
		if (totalValueOfFlagInfos != 0)
		{
			num -= totalValueOfFlagInfos;
		}
		return ResetItemPack(num);
	}

	private List<Card> ResetItemPack(int newItemPackLimit)
	{
		if (newItemPackLimit == itemPack.Count)
		{
			return null;
		}
		if (newItemPackLimit < 0)
		{
			newItemPackLimit = 0;
		}
		List<ItemPackInfo> list = new List<ItemPackInfo>();
		int num = 0;
		for (int i = 0; i < newItemPackLimit; i++)
		{
			ItemPackInfo itemPackInfo = null;
			while (num >= 0 && num < itemPack.Count)
			{
				itemPackInfo = itemPack[num];
				num++;
				if (itemPackInfo != null)
				{
					break;
				}
			}
			if (itemPackInfo == null)
			{
				itemPackInfo = new ItemPackInfo();
			}
			list.Add(itemPackInfo);
		}
		List<Card> list2 = new List<Card>();
		for (int j = num; j < itemPack.Count; j++)
		{
			ItemPackInfo itemPackInfo2 = itemPack[j];
			if (itemPackInfo2 != null && itemPackInfo2.card != null)
			{
				list2.Add(itemPackInfo2.card);
			}
		}
		itemPack = list;
		return list2;
	}

	private void LoadItemPackFromGameProgress(GameProgress gp)
	{
		itemPack.Clear();
		Card[] itemPackCardsFromDB = gp.GetItemPackCardsFromDB(itemDB);
		for (int i = 0; i < itemPackCardsFromDB.Length; i++)
		{
			ItemPackInfo itemPackInfo = new ItemPackInfo();
			itemPack.Add(itemPackInfo);
			itemPackInfo.card = itemPackCardsFromDB[i];
		}
		itemPackLimit = gp.itemPackLimit;
	}

	private void LoadItemPackFromCharaSetting(StartCharacterSetting characterSetting)
	{
		itemPack.Clear();
		for (int i = 0; i < characterSetting.itemPack.Count; i++)
		{
			ItemPackInfo itemPackInfo = new ItemPackInfo();
			itemPack.Add(itemPackInfo);
			CardData cardData = characterSetting.itemPack[i];
			if (!(cardData == null))
			{
				itemPackInfo.card = new Card(cardData);
			}
		}
		itemPackLimit = itemPack.Count;
	}

	public void Free()
	{
		handCards = null;
		drawPile = null;
		discardPile = null;
		exhaustPile = null;
		deckCards = null;
	}

	public void ResetAllPilesOnEndFight()
	{
		handCards.Clear();
		drawPile.Clear();
		discardPile.Clear();
		exhaustPile.Clear();
	}

	public void ResetAllPilesOnStartFight(CharacterBase character)
	{
		handCards.Clear();
		drawPile.Clear();
		discardPile.Clear();
		exhaustPile.Clear();
		List<Card> list = new List<Card>();
		List<Card> list2 = new List<Card>();
		foreach (Card deckCard in deckCards)
		{
			if (deckCard.data.HasAnyCardFlag(CardFlagType.Innate))
			{
				list.Add(deckCard);
			}
			else
			{
				list2.Add(deckCard);
			}
		}
		list = list.ResortByRandom();
		list2 = list2.ResortByRandom();
		foreach (Card item in list)
		{
			AddCardToPile(drawPile, item);
		}
		foreach (Card item2 in list2)
		{
			AddCardToPile(drawPile, item2);
		}
		foreach (Card item3 in drawPile)
		{
			item3.ResetAllTempInfos();
		}
		foreach (Card equipmentUniqueCard in character.GetEquipmentUniqueCards())
		{
			equipmentUniqueCard.ResetAllTempInfos();
		}
		foreach (ItemPackInfo item4 in itemPack)
		{
			if (item4.card != null)
			{
				item4.card.ResetAllTempInfos();
			}
		}
	}

	private bool AddCardToPile(List<Card> pile, Card card)
	{
		if (pile.Contains(card))
		{
			Debug.LogErrorFormat("[CardCtrl] AddCardToPile ERROR, still has some card[{0}] in pile: ", card.title);
			return false;
		}
		pile.Add(card);
		return true;
	}

	private bool InsertCardToPile(List<Card> pile, Card card, int index)
	{
		if (pile.Contains(card))
		{
			Debug.LogErrorFormat("[CardCtrl] InsertCardToPileTop ERROR, still has some card[{0}] in pile: ", card.title);
			return false;
		}
		pile.Insert(index, card);
		return true;
	}

	private bool RemoveCardFromPile(List<Card> pile, Card card)
	{
		if (!pile.Contains(card))
		{
			Debug.LogErrorFormat("[CardCtrl] RemoveCardFromPile ERROR, no find Card[{0}] in pile: ", card.title);
			return false;
		}
		pile.Remove(card);
		return true;
	}

	public void RemoveCardsFromDeck(params Card[] removeCards)
	{
		foreach (Card card in removeCards)
		{
			RemoveCardFromPile(deckCards, card);
		}
	}

	public void RemoveCardsFromExhaustPile(params Card[] removeCards)
	{
		foreach (Card card in removeCards)
		{
			RemoveCardFromPile(exhaustPile, card);
		}
	}

	public void RemoveCardsFromDiscardPile(params Card[] removeCards)
	{
		foreach (Card card in removeCards)
		{
			RemoveCardFromPile(discardPile, card);
		}
	}

	public void RemoveCardsFromDrawPile(params Card[] removeCards)
	{
		foreach (Card card in removeCards)
		{
			RemoveCardFromPile(drawPile, card);
		}
	}

	public bool HasAnyCanPlayCard(ArenaInfo arenaInfo, CharacterBase srcChar)
	{
		foreach (Card handCard in handCards)
		{
			if (!handCard.data.IsItem() && handCard.HasEnoughEnergyToPlay(arenaInfo, srcChar))
			{
				return true;
			}
		}
		return false;
	}

	public List<Card> GetCardsFromDrawPile(CardData cardDataRef, Card excludeCard = null)
	{
		List<Card> list = new List<Card>();
		foreach (Card item in drawPile)
		{
			if (item != null && !(item.data == null) && item.data.IsSameData(cardDataRef) && (excludeCard == null || item != excludeCard))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<Card> GetCardsFromDiscardPile(CardFlagType cardFlag, Card excludeCard = null)
	{
		List<Card> list = new List<Card>();
		foreach (Card item in discardPile)
		{
			if (item != null && !(item.data == null) && item.data.HasAnyCardFlag(cardFlag) && (excludeCard == null || item != excludeCard))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public DrawCardsInfo DrawCards(CharacterBase theHero, int getCnt, int drawCardsOnGotStatus, bool isResetDrawPile = true)
	{
		bool isDisableDraw = theHero.usingCharaFlags.HasFlag(CharacterFlagType.DisableDraw);
		return DrawCards(isDisableDraw, getCnt, drawCardsOnGotStatus, isResetDrawPile);
	}

	public DrawCardsInfo DrawCards(bool isDisableDraw, int getCnt, int drawCardsOnGotStatus, bool isResetDrawPile)
	{
		DrawCardsInfo drawCardsInfo = new DrawCardsInfo();
		if (isDisableDraw)
		{
			drawCardsInfo.isDisableDraw = true;
			return drawCardsInfo;
		}
		if (getCnt <= 0)
		{
			return drawCardsInfo;
		}
		int handLimit = gamePlayCtrl.arenaInfo.handLimit;
		int num = getCnt;
		int count = handCards.Count;
		if (count + getCnt > handLimit)
		{
			num = handLimit - count;
			drawCardsInfo.isFull = true;
		}
		for (int i = 0; i < num; i++)
		{
			Card card = DrawSingleCard(ref drawCardsInfo, isResetDrawPile);
			if (card == null)
			{
				drawCardsInfo.isFull = false;
			}
			else
			{
				drawCardsInfo.drawCards.Add(card);
			}
		}
		if (drawCardsOnGotStatus > 0)
		{
			int num2 = drawCardsInfo.drawCards.Count((Card o) => o.data.cardType == CardType.Status);
			num2 *= drawCardsOnGotStatus;
			if (num2 > 0)
			{
				DrawCardsInfo drawCardsInfo2 = (drawCardsInfo.next = DrawCards(isDisableDraw, num2, drawCardsOnGotStatus, isResetDrawPile));
			}
		}
		return drawCardsInfo;
	}

	private Card DrawSingleCard(ref DrawCardsInfo drawCardsInfo, bool isResetDrawPile)
	{
		if (drawPile.Count <= 0)
		{
			if (!isResetDrawPile)
			{
				return null;
			}
			drawCardsInfo.isResetDrawPile = true;
			drawCardsInfo.resetDrawPileCardCnt = ResetDiscardPileToDrawPile();
			if (drawPile.Count <= 0)
			{
				Debug.Log("[CardCtrl] DrawSingleCard, card is null");
				return null;
			}
		}
		Card card = drawPile[0];
		RemoveCardFromPile(drawPile, card);
		AddCardToPile(handCards, card);
		return card;
	}

	private int ResetDiscardPileToDrawPile()
	{
		List<Card> list = new List<Card>();
		int count = discardPile.Count;
		for (int num = discardPile.Count - 1; num >= 0; num--)
		{
			Card card = discardPile[num];
			RemoveCardFromPile(discardPile, card);
			list.Add(card);
		}
		list = list.ResortByRandom();
		foreach (Card item in list)
		{
			AddCardToPile(drawPile, item);
		}
		return count;
	}

	public List<Card> AddCardsToHands(params Card[] cards)
	{
		if (cards == null)
		{
			return null;
		}
		List<Card> list = new List<Card>();
		foreach (Card card in cards)
		{
			if (card == null)
			{
				Debug.LogWarning("[CardCtrl] AddNewCardsToHands, card is null");
			}
			else if (AddCardToPile(handCards, card))
			{
				list.Add(card);
			}
		}
		return list;
	}

	public VisualActionsCommand DoHurtByWoundOnRoundEnd(CharacterBase dstChar, GamePlayController gamePlayCtrl, ref ShowSymbolTextInfos showSymbolTexts)
	{
		int cardCountFromHand = GetCardCountFromHand(woundCardData);
		if (cardCountFromHand <= 0)
		{
			return null;
		}
		VisualActionsCommand visualActionsCommand = new VisualActionsCommand();
		int num = cardCountFromHand;
		gamePlayCtrl.executeSystem.ExecuteWoundInHand(visualActionsCommand, dstChar, num, cardCountFromHand);
		if (num > 0)
		{
			dstChar.usingCharaFlags.DoAddFlagToSelfOnHurt(cardCountFromHand, ref showSymbolTexts);
		}
		return visualActionsCommand;
	}

	public void DoHurtByLightingOnFinishAction(Card usingCard, CharacterBase dstChar, ref ShowSymbolTextInfos showSymbolTexts, VisualActionsCommand visualCmd)
	{
		if (usingCard == null || usingCard.data.IsItem())
		{
			return;
		}
		int cardCountFromHand = GetCardCountFromHand(lightingCardData);
		if (cardCountFromHand > 0)
		{
			int attackPower = lightingCardData.attackPower;
			gamePlayCtrl.executeSystem.ExecuteLightingInHand(visualCmd, dstChar, attackPower, cardCountFromHand);
			if (attackPower > 0)
			{
				dstChar.usingCharaFlags.DoAddFlagToSelfOnHurt(cardCountFromHand, ref showSymbolTexts);
			}
		}
	}

	public IEnumerator DiscardHandCardsOnRoundEndProcess(bool isForceRecycleAllHands, int keepHandCardsCnt = 0)
	{
		if (isForceRecycleAllHands)
		{
			DiscardHandCardsForRoundExit(handCards.ToArray());
			yield break;
		}
		List<Card> handsExcludeRetain = GetCardsFromHandsByExcludeRetain();
		if (handsExcludeRetain.Count <= 0)
		{
			yield break;
		}
		if (keepHandCardsCnt <= 0)
		{
			DiscardHandCardsForRoundExit(handsExcludeRetain.ToArray());
			yield break;
		}
		string locString = StringDB_UI.instance.GetLocString(StringDB_UI_Keys.UICardView_RetainCardOnRoundEnd);
		List<Card> selectedCards = new List<Card>();
		UIGamePlayController.UISelectHandCardsMode selectCardMode = UIGamePlayController.UISelectHandCardsMode.LESS_THAN_EQUAL;
		yield return uiGamePlayCtrl.PlaySelectHandCardsMode(locString, selectCardMode, keepHandCardsCnt, handsExcludeRetain, selectedCards);
		List<Card> list = new List<Card>();
		foreach (Card item in handsExcludeRetain)
		{
			if (item != null && !selectedCards.Contains(item))
			{
				list.Add(item);
			}
		}
		DiscardHandCardsForRoundExit(list.ToArray());
		uiGamePlayCtrl.SetHandCardViewsUnselectable();
	}

	private void DiscardHandCardsForRoundExit(Card[] discards)
	{
		if (discards != null && discards.Length != 0)
		{
			foreach (Card card in discards)
			{
				PlayedCardToType playedCardToType = DiscardHandCardForRoundExit(card);
				UICardItem playedCardItem = uiGamePlayCtrl.PlayAndRecycleCardFromHand(card, playedCardToType);
				gamePlayCtrl.uiRootCtrl.uiEffectLightLineCtrl.PlayPlayedCardEff(playedCardItem, playedCardToType);
			}
			SortHandCards();
		}
	}

	public void ExecuteHandCardsOnPlayerRoundEnd()
	{
		foreach (Card handCard in handCards)
		{
			if (handCard == null)
			{
				continue;
			}
			List<CardFlag> totalCardFlags = handCard.GetTotalCardFlags();
			if (totalCardFlags == null)
			{
				continue;
			}
			foreach (CardFlag item in totalCardFlags)
			{
				if (item != null && item.flag == CardFlagType.SubEnergyOnPlayRndEnd && handCard.cardInfo != null)
				{
					int num = (int)item.value;
					if (num > 0)
					{
						handCard.cardInfo.SetSubEnergy(handCard.data, num);
					}
				}
			}
		}
	}

	public void ExecuteRetainHandsCardFlagsOnRoundEnd()
	{
		foreach (Card handCard in handCards)
		{
			if (handCard == null)
			{
				continue;
			}
			List<CardFlag> totalCardFlags = handCard.GetTotalCardFlags();
			if (totalCardFlags == null)
			{
				continue;
			}
			foreach (CardFlag item in totalCardFlags)
			{
				if (item == null)
				{
					continue;
				}
				switch (item.flag)
				{
				case CardFlagType.AddTempDmgByRetain:
				{
					CardTempInfoAddTo tempInfoAddTo3 = item.tempInfoAddTo;
					int num3 = (int)item.value;
					if (num3 > 0)
					{
						CardTempInfo cardTempInfo3 = handCard.GetCardTempInfo(tempInfoAddTo3);
						if (cardTempInfo3 != null)
						{
							cardTempInfo3.addAttPower += num3;
						}
					}
					break;
				}
				case CardFlagType.AddTempDefByRetain:
				{
					CardTempInfoAddTo tempInfoAddTo2 = item.tempInfoAddTo;
					int num2 = (int)item.value;
					if (num2 > 0)
					{
						CardTempInfo cardTempInfo2 = handCard.GetCardTempInfo(tempInfoAddTo2);
						if (cardTempInfo2 != null)
						{
							cardTempInfo2.addDefPower += num2;
						}
					}
					break;
				}
				case CardFlagType.SubTempEnergyByRetain:
				{
					CardTempInfoAddTo tempInfoAddTo = item.tempInfoAddTo;
					int num = (int)item.value;
					if (num > 0)
					{
						CardTempInfo cardTempInfo = handCard.GetCardTempInfo(tempInfoAddTo);
						if (cardTempInfo != null)
						{
							cardTempInfo.addEnergy -= num;
						}
					}
					break;
				}
				}
			}
		}
	}

	public void AddCardsToDrawPile(params Card[] cards)
	{
		if (cards == null)
		{
			return;
		}
		foreach (Card card in cards)
		{
			if (card != null)
			{
				AddCardToPile(drawPile, card);
			}
		}
	}

	public void AddCardsToDiscardPile(params Card[] cards)
	{
		if (cards == null)
		{
			return;
		}
		foreach (Card card in cards)
		{
			if (card != null)
			{
				AddCardToPile(discardPile, card);
			}
		}
	}

	public void InsertCardToDrawPile(Card card, int index)
	{
		InsertCardToPile(drawPile, card, index);
	}

	public List<Card> GetTotalFightingCards(bool isIncludeExhaustPile = true)
	{
		List<Card> list = new List<Card>();
		list.AddRange(handCards);
		list.RemoveAll((Card card) => card.data.cardType == CardType.GeneralMove);
		list.AddRange(drawPile);
		list.AddRange(discardPile);
		if (isIncludeExhaustPile)
		{
			list.AddRange(exhaustPile);
		}
		return list;
	}

	public int GetSameColorCardCntByAttack(List<Card> cards, CardAttackColor cardAttColor)
	{
		int result = cards.Count((Card o) => o.attColor == cardAttColor && o.data.attackPower > 0 && o.data.actMethodPointVisual == CardActMethodPointVisual.Attack);
		Debug.Log("GetSameColorAttackCardCnt: " + result);
		return result;
	}

	public int GetSameColorCardCntByDefend(List<Card> cards, CardAttackColor cardAttColor)
	{
		int result = cards.Count((Card o) => o.attColor == cardAttColor && o.data.defendPower > 0 && o.data.actMethodPointVisual == CardActMethodPointVisual.Defend);
		Debug.Log("GetSameColorDefendCardCnt: " + result);
		return result;
	}

	public List<Card> GetDrawPile()
	{
		return new List<Card>(drawPile);
	}

	public List<Card> GetDiscardPile(bool isExcludeUnplayable = false)
	{
		List<Card> list = new List<Card>(discardPile);
		if (isExcludeUnplayable)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].HasCardFlag(CardFlagType.Unplayable))
				{
					list.RemoveAt(num);
				}
			}
		}
		return list;
	}

	public List<Card> GetExhaustPile()
	{
		return new List<Card>(exhaustPile);
	}

	public List<Card> GetDeckCards(params CardType[] filters)
	{
		List<Card> list = new List<Card>(deckCards);
		if (filters != null && filters.Length != 0)
		{
			list = list.Where((Card obj) => filters.Contains(obj.data.cardType)).ToList();
		}
		return list;
	}

	public Card GetRandomCardFromDeck()
	{
		return deckCards.GetRandomOne();
	}

	public List<Card> GetNotLevelupCardsFromDeck()
	{
		return new List<Card>(deckCards).Where((Card obj) => obj.data.CanLevelup()).ToList();
	}

	public List<Card> GetNotLevelupGeneralAttackCardsFromDeck()
	{
		return new List<Card>(deckCards).Where((Card obj) => obj.data.isClassGeneralCard && obj.data.CanLevelup() && obj.data.actMethodPointVisual == CardActMethodPointVisual.Attack).ToList();
	}

	public List<Card> GetNotLevelupGeneralDefendCardsFromDeck()
	{
		return new List<Card>(deckCards).Where((Card obj) => obj.data.isClassGeneralCard && obj.data.CanLevelup() && obj.data.actMethodPointVisual == CardActMethodPointVisual.Defend).ToList();
	}

	public Dictionary<CardData, int> GetTypeDictionaryByStatus(List<Card> checkCards)
	{
		Dictionary<CardData, int> dictionary = new Dictionary<CardData, int>();
		foreach (Card checkCard in checkCards)
		{
			if (checkCard != null && !(checkCard.data == null) && checkCard.data.cardType == CardType.Status)
			{
				CardData data = checkCard.data;
				if (dictionary.ContainsKey(data))
				{
					dictionary[data]++;
				}
				else
				{
					dictionary.Add(data, 1);
				}
			}
		}
		return dictionary;
	}

	public int GetTypeCountByStatusInGeneral()
	{
		return GetTypeDictionaryByStatus(deckCards).Count;
	}

	public int GetTypeCountByStatusInFight()
	{
		List<Card> totalFightingCards = GetTotalFightingCards();
		return GetTypeDictionaryByStatus(totalFightingCards).Count;
	}

	public void AddCardsToDeck(List<Card> addCards)
	{
		if (addCards == null)
		{
			Debug.LogError("[CardCtrl] AddCardsToDeck fail, addCards is null");
			return;
		}
		foreach (Card addCard in addCards)
		{
			AddCardToPile(deckCards, addCard);
		}
	}

	public void AddCardsToDeck(params Card[] addCards)
	{
		if (addCards == null)
		{
			Debug.LogError("[CardCtrl] AddCardsToDeck fail, addCards is null");
			return;
		}
		foreach (Card card in addCards)
		{
			AddCardToPile(deckCards, card);
		}
	}

	public PlayedCardToType PlayHandCard(Card card)
	{
		if (!RemoveCardFromPile(handCards, card))
		{
			return PlayedCardToType.DiscardPile;
		}
		PlayedCardToType playHandCardTo = GetPlayHandCardTo(card);
		switch (playHandCardTo)
		{
		case PlayedCardToType.ExhaustPile:
			AddCardToPile(exhaustPile, card);
			break;
		case PlayedCardToType.DiscardPile:
			AddCardToPile(discardPile, card);
			break;
		case PlayedCardToType.Destroy:
			if (deckCards.Contains(card))
			{
				Debug.Log("用盡，牌來自牌庫同時移除: " + card.data.GetTitle());
				RemoveCardFromPile(deckCards, card);
			}
			break;
		}
		return playHandCardTo;
	}

	public PlayedCardToType PlayHandCardForReload(Card card)
	{
		if (!RemoveCardFromPile(handCards, card))
		{
			return PlayedCardToType.DiscardPile;
		}
		AddCardToPile(discardPile, card);
		return PlayedCardToType.DiscardPile;
	}

	public PlayedCardToType GetPlayHandCardTo(Card card)
	{
		if (card.HasCardFlag(CardFlagType.Destroy))
		{
			return PlayedCardToType.Destroy;
		}
		if (card.HasCardFlag(CardFlagType.Exhaust))
		{
			return PlayedCardToType.ExhaustPile;
		}
		return PlayedCardToType.DiscardPile;
	}

	public PlayedCardToType DiscardHandCardForRoundExit(Card card)
	{
		if (!RemoveCardFromPile(handCards, card))
		{
			return PlayedCardToType.Destroy;
		}
		if (card.HasCardFlag(CardFlagType.AutoExhaust))
		{
			AddCardToPile(exhaustPile, card);
			return PlayedCardToType.ExhaustPile;
		}
		if (card.HasCardFlag(CardFlagType.AutoDestroy))
		{
			return PlayedCardToType.Destroy;
		}
		AddCardToPile(discardPile, card);
		return PlayedCardToType.DiscardPile;
	}

	public PlayedCardToType DiscardHandCard(Card card)
	{
		if (!RemoveCardFromPile(handCards, card))
		{
			return PlayedCardToType.Destroy;
		}
		AddCardToPile(discardPile, card);
		return PlayedCardToType.DiscardPile;
	}

	public void DiscardHandCards(Card[] discards)
	{
		if (discards != null && discards.Length != 0)
		{
			foreach (Card card in discards)
			{
				PlayedCardToType playedCardToType = DiscardHandCard(card);
				UICardItem playedCardItem = uiGamePlayCtrl.PlayAndRecycleCardFromHand(card, playedCardToType);
				gamePlayCtrl.uiRootCtrl.uiEffectLightLineCtrl.PlayPlayedCardEff(playedCardItem, playedCardToType);
			}
			SortHandCards();
		}
	}

	public PlayedCardToType ExhaustHandCard(Card card)
	{
		if (!RemoveCardFromPile(handCards, card))
		{
			return PlayedCardToType.Destroy;
		}
		AddCardToPile(exhaustPile, card);
		return PlayedCardToType.ExhaustPile;
	}

	public void ExhaustHandCards(Card[] exhausts)
	{
		if (exhausts != null && exhausts.Length != 0)
		{
			foreach (Card card in exhausts)
			{
				PlayedCardToType playedCardToType = ExhaustHandCard(card);
				UICardItem playedCardItem = uiGamePlayCtrl.PlayAndRecycleCardFromHand(card, playedCardToType);
				gamePlayCtrl.uiRootCtrl.uiEffectLightLineCtrl.PlayPlayedCardEff(playedCardItem, playedCardToType);
			}
			SortHandCards();
		}
	}

	public PlayedCardToType PlayFirstCard(Card card)
	{
		if (!RemoveCardFromPile(handCards, card))
		{
			return PlayedCardToType.Destroy;
		}
		if (card.HasCardFlag(CardFlagType.FirstCardDestroy))
		{
			return PlayedCardToType.Destroy;
		}
		AddCardToPile(discardPile, card);
		return PlayedCardToType.DiscardPile;
	}

	public List<Card> GetCardsFromHand()
	{
		return new List<Card>(handCards);
	}

	public List<Card> GetCardsFromHand(Predicate<Card> predicate)
	{
		return handCards.FindAll(predicate);
	}

	public List<Card> GetCardsFromHandByExcludeType(params CardType[] filters)
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			if (handCard != null && !filters.Contains(handCard.data.cardType))
			{
				list.Add(handCard);
			}
		}
		return list;
	}

	public List<Card> GetCardsFromHandsByExcludeRetain()
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			if (handCard != null && !handCard.HasCardFlag(CardFlagType.Retain))
			{
				list.Add(handCard);
			}
		}
		return list;
	}

	public List<Card> GetCardsFromHandExcludeUnplayable()
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			if (handCard != null && !handCard.HasCardFlag(CardFlagType.Unplayable))
			{
				list.Add(handCard);
			}
		}
		return list;
	}

	public List<Card> GetCardsFromHandByGreaterThanZero(ArenaInfo arenaInfo, CharacterBase theHero, bool isExcludeUnplayable)
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			if (handCard != null && (!isExcludeUnplayable || !handCard.HasCardFlag(CardFlagType.Unplayable)) && handCard.GetAdjustedEnergy(arenaInfo, theHero) > 0)
			{
				list.Add(handCard);
			}
		}
		return list;
	}

	public List<Card> GetCardsFromHandByGreaterThanOne(ArenaInfo arenaInfo, CharacterBase theHero, bool isExcludeUnplayable)
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			if (handCard != null && (!isExcludeUnplayable || !handCard.HasCardFlag(CardFlagType.Unplayable)) && handCard.GetAdjustedEnergy(arenaInfo, theHero) > 1)
			{
				list.Add(handCard);
			}
		}
		return list;
	}

	public Card[] GetCardsFromHandArray()
	{
		return new List<Card>(handCards).ToArray();
	}

	public Card[] GetCardsFromHandArrayBy(CardData cardData)
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			if (!(handCard.data != cardData))
			{
				list.Add(handCard);
			}
		}
		return list.ToArray();
	}

	public Card[] GetCardsFromHandArrayBy(params CardFlagType[] checkFlags)
	{
		List<Card> list = new List<Card>();
		foreach (Card handCard in handCards)
		{
			foreach (CardFlagType flagType in checkFlags)
			{
				if (handCard.HasCardFlag(flagType))
				{
					list.Add(handCard);
					break;
				}
			}
		}
		return list.ToArray();
	}

	public bool ContainsInHandCards(Card card)
	{
		return handCards.Contains(card);
	}

	public bool IsEarlyStage(int indexOfStage, int levelOfStage)
	{
		int num = 3;
		if (indexOfStage > 0)
		{
			return false;
		}
		if (levelOfStage > num)
		{
			return false;
		}
		Debug.LogFormat("IsEarlyStage[{0}][{1}] > [{2}]", indexOfStage, levelOfStage, num);
		return true;
	}

	public CardRare GetRareForGeneral(int indexOfStage, int levelOfStage)
	{
		Dictionary<CardRare, int> dictionary = new Dictionary<CardRare, int>();
		if (indexOfStage >= 2)
		{
			dictionary.Add(CardRare.Basic, 50);
			dictionary.Add(CardRare.Normal, 500);
			dictionary.Add(CardRare.Rare, 100 + levelOfStage * 10);
		}
		else if (indexOfStage >= 1)
		{
			dictionary.Add(CardRare.Basic, 300);
			dictionary.Add(CardRare.Normal, 600);
			dictionary.Add(CardRare.Rare, 50 + levelOfStage * 5);
		}
		else
		{
			dictionary.Add(CardRare.Basic, 500);
			dictionary.Add(CardRare.Normal, levelOfStage * 50);
			dictionary.Add(CardRare.Rare, levelOfStage * 5);
		}
		return CardData.GetCardDataByRate(dictionary);
	}

	private List<CardData> GetUnlocked()
	{
		return MainController.instance.unlockedInfo.GetUnlockedCards(cardDB);
	}

	public List<CardData> GetRewardCardDatas(int requireCount, int indexOfStage, int levelOfStage, bool isForceSR)
	{
		if (indexOfStage > ConstValue.MAX_ARENA_CNT)
		{
			Debug.LogWarning("[CardCtrl] GetRewardCardDatas indexOfStage error, go to check: " + indexOfStage);
		}
		List<CardData> unlocked = GetUnlocked();
		List<CardData> result = new List<CardData>();
		if (isForceSR)
		{
			CardRare[] rares = new CardRare[1] { CardRare.SuperRare };
			List<CardData> cardDatas = cardDB.GetCardDatas(rares, unlocked, null, result);
			AddCardsToRewards(requireCount, ref result, cardDatas);
		}
		else if (requireCount >= 5)
		{
			CardSubType[] subTypes = new CardSubType[3]
			{
				CardSubType.Move,
				CardSubType.Long,
				CardSubType.Draw
			};
			CardRare[] rares2 = new CardRare[2]
			{
				CardRare.Normal,
				CardRare.Rare
			};
			List<CardData> cardDatas2 = cardDB.GetCardDatas(rares2, unlocked, subTypes, result);
			AddCardsToRewards(1, ref result, cardDatas2);
			CardRare[] rares3 = new CardRare[1] { CardRare.Rare };
			List<CardData> cardDatas3 = cardDB.GetCardDatas(rares3, unlocked, null, result);
			AddCardsToRewards(1, ref result, cardDatas3);
			CardRare[] rares4 = new CardRare[1] { CardRare.SuperRare };
			List<CardData> cardDatas4 = cardDB.GetCardDatas(rares4, unlocked, null, result);
			AddCardsToRewards(1, ref result, cardDatas4);
		}
		else if (requireCount >= 3)
		{
			if (IsEarlyStage(indexOfStage, levelOfStage))
			{
				CardSubType[] subTypes2 = new CardSubType[1] { CardSubType.Short };
				CardRare[] rares5 = new CardRare[2]
				{
					CardRare.Normal,
					CardRare.Rare
				};
				List<CardData> cardDatas5 = cardDB.GetCardDatas(rares5, unlocked, subTypes2, result);
				if (AddCardsToRewards(1, ref result, cardDatas5))
				{
					Debug.Log("[特別多給短程卡]: " + result[0].name);
				}
			}
			else
			{
				CardRare cardDataByRate = CardData.GetCardDataByRate(new Dictionary<CardRare, int>
				{
					{
						CardRare.Normal,
						25
					},
					{
						CardRare.Rare,
						300 + indexOfStage * 50 + levelOfStage * 10
					},
					{
						CardRare.SuperRare,
						100 + indexOfStage * 150 + levelOfStage * 25
					}
				});
				CardRare[] rares6 = new CardRare[1] { cardDataByRate };
				List<CardData> cardDatas6 = cardDB.GetCardDatas(rares6, unlocked, null, result);
				AddCardsToRewards(1, ref result, cardDatas6);
			}
		}
		FullUpRequired(requireCount, ref result, result, CardRare.Normal, CardRare.Rare);
		result.Sort((CardData a, CardData b) => (a.cardRare <= b.cardRare) ? 1 : (-1));
		UpgradeRewardChance upgradeRewardChance = null;
		UpgradeRewardCards(upgradeChance: (indexOfStage == 1) ? gamePlayCtrl.arenaInfo.arenaData.upgradeRewardChanceStage1 : ((indexOfStage < 2) ? gamePlayCtrl.arenaInfo.arenaData.upgradeRewardChanceStage0 : gamePlayCtrl.arenaInfo.arenaData.upgradeRewardChanceStage2), rewardCards: ref result, indexOfStage: indexOfStage, levelOfStage: levelOfStage);
		bool flag = false;
		List<CardData> list = new List<CardData>();
		for (int i = 0; i < result.Count; i++)
		{
			int num = 0;
			num = (flag ? list.Count : 0);
			flag = !flag;
			list.Insert(num, result[i]);
		}
		return list;
	}

	private void UpgradeRewardCards(ref List<CardData> rewardCards, int indexOfStage, int levelOfStage, UpgradeRewardChance upgradeChance)
	{
		if (upgradeChance == null)
		{
			return;
		}
		float currentChance = 0f;
		upgradeChance.InitCurrentChance(out currentChance, levelOfStage);
		for (int i = 0; i < rewardCards.Count; i++)
		{
			CardData cardData = rewardCards[i];
			if (!(cardData == null) && cardData.CanLevelup() && !(cardData.levelupCard == null) && upgradeChance.IsUpgrade(ref currentChance, cardData))
			{
				rewardCards[i] = cardData.levelupCard;
			}
		}
	}

	public void ForceUpgradeCards(ref List<CardData> cards)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			CardData cardData = cards[i];
			if (!(cardData == null) && cardData.CanLevelup() && !(cardData.levelupCard == null))
			{
				cards[i] = cardData.levelupCard;
			}
		}
	}

	public List<CardData> GetCardDatas(int getCnt, CardRare[] cardRares)
	{
		List<CardData> unlocked = GetUnlocked();
		List<CardData> result = new List<CardData>();
		List<CardData> cardDatas = cardDB.GetCardDatas(cardRares, unlocked);
		AddCardsToRewards(getCnt, ref result, cardDatas);
		return result;
	}

	public void FetchCardDatasForLaunchStore(ref List<CardData> result, int requireCnt, List<CardData> excludes)
	{
		CardRare[] rares = new CardRare[3]
		{
			CardRare.Normal,
			CardRare.Rare,
			CardRare.SuperRare
		};
		List<CardData> unlocked = GetUnlocked();
		List<CardData> cardDatas = cardDB.GetCardDatas(rares, unlocked, null, excludes);
		AddCardsToRewards(requireCnt, ref result, cardDatas);
	}

	public List<CardData> GetStoreCardDatas(int requireCount, int indexOfStage, int levelOfStage, List<CardData> excludes)
	{
		List<CardData> unlocked = GetUnlocked();
		List<CardData> result = new List<CardData>();
		CardRare[] fullupRares = new CardRare[3]
		{
			CardRare.Normal,
			CardRare.Rare,
			CardRare.SuperRare
		};
		CardRare[] rares = new CardRare[2]
		{
			CardRare.Rare,
			CardRare.SuperRare
		};
		List<CardData> cardDatas = cardDB.GetCardDatas(rares, unlocked, null, excludes);
		AddCardsToRewards(1, ref result, cardDatas);
		excludes.AddRange(result);
		FullUpRequired(requireCount, ref result, excludes, fullupRares);
		result.Sort((CardData a, CardData b) => (a.cardRare <= b.cardRare) ? 1 : (-1));
		UpgradeRewardChance upgradeRewardChance = null;
		upgradeRewardChance = ((indexOfStage == 1) ? gamePlayCtrl.arenaInfo.arenaData.upgradeStoreChanceStage1 : ((indexOfStage < 2) ? gamePlayCtrl.arenaInfo.arenaData.upgradeStoreChanceStage0 : gamePlayCtrl.arenaInfo.arenaData.upgradeStoreChanceStage2));
		UpgradeRewardCards(ref result, indexOfStage, levelOfStage, upgradeRewardChance);
		return result;
	}

	private void FullUpRequired(int requireCount, ref List<CardData> currentDatas, List<CardData> excludes, params CardRare[] fullupRares)
	{
		int num = requireCount - currentDatas.Count;
		if (num > 0)
		{
			List<CardData> unlocked = GetUnlocked();
			List<CardData> cardDatas = cardDB.GetCardDatas(fullupRares, unlocked, null, excludes);
			AddCardsToRewards(num, ref currentDatas, cardDatas);
		}
	}

	private bool AddCardsToRewards(int cnt, ref List<CardData> result, List<CardData> sources)
	{
		if (sources == null)
		{
			return false;
		}
		if (sources.Count <= 0)
		{
			return false;
		}
		FetchCardHelper fetchCardHelper = new FetchCardHelper(sources);
		fetchCardHelper.UpdateChancesByDeck(deckCards);
		result.AddRange(fetchCardHelper.Get(cnt));
		return true;
	}

	public int GetCardCountFromDeckInFighting(CardData cardData)
	{
		return 0 + handCards.Count((Card card) => card.data.IsSameData(cardData)) + drawPile.Count((Card card) => card.data.IsSameData(cardData)) + discardPile.Count((Card card) => card.data.IsSameData(cardData));
	}

	public int GetCardCountFromHand(CardData cardData)
	{
		return handCards.Count((Card obj) => obj.data == cardData);
	}

	public bool HasAnyDefendCardsInHand()
	{
		foreach (Card handCard in handCards)
		{
			if (handCard.data == tuteDefCard)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyCardFlagCardsInHand(CardFlagType cardFlag)
	{
		foreach (Card handCard in handCards)
		{
			if (handCard.data.HasAnyCardFlag(cardFlag))
			{
				return true;
			}
		}
		return false;
	}

	public List<Card> GetItemsFromPlayer(CharacterBase theHero)
	{
		List<Card> equipmentUniqueCards = theHero.GetEquipmentUniqueCards();
		foreach (ItemPackInfo item in itemPack)
		{
			if (item != null && item.card != null && !(item.card.data == null))
			{
				equipmentUniqueCards.Add(item.card);
			}
		}
		return equipmentUniqueCards;
	}

	public bool ContainsInItemPack(Card usingCard)
	{
		return itemPack.Any((ItemPackInfo o) => o.card == usingCard);
	}

	public void RemoveCardFromItemPacks(Card usingCard)
	{
		foreach (ItemPackInfo item in itemPack)
		{
			if (item.card == usingCard)
			{
				item.card = null;
			}
		}
	}

	public Card[] CreateNewCards(CardData cardData, int addAmount, Card srcCard = null)
	{
		if (cardData == null)
		{
			Debug.LogWarning("[CardController] CreateNewCards fail, cardData is null");
			return null;
		}
		if (addAmount <= 0)
		{
			Debug.LogWarning("[UsingCardFlag] CreateNewCards fail, addAmount <= 0");
			return null;
		}
		Card[] array = new Card[addAmount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = CreateNewCard(cardData, srcCard);
		}
		return array;
	}

	public Card CreateNewCard(CardData cardData, Card srcCard = null)
	{
		if (cardData == null)
		{
			Debug.LogWarning("[CardController] CreateNewCard fail, cardData is null");
			return null;
		}
		if (srcCard != null)
		{
			Card card = new Card(cardData, srcCard.cardInfo);
			card.CloneTempInfo(srcCard);
			return card;
		}
		return new Card(cardData);
	}

	public List<Card> AddStunCardToPlayer(int addAmount)
	{
		Card[] array = new Card[addAmount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Card(stunCardData);
		}
		return AddCardsToHands(array);
	}

	public void PlayNewCardToPile(Card card, PlayedCardToType gotoPile)
	{
		if (card == null)
		{
			Debug.LogWarning("PlayNewCardToPile fail, card is null");
			return;
		}
		UICardItem tempUICardItem = gamePlayCtrl.uiRootCtrl.uiEffectCtrl.GetTempUICardItem(card);
		if (tempUICardItem == null)
		{
			Debug.LogWarning("PlayNewCardToPile fail, no free tempUICardItem");
		}
		else
		{
			StartCoroutine(PlayNewCardToDiscardPileProcess(tempUICardItem, gotoPile));
		}
	}

	private IEnumerator PlayNewCardToDiscardPileProcess(UICardItem uiCardItem, PlayedCardToType gotoPile)
	{
		UIEffectController uiEffectCtrl = gamePlayCtrl.uiRootCtrl.uiEffectCtrl;
		UIEffectLightLineController uiEffectLigitLineCtrl = gamePlayCtrl.uiRootCtrl.uiEffectLightLineCtrl;
		RectTransform obj = uiCardItem.transform as RectTransform;
		Vector2 anchoredPosition = obj.anchoredPosition;
		float num = 20f;
		anchoredPosition.x += UnityEngine.Random.Range(0f - num, num);
		anchoredPosition.y += UnityEngine.Random.Range(0f - num, num);
		obj.anchoredPosition = anchoredPosition;
		uiCardItem.PlayEnterAni(0.35f);
		yield return new WaitForSeconds(1f);
		uiCardItem.PlayFadeOutAni(0.25f);
		uiEffectLigitLineCtrl.PlayPlayedCardEff(uiCardItem, gotoPile);
		yield return new WaitForSeconds(0.25f);
		uiEffectCtrl.RecycleTempUICardItem(uiCardItem.gameObject);
	}

	private void TestAddCardToDeck(CardData cardData)
	{
		AddCardsToDeck(new Card(cardData));
	}

	private void TestRewardCardRate(int total, CardData checkData, int requireRewardCnt, int indexOfStage, int levelOfStage, bool isForceSR)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		for (int i = 0; i < total; i++)
		{
			List<CardData> rewardCardDatas = GetRewardCardDatas(requireRewardCnt, indexOfStage, levelOfStage, isForceSR);
			if (checkData != null && rewardCardDatas.Contains(checkData))
			{
				num++;
			}
			bool flag = false;
			foreach (CardData item in rewardCardDatas)
			{
				num4++;
				switch (item.cardRare)
				{
				case CardRare.Normal:
					num5++;
					break;
				case CardRare.Rare:
					num6++;
					break;
				case CardRare.SuperRare:
					num7++;
					break;
				}
				if (item.isLevelupCard)
				{
					num3++;
					if (!flag)
					{
						flag = true;
						num2++;
					}
				}
			}
		}
		Debug.LogFormat("TEST REWARD RATE: [{0}]/[{1}][{2}%],HasLvUp[{3}][{4}%], TotalLvCard[{5}], N[{6}%] R[{7}%], SR[{8}%]", num, total, (float)num / (float)total * 100f, num2, (float)num2 / (float)total * 100f, num3, (float)num5 / (float)num4 * 100f, (float)num6 / (float)num4 * 100f, (float)num7 / (float)num4 * 100f);
	}

	private void TestStoreCardRate(int total, CardData checkData, int indexOfStage, int levelOfStage)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < total; i++)
		{
			List<CardData> excludes = new List<CardData>();
			List<CardData> storeCardDatas = GetStoreCardDatas(6, indexOfStage, levelOfStage, excludes);
			if (checkData != null && storeCardDatas.Contains(checkData))
			{
				num++;
			}
			bool flag = false;
			foreach (CardData item in storeCardDatas)
			{
				if (item.isLevelupCard)
				{
					num3++;
					if (!flag)
					{
						num2++;
						flag = true;
					}
				}
			}
		}
		Debug.LogFormat("TEST STORE RATE: [{0}]/[{1}] [{2}%] HasLvUp[{3}][{4}%] TotalLvCard[{5}]", num, total, (float)num / (float)total * 100f, num2, (float)num2 / (float)total * 100f, num3);
	}

	void IDebugDrawable.DrawStaticInfomation()
	{
	}

	bool IDebugDrawable.DrawDebugContent()
	{
		if (GUILayout.Button("Show All Cards"))
		{
			List<Card> list = new List<Card>();
			foreach (CardData allData in cardDB.GetAllDatas())
			{
				Card card = new Card(allData);
				if (card.attColor != 0)
				{
					card.SetCardColor(CardAttackColor.Left);
				}
				list.Add(card);
			}
			StartCoroutine(MainController.instance.uiRootCtrl.uiDeckCtrl.Play("All Cards", "for test", "test", UIDeckMode.Read, list, null));
			return true;
		}
		foreach (CardRare value in Enum.GetValues(typeof(CardRare)))
		{
			if (!GUILayout.Button(value.ToString()))
			{
				continue;
			}
			List<Card> list2 = new List<Card>();
			foreach (CardData cardData in cardDB.GetCardDatas(new CardRare[1] { value }, null))
			{
				Card card2 = new Card(cardData);
				if (card2.attColor != 0)
				{
					card2.SetCardColor(CardAttackColor.Left);
				}
				list2.Add(card2);
			}
			StartCoroutine(MainController.instance.uiRootCtrl.uiDeckCtrl.Play(value.ToString(), "for test", "test", UIDeckMode.Read, list2, null));
			return true;
		}
		return false;
	}
}
