using ParadoxNotion;
using NodeCanvas.Framework;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace NodeCanvas.DialogueTrees
{
    ///<summary>An interface to use for whats being said by a dialogue actor</summary>
    public interface IStatement
    {
        string Text { get; }
        AudioClip Audio { get; }
        string Meta { get; }
    }

    ///<summary>Holds data of what's being said usualy by an actor</summary>
    [System.Serializable]
    public class Statement : IStatement
    {
        public string Text { get => text; set => text = value; }
        public AudioClip Audio { get => audio; set => audio = value; }
        public string Meta { get => meta; set => meta = value; }

        [SerializeField] private string text;
        [SerializeField] private AudioClip audio;
        [SerializeField] private string meta = string.Empty;

        public bool isShowFoldout = false;

        //required
        public Statement() { }
        public Statement(string text) {
            this.Text = text;
        }

        ///<summary>Replace the text of the statement found in brackets, with blackboard variables ToString and returns a Statement copy</summary>
        public IStatement BlackboardReplace(IBlackboard bb) {
            var copy = ParadoxNotion.Serialization.JSONSerializer.Clone<Statement>(this);

            copy.Text = copy.Text.ReplaceWithin('[', ']', (input) =>
            {
                object o = null;
                if ( bb != null ) { //referenced blackboard replace
                    var v = bb.GetVariable(input, typeof(object));
                    if ( v != null ) { o = v.value; }
                }

                if ( input.Contains("/") ) { //global blackboard replace
                    var globalBB = GlobalBlackboard.Find(input.Split('/').First());
                    if ( globalBB != null ) {
                        var v = globalBB.GetVariable(input.Split('/').Last(), typeof(object));
                        if ( v != null ) { o = v.value; }
                    }
                }
                return o != null ? o.ToString() : input;
            });

            return copy;
        }


#if UNITY_EDITOR
        public void OnGUI(string language)
		{
            string foldoutLabel = (string.IsNullOrEmpty(Text) ? "Empty" : Text.CapLength(50)).Replace("\n", "/n ");
            isShowFoldout = EditorGUILayout.Foldout(isShowFoldout, $"{language}: {foldoutLabel}", true);

            if (isShowFoldout)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label(language);
                Text = EditorGUILayout.TextField(Text);
                Audio = EditorGUILayout.ObjectField("Audio File", Audio, typeof(AudioClip), false) as AudioClip;
                Meta = EditorGUILayout.TextField("Meta Data", Meta);
                GUILayout.EndVertical();
            }
        }
#endif
        public override string ToString() {
            return Text;
        }
    }
}