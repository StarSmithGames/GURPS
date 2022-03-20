public interface IDamegeable<T> where T : struct
{
	void ApplyDamage(T value);
}