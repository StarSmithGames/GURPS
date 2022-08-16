using Game.Entities;

using System.Collections.Generic;

namespace Game.Managers.StorageManager
{
	public interface IStorageData<T>
	{
		T GetData();
		void SetData(T data);
	}
	public class StorageData<T> : IStorageData<T>
	{
		private Database database;
		private string key;
		private T defaultValue;

		public StorageData(Database database, string key, T defaultValue = default)
		{
			this.database = database;
			this.key = key;
			this.defaultValue = defaultValue;
		}

		public string GetDataKey()
		{
			return key + "_key";
		}
		public T GetData()
		{
			return database.Get<T>(GetDataKey(), defaultValue);
		}
		public void SetData(T data)
		{
			database.Set(GetDataKey(), data);
		}
	}

	public class Storage
	{
		public Database Database => database;
		private Database database;

		public IStorageData<bool> IsFirstTime { get; private set; }
		public IStorageData<Profile> CurrentProfile { get; private set; }
		public IStorageData<List<Profile>> Profilers { get; private set; }

		public Storage(DefaultData data)
		{
			database = new Database();

			Initialization();

			//Set storage to default
			//CommonDictionaryData.SetData(new CommonDictionaryData());
			//ReaderData_Settings.SetData(data.readerDefaultSettings.GetData());
		}

		public Storage(string json)
		{
			database = new Database();
			Database.LoadJson(json);

			Initialization();
		}

		public void Clear()
		{
			Database.Drop();
			Initialization();
		}

		private void Initialization()
		{
			IsFirstTime = new StorageData<bool>(database, "is_first_time", false);
			CurrentProfile = new StorageData<Profile>(database, "profile_current");
			Profilers = new StorageData<List<Profile>>(database, "profilers");
		}

		[System.Serializable]
		public class Reference
		{
			public string displayName;
			public string fileName;
		}
	}

	public static class FastStorage
	{
		public static DefaultTransform LastTransformOnMap;

		public static void Clear()
		{
			LastTransformOnMap = null;
		}
	}

	[System.Serializable]
	public class DefaultData
	{
		//public ReaderData.Settings readerDefaultSettings;
	}
}