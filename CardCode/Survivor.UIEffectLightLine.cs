// Survivor.UIEffectLightLine
using System.Collections.Generic;
using DG.Tweening;
using MWUtil;
using UnityEngine;

public class UIEffectLightLine : MonoBehaviour
{
	[SerializeField]
	public ParticleSystem particleSys;

	private List<Vector3> beizerPoints = new List<Vector3>();

	private List<Vector3> pathPoints;

	private GameObject followObj;

	private static int numOfPathPoints = 30;

	private float time;

	private float timeMax;

	private void Update()
	{
		if (pathPoints != null)
		{
			time += Time.deltaTime;
			float lifetimePercentage = Mathf.InverseLerp(0f, timeMax, time);
			int num = (int)DOVirtual.EasedValue(0f, pathPoints.Count, lifetimePercentage, Ease.Linear);
			if (num >= 0 && num < pathPoints.Count)
			{
				Vector3 position = pathPoints[num];
				base.transform.position = position;
			}
		}
	}

	private void FixedUpdate()
	{
		if (pathPoints != null && beizerPoints != null && beizerPoints.Count >= 1 && followObj != null && !(followObj.transform.position == beizerPoints[beizerPoints.Count - 1]))
		{
			beizerPoints[beizerPoints.Count - 1] = followObj.transform.position;
			BezierCurve.UpdateCurve(ref pathPoints, beizerPoints, numOfPathPoints);
		}
	}

	public void PlayDrawCard(Vector3 srcPos, Vector3 dstPos, float duration)
	{
		PlayStraight(srcPos, dstPos, duration, new Color(0.5f, 1f, 0.5f));
	}

	public void PlayStraight(Vector3 srcPos, Vector3 dstPos, float duration, Color color)
	{
		base.gameObject.SetActive(value: true);
		particleSys.Clear(withChildren: true);
		ParticleSystem.MainModule main = particleSys.main;
		main.startColor = color;
		particleSys.Play(withChildren: true);
		base.transform.position = srcPos;
		base.transform.DOMove(dstPos, duration).SetDelay(0.05f);
	}

	public void PlayBeizer(List<Vector3> beizer, GameObject dstObj, float duration, Color color)
	{
		if (beizer != null && !(dstObj == null))
		{
			followObj = dstObj;
			PlayBeizer(beizer, duration, color);
		}
	}

	public void PlayBeizer(List<Vector3> beizer, float duration, Color color)
	{
		if (beizer != null)
		{
			base.gameObject.SetActive(value: true);
			particleSys.Clear(withChildren: true);
			ParticleSystem.MainModule main = particleSys.main;
			main.startColor = color;
			particleSys.Play(withChildren: true);
			beizerPoints = beizer;
			pathPoints = BezierCurve.Create(beizerPoints, numOfPathPoints);
			base.transform.position = beizer[0];
			timeMax = duration;
			time = 0f;
		}
	}

	public void Stop()
	{
		particleSys.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		pathPoints = null;
		followObj = null;
	}
}
