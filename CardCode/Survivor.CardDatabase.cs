// Survivor.CardDatabase
using System;
using System.Collections.Generic;
using System.Linq;
using MWUtil;
using Survivor;
using UnityEngine;

[CreateAssetMenu]
public class CardDatabase : ScriptableDatabase<CardData>, ILocalizableDatabase
{
	[SerializeField]
	private CardSubType[] subTypesDebug;

	[SerializeField]
	private int[] costFilter;

	[SerializeField]
	private CardSchoolType[] schoolTypesDebug;

	[NonSerialized]
	private List<DebugCardDataTable> debugTable = new List<DebugCardDataTable>();

	public string jsonFileName = "card";

	string ILocalizableDatabase.jsonFileName => jsonFileName;

	public List<CardData> GetAllDatas()
	{
		return new List<CardData>(datas);
	}

	public List<CardData> GetNeedUnlockDatas()
	{
		List<CardData> list = new List<CardData>();
		foreach (CardData data in datas)
		{
			if (!data.isLevelupCard && data.isLock)
			{
				list.Add(data);
			}
		}
		return list;
	}

	public List<CardData> GetCardDatas(CardRare[] rares, List<CardData> unlocked, CardSubType[] subTypes = null, List<CardData> exclude = null, bool isIncludeLevelup = false)
	{
		IEnumerable<CardData> enumerable = null;
		enumerable = ((!isIncludeLevelup) ? datas.Where((CardData o) => rares.Contains(o.cardRare) && !o.isLevelupCard) : datas.Where((CardData o) => rares.Contains(o.cardRare)));
		if (subTypes != null)
		{
			enumerable = enumerable.Where((CardData o) => o.HasAnySubTypes(subTypes));
		}
		if (exclude != null)
		{
			enumerable = enumerable.Where((CardData o) => !exclude.Contains(o));
		}
		if (unlocked != null)
		{
			enumerable = enumerable.Where((CardData o) => !o.isLock || unlocked.Contains(o));
		}
		return enumerable.ToList();
	}

	public CardData GetCardData(string cardKey)
	{
		return datas.Find((CardData obj) => obj.name == cardKey);
	}

