using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


using Zenject;

namespace Game.Managers.SceneManager
{
	public class SceneManager : ITickable, IInitializable, IDisposable
	{
		public IProgressHandle ProgressHandle { get; private set; }

		private string currentScene;

		private Dictionary<Scenes, string> scenes = new Dictionary<Scenes, string>()
		{
			{ Scenes.Menu,		"Menu" },
			{ Scenes.Map,		"MapRTS" },
			{ Scenes.Polygon,	"Polygon" }
		};

		private AsyncManager asyncManager;

		public SceneManager(AsyncManager asyncManager)
		{
			this.asyncManager = asyncManager;
		}

		public void Initialize()
		{
			currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void Dispose()
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		public void Tick()
		{

		}

		public void SwitchScene(Scenes scene, UnityAction callback = null)
		{
			scenes.TryGetValue(scene, out string name);
			SwitchScene(name, callback);
		}

		public void SwitchScene(string sceneName, UnityAction callback = null)
		{
			SetCurrentSceneAsync(sceneName, callback);
		}

		//LoadFromEditor
		//LoadFromBuild
		//LoadFromAddresables
		public void SetCurrentSceneAsync(string sceneName, UnityAction callback = null)
		{
			//if (Application.isEditor)
			//{
			//	asyncManager.StartCoroutine(LoadFromEditor(sceneName));
			//}
			//else
			{
				asyncManager.StartCoroutine(LoadFromBuild(sceneName, callback));
			}
		}

        private IEnumerator LoadFromEditor(string sceneName, UnityAction callback = null)
        {
#if UNITY_EDITOR

            BuildProgressHandle handle = new BuildProgressHandle();
            ProgressHandle = handle;
            currentScene = sceneName;

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

        private IEnumerator LoadFromBuild(string sceneName, UnityAction callback = null)
		{
			currentScene = sceneName;
			BuildProgressHandle handle = new BuildProgressHandle();
			ProgressHandle = handle;

			handle.AsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Single);
			handle.AsyncOperation.allowSceneActivation = true;

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
		}


		public AsyncOperation UnloadCurrentScene()
		{
			if (currentScene != null)
			{
				if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(currentScene).isLoaded)
				{
					var scene = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
					currentScene = null;
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

	public interface IProgressHandle
	{
		float GetProgressPercent();
	}
	public class BuildProgressHandle : IProgressHandle
	{
		public AsyncOperation AsyncOperation = null;

		public float GetProgressPercent()
		{
			return AsyncOperation?.progress ?? 0f;
		}
	}
}