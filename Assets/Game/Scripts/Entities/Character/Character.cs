using Game.Managers.GameManager;
using Game.Systems.InventorySystem;
using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class Character : MonoBehaviour, IEntity
	{
		public CharacterData CharacterData => characterData;
		[SerializeField] private CharacterData characterData;

		public IInventory Inventory
		{
			get
			{
				if(inventory == null)
				{
					inventory = new Inventory(characterData.inventory);
					//TODO Load data
				}

				return inventory;
			}
		}
		private IInventory inventory;

		public Transform Transform => transform;
		public CharacterController3D Controller { get; private set; }
		public Transform CameraPivot { get; private set; }

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus, CharacterController3D controller, [Inject(Id = "CameraPivot")] Transform cameraPivot)
		{
			this.signalBus = signalBus;

			Controller = controller;
			CameraPivot = cameraPivot;

			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		public void Freeze()
		{
			Controller.Freeze();
		}

		public void UnFreeze()
		{
			Controller.UnFreeze();
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



		private void OnGameStateChanged(SignalGameStateChanged signal)
		{

		}
	}
}