using UnityEngine;


namespace NodeCanvas.DialogueTrees
{
    ///<summary> An interface to use for DialogueActors within a DialogueTree.</summary>
	public interface IDialogueActor
    {
        string name { get; }
        Transform transform { get; }
    }

    ///<summary>A basic rather limited implementation of IDialogueActor</summary>
    [System.Serializable]
    public class DefaultDialogueActor : IDialogueActor
    {
        private string _name;
        private Transform _transform;

        public string name {
            get { return _name; }
        }

        public Transform transform {
            get { return _transform; }
        }

        public DefaultDialogueActor(string name, Transform transform) {
            this._name = name;
            this._transform = transform;
        }
    }
}