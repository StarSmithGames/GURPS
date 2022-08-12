using System;

using Zenject;

namespace Game.Managers.SceneManager
{
	public class SceneHandler : IInitializable, IDisposable
	{
		private SignalBus signalBus;
		private GameManager.GameManager gameManager;

		public SceneHandler(SignalBus signalBus, GameManager.GameManager gameManager)
		{
			this.signalBus = signalBus;
			this.gameManager = gameManager;
		}

		public void Initialize()
		{
			signalBus?.Subscribe<SignalSceneChanged>(OnSceneChanged);
		}

		public void Dispose()
		{
			signalBus?.Unsubscribe<SignalSceneChanged>(OnSceneChanged);
		}

		private void OnSceneChanged(SignalSceneChanged signal)
		{
			if(signal.data.gameLocation == GameManager.GameLocation.Map)
			{
				gameManager.ChangeLocation(GameManager.GameLocation.Map);
			}
			else if(signal.data.gameLocation == GameManager.GameLocation.Location)
			{
				gameManager.ChangeLocation(GameManager.GameLocation.Location);
			}
		}
	}
}