using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	[SerializeField] private Settings settings;

	[SerializeField] private float delay = 0.2f;

	private Coroutine viewCoroutine = null;
	public bool IsViewProccess => viewCoroutine != null;
	private WaitForSeconds seconds;

	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

	public void StartView()
	{
		if (!IsViewProccess)
		{
			seconds = new WaitForSeconds(delay);
			viewCoroutine = StartCoroutine(FindTargetsWithDelay());
		}
	}
	private IEnumerator FindTargetsWithDelay()
	{
		while (true)
		{
			Debug.LogError("Try Find");
			yield return seconds;
			FindVisibleTargets();
		}

		StopView();
	}
	public void StopView()
	{
		visibleTargets.Clear();

		if (IsViewProccess)
		{
			StopCoroutine(viewCoroutine);
			viewCoroutine = null;
		}
	}

	private void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, settings.viewRadius, settings.targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < settings.viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, settings.obstacleMask))
				{
					visibleTargets.Add(target);
				}
			}
		}
	}


	private void OnDrawGizmos()
	{
		for (int i = 0; i < visibleTargets.Count; i++)
		{
			float distance = Vector3.Distance(transform.position, visibleTargets[i].position);
			if (distance <= settings.nearestZone)
			{
				Gizmos.color = Color.red;
			}
			else if (distance <= settings.farthestZone)
			{
				Gizmos.color = Color.yellow;
			}
			else if(distance <= settings.viewRadius)
			{
				Gizmos.color = Color.white;
			}

			Gizmos.DrawLine(transform.position, visibleTargets[i].position);
		}
	}

	[System.Serializable]
	public class Settings
	{
		public float viewRadius = 10f;
		public float farthestZone = 8f;
		public float nearestZone = 3f;
		[Space]
		[Range(0, 360f)]
		public float viewAngle = 90f;
		[Range(0, 360f)]
		public float farthestAngle = 90f;
		[Range(0, 360f)]
		public float nearestAngle = 90f;
		[Space]
		public LayerMask targetMask;
		public LayerMask obstacleMask;
	}
}