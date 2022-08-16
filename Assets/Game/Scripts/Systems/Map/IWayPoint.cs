using Game.Systems.InteractionSystem;

namespace Game.Map
{
	public interface IWayPoint
	{
		InteractionPoint InteractionPoint { get; }

		void Action();
	}
}