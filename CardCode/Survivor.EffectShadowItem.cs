// Survivor.EffectShadowItem
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MWUtil;
using Survivor;
using UnityEngine;
using UnityEngine.U2D;

public class EffectShadowItem : MonoBehaviour
{
	[SerializeField]
	public SpriteRenderer img;

	[SerializeField]
	public SpriteAtlas atlas;

	public void SetSprite(ShadowType shadowType)
	{
		Sprite sprite = atlas.GetSprite(shadowType.ToString());
		img.sprite = sprite;
	}

	public Sequence PlayMove(ShadowType shadowType, EffectController effectCtrl, Vector3 srcPos, Vector3 dstPos, float duration, bool isFadeOut, AnimationCurve curve = null)
	{
		img.SetAlpha(1f);
		base.transform.position = srcPos;
		SetSprite(shadowType);
		Sequence sequence = DOTween.Sequence();
		TweenerCore<Vector3, Vector3, VectorOptions> t = base.transform.DOMove(dstPos, duration);
		if (curve != null)
		{
			t.SetEase(curve);
		}
		sequence.Append(t);
		if (isFadeOut)
		{
			float num = duration * 0.8f;
			float duration2 = duration - num;
			sequence.Insert(num, img.DOFade(0f, duration2));
		}
		sequence.OnComplete(delegate
		{
			effectCtrl.RecycleShadowEffect(this);
		});
		return sequence;
	}

	public void PlayFalldown(ShadowType shadowType, Vector3 dstPos, float duration)
	{
		base.transform.position = dstPos;
		SetSprite(shadowType);
		img.SetAlpha(0f);
		img.DOFade(1f, duration);
		base.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
		base.transform.DOScale(Vector3.one, duration);
	}
}
