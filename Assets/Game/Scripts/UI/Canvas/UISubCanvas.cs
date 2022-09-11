using UnityEngine;

namespace Game.UI.CanvasSystem
{
	public class UISubCanvas : UICanvas
	{
		public Transform VFXIndicators
		{
			get
			{
				if (vfxIndicators == null)
				{
					vfxIndicators = transform.Find("VFX/Indicators");
				}

				return vfxIndicators;
			}
		}
		private Transform vfxIndicators;
	}
}