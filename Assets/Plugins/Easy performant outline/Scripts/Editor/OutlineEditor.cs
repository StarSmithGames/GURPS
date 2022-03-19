using EPOOutline;

using Sirenix.OdinInspector.Editor;


using UnityEditor;

[CustomEditor(typeof(Outlinable))]
public class OutlineEditor : OdinEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var ourliner = (Outlinable)target;
		var settings = serializedObject.FindProperty("settings").GetValue<Outlinable.Settings>();

		if (serializedObject.FindProperty("isCustom").boolValue)
		{
			DrawPropertiesExcluding(serializedObject,
				"data",
				"m_Script");
		}
		else
		{
			DrawPropertiesExcluding(serializedObject,
				"settings",
				"m_Script");
		}
	}
}