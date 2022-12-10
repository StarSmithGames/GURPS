using Game.Entities;
using Game.Systems.VFX;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class GreaseSkill : TargetSkill
	{
		public override SkillData Data => data;
		private GreaseData data;

		[Inject]
		public void Construct(GreaseData data)
		{
			this.data = data;
		}

		protected override ProjectileVFX GetProjectile()
		{
			throw new System.NotImplementedException();
		}

		public class Factory : PlaceholderFactory<GreaseData, ICharacter, GreaseSkill> { }
	}
}