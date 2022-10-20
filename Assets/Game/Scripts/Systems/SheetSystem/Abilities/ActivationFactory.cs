using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public abstract class Activation : IActivation
	{
		public bool IsActive { get; }

		protected IAbility ability;

		public Activation(IAbility ability)
		{
			this.ability = ability;
		}

		public abstract void Activate();

		public void Deactivate()
		{
			throw new System.NotImplementedException();
		}
	}

	public class InstantActivation : Activation
	{
		public InstantActivation(IAbility ability) : base(ability) { }

		public override void Activate()
		{
			ability.Apply();
		}

		public class Factory : PlaceholderFactory<IAbility, InstantActivation> { }
	}

	public class CastActivation : Activation
	{
		private AsyncManager asyncManager;

		public CastActivation(IAbility ability, AsyncManager asyncManager) : base(ability)
		{
			this.asyncManager = asyncManager;
		}

		public override void Activate()
		{
			//asyncManager
		}

		public class Factory : PlaceholderFactory<IAbility, CastActivation> { }
	}

	public class ActivationFactory : PlaceholderFactory<ActivationType, IAbility, IActivation> { }

	public class CustomActivationFactory : IFactory<ActivationType, IAbility, IActivation>
	{
		private InstantActivation.Factory instantActivationFactory;
		private CastActivation.Factory castActivationFactory;

		public CustomActivationFactory(
			InstantActivation.Factory instantActivationFactory,
			CastActivation.Factory castActivationFactory)
		{
			this.instantActivationFactory = instantActivationFactory;
			this.castActivationFactory = castActivationFactory;
		}

		public IActivation Create(ActivationType activation, IAbility ability)
		{
			if(activation == ActivationType.Casted)
			{
				return castActivationFactory.Create(ability);
			}

			return instantActivationFactory.Create(ability);
		}
	}

	public enum BehaviorType
	{
		AuraSpell,
		BoostEffect
	}

	public enum ActivationType
	{
		Instant,
		Casted,
		CastedTurn,
		CastedArea,
	}
}