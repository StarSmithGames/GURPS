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
		public bool isHasIcon = true;
		[HideLabel]
		[PreviewField(ObjectFieldAlignment.Left, Height = 64)]
		[ShowIf("isHasIcon")]
		public Sprite icon;

		public string abilityName;
		public string abilityDisplayName;
		public string description;

		public virtual IActivation Activation { get; protected set; }

		protected ActivationFactory activationFactory;

		[Inject]
		public void Construct(ActivationFactory activationFactory)
		{
			this.activationFactory = activationFactory;
		}

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