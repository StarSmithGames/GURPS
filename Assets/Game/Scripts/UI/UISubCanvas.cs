using Game.UI.Windows;

using UnityEngine;

namespace Game.UI
{
	public class UISubCanvas : MonoBehaviour
	{
		public WindowsManager WindowsManager
		{
			get
			{
				if(windowsManager == null)
				{
					windowsManager = new WindowsManager();
				}

				return windowsManager;
			}
		}
		private WindowsManager windowsManager;

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