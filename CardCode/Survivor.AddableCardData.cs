// Survivor.AddableCardData
using System;
using Survivor;

[Serializable]
public class AddableCardData
{
	public CardData cardData;

	public int amount = 1;

	public AddableCardData(CardData cardData, int amount)
	{
		this.cardData = cardData;
		this.amount = amount;
	}
}
