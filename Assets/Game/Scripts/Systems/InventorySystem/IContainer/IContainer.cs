using Game.Systems.InteractionSystem;

namespace Game.Systems.InventorySystem
{
    public interface IContainer : IInteractable, IObservable
    {
        bool IsOpened { get; }

        bool IsLocked { get; }
        bool IsSearched { get; }

        void UnLock(IInteractable interactor);
        void Open(IInteractable interactor);
        void Close();
    }
}