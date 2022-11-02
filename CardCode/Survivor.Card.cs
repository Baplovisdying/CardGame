// Survivor.Card
using System;
using System.Collections.Generic;
using Survivor;
using UnityEngine;

[Serializable]
public class Card
{
	private CardData dataOri;

	private CardData dataInc;

	private CardTempInfo fightTempInfo = new CardTempInfo();

	private CardTempInfo roundTempInfo = new CardTempInfo();

	private CardTempInfo actionTempInfo = new CardTempInfo();

	public CardData data
	{
		get
		{
			if (isUsingIncrease && dataInc != null)
			{
				return dataInc;
			}
			return dataOri;
		}
	}

	public bool isUsingIncrease { get; private set; }

	public string title
	{
		get
		{
			if (data == null)
			{
				return string.Empty;
			}
			return data.GetTitle();
		}
	}

	public CardAttackColor attColor
	{
		get
		{
			if (cardInfo == null)
			{
				return CardAttackColor.None;
			}
			return cardInfo.attColor;
		}
	}

	public CardInfo cardInfo { get; private set; }

	public bool isRequireTwoHands => HasCardFlag(CardFlagType.RequireTwoHandsToEquip);

	public Card(CardData cardData, CardInfo oldInfo = null)
	{
		dataOri = cardData;
		dataInc = cardData.increaseCard;
		InitCardInfo(cardData, oldInfo);
	}

	public Card(Card cloneCard)
		: this(cloneCard.data, cloneCard.cardInfo)
	{
	}

	public void CloneTempInfo(Card srcCard)
	{
		fightTempInfo = srcCard.fightTempInfo.ShallowCopy();
		roundTempInfo = srcCard.roundTempInfo.ShallowCopy();
		actionTempInfo = srcCard.actionTempInfo.ShallowCopy();
	}

	public List<CardTempInfo> GetAllTempInfos()
	{
		return new List<CardTempInfo> { fightTempInfo, roundTempInfo, actionTempInfo };
	}

	private void InitCardInfo(CardData cardData, CardInfo oldInfo = null)
	{
		if (oldInfo != null)
		{
			cardInfo = oldInfo.ShallowCopy();
			return;
		}
		cardInfo = new CardInfo(cardData.name);
		cardInfo.increase = data.increase;
		if (cardData.cardType != CardType.UseEquip)
		{
			cardInfo.attColor = CardAttackColor.None;
		}
		else if (UnityEngine.Random.Range(0, 1000) < 500)
		{
			cardInfo.attColor = CardAttackColor.Left;
		}
		else
		{
			cardInfo.attColor = CardAttackColor.Right;
		}
	}

	public void SetCardColor(CardAttackColor color)
	{
		if (cardInfo == null)
		{
			Debug.LogWarning("[Card] SetCardColor fail, no cardInfo");
		}
		else
		{
			cardInfo.attColor = color;
		}
	}

	public void SwapCardColor()
	{
		switch (attColor)
		{
		case CardAttackColor.Left:
			SetCardColor(CardAttackColor.Right);
			break;
		case CardAttackColor.Right:
			SetCardColor(CardAttackColor.Left);
			break;
		}
	}

	public bool CanLevelup()
	{
		if (dataOri == null)
		{
			return false;
		}
		return dataOri.CanLevelup();
	}

	public void LevelupCard()
	{
		if (!dataOri.HasLevelup() || dataOri.levelupCard == null)
		{
			Debug.LogError("[Card] LevelupCard Fail: " + title);
			return;
		}
		dataOri = data.levelupCard;
		cardInfo.key = dataOri.name;
	}

	public void UpgradeEquipment()
	{
		if (!data.IsEquipment())
		{
			Debug.LogError("[Card] UpgradeEquipment fail, is not equipment: " + title);
			return;
		}
		cardInfo.upgradeTimes++;
		cardInfo.addAttPower++;
		cardInfo.addDefPower++;
	}

	public void UpgradeIncrease()
	{
		if (!data.IsEquipment())
		{
			Debug.LogError("[Card] UpgradeIncrease fail, is not equipment: " + title);
			return;
		}
		if (!data.HasIncreaseFunction())
		{
			Debug.LogError("[Card] UpgradeIncrease fail, can't use increase: " + title);
			return;
		}
		cardInfo.upgradeIncreaseTimes++;
		cardInfo.addIncreaseMax++;
		AddIncreaseCount(1);
	}

