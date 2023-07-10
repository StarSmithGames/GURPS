using DG.Tweening.Core.Easing;

using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Systems.SpawnManager;
using System.Collections;
using System.Reflection;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Managers.GameManager
{
	public class GamePipeline
	{
		private SignalBus signalBus;
		private AsyncManager asyncManager;
		private CharacterManager.CharacterManager characterManager;
		private PartyManager.PartyManager partyManager;
		private GameManager gameManager;
		private SpawnManager spawnManager;

		public GamePipeline(SignalBus signalBus,
			AsyncManager asyncManager,
			CharacterManager.CharacterManager characterManager,
			PartyManager.PartyManager partyManager,
			GameManager gameManager,
			SpawnManager spawnManager)
		{
			this.signalBus = signalBus;
			this.asyncManager = asyncManager;
			this.characterManager = characterManager;
			this.partyManager = partyManager;
			this.gameManager = gameManager;
			this.spawnManager = spawnManager;

			signalBus?.Subscribe<SignalGameLocationChanged>(OnGameLocationChanged);//required editor
		}

		private IEnumerator Pipeline()
		{
			Debug.Log("[GamePipeline] Pipeline Start");
//#if UNITY_EDITOR
			//Profile == null ? generate player
			//if (!saveLoad.GetStorage().IsHasProfile)
			{
				characterManager.CreatePlayer();
				partyManager.CreatePlayerParty();
			}

			if (gameManager.CurrentGameLocation == GameLocation.Location)
			{
				yield return SpawnProcess();
			}
//#endif
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

		private void OnGameLocationChanged(SignalGameLocationChanged signal)
		{
			if (signal.newGameLocation == GameLocation.Location)
			{
				asyncManager.StartCoroutine(Pipeline());
			}
		}
	}
}