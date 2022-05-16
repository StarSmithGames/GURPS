#if UNITY_EDITOR

using ParadoxNotion.Design;
using UnityEngine;
using NodeCanvas.DialogueTrees;

namespace NodeCanvas.Editor
{

    ///<summary>A drawer for dialogue tree statements</summary>
    public class StatementDrawer : ObjectDrawer<Statement>
    {
        public override Statement OnGUI(GUIContent content, Statement instance) {
            if ( instance == null ) { instance = new Statement("..."); }
            instance.Text = UnityEditor.EditorGUILayout.TextArea(instance.Text, Styles.wrapTextArea, GUILayout.Height(100));
            instance.Audio = UnityEditor.EditorGUILayout.ObjectField("Audio File", instance.Audio, typeof(AudioClip), false) as AudioClip;
            instance.Meta = UnityEditor.EditorGUILayout.TextField("Metadata", instance.Meta);
            return instance;
        }
    }
}

#endif