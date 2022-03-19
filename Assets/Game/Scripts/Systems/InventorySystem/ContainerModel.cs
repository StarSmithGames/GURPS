using EPOOutline;

using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : MonoBehaviour, IInteractable, IObservable
	{
		[SerializeField] private Collider collider;
		[SerializeField] private Outlinable outline;

		public ContainerData ContainerData => containerData;
		[SerializeField] private ContainerData containerData;

		public IInventory Inventory { get; private set; }
		public bool IsSearched => data.isSearched;

		private Data data;

		private UIManager uiManager;

		private void Construct(UIManager uiManager)
		{
			this.uiManager = uiManager;
		}

		private void Awake()
		{
			outline.enabled = false;
		}

		private void Start()
		{
			if (data == null)
			{
				data = new Data();
			}

			Inventory = new Inventory(ContainerData.inventory);
		}

		public void Interact()
		{
			collider.enabled = false;
			outline.enabled = false;

			//uiManager.Show<UIChest>();
		}


		public void StartObserve()
		{
			outline.enabled = true;
		}
		public void Observe()
		{
		}
		public void EndObserve()
		{
			outline.enabled = false;
		}

		public class Data
		{
			public bool isSearched = false;
		}
	}
}