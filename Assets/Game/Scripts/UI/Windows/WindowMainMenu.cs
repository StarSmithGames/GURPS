using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Game.Managers.InputManager;
using System.Diagnostics;

namespace Game.UI.Windows
{
	public class WindowMainMenu : MonoBehaviour, IWindow
	{
		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; } 
		[field: SerializeField] public Button Continue { get; private set; }
		[field: SerializeField] public Button Exit { get; private set; }

		private bool isProcess = false;

		private UISubCanvas subCanvas;
		private InputManager inputManager;

		[Inject]
		private void Construct(UISubCanvas subCanvas, InputManager inputManager)
		{
			this.subCanvas = subCanvas;
			this.inputManager = inputManager;
		}

		private void Start()
		{
			Enable(false);
			subCanvas.WindowsManager.Register(this);

			Continue.onClick.AddListener(OnContinue);
			Exit.onClick.AddListener(OnExit);
		}

		private void OnDestroy()
		{
			subCanvas?.WindowsManager.UnRegister(this);

			Continue?.onClick.RemoveAllListeners();
			Exit?.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (!isProcess)
			{
				if (inputManager.GetKeyDown(KeyAction.InGameMenu))
				{
					if (!IsShowing)
					{
						Show();
					}
					else
					{
						Hide();
					}
				}
			}
		}

		public void Enable(bool trigger)
		{
			CanvasGroup.alpha = trigger ? 1f : 0f;

			CanvasGroup.blocksRaycasts = trigger;
			CanvasGroup.interactable = trigger;

			IsShowing = trigger;

			isProcess = false;
		}

		public void Show(UnityAction callback = null)
		{
			isProcess = true;
			IsShowing = true;

			CanvasGroup.alpha = 0f;

			CanvasGroup.blocksRaycasts = true;
			CanvasGroup.interactable = true;

			CanvasGroup.DOFade(1f, 0.2f)
				.OnComplete(() =>
				{
					isProcess = false;
					callback?.Invoke();
				});
		}

		public void Hide(UnityAction callback = null)
		{
			isProcess = true;

			CanvasGroup.DOFade(0f, 0.15f)
				.OnComplete(() =>
				{
					CanvasGroup.blocksRaycasts = false;
					CanvasGroup.interactable = false;

					IsShowing = false;
					isProcess = false;
					callback?.Invoke();
				});
		}

		private void OnContinue()
		{
			Hide();
		}

		private void OnExit()
		{

		}
	}
}