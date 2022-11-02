// Survivor.UIEffectLightLineController
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MWUtil;
using Survivor;
using UnityEngine;

public class UIEffectLightLineController : MonoBehaviour
{
	[SerializeField]
	public ObjectPool uiEffectLightLinePool;

	public float playedDuration = 0.8f;

	public Color drawCardColor = new Color(0.5f, 1f, 0.5f);

	public Color discardCardColor = new Color(1f, 0.5f, 0.5f);

	public Color exhaustCardColor = new Color(1f, 0.5f, 0.5f);

	public Color getNewCardColor = new Color(1f, 1f, 1f);

	private UIRootController uiRootCtrl;

	private UIGamePlayController uiGamePlayCtrl;

	private AudioController audioCtrl;

	[SerializeField]
	public List<GameObject> testPos;

	[SerializeField]
	private int testNumOfPathPoints = 10;

	public void Init(UIRootController uiRootCtrl)
	{
		this.uiRootCtrl = uiRootCtrl;
		uiGamePlayCtrl = uiRootCtrl.uiGamePlayCtrl;
		audioCtrl = MainController.instance.audioCtrl;
	}

	public void PlayDrawCardEff(Vector3 srcPos, Vector3 dstPos, float duration)
	{
		StartCoroutine(PlayDrawCardProcess(srcPos, dstPos, duration));
	}

