using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

namespace Game.Systems.InventorySystem
{
    public interface IContainer : ISheetable, IInteractable, IObservable
    {
        bool IsOpened { get; }

        bool IsLocked { get; }
        bool IsSearched { get; }

        void UnLock(IInteractable interactor);
        void Open(IInteractable interactor);
        void Close();
    }
}