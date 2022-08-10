using Game.Systems.SheetSystem;

namespace Game.Systems.DialogueSystem.Nodes
{
	public interface IRequirement
	{
		bool Check();
	}

	public abstract class SheetRequirement : IRequirement
	{
		protected ISheet sheet;

		public SheetRequirement(ISheet sheet)
		{
			this.sheet = sheet;
		}

		public abstract bool Check();
	}

	public class AlignmentRequirement : SheetRequirement
	{
		public AlignmentType alignmentRequired;

		public AlignmentRequirement(ISheet sheet) : base(sheet) { }

		public override bool Check()
		{
			return alignmentRequired == (sheet.Characteristics.Alignment as AlignmentCharacteristic).AlignmentType;
		}
	}
}