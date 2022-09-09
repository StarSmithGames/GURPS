using UnityEngine;

namespace Game.Systems.SheetSystem.Actions
{
	public interface IAction
	{
		void Execute(object target);
	}

	public abstract class BaseAction : ScriptableObject, IAction
	{
		public abstract void Execute(object target);
	}
}