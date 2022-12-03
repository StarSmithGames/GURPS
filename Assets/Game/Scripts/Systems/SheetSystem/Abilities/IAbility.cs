using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public interface IAbility
	{
		bool Activate();

		void Apply();
	}

	public abstract class BaseAbility : MonoBehaviour, IAbility
	{
		[HideLabel]
		public Information information;

		public virtual IActivation Activation { get; protected set; }

		public abstract void Apply();

		public virtual bool Activate()
		{
			Activation.Activate();

			return true;
		}
	}


	[System.Serializable]
	public class Requirements
	{

	}

	[System.Serializable]
	public class Requirement
	{

	}
}