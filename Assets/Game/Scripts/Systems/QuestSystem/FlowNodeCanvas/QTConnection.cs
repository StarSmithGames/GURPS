using NodeCanvas.Framework;

namespace Game.Systems.QuestSystem.Nodes
{
	public class QTConnection : Connection
	{
#if UNITY_EDITOR
		public override ParadoxNotion.PlanarDirection direction
		{
			get { return ParadoxNotion.PlanarDirection.Auto; }
		}
#endif
	}
}