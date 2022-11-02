// Survivor.CardDataLocalization
using MWUtil;
using Survivor;

public class CardDataLocalization
{
	public string title = string.Empty;

	public string _titleRef;

	public string _titleOrign;

	public string desc = string.Empty;

	public string _descRef;

	public string _descOrigin;

	public string increaseDesc;

	public string _increaseDescRef;

	public string _increaseDescOrigin;

	public string note = string.Empty;

	public string status = string.Empty;

	public CardDataLocalization()
	{
	}

	public CardDataLocalization(CardData cardData)
	{
		title = cardData.GetOriginTitle();
		desc = cardData.GetOriginDesc();
		increaseDesc = cardData.GetOriginIncreaseDesc();
		note = string.Empty;
		status = LocalizationStatusHelper.GetStatusStr(cardData.localizationStatus);
		CreateRefByGameData(cardData);
	}

	public CardDataLocalization(CardData cardData, CardDataLocalization lastData)
	{
		title = lastData.title;
		desc = lastData.desc;
		increaseDesc = lastData.increaseDesc;
		note = lastData.note;
		status = LocalizationStatusHelper.GetStatusStr(cardData.localizationStatus);
	}

	public void CreateRefByGameData(CardData cardData)
	{
		_titleRef = cardData.GetTitleRef(cardData.GetOriginTitle());
		_titleOrign = cardData.GetOriginTitle();
		if (_titleRef == _titleOrign)
		{
			_titleRef = null;
		}
		_descRef = cardData.GetDescRef(cardData.GetOriginDesc());
		_descOrigin = cardData.GetOriginDesc();
		_increaseDescRef = cardData.GetIncreaseDescRef(cardData.GetOriginIncreaseDesc());
		_increaseDescOrigin = cardData.GetOriginIncreaseDesc();
	}

	public void CreateRefByEnglish(CardData cardData, CardDataLocalization engLocData)
	{
		_titleRef = null;
		_titleOrign = engLocData.title;
		_descRef = null;
		_descOrigin = engLocData.desc;
		_increaseDescRef = null;
		_increaseDescOrigin = engLocData.increaseDesc;
	}
}
