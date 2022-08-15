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
	}
}