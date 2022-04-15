
namespace Game.Systems.DamageSystem
{
	public interface IDamegeable
	{
		void ApplyDamage<T>(T value);
		Damage GetDamage();
	}
}