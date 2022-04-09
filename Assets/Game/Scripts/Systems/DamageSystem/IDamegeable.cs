
namespace Game.Systems.DamageSystem
{
	public interface IDamegeable
	{
		void ApplyDamage<T>(T value) where T : struct;
		Damage GetDamage();
	}
}