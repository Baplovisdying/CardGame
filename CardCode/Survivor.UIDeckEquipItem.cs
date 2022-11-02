// Survivor.UIDeckEquipItem
using Survivor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIDeckEquipItem : MonoBehaviour, IDropHandler, IEventSystemHandler, IPointerClickHandler
{
	public delegate void OnBorderPointCallback(UIDeckEquipItem cardItem, PointerEventData eventData);

	[SerializeField]
	private SpriteAtlas slotAtlas;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private Image hintArrow;

	[SerializeField]
	private Image hintBorder;

	[SerializeField]
	public UICardItem cardItem;

	[SerializeField]
	public Image colorBlindImg;

	public OnBorderPointCallback onDrop;

	public OnBorderPointCallback onClick;

	public void SetSlotScaleX(int x)
	{
		Vector3 localScale = bg.transform.localScale;
		localScale.x = x;
		bg.transform.localScale = localScale;
		colorBlindImg.transform.localScale = localScale;
	}

	public void SetSlotImage(UICardItemSlotType slotType)
	{
		bg.sprite = slotAtlas.GetSprite(slotType.ToString());
	}

	public void SetBgColor(Color color)
	{
		bg.color = color;
		color *= ConstValue.EQUIP_HINT_COLOR_MULTIPLY_RATE;
		hintBorder.color = color;
		hintArrow.color = color;
		colorBlindImg.color = color;
	}

	public void ShowHintBorder()
	{
		hintBorder.enabled = true;
		hintArrow.enabled = true;
	}

	public void HideHintBorder()
	{
		hintBorder.enabled = false;
		hintArrow.enabled = false;
	}

	void IDropHandler.OnDrop(PointerEventData eventData)
	{
		if (onDrop != null)
		{
			onDrop(this, eventData);
		}
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && onClick != null)
		{
			onClick(this, eventData);
		}
	}
}
