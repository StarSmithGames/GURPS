using Game.Managers.GameManager;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Managers.TransitionManager;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.UI.Windows
{
	public class WindowMainMenuScreen : MonoBehaviour
	{
		[field: SerializeField] public Button ButtonContinue;
		[field: SerializeField] public Button ButtonNewGame;
		[field: SerializeField] public Button ButtonLoad;
		[field: SerializeField] public Button ButtonPreferences;
		[field: SerializeField] public Button ButtonExit;

		private ISaveLoad saveLoad;
		private SaveLoadOverseer saveLoadOverseer;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas, ISaveLoad saveLoad, SaveLoadOverseer saveLoadOverseer)
		{
			this.saveLoad = saveLoad;
			this.saveLoadOverseer = saveLoadOverseer;
		}

		private void Start()
		{
			bool isAvailable = saveLoad.GetStorage().CurrentProfile.GetData() != null && saveLoad.GetStorage().CurrentProfile.GetData().commits.Count > 0;
			ButtonContinue.gameObject.SetActive(isAvailable);

			ButtonContinue.onClick.AddListener(OnContinueClick);
			ButtonNewGame.onClick.AddListener(OnNewGameClick);
			ButtonLoad.onClick.AddListener(OnLoadClick);
			ButtonPreferences.onClick.AddListener(OnPreferencesClick);
		}

		private void OnDestroy()
		{
			ButtonContinue?.onClick.RemoveAllListeners();
			ButtonNewGame?.onClick.RemoveAllListeners();
			ButtonLoad?.onClick.RemoveAllListeners();
			ButtonPreferences?.onClick.RemoveAllListeners();
		}

		private void OnContinueClick()
		{
			ButtonContinue.enabled = false;

			saveLoadOverseer.LoadLastGame();
		}

		private void OnNewGameClick()
		{
			ButtonNewGame.enabled = false;

			saveLoadOverseer.LoadNewGame();
		}

		private void OnLoadClick()
		{

		}

		private void OnPreferencesClick()
		{

		}
	}
}