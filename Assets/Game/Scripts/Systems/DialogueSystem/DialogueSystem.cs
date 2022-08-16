using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Systems.SheetSystem;

using NodeCanvas.DialogueTrees;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	public class DialogueSystem
	{
		public bool IsDialogueProcess => dialogueCoroutine != null;
		private Coroutine dialogueCoroutine = null;
		public DialogueTree CurrentDialogue { get; private set; }

		private Dictionary<string, IDialogueActor> actorsDictionary = new Dictionary<string, IDialogueActor>();

		private SignalBus signalBus;
		private DialogueTreeController dialogueController;
		private AsyncManager asyncManager;
		private CharacterManager characterManager;

		public DialogueSystem(SignalBus signalBus, DialogueTreeController dialogueController, AsyncManager asyncManager, CharacterManager characterManager)
		{
			this.signalBus = signalBus;
			this.dialogueController = dialogueController;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
		}

		public void StartBarkConversation(IActor initiator, IActor actor)
		{
			initiator.Bark();
			actor.Bark();
		}

		/// <summary>
		/// ������ ����������� ���� ����� ����������� � � ��� �������.
		/// </summary>
		/// <param name="initiator">������ �����.</param>
		/// <param name="actor">���� � �������� ���� ���������� ������.</param>
		public void StartDialogue(IActor initiator, IActor actor)
		{
			if (!dialogueController.isRunning && !IsDialogueProcess)
			{
				dialogueController.graph = CurrentDialogue = actor.ActorSettings.dialogues;
				JoinToDialogue(initiator);
				JoinToDialogue(actor);

				Assert.AreEqual(actorsDictionary.Count, CurrentDialogue.actorParameters.Count, "dialogueActors.Count != CurrentDialogue.actorParameters.Count");

				CurrentDialogue.SetActorReferences(actorsDictionary);
				CurrentDialogue.TreeData.isFirstTime = false;

				signalBus?.Fire(new StartDialogueSignal() { dialogue = CurrentDialogue });

				dialogueCoroutine = asyncManager.StartCoroutine(Dialogue());
			}
		}

		public void JoinToDialogue(IActor actor)
		{
			actor.IsInDialogue = true;
			//actorsDictionary.Add(actor.Sheet.Information.nameId, actor);
		}

		private IEnumerator Dialogue()
		{
			dialogueController.StartDialogue();

			yield return new WaitWhile(() =>
			{
				return dialogueController.isRunning;
			});

			foreach (var actor in actorsDictionary.Values)
			{
				(actor as IActor).IsInDialogue = false;
			}
			actorsDictionary.Clear();
			dialogueCoroutine = null;

			signalBus?.Fire(new EndDialogueSignal() { dialogue = CurrentDialogue });
		}
	}
}