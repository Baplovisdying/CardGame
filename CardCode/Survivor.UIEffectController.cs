// Survivor.UIEffectController
using System.Collections;
using System.Collections.Generic;
using MWUtil;
using Survivor;
using TMPro;
using UnityEngine;

public class UIEffectController : MonoBehaviour
{
	[SerializeField]
	private ObjectPool numberEffPool;

	[SerializeField]
	private RectTransform textEffRect;

	[SerializeField]
	private ObjectPool textEffPool;

	[SerializeField]
	private ObjectPool addCardEffPool;

	[SerializeField]
	private ObjectPool murmurPool;

	[SerializeField]
	private ObjectPool hintPowerMovePool;

	[SerializeField]
	private ObjectPool addTempUICardItemPool;

	[SerializeField]
	private RectTransform addTempUICardItemStartAnchor;

	[SerializeField]
	private ObjectPool waitInputMurmurPool;

	private GamePlayController gamePlayCtrl;

	public void Init()
	{
	}

	public void InitOnGamePlay(GamePlayController gamePlayCtrl)
	{
		this.gamePlayCtrl = gamePlayCtrl;
	}

	public void FreeOnGamePlay()
	{
		gamePlayCtrl = null;
	}

	public void ShowAddNumberEffect(Vector3 uiWorldPos, int number)
	{
		if (number != 0)
		{
			TextMeshProUGUI poolObject = numberEffPool.GetPoolObject<TextMeshProUGUI>();
			GameObject gameObject = poolObject.gameObject;
			poolObject.color = Color.yellow;
			poolObject.text = "+" + number;
			gameObject.transform.position = uiWorldPos;
			gameObject.SetActive(value: true);
		}
	}

	public void ShowSubNumberEffect(Vector3 uiWorldPos, int number)
	{
		if (number != 0)
		{
			TextMeshProUGUI poolObject = numberEffPool.GetPoolObject<TextMeshProUGUI>();
			GameObject gameObject = poolObject.gameObject;
			poolObject.color = Color.red;
			poolObject.text = "-" + number;
			gameObject.transform.position = uiWorldPos;
			gameObject.SetActive(value: true);
		}
	}

	private void PlayTextEffect(Vector3 screenPos, string str)
	{
		CameraController cameraCtrl = gamePlayCtrl.cameraCtrl;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(textEffRect, screenPos, cameraCtrl.uiCam, out var localPoint);
		UITextEffectItem poolObject = textEffPool.GetPoolObject<UITextEffectItem>();
		poolObject.Set(localPoint, str);
		poolObject.Play();
	}

	public void ShowTextEffect(string str)
	{
		Vector3 position = textEffPool.transform.position;
		ShowTextEffect(position, str);
	}

