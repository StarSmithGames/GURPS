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
		[HideLabel]
		[SerializeField] private ActionInformation information;

		public abstract void Execute(object target);

		public class Factory : PlaceholderFactory<Action> { }
	}

	[System.Serializable]
	public sealed class ActionInformation : Information
	{
	}
}