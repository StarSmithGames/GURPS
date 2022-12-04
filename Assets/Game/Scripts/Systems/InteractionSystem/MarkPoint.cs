using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.InteractionSystem
{
	public class MarkPoint : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 1f));
		}
	}
}