	public void ShowTextEffect(Vector3 uiPos, string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			Vector3 screenPointOnUICam = gamePlayCtrl.cameraCtrl.GetScreenPointOnUICam(uiPos);
			PlayTextEffect(screenPointOnUICam, str);
		}
	}

	public void ShowTextEffect(TileUnit dstTile, string str)
	{
		if (!(dstTile == null))
		{
			Vector3 screenPointOnMainCam = gamePlayCtrl.cameraCtrl.GetScreenPointOnMainCam(dstTile.transform.position);
			PlayTextEffect(screenPointOnMainCam, str);
		}
	}

	public void ShowHintPowerMoveEffect(Vector3 uiWorldPos)
	{
		GameObject poolObject = hintPowerMovePool.GetPoolObject();
		poolObject.transform.position = uiWorldPos;
		poolObject.SetActive(value: true);
	}

	public void PlayMurmurEffect(Vector3 srcPos, string murmur, Color color)
	{
		Vector3 position = gamePlayCtrl.cameraCtrl.ChangeMainCamPosToUICamPos(srcPos);
		GameObject poolObject = murmurPool.GetPoolObject();
		UIMurmurEffectItem component = poolObject.GetComponent<UIMurmurEffectItem>();
		component.SetText(murmur);
		component.SetColor(color);
		poolObject.transform.position = position;
		poolObject.SetActive(value: true);
	}

	public void PlayMurmurEffects(int playCnt, List<string> murmurs, Color color, bool isRemovePlayed = true)
	{
		StartCoroutine(PlayMurmurEffectProcess(playCnt, murmurs, color, isRemovePlayed));
	}

	private IEnumerator PlayMurmurEffectProcess(int playCnt, List<string> murmurs, Color color, bool isRemovePlayed)
	{
		List<Vector3> audiencePos = gamePlayCtrl.itemCtrl.GetAudiencePositions(playCnt);
		for (int i = 0; i < playCnt; i++)
		{
			string randomOne = murmurs.GetRandomOne();
			if (isRemovePlayed)
			{
				murmurs.Remove(randomOne);
			}
			if (!string.IsNullOrEmpty(randomOne) && i < audiencePos.Count)
			{
				Vector3 srcPos = audiencePos[i];
				PlayMurmurEffect(srcPos, randomOne, color);
				float seconds = Random.Range(0.1f, 0.35f);
				yield return new WaitForSeconds(seconds);
			}
		}
	}

	public bool PlayMurmurEffectForKill(int addAtmosphere)
	{
		bool result = false;
		int num = 1;
		List<string> list = null;
		Color white = Color.white;
		if (addAtmosphere >= ConstValue.ATMOSPHERE_KILL_LARGE_CEILING)
		{
			num = Mathf.Min(addAtmosphere, 6);
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.NicePlayLarge);
			white = GameColorData.instance.MURMUR_VERYGOOD;
			Debug.LogFormat("PlayMurmurEffectForKill Large addAtm[{0}] playCnt[{1}]", addAtmosphere, num);
		}
		else
		{
			if (RandomHelper.IsPass(50))
			{
				Debug.LogFormat("PlayMurmurEffectForKill but no pass [{0}]", addAtmosphere);
				return false;
			}
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.NicePlay);
			white = GameColorData.instance.MURMUR_GOOD;
			Debug.LogFormat("PlayMurmurEffectForKill addAtm[{0}] playCnt[{1}]", addAtmosphere, num);
		}
		PlayMurmurEffects(num, list, white);
		return result;
	}

	public void PlayMurmurEffectForComboOrHighDmg()
	{
		int playCnt = Random.Range(1, 3);
		List<string> stringGroup = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.NicePlayLarge);
		Color mURMUR_VERYGOOD = GameColorData.instance.MURMUR_VERYGOOD;
		Debug.Log("PlayMurmurEffectForComboOrHighDmg");
		PlayMurmurEffects(playCnt, stringGroup, mURMUR_VERYGOOD);
	}

	public bool PlayMurmurEffectForReturnAttack(int totalReturnDmg)
	{
		bool flag = false;
		int num = 1;
		List<string> list = null;
		Color white = Color.white;
		if (totalReturnDmg >= ConstValue.DAMAGE_NUMBER_LV2_FLOOR)
		{
			num = Random.Range(1, 4);
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.NicePlayLarge);
			white = GameColorData.instance.MURMUR_VERYGOOD;
			flag = true;
		}
		else
		{
			num = Random.Range(1, 2);
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.NicePlay);
			white = GameColorData.instance.MURMUR_GOOD;
		}
		Debug.LogFormat("PlayMurmurEffectForReturnAttack isLarge[{0}] totalReturnDmg[{1}]", flag, totalReturnDmg);
		PlayMurmurEffects(num, list, white);
		return flag;
	}

	public bool PlayMurmurEffectForKeepEscape(int keepEscapeRound, int subAtmos)
	{
		if (keepEscapeRound < ConstValue.KEEP_ESCAPE_CNT_MIN)
		{
			return false;
		}
		bool result = false;
		int num = 1;
		List<string> list = null;
		Color white = Color.white;
		if (keepEscapeRound >= ConstValue.KEEP_ESCAPE_CNT_LARGE_CEILING || subAtmos >= ConstValue.ATMOSPHERE_KEEP_ESCAPE_LARGE_CEILING)
		{
			num = keepEscapeRound;
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.KeepEscapeLarge);
			white = GameColorData.instance.MURMUR_BAD;
			if (keepEscapeRound % ConstValue.KEEP_ESCAPE_CNT_LARGE_CEILING == 0)
			{
				result = true;
			}
		}
		else
		{
			num = Random.Range(1, keepEscapeRound);
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.KeepEscape);
			white = GameColorData.instance.MURMUR_LITTLEBAD;
		}
		Debug.LogFormat("PlayMurmurEffectForKeepEscape keepEscapeRound[{0}] subAtmos[{1}] ", keepEscapeRound, subAtmos);
		num = Mathf.Min(num, 6);
		PlayMurmurEffects(num, list, white);
		return result;
	}

	public void PlayMurmurEffectForStopEscape()
	{
		Vector3 audiencePosition = gamePlayCtrl.itemCtrl.GetAudiencePosition();
		List<string> stringGroup = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.StopKeepEscape);
		Color mURMUR_GOOD = GameColorData.instance.MURMUR_GOOD;
		Debug.Log("PlayMurmurEffectForStopEscape");
		PlayMurmurEffect(audiencePosition, stringGroup.GetRandomOne(), mURMUR_GOOD);
	}

	public bool PlayMurmurEffectForUnderAttack(int makedDamage)
	{
		bool result = false;
		int playCnt = 1;
		List<string> list = null;
		Color white = Color.white;
		if (makedDamage >= 50)
		{
			playCnt = Random.Range(1, 4);
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.UnderAttackLarge);
			white = GameColorData.instance.MURMUR_BAD;
			result = true;
		}
		else
		{
			list = StringDB_Murmur.instance.GetStringGroup(StringDB_Murmur_Keys.UnderAttack);
			white = GameColorData.instance.MURMUR_LITTLEBAD;
		}
		Debug.Log("PlayMurmurEffectForUnderAttack");
		PlayMurmurEffects(playCnt, list, white);
		return result;
	}

	public void ShowAddCardEffect(UICardItem cardItem, float offsetY)
	{
		if (!(cardItem == null))
		{
			addCardEffPool.GetPoolObject<UIAddCardEffectItem>().Play(cardItem.gameObject, offsetY);
		}
	}

	public UICardItem GetTempUICardItem(Card card)
	{
		UICardItem poolObject = addTempUICardItemPool.GetPoolObject<UICardItem>();
		poolObject.SetCard(card);
		poolObject.UpdateCardVisual(card);
		poolObject.ShowCard();
		poolObject.ShowLightBorder();
		poolObject.EnableInteractable();
		poolObject.gameObject.SetActive(value: true);
		poolObject.transform.position = addTempUICardItemStartAnchor.position;
		return poolObject;
	}

	public void RecycleTempUICardItem(GameObject obj)
	{
		addTempUICardItemPool.RecyclePoolObject(obj);
	}

	public void RecycleAllTempUICardItem()
	{
		addTempUICardItemPool.RecycleAllPoolObjects();
	}

	public UIMurmurWaitInputEffectItem PlayWaitInputMurmur(Vector3 srcPos, string murmur, Color color, float floatY, float floatDuration, float fadeInDuration)
	{
		Vector3 pos = gamePlayCtrl.cameraCtrl.ChangeMainCamPosToUICamPos(srcPos);
		UIMurmurWaitInputEffectItem component = waitInputMurmurPool.GetPoolObject().GetComponent<UIMurmurWaitInputEffectItem>();
		component.Play(pos, murmur, color, floatY, floatDuration, fadeInDuration);
		return component;
	}

	public UIMurmurWaitInputEffectItem PlayWaitInputMurmurShake(Vector3 srcPos, string murmur, Color color, float shakeDuration, float scale, float floatY)
	{
		Vector3 pos = gamePlayCtrl.cameraCtrl.ChangeMainCamPosToUICamPos(srcPos);
		UIMurmurWaitInputEffectItem component = waitInputMurmurPool.GetPoolObject().GetComponent<UIMurmurWaitInputEffectItem>();
		component.PlayShake(pos, murmur, color, shakeDuration, scale, floatY);
		return component;
	}

	public void PlayEndWaitInputMurmurEffect(UIMurmurWaitInputEffectItem item)
	{
		item.PlayEndAndFree();
	}

	public void RecycleWaitInputMurmur(GameObject obj)
	{
		waitInputMurmurPool.RecyclePoolObject(obj);
	}

	public void RecycleAllWaitInputMurmur()
	{
		waitInputMurmurPool.RecycleAllPoolObjects();
	}
}
