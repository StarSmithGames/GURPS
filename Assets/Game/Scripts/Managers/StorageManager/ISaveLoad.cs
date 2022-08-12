using System;
using UnityEngine;

using Zenject;

namespace Game.Managers.StorageManager
{
	public interface ISaveLoad
	{
		void Save(CommitType saveType);

		// Get currently selected storage
		Storage GetStorage();
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
			Load();
		}

		public void Dispose()
		{
			//Save();
		}

		public void Save(CommitType saveType)
		{
			signalBus?.Fire(new SignalSaveStorage() { storage = activeStorage, saveType = saveType });

			string preferenceName = settings.preferenceName;
			PlayerPrefs.SetString(preferenceName, activeStorage.Database.GetJson());
			PlayerPrefs.Save();

			signalBus?.Fire(new SignalStorageSaved());

			Debug.Log($"[SAVE] Save storage to pref: {preferenceName}");
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

				Debug.Log($"[SAVE] Create new save");

				//Save();
			}

			signalBus?.Fire(new SignalStorageLoaded() { storage = activeStorage });

			Debug.Log($"[LOAD] Load storage from pref: {settings.preferenceName}");
		}

		public Storage GetStorage() => activeStorage;

		[System.Serializable]
		public class Settings
		{
			public string preferenceName = "save_data";
			public string storageDisplayName = "Profile";
			public string storageFileName = "Profile.dat";
		}
	}
}