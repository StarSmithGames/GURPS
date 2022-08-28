using Game.Systems.InteractionSystem;

namespace Game.Systems.InventorySystem
{
    public interface IContainer : IInteractable, IObservable
    {
        bool IsOpened { get; }

        bool IsSearched { get; }

        void Open(IInteractable interactor);
        void Close();
    }
}