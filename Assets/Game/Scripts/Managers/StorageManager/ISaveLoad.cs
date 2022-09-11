using System;
using UnityEngine;

using Zenject;

namespace Game.Managers.StorageManager
{
	public interface ISaveLoad
	{
		void Save(CommitType saveType);
		void Save(CommitType saveType, string title);

		// Get currently selected storage
		Storage GetStorage();

		void Clear();
	}

	public class PlayerPrefsSaveLoad : ISaveLoad, IInitializable, IDisposable
	{
		private SignalBus signalBus;
		private Settings settings;

		private DefaultData defaultData;
		private Storage activeStorage;

		public PlayerPrefsSaveLoad(SignalBus signalBus, DefaultData defaultData, Settings settings)
		{
			this.signalBus = signalBus;
			this.defaultData = defaultData;
			this.settings = settings;
		}

		public void Initialize()
		{
			if (activeStorage == null)
			{
				Load();
			}
		}

		public void Dispose()
		{
			//Save();
		}

		public void Save(CommitType saveType)
		{
			Save(saveType, "");
		}

		public void Save(CommitType saveType, string title)
		{
			signalBus?.Fire(new SignalSaveStorage() { storage = activeStorage, title = title, saveType = saveType });

			string preferenceName = settings.preferenceName;
			PlayerPrefs.SetString(preferenceName, activeStorage.Database.GetJson());
			PlayerPrefs.Save();

			signalBus?.Fire(new SignalStorageSaved());

			Debug.Log($"[PlayerPrefsSaveLoad] Save storage to pref: {preferenceName}");
		}

		public void Load()
		{
			if (PlayerPrefs.HasKey(settings.preferenceName))
			{
				string json = PlayerPrefs.GetString(settings.preferenceName);

				activeStorage = new Storage(json);
			}
			else//first time
			{
				activeStorage = new Storage(defaultData);

				Debug.Log($"[PlayerPrefsSaveLoad] Create new save");

				//Save();
			}

			signalBus?.Fire(new SignalStorageLoaded() { storage = activeStorage });

			Debug.Log($"[PlayerPrefsSaveLoad] Load storage from pref: {settings.preferenceName}");
		}

		public void Clear()
		{
			if (PlayerPrefs.HasKey(settings.preferenceName))
			{
				PlayerPrefs.DeleteKey(settings.preferenceName);
			}

			activeStorage.Clear();

			signalBus?.Fire(new SignalStorageCleared());
		}

		public Storage GetStorage()
		{
			if(activeStorage == null)
			{
				Load();
			}

			return activeStorage;
		}

		[System.Serializable]
		public class Settings
		{
			public string preferenceName = "save_data";
			public string storageDisplayName = "Profile";
			public string storageFileName = "Profile.dat";
		}
	}
}