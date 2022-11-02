// Survivor.EffectItem
using System;
using Survivor;
using UnityEngine;

public class EffectItem : MonoBehaviour
{
	[Serializable]
	internal enum EffectRotationType
	{
		AttackDirection,
		Random,
		NoRotation
	}

	[SerializeField]
	private ParticleSystem rootParticle;

	[SerializeField]
	private Animator mainAnimator;

	[SerializeField]
	private bool isTopDownEffect;

	[SerializeField]
	private float autoDisableDuration = 1f;

	[SerializeField]
	private EffectRotationType rotationType;

	[SerializeField]
	private float effectOffset;

	[SerializeField]
	private Vector2 effectGlobalOffset = Vector2.zero;

	public static readonly float HYPOTENUSE_RATIO = 0.707106769f;

	public static readonly float TOPDOWNEFFECT_SCALE_Y = 0.7f;

	public static readonly string STRING_topDownEffectRotParent = "topDownEffectRotParent";

	private Transform itemTransform;

	private Transform topDownEffectRotParent;

	private float autoDisableTimer;

	private void Awake()
	{
		if (rootParticle != null)
		{
			itemTransform = rootParticle.transform;
		}
		if (mainAnimator != null)
		{
			itemTransform = mainAnimator.transform;
		}
		if (isTopDownEffect)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = STRING_topDownEffectRotParent;
			topDownEffectRotParent = gameObject.transform;
			topDownEffectRotParent.SetParent(base.transform);
			topDownEffectRotParent.localPosition = Vector3.zero;
			if (itemTransform != null)
			{
				itemTransform.SetParent(topDownEffectRotParent);
			}
			base.transform.localScale = new Vector3(base.transform.localScale.x, TOPDOWNEFFECT_SCALE_Y, 1f);
		}
	}

	private void OnEnable()
	{
		if (rootParticle != null)
		{
			rootParticle.Play(withChildren: true);
		}
		autoDisableTimer = autoDisableDuration;
	}

	private void OnDisable()
	{
		if (rootParticle != null)
		{
			rootParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	private void FixedUpdate()
	{
		autoDisableTimer -= Time.fixedDeltaTime;
		if (autoDisableTimer <= 0f)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void Play(Vector3 worldPos, Vector3 directTo)
	{
		switch (rotationType)
		{
		default:
		{
			float num = Vector3.Angle(Vector3.up, directTo);
			if (directTo.x < 0f)
			{
				num = 0f - num;
			}
			ApplyFacing(num);
			Quaternion rotation = Quaternion.AngleAxis(num, Vector3.back);
			if (isTopDownEffect)
			{
				topDownEffectRotParent.rotation = rotation;
			}
			else
			{
				base.transform.rotation = rotation;
			}
			break;
		}
		case EffectRotationType.Random:
			RandomRotating();
			break;
		case EffectRotationType.NoRotation:
			break;
		}
		float multiple = 1f;
		if (directTo.y > 0.001f || directTo.y < -0.001f)
		{
			multiple = HYPOTENUSE_RATIO;
		}
		ApplyEffectOffset(multiple);
		base.transform.position = worldPos + new Vector3(effectGlobalOffset.x, effectGlobalOffset.y);
		base.gameObject.SetActive(value: true);
	}

	private void RandomRotating()
	{
		base.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
	}

	private void ApplyEffectOffset(float multiple = 1f)
	{
		if (!(itemTransform == null))
		{
			itemTransform.localPosition = new Vector3(0f, effectOffset * multiple, 0f);
		}
	}

	private void ApplyFacing(float faceTo)
	{
		Vector3 localScale = new Vector3(Mathf.Sign(faceTo), 1f, 1f);
		if (isTopDownEffect)
		{
			topDownEffectRotParent.localScale = localScale;
		}
		else
		{
			base.transform.localScale = localScale;
		}
	}
}
