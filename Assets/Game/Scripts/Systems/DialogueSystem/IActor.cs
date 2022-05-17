using Game.Systems.VFX;

using NodeCanvas.DialogueTrees;

using Sirenix.OdinInspector;

using System.Collections.Generic;

namespace Game.Systems.DialogueSystem
{
    public interface IActor : IDialogueActor//>:c
    {
        bool IsHaveSomethingToSay { get; }

        ActorSettings ActorSettings { get; }

        void Bark();
    }

    [System.Serializable]
	public class ActorSettings
    {
        public bool isImportanatBark = false;
        [ShowIf("isImportanatBark")]
        public MarkIndicator indicator = MarkIndicator.Exclamation;
        public BarkTree barks;
        public DialogueTree dialogues;
    }
}