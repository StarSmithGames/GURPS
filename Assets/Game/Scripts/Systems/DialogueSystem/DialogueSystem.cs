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

		public void StartBark()
		{

		}

		public void StartConversation(IActor initiator)
		{
			initiator.Bark();
		}

		/// <summary>
		/// Запуск диалогового окна между инициатором и с кем говорим.
		/// </summary>
		/// <param name="initiator">Обычно игрок.</param>
		/// <param name="actor">Актёр у которого есть диалоговое дерево.</param>
		public void StartDialogue(IActor initiator, IActor actor)
		{
			if (!dialogueController.isRunning)
			{
				dialogueController.StartDialogue();
			}
		}
	}
}