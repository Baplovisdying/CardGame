// Survivor.AddCardInfos
using System.Collections.Generic;
using Survivor;
using UnityEngine;

public class AddCardInfos
{
	public List<AddCardInfo> infos = new List<AddCardInfo>();

	public AddCardInfos()
	{
	}

	public AddCardInfos(List<Card> cards)
	{
		AddInfos(cards, null);
	}

	public AddCardInfos(List<Card> cards, GameObject fromObj)
	{
		AddInfos(cards, fromObj);
	}

	public bool HasAnyInfos()
	{
		return infos.Count > 0;
	}

	public void AddInfos(List<Card> cards, GameObject fromObj)
	{
		for (int i = 0; i < cards.Count; i++)
		{
			Card card = cards[i];
			infos.Add(new AddCardInfo(card, fromObj));
		}
	}
}
