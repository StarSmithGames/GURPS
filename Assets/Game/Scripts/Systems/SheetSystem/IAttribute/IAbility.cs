using Sirenix.OdinInspector;

using Zenject;

namespace Game.Systems.SheetSystem.Abilities
{
	public interface IAbility
	{
		AbilityData GetData();
	}

	public abstract class BaseAbility<T> : IAbility where T : AbilityData
	{
		public T Data { get; protected set; }

		public BaseAbility(T data)
		{
			Data = data;
		}

		public AbilityData GetData() => Data;
	}

	[InlineProperty]
	[System.Serializable]
	public class AttackAbility : BaseAbility<AttackAbilityData>
	{
		public bool isHumanoid;

		public AttackAbility(AttackAbility ability, AttackAbilityData data) : base(data)
		{
			isHumanoid = ability.isHumanoid;
		}
	}

	public interface IActivation
	{
		void Activate();
	}

	public abstract class Activation : IActivation
	{
		protected IAbility ability;

		public Activation(IAbility ability)
		{
			this.ability = ability;
		}

		public abstract void Activate();
	}

	public class InstantActivation : Activation
	{
		public InstantActivation(IAbility ability) : base(ability) { }

		public override void Activate()
		{

		}

		public class Factory : PlaceholderFactory<IAbility, InstantActivation> { }
	}

	public class CastActivation : Activation
	{
		public CastActivation(IAbility ability) : base(ability) { }

		public override void Activate()
		{

		}

		public class Factory : PlaceholderFactory<IAbility, CastActivation> { }
	}

	public class ActivationFactory : PlaceholderFactory<IAbility, IActivation> { }

	public class CustomActivationFactory : IFactory<IAbility, IActivation>
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

		public IActivation Create(IAbility ability)
		{
			if(ability.GetData().activation == ActivationType.Casted)
			{
				return castActivationFactory.Create(ability);
			}

			return instantActivationFactory.Create(ability);
		}
	}

	public enum ActivationType
	{
		Instant,
		Casted,
	}
}