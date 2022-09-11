using Game.UI.Windows;
using UnityEngine;

namespace Game.UI.CanvasSystem
{
	public class UICanvas : MonoBehaviour
	{
		public WindowsRegistrator WindowsRegistrator
		{
			get
			{
				if (windowsManager == null)
				{
					windowsManager = new WindowsRegistrator();
				}

				return windowsManager;
			}
		}
		protected WindowsRegistrator windowsManager;

		public Transform Windows
		{
			get
			{
				if (windows == null)
				{
					windows = transform.Find("Windows");
				}

				return windows;
			}
		}
		protected Transform windows;
	}
}