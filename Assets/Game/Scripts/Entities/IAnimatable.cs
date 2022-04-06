public interface IAnimatable
{
	AnimatorControl AnimatorControl { get; }

	void Attack(int type = -1);
	void Hit(int type = -1);
}