	public bool HasEnoughEnergyToPlay(ArenaInfo arenaInfo, CharacterBase srcChara)
	{
		if (HasCardFlag(CardFlagType.Unplayable))
		{
			return false;
		}
		if (arenaInfo == null)
		{
			return true;
		}
		int adjustedEnergy = GetAdjustedEnergy(arenaInfo, srcChara);
		return arenaInfo.energyCnt >= adjustedEnergy;
	}

	public bool CanPlayByReload(CharacterBase srcChara)
	{
		if (srcChara == null)
		{
			return false;
		}
		CharacterEquipment equipment = srcChara.GetEquipment(attColor);
		if (equipment == null)
		{
			return false;
		}
		if (!equipment.IsNeedReload())
		{
			return false;
		}
		return equipment.CanReloadBy(this);
	}

	public bool HasCardFlag(CardFlagType flagType)
	{
		if (data == null)
		{
			return false;
		}
		foreach (CardFlag totalCardFlag in GetTotalCardFlags())
		{
			if (totalCardFlag.flag == flagType)
			{
				return true;
			}
		}
		return false;
	}

	public CardFlag GetCardFlag(CardFlagType flagType)
	{
		if (data == null)
		{
			return null;
		}
		foreach (CardFlag totalCardFlag in GetTotalCardFlags())
		{
			if (totalCardFlag.flag == flagType)
			{
				return totalCardFlag;
			}
		}
		return null;
	}

	public void RestoreIncreaseTimes()
	{
		cardInfo.increase = GetIncreaseMax();
	}

	public int GetIncreaseMax()
	{
		return data.increase + cardInfo.addIncreaseMax;
	}

	public void AddIncreaseCount(int value)
	{
		cardInfo.increase += value;
		int increaseMax = GetIncreaseMax();
		if (cardInfo.increase >= increaseMax)
		{
			cardInfo.increase = increaseMax;
		}
	}

	public void ResetAllTempInfos()
	{
		ResetRoundTempInfo();
		ResetFightTempInfo();
		ResetActionTempInfo();
	}

	public void ResetRoundTempInfo()
	{
		roundTempInfo.Reset();
	}

	public void ResetFightTempInfo()
	{
		fightTempInfo.Reset();
	}

	public void ResetActionTempInfo()
	{
		actionTempInfo.Reset();
	}

	public List<CardFlag> GetTotalCardFlags()
	{
		List<CardFlag> list = new List<CardFlag>();
		list.AddRange(data.cardFlags);
		foreach (CardTempInfo allTempInfo in GetAllTempInfos())
		{
			list.AddRange(allTempInfo.cardFlags);
		}
		return list;
	}

	public void AddCardFlagToTempInfo(CardTempInfoAddTo addTo, CardFlagType cardFlag, bool isAddable = false)
	{
		CardTempInfo cardTempInfo = null;
		switch (addTo)
		{
		case CardTempInfoAddTo.Round:
			cardTempInfo = roundTempInfo;
			break;
		case CardTempInfoAddTo.Fight:
			cardTempInfo = fightTempInfo;
			break;
		case CardTempInfoAddTo.Action:
			cardTempInfo = actionTempInfo;
			break;
		}
		if (cardTempInfo == null)
		{
			Debug.LogError("[Card] AddTempCardFlag fail: " + addTo);
			return;
		}
		CardFlag cardFlag2 = cardTempInfo.cardFlags.Find((CardFlag o) => o.flag == cardFlag);
		if (isAddable || cardFlag2 == null)
		{
			cardTempInfo.cardFlags.Add(new CardFlag(cardFlag));
		}
	}

	public void RemoveCardFlagFromTempInfo(CardTempInfoAddTo removeFrom, CardFlagType cardFlag)
	{
		CardTempInfo cardTempInfo = null;
		switch (removeFrom)
		{
		case CardTempInfoAddTo.Round:
			cardTempInfo = roundTempInfo;
			break;
		case CardTempInfoAddTo.Fight:
			cardTempInfo = fightTempInfo;
			break;
		case CardTempInfoAddTo.Action:
			cardTempInfo = actionTempInfo;
			break;
		}
		if (cardTempInfo == null)
		{
			Debug.LogError("[Card] RemoveTempCardFlag fail: " + removeFrom);
			return;
		}
		CardFlag cardFlag2 = cardTempInfo.cardFlags.Find((CardFlag o) => o.flag == cardFlag);
		if (cardFlag2 != null)
		{
			cardTempInfo.cardFlags.Remove(cardFlag2);
		}
	}