	private IEnumerator PlayDrawCardProcess(Vector3 srcPos, Vector3 dstPos, float duration)
	{
		UIEffectLightLine obj = uiEffectLightLinePool.GetPoolObject<UIEffectLightLine>();
		obj.PlayStraight(srcPos, dstPos, duration, drawCardColor);
		GameObject drawPileObj = uiGamePlayCtrl.GetDrawPileObj();
		DOTween.Kill(drawPileObj.transform);
		drawPileObj.transform.localScale = Vector3.one;
		drawPileObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.15f).From();
		yield return new WaitForSeconds(duration);
		obj.Stop();
		yield return new WaitForSeconds(0.6f);
		uiEffectLightLinePool.RecyclePoolObject(obj.gameObject);
	}

	public void PlayGetNewCardEffect(Vector3 srcPos, float duration = 0.8f)
	{
		GameObject ownCardsBtnObj = uiRootCtrl.uiPlayerInfoCtrl.GetOwnCardsBtnObj();
		StartCoroutine(PlayNewCardProcess(srcPos, ownCardsBtnObj.transform.position, ownCardsBtnObj, duration, getNewCardColor));
	}

	public void PlayGetNewCardEffectInStore(Vector3 srcPos, GameObject pileObj, float duration = 0.8f)
	{
		StartCoroutine(PlayNewCardProcess(srcPos, pileObj.transform.position, pileObj, duration, getNewCardColor));
	}

	private IEnumerator PlayNewCardProcess(Vector3 srcPos, Vector3 dstPos, GameObject dstObj, float duration, Color color)
	{
		UIEffectLightLine obj = uiEffectLightLinePool.GetPoolObject<UIEffectLightLine>();
		obj.PlayBeizer(GetNewCardBeizerPoints(srcPos, dstPos), dstObj, duration, color);
		yield return new WaitForSeconds(duration);
		obj.Stop();
		audioCtrl.PlaySFX(GameSFX.UIEffectLine_GetNewCard);
		DOTween.Kill(dstObj.transform);
		dstObj.transform.localScale = Vector3.one;
		dstObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.15f).From();
		yield return new WaitForSeconds(0.6f);
		uiEffectLightLinePool.RecyclePoolObject(obj.gameObject);
	}

	private List<Vector3> GetNewCardBeizerPoints(Vector3 srcPos, Vector3 dstPos)
	{
		return new List<Vector3>
		{
			srcPos,
			srcPos + new Vector3(1f + GetRandomOffset(0.8f), -4f + GetRandomOffset(1f), 0f),
			dstPos + new Vector3(-1f, -1f, 0f),
			dstPos
		};
	}

	public void PlayPlayedCardEff(UICardItem playedCardItem, PlayedCardToType dstTo)
	{
		if (!(playedCardItem == null))
		{
			Vector3 position = playedCardItem.transform.position;
			PlayToPile(position, dstTo, playedDuration);
		}
	}

	public void PlayPileToPile(PlayedCardToType src, PlayedCardToType dst, float duration)
	{
		GameObject gameObject = null;
		switch (src)
		{
		default:
			return;
		case PlayedCardToType.DiscardPile:
			gameObject = uiGamePlayCtrl.GetDiscardPileObj();
			break;
		case PlayedCardToType.ExhaustPile:
			gameObject = uiGamePlayCtrl.GetExhaustPileObj();
			break;
		case PlayedCardToType.DrawPile:
			gameObject = uiGamePlayCtrl.GetDrawPileObj();
			break;
		}
		PlayToPile(gameObject.transform.position, dst, duration);
	}

	public void PlayToPile(Vector3 srcPos, PlayedCardToType dstTo, float duration)
	{
		GameObject gameObject = null;
		Color white = Color.white;
		switch (dstTo)
		{
		default:
			return;
		case PlayedCardToType.DiscardPile:
			gameObject = uiGamePlayCtrl.GetDiscardPileObj();
			white = discardCardColor;
			break;
		case PlayedCardToType.ExhaustPile:
			gameObject = uiGamePlayCtrl.GetExhaustPileObj();
			white = exhaustCardColor;
			break;
		case PlayedCardToType.DrawPile:
			gameObject = uiGamePlayCtrl.GetDrawPileObj();
			white = drawCardColor;
			break;
		}
		List<Vector3> bottomIconsBeizerPoints = GetBottomIconsBeizerPoints(srcPos, gameObject.transform.position);
		StartCoroutine(PlayBeizerProcess(bottomIconsBeizerPoints, gameObject, duration, white));
	}

	private List<Vector3> GetBottomIconsBeizerPoints(Vector3 srcPos, Vector3 dstPos)
	{
		return new List<Vector3>
		{
			srcPos,
			srcPos + new Vector3(1.7f + GetRandomOffset(1.5f), 5f + GetRandomOffset(2f), 0f),
			dstPos + new Vector3(-1f, 2f, 0f),
			dstPos
		};
	}

	private float GetRandomOffset(float range)
	{
		return UnityEngine.Random.Range(0f - range, range);
	}

	private IEnumerator PlayBeizerProcess(List<Vector3> beizerPoints, GameObject dstObj, float duration, Color color)
	{
		UIEffectLightLine obj = uiEffectLightLinePool.GetPoolObject<UIEffectLightLine>();
		obj.PlayBeizer(beizerPoints, dstObj, duration, color);
		yield return new WaitForSeconds(duration);
		obj.Stop();
		DOTween.Kill(dstObj.transform);
		dstObj.transform.localScale = Vector3.one;
		dstObj.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.15f).From();
		yield return new WaitForSeconds(0.6f);
		uiEffectLightLinePool.RecyclePoolObject(obj.gameObject);
	}

	public void PlayLineToEffectProcess(Vector3 srcPos, Vector3 dstPos, Color color, float duration = 0.8f, Action onEnd = null)
	{
		StartCoroutine(PlayLineToEffect(srcPos, dstPos, duration, color, onEnd));
	}

	private IEnumerator PlayLineToEffect(Vector3 srcPos, Vector3 dstPos, float duration, Color color, Action onEnd = null)
	{
		UIEffectLightLine obj = uiEffectLightLinePool.GetPoolObject<UIEffectLightLine>();
		obj.PlayBeizer(GetNewCardBeizerPoints(srcPos, dstPos), duration, color);
		yield return new WaitForSeconds(duration);
		obj.Stop();
		onEnd?.Invoke();
		yield return new WaitForSeconds(0.6f);
		uiEffectLightLinePool.RecyclePoolObject(obj.gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		if (testPos == null || testPos.Count <= 0)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < testPos.Count; i++)
		{
			list.Add(testPos[i].transform.position);
		}
		List<Vector3> list2 = BezierCurve.Create(list, testNumOfPathPoints);
		for (int j = 0; j < list2.Count; j++)
		{
			int num = j + 1;
			if (num < list2.Count)
			{
				Gizmos.DrawLine(list2[j], list2[num]);
				Gizmos.DrawSphere(list2[j], 0.25f);
			}
		}
	}
}
