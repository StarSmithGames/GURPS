using UnityEngine;

using Zenject;

namespace Game.UI.GlobalCanvas
{
	public class UIGlobalCanvas : MonoBehaviour
	{
		public WindowsManager WindowsManager { get; private set; }

		[Inject]
		private void Construct(WindowsManager windowsManager)
		{
			WindowsManager = windowsManager;
		}
	}
}