	private void DebugFetchCardDatasBy(CardFlagType cardFlagFilter)
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o != null && o.cardFlags.Any((CardFlag f) => f.flag == cardFlagFilter)))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	private void DebugFetchCardDatasBySubTypes()
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o != null && o.HasAnySubTypes(subTypesDebug) && costFilter.Contains(o.point)))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	private void DebugFetchCardDatasBySchoolType()
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o != null && o.HasAnySchoolTypes(schoolTypesDebug)))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	public void DebugFetchCardDatasBy()
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o != null))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	public void DebugFetchCardDatasBy(CardType cardType)
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o != null && o.cardType == cardType))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	public void DebugFetchCardDatasBy(CardRare cardRare)
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o != null && o.cardRare == cardRare))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	private void AddDebugTable(CardData cardData)
	{
		if (debugTable.Find((DebugCardDataTable o) => o.cardData == cardData || o.lvUpCardData == cardData) == null)
		{
			debugTable.Add(new DebugCardDataTable(cardData));
		}
	}

	private void DebugSortByRare()
	{
		if (debugTable == null)
		{
			return;
		}
		debugTable.Sort(delegate(DebugCardDataTable a, DebugCardDataTable b)
		{
			if (a.rare < b.rare)
			{
				return -1;
			}
			return (a.rare > b.rare) ? 1 : 0;
		});
	}

	private void GetCardDatasByIsLockDemo()
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o._isLockForDemo))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	private void GetCardDatasByIsAllLock()
	{
		debugTable.Clear();
		foreach (CardData item in datas.Where((CardData o) => o._isLock || o._isLockForDemo))
		{
			AddDebugTable(item);
		}
		DebugSortByRare();
	}

	public List<CardData> GetCardDatasByHasCharacterFlagPlus()
	{
		List<CardData> list = new List<CardData>();
		foreach (CardData data in datas)
		{
			foreach (CardFlag cardFlag in data.cardFlags)
			{
				if (!(cardFlag.characterFlagDataRef == null))
				{
					list.Add(data);
				}
			}
		}
		return list;
	}

	public List<CardData> GetCardDatasByStringOfDesc(string strOfDesc)
	{
		return datas.Where((CardData o) => o != null && o.GetOriginDesc().Contains(strOfDesc)).ToList();
	}

	public List<CardData> GetCardDatasByDstCharacterFlags(CharacterFlagData charaFlagData)
	{
		return datas.Where((CardData o) => o != null && o.addCharaFlagsToDstCharas.Find((AddCharacterFlag oo) => oo.flagData == charaFlagData) != null).ToList();
	}

	public List<CardData> GetCardDatasBySrcCharacterFlags(CharacterFlagData charaFlagData)
	{
		return datas.Where((CardData o) => o != null && o.addCharaFlagsToSrcChara.Find((AddCharacterFlag oo) => oo.flagData == charaFlagData) != null).ToList();
	}

	public void ResetLocalizationStatus()
	{
	}

	void ILocalizableDatabase.Init()
	{
	}

	Type ILocalizableDatabase.GetLocalizationDataType()
	{
		return typeof(Dictionary<string, CardDataLocalization>);
	}

	object ILocalizableDatabase.GetLocalizationDatas()
	{
		Dictionary<string, CardDataLocalization> dictionary = new Dictionary<string, CardDataLocalization>();
		foreach (CardData data in datas)
		{
			CardDataLocalization value = new CardDataLocalization(data);
			dictionary.Add(data.name, value);
		}
		return dictionary;
	}

	void ILocalizableDatabase.SetLocalizationDatas(object serializeData)
	{
		Dictionary<string, CardDataLocalization> dictionary = (Dictionary<string, CardDataLocalization>)serializeData;
		foreach (CardData data in datas)
		{
			if (data == null)
			{
				if (Application.isEditor)
				{
					Debug.LogError("[CardDatabase] SetLocalizationDatas data is null ");
				}
				else
				{
					Debug.LogWarning("[CardDatabase] SetLocalizationDatas data is null ");
				}
				continue;
			}
			CardDataLocalization value = null;
			if (!dictionary.TryGetValue(data.name, out value))
			{
				if (Application.isEditor)
				{
					Debug.LogError("[CardDatabase] SetLocalizationDatas missing data: " + data.name);
				}
				else
				{
					Debug.LogWarning("[CardDatabase] SetLocalizationDatas missing data: " + data.name);
				}
			}
			else
			{
				data.localization = value;
			}
		}
	}

	object ILocalizableDatabase.UpdateLocalizationDatas(object oldData, object engData)
	{
		Dictionary<string, CardDataLocalization> dictionary = new Dictionary<string, CardDataLocalization>();
		Dictionary<string, CardDataLocalization> dictionary2 = (Dictionary<string, CardDataLocalization>)oldData;
		Dictionary<string, CardDataLocalization> dictionary3 = null;
		if (engData != null)
		{
			dictionary3 = (Dictionary<string, CardDataLocalization>)engData;
		}
		foreach (CardData data in datas)
		{
			CardDataLocalization value = null;
			if (dictionary2.TryGetValue(data.name, out value))
			{
				CardDataLocalization cardDataLocalization = new CardDataLocalization(data, value);
				if (dictionary3 == null)
				{
					cardDataLocalization.CreateRefByGameData(data);
				}
				else
				{
					CardDataLocalization value2 = null;
					if (dictionary3.TryGetValue(data.name, out value2))
					{
						cardDataLocalization.CreateRefByEnglish(data, value2);
					}
				}
				dictionary.Add(data.name, cardDataLocalization);
				dictionary2.Remove(data.name);
			}
			else
			{
				dictionary.Add(data.name, new CardDataLocalization(data));
			}
		}
		return dictionary;
	}

	void ILocalizableDatabase.CleanLocalization()
	{
		foreach (CardData data in datas)
		{
			data.CleanLocalization();
		}
	}

	int ILocalizableDatabase.GetWordsCount(object serializeData)
	{
		Dictionary<string, CardDataLocalization> dictionary = (Dictionary<string, CardDataLocalization>)serializeData;
		int num = 0;
		char[] separator = new char[1] { ' ' };
		foreach (KeyValuePair<string, CardDataLocalization> item in dictionary)
		{
			CardDataLocalization value = item.Value;
			if (value.title != null)
			{
				num += value.title.Split(separator).Length;
			}
			if (value.desc != null)
			{
				num += value.desc.Split(separator).Length;
			}
			if (value.increaseDesc != null)
			{
				num += value.increaseDesc.Split(separator).Length;
			}
		}
		return num;
	}

	int ILocalizableDatabase.GetCharactersCount(object serializeData)
	{
		Dictionary<string, CardDataLocalization> obj = (Dictionary<string, CardDataLocalization>)serializeData;
		int num = 0;
		foreach (KeyValuePair<string, CardDataLocalization> item in obj)
		{
			CardDataLocalization value = item.Value;
			if (value.title != null)
			{
				num += value.title.Length;
			}
			if (value.desc != null)
			{
				num += value.desc.Length;
			}
			if (value.increaseDesc != null)
			{
				num += value.increaseDesc.Length;
			}
		}
		return num;
	}

	void ILocalizableDatabase.FetchUniqueCharacters(object serializeData, ref HashSet<char> uniqueCharacters)
	{
		foreach (KeyValuePair<string, CardDataLocalization> item4 in (Dictionary<string, CardDataLocalization>)serializeData)
		{
			CardDataLocalization value = item4.Value;
			if (value.title != null)
			{
				string title = value.title;
				foreach (char item in title)
				{
					uniqueCharacters.Add(item);
				}
			}
			if (value.desc != null)
			{
				string title = value.desc;
				foreach (char item2 in title)
				{
					uniqueCharacters.Add(item2);
				}
			}
			if (value.increaseDesc != null)
			{
				string title = value.increaseDesc;
				foreach (char item3 in title)
				{
					uniqueCharacters.Add(item3);
				}
			}
		}
	}

	void ILocalizableDatabase.ValidateLocalizationDatas(object serializeData)
	{
		Dictionary<string, CardDataLocalization> dictionary = (Dictionary<string, CardDataLocalization>)serializeData;
		foreach (CardData data in datas)
		{
			if (data == null)
			{
				Debug.LogError("[CardDatabase] ValidateLocalizationDatas data is null ");
				continue;
			}
			CardDataLocalization value = null;
			if (!dictionary.TryGetValue(data.name, out value))
			{
				Debug.LogError("[CardDatabase] ValidateLocalizationDatas missing data: " + data.name);
				continue;
			}
			string validateDesc = null;
			if (!data.ValidateDesc(value.desc, out validateDesc))
			{
				Debug.LogErrorFormat("[CardDatabase] ValidateLocalizationDatas desc fail: [{0}][{1}]", data.name, validateDesc);
			}
			validateDesc = null;
			if (value.increaseDesc != null && !data.ValidateDesc(value.increaseDesc, out validateDesc))
			{
				Debug.LogErrorFormat("[CardDatabase] ValidateLocalizationDatas increaseDesc fail: [{0}][{1}]", data.name, validateDesc);
			}
		}
	}
}
