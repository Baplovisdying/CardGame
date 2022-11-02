// Survivor.UIDeckEquipHotkeyController
using Survivor;

public class UIDeckEquipHotkeyController : UIHotkeyControllerBase<UIDeckEquipController>
{
	public override void SetHotkeyCallbacks()
	{
		SetHotkeyCallback(Hotkey.UIGeneral_Exit, OnOptionExit);
		SetHotkeyCallback(Hotkey.UIGeneral_Deck, OnDeck);
	}

	private void OnDeck()
	{
		uiCtrl.OnDeckBtnClick();
	}

	private void OnOptionExit()
	{
		uiCtrl.DoExit();
	}

	public override void UpdateHotkeysVisible(bool isShow)
	{
		if (uiCtrl == null)
		{
			return;
		}
		UIDeckEquipView view = uiCtrl.view;
		if (!(view == null))
		{
			view.deckHotkey.HideHotkey();
			if (isShow)
			{
				view.deckHotkey.SetHotkey(hotkeyDB.GetHotkeyHintString(Hotkey.UIGeneral_Deck));
				view.deckHotkey.ShowHotkey();
			}
		}
	}
}
