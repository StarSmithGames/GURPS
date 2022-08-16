using Game.Managers.InputManager;
using Game.Managers.StorageManager;

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
		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public Button RemoveSaves { get; private set; }

		[field: SerializeField] public Button Back { get; private set; }

		private ISaveLoad saveLoad;
		private UIGlobalCanvas globalCanvas;
		private UISubCanvas subCanvas;
		private InputManager inputManager;

		[Inject]
		private void Construct(ISaveLoad saveLoad, UIGlobalCanvas globalCanvas, UISubCanvas subCanvas, InputManager inputManager)
		{
			this.saveLoad = saveLoad;
			this.globalCanvas = globalCanvas;
			this.subCanvas = subCanvas;
			this.inputManager = inputManager;
		}

		private void Start()
		{
			Enable(false);

			RemoveSaves.onClick.AddListener(OnRemoveSaves);
			Back.onClick.AddListener(OnBack);

			subCanvas.WindowsManager.Register(this);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsManager.UnRegister(this);

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

		public override void Enable(bool trigger)
		{
			CanvasGroup.Enable(trigger);
			IsShowing = trigger;
		}

		public override void Show(UnityAction callback = null)
		{
			Enable(true);
			IsShowing = true;
		}

		public override void Hide(UnityAction callback = null)
		{
			IsShowing = false;
			Enable(false);
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