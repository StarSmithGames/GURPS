using Game.Entities;
using Game.Systems.ContextMenu;
using NodeCanvas.DialogueTrees;

using ParadoxNotion.Design;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Add Experience")]
	[Description("Is Use CommandAddExperience")]
	[Category("\x2724 Sheet")]
	public class SheetAddExperienceActionCommand : CommandActionTask
	{
		public int addLevel = 0;
		public int addExperience = 1;

		public bool useRandom = false;
		public float minimumExperienceRandom = 1;
		public float maximumExperienceRandom = 1000;

		public override void Initialize()
		{
			var dt = ownerSystem as DialogueTree;
			if (dt != null)
			{
				var node = dt.CurrentNode;
				var sheet = (node.FinalActor as IActor)?.GetSheet();

				if (sheet != null)
				{
					Command = new CommandAddExperience(sheet, useRandom ? (int)Random.Range(minimumExperienceRandom, maximumExperienceRandom) : addExperience, addLevel);
				}
			}

			Assert.IsNotNull(Command, "Add Experience command == null");
		}

		protected override void OnExecute()
		{
			Command?.Execute();

			EndAction(true);
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			addLevel = EditorGUILayout.IntSlider("Add Level", addLevel, 0, 3);

			if (!useRandom)
			{
				addExperience = EditorGUILayout.IntSlider("Add Experience", addExperience, 1, 1000);
			}
			else
			{
				GUILayout.BeginHorizontal();
				minimumExperienceRandom = EditorGUILayout.IntField("MinMax Experience", (int)minimumExperienceRandom, GUILayout.MaxWidth(200));
				EditorGUILayout.MinMaxSlider(ref minimumExperienceRandom, ref maximumExperienceRandom, 1, 1000);
				maximumExperienceRandom = EditorGUILayout.IntField((int)maximumExperienceRandom, GUILayout.MaxWidth(50));
				GUILayout.EndHorizontal();
			}

			useRandom = EditorGUILayout.Toggle("Use Random?", useRandom);
		}
#endif
	}
}