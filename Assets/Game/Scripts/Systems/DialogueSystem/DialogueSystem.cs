using Game.Managers.CharacterManager;

using NodeCanvas.DialogueTrees;

using UnityEngine;

namespace Game.Systems.DialogueSystem
{
	public class DialogueSystem
	{
		private DialogueTreeController dialogueController;
		private CharacterManager characterManager;

		public DialogueSystem(DialogueTreeController dialogueController, CharacterManager characterManager)
		{
			this.dialogueController = dialogueController;
			this.characterManager = characterManager;
		}

		public void StartConversation(IActor initiator)
		{

		}

		public void StartDialogue(IActor initiator)
		{
			if (!dialogueController.isRunning)
			{
				dialogueController.StartDialogue();
			}
		}
	}
}