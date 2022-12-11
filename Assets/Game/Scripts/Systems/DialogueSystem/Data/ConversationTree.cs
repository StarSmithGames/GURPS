using NodeCanvas.DialogueTrees;

#if UNITY_EDITOR
using NodeCanvas.Editor;
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.DialogueSystem
{
	[CreateAssetMenu(fileName = "Conversation", menuName = "Game/Dialogues/Conversation")]
	public class ConversationTree : DialogueTree { }

#if UNITY_EDITOR
    [CustomEditor(typeof(ConversationTree), true)]
    public class ConversationTreeEditor : DialogueTreeInspector { }
#endif
}