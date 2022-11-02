// Survivor.EffectThrowItem
using Survivor;
using UnityEngine;

public class EffectThrowItem : MonoBehaviour
{
	public SpriteRenderer spriteRen;

	public TrailRenderer trailEff_WhiteLine;

	public ParticleSystem trailEff_FireBall;

	public ParticleSystem trailEff_SoulArrow;

	public ParticleSystem trailEff_CurseBomb;

	public ParticleSystem trailEff_AddFlag;

	public ParticleSystem trailEff_WizardMagicBall_Particle;

	public TrailRenderer trailEff_WizardMagicBall_TrailRen;

	private bool isDelayCloserPlaying;

	[SerializeField]
	private float delayCloserTimer;

	[SerializeField]
	private float delayCloseDuration = 0.8f;

	private void FixedUpdate()
	{
		if (isDelayCloserPlaying)
		{
			delayCloserTimer -= Time.fixedDeltaTime;
			if (!(delayCloserTimer > 0f))
			{
				Close();
			}
		}
	}

	public void ResetItem()
	{
		isDelayCloserPlaying = false;
		SetDirect(Vector3.zero);
	}

	private void StartDelayCloser()
	{
		isDelayCloserPlaying = true;
		delayCloserTimer = delayCloseDuration;
	}

	private void Close()
	{
		isDelayCloserPlaying = false;
		trailEff_WhiteLine.enabled = false;
		trailEff_WizardMagicBall_TrailRen.enabled = false;
		base.gameObject.SetActive(value: false);
	}

	public void SetSprite(Sprite sprite)
	{
		spriteRen.sprite = sprite;
	}

	public void SetDirect(Vector3 directTo)
	{
		float num = Vector3.Angle(Vector3.up, directTo);
		if (directTo.x < 0f)
		{
			num = 0f - num;
		}
		ApplyFacing(num);
		Quaternion rotation = Quaternion.AngleAxis(num, Vector3.back);
		base.transform.rotation = rotation;
	}

	private void ApplyFacing(float faceTo)
	{
		base.transform.localScale = new Vector3(Mathf.Sign(faceTo), 1f, 1f);
	}

	public void SetTrailEffect(ActionVisualThrowTrailEffect effect)
	{
		trailEff_WhiteLine.enabled = false;
		trailEff_FireBall.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		trailEff_SoulArrow.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		trailEff_CurseBomb.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		trailEff_AddFlag.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		trailEff_WizardMagicBall_Particle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		trailEff_WizardMagicBall_TrailRen.enabled = false;
		switch (effect)
		{
		case ActionVisualThrowTrailEffect.WhiteLine:
			trailEff_WhiteLine.enabled = true;
			break;
		case ActionVisualThrowTrailEffect.FireBall:
			trailEff_FireBall.Play();
			break;
		case ActionVisualThrowTrailEffect.SoulArrow:
			trailEff_SoulArrow.Play();
			break;
		case ActionVisualThrowTrailEffect.CurseBomb:
			trailEff_CurseBomb.Play();
			break;
		case ActionVisualThrowTrailEffect.AddFlag:
			trailEff_AddFlag.Play();
			break;
		case ActionVisualThrowTrailEffect.WizardMagicBall:
			trailEff_WizardMagicBall_Particle.Play();
			trailEff_WizardMagicBall_TrailRen.enabled = true;
			break;
		case ActionVisualThrowTrailEffect.None:
			break;
		}
	}

	public void Disable()
	{
		spriteRen.sprite = null;
		trailEff_FireBall.Stop();
		trailEff_SoulArrow.Stop();
		trailEff_CurseBomb.Stop();
		trailEff_AddFlag.Stop();
		trailEff_WizardMagicBall_Particle.Stop();
		StartDelayCloser();
	}
}
