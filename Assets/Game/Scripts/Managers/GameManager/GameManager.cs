using UnityEngine;
using Zenject;

namespace Game.Managers.GameManager
{
    public class GameManager
    {
        public GameState CurrentGameState { get; private set; }
        public GameState PreviousGameState { get; private set; }

        public GameLocation CurrentGameLocation { get; private set; }

		private SignalBus signalBus;

        public GameManager(SignalBus signalBus)
		{
            this.signalBus = signalBus;
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
}