	public CardTempInfo GetCardTempInfo(CardTempInfoAddTo addTo)
	{
		return addTo switch
		{
			CardTempInfoAddTo.Fight => fightTempInfo, 
			CardTempInfoAddTo.Action => actionTempInfo, 
			_ => roundTempInfo, 
		};
	}

	public int GetMultiAttackCount()
	{
		foreach (CardFlag totalCardFlag in GetTotalCardFlags())
		{
			if (totalCardFlag.flag == CardFlagType.MultiAttack)
			{
				return (int)totalCardFlag.value;
			}
		}
		return 1;
	}

	public bool SwitchIncrease()
	{
		isUsingIncrease = !isUsingIncrease;
		return isUsingIncrease;
	}

	public void DisableIncrease()
	{
		isUsingIncrease = false;
	}

	public bool CanUseIncreasePower()
	{
		if (dataInc == null)
		{
			return false;
		}
		if (GetAdjustedIncrease() <= 0)
		{
			return false;
		}
		return true;
	}

	public CardData GetOriginData()
	{
		return dataOri;
	}

	public CardData GetIncreaseData()
	{
		return dataInc;
	}

	public bool IsNeedReload()
	{
		return HasCardFlag(CardFlagType.NeedReload);
	}

	public void SetNeedReload()
	{
		AddCardFlagToTempInfo(CardTempInfoAddTo.Fight, CardFlagType.NeedReload);
	}

	public void RemoveNeedReload()
	{
		RemoveCardFlagFromTempInfo(CardTempInfoAddTo.Fight, CardFlagType.NeedReload);
	}

