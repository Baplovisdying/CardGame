// Survivor.UICardOfThisTileController
using DG.Tweening;
using Survivor;
using UnityEngine;

public class UICardOfThisTileController : MonoBehaviour
{
	[SerializeField]
	public CanvasGroup rootCanvasGroup;

	[SerializeField]
	public UICardItem cardItem;

	[SerializeField]
	public RectTransform anchorOfKeyword;

	public void Init()
	{
		rootCanvasGroup.alpha = 0f;
	}

	public void Show(Card card)
	{
		cardItem.ShowCard();
		cardItem.SetView(card.data);
		cardItem.UpdateCardVisual(card);
		DOTween.Kill(rootCanvasGroup);
		rootCanvasGroup.alpha = 0f;
		rootCanvasGroup.DOFade(1f, 0.35f);
	}

	public void Hide()
	{
		rootCanvasGroup.DOFade(0f, 0.35f);
	}

	public Vector3 GetPosOfKeyword()
	{
		return anchorOfKeyword.position;
	}
}
