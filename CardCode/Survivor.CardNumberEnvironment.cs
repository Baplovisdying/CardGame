// Survivor.CardNumberEnvironment
using Survivor;

public class CardNumberEnvironment
{
	public CharacterBase srcChara;

	public CharacterBase dstChara;

	public ArenaInfo arenaInfo;

	public CardNumberEnvironment(CharacterBase srcChara, CharacterBase dstChara, ArenaInfo arenaInfo)
	{
		this.srcChara = srcChara;
		this.dstChara = dstChara;
		this.arenaInfo = arenaInfo;
	}

	public bool IsNeedReload(Card card)
	{
		if (srcChara == null)
		{
			return false;
		}
		CharacterEquipment usingEquipment = srcChara.GetUsingEquipment(card);
		if (usingEquipment != null && usingEquipment.card != null)
		{
			return usingEquipment.card.IsNeedReload();
		}
		return false;
	}

	public int GetMultiAttackCount(Card card)
	{
		if (srcChara == null)
		{
			return card.GetMultiAttackCount();
		}
		CharacterEquipment usingEquipment = srcChara.GetUsingEquipment(card);
		if (usingEquipment == null)
		{
			return card.GetMultiAttackCount();
		}
		return new UsingCardFlags(card, usingEquipment)?.GetMultiAttackCount() ?? card.GetMultiAttackCount();
	}
}
