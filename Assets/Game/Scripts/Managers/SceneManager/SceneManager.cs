using Game.Managers.GameManager;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


using Zenject;

namespace Game.Managers.SceneManager
{
	public class SceneManager : IInitializable, IDisposable
	{
		private const string ScenesPath = "Assets/Scenes";

		public string CurrentScene { get; private set; }
		public IProgressHandle ProgressHandle { get; private set; }

		private List<SceneData> scenes = new List<SceneData>();

		private SignalBus signalBus;
		private AsyncManager asyncManager;
		private ZenjectSceneLoader sceneLoader;

		public SceneManager(SignalBus signalBus, AsyncManager asyncManager,
			ZenjectSceneLoader sceneLoader)
		{
			this.signalBus = signalBus;
			this.asyncManager = asyncManager;
			this.sceneLoader = sceneLoader;
		}

		public void Initialize()
		{
			CurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

			//editor set active scene Game
#if UNITY_EDITOR
			if (CurrentScene == "Game")
			{
				for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
				{
					var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

					if(scene.name != "Game")
					{
						CurrentScene = scene.name;
						UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
						
						break;
					}
				}
			}
#endif

			scenes = Resources.LoadAll<SceneData>(ScenesPath).ToList();

			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void Dispose()
		{
			//editor unload scene Game
#if UNITY_EDITOR
			for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				if(scene.name == "Game")
				{
					UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
				}
			}
#endif
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		public GameLocation GetSceneLocation()
		{
			return scenes.FirstOrDefault((x) => x.sceneName.name == CurrentScene)?.gameLocation ?? GameLocation.None;
		}

		public void SwitchScene(string sceneName, bool allow = true, UnityAction callback = null)
		{
			SetCurrentSceneAsync(sceneName, allow, callback);
		}

		//LoadFromEditor
		//LoadFromBuild
		//LoadFromAddresables
		private void SetCurrentSceneAsync(string sceneName, bool allow = true, UnityAction callback = null)
		{
			//if (Application.isEditor)
			//{
			//	asyncManager.StartCoroutine(LoadFromEditor(sceneName));
			//}
			//else
			{
				asyncManager.StartCoroutine(LoadFromBuild(sceneName, allow, callback));
			}
		}

        private IEnumerator LoadFromEditor(string sceneName, UnityAction callback = null)
        {
#if UNITY_EDITOR

            BuildProgressHandle handle = new BuildProgressHandle();
            ProgressHandle = handle;
            CurrentScene = sceneName;

            var path = $"Assets/Scenes/{sceneName}.unity";

            if (File.Exists(path))
            {
                handle.AsyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters() { loadSceneMode = LoadSceneMode.Single });
            }

            if (handle.AsyncOperation == null)
            {
				handle.AsyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path, new LoadSceneParameters() { loadSceneMode = LoadSceneMode.Single });
            }

            yield return handle.AsyncOperation;

            if (handle.AsyncOperation.isDone)
            {
				Debug.LogError("Scene loaded");
				callback?.Invoke();
			}
            else
            {
				Debug.LogError("REJECT Scene no loaded");
			}
#else
            throw new NotImplementedException("Not implemented LoadFromEditor");
#endif
        }

        private IEnumerator LoadFromBuild(string sceneName, bool allow = true, UnityAction callback = null)
		{
			CurrentScene = sceneName;
			BuildProgressHandle handle = new BuildProgressHandle();
			ProgressHandle = handle;

			handle.AsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(CurrentScene, LoadSceneMode.Single);
			handle.AsyncOperation.allowSceneActivation = allow;

			yield return handle.AsyncOperation;

			if (handle.AsyncOperation.isDone)
			{
				signalBus?.Fire(new SignalSceneChanged() { data = scenes.FirstOrDefault((x) => x.sceneName.name == CurrentScene) });
				callback?.Invoke();
			}
			else
			{
				Debug.LogError("REJECT Scene no loaded");
			}
		}


		public AsyncOperation UnloadCurrentScene()
		{
			if (CurrentScene != null)
			{
				if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(CurrentScene).isLoaded)
				{
					var scene = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(CurrentScene);
					CurrentScene = null;
					return scene;
				}
			}

			return null;
		}


		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			var currentActiveScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
			UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
		}
	}

	public static class SceneStorage
	{
		public static Dictionary<Scenes, string> scenes = new Dictionary<Scenes, string>()
		{
			{ Scenes.Menu,      "Menu" },
			{ Scenes.Map,       "Map" },
			{ Scenes.Polygon,   "Polygon" }
		};

		public static string GetSceneName(Scenes scene)
		{
			scenes.TryGetValue(scene, out string name);
			return name;
		}
	}

	public interface IProgressHandle
	{
		bool IsAllowed { get; }

		float GetProgressPercent();
		void AllowSceneActivation();
	}
	public class BuildProgressHandle : IProgressHandle
	{
		public AsyncOperation AsyncOperation = null;

		public bool IsAllowed => AsyncOperation?.allowSceneActivation ?? true;

		public float GetProgressPercent()
		{
			return AsyncOperation?.progress ?? 0f;
		}

		public void AllowSceneActivation()
		{
			AsyncOperation.allowSceneActivation = true;
		}
	}
}