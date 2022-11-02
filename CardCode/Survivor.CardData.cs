// Survivor.CardData
using System;
using System.Collections.Generic;
using System.Text;
using MWUtil;
using Survivor;
using UnityEngine;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
	[NonSerialized]
	public CardDataLocalization localization;

	private static string TAG_MOV = "<M>";

	private static string TAG_ATT = "<A>";

	private static string TAG_DEF = "<D>";

	private static string TAG_CDF = "<CDF{0}>";

	private static string TAG_CDF_VALUE_AMOUNT_PERCENT = "<CDFVAP{0}>";

	private static string TAG_CDFCDT = "<CDFCDT{0}>";

	private static string TAG_CDFCHFT = "<CDFCHFT{0}>";

	private static string TAG_CHFDK = "<CHFDK{0}>";

	private static string TAG_CHFDA = "<CHFDA{0}>";

	private static string TAG_CHFSK = "<CHFSK{0}>";

	private static string TAG_CHFSD = "<CHFSD{0}>";

	private static string TAG_CHFSA = "<CHFSA{0}>";

	private static string TAG_HIGHLIGHT = "<H>";

	private static string TAG_HIGHLIGHT_END = "</H>";

	private static string TAG_AMR = "<AMR>";

	private static string TAG_WMR = "<WMR>";

	private static string TAG_TMPA_A = "<TMPAA>";

	private static string TAG_TMPF_B = "<TMPFB>";

	private static string TAG_ADJUESTED_B = "<ADJB>";

	[SerializeField]
	private string title = string.Empty;

	[TextArea]
	[SerializeField]
	private string desc = string.Empty;

	[SerializeField]
	public LocalizationStatus localizationStatus;

	public CardType cardType;

	public CardCombatType cardCombatType;

	public CardSubType cardSubType;

	public CardSchoolType cardSchoolTypes;

	public CardRare cardRare = CardRare.Normal;

	public Sprite cardImage;

	public bool _isLock;

	public bool _isLockForDemo;

	public bool isClassGeneralCard;

	public bool isHideInCollection;

	public bool isHidePoint;

	public int point = 1;

	public CardActMethodPointVisual actMethodPointVisual;

	public CharacterEquipmentVisualType equipVisual;

	public int increase;

	public bool isAdjustEnergyByMovableNerf;

	public int movePower;

	public int attackPower;

	public int defendPower;

	[Range(-400f, 400f)]
	public int floatPrice;

	public List<CardFlag> cardFlags = new List<CardFlag>();

	public List<AddCharacterFlag> addCharaFlagsToDstCharas = new List<AddCharacterFlag>();

	public List<AddCharacterFlag> addCharaFlagsToSrcChara = new List<AddCharacterFlag>();

	public ActionMethod actionMethod;

	public ActionMethod weaponActMethod;

	public CardData increaseCard;

	public bool isIncreaseCard;

	[TextArea]
	[SerializeField]
	private string increaseDesc = string.Empty;

	public CardData levelupCard;

	public bool isLevelupCard;

	public bool isHasNoLevelup;

	public Sprite cardImgItem
	{
		get
		{
			return cardImage;
		}
		set
		{
			cardImage = value;
		}
	}

	public Sprite trinketIcon
	{
		get
		{
			return cardImage;
		}
		set
		{
			cardImage = value;
		}
	}

	public bool isLock => _isLock;

	public void CleanLocalization()
	{
		localization = null;
	}

	public string GetAnalyticCardKey()
	{
		if (isLevelupCard && levelupCard != null)
		{
			return levelupCard.GetAnalyticCardKey();
		}
		return base.name;
	}

	public string GetOriginTitle()
	{
		return title;
	}

	public string GetTitle()
	{
		string srcTitle = title;
		if (localization != null)
		{
			srcTitle = localization.title;
		}
		StringBuilder builder = new StringBuilder();
		UpdateTitlebuilder(ref builder, srcTitle);
		return builder.ToString();
	}

	private void UpdateTitlebuilder(ref StringBuilder builder, string srcTitle)
	{
		builder.Append(srcTitle);
		for (int i = 0; i < addCharaFlagsToSrcChara.Count; i++)
		{
			AddCharacterFlag addCharacterFlag = addCharaFlagsToSrcChara[i];
			if (addCharacterFlag != null && !(addCharacterFlag.flagData == null))
			{
				string oldValue = string.Format(TAG_CHFSK, i);
				builder.Replace(oldValue, addCharacterFlag.flagData.GetKeyword());
			}
		}
	}

	public string GetTitleRef(string srcTitle)
	{
		StringBuilder builder = new StringBuilder();
		UpdateTitlebuilder(ref builder, srcTitle);
		return builder.ToString();
	}

	public void SetOriginTitle(string value)
	{
		title = value;
	}

	private static string GetDescInfoStr()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(TAG_MOV + " = 移動力:movePower");
		stringBuilder.AppendLine(TAG_ATT + " = 攻擊力:attackPower");
		stringBuilder.AppendLine(TAG_DEF + " = 防禦力:defendPower");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(TAG_CDF + " = CardFlag.value");
		stringBuilder.AppendLine(TAG_CDF_VALUE_AMOUNT_PERCENT + " = (value * amount * 100)");
		stringBuilder.AppendLine(TAG_CDFCDT + " = CardFlag.CardDataRef.Title");
		stringBuilder.AppendLine(TAG_CDFCHFT + " = CardFlag.CharacterFlagDataRef");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(TAG_CHFDK + " = addCharaFlagsToDstCharas.Keyword");
		stringBuilder.AppendLine(TAG_CHFDA + " = Amount of addCharaFlagsToDstCharas");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(TAG_CHFSK + " = addCharaFlagsToSrcChara.Keyword");
		stringBuilder.AppendLine(TAG_CHFSD + " = addCharaFlagsToSrcChara.Desc");
		stringBuilder.AppendLine(TAG_CHFSA + " = Amount of addCharaFlagsToSrcChara");
		stringBuilder.AppendLine(TAG_HIGHLIGHT + "增加顏色" + TAG_HIGHLIGHT_END);
		stringBuilder.AppendLine(TAG_AMR + " = ActionMethod.Range");
		stringBuilder.AppendLine(TAG_WMR + " = WeaponActMethod.Range");
		stringBuilder.AppendLine(TAG_TMPA_A + "= TempInfo_Action.Attack 本場戰鬥執行前臨時增加傷害 ex:(+3)");
		stringBuilder.AppendLine(TAG_TMPF_B + "= TempInfo_Fight.Block 本場戰鬥臨時增加格擋 ex:(+3)");
		stringBuilder.AppendLine(TAG_ADJUESTED_B + "+調整後格擋 ex:(-3)");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("Sprite Tag:");
		foreach (GameSpriteText value in Enum.GetValues(typeof(GameSpriteText)))
		{
			stringBuilder.AppendFormat("<{0}> ", value.ToString());
		}
		if (KeywordDatabase.instance != null)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Keyword Tag:");
			foreach (KeywordData data in KeywordDatabase.instance.datas)
			{
				stringBuilder.AppendFormat("{0} ", data.tagStr);
			}
		}
		return stringBuilder.ToString();
	}

	public string GetOriginDesc()
	{
		return desc;
	}

	public bool ValidateDesc(string srcDesc, out string validateDesc)
	{
		StringBuilder stringBuilder = new StringBuilder();
		AddDesc(stringBuilder, srcDesc);
		AddKeywords(stringBuilder, cardFlags);
		stringBuilder.Replace(ConstValue.KEYWORD_COLOR_TAG_S, string.Empty);
		stringBuilder.Replace(ConstValue.KEYWORD_COLOR_TAG_E, string.Empty);
		foreach (GameSpriteText value in Enum.GetValues(typeof(GameSpriteText)))
		{
			string oldValue = $"<sprite index={(int)value}>";
			stringBuilder.Replace(oldValue, string.Empty);
		}
		validateDesc = stringBuilder.ToString();
		char[] anyOf = new char[2] { '<', '>' };
		return validateDesc.IndexOfAny(anyOf) <= 0;
	}

	public string GetDesc()
	{
		return GetDesc(null);
	}

	public string GetDesc(Card card)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string srcDesc = desc;
		if (localization != null)
		{
			srcDesc = localization.desc;
		}
		AddDesc(stringBuilder, srcDesc, card);
		AddKeywords(stringBuilder, cardFlags);
		return stringBuilder.ToString();
	}

	public string GetDescRef(string srcDesc)
	{
		StringBuilder stringBuilder = new StringBuilder();
		AddDesc(stringBuilder, srcDesc);
		return stringBuilder.ToString();
	}

	private void AddDesc(StringBuilder builder, string srcDesc, Card card = null)
	{
		builder.Append(srcDesc);
		KeywordCheckGroup keywordCheckGroup = KeywordDatabase.instance?.CreateCheckGroup(srcDesc);
		builder.Replace(TAG_MOV, movePower.ToString());
		keywordCheckGroup?.Check(TAG_MOV, movePower);
		builder.Replace(TAG_ATT, attackPower.ToString());
		builder.Replace(TAG_DEF, defendPower.ToString());
		builder.Replace(TAG_HIGHLIGHT, ConstValue.KEYWORD_COLOR_TAG_S);
		builder.Replace(TAG_HIGHLIGHT_END, ConstValue.KEYWORD_COLOR_TAG_E);
		if (card == null)
		{
			builder.Replace(TAG_TMPA_A, ConstValue.KEYWORD_COLOR_TAG_S + ConstValue.KEYWORD_CARD_ADD_S + 0 + ConstValue.KEYWORD_CARD_ADD_E + ConstValue.KEYWORD_COLOR_TAG_E);
			builder.Replace(TAG_TMPF_B, ConstValue.KEYWORD_COLOR_TAG_S + ConstValue.KEYWORD_CARD_ADD_S + 0 + ConstValue.KEYWORD_CARD_ADD_E + ConstValue.KEYWORD_COLOR_TAG_E);
			builder.Replace(TAG_ADJUESTED_B, ConstValue.KEYWORD_COLOR_TAG_S + ConstValue.KEYWORD_CARD_SUB_S + 0 + ConstValue.KEYWORD_CARD_SUB_E + ConstValue.KEYWORD_COLOR_TAG_E);
		}
		else
		{
			CardTempInfo cardTempInfo = card.GetCardTempInfo(CardTempInfoAddTo.Action);
			CardTempInfo cardTempInfo2 = card.GetCardTempInfo(CardTempInfoAddTo.Fight);
			builder.Replace(TAG_TMPA_A, ConstValue.KEYWORD_COLOR_TAG_S + ConstValue.KEYWORD_CARD_ADD_S + cardTempInfo.addAttPower + ConstValue.KEYWORD_CARD_ADD_E + ConstValue.KEYWORD_COLOR_TAG_E);
			builder.Replace(TAG_TMPF_B, ConstValue.KEYWORD_COLOR_TAG_S + ConstValue.KEYWORD_CARD_ADD_S + cardTempInfo2.addDefPower + ConstValue.KEYWORD_CARD_ADD_E + ConstValue.KEYWORD_COLOR_TAG_E);
			builder.Replace(TAG_ADJUESTED_B, ConstValue.KEYWORD_COLOR_TAG_S + ConstValue.KEYWORD_CARD_SUB_S + card.GetAdjustedDefend() + ConstValue.KEYWORD_CARD_SUB_E + ConstValue.KEYWORD_COLOR_TAG_E);
		}
		for (int i = 0; i < cardFlags.Count; i++)
		{
			CardFlag cardFlag = cardFlags[i];
			if (cardFlag != null)
			{
				string text = string.Format(TAG_CDF, i);
				builder.Replace(text, cardFlag.value.ToString());
				keywordCheckGroup?.Check(text, (int)cardFlag.value);
				text = string.Format(TAG_CDF_VALUE_AMOUNT_PERCENT, i);
				builder.Replace(text, (cardFlag.value * 100f).ToString());
				if (cardFlag.cardDataRef != null)
				{
					text = string.Format(TAG_CDFCDT, i);
					builder.Replace(text, ConstValue.KEYWORD_COLOR_TAG_S + cardFlag.cardDataRef.GetTitle() + ConstValue.KEYWORD_COLOR_TAG_E);
				}
				if (cardFlag.characterFlagDataRef != null)
				{
					text = string.Format(TAG_CDFCHFT, i);
					string keyword = cardFlag.characterFlagDataRef.GetKeyword();
					builder.Replace(text, ConstValue.KEYWORD_COLOR_TAG_S + keyword + ConstValue.KEYWORD_COLOR_TAG_E);
				}
			}
		}
		for (int j = 0; j < addCharaFlagsToDstCharas.Count; j++)
		{
			AddCharacterFlag addCharacterFlag = addCharaFlagsToDstCharas[j];
			if (addCharacterFlag != null && !(addCharacterFlag.flagData == null))
			{
				string oldValue = string.Format(TAG_CHFDK, j);
				builder.Replace(oldValue, ConstValue.KEYWORD_COLOR_TAG_S + addCharacterFlag.flagData.GetKeyword() + ConstValue.KEYWORD_COLOR_TAG_E);
				oldValue = string.Format(TAG_CHFDA, j);
				builder.Replace(oldValue, addCharacterFlag.amount.ToString());
				keywordCheckGroup?.Check(oldValue, addCharacterFlag.amount);
			}
		}
		for (int k = 0; k < addCharaFlagsToSrcChara.Count; k++)
		{
			AddCharacterFlag addCharacterFlag2 = addCharaFlagsToSrcChara[k];
			if (addCharacterFlag2 != null && !(addCharacterFlag2.flagData == null))
			{
				string oldValue2 = string.Format(TAG_CHFSK, k);
				builder.Replace(oldValue2, ConstValue.KEYWORD_COLOR_TAG_S + addCharacterFlag2.flagData.GetKeyword() + ConstValue.KEYWORD_COLOR_TAG_E);
				oldValue2 = string.Format(TAG_CHFSD, k);
				builder.Replace(oldValue2, addCharacterFlag2.flagData.GetDesc(addCharacterFlag2.amount));
				oldValue2 = string.Format(TAG_CHFSA, k);
				builder.Replace(oldValue2, addCharacterFlag2.amount.ToString());
				keywordCheckGroup?.Check(oldValue2, addCharacterFlag2.amount);
			}
		}
		if (actionMethod != null && actionMethod.selectArea != null)
		{
			builder.Replace(TAG_AMR, actionMethod.selectArea.radius.ToString());
			keywordCheckGroup?.Check(TAG_AMR, actionMethod.selectArea.radius);
		}
		if (weaponActMethod != null && weaponActMethod.selectArea != null)
		{
			builder.Replace(TAG_WMR, weaponActMethod.selectArea.radius.ToString());
			keywordCheckGroup?.Check(TAG_WMR, weaponActMethod.selectArea.radius);
		}
		foreach (GameSpriteText value in Enum.GetValues(typeof(GameSpriteText)))
		{
			string oldValue3 = $"<{value.ToString()}>";
			builder.Replace(oldValue3, $"<sprite index={(int)value}>");
		}
		if (keywordCheckGroup == null || !(KeywordDatabase.instance != null))
		{
			return;
		}
		foreach (KeywordCheckInfo info in keywordCheckGroup.infos)
		{
			KeywordDatabase.instance.ReplaceKeyword(builder, info.data.key, info.number);
		}
	}

	private void AddKeywords(StringBuilder builder, List<CardFlag> usingCardFlags)
	{
		if (usingCardFlags.Count <= 0)
		{
			return;
		}
		if (builder.Length != 0)
		{
			builder.AppendLine();
		}
		builder.AppendFormat(ConstValue.KEYWORD_COLOR_TAG_S);
		foreach (CardFlag usingCardFlag in usingCardFlags)
		{
			if (usingCardFlag.HasKeyword())
			{
				string keyword = usingCardFlag.GetKeyword();
				builder.AppendFormat("[{0}] ", keyword);
			}
		}
		builder.Append(ConstValue.KEYWORD_COLOR_TAG_E);
	}

	public string GetCardType()
	{
		switch (cardType)
		{
		case CardType.Action:
		case CardType.GeneralMove:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Action);
		case CardType.UseEquip:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Combat);
		case CardType.Skill:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Skill);
		case CardType.Equipment:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Equipment);
		case CardType.Item:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Item);
		case CardType.Power:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Power);
		case CardType.Status:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Status);
		case CardType.Trap:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Trap);
		case CardType.Trinket:
			return StringDB_CardType.instance.GetLocString(StringDB_CardType_Keys.Trinket);
		default:
			Debug.LogWarning("[CardData] GetCardType fail: " + cardType);
			return "None";
		}
	}

	public bool HasAnySubTypes(params CardSubType[] subTypes)
	{
		if (subTypes == null)
		{
			return false;
		}
		foreach (CardSubType cardSubType in subTypes)
		{
			if (this.cardSubType.HasFlag(cardSubType))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnySchoolTypes(CardSchoolType[] schoolTypes)
	{
		if (schoolTypes == null)
		{
			return false;
		}
		foreach (CardSchoolType cardSchoolType in schoolTypes)
		{
			if (cardSchoolTypes.HasFlag(cardSchoolType))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsShowGeneralCardImage()
	{
		if (IsItem())
		{
			return false;
		}
		if (IsTrinket())
		{
			return false;
		}
		return true;
	}

	public bool IsEquipment()
	{
		return cardType == CardType.Equipment;
	}

	public bool IsItem()
	{
		if (cardType != CardType.Equipment && cardType != CardType.Item)
		{
			return cardType == CardType.Trap;
		}
		return true;
	}

	public bool IsTrinket()
	{
		return cardType == CardType.Trinket;
	}

	public bool IsTrinketCharm()
	{
		if (cardType != CardType.Trinket)
		{
			return false;
		}
		if (cardRare != CardRare.SuperRare)
		{
			return false;
		}
		return true;
	}

	private bool IsShowWarningEquipVisual()
	{
		if (cardType != CardType.Equipment && equipVisual != 0)
		{
			return true;
		}
		return false;
	}

	public bool IsIncreaseValueError()
	{
		if (HasIncreaseFunction() || isIncreaseCard)
		{
			return increase <= 0;
		}
		return increase != 0;
	}

	public bool IsShowIncrease()
	{
		if (!IsEquipment())
		{
			return false;
		}
		if (!HasIncreaseFunction() && !isIncreaseCard)
		{
			return false;
		}
		return true;
	}

	private string GetCardRareTitle()
	{
		if (floatPrice >= 0)
		{
			return $"PRICE: {GetGeneralPrice()} +{floatPrice} = {GetPrice(isAutoFloat: false)}, SELL_PRICE: {GetSellPrice()}";
		}
		return $"PRICE: {GetGeneralPrice()} {floatPrice} = {GetPrice(isAutoFloat: false)}, SELL_PRICE: {GetSellPrice()}";
	}

	private int GetPriceByDefaultCard()
	{
		return cardRare switch
		{
			CardRare.Normal => 120, 
			CardRare.Rare => 180, 
			CardRare.SuperRare => 240, 
			_ => 10, 
		};
	}

	private int GetPriceByEquipment()
	{
		return cardRare switch
		{
			CardRare.Normal => 80, 
			CardRare.Rare => 150, 
			CardRare.SuperRare => 250, 
			_ => 10, 
		};
	}

	private int GetPriceByItem()
	{
		return cardRare switch
		{
			CardRare.Normal => 20, 
			CardRare.Rare => 40, 
			CardRare.SuperRare => 80, 
			_ => 10, 
		};
	}

	private int GetPriceByTrap()
	{
		return cardRare switch
		{
			CardRare.Normal => 20, 
			CardRare.Rare => 40, 
			CardRare.SuperRare => 80, 
			_ => 10, 
		};
	}

	private int GetGeneralPrice()
	{
		return cardType switch
		{
			CardType.Equipment => GetPriceByEquipment(), 
			CardType.Item => GetPriceByItem(), 
			CardType.Trap => GetPriceByTrap(), 
			_ => GetPriceByDefaultCard(), 
		};
	}

	public int GetPrice(bool isAutoFloat = true)
	{
		int generalPrice = GetGeneralPrice();
		generalPrice += floatPrice;
		if (isAutoFloat)
		{
			int minInclusive = Mathf.RoundToInt((float)generalPrice * 0.9f);
			int maxExclusive = Mathf.RoundToInt((float)generalPrice * 1.1f);
			generalPrice = UnityEngine.Random.Range(minInclusive, maxExclusive);
		}
		return Mathf.Clamp(generalPrice, 1, ConstValue.MAX_PRICE);
	}

	public int GetSellPrice()
	{
		int generalPrice = GetGeneralPrice();
		generalPrice += floatPrice;
		return Mathf.Clamp(cardType switch
		{
			CardType.Equipment => Mathf.RoundToInt((float)generalPrice * 0.05f), 
			_ => Mathf.RoundToInt((float)generalPrice * 0.1f), 
		}, 1, ConstValue.MAX_PRICE);
	}

	public int GetKillCoinPrice()
	{
		CardRare cardRare = this.cardRare;
		if (cardRare == CardRare.Rare || cardRare == CardRare.SuperRare)
		{
			return 2;
		}
		return 1;
	}

	public bool HasAnyCardFlag(CardFlagType checkCardFlag)
	{
		if (cardFlags.Find((CardFlag o) => o.flag == checkCardFlag) == null)
		{
			return false;
		}
		return true;
	}

	public bool ValidateAddCharaFlagsToSrc()
	{
		if (cardType != CardType.Power)
		{
			return false;
		}
		if (GetSprite() == null)
		{
			return true;
		}
		return false;
	}

	public Sprite GetSprite()
	{
		if (cardType != CardType.Power)
		{
			return null;
		}
		if (addCharaFlagsToSrcChara == null || addCharaFlagsToSrcChara.Count <= 0)
		{
			return null;
		}
		if (addCharaFlagsToSrcChara[0].flagData == null)
		{
			return null;
		}
		return addCharaFlagsToSrcChara[0].flagData.sprite;
	}

	private bool IsWarningForWeaponActMethod()
	{
		return !ValidateActionMethod();
	}

	public bool ValidateActionMethod()
	{
		if (cardType == CardType.Equipment)
		{
			if (weaponActMethod == null)
			{
				return false;
			}
			if (actionMethod == null)
			{
				return false;
			}
		}
		else if (weaponActMethod != null)
		{
			return false;
		}
		return true;
	}

	private bool ValidateIncrease()
	{
		if (HasIncreaseFunction() && isIncreaseCard)
		{
			return true;
		}
		return false;
	}

	public bool HasIncreaseFunction()
	{
		return increaseCard != null;
	}

	public string GetIncreaseDesc()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string srcDesc = increaseDesc;
		if (localization != null)
		{
			srcDesc = localization.increaseDesc;
		}
		AddDesc(stringBuilder, srcDesc);
		return stringBuilder.ToString();
	}

	public string GetOriginIncreaseDesc()
	{
		if (!isIncreaseCard)
		{
			return null;
		}
		return increaseDesc;
	}

	public string GetIncreaseDescRef(string srcDesc)
	{
		if (!isIncreaseCard)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		AddDesc(stringBuilder, srcDesc);
		return stringBuilder.ToString();
	}

	public bool HasLevelup()
	{
		CardType cardType = this.cardType;
		if ((uint)(cardType - 3) <= 2u || (uint)(cardType - 7) <= 2u)
		{
			return false;
		}
		if (isLevelupCard)
		{
			return false;
		}
		if (isHasNoLevelup)
		{
			return false;
		}
		return true;
	}

	public bool CanLevelup()
	{
		if (!HasLevelup())
		{
			return false;
		}
		return levelupCard != null;
	}

	private bool IsShowErrorForLevelup()
	{
		if (HasLevelup())
		{
			if (levelupCard == null)
			{
				return true;
			}
		}
		else if (isLevelupCard)
		{
			if (levelupCard == null)
			{
				return true;
			}
		}
		else if (levelupCard != null)
		{
			return true;
		}
		return false;
	}

	public static CardRare GetCardDataByRate(Dictionary<CardRare, int> rateArray)
	{
		int num = 0;
		foreach (KeyValuePair<CardRare, int> item in rateArray)
		{
			num += item.Value;
		}
		foreach (KeyValuePair<CardRare, int> item2 in rateArray)
		{
			if (UnityEngine.Random.Range(0, num) < item2.Value)
			{
				return item2.Key;
			}
			num -= item2.Value;
		}
		Debug.LogWarning("[CardData] GetCardDataByRate fail");
		return CardRare.Normal;
	}

	public bool IsSameData(CardData checkCard)
	{
		if (this == checkCard)
		{
			return true;
		}
		if (checkCard.levelupCard != null && this == checkCard.levelupCard)
		{
			return true;
		}
		if (levelupCard != null && levelupCard == checkCard)
		{
			return true;
		}
		if (levelupCard != null && checkCard.levelupCard != null && levelupCard == checkCard.levelupCard)
		{
			return true;
		}
		return false;
	}
}
