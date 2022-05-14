using NodeCanvas.DialogueTrees;

using UnityEngine;

namespace Game.Systems.DialogueSystem
{
    public interface IActor
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