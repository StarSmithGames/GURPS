using Game.Systems.VFX;

using NodeCanvas.DialogueTrees;

using Sirenix.OdinInspector;

namespace Game.Systems.DialogueSystem
{
    public interface IActor : IDialogueActor//>:c
    {
        bool IsHaveSomethingToSay { get; }
        bool IsInDialogue { get; set; }

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