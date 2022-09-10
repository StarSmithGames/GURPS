using Game.Entities;
using Game.Entities.Models;
using Game.Systems.CommandCenter;
using Game.Systems.ContextMenu;

using NodeCanvas.DialogueTrees;

using ParadoxNotion.Design;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Systems.DialogueSystem.Nodes
{
	[Name("Add Companion")]
	[Description("Is Use CommandAddCompanionInPlayerParty")]
	[Category("\x2723 Party")]
	public class PartyAddCompanionCommand : CommandActionTask
	{
		public bool isSelf = true;
		public PlayableCharacterData data;

		private ICommandExecutor<IPartyManagerCommand> executor;
		private PlayableCharacter companion;
		//var controller = ownerSystemAgent as DialogueTreeController;

		public override void Initialize()
		{
			if (isSelf)
			{
				var tree = ownerSystem as DialogueTree;

				if (tree != null)
				{
					var node = tree.CurrentNode;

					var model = (node.FinalActor as ICharacterModel);
					Assert.IsNotNull(model, "Party Add Companion model == null");

					companion = model.Character as PlayableCharacter;
					Assert.IsNotNull(companion, "Party Add Companion companion == null");

					executor = CommandCenter.CommandCenter.Instance.Registrator.GetAs<ICommandExecutor<IPartyManagerCommand>>();
					Assert.IsNotNull(executor, "Party Add Companion executor == null");
				}
			}
			else
			{

			}
		}

		protected override void OnExecute()
		{
			executor.Execute(new CommandAddCompanionInPlayerParty(companion));

			EndAction(true);
		}

#if UNITY_EDITOR
		protected override void OnTaskInspectorGUI()
		{
			isSelf = EditorGUILayout.Toggle("Is Self", isSelf);

			if (!isSelf)
			{
				data = (PlayableCharacterData)EditorGUILayout.ObjectField("Data", data, typeof(PlayableCharacterData), true);
			}
		}
#endif
	}
}