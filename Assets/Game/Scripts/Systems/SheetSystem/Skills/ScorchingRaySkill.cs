using Game.Entities;
using Game.Systems.VFX;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class ScorchingRaySkill : TargetSkill
	{
		public override SkillData Data => data;
		protected ScorchingRayData data;

		private ElectricBallProjectileVFX.Factory electricBallFactory;

		[Inject]
		public void Construct(ScorchingRayData data, [Inject(Id = "Version2")]ElectricBallProjectileVFX.Factory electricBallFactory)
		{
			this.data = data;

			this.electricBallFactory = electricBallFactory;
		}

		protected override ProjectileVFX GetProjectile()
		{
			return electricBallFactory.Create();
		}

		public class Factory : PlaceholderFactory<ScorchingRayData, ICharacter, ScorchingRaySkill> { }
	}
}