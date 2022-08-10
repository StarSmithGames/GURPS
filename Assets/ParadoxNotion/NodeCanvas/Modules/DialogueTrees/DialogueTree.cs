using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;

namespace NodeCanvas.DialogueTrees
{
    ///<summary> Use DialogueTrees to create Dialogues between Actors</summary>
    [GraphInfo(
        packageName = "NodeCanvas",
        docsURL = "https://nodecanvas.paradoxnotion.com/documentation/",
        resourcesURL = "https://nodecanvas.paradoxnotion.com/downloads/",
        forumsURL = "https://nodecanvas.paradoxnotion.com/forums-page/"
        )]
    [CreateAssetMenu(menuName = "Game/Dialogues/Dialogue")]
    public partial class DialogueTree : Graph
    {
        ///----------------------------------------------------------------------------------------------
        [System.Serializable]
        class DerivedSerializationData
        {
            public List<ActorParameter> actorParameters;
        }

        public override object OnDerivedDataSerialization() {
            var data = new DerivedSerializationData();
            data.actorParameters = this.actorParameters;
            return data;
        }

        public override void OnDerivedDataDeserialization(object data) {
            if ( data is DerivedSerializationData ) {
                this.actorParameters = ( (DerivedSerializationData)data ).actorParameters;
            }
        }
        ///----------------------------------------------------------------------------------------------

        ///<summary>The string used for the starting actor"</summary>
        public const string INSTIGATOR_NAME = "SELF";


        ///<summary>The current DialoguTree running</summary>
        public static DialogueTree currentDialogue { get; private set; }
        ///<summary>The previous DialoguTree running</summary>
        public static DialogueTree previousDialogue { get; private set; }

        ///<summary>The current node of this DialogueTree</summary>
        public DTNode CurrentNode { get; private set; }

        public int LastNodeConnectionIndex { get; private set; }
        public DTNode LastNode { get; private set; }

        ///----------------------------------------------------------------------------------------------
        public override System.Type baseNodeType => typeof(DTNode);
        public override bool requiresAgent => false;
        public override bool requiresPrimeNode => true;
        public override bool isTree => true;
        public override bool allowBlackboardOverrides => true;
        sealed public override bool canAcceptVariableDrops => false;
        ///----------------------------------------------------------------------------------------------

        ///<summary>Continues the DialogueTree at provided child connection index of currentNode</summary>
        public void Continue(int index = 0) {
            if ( index < 0 || index > CurrentNode.outConnections.Count - 1 ) {
                Stop(true);
                return;
            }
            CurrentNode.outConnections[index].status = Status.Success; //editor vis
            LastNodeConnectionIndex = index;
            EnterNode((DTNode)CurrentNode.outConnections[index].targetNode);
        }

        ///<summary>Enters the provided node</summary>
        public void EnterNode(DTNode node)
        {
            LastNode = CurrentNode;
            CurrentNode = node;
            CurrentNode.Reset(false);
            if ( CurrentNode.Execute(agent, blackboard) == Status.Error ) {
                Stop(false);
            }
        }

        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Dialogue Tree Object", false, 2)]
        static void Editor_CreateGraph() {
            var dt = new GameObject("DialogueTree").AddComponent<DialogueTreeController>();
            UnityEditor.Selection.activeObject = dt;
        }
#endif
    }

	//IDialogueActor
	public partial class DialogueTree
	{
        ///<summary>The dialogue actor parameters. We let Unity serialize this as well</summary>
        [SerializeField] public List<ActorParameter> actorParameters = new List<ActorParameter>();

        ///<summary>A list of the defined names for the involved actor parameters</summary>
        public List<string> definedActorParameterNames
        {
            get
            {
                var list = actorParameters.Select(r => r.Name).ToList();
                list.Insert(0, INSTIGATOR_NAME);
                return list;
            }
        }

