using Game.Managers.InputManager;
using Game.Managers.StorageManager;
using Game.UI.CanvasSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.UI.Windows
{
	public class WindowPreferences : WindowBase
	{
		[field: SerializeField] public Button RemoveSaves { get; private set; }

		[field: SerializeField] public Button Back { get; private set; }

		private ISaveLoad saveLoad;
		private UISubCanvas subCanvas;
		private InputManager inputManager;

		[Inject]
		private void Construct(ISaveLoad saveLoad, UISubCanvas subCanvas, InputManager inputManager)
		{
			this.saveLoad = saveLoad;
			this.subCanvas = subCanvas;
			this.inputManager = inputManager;
		}

		private void Start()
		{
			Enable(false);

			RemoveSaves.onClick.AddListener(OnRemoveSaves);
			Back.onClick.AddListener(OnBack);

			subCanvas.WindowsRegistrator.Registrate(this);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);

			RemoveSaves?.onClick.RemoveAllListeners();
			Back?.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (IsShowing)
			{
				if (inputManager.GetKeyDown(KeyAction.InGameMenu))
				{
					Hide();
				}
			}
		}

		private void OnRemoveSaves()
		{
			saveLoad.Clear();
			FastStorage.Clear();
		}

		private void OnBack()
		{
			Hide();
		}
	}
}