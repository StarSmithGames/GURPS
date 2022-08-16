using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Game.Managers.InputManager;
using Game.Managers.StorageManager;

namespace Game.UI.Windows
{
	public class WindowMainMenu : MonoBehaviour, IWindow
	{
		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; } 
		[field: SerializeField] public Button Continue { get; private set; }
		[field: SerializeField] public Button QSave { get; private set; }
		[field: SerializeField] public Button Save { get; private set; }
		[field: SerializeField] public Button Load { get; private set; }
		[field: SerializeField] public Button Preferences { get; private set; }
		[field: SerializeField] public Button Exit { get; private set; }

		private bool isProcess = false;
		private WindowLoadingCommit windowLoadingCommit = null;

		private UISubCanvas subCanvas;
		private InputManager inputManager;
		private ISaveLoad saveLoad;

		[Inject]
		private void Construct(UISubCanvas subCanvas, InputManager inputManager, ISaveLoad saveLoad)
		{
			this.subCanvas = subCanvas;
			this.inputManager = inputManager;
			this.saveLoad = saveLoad;
		}

		private void Start()
		{
			Enable(false);

			Preferences.gameObject.SetActive(false);

			subCanvas.WindowsManager.Register(this);

			Continue.onClick.AddListener(OnContinue);
			QSave.onClick.AddListener(OnQSave);
			Save.onClick.AddListener(OnSave);
			Load.onClick.AddListener(OnLoad);
			Preferences.onClick.AddListener(OnPreferences);
			Exit.onClick.AddListener(OnExit);
		}

		private void OnDestroy()
		{
			subCanvas?.WindowsManager.UnRegister(this);

			Continue?.onClick.RemoveAllListeners();
			QSave?.onClick.RemoveAllListeners();
			Save?.onClick.RemoveAllListeners();
			Load?.onClick.RemoveAllListeners();
			Preferences?.onClick.RemoveAllListeners();
			Exit?.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (!isProcess && (windowLoadingCommit == null || !windowLoadingCommit.IsShowing))
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

		private void OnQSave()
		{
			saveLoad.Save(CommitType.QuickSave);
		}

		private void OnSave()
		{
			windowLoadingCommit = subCanvas.WindowsManager.GetAs<WindowLoadingCommit>();
			windowLoadingCommit.IsLoading = false;
			windowLoadingCommit.Show();
		}

		private void OnLoad()
		{
			windowLoadingCommit = subCanvas.WindowsManager.GetAs<WindowLoadingCommit>();
			windowLoadingCommit.IsLoading = true;
			windowLoadingCommit.Show();
		}

		private void OnPreferences()
		{

		}

		private void OnExit()
		{

		}
	}
}