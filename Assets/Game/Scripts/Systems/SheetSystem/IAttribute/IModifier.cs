namespace Game.Systems.SheetSystem
{
	public interface IModifier<T> where T : struct
	{
		T Value { get; }
	}

	public class Modifier<T> : IModifier<T> where T : struct
	{
		public T Value { get; set; }

		public Modifier(T value)
		{
			Value = value;
		}
	}
}