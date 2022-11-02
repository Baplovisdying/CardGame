// Survivor.UIDeckScrollItem
using System;
using MWUtil;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDeckScrollItem : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IDragHandler
{
	[SerializeField]
	public ObjectPool deckCardPool;

	private Action onDeckPanelClick;

	private Action onDeckScrollDrag;

	public void SetClickCallback(Action onDeckPanelClick)
	{
		this.onDeckPanelClick = onDeckPanelClick;
	}

	public void SetDragCallback(Action onDeckScrollDrag)
	{
		this.onDeckScrollDrag = onDeckScrollDrag;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && onDeckPanelClick != null)
		{
			onDeckPanelClick();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (onDeckScrollDrag != null)
		{
			onDeckScrollDrag();
		}
	}
}
