using Game.Systems.InteractionSystem;

namespace Game.Systems.InventorySystem
{
    public interface IContainer : IInteractable
    {
        bool IsOpened { get; }

        bool IsSearched { get; }

        void Open();
        void Close();
    }
}