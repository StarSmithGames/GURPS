using Game.Systems.SheetSystem;
using Game.Systems.VFX;

using NodeCanvas.DialogueTrees;

using Sirenix.OdinInspector;

namespace Game.Systems.DialogueSystem
{
    public interface IActor : IDialogueActor//>:c
    {
        bool IsHasSomethingToSay { get; }
        bool IsHasImportantToSay { get; }
        bool IsInDialogue { get; set; }

        bool TalkWith(IActor actor);

        void Bark();

        ISheet GetSheet();
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