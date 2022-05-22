using Game.Systems.ContextMenu;

using NodeCanvas.Framework;

namespace Game.Systems.DialogueSystem.Nodes
{
	public abstract class CommandActionTask : ActionTask
	{
		public ICommand Command { get; protected set; }

		public virtual void Initialize() { }
	}
}