        ///<summary>Returns the ActorParameter by id</summary>
        public ActorParameter GetParameterByID(string id)
        {
            return actorParameters.Find(p => p.ID == id);
        }

        ///<summary>Returns the ActorParameter by name</summary>
        public ActorParameter GetParameterByName(string paramName)
        {
            return actorParameters.Find(p => p.Name == paramName);
        }

        ///<summary>Returns the actor by parameter id.</summary>
        public IDialogueActor GetActorReferenceByID(string id)
        {
            var param = GetParameterByID(id);
            return param != null ? GetActorReferenceByName(param.Name) : null;
        }

        ///<summary>Resolves and gets an actor based on the key name</summary>
        public IDialogueActor GetActorReferenceByName(string paramName)
        {
            //Check for INSTIGATOR selection
            if (paramName == INSTIGATOR_NAME)
            {
                //return it directly if it implements IDialogueActor
                if (agent is IDialogueActor)
                {
                    return (IDialogueActor)agent;
                }

                //Otherwise use the default actor and set name and transform from agent
                if (agent != null)
                {
                    return new DefaultDialogueActor(agent.gameObject.name, agent.transform);
                }

                return new DefaultDialogueActor("NO ACTOR", null);
            }

            //Check for non INSTIGATOR selection. If there IS an actor reference return it
            var refData = actorParameters.Find(r => r.Name == paramName);
            if (refData != null && refData.Actor != null)
            {
                return refData.Actor;
            }

            //Otherwise use the default actor and set the name to the key and null transform
            Logger.Log(string.Format("An actor entry '{0}' on DialogueTree has no reference. A dummy Actor will be used with the entry Key for name", paramName), "Dialogue Tree", this);
            return new DefaultDialogueActor(paramName, null);
        }

        ///<summary>Set the target IDialogueActor for the provided key parameter name</summary>
        public void SetActorReference(string paramName, IDialogueActor actor)
        {
            var param = actorParameters.Find(p => p.Name == paramName);
            if (param == null)
            {
                Logger.LogError(string.Format("There is no defined Actor key name '{0}'", paramName), "Dialogue Tree", this);
                return;
            }
            param.Actor = actor;
        }

        ///<summary>Set all target IDialogueActors at once by provided dictionary</summary>
        public void SetActorReferences(Dictionary<string, IDialogueActor> actors)
        {
            foreach (var pair in actors)
            {
                var param = actorParameters.Find(p => p.Name == pair.Key);
                if (param == null)
                {
                    Logger.LogWarning(string.Format("There is no defined Actor key name '{0}'. Seting actor skiped", pair.Key), "Dialogue Tree", this);
                    continue;
                }
                param.Actor = pair.Value;
            }
        }

        ///<summary>An Actor Parameter</summary>
        [System.Serializable]
        public class ActorParameter
        {
            public string ID => string.IsNullOrEmpty(id) ? id = System.Guid.NewGuid().ToString() : id;
            public string Name { get => name; set => name = value; }

            public IDialogueActor Actor
            {
                get
                {
                    if (actor == null)
                    {
                        actor = actorObject as IDialogueActor;
                    }
                    return actor;
                }
                set
                {
                    actor = value;
                    actorObject = value as UnityEngine.Object;
                }
            }

            [SerializeField] private string name;
            [SerializeField] private string id;
            [SerializeField] private UnityEngine.Object actorObject;
            [System.NonSerialized] private IDialogueActor actor;

            public ActorParameter(string name) { this.Name = name; }
        }
    }

    //Actions
    public partial class DialogueTree
    {
        public static event Action<DialogueTree> OnDialogueStarted;
        public static event Action<DialogueTree> OnDialoguePaused;
        public static event Action<DialogueTree> OnDialogueFinished;
        public static event Action<SubtitlesRequestInfo> OnSubtitlesRequest;
        public static event Action<MultipleChoiceRequestInfo> OnMultipleChoiceRequest;

