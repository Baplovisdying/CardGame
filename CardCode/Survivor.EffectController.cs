// Survivor.EffectController
using System;
using System.Collections.Generic;
using System.Linq;
using MWUtil;
using Survivor;
using UnityEngine;

public class EffectController : MonoBehaviour
{
	[SerializeField]
	public ThrowCoinEffectController throwCoinEffCtrl;

	[SerializeField]
	private ObjectPool dmgEffectPool;

	[SerializeField]
	private ObjectPool throwObjEffectPool;

	[SerializeField]
	private ObjectPool shadowEffectPool;

	[SerializeField]
	private GameObject roarEffectPrefab;

	[SerializeField]
	public List<ObjectPoolPair> effectPoolPairs;

	[SerializeField]
	private List<ObjectPoolPair> runtimePoolPairs = new List<ObjectPoolPair>();

	private UIRootController uiRootCtrl;

	private RoarEffectController roarEffectCtrl;

	[SerializeField]
	private GameObject newEffectPrefab;

	private void OnValidate()
	{
		ValidateDatas();
	}

	public void ValidateDatas()
	{
		foreach (ObjectPoolPair effectPoolPair in effectPoolPairs)
		{
			if (!effectPoolPair.key.TryEnum<ActionEffect>())
			{
				Debug.LogError("[EffectController] key is not ActionEffect");
			}
			effectPoolPair.objPool.prefab.GetComponent<EffectItem>();
		}
		Debug.Log("[EffectController] ValidateDatas DONE");
	}

	public void Init(UIRootController uiRootCtrl)
	{
		this.uiRootCtrl = uiRootCtrl;
	}

	public void ShowUnderAttackEffect()
	{
		uiRootCtrl.uiUnderAttack.SetActive(value: true);
	}

	public void PlayDamageNumber(int dmgValue, Vector3 worldPos)
	{
		PlayDamageNumber(dmgValue, worldPos, GameColorData.instance.DMG_NUMBER_UNDERATTACk);
	}

	public void PlayDamageNumber(int dmgValue, Vector3 worldPos, Color color)
	{
		dmgEffectPool.GetPoolObject<DamageNumber>().Play(dmgValue, worldPos, Mathf.Sign(UnityEngine.Random.value - 0.5f), color);
	}

	public void PlayLostBlockNumber(int value, Vector3 worldPos)
	{
		dmgEffectPool.GetPoolObject<DamageNumber>().Play(numColor: GameColorData.instance.DMG_NUMBER_BLOCKLOST, dmgNumber: value, spawnPos: worldPos, direction: Mathf.Sign(UnityEngine.Random.value - 0.5f));
	}

	public EffectThrowItem GetThrowObj()
	{
		EffectThrowItem poolObject = throwObjEffectPool.GetPoolObject<EffectThrowItem>();
		poolObject.ResetItem();
		return poolObject;
	}

	public void RecycleAllThrowObjects()
	{
		throwObjEffectPool.RecycleAllPoolObjects();
	}

	public EffectShadowItem GetShadowEffect()
	{
		return shadowEffectPool.GetPoolObject<EffectShadowItem>();
	}

	public void RecycleShadowEffect(EffectShadowItem item)
	{
		shadowEffectPool.RecyclePoolObject(item.gameObject);
	}

	public void RecycleAllShadowEffect()
	{
		shadowEffectPool.RecycleAllPoolObjects();
	}

	private void PlayEffectItem(string key, Vector3 worldPos, Vector3 direct)
	{
		EffectItem effectItem = GetEffectItem(key);
		if (!(effectItem == null))
		{
			effectItem.Play(worldPos, direct);
		}
	}

	private EffectItem GetEffectItem(string key)
	{
		foreach (ObjectPoolPair effectPoolPair in effectPoolPairs)
		{
			if (effectPoolPair.key == key)
			{
				return effectPoolPair.objPool.GetPoolObject<EffectItem>();
			}
		}
		return null;
	}

	public void PlayActionEffect(ActionEffect effectType, Vector3 worldPos)
	{
		PlayEffectItem(effectType.ToString(), worldPos, Vector3.zero);
	}

	public void PlayActionEffect(ActionEffect effectType, Vector3 worldPos, Vector3 direct)
	{
		PlayEffectItem(effectType.ToString(), worldPos, direct);
	}

