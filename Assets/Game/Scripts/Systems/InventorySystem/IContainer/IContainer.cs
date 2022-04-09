namespace Game.Systems.InventorySystem
{
    public interface IContainer
    {
        bool IsOpened { get; }

        bool IsSearched { get; }

        void Open();
        void Close();
    }
}