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

		[Inject]
		private void Construct(CharacterController3D controller, [Inject(Id = "CameraPivot")] Transform cameraPivot)
		{
			Controller = controller;
			CameraPivot = cameraPivot;
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
	}
}