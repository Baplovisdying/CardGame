// Survivor.UsingCardFlags
using System.Collections;
using System.Collections.Generic;
using MWUtil;
using Survivor;
using UnityEngine;

public class UsingCardFlags : IEnumerable<CardFlag>, IEnumerable
{
	private List<CardFlag> usingCardFlags;

	public IEnumerator<CardFlag> GetEnumerator()
	{
		for (int i = 0; i < usingCardFlags.Count; i++)
		{
			yield return usingCardFlags[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public UsingCardFlags(Card usingCard, CharacterEquipment usingEquip)
	{
		usingCardFlags = new List<CardFlag>();
		if (usingCard != null && !usingCard.data.IsEquipment())
		{
			foreach (CardFlag totalCardFlag in usingCard.GetTotalCardFlags())
			{
				usingCardFlags.Add(new CardFlag(totalCardFlag));
			}
		}
		if (usingEquip == null || usingEquip.card == null)
		{
			return;
		}
		foreach (CardFlag totalCardFlag2 in usingEquip.card.GetTotalCardFlags())
		{
			usingCardFlags.Add(new CardFlag(totalCardFlag2));
		}
	}

	public UsingCardFlags(params CardFlag[] cardFlags)
	{
		usingCardFlags = new List<CardFlag>();
		foreach (CardFlag oldContent in cardFlags)
		{
			usingCardFlags.Add(new CardFlag(oldContent));
		}
	}

	public UsingCardFlags(List<CardFlag> cardFlags)
	{
		usingCardFlags = new List<CardFlag>(cardFlags);
	}

	public void AddFlag(CardFlag cardFlag)
	{
		usingCardFlags.Add(cardFlag);
	}

	public bool HasFlag(CardFlagType flag)
	{
		if (usingCardFlags == null)
		{
			return false;
		}
		foreach (CardFlag usingCardFlag in usingCardFlags)
		{
			if (usingCardFlag != null && usingCardFlag.flag == flag)
			{
				return true;
			}
		}
		return false;
	}

	public CardFlag GetCardFlag(CardFlagType flag)
	{
		foreach (CardFlag usingCardFlag in usingCardFlags)
		{
			if (usingCardFlag.flag == flag)
			{
				return usingCardFlag;
			}
		}
		return null;
	}

	public float GetTotalValueOfFlag(CardFlagType flag)
	{
		List<CardFlag> list = usingCardFlags.FindAll((CardFlag obj) => obj.flag == flag);
		if (list == null || list.Count <= 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (CardFlag item in list)
		{
			num += item.value;
		}
		return num;
	}

	public float GetMultiplyValueOfFlag(CardFlagType flagType)
	{
		List<CardFlag> list = usingCardFlags.FindAll((CardFlag obj) => obj.flag == flagType);
		if (list == null || list.Count <= 0)
		{
			return 0f;
		}
		float num = 1f;
		foreach (CardFlag item in list)
		{
			num *= item.value;
		}
		return num;
	}

	public int GetMultiAttackCount()
	{
		int result = 1;
		if (HasFlag(CardFlagType.MultiAttack))
		{
			result = (int)GetMultiplyValueOfFlag(CardFlagType.MultiAttack);
		}
		return result;
	}

	public void ExecuteUsingCardFlagsOnFinishAction(VisualActionsCommand actCmd, DoActionResultBundle resultBundle, GamePlayController gamePlayCtrl, ref ShowSymbolTextInfos symbolTextInfos)
	{
		CharacterAction charaAct = gamePlayCtrl.charaAct;
		CharacterBase charaBase = charaAct.charaBase;
		Card usingCard = charaAct.usingCard;
		CharacterExecuteSystem exeSystem = gamePlayCtrl.executeSystem;
		CardController cardCtrl = gamePlayCtrl.cardCtrl;
		UIRootController uiRootCtrl = gamePlayCtrl.uiRootCtrl;
		ArenaInfo arenaInfo = gamePlayCtrl.arenaInfo;
		int multiAttackCount = GetMultiAttackCount();
		foreach (CardFlag usingCardFlag in usingCardFlags)
		{
			switch (usingCardFlag.flag)
			{
			case CardFlagType.AutoPlayHandCards:
			{
				Card[] cardsFromHandArrayBy = cardCtrl.GetCardsFromHandArrayBy(usingCardFlag.cardDataRef);
				Card[] array = cardsFromHandArrayBy;
				foreach (Card card in array)
				{
					PlayedCardToType playedCardToType = cardCtrl.PlayHandCard(card);
					UICardItem playedCardItem = uiRootCtrl.uiGamePlayCtrl.PlayAndRecycleCardFromHand(card, playedCardToType);
					uiRootCtrl.uiEffectLightLineCtrl.PlayPlayedCardEff(playedCardItem, playedCardToType);
				}
				resultBundle.AddPlayedCards(cardsFromHandArrayBy);
				break;
			}
			case CardFlagType.AutoDropHandCards:
			{
				Card[] cardsFromHandArray = cardCtrl.GetCardsFromHandArray();
				cardCtrl.DiscardHandCards(cardsFromHandArray);
				resultBundle.AddAutoDropedCards(cardsFromHandArray);
				break;
			}
			case CardFlagType.DropRandomHands:
			{
				List<Card> cardsFromHand = cardCtrl.GetCardsFromHand();
				if (cardsFromHand != null && cardsFromHand.Count > 0)
				{
					int count = (int)usingCardFlag.value;
					List<Card> uniqueObjs = cardsFromHand.GetUniqueObjs(count);
					if (uniqueObjs != null)
					{
						Card[] array2 = uniqueObjs.ToArray();
						cardCtrl.DiscardHandCards(array2);
						resultBundle.AddAutoDropedCards(array2);
					}
				}
				break;
			}
			case CardFlagType.AddDmgByCostDef:
				charaBase.RemoveBlockValue();
				break;
			case CardFlagType.CostHp:
			{
				int num2 = multiAttackCount * (int)usingCardFlag.value;
				if (num2 > 0)
				{
					if (charaBase.hp <= num2)
					{
						num2 = charaBase.hp - 1;
					}
					if (num2 > 0 && charaBase.TrueAttackKeefLife(num2))
					{
						actCmd.Add(exeSystem.PlayHurtSelfEffect(charaBase, num2));
						charaBase.usingCharaFlags.DoAddFlagToSelfOnHurt(1, ref symbolTextInfos);
					}
				}
				break;
			}
			case CardFlagType.CostHpByEquipDef:
			{
				CharacterEquipment usingEquipment = charaAct.usingEquipment;
				if (usingEquipment == null)
				{
					break;
				}
				int num = multiAttackCount * usingEquipment.card.GetAdjustedDefend();
				if (num > 0)
				{
					if (charaBase.hp <= num)
					{
						num = charaBase.hp - 1;
					}
					if (num > 0 && charaBase.TrueAttackKeefLife(num))
					{
						actCmd.Add(exeSystem.PlayHurtSelfEffect(charaBase, num));
						charaBase.usingCharaFlags.DoAddFlagToSelfOnHurt(1, ref symbolTextInfos);
					}
				}
				break;
			}
			case CardFlagType.ClearSelfCharaFlags:
			{
				CharacterFlagData characterFlagDataRef2 = usingCardFlag.characterFlagDataRef;
				if (characterFlagDataRef2 == null)
				{
					Debug.LogWarning("[UsingCardFlags] ClearSelfCharaFlags fail, CharaFlagType is none");
					break;
				}
				CharacterFlagInfo characterFlagInfo2 = charaBase.usingCharaFlags.RemoveFlag(characterFlagDataRef2);
				if (characterFlagInfo2 != null)
				{
					symbolTextInfos.AddInfo(charaBase, characterFlagInfo2.data, -characterFlagInfo2.amount, isForceAdd: true);
				}
				break;
			}
			case CardFlagType.ClearDstCharaFlagsOnHit:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CharacterExecuteResult charaExeResult2 = resultBundle.charaExeResult;
				if (charaExeResult2 == null || charaExeResult2.GetHitCount() <= 0)
				{
					break;
				}
				CharacterFlagData characterFlagDataRef3 = usingCardFlag.characterFlagDataRef;
				if (characterFlagDataRef3 == null)
				{
					Debug.LogWarning("[UsingCardFlags] ClearDstCharaFlagsOnHit fail, CharaFlagType is none");
					break;
				}
				foreach (CharacterUnderAttackInfo target in charaExeResult2.targetList)
				{
					if (target == null)
					{
						continue;
					}
					CharacterBase underAttacker2 = target.underAttacker;
					if (!(underAttacker2 == null))
					{
						CharacterFlagInfo characterFlagInfo3 = underAttacker2.usingCharaFlags.RemoveFlag(characterFlagDataRef3);
						if (characterFlagInfo3 != null)
						{
							symbolTextInfos.AddInfo(underAttacker2, characterFlagInfo3.data, -characterFlagInfo3.amount, isForceAdd: true);
						}
					}
				}
				break;
			}
			case CardFlagType.ClearDstCharaFlags:
			{
				CharacterExecuteResult charaExeResult = resultBundle.charaExeResult;
				if (charaExeResult == null || charaExeResult.GetHitCount() <= 0)
				{
					break;
				}
				CharacterFlagData characterFlagDataRef = usingCardFlag.characterFlagDataRef;
				if (characterFlagDataRef == null)
				{
					Debug.LogWarning("[UsingCardFlags] ClearDstCharaFlags fail, CharaFlagType is none");
					break;
				}
				foreach (CharacterUnderAttackInfo target2 in charaExeResult.targetList)
				{
					if (target2 == null)
					{
						continue;
					}
					CharacterBase underAttacker = target2.underAttacker;
					if (!(underAttacker == null))
					{
						CharacterFlagInfo characterFlagInfo = underAttacker.usingCharaFlags.RemoveFlag(characterFlagDataRef);
						if (characterFlagInfo != null)
						{
							symbolTextInfos.AddInfo(underAttacker, characterFlagInfo.data, -characterFlagInfo.amount, isForceAdd: true);
						}
					}
				}
				break;
			}
			case CardFlagType.CostCoin:
			{
				_ = arenaInfo.coinAmount;
				int value = Mathf.RoundToInt(usingCardFlag.value * (float)arenaInfo.coinAmount);
				value = arenaInfo.CostCoin(value);
				if (value > 0)
				{
					actCmd.Add(exeSystem.PlayCostCoinEffect(value, arenaInfo.coinAmount));
				}
				break;
			}
			}
		}
		Card[] addCards;
		ItemController itemCtrl;
		CardData cardData;
		foreach (CardFlag usingCardFlag2 in usingCardFlags)
		{
			switch (usingCardFlag2.flag)
			{
			case CardFlagType.HealSelf:
			{
				int addValue3 = multiAttackCount * (int)usingCardFlag2.value;
				DoAddHp(charaBase, addValue3, actCmd, exeSystem, uiRootCtrl);
				break;
			}
			case CardFlagType.AddHpByMakeDmg:
			{
				int addValue2 = Mathf.RoundToInt((float)resultBundle.charaExeResult.totalAttackDmg * usingCardFlag2.value);
				DoAddHp(charaBase, addValue2, actCmd, exeSystem, uiRootCtrl);
				break;
			}
			case CardFlagType.AddDef:
			{
				int addValue = multiAttackCount * (int)usingCardFlag2.value;
				DoAddBlock(resultBundle, charaBase, addValue, actCmd, exeSystem);
				break;
			}
			case CardFlagType.AddDefByMakeDmg:
			{
				int totalAttackDmg = resultBundle.charaExeResult.totalAttackDmg;
				DoAddBlock(resultBundle, charaBase, totalAttackDmg, actCmd, exeSystem);
				break;
			}
			case CardFlagType.AddDefByAutoDropHands:
			{
				int autoDropedCardCount2 = resultBundle.GetAutoDropedCardCount();
				int addValue4 = multiAttackCount * (int)usingCardFlag2.value * autoDropedCardCount2;
				DoAddBlock(resultBundle, charaBase, addValue4, actCmd, exeSystem);
				break;
			}
			case CardFlagType.DrawCards:
			{
				int allAmount5 = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
				int getCnt4 = multiAttackCount * (int)usingCardFlag2.value;
				DrawCardsInfo info5 = cardCtrl.DrawCards(charaBase, getCnt4, allAmount5);
				resultBundle.AddDrawCardsInfo(info5);
				break;
			}
			case CardFlagType.DrawCardsByAutoDropHands:
			{
				int allAmount4 = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
				int autoDropedCardCount = resultBundle.GetAutoDropedCardCount();
				DrawCardsInfo info4 = cardCtrl.DrawCards(charaBase, autoDropedCardCount, allAmount4);
				resultBundle.AddDrawCardsInfo(info4);
				break;
			}
			case CardFlagType.DrawCardsByHitCnt:
				if (resultBundle.IsSuccessHit())
				{
					int allAmount3 = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
					int getCnt3 = resultBundle.charaExeResult.GetHitCount() * (int)usingCardFlag2.value;
					DrawCardsInfo info3 = cardCtrl.DrawCards(charaBase, getCnt3, allAmount3);
					resultBundle.AddDrawCardsInfo(info3);
				}
				break;
			case CardFlagType.DrawCardsOnHit:
				if (resultBundle.IsSuccessHit())
				{
					int allAmount2 = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
					int getCnt2 = (int)usingCardFlag2.value;
					DrawCardsInfo info2 = cardCtrl.DrawCards(charaBase, getCnt2, allAmount2);
					resultBundle.AddDrawCardsInfo(info2);
				}
				break;
			case CardFlagType.DrawCardsByNeighbors:
			{
				int allAmount = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
				int getCnt = charaBase.GetCharacterCountOfNeighbors() * (int)usingCardFlag2.value;
				DrawCardsInfo info = cardCtrl.DrawCards(charaBase, getCnt, allAmount);
				resultBundle.AddDrawCardsInfo(info);
				break;
			}
			case CardFlagType.DrawCardsByCostHand:
			{
				CardData cardDataRef4 = usingCardFlag2.cardDataRef;
				if (!(cardDataRef4 == null))
				{
					int playedCardCount = resultBundle.GetPlayedCardCount(cardDataRef4);
					if (playedCardCount > 0)
					{
						int allAmount6 = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
						int getCnt5 = (int)usingCardFlag2.value * playedCardCount;
						DrawCardsInfo info6 = cardCtrl.DrawCards(charaBase, getCnt5, allAmount6);
						resultBundle.AddDrawCardsInfo(info6);
					}
				}
				break;
			}
			case CardFlagType.AddCharFlagByHitCnt:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CharacterFlagData characterFlagDataRef9 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef9 == null)
				{
					Debug.LogWarning("[UsingCardFlags] AddCharFlagByHitCnt fail, targetCharaFlag is none");
					break;
				}
				int num9 = resultBundle.charaExeResult.GetHitCount() * (int)usingCardFlag2.value;
				if (num9 > 0)
				{
					num9 = charaBase.usingCharaFlags.AddNewFlag(characterFlagDataRef9, num9);
					if (charaBase.usingCharaFlags.IsSuccessAddNewFlag(num9))
					{
						symbolTextInfos.AddInfo(charaBase, characterFlagDataRef9, num9);
					}
				}
				break;
			}
			case CardFlagType.Dagger:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CharacterExecuteResult charaExeResult3 = resultBundle.charaExeResult;
				if (charaExeResult3 == null || charaExeResult3.GetHitCount() <= 0)
				{
					break;
				}
				CharacterFlagData characterFlagDataRef4 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef4 == null)
				{
					Debug.LogWarning("[UsingCardFlags] Dagger fail, targetCharaFlag is none");
					break;
				}
				foreach (CharacterUnderAttackInfo target3 in charaExeResult3.targetList)
				{
					if (target3 == null)
					{
						continue;
					}
					CharacterBase underAttacker3 = target3.underAttacker;
					if (underAttacker3 == null)
					{
						continue;
					}
					int num3 = (int)usingCardFlag2.value;
					if (num3 > 0)
					{
						num3 = underAttacker3.usingCharaFlags.AddNewFlag(characterFlagDataRef4, num3, isSkipExtra: false, target3);
						if (underAttacker3.usingCharaFlags.IsSuccessAddNewFlag(num3))
						{
							symbolTextInfos.AddInfo(underAttacker3, characterFlagDataRef4, num3);
						}
					}
				}
				break;
			}
			case CardFlagType.AddTired:
			case CardFlagType.Heavy:
			{
				CardData cardDataRef3 = usingCardFlag2.cardDataRef;
				int addAmount2 = multiAttackCount;
				Card[] cards = cardCtrl.CreateNewCards(cardDataRef3, addAmount2);
				List<Card> list4 = cardCtrl.AddCardsToHands(cards);
				if (list4 != null)
				{
					resultBundle.AddNewCards(list4, null);
				}
				break;
			}
			case CardFlagType.CloneCardToDiscard:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CardData data2 = usingCard.data;
				_ = usingCard.cardInfo;
				int addAmount = multiAttackCount;
				addCards = cardCtrl.CreateNewCards(data2, addAmount, usingCard);
				cardCtrl.AddCardsToDiscardPile(addCards);
				actCmd.Add(CoroutineExtension.YieldAction(delegate
				{
					Card[] array3 = addCards;
					foreach (Card card2 in array3)
					{
						cardCtrl.PlayNewCardToPile(card2, PlayedCardToType.DiscardPile);
					}
				}));
				break;
			}
			case CardFlagType.RandomZeroEnergyDiscardToHand:
			{
				List<Card> discardPile = cardCtrl.GetDiscardPile(isExcludeUnplayable: true);
				if (discardPile == null)
				{
					break;
				}
				discardPile.RemoveAll((Card o) => o.GetAdjustedEnergyWithoutEquipment(arenaInfo, charaBase) != 0);
				discardPile.RemoveAll((Card o) => o.data.IsSameData(usingCard.data));
				if (discardPile.Count <= 0)
				{
					break;
				}
				Card randomOne3 = discardPile.GetRandomOne();
				if (randomOne3 != null)
				{
					cardCtrl.RemoveCardsFromDiscardPile(randomOne3);
					List<Card> list3 = cardCtrl.AddCardsToHands(randomOne3);
					GameObject discardPileObj = uiRootCtrl.uiGamePlayCtrl.GetDiscardPileObj();
					if (list3 != null)
					{
						resultBundle.AddNewCards(list3, discardPileObj);
					}
				}
				break;
			}
			case CardFlagType.AddTempIncreaseToEquips:
			{
				CardTempInfoAddTo tempInfoAddTo2 = usingCardFlag2.tempInfoAddTo;
				int num4 = multiAttackCount * (int)usingCardFlag2.value;
				if (num4 <= 0)
				{
					break;
				}
				foreach (Card item in cardCtrl.GetItemsFromPlayer(charaBase))
				{
					CardTempInfo cardTempInfo = item.GetCardTempInfo(tempInfoAddTo2);
					if (cardTempInfo != null)
					{
						cardTempInfo.extraIncrease += num4;
					}
				}
				break;
			}
			case CardFlagType.AddTempDefByUsedCnt:
			{
				CardTempInfoAddTo tempInfoAddTo4 = usingCardFlag2.tempInfoAddTo;
				int num15 = multiAttackCount * (int)usingCardFlag2.value;
				if (num15 > 0)
				{
					CardTempInfo cardTempInfo4 = usingCard.GetCardTempInfo(tempInfoAddTo4);
					if (cardTempInfo4 != null)
					{
						cardTempInfo4.addDefPower += num15;
					}
				}
				break;
			}
			case CardFlagType.AddTempDefToEqByUsedCnt:
			{
				CharacterEquipment usingEquipment4 = charaAct.usingEquipment;
				if (usingEquipment4 == null)
				{
					break;
				}
				CardTempInfoAddTo tempInfoAddTo3 = usingCardFlag2.tempInfoAddTo;
				int num14 = multiAttackCount * (int)usingCardFlag2.value;
				if (num14 > 0)
				{
					CardTempInfo cardTempInfo3 = usingEquipment4.card.GetCardTempInfo(tempInfoAddTo3);
					if (cardTempInfo3 != null)
					{
						cardTempInfo3.addDefPower += num14;
					}
				}
				break;
			}
			case CardFlagType.AddEnergy:
			{
				int num12 = multiAttackCount * (int)usingCardFlag2.value;
				if (num12 > 0)
				{
					exeSystem.PlayAddEnergy(arenaInfo, charaBase, num12, actCmd);
				}
				break;
			}
			case CardFlagType.AddEnergyByKill:
			{
				int killCount = resultBundle.GetKillCount();
				if (killCount > 0)
				{
					int num11 = (int)usingCardFlag2.value * killCount;
					if (num11 > 0)
					{
						exeSystem.PlayAddEnergy(arenaInfo, charaBase, num11, actCmd);
					}
				}
				break;
			}
			case CardFlagType.Reload:
			{
				CharacterEquipment usingEquipment2 = charaAct.usingEquipment;
				if (usingEquipment2 != null && charaAct.usingActMethod != null)
				{
					usingEquipment2.card.SetNeedReload();
				}
				break;
			}
			case CardFlagType.AutoReload:
				exeSystem.PlayReloadEquipmentsInHands(charaBase, actCmd);
				break;
			case CardFlagType.GetCoinsByKill:
				if (resultBundle.IsSuccessKill())
				{
					int coinAmount = arenaInfo.coinAmount;
					int generalValue = (int)usingCardFlag2.value;
					generalValue = arenaInfo.GetAddCoinByKill(generalValue);
					generalValue = arenaInfo.AddCoin(generalValue);
					if (generalValue > 0)
					{
						actCmd.Add(exeSystem.PlayGetCoinEffect(coinAmount, generalValue));
					}
				}
				break;
			case CardFlagType.GetRewardsByKill:
			{
				if (!resultBundle.IsSuccessKill() || charaBase.locationTile == null)
				{
					break;
				}
				itemCtrl = gamePlayCtrl.itemCtrl;
				int indexOfStage = gamePlayCtrl.gameProgressCtrl.currentGameProgress.indexOfStage;
				int levelOfStage = gamePlayCtrl.gameProgressCtrl.currentGameProgress.levelOfStage;
				cardData = null;
				if (RandomHelper.IsPass(30))
				{
					cardData = itemCtrl.GetThrowEquipmentOnFinishAction(indexOfStage, levelOfStage, FightType.General);
				}
				else
				{
					cardData = itemCtrl.GetThrowItemOnFinishAction(indexOfStage);
				}
				if (cardData == null)
				{
					Debug.LogWarning("[UsingCardFlags] GetRewardsByKill fail, cardData is null");
					break;
				}
				actCmd.Add(CoroutineExtension.YieldAction(delegate
				{
					Vector3 audiencePosition = itemCtrl.GetAudiencePosition();
					TileUnit randomNoItemNeighborTileNoCharFirst = charaBase.locationTile.GetRandomNoItemNeighborTileNoCharFirst();
					Card card3 = new Card(cardData);
					exeSystem.PlayThrowItemToArenaFromAudience(audiencePosition, randomNoItemNeighborTileNoCharFirst, card3);
					string randomOne4 = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.GiveReward).GetRandomOne();
					Color mURMUR_GOOD = GameColorData.instance.MURMUR_GOOD;
					uiRootCtrl.uiEffectCtrl.PlayMurmurEffect(audiencePosition, randomOne4, mURMUR_GOOD);
				}));
				break;
			}
			case CardFlagType.GetAtmosphereByKill:
				if (resultBundle.IsSuccessKill())
				{
					int amount = (int)usingCardFlag2.value;
					arenaInfo.AddAtmosphere(amount);
					gamePlayCtrl.stageCtrl.audiencesCtrl.UpdateAudiences(arenaInfo);
					resultBundle.SetPlayNicePlaySFX();
				}
				break;
			case CardFlagType.AddKeepDmgToEqByKill:
			{
				int killCount2 = resultBundle.GetKillCount();
				if (killCount2 <= 0)
				{
					break;
				}
				CharacterEquipment usingEquipment3 = charaAct.usingEquipment;
				if (usingEquipment3 != null && usingEquipment3.card != null)
				{
					int num13 = Mathf.RoundToInt(usingCardFlag2.value * (float)killCount2);
					if (num13 > 0)
					{
						usingEquipment3.card.cardInfo.AddAttPower(num13);
						string locString = StringDB_General.instance.GetLocString(StringDB_General_Keys.MSG_AddAttPower);
						string text2 = $"{usingEquipment3.card.data.GetTitle()}: {locString} +{num13}";
						actCmd.Add(gamePlayCtrl.symbolCtrl.PlayActionTextsProcess(charaBase, text2, isSub: false, null));
					}
				}
				break;
			}
			case CardFlagType.MultiplyDstCharaFlags:
			{
				CharacterExecuteResult charaExeResult4 = resultBundle.charaExeResult;
				if (charaExeResult4 == null || charaExeResult4.GetHitCount() <= 0)
				{
					break;
				}
				CharacterFlagData characterFlagDataRef10 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef10 == null)
				{
					Debug.LogWarning("[UsingCardFlags] MultiplyDstCharaFlags fail, targetCharaFlag is null");
					break;
				}
				float value4 = usingCardFlag2.value;
				if (value4 <= 0f)
				{
					Debug.LogWarning("[UsingCardFlags] MultiplyDstCharaFlags fail, cardFlag.value <= 0");
					break;
				}
				foreach (CharacterUnderAttackInfo target4 in charaExeResult4.targetList)
				{
					if (target4 == null)
					{
						continue;
					}
					CharacterBase underAttacker4 = target4.underAttacker;
					if (underAttacker4 == null)
					{
						continue;
					}
					CharacterFlagInfo flagInfo2 = underAttacker4.usingCharaFlags.GetFlagInfo(characterFlagDataRef10);
					if (flagInfo2 == null)
					{
						continue;
					}
					CharacterFlagData data = flagInfo2.data;
					int num10 = Mathf.RoundToInt((float)flagInfo2.amount * value4) - flagInfo2.amount;
					if (num10 > 0)
					{
						num10 = underAttacker4.usingCharaFlags.AddNewFlag(data, num10, isSkipExtra: true);
						if (underAttacker4.usingCharaFlags.IsSuccessAddNewFlag(num10))
						{
							symbolTextInfos.AddInfo(underAttacker4, data, num10);
						}
					}
				}
				break;
			}
			case CardFlagType.MultiplySrcCharaFlags:
			{
				CharacterFlagData characterFlagDataRef6 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef6 == null)
				{
					Debug.LogWarning("[UsingCardFlags] MultiplySrcCharaFlags fail, targetCharaFlag is null");
					break;
				}
				float value3 = usingCardFlag2.value;
				if (value3 <= 0f)
				{
					Debug.LogWarning("[UsingCardFlags] MultiplySrcCharaFlags fail, cardFlag.value <= 0");
					break;
				}
				CharacterFlagInfo flagInfo = charaBase.usingCharaFlags.GetFlagInfo(characterFlagDataRef6);
				if (flagInfo == null)
				{
					break;
				}
				int num6 = Mathf.RoundToInt((float)flagInfo.amount * value3) - flagInfo.amount;
				if (num6 > 0)
				{
					num6 = charaBase.usingCharaFlags.AddNewFlag(characterFlagDataRef6, num6, isSkipExtra: true);
					if (charaBase.usingCharaFlags.IsSuccessAddNewFlag(num6))
					{
						symbolTextInfos.AddInfo(charaBase, characterFlagDataRef6, num6);
					}
				}
				break;
			}
			case CardFlagType.Loss:
			{
				CardTempInfo cardTempInfo2 = usingCard.GetCardTempInfo(CardTempInfoAddTo.Fight);
				if (cardTempInfo2 != null)
				{
					cardTempInfo2.addEnergy++;
				}
				break;
			}
			case CardFlagType.AddTempFlagToDrawByPlay:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CardData cardDataRef = usingCardFlag2.cardDataRef;
				List<Card> cardsFromDrawPile = cardCtrl.GetCardsFromDrawPile(cardDataRef, usingCard);
				if (cardsFromDrawPile == null || cardsFromDrawPile.Count <= 0)
				{
					break;
				}
				Card randomOne = cardsFromDrawPile.GetRandomOne();
				if (randomOne != null)
				{
					cardCtrl.RemoveCardsFromDrawPile(randomOne);
					List<Card> list = cardCtrl.AddCardsToHands(randomOne);
					GameObject drawPileObj = uiRootCtrl.uiGamePlayCtrl.GetDrawPileObj();
					if (list != null)
					{
						resultBundle.AddNewCards(list, drawPileObj);
					}
					CardTempInfoAddTo tempInfoAddTo = usingCardFlag2.tempInfoAddTo;
					CardFlagType tempCardFlagAddType = usingCardFlag2.tempCardFlagAddType;
					randomOne.AddCardFlagToTempInfo(tempInfoAddTo, tempCardFlagAddType);
				}
				break;
			}
			case CardFlagType.AddCardsToItemSlots:
			{
				CardData cardDataRef5 = usingCardFlag2.cardDataRef;
				if (cardDataRef5 == null)
				{
					break;
				}
				for (int k = 0; k < cardCtrl.itemPack.Count; k++)
				{
					ItemPackInfo itemPackInfo = cardCtrl.itemPack[k];
					if (itemPackInfo != null && itemPackInfo.card == null)
					{
						itemPackInfo.card = new Card(cardDataRef5);
					}
				}
				break;
			}
			case CardFlagType.KeepHalfCharaFlagOnKill:
				if (resultBundle.GetKillCount() > 0)
				{
					CharacterFlagData characterFlagDataRef15 = usingCardFlag2.characterFlagDataRef;
					if (!(characterFlagDataRef15 == null))
					{
						resultBundle.SetKeepHalfCharacterFlagOnAttack(characterFlagDataRef15);
					}
				}
				break;
			case CardFlagType.KeepFullCharaFlagOnKill:
				if (resultBundle.GetKillCount() > 0)
				{
					CharacterFlagData characterFlagDataRef14 = usingCardFlag2.characterFlagDataRef;
					if (!(characterFlagDataRef14 == null))
					{
						resultBundle.SetKeepFullCharacterFlagOnAttack(characterFlagDataRef14);
					}
				}
				break;
			case CardFlagType.KeepHalfCharaFlagOnHit:
				if (resultBundle.IsSuccessHit())
				{
					CharacterFlagData characterFlagDataRef13 = usingCardFlag2.characterFlagDataRef;
					if (!(characterFlagDataRef13 == null))
					{
						resultBundle.SetKeepHalfCharacterFlagOnAttack(characterFlagDataRef13);
					}
				}
				break;
			case CardFlagType.KeepFullCharaFlagOnHit:
				if (resultBundle.IsSuccessHit())
				{
					CharacterFlagData characterFlagDataRef12 = usingCardFlag2.characterFlagDataRef;
					if (!(characterFlagDataRef12 == null))
					{
						resultBundle.SetKeepFullCharacterFlagOnAttack(characterFlagDataRef12);
					}
				}
				break;
			case CardFlagType.IgnoreDmgByFocus:
			{
				CharacterFlagData characterFlagDataRef11 = usingCardFlag2.characterFlagDataRef;
				if (!(characterFlagDataRef11 == null))
				{
					resultBundle.SetKeepFullCharacterFlagOnAttack(characterFlagDataRef11);
				}
				break;
			}
			case CardFlagType.AddSrcCharaFlagOnMakeDmg:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CharacterFlagData characterFlagDataRef8 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef8 == null)
				{
					break;
				}
				int totalAttackDmg2 = resultBundle.charaExeResult.totalAttackDmg;
				if (totalAttackDmg2 <= 0)
				{
					break;
				}
				int num8 = Mathf.RoundToInt((float)totalAttackDmg2 * usingCardFlag2.value);
				if (num8 > 0)
				{
					num8 = charaBase.usingCharaFlags.AddNewFlag(characterFlagDataRef8, num8);
					if (charaBase.usingCharaFlags.IsSuccessAddNewFlag(num8))
					{
						symbolTextInfos.AddInfo(charaBase, characterFlagDataRef8, num8);
					}
				}
				break;
			}
			case CardFlagType.AddSrcCharFlagOnHit:
			{
				if (!resultBundle.IsSuccessHit())
				{
					break;
				}
				CharacterFlagData characterFlagDataRef7 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef7 == null)
				{
					break;
				}
				int num7 = Mathf.RoundToInt(usingCardFlag2.value);
				if (num7 > 0)
				{
					num7 = charaBase.usingCharaFlags.AddNewFlag(characterFlagDataRef7, num7);
					if (charaBase.usingCharaFlags.IsSuccessAddNewFlag(num7))
					{
						symbolTextInfos.AddInfo(charaBase, characterFlagDataRef7, num7);
					}
				}
				break;
			}
			case CardFlagType.AddSrcCharFlagByCostHand:
			{
				CardData cardDataRef2 = usingCardFlag2.cardDataRef;
				if (cardDataRef2 == null)
				{
					break;
				}
				CharacterFlagData characterFlagDataRef5 = usingCardFlag2.characterFlagDataRef;
				if (characterFlagDataRef5 == null)
				{
					break;
				}
				int num5 = resultBundle.GetPlayedCardCount(cardDataRef2) * Mathf.RoundToInt(usingCardFlag2.value);
				if (num5 > 0)
				{
					num5 = charaBase.usingCharaFlags.AddNewFlag(characterFlagDataRef5, num5);
					if (charaBase.usingCharaFlags.IsSuccessAddNewFlag(num5))
					{
						symbolTextInfos.AddInfo(charaBase, characterFlagDataRef5, num5);
					}
				}
				break;
			}
			case CardFlagType.AddTempFlagToRandomHand:
			{
				float value2 = usingCardFlag2.value;
				if (value2 <= 0f)
				{
					break;
				}
				List<Card> list2 = null;
				switch (usingCardFlag2.tempCardFlagAddType)
				{
				case CardFlagType.SetCostOne:
					list2 = cardCtrl.GetCardsFromHandByGreaterThanOne(arenaInfo, charaBase, isExcludeUnplayable: true);
					break;
				case CardFlagType.SetCostZero:
					list2 = cardCtrl.GetCardsFromHandByGreaterThanZero(arenaInfo, charaBase, isExcludeUnplayable: true);
					break;
				case CardFlagType.Retain:
					list2 = cardCtrl.GetCardsFromHandsByExcludeRetain();
					break;
				case CardFlagType.AddCostEnergy:
					list2 = cardCtrl.GetCardsFromHandExcludeUnplayable();
					break;
				default:
					Debug.LogWarning("AddTempFlagToRandomHand tempCardFlagAddType fail: " + usingCardFlag2.tempCardFlagAddType);
					break;
				}
				if (list2 == null || list2.Count <= 0)
				{
					break;
				}
				for (int j = 0; (float)j < value2; j++)
				{
					Card randomOne2 = list2.GetRandomOne();
					if (randomOne2 != null)
					{
						randomOne2.AddCardFlagToTempInfo(usingCardFlag2.tempInfoAddTo, usingCardFlag2.tempCardFlagAddType);
						list2.Remove(randomOne2);
						string text = $"{usingCard.data.GetTitle()}: {randomOne2.data.GetTitle()}";
						actCmd.Add(gamePlayCtrl.symbolCtrl.PlayActionTextsProcess(charaBase, text, isSub: false, null));
					}
				}
				break;
			}
			case CardFlagType.AddTempFlagToHands:
			{
				List<Card> cardsFromHand2 = cardCtrl.GetCardsFromHand();
				if (cardsFromHand2 == null || cardsFromHand2.Count <= 0)
				{
					break;
				}
				foreach (Card item2 in cardsFromHand2)
				{
					item2.AddCardFlagToTempInfo(usingCardFlag2.tempInfoAddTo, usingCardFlag2.tempCardFlagAddType);
				}
				break;
			}
			}
		}
	}

	private void DoAddBlock(DoActionResultBundle resultBundle, CharacterBase charaBase, int addValue, VisualActionsCommand actCmd, CharacterExecuteSystem exeSystem)
	{
		if (!charaBase.AddBlock(addValue, actCmd, exeSystem))
		{
			return;
		}
		List<AddableCharacterFlagData> list = new List<AddableCharacterFlagData>();
		charaBase.usingCharaFlags.ExecuteCharacterFlagsOnAddBlock(list);
		foreach (AddableCharacterFlagData item in list)
		{
			resultBundle.AddCharacterFlagAtAfterAction(item);
		}
	}

	public void ExecuteUsingCardFlagsOnReloadAction(VisualActionsCommand actCmd, DoActionResultBundle resultBundle, GamePlayController gamePlayCtrl, ref ShowSymbolTextInfos symbolTextInfos)
	{
		CharacterBase charaBase = gamePlayCtrl.charaAct.charaBase;
		CardController cardCtrl = gamePlayCtrl.cardCtrl;
		foreach (CardFlag usingCardFlag in usingCardFlags)
		{
			switch (usingCardFlag.flag)
			{
			case CardFlagType.DrawCardsByReload:
			{
				int allAmount = charaBase.usingCharaFlags.GetAllAmount(CharacterFlagType.AddCardOnDrawStatus);
				int getCnt = (int)usingCardFlag.value;
				DrawCardsInfo info = cardCtrl.DrawCards(charaBase, getCnt, allAmount);
				resultBundle.AddDrawCardsInfo(info);
				break;
			}
			case CardFlagType.AddSrcCharFlagByReload:
			{
				CharacterFlagData characterFlagDataRef = usingCardFlag.characterFlagDataRef;
				if (characterFlagDataRef == null)
				{
					Debug.LogWarning("[UsingCardFlags] AddSrcCharFlagByReload fail, targetCharaFlag is null");
					break;
				}
				int num = (int)usingCardFlag.value;
				if (num > 0)
				{
					num = charaBase.usingCharaFlags.AddNewFlag(characterFlagDataRef, num);
					if (charaBase.usingCharaFlags.IsSuccessAddNewFlag(num))
					{
						symbolTextInfos.AddInfo(charaBase, characterFlagDataRef, num);
					}
				}
				break;
			}
			}
		}
	}

	private void DoAddHp(CharacterBase charaBase, int addValue, VisualActionsCommand actCmd, CharacterExecuteSystem exeSystem, UIRootController uiRootCtrl)
	{
		if (addValue > 0)
		{
			addValue = charaBase.AddHp(addValue, canResurrect: true);
			actCmd.Add(CoroutineExtension.YieldAction(delegate
			{
				uiRootCtrl.uiPlayerInfoCtrl.PlayAddNumEffToHpBar(addValue);
			}));
			actCmd.Add(exeSystem.PlayHealEffect(charaBase, addValue));
		}
	}

	public static bool CanUseItemInNotFighting(Card usingCard)
	{
		if (usingCard == null)
		{
			return false;
		}
		foreach (CardFlag totalCardFlag in usingCard.GetTotalCardFlags())
		{
			if (totalCardFlag.flag == CardFlagType.HealSelf)
			{
				return true;
			}
		}
		return false;
	}

	public static bool UseHealItemOnNotFighting(Card usingCard, CharacterBase srcChara, CardController cardCtrl, out int healValue)
	{
		healValue = 0;
		if (usingCard == null)
		{
			return false;
		}
		if (srcChara == null)
		{
			return false;
		}
		if (cardCtrl == null)
		{
			return false;
		}
		foreach (CardFlag totalCardFlag in usingCard.GetTotalCardFlags())
		{
			if (totalCardFlag.flag == CardFlagType.HealSelf)
			{
				healValue = (int)totalCardFlag.value;
				healValue = srcChara.AddHp(healValue);
				if (srcChara.ContainsInEquipments(usingCard))
				{
					srcChara.RemoveCardFromEquipment(usingCard);
				}
				else if (cardCtrl.ContainsInItemPack(usingCard))
				{
					cardCtrl.RemoveCardFromItemPacks(usingCard);
				}
				return true;
			}
		}
		return false;
	}

	public static void ExecuteEquipmentCardFlagsOnPlayerRoundEnter(CampGroup campGroup, ref ShowSymbolTextInfos symbolTextInfos)
	{
		for (int i = 0; i < campGroup.characters.Count; i++)
		{
			CharacterBase characterBase = campGroup.characters[i];
			if (characterBase.isActive)
			{
				ExecuteEquipmentCardFlagsOnPlayerRoundEnter(characterBase, ref symbolTextInfos);
			}
		}
	}

	public static void ExecuteEquipmentCardFlagsOnPlayerRoundEnter(CharacterBase character, ref ShowSymbolTextInfos symbolTextInfos)
	{
		if (character == null)
		{
			return;
		}
		List<Card> equipmentUniqueCards = character.GetEquipmentUniqueCards();
		if (equipmentUniqueCards == null || equipmentUniqueCards.Count <= 0)
		{
			return;
		}
		foreach (Card item in equipmentUniqueCards)
		{
			if (item == null || item.data == null)
			{
				continue;
			}
			foreach (CardFlag cardFlag in item.data.cardFlags)
			{
				if (cardFlag.flag != CardFlagType.Katana)
				{
					continue;
				}
				CharacterFlagData characterFlagDataRef = cardFlag.characterFlagDataRef;
				if (characterFlagDataRef == null)
				{
					Debug.LogWarning("AddFlagOnPlayerRoundSt fail, characterFlagDataRef is null");
					continue;
				}
				int addAmount = (int)cardFlag.value;
				addAmount = character.usingCharaFlags.AddNewFlag(characterFlagDataRef, addAmount);
				if (character.usingCharaFlags.IsSuccessAddNewFlag(addAmount))
				{
					symbolTextInfos.AddInfo(character, characterFlagDataRef, addAmount);
				}
			}
		}
	}

	public static bool ExecuteOnDraw(DrawCardsInfo drawCardsInfo, ref ShowSymbolTextInfos symbolTextInfos, CardController cardCtrl, ArenaInfo arenaInfo, CharacterBase dstChar)
	{
		bool flag = false;
		foreach (Card drawCard in drawCardsInfo.drawCards)
		{
			foreach (CardFlag totalCardFlag in drawCard.GetTotalCardFlags())
			{
				if (totalCardFlag.flag != CardFlagType.AddTempFlagToRngHandOnDraw)
				{
					continue;
				}
				float value = totalCardFlag.value;
				if (value <= 0f)
				{
					continue;
				}
				bool isForcePlayNerfSFX = false;
				List<Card> list = null;
				switch (totalCardFlag.tempCardFlagAddType)
				{
				case CardFlagType.SetCostOne:
					list = cardCtrl.GetCardsFromHandByGreaterThanOne(arenaInfo, dstChar, isExcludeUnplayable: true);
					break;
				case CardFlagType.SetCostZero:
					list = cardCtrl.GetCardsFromHandByGreaterThanZero(arenaInfo, dstChar, isExcludeUnplayable: true);
					break;
				case CardFlagType.Retain:
					list = cardCtrl.GetCardsFromHandsByExcludeRetain();
					break;
				case CardFlagType.AddCostEnergy:
					list = cardCtrl.GetCardsFromHandExcludeUnplayable();
					isForcePlayNerfSFX = true;
					break;
				default:
					Debug.LogWarning("AddTempFlagToRngHandOnDraw tempCardFlagAddType fail: " + totalCardFlag.tempCardFlagAddType);
					break;
				}
				if (list == null || list.Count <= 0)
				{
					continue;
				}
				for (int i = 0; (float)i < value; i++)
				{
					Card randomOne = list.GetRandomOne();
					if (randomOne != null)
					{
						randomOne.AddCardFlagToTempInfo(totalCardFlag.tempInfoAddTo, totalCardFlag.tempCardFlagAddType);
						list.Remove(randomOne);
						string text = $"{drawCard.data.GetTitle()}: {randomOne.data.GetTitle()}";
						symbolTextInfos.AddInfo(dstChar, text, isForcePlayNerfSFX);
					}
				}
				flag = true;
			}
		}
		DrawCardsInfo next = drawCardsInfo.next;
		if (next == null)
		{
			return flag;
		}
		bool flag2 = ExecuteOnDraw(next, ref symbolTextInfos, cardCtrl, arenaInfo, dstChar);
		if (flag)
		{
			return true;
		}
		if (flag2)
		{
			return true;
		}
		return false;
	}
}
