using Game.Managers.GameManager;
using Game.Managers.StorageManager;
using Game.UI.CanvasSystem;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.Menu
{
	public class WindowMainMenuScreen : MonoBehaviour
	{
		[field: SerializeField] public Button Continue { get; private set; }
		[field: SerializeField] public Button NewGame { get; private set; }
		[field: SerializeField] public Button Load { get; private set; }
		[field: SerializeField] public Button Preferences { get; private set; }
		[field: SerializeField] public Button Exit { get; private set; }

		private SignalBus signalBus;
		private ISaveLoad saveLoad;
		private LoadingController loadingController;
		private UISubCanvas subCanvas;
		private GameManager gameManager;

		[Inject]
		private void Construct(SignalBus signalBus, ISaveLoad saveLoad, UISubCanvas subCanvas, GameManager gameManager, LoadingController loadingController)
		{
			this.signalBus = signalBus;
			this.subCanvas = subCanvas;
			this.saveLoad = saveLoad;
			this.gameManager = gameManager;
			this.loadingController = loadingController;
		}

		private void Start()
		{
			RefreshMenu();

			Continue.onClick.AddListener(OnContinueClick);
			NewGame.onClick.AddListener(OnNewGameClick);
			Load.onClick.AddListener(OnLoadClick);
			Preferences.onClick.AddListener(OnPreferencesClick);
			Exit.onClick.AddListener(OnExit);

			signalBus?.Subscribe<SignalStorageCleared>(OnStorageCleared);

			gameManager.ChangeState(GameState.Menu);
		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalStorageCleared>(OnStorageCleared);

			Continue?.onClick.RemoveAllListeners();
			NewGame?.onClick.RemoveAllListeners();
			Load?.onClick.RemoveAllListeners();
			Preferences?.onClick.RemoveAllListeners();
			Exit?.onClick.RemoveAllListeners();
		}

		private void RefreshMenu()
		{
			bool isAvailable = saveLoad.GetStorage().CurrentProfile.GetData() != null && saveLoad.GetStorage().CurrentProfile.GetData().commits.Count > 0;
			Continue.gameObject.SetActive(isAvailable);
			Load.interactable = isAvailable;
		}

		private void OnContinueClick()
		{
			Continue.enabled = false;

			loadingController.LoadLastGame();
		}

		private void OnNewGameClick()
		{
			NewGame.enabled = false;

			loadingController.LoadNewGame();
		}

		private void OnLoadClick()
		{
			var window = subCanvas.WindowsRegistrator.GetAs<WindowLoadingCommit>();
			window.IsLoading = true;
			window.Show();
		}

		private void OnPreferencesClick()
		{
			subCanvas.WindowsRegistrator.Show<WindowPreferences>();
		}

		private void OnExit()
		{
			loadingController.ExitGame();
		}

		private void OnStorageCleared(SignalStorageCleared signal)
		{
			RefreshMenu();
		}
	}
}