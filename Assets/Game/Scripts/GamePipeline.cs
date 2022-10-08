using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Systems.SpawnManager;
using Game.UI;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using System.Collections;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Managers.GameManager
{
	public class GamePipeline : IInitializable
	{
		private SignalBus signalBus;
		private AsyncManager asyncManager;
		private ISaveLoad saveLoad;
		private SceneManager.SceneManager sceneManager;
		private CharacterManager.CharacterManager characterManager;
		private PartyManager.PartyManager partyManager;
		private GameManager gameManager;
		private SpawnManager spawnManager;

		public GamePipeline(SignalBus signalBus,
			AsyncManager asyncManager,
			ISaveLoad saveLoad,
			SceneManager.SceneManager sceneManager,
			CharacterManager.CharacterManager characterManager,
			PartyManager.PartyManager partyManager,
			GameManager gameManager,
			SpawnManager spawnManager)
		{
			this.signalBus = signalBus;
			this.asyncManager = asyncManager;
			this.saveLoad = saveLoad;
			this.sceneManager = sceneManager;
			this.characterManager = characterManager;
			this.partyManager = partyManager;
			this.gameManager = gameManager;
			this.spawnManager = spawnManager;
		}

		public void Initialize()
		{
			signalBus?.Subscribe<SignalSceneChanged>(OnSceneChanged);

			asyncManager.StartCoroutine(Pipeline());
		}

		private IEnumerator Pipeline()
		{
			Debug.Log("[GamePipeline] Pipeline Start");
#if UNITY_EDITOR
			//Profile == null ? generate player
			if (!saveLoad.GetStorage().IsHasProfile)
			{
				characterManager.CreatePlayer();
				partyManager.CreatePlayerParty();
			}

			gameManager.ChangeLocation(sceneManager.GetSceneLocation());

			if (gameManager.CurrentGameLocation == GameLocation.Location)
			{
				yield return SpawnProcess();
			}
#endif
			gameManager.ChangeState(GameState.Gameplay);

			Debug.Log("[GamePipeline] Pipeline End");

			yield return null;
		}

		private IEnumerator SpawnProcess()
		{
			Assert.IsTrue(spawnManager.registers.Count == 0, "No spawn points on scene.");

			yield return null;//take all spawn points need tests
			yield return new WaitUntil(() => spawnManager.registers.Count > 0);
			spawnManager.Spawn();
			yield return new WaitWhile(() => spawnManager.IsSpawnProcess);
		}


		private void OnSceneChanged(SignalSceneChanged signal)
		{
			if (signal.data.gameLocation == GameLocation.Map)
			{
				gameManager.ChangeLocation(GameLocation.Map);
			}
			else if (signal.data.gameLocation == GameLocation.Location)
			{
				gameManager.ChangeLocation(GameLocation.Location);
			}
		}
	}
}