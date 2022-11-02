// Survivor.AddCardInfo
using Survivor;
using UnityEngine;

public class AddCardInfo
{
	public Card card;

	public GameObject fromObject;

	public UICardItem cardItem;

	public AddCardInfo(Card card)
	{
		this.card = card;
	}

	public AddCardInfo(Card card, GameObject fromObj)
	{
		this.card = card;
		fromObject = fromObj;
	}
}
