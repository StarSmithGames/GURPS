using Game.Entities;
using Game.Managers.CharacterManager;

using NodeCanvas.DialogueTrees;

using System.Collections;
using System.Collections.Generic;
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

		public void StartDialogue(IEntity initiator)
		{
			Debug.LogError("HERE");
			dialogueController.StartDialogue();
		}
	}
}