// Survivor.UICardItemBorder
using Survivor;
using UnityEngine;
using UnityEngine.UI;

public class UICardItemBorder : Image
{
	public AutoShine autoShine;

	private Material modifiedMat;

	protected override void Awake()
	{
		autoShine = GetComponent<AutoShine>();
		base.Awake();
	}

	protected override void OnDisable()
	{
		modifiedMat = null;
		base.OnDisable();
	}

	private void Update()
	{
		if (!(modifiedMat == null) && !(autoShine == null) && !(autoShine.usingMaterial == null) && !(modifiedMat == autoShine.usingMaterial))
		{
			string text = "_Color";
			Color value = autoShine.usingMaterial.GetColor(text);
			modifiedMat.SetColor(text, value);
		}
	}

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		modifiedMat = base.GetModifiedMaterial(baseMaterial);
		return modifiedMat;
	}
}
