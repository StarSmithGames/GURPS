using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Game.Managers.InputManager;
using Game.Managers.StorageManager;
using Game.UI.CanvasSystem;
using Game.UI.Windows;
using Game.Managers.SceneManager;
using Sirenix.OdinInspector;

namespace Game.Menu
{
	public class WindowMainMenu : WindowBase
	{
		[field: SerializeField] public Button Continue { get; private set; }
		[field: SerializeField] public Button QSave { get; private set; }
		[field: SerializeField] public Button Save { get; private set; }
		[field: SerializeField] public Button Load { get; private set; }
		[field: SerializeField] public Button Preferences { get; private set; }
		[field: SerializeField] public Button Menu { get; private set; }
		[field: SerializeField] public Button Exit { get; private set; }

		[SerializeField] private SceneName menu;

		private WindowLoadingCommit windowLoadingCommit = null;

		private UISubCanvas gameCanvas;
		private InputManager inputManager;
		private LoadingController loadingController;

		[Inject]
		private void Construct(UIGameCanvas gameCanvas, InputManager inputManager, LoadingController loadingController)
		{
			this.gameCanvas = gameCanvas;
			this.inputManager = inputManager;
			this.loadingController = loadingController;
		}

		private void Start()
		{
			Enable(false);

			//Preferences.gameObject.SetActive(false);

			Continue.onClick.AddListener(OnContinue);
			//QSave.onClick.AddListener(OnQSave);
			//Save.onClick.AddListener(OnSave);
			//Load.onClick.AddListener(OnLoad);
			//Preferences.onClick.AddListener(OnPreferences);
			Menu.onClick.AddListener(OnGoToMenu);
			Exit.onClick.AddListener(OnExit);

			gameCanvas.WindowsRegistrator.Registrate(this);
		}

		private void OnDestroy()
		{
			Continue?.onClick.RemoveAllListeners();
			QSave?.onClick.RemoveAllListeners();
			Save?.onClick.RemoveAllListeners();
			Load?.onClick.RemoveAllListeners();
			Preferences?.onClick.RemoveAllListeners();
			Menu?.onClick.RemoveAllListeners();
			Exit?.onClick.RemoveAllListeners();

			gameCanvas.WindowsRegistrator.UnRegistrate(this);
		}

		private void Update()
		{
			if (!IsInTransition && (windowLoadingCommit == null || !windowLoadingCommit.IsShowing))
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

		private void OnContinue()
		{
			Hide();
		}

		private void OnQSave()
		{
			//saveLoad.Save(CommitType.QuickSave);
		}

		private void OnSave()
		{
			windowLoadingCommit = gameCanvas.WindowsRegistrator.GetAs<WindowLoadingCommit>();
			windowLoadingCommit.IsLoading = false;
			windowLoadingCommit.Show();
		}

		private void OnLoad()
		{
			windowLoadingCommit = gameCanvas.WindowsRegistrator.GetAs<WindowLoadingCommit>();
			windowLoadingCommit.IsLoading = true;
			windowLoadingCommit.Show();
		}

		private void OnPreferences()
		{

		}

		private void OnGoToMenu()
		{
			loadingController.LoadScene(menu);
		}

		private void OnExit()
		{
			loadingController.ExitGame();
		}
	}
}