using NodeCanvas.DialogueTrees;

#if UNITY_EDITOR
using NodeCanvas.Editor;
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.DialogueSystem
{
	[CreateAssetMenu(menuName = "Game/Dialogues/Barks", fileName = "Barks")]
	public class BarkTree : DialogueTree
	{
		public BarkType barkType = BarkType.Random;
	}

	public enum BarkType
	{
		First,
		Random,
		Sequence,
	}


#if UNITY_EDITOR
	[CustomEditor(typeof(BarkTree), true)]
	public class BarkTreeEditor : GraphInspector
	{
		private BarkTree bark
		{
			get { return target as BarkTree; }
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			bark.barkType = (BarkType)EditorGUILayout.EnumPopup("Bark type :", bark.barkType);
		}
	}
#endif
}