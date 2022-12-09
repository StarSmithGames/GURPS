using Game.Entities;
using Game.Systems.VFX;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class BlitzBoltSkill : TargetSkill
	{
		public override SkillData Data => data;
		protected BlitzBoltData data;

		private ElectricBallProjectileVFX.Factory electricBallFactory;

		[Inject]
		public void Construct(BlitzBoltData data, ElectricBallProjectileVFX.Factory electricBallFactory)
		{
			this.data = data;
			this.electricBallFactory = electricBallFactory;
		}

		protected override ProjectileVFX GetProjectile()
		{
			return electricBallFactory.Create();
		}

		public class Factory : PlaceholderFactory<BlitzBoltData, ICharacter, BlitzBoltSkill> { }
	}
}