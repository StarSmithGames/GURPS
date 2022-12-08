using UnityEngine.Events;

namespace Game.Systems.SheetSystem
{
    public interface IAction
    {
		event UnityAction<IAction> onUsed;

		void Use();
    }
}