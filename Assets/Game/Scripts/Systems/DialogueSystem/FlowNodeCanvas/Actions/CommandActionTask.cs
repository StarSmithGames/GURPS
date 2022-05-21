using Game.Systems.ContextMenu;

using NodeCanvas.Framework;

namespace Game.Systems.DialogueSystem.Nodes
{
	public abstract class CommandActionTask : ActionTask
	{
		public ICommand command;

		public virtual void Initialize() { }
	}
}