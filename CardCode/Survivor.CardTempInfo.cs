// Survivor.CardTempInfo
using System.Collections.Generic;
using Survivor;

public class CardTempInfo
{
	public int addEnergy;

	public int addMovPower;

	public int addAttPower;

	public int addDefPower;

	public int extraIncrease;

	public List<CardFlag> cardFlags = new List<CardFlag>();

	public void Reset()
	{
		addEnergy = 0;
		addMovPower = 0;
		addAttPower = 0;
		addDefPower = 0;
		extraIncrease = 0;
		cardFlags.Clear();
	}

	public CardTempInfo ShallowCopy()
	{
		CardTempInfo obj = (CardTempInfo)MemberwiseClone();
		obj.cardFlags = new List<CardFlag>(cardFlags);
		return obj;
	}
}
