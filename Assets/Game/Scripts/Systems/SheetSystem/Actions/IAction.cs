using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Systems.SheetSystem.Actions
{
	public interface IAction
	{
		public ActionInformation Information { get; }

		void Execute(object target);
	}

	public abstract class Action : MonoBehaviour, IAction
	{
		public ActionInformation Information => information;
		[SerializeField] private ActionInformation information;

		public abstract void Execute(object target);

		public class Factory : PlaceholderFactory<Action> { }
	}

	[System.Serializable]
	public sealed class ActionInformation
	{
		[TitleGroup("Information")]
		[HorizontalGroup("Information/Split", LabelWidth = 100)]
		[VerticalGroup("Information/Split/Left")]
		[PreviewField(ObjectFieldAlignment.Left, Height = 64)]
		[HideLabel]
		public Sprite icon;

		[VerticalGroup("Information/Split/Right")]
		public string nameId;

		[VerticalGroup("Information/Split/Right")]
		public string descriptionId;
	}
}