using UnityEngine;

namespace Game.UI.CanvasSystem
{
	public class UIGlobalCanvas : UICanvas
	{
		public Transform Transitions
		{
			get
			{
				if(transitions == null)
				{
					transitions = transform.Find("Transitions");
				}

				return transitions;
			}
		}
		private Transform transitions;
	}
}