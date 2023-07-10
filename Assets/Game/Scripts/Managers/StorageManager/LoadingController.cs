using Game.Systems;
using Game.UI.Windows;
using Game.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

using Zenject;
using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;
using UnityEngine.Events;
using Game.Entities;
using Game.Managers.PartyManager;
using Game.UI.CanvasSystem;
using Game.Menu;
using UnityEngine;

namespace Game.Managers.StorageManager
{
	public class LoadingController : IInitializable, IDisposable
	{
		private WindowInfinityLoading WindowInfinityLoading
		{
			get
			{
				if(windowInfinityLoading == null)
				{
					windowInfinityLoading = globalCanvas.WindowsRegistrator.GetAs<WindowInfinityLoading>();
				}

				return windowInfinityLoading;
			}
		}
		private WindowInfinityLoading windowInfinityLoading;

		private SignalBus signalBus;
		private DiContainer container;
		private ISaveLoad saveLoad;
		private GameManager.GameManager gameManager;
		private SceneManager.SceneManager sceneManager;
		private UIGlobalCanvas globalCanvas;

		public LoadingController(SignalBus signalBus, DiContainer container, ISaveLoad saveLoad,
			GameManager.GameManager gameManager,
			SceneManager.SceneManager sceneManager,
			UIGlobalCanvas globalCanvas)
		{
			this.signalBus = signalBus;
			this.container = container;
			this.saveLoad = saveLoad;
			this.gameManager = gameManager;
			this.sceneManager = sceneManager;
			this.globalCanvas = globalCanvas;
		}

		public void Initialize()
		{
			signalBus?.Subscribe<SignalSaveStorage>(OnSaveStorage);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalSaveStorage>(OnSaveStorage);
		}

		public void LoadScene(SceneName sceneName)
		{
			WindowInfinityLoading.Show(sceneName.GetScene());
		}

		public void LoadLastGame()
		{
			if (saveLoad.GetStorage().CurrentProfile.GetData().commits.Count > 0)
			{
				//last commit
				LoadGame(saveLoad.GetStorage().CurrentProfile.GetData().commits.Last());
			}
		}

		public void LoadNewGame()
		{
			var profile = saveLoad.GetStorage().CurrentProfile.GetData();

			if (profile == null)
			{
				profile = new Profile()
				{
					time = DateTime.Now,
				};

				saveLoad.GetStorage().CurrentProfile.SetData(profile);
			}

			FastStorage.Clear();

			WindowInfinityLoading.Show(SceneStorage.GetSceneName(Scenes.Polygon));
		}

		/// <param name="callback">Вызывается до появления кнопки.</param>
		public void LoadGame(Commit commit, UnityAction callback = null)
		{
			WindowInfinityLoading.Show(commit.data.lastScene, () =>
			{
				callback?.Invoke();

				//FastStorage.LastTransformOnMap = commit.data.lastTransformOnMap;

				if (gameManager.CurrentGameLocation == GameManager.GameLocation.Location)
				{
					//var location = commit.data.lastTransformInLocation;
				}

				callback?.Invoke();
			});
		}

		public void ExitGame()
		{
			Application.Quit();
		}

		//For Save
		private void OnSaveStorage(SignalSaveStorage signal)
		{
			var storage = signal.storage;

			storage.CurrentProfile.SetData(GetData(signal.title, signal.saveType));
			//storage.Profilers.SetData();//need update
		}

		public Profile GetData(string title, CommitType saveType)
		{
			var profile = saveLoad.GetStorage().CurrentProfile.GetData();

			//DefaultTransform transformOnMap = null;
			//DefaultTransform transformInLocation = null;

			if (gameManager.CurrentGameLocation == GameManager.GameLocation.Map)
			{
				//var playerRTS = player.RTSModel.transform;
				//transformOnMap = new DefaultTransform()
				//{
				//	position = playerRTS.position,
				//	rotation = playerRTS.rotation,
				//	scale = playerRTS.localScale,
				//};
			}
			else if (gameManager.CurrentGameLocation == GameManager.GameLocation.Location)
			{
				//transformOnMap = FastStorage.LastTransformOnMap;
				//transformInLocation = new DefaultTransform()
				//{
					//position = playerRTS.transform.position,
					//rotation = playerRTS.transform.rotation,
					//scale = playerRTS.transform.localScale,
				//};
			}

			if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title))
			{
				if(saveType == CommitType.QuickSave)
				{
					title = "quick_save";
				}
				else if(saveType == CommitType.AutoSave)
				{
					title = "auto_save";
				}
				else if (saveType == CommitType.Checkpoint)
				{
					title = "checkpoint";
				}
				else
				{
					title = "manual_save";
				}
			}

			profile.commits.Add(new Commit()
			{
				guid = Guid.NewGuid().ToString(),
				title = title,
				type = saveType,
				date = DateTime.Now,
				data = new Commit.Data()
				{
					lastScene = sceneManager.CurrentScene,
					//lastTransformOnMap = transformOnMap,
					//lastTransformInLocation = transformInLocation,
				},
			});

			return profile;
		}
	}

	public class Profile
	{
		public string name;
		public DateTime time;

		public List<Commit> commits = new List<Commit>();
	}

	public class Commit
	{
		public string guid;
		public string title;
		public CommitType type = CommitType.ManualSave;
		public DificaltyType dificalty = DificaltyType.Normal;
		public DateTime date;
		//public string sceenshotPath;

		public Data data;

		public class Data
		{
			public string lastScene;

			//public DefaultTransform lastTransformOnMap;
			//public DefaultTransform lastTransformInLocation;

			public Party.Data party;
			public Statiscs statiscs;
		}
	}

	public class Statiscs
	{

	}

	public enum CommitType
	{
		AutoSave,
		QuickSave,
		ManualSave,
		Checkpoint,
	}
}