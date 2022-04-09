using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.CameraSystem
{
	public class CameraPivot : MonoBehaviour
	{
		[ReadOnly] public Settings settings;
		private void Awake()
		{
			settings.startPosition = transform.localPosition;
			settings.startRotation = transform.localRotation;
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				settings.startPosition = transform.localPosition;
				settings.startRotation = transform.localRotation;
			}

			Gizmos.color = Color.blue;
			Gizmos.DrawCube(transform.position, Vector3.one * 0.1f);
		}


		[System.Serializable]
		public class Settings
		{
			public Vector3 startPosition;
			public Quaternion startRotation;
		}
	}
}