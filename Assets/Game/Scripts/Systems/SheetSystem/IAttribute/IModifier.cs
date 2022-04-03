namespace Game.Systems.SheetSystem
{
	public interface IModifier
	{
		float Value { get; }
	}

	public class Modifier : IModifier
	{
		public float Value { get; set; }

		public Modifier(float value)
		{
			Value = value;
		}
	}
}