	public void PlayActionEffectIOP(GameObject[] effIOPs, Vector3 worldPos, Vector3 direct)
	{
		if (effIOPs == null || effIOPs.Length == 0)
		{
			return;
		}
		foreach (GameObject gameObject in effIOPs)
		{
			if (!(gameObject == null))
			{
				string key = gameObject.name;
				ObjectPool objectPool = GeRuntimeObjectPool(key);
				if (objectPool == null)
				{
					objectPool = CreateRuntimeObjectPool(key, gameObject);
				}
				EffectItem poolObject = objectPool.GetPoolObject<EffectItem>();
				if (!(poolObject == null))
				{
					poolObject.Play(worldPos, direct);
				}
			}
		}
	}

	private ObjectPool GeRuntimeObjectPool(string key)
	{
		foreach (ObjectPoolPair runtimePoolPair in runtimePoolPairs)
		{
			if (runtimePoolPair.key == key)
			{
				return runtimePoolPair.objPool;
			}
		}
		return null;
	}

	private ObjectPool CreateRuntimeObjectPool(string key, GameObject prefab)
	{
		GameObject obj = new GameObject(key + "Pool");
		obj.transform.parent = base.transform;
		ObjectPool objectPool = obj.AddComponent<ObjectPool>();
		objectPool.prefab = prefab;
		objectPool.preAmount = 0;
		objectPool.spawn = true;
		objectPool.inPool = true;
		objectPool.worldPosStays = false;
		ObjectPoolPair objectPoolPair = new ObjectPoolPair();
		objectPoolPair.key = key;
		objectPoolPair.objPool = objectPool;
		runtimePoolPairs.Add(objectPoolPair);
		return objectPool;
	}

	public void RecycleAllRuntimePools()
	{
		foreach (ObjectPoolPair runtimePoolPair in runtimePoolPairs)
		{
			if (!(runtimePoolPair.objPool == null))
			{
				UnityEngine.Object.Destroy(runtimePoolPair.objPool.gameObject);
			}
		}
		runtimePoolPairs.Clear();
	}

	public RoarEffectController CreateRoarEffect()
	{
		if (roarEffectCtrl != null)
		{
			return roarEffectCtrl;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(roarEffectPrefab, base.transform);
		roarEffectCtrl = gameObject.GetComponent<RoarEffectController>();
		return roarEffectCtrl;
	}

	public void FreeRoarEffect()
	{
		if (!(roarEffectCtrl == null))
		{
			UnityEngine.Object.Destroy(roarEffectCtrl.gameObject);
			roarEffectCtrl = null;
		}
	}

	public void PlayRoarEffect()
	{
		if (!(roarEffectCtrl == null))
		{
			roarEffectCtrl.Play();
		}
	}

	public void StopRoarEffect()
	{
		if (!(roarEffectCtrl == null))
		{
			roarEffectCtrl.Stop();
		}
	}

	public void AddNewActionEffectPool()
	{
		string pairKey = Enum.GetValues(typeof(ActionEffect)).Cast<ActionEffect>().Last()
			.ToString();
		AddNewEffectPool(pairKey);
	}

	private void AddNewEffectPool(string pairKey)
	{
		if (newEffectPrefab == null)
		{
			Debug.LogWarning("[EffectController] please assign NewEffectPrefab");
			return;
		}
		if (newEffectPrefab.GetComponent<EffectItem>() == null)
		{
			Debug.LogWarning("[EffectController] NewEffectPrefab haven't component [EffectItem], please make sure it is a effect object");
			return;
		}
		if (CheckKeyExist(pairKey))
		{
			Debug.LogWarning("[EffectController] effect pool key already exist :" + pairKey);
			return;
		}
		ObjectPoolPair objectPoolPair = new ObjectPoolPair();
		objectPoolPair.key = pairKey;
		GameObject gameObject = new GameObject();
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.name = newEffectPrefab.name.Replace("Effect", "") + "EffectPool";
		ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
		objectPool.prefab = newEffectPrefab;
		objectPoolPair.objPool = objectPool;
		effectPoolPairs.Add(objectPoolPair);
		Debug.Log("[EffectController] effect object pool object added", gameObject);
	}

	private bool CheckKeyExist(string key)
	{
		foreach (ObjectPoolPair effectPoolPair in effectPoolPairs)
		{
			if (effectPoolPair.key == key)
			{
				return true;
			}
		}
		return false;
	}
}
