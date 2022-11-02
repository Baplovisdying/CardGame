// Survivor.UseCardActionMap
using Survivor;
using UnityEngine;

public class UseCardActionMap : ActionMapBase
{
	public override void CreateMap()
	{
		map = new CircleMap(srcTile, 0);
	}

	public override void ShowMap(bool disable)
	{
		Color cOLOR_USECARD = GameColorData.instance.COLOR_USECARD;
		if (disable)
		{
			symbolCtrl.ShowTileType(srcTile.transform.position, TileFlagType.TileActionRange, cOLOR_USECARD);
		}
		else
		{
			symbolCtrl.ShowTileType(srcTile.transform.position, TileFlagType.TileSelf, cOLOR_USECARD);
		}
	}

	public override bool ShowReadyMapAndAddPreMoveChars(TileUnit dstTile)
	{
		if (srcTile != dstTile)
		{
			return false;
		}
		symbolCtrl.ShowSelectTile(dstTile.transform.position, SymbolTileSelectType.UseCard);
		if (usingCardFlags.HasFlag(CardFlagType.HealSelf))
		{
			float totalValueOfFlag = usingCardFlags.GetTotalValueOfFlag(CardFlagType.HealSelf);
			ShowHealNumber(dstTile.charaBase, (int)totalValueOfFlag);
		}
		return true;
	}

	public override DoActionResultBundle DoAction(TileUnit dstTile, VisualActionsCommand visualActsCmd)
	{
		if (map.GetMapUnit(dstTile) == null)
		{
			return new DoActionResultBundle(DoActionResult.NotIncludeActionMap);
		}
		if (usingActMethod != null && usingActMethod.actionVisual != null)
		{
			ActionVisual actionVisual = usingActMethod.actionVisual;
			visualActsCmd.Add(PlayActionVisual(dstTile, actionVisual));
		}
		return new DoActionResultBundle(DoActionResult.DoUseCard);
	}
}
