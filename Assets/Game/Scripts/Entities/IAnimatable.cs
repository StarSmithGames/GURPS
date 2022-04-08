public interface IAnimatable
{
	AnimatorControl AnimatorControl { get; }

	void Hit(int type = -1);
}