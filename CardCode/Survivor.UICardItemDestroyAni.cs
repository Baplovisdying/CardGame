// Survivor.UICardItemDestroyAni
using DG.Tweening;
using Survivor;
using UnityEngine;
using UnityEngine.UI;

public class UICardItemDestroyAni : MonoBehaviour
{
	private static string maskFadeAmountKey = "_FadeAmount";

	[SerializeField]
	private UICardItemMask cardMask;

	[SerializeField]
	private Image cardMaskImg;

	[SerializeField]
	private UICardItemMaskImg cardMaskFrontImg;

	private readonly float maskFadeValueMin = -0.1f;

	private readonly float maskFadeValueMax = 1f;

	[SerializeField]
	[Range(-0.1f, 1f)]
	private float maskFadeValue;

	[SerializeField]
	private bool isInit;

	private Material _cardMaskMat;

	private Material _cardMaskFrontMat;

	private Tweener maskFadeValueTweener;

	private void CreateNewMaterials()
	{
		if (_cardMaskMat == null)
		{
			_cardMaskMat = new Material(cardMaskImg.material);
			Debug.Log("[UICardItem] Create _cardMaskMat: " + base.gameObject.name);
		}
		cardMaskImg.material = _cardMaskMat;
		if (_cardMaskFrontMat == null)
		{
			_cardMaskFrontMat = new Material(cardMaskFrontImg.material);
			Debug.Log("[UICardItem] Create _cardMaskFrontMat: " + base.gameObject.name);
		}
		cardMaskFrontImg.material = _cardMaskFrontMat;
	}

	private void Enable()
	{
		base.enabled = true;
		cardMask.enabled = true;
		cardMaskImg.enabled = true;
		cardMaskFrontImg.enabled = true;
	}

	public void Disable()
	{
		cardMask.enabled = false;
		cardMaskImg.enabled = false;
		cardMaskFrontImg.enabled = false;
		base.enabled = false;
	}

	public void Stop()
	{
		if (maskFadeValueTweener != null)
		{
			DOTween.Kill(maskFadeValueTweener);
			maskFadeValueTweener = null;
		}
	}

	public void Play(float duration, TweenCallback callback)
	{
		if (!isInit)
		{
			isInit = true;
			CreateNewMaterials();
		}
		Enable();
		maskFadeValue = maskFadeValueMin;
		_cardMaskMat.SetFloat(maskFadeAmountKey, maskFadeValueMin);
		_cardMaskFrontMat.SetFloat(maskFadeAmountKey, maskFadeValueMin);
		maskFadeValueTweener = DOVirtual.Float(maskFadeValueMin, maskFadeValueMax, duration, OnMaskFadeValueUpdate).OnComplete(callback);
	}

	private void OnMaskFadeValueUpdate(float value)
	{
		maskFadeValue = value;
	}

	private void Update()
	{
		if (!(_cardMaskMat == null) && !(_cardMaskFrontMat == null))
		{
			_cardMaskMat.SetFloat(maskFadeAmountKey, maskFadeValue);
			_cardMaskFrontMat.SetFloat(maskFadeAmountKey, maskFadeValue);
		}
	}
}
