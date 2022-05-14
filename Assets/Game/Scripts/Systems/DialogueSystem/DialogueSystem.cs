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
		/// ������ ����������� ���� ����� ����������� � � ��� �������.
		/// </summary>
		/// <param name="initiator">������ �����.</param>
		/// <param name="actor">���� � �������� ���� ���������� ������.</param>
		public void StartDialogue(IActor initiator, IActor actor)
		{
			if (!dialogueController.isRunning)
			{
				dialogueController.StartDialogue();
			}
		}
	}
}