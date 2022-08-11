using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.UI
{
	public class UISubCanvas : MonoBehaviour
	{
		public WindowsManager WindowsManager { get; private set; }

		[Inject]
		private void Construct(WindowsManager windowsManager)
		{
			WindowsManager = windowsManager;
		}
	}
}