	public int GetOriginEnergy()
	{
		int num = dataOri.point;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int GetOriginMove()
	{
		int num = dataOri.movePower;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int GetOriginAttack()
	{
		int num = dataOri.attackPower;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int GetOriginDefend()
	{
		int num = dataOri.defendPower;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int GetOriginIncrease()
	{
		int num = dataOri.increase;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public int GetAdjustedEnergyWithoutEquipment(ArenaInfo arenaInfo, CharacterBase srcChara)
	{
		return GetAdjustedEnergy(arenaInfo, srcChara, null);
	}

	public int GetAdjustedEnergy(ArenaInfo arenaInfo = null, CharacterBase srcChara = null)
	{
		CharacterEquipment usingEquipment = null;
		if (srcChara != null)
		{
			usingEquipment = srcChara.GetUsingEquipment(this);
		}
		return GetAdjustedEnergy(arenaInfo, srcChara, usingEquipment);
	}

	private int GetAdjustedEnergy(ArenaInfo arenaInfo = null, CharacterBase srcChara = null, CharacterEquipment usingEquipment = null)
	{
		CardController cardController = null;
		if (arenaInfo != null)
		{
			cardController = arenaInfo.cardCtrl;
		}
		int point = data.point;
		point += cardInfo.adjustEnergy;
		foreach (CardTempInfo allTempInfo in GetAllTempInfos())
		{
			point += allTempInfo.addEnergy;
		}
		UsingCardFlags usingCardFlags = new UsingCardFlags(this, usingEquipment);
		bool flag = false;
		int num = 100;
		foreach (CardFlag item in usingCardFlags)
		{
			switch (item.flag)
			{
			case CardFlagType.SetCostZero:
				num = Mathf.Min(num, 0);
				flag = true;
				break;
			case CardFlagType.SetCostOne:
				num = Mathf.Min(num, 1);
				flag = true;
				break;
			case CardFlagType.SubCostByUnderAttCnt:
				if (arenaInfo != null)
				{
					point -= arenaInfo.underDamageCnt;
				}
				break;
			case CardFlagType.SubCostByGreaterHandCnt:
				if (arenaInfo != null && cardController != null)
				{
					int num3 = (int)item.value;
					if (cardController.handCardCount > num3)
					{
						point--;
					}
				}
				break;
			case CardFlagType.SubCostByCombo:
				if (arenaInfo != null && arenaInfo.isInFighting)
				{
					int num2 = (int)item.value;
					if (arenaInfo.comboCount >= num2)
					{
						point--;
					}
				}
				break;
			case CardFlagType.AddCostEnergy:
				point++;
				break;
			case CardFlagType.SubCostEnergy:
				point--;
				break;
			}
		}
		if (srcChara != null)
		{
			foreach (CharacterFlagInfo usingCharaFlag in srcChara.usingCharaFlags)
			{
				foreach (CharacterFlagFunction function in usingCharaFlag.data.functions)
				{
					switch (function.type)
					{
					case CharacterFlagType.MakeMoveGeneralCostZero:
						if (data.cardType == CardType.GeneralMove)
						{
							num = Mathf.Min(num, 0);
							flag = true;
						}
						break;
					case CharacterFlagType.MakeStatusCostZero:
						if (data.cardType == CardType.Status)
						{
							num = Mathf.Min(num, 0);
							flag = true;
						}
						break;
					case CharacterFlagType.MakeRefCardCostZero:
					{
						CardData cardDataRef2 = function.cardDataRef;
						if (data == cardDataRef2)
						{
							num = Mathf.Min(num, 0);
							flag = true;
						}
						break;
					}
					case CharacterFlagType.MakeRefCardCostDown:
					{
						CardData cardDataRef = function.cardDataRef;
						if (data == cardDataRef)
						{
							point--;
						}
						break;
					}
					case CharacterFlagType.AddMovableCost:
						if (data.isAdjustEnergyByMovableNerf)
						{
							point++;
						}
						break;
					}
				}
			}
		}
		if (arenaInfo != null && cardController != null && data.isAdjustEnergyByMovableNerf && cardController.HasAnyCardFlagCardsInHand(CardFlagType.AddCostEnergyForMovableCard))
		{
			point++;
		}
		if (flag)
		{
			point = num;
		}
		if (point < 0)
		{
			point = 0;
		}
		return point;
	}

	public int GetAdjustedMove()
	{
		int movePower = data.movePower;
		movePower += cardInfo.addMovPower;
		foreach (CardTempInfo allTempInfo in GetAllTempInfos())
		{
			movePower += allTempInfo.addMovPower;
		}
		if (movePower < 0)
		{
			movePower = 0;
		}
		return movePower;
	}

	public int GetAdjustedAttack()
	{
		int attackPower = data.attackPower;
		attackPower += cardInfo.addAttPower;
		foreach (CardTempInfo allTempInfo in GetAllTempInfos())
		{
			attackPower += allTempInfo.addAttPower;
		}
		foreach (CardFlag cardFlag in data.cardFlags)
		{
			if (cardFlag.flag == CardFlagType.AddDmgForEquipment)
			{
				attackPower += Mathf.RoundToInt(cardFlag.value);
			}
		}
		if (attackPower < 0)
		{
			attackPower = 0;
		}
		return attackPower;
	}

	public int GetAdjustedDefend()
	{
		int defendPower = data.defendPower;
		defendPower += cardInfo.addDefPower;
		foreach (CardTempInfo allTempInfo in GetAllTempInfos())
		{
			defendPower += allTempInfo.addDefPower;
		}
		if (defendPower < 0)
		{
			defendPower = 0;
		}
		return defendPower;
	}

	public int GetAdjustedIncrease()
	{
		int num = cardInfo.increase;
		foreach (CardTempInfo allTempInfo in GetAllTempInfos())
		{
			num += allTempInfo.extraIncrease;
		}
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public void CostIncrease()
	{
		if (roundTempInfo.extraIncrease > 0)
		{
			roundTempInfo.extraIncrease--;
			if (roundTempInfo.extraIncrease < 0)
			{
				roundTempInfo.extraIncrease = 0;
			}
		}
		else if (fightTempInfo.extraIncrease > 0)
		{
			fightTempInfo.extraIncrease--;
			if (fightTempInfo.extraIncrease < 0)
			{
				fightTempInfo.extraIncrease = 0;
			}
		}
		else if (actionTempInfo.extraIncrease > 0)
		{
			actionTempInfo.extraIncrease--;
			if (actionTempInfo.extraIncrease < 0)
			{
				actionTempInfo.extraIncrease = 0;
			}
		}
		else
		{
			cardInfo.increase--;
			if (cardInfo.increase < 0)
			{
				cardInfo.increase = 0;
			}
		}
	}

	public static int CompareInDeck(Card xCard, Card yCard)
	{
		if (xCard.data.cardType != yCard.data.cardType)
		{
			int cardTypePriority = GetCardTypePriority(xCard.data.cardType);
			int cardTypePriority2 = GetCardTypePriority(yCard.data.cardType);
			if (cardTypePriority <= cardTypePriority2)
			{
				return 1;
			}
			return -1;
		}
		if (xCard.attColor != yCard.attColor)
		{
			if (xCard.attColor != CardAttackColor.Left)
			{
				return 1;
			}
			return -1;
		}
		if (xCard.data.actMethodPointVisual != yCard.data.actMethodPointVisual)
		{
			if (xCard.data.actMethodPointVisual != CardActMethodPointVisual.Attack)
			{
				return 1;
			}
			return -1;
		}
		if (xCard.data.cardRare != yCard.data.cardRare)
		{
			return xCard.data.cardRare.CompareTo(yCard.data.cardRare);
		}
		return xCard.data.name.CompareTo(yCard.data.name);
	}

	public static int CompareInHandCard(Card xCard, Card yCard)
	{
		if (xCard.data.cardType == CardType.GeneralMove)
		{
			return -1;
		}
		if (yCard.data.cardType == CardType.GeneralMove)
		{
			return 1;
		}
		if (xCard.attColor != yCard.attColor)
		{
			if (xCard.attColor == CardAttackColor.Left)
			{
				return -1;
			}
			if (xCard.attColor == CardAttackColor.Right)
			{
				return 1;
			}
			if (yCard.attColor == CardAttackColor.Left)
			{
				return 1;
			}
			if (yCard.attColor == CardAttackColor.Right)
			{
				return -1;
			}
		}
		if (xCard.data.cardType != yCard.data.cardType)
		{
			int cardTypePriority = GetCardTypePriority(xCard.data.cardType);
			int cardTypePriority2 = GetCardTypePriority(yCard.data.cardType);
			if (cardTypePriority <= cardTypePriority2)
			{
				return 1;
			}
			return -1;
		}
		return 0;
	}

	private static int GetCardTypePriority(CardType cardType)
	{
		return cardType switch
		{
			CardType.GeneralMove => 100, 
			CardType.UseEquip => 90, 
			CardType.Action => 80, 
			CardType.Skill => 70, 
			CardType.Power => 60, 
			CardType.Equipment => 50, 
			CardType.Item => 40, 
			CardType.Status => 30, 
			CardType.Trap => 20, 
			_ => 0, 
		};
	}

	public bool CanUseByDropInExecutableArea()
	{
		if (data.actionMethod == null)
		{
			return false;
		}
		CardActionMap cardActMap = data.actionMethod.cardActMap;
		if (cardActMap == CardActionMap.Defend || cardActMap == CardActionMap.SelfAround || cardActMap == CardActionMap.UseCard)
		{
			return true;
		}
		return false;
	}

	public bool IsShowMainNumber()
	{
		if (data == null)
		{
			return false;
		}
		if (data.actionMethod == null)
		{
			return false;
		}
		if (data.IsEquipment())
		{
			return false;
		}
		if (data.actMethodPointVisual == CardActMethodPointVisual.None)
		{
			return false;
		}
		return true;
	}

	public int GetOriginNumber()
	{
		return data.actMethodPointVisual switch
		{
			CardActMethodPointVisual.Attack => GetOriginAttack(), 
			CardActMethodPointVisual.Defend => GetOriginDefend(), 
			CardActMethodPointVisual.Move => GetOriginMove(), 
			CardActMethodPointVisual.Hemorrhage => 1, 
			CardActMethodPointVisual.OriginAttack => GetOriginAttack(), 
			_ => 0, 
		};
	}

	public int GetAdjustNumber()
	{
		return data.actMethodPointVisual switch
		{
			CardActMethodPointVisual.Attack => GetAdjustedAttack(), 
			CardActMethodPointVisual.Defend => GetAdjustedDefend(), 
			CardActMethodPointVisual.Move => GetAdjustedMove(), 
			CardActMethodPointVisual.Hemorrhage => 1, 
			CardActMethodPointVisual.OriginAttack => GetOriginAttack(), 
			_ => 0, 
		};
	}

	public int GetAdjustNumberByEnvironment(CardNumberEnvironment env)
	{
		CharacterBase srcChara = env.srcChara;
		CharacterBase dstChara = env.dstChara;
		ArenaInfo arenaInfo = env.arenaInfo;
		CardController cardController = null;
		if (arenaInfo != null)
		{
			cardController = arenaInfo.cardCtrl;
		}
		CharacterEquipment characterEquipment = null;
		if (srcChara != null)
		{
			characterEquipment = srcChara.GetUsingEquipment(this);
		}
		UsingCardFlags usingCardFlags = new UsingCardFlags(this, characterEquipment);
		UsingActionMethod usingActionMethod = new UsingActionMethod(this, characterEquipment, usingCardFlags);
		int result = 0;
		switch (data.actMethodPointVisual)
		{
		case CardActMethodPointVisual.Attack:
			result = AttackDamageCalculator.CalculatorUnderAttackInfo(usingActionMethod.attackPower, srcChara, dstChara, usingCardFlags, isSelfKillAttack: false, isLastAttack: false, isRangeAttack: false, arenaInfo, this).attPower;
			break;
		case CardActMethodPointVisual.Defend:
			result = DefendValueCalculator.GetDefendValue(usingActionMethod.defendPower, usingCardFlags, this, srcChara, arenaInfo);
			break;
		case CardActMethodPointVisual.Move:
			result = usingActionMethod.movePower;
			break;
		case CardActMethodPointVisual.Hemorrhage:
			if (cardController != null)
			{
				result = cardController.GetCardCountFromHand(cardController.woundCardData);
			}
			break;
		case CardActMethodPointVisual.OriginAttack:
			result = GetOriginAttack();
			break;
		}
		return result;
	}

	public bool IsLocked(UnlockedInfo unlockedInfo)
	{
		if (!data.isLock)
		{
			return false;
		}
		if (unlockedInfo == null)
		{
			return false;
		}
		if (unlockedInfo.IsUnlocked(data))
		{
			return false;
		}
		return true;
	}

	public int GetPriceOfUpgradeEquipment(ArenaInfo arenaInfo, float priceOff = 1f, bool isGetPrePrice = false)
	{
		if (arenaInfo == null)
		{
			Debug.LogWarning("GetPriceOfUpgradeEquipment fail, arenaInfo is null: " + data.name);
			return 999;
		}
		int priceOfUpgradeEquipment = arenaInfo.arenaData.priceOfUpgradeEquipment;
		int addPriceOfUpgradeEquipment = arenaInfo.arenaData.addPriceOfUpgradeEquipment;
		int num = cardInfo.GetUpgradeTimes();
		if (isGetPrePrice)
		{
			num--;
			if (num < 0)
			{
				num = 0;
			}
		}
		return Mathf.RoundToInt((float)(priceOfUpgradeEquipment + addPriceOfUpgradeEquipment * num) * priceOff);
	}

	public int GetPriceOfUpgradeIncrease(ArenaInfo arenaInfo, float priceOff = 1f, bool isGetPrePrice = false)
	{
		if (arenaInfo == null)
		{
			Debug.LogWarning("GetPriceOfUpgradeIncrease fail, arenaInfo is null: " + data.name);
			return 999;
		}
		int priceOfUpgradeIncrease = arenaInfo.arenaData.priceOfUpgradeIncrease;
		int addPriceOfUpgradeIncrease = arenaInfo.arenaData.addPriceOfUpgradeIncrease;
		int num = cardInfo.GetUpgradeIncreaseTimes();
		if (isGetPrePrice)
		{
			num--;
			if (num < 0)
			{
				num = 0;
			}
		}
		return Mathf.RoundToInt((float)(priceOfUpgradeIncrease + addPriceOfUpgradeIncrease * num) * priceOff);
	}

	public int GetSellPrice()
	{
		return data.GetSellPrice() + GetSellPriceForUpgrade();
	}

	private int GetSellPriceForUpgrade()
	{
		int upgradeTimes = cardInfo.GetUpgradeTimes();
		int upgradeIncreaseTimes = cardInfo.GetUpgradeIncreaseTimes();
		return (upgradeTimes + upgradeIncreaseTimes) * 10;
	}
}
