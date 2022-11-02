// Survivor.UICardItemFrontImg
using UnityEngine;
using UnityEngine.UI;

public class UICardItemFrontImg : Image
{
	public Material modifiedMat;

	protected override void OnDisable()
	{
		modifiedMat = null;
		base.OnDisable();
	}

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		modifiedMat = base.GetModifiedMaterial(baseMaterial);
		return modifiedMat;
	}
}
