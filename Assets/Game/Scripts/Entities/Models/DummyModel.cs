using Game.Entities.Models;
using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities.Models
{
	public class DummyModel : Model, ISheetable, IActor
	{
		public ActorSettings Actor => Sheet.Settings.actor;

		public bool IsHaveSomethingToSay => (Actor.barks != null && IsHasFreshAndImportantBarks()) || (Actor.dialogues != null && IsHasFreshAndImportantDialogues());
		public bool IsInDialogue { get; set; }

		public Transform DialogueTransform => transform;

		public ModelData data;

		public ISheet Sheet
		{
			get
			{
				if (sheet == null)
				{
					sheet = new ModelSheet(data);
				}

				return sheet;
			}
		}
		private ISheet sheet;

		private DialogueSystem dialogueSystem;
		
		[Inject]
		private void Construct(DialogueSystem dialogueSystem)
		{
			this.dialogueSystem = dialogueSystem;
		}

		public bool TalkWith(IActor actor)
		{
			if (IsHaveSomethingToSay)
			{
				dialogueSystem.StartDialogue(this, actor);
				return true;
			}

			return false;
		}

		public void Bark()
		{

		}

		private bool IsHasFreshAndImportantBarks()
		{
			return Actor.barks.TreeData.isFirstTime && Actor.isImportanatBark;
		}
		private bool IsHasFreshAndImportantDialogues()
		{
			return Actor.dialogues.TreeData.isFirstTime;
		}
	}
}