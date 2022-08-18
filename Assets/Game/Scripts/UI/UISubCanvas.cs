using Game.UI.Windows;

using UnityEngine;

namespace Game.UI
{
	public class UISubCanvas : MonoBehaviour
	{
		public WindowsRegistrator WindowsRegistrator
		{
			get
			{
				if(windowsManager == null)
				{
					windowsManager = new WindowsRegistrator();
				}

				return windowsManager;
			}
		}
		private WindowsRegistrator windowsManager;

		public Transform Windows
		{
			get
			{
				if(windows == null)
				{
					windows = transform.Find("Windows");
				}

				return windows;
			}
		}
		private Transform windows;
	}
}