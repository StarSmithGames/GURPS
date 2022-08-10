using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;


using Zenject;

namespace Game.Managers.SceneManager
{
	public class SceneManager : ITickable, IInitializable, IDisposable
	{
		private string currentScene;
		private IProgressHandle progressHandle;

		private Dictionary<Scenes, string> scenes = new Dictionary<Scenes, string>()
		{
			{ Scenes.Menu, "Menu" },
			{ Scenes.Map, "MapRTS" },
			{ Scenes.Polygon, "Polygon" }
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

		public void SwitchScene(Scenes scene)
		{
			scenes.TryGetValue(scene, out string name);
			SwitchScene(name);
		}

		public void SwitchScene(string sceneName)
		{
			SetCurrentSceneAsync(sceneName);
		}

		//LoadFromEditor
		//LoadFromBuild
		//LoadFromAddresables
		public void SetCurrentSceneAsync(string sceneName)
		{
			//if (Application.isEditor)
			//{
			//	asyncManager.StartCoroutine(LoadFromEditor(sceneName));
			//}
			//else
			{
				asyncManager.StartCoroutine(LoadFromBuild(sceneName));
			}
		}

        private IEnumerator LoadFromEditor(string sceneName)
        {
#if UNITY_EDITOR

            BuildProgressHandle handle = new BuildProgressHandle();
            progressHandle = handle;
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
			}
            else
            {
				Debug.LogError("REJECT Scene no loaded");
			}
#else
            throw new NotImplementedException("Not implemented LoadFromEditor");
#endif
        }

        private IEnumerator LoadFromBuild(string sceneName)
		{
			currentScene = sceneName;
			BuildProgressHandle handle = new BuildProgressHandle();
			progressHandle = handle;

			handle.AsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Single);

			yield return handle.AsyncOperation;

			if (handle.AsyncOperation.isDone)
			{
				Debug.LogError("Scene loaded");
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