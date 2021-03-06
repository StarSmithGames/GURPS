using Game.Systems.SheetSystem;
using Game.Systems.VFX;

using NodeCanvas.DialogueTrees;

using Sirenix.OdinInspector;

namespace Game.Systems.DialogueSystem
{
    public interface IActor : ISheetable, IDialogueActor//>:c
    {
        bool IsHaveSomethingToSay { get; }
        bool IsInDialogue { get; set; }

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