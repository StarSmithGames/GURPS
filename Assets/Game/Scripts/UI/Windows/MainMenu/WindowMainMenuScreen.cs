using Game.Managers.GameManager;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Managers.TransitionManager;
using Game.UI.MainMenu;
using Game.UI.Windows;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.UI.MainMenu
{
	public class WindowMainMenuScreen : MonoBehaviour
	{
		[field: SerializeField] public Button Continue { get; private set; }
		[field: SerializeField] public Button NewGame { get; private set; }
		[field: SerializeField] public Button Load { get; private set; }
		[field: SerializeField] public Button Preferences { get; private set; }
		[field: SerializeField] public Button Exit { get; private set; }

		private ISaveLoad saveLoad;
		private SaveLoadOverseer saveLoadOverseer;
		private UISubCanvas subCanvas;

		[Inject]
		private void Construct(ISaveLoad saveLoad, UISubCanvas subCanvas, SaveLoadOverseer saveLoadOverseer)
		{
			this.subCanvas = subCanvas;
			this.saveLoad = saveLoad;
			this.saveLoadOverseer = saveLoadOverseer;
		}

		private void Start()
		{
			Preferences.gameObject.SetActive(false);

			bool isAvailable = saveLoad.GetStorage().CurrentProfile.GetData() != null && saveLoad.GetStorage().CurrentProfile.GetData().commits.Count > 0;
			Continue.gameObject.SetActive(isAvailable);
			Load.interactable = isAvailable;

			Continue.onClick.AddListener(OnContinueClick);
			NewGame.onClick.AddListener(OnNewGameClick);
			Load.onClick.AddListener(OnLoadClick);
			Preferences.onClick.AddListener(OnPreferencesClick);
		}

		private void OnDestroy()
		{
			Continue?.onClick.RemoveAllListeners();
			NewGame?.onClick.RemoveAllListeners();
			Load?.onClick.RemoveAllListeners();
			Preferences?.onClick.RemoveAllListeners();
		}

		private void OnContinueClick()
		{
			Continue.enabled = false;

			saveLoadOverseer.LoadLastGame();
		}

		private void OnNewGameClick()
		{
			NewGame.enabled = false;

			saveLoadOverseer.LoadNewGame();
		}

		private void OnLoadClick()
		{
			var window = subCanvas.WindowsManager.GetAs<WindowLoadingCommit>();
			window.IsLoading = true;
			window.Show();
		}

		private void OnPreferencesClick()
		{

		}
	}
}