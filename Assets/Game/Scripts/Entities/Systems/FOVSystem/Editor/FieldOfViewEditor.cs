using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector.Editor;

namespace Game.Entities
{
	[CustomEditor(typeof(FieldOfView))]
	public class FieldOfViewEditor : OdinEditor
	{
		private void OnSceneGUI()
		{
			FieldOfView fow = (FieldOfView)target;

			var settings = serializedObject.FindProperty("settings").GetValue<FieldOfView.Settings>();

			Transform transform = fow.transform;

			Color color = Color.white;
			color.a = 0.1f;
			Handles.color = color;
			Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -settings.viewAngle / 2, settings.viewRadius);
			Handles.DrawSolidArc(transform.position, transform.up, transform.forward, settings.viewAngle / 2, settings.viewRadius);

			color = Color.yellow;
			color.a = 0.1f;
			Handles.color = color;
			Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -settings.farthestAngle / 2, settings.farthestZone);
			Handles.DrawSolidArc(transform.position, transform.up, transform.forward, settings.farthestAngle / 2, settings.farthestZone);

			color = Color.red;
			color.a = 0.1f;
			Handles.color = color;
			Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -settings.nearestAngle / 2, settings.nearestZone);
			Handles.DrawSolidArc(transform.position, transform.up, transform.forward, settings.nearestAngle / 2, settings.nearestZone);




			//Vector3 viewAngleA = fow.DirFromAngle(-settings.viewAngle / 2, false);
			//Vector3 viewAngleB = fow.DirFromAngle(settings.viewAngle / 2, false);

			//Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, viewRadius);
			//Handles.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
			//Handles.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
		}

		private Vector3 DirFromAngle(float angleInDegrees)
		{
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
	}
}