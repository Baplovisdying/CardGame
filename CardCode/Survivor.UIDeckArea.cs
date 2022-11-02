// Survivor.UIDeckArea
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDeckArea : MonoBehaviour, IDropHandler, IEventSystemHandler, IPointerClickHandler
{
	public Action onDrop;

	public Action onClick;

	void IDropHandler.OnDrop(PointerEventData eventData)
	{
		if (onDrop != null)
		{
			onDrop();
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		if (onClick != null)
		{
			onClick();
		}
	}
}
