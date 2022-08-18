using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;
using Zenject.Asteroids;

namespace Game.Managers.GameManager
{
    public class GameManager : IInitializable
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
        }

        public void Initialize()
        {
            ChangeLocation(sceneManager.GetSceneLocation());
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
            if (CurrentGameLocation != gameLocation)
            {
                CurrentGameLocation = gameLocation;

                signalBus.Fire(new SignalGameLocationChanged
                {
                    newGameLocation = CurrentGameLocation,
                });
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

    public enum GameplayType
	{
        //battle
	}
}