using Game.Entities;

namespace Game.Systems.SheetSystem
{
	public sealed class Effects : Registrator<Effect>
	{
		private ISheet sheet;
		private AsyncManager asyncManager;

		public Effects(ICharacter character, AsyncManager asyncManager)
		{
			this.sheet = character.Sheet;
			this.asyncManager = asyncManager;
		}

		public override bool Registrate(Effect register)
		{
			var copy = register.Copy();
			if (base.Registrate(copy))
			{
				copy.Activate(sheet);
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
	public abstract class Effect : IEffect, ICopyable<Effect>
	{
		public bool IsActive { get; private set; }

		public EffectData data;

		private ISheet sheet;

		public virtual void Activate(ISheet sheet)
		{
			IsActive = true;

			this.sheet = sheet;
		}

		public virtual void Deactivate()
		{
			IsActive = false;

			sheet = null;
		}

		public abstract Effect Copy();
	}

	[System.Serializable]
	public class HPEffect : Effect
	{
		public float healAmmount;

		public override void Activate(ISheet sheet)
		{
			base.Activate(sheet);

			sheet.Stats.HitPoints.CurrentValue += healAmmount;

			Deactivate();
		}

		public override Effect Copy()
		{
			return new HPEffect()
			{
				data = data,
				healAmmount = healAmmount,
			};
		}
	}
}