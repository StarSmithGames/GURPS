using Game.Entities;
using Game.Systems;
using Game.UI.Windows;
using Game.UI;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;
using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;

namespace Game.Managers.StorageManager
{
	//Attached on Scene Context
	public class SaveLoadOverseer : IInitializable, IDisposable
	{

		private Transitions transitionIn = Transitions.Fade;
		private Transitions transitionOut = Transitions.Fade;

		private SignalBus signalBus;
		private DiContainer container;
		private ISaveLoad saveLoad;
		private GameManager.GameManager gameManager;
		private SceneManager.SceneManager sceneManager;
		private UIGlobalCanvas globalCanvas;

		public SaveLoadOverseer(SignalBus signalBus, DiContainer container, ISaveLoad saveLoad, GameManager.GameManager gameManager,
			SceneManager.SceneManager sceneManager, UIGlobalCanvas globalCanvas)
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
			//signalBus?.Subscribe<SignalStorageSaved>(OnStorageSaved);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalSaveStorage>(OnSaveStorage);
			//signalBus?.Unsubscribe<SignalStorageSaved>(OnStorageSaved);
		}

		public void LoadLastGame()
		{
			var lastCommit = saveLoad.GetStorage().CurrentProfile.GetData().commits.Last();

			globalCanvas.WindowsManager.GetAs<WindowInfinityLoading>().Show(lastCommit.data.lastScene, transitionIn, transitionOut, () =>
			{
				var playerRTS = container.Resolve<PlayerRTS>();

				if (gameManager.CurrentGameLocation == GameManager.GameLocation.Map)
				{
					var map = lastCommit.data.lastTransformOnMap;

					playerRTS.transform.position = map.position;
					playerRTS.transform.rotation = map.rotation;
					playerRTS.transform.localScale = map.scale;
				}
				else if (gameManager.CurrentGameLocation == GameManager.GameLocation.Location)
				{
					FastStorage.LastTransformOnMap = lastCommit.data.lastTransformOnMap;

					var location = lastCommit.data.lastTransformInLocation;

					playerRTS.transform.position = location.position;
					playerRTS.transform.rotation = location.rotation;
					playerRTS.transform.localScale = location.scale;
				}
			});
		}

		public void LoadNewGame()
		{
			var list = saveLoad.GetStorage().Profilers.GetData();

			if (list == null)
			{
				list = new List<Profile>();
			}

			var profile = new Profile()
			{
				time = DateTime.Now,
			};

			list.Add(profile);

			saveLoad.GetStorage().CurrentProfile.SetData(profile);
			saveLoad.GetStorage().Profilers.SetData(list);

			globalCanvas.WindowsManager.GetAs<WindowInfinityLoading>().Show(Scenes.Map, transitionIn, transitionOut);
		}

		private void OnSaveStorage(SignalSaveStorage signal)
		{
			var storage = signal.storage;

			storage.CurrentProfile.SetData(GetData(signal.saveType));
			//storage.Profilers.SetData();//need update
		}

		public Profile GetData(CommitType saveType)
		{
			var profile = saveLoad.GetStorage().CurrentProfile.GetData();

			DefaultTransform transformOnMap = null;
			DefaultTransform transformInLocation = null;

			if (gameManager.CurrentGameLocation == GameManager.GameLocation.Map)
			{
				var playerRTS = container.Resolve<PlayerRTS>();//>:C
				Assert.IsNotNull(playerRTS, "PlayerRTS == NULL");

				transformOnMap = new DefaultTransform()
				{
					position = playerRTS.transform.position,
					rotation = playerRTS.transform.rotation,
					scale = playerRTS.transform.localScale,
				};
			}
			else if (gameManager.CurrentGameLocation == GameManager.GameLocation.Location)
			{
				transformOnMap = FastStorage.LastTransformOnMap;
				//transformInLocation = new DefaultTransform()
				//{
					//position = playerRTS.transform.position,
					//rotation = playerRTS.transform.rotation,
					//scale = playerRTS.transform.localScale,
				//};
			}

			profile.commits.Add(new Commit()
			{
				guid = Guid.NewGuid().ToString(),
				type = saveType,
				time = DateTime.Now,
				data = new Commit.Data()
				{
					lastScene = sceneManager.CurrentScene,
					lastTransformOnMap = transformOnMap,
					lastTransformInLocation = transformInLocation,
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
		public CommitType type = CommitType.ManualSave;
		public DificaltyType dificalty = DificaltyType.Normal;
		public DateTime time;
		//public string sceenshotPath;

		public Data data;

		public class Data
		{
			public string lastScene;

			public DefaultTransform lastTransformOnMap;
			public DefaultTransform lastTransformInLocation;
		}
	}

	public enum CommitType
	{
		AutoSave,
		QuickSave,
		ManualSave,
		Checkpoint,
	}

	public class DefaultTransform
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
	}
}