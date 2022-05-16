using NodeCanvas.DialogueTrees;

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
        public BarkTree barks;
        public DialogueTree dialogues;
    }
}