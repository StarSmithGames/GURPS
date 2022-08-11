using UnityEngine;

using Zenject;

namespace Game.UI
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