        protected override void OnGraphStarted()
        {
            previousDialogue = currentDialogue;
            currentDialogue = this;

            Logger.Log(string.Format("Dialogue Started '{0}'", this.name), "Dialogue Tree", this);
            if (OnDialogueStarted != null)
            {
                OnDialogueStarted(this);
            }

            if (!(agent is IDialogueActor))
            {
                Logger.Log("Agent used in DialogueTree does not implement IDialogueActor. A dummy actor will be used.", "Dialogue Tree", this);
            }

            CurrentNode = CurrentNode != null ? CurrentNode : (DTNode)primeNode;
            EnterNode(CurrentNode);
        }

        protected override void OnGraphUpdate()
        {
            if (CurrentNode is IUpdatable updatable)
            {
                updatable.Update();
            }
        }

        protected override void OnGraphStoped()
        {
            currentDialogue = previousDialogue;
            previousDialogue = null;
            CurrentNode = null;

            Logger.Log(string.Format("Dialogue Finished '{0}'", this.name), "Dialogue Tree", this);
            if (OnDialogueFinished != null)
            {
                OnDialogueFinished(this);
            }
        }

        protected override void OnGraphPaused()
        {
            Logger.Log(string.Format("Dialogue Paused '{0}'", this.name), "Dialogue Tree", this);
            if (OnDialoguePaused != null)
            {
                OnDialoguePaused(this);
            }
        }

        protected override void OnGraphUnpaused()
        {
            CurrentNode = CurrentNode != null ? CurrentNode : (DTNode)primeNode;
            EnterNode(CurrentNode);

            Logger.Log(string.Format("Dialogue Resumed '{0}'", this.name), "Dialogue Tree", this);
            if (OnDialogueStarted != null)
            {
                OnDialogueStarted(this);
            }
        }

        ///<summary>Raise the OnSubtitlesRequest event</summary>
        public static void RequestSubtitles(SubtitlesRequestInfo info)
        {
            if (OnSubtitlesRequest != null)
                OnSubtitlesRequest(info);
            else Logger.LogWarning("Subtitle Request event has no subscribers. Make sure to add the default '@DialogueGUI' prefab or create your own GUI.", "Dialogue Tree");
        }

        ///<summary>Raise the OnMultipleChoiceRequest event</summary>
        public static void RequestMultipleChoices(MultipleChoiceRequestInfo info)
        {
            if (OnMultipleChoiceRequest != null)
                OnMultipleChoiceRequest(info);
            else Logger.LogWarning("Multiple Choice Request event has no subscribers. Make sure to add the default '@DialogueGUI' prefab or create your own GUI.", "Dialogue Tree");
        }
    }

    //Data
    public partial class DialogueTree
    {
        public Data TreeData
        {
            get
            {
                if (data == null)
                {
                    data = new Data();
                }

                return data;
            }
        }
        private Data data;

        public class Data
        {
            public bool isFirstTime = true;
        }
    }




    public class SubtitlesRequestInfo
    {
        ///<summary>The actor speaking</summary>
        public IDialogueActor actor;
        ///<summary>The statement said</summary>
        public IStatement statement;

        public bool waitForInput = true;

        ///<summary>Call this to Continue the DialogueTree</summary>
        public Action Continue;

        public SubtitlesRequestInfo(IDialogueActor actor, IStatement statement, Action callback)
        {
            this.actor = actor;
            this.statement = statement;
            this.Continue = callback;
        }
    }

    public class MultipleChoiceRequestInfo
    {
        ///<summary>Call this with to select the option to continue with in the DialogueTree</summary>
        public Action<int> SelectOption;

        ///<summary>The actor related. This is usually the actor that will also say the options</summary>
        public IDialogueActor actor;

        public bool saySelection;
        ///<summary>The available choice option. Key: The statement, Value: the child index of the option</summary>
        public List<IChoice> choices;
        ///<summary>The available time for a choice</summary>
        public float availableTime;
    }
}