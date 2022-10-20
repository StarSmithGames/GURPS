namespace Game.Systems.SheetSystem
{
	public sealed class Effects : Registrator<Effect>
	{
		public Effects(ISheet sheet)
		{

		}

		public override bool Registrate(Effect register)
		{
			if (base.Registrate(register))
			{
				register.Activate();
				return true;
			}

			return false;
		}

		public override bool UnRegistrate(Effect register)
		{
			if (base.UnRegistrate(register))
			{
				register.Deactivate();
				return true;
			}

			return false;
		}
	}

	[System.Serializable]
	public abstract class Effect : IEffect
	{
		public bool IsActive { get; private set; }

		public EffectData data;

		public virtual void Activate()
		{
			IsActive = true;
		}

		public virtual void Deactivate()
		{
			IsActive = false;
		}
	}

	[SearchPath("★Effects/HP Effect")]
	[System.Serializable]
	public class HPEffect : Effect
	{
		public float healAmmount;
	}
}