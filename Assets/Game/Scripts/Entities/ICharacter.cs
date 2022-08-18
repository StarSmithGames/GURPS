using Game.Systems.SheetSystem;

namespace Game.Entities
{
	public interface ICharacter : IEntity, ISheetable { }

	public abstract class Character : Entity, ICharacter
	{
		public virtual ISheet Sheet { get; protected set; }
	}
}