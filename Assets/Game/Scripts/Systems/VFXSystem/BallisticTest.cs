using Game.Systems.VFX;
using UnityEngine;

public class BallisticTest : MonoBehaviour
{
	public LineTargetVFX lineTarget;
	public Vector3 target;
	public float height = 25f;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.TransformPoint(target), 0.1f);

		lineTarget.DrawLine(KinematicBallistic.GetTraectory(transform.position, transform.position + target));
	}
}