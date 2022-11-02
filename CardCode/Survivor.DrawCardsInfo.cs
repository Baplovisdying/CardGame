// Survivor.DrawCardsInfo
using System.Collections.Generic;
using Survivor;

public class DrawCardsInfo
{
	public List<Card> drawCards = new List<Card>();

	public bool isFull;

	public bool isResetDrawPile;

	public int resetDrawPileCardCnt;

	public bool isDisableDraw;

	public DrawCardsInfo next;
}
