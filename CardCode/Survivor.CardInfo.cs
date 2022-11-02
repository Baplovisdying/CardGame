// Survivor.CardInfo
using System;
using Survivor;

[Serializable]
public class CardInfo
{
	public string key = string.Empty;

	public CardAttackColor attColor;

	public int increase;

	public int addMovPower;

	public int addAttPower;

	public int addDefPower;

	public int addIncreaseMax;

	public int adjustEnergy;

	public int upgradeTimes;

	public int upgradeIncreaseTimes;

	private string GetFoldoutGroupStr()
	{
		string arg = string.Empty;
		switch (attColor)
		{
		case CardAttackColor.Left:
			arg = "(L)";
			break;
		case CardAttackColor.Right:
			arg = "(R)";
			break;
		}
		return $"{key}{arg}";
	}

	public CardInfo(string key)
	{
		this.key = key;
	}

	public CardInfo ShallowCopy()
	{
		return (CardInfo)MemberwiseClone();
	}

	public void SetSubEnergy(CardData data, int subValue)
	{
		adjustEnergy -= subValue;
		if (adjustEnergy < -data.point)
		{
			adjustEnergy = -data.point;
		}
	}

	public void AddAttPower(int addValue)
	{
		addAttPower += addValue;
	}

	public int GetUpgradeValue()
	{
		return Math.Max(addAttPower, addDefPower);
	}

	public int GetUpgradeTimes()
	{
		return upgradeTimes;
	}

	public int GetUpgradeIncreaseMax()
	{
		return addIncreaseMax;
	}

	public int GetUpgradeIncreaseTimes()
	{
		return upgradeIncreaseTimes;
	}
}
