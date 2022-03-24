using Game.Systems.InventorySystem;
using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class Character : Entity
	{
		[Inject]
		private void Construct()
		{
			//signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		private void OnDestroy()
		{
			//signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		public void InteractWith(IObservable observable)
		{
			switch (observable)
			{
				case IInteractable interactable:
				{
					interactable.InteractFrom(this);
					break;
				}
			}
		}
	}
}