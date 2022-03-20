using EPOOutline;

using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

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
		private UIChestWindow.Factory factory;

		[Inject]
		private void Construct(UIManager uiManager, UIChestWindow.Factory factory)
		{
			this.uiManager = uiManager;
			this.factory = factory;
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
		}


		public void StartObserve()
		{
			outline.enabled = true;

			var window = factory.Create();
			window.transform.parent = uiManager.WindowsRoot;

			(window.transform as RectTransform).anchoredPosition = Vector3.zero;
			window.transform.localScale = Vector3.one;
			window.transform.rotation = Quaternion.Euler(Vector3.zero);
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