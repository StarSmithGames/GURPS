using Sirenix.OdinInspector;

using UnityEngine;

namespace Game.Systems.SheetSystem.Actions
{
	public interface IAction
	{
		void Execute(object target);
	}

	public abstract class BaseAction : ScriptableObject, IAction
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

		public abstract void Execute(object target);
	}
}