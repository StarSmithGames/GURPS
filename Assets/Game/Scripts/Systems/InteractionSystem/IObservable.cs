namespace Game.Systems.InteractionSystem
{
	public interface IObservable
	{
		void StartObserve();
		void Observe();
		void EndObserve();
	}
}