using UnityEngine;

namespace NodeCanvas.DialogueTrees
{
	///<summary> An interface to use for DialogueActors within a DialogueTree.</summary>
	public partial interface IDialogueActor
    {
        Transform Transform { get; }
    }

    ///<summary>A basic rather limited implementation of IDialogueActor</summary>
    [System.Serializable]
    public class DefaultDialogueActor : IDialogueActor
    {
        public string Name { get; private set; }
        public Transform Transform { get; private set; }

        public DefaultDialogueActor(string name, Transform transform) {
            Name = name;
            Transform = transform;
        }
    }
}