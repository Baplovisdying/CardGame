// Survivor.UIDeckHotkeyController
using Survivor;

public class UIDeckHotkeyController : UIHotkeyControllerBase<UIDeckController>
{
	public override void SetHotkeyCallbacks()
	{
		SetHotkeyCallback(Hotkey.UIGeneral_Exit, OnExit);
	}

	public override void UpdateHotkeysVisible(bool isShow)
	{
	}

	private void OnExit()
	{
		uiCtrl.DoExit();
	}
}
