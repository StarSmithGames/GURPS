using DG.Tweening.Core.Easing;

using Game.Managers.SceneManager;

using System;

using UnityEngine;
using Zenject;

namespace Game.Managers.GameManager
{
    public class GameManager : IDisposable
    {
        public GameState CurrentGameState { get; private set; }
        public GameState PreviousGameState { get; private set; }

        public GameLocation CurrentGameLocation { get; private set; }

		private SignalBus signalBus;
        private SceneManager.SceneManager sceneManager;

		public GameManager(SignalBus signalBus, SceneManager.SceneManager sceneManager)
		{
            this.signalBus = signalBus;
            this.sceneManager = sceneManager;

			signalBus?.Subscribe<SignalSceneChanged>(OnSceneChanged);//required editor(scene Game)
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalSceneChanged>(OnSceneChanged);
		}

		public void ChangeState(GameState gameState)
        {
            if (CurrentGameState != gameState)
            {
                PreviousGameState = CurrentGameState;
                CurrentGameState = gameState;

                signalBus?.Fire(new SignalGameStateChanged
                {
                    newGameState = CurrentGameState,
                    oldGameState = PreviousGameState
                });
            }
            else
            {
                Debug.LogError($"Try to set state: {gameState}, but it's already setted.");
            }
        }

        public void ChangeLocation(GameLocation gameLocation)
        {
            CurrentGameLocation = gameLocation;

            signalBus.Fire(new SignalGameLocationChanged
            {
                newGameLocation = CurrentGameLocation,
            });
        }

		private void OnSceneChanged(SignalSceneChanged signal)
		{
            if(signal.data == null)
            {
                ChangeLocation(GameLocation.None);
            }
            else if (signal.data.gameLocation == GameLocation.Map)
            {
				ChangeLocation(GameLocation.Map);
			}
			else if (signal.data.gameLocation == GameLocation.Location)
            {
				ChangeLocation(GameLocation.Location);
			}
		}
	}
    public enum GameState
    {
        Empty,
        Menu,
        Loading,
        Gameplay,
    }

    public enum GameLocation
	{
        None,
        Map,
        Location,
	}
}