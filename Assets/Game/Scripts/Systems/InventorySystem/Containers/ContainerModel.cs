using EPOOutline;

using System.Collections;
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

		[SerializeField] private Settings settings;

		public IInventory Inventory 
		{
			get
			{
				if(inventory == null)
				{
					inventory = new Inventory(ContainerData.inventory);
					//TODO Load from data
				}
				
				return inventory;
			}
		}
		private IInventory inventory;

		public bool IsOpened => containerWindow?.IsShowing ?? false;
		public bool IsSearched => data.isSearched;
		public bool IsInteractable => currentInteractor == null;

		private UIContainerWindow containerWindow = null;
		private IEntity currentInteractor = null;

		private Data data;

		private UIManager uiManager;
		private InventoryContainerHandler containerHandler;

		[Inject]
		private void Construct(UIManager uiManager, InventoryContainerHandler containerHandler)
		{
			this.uiManager = uiManager;
			this.containerHandler = containerHandler;
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
		}


		public void Interact() { }
		public void InteractFrom(IEntity entity)
		{
			if (currentInteractor != null)
			{
				return;
			}

			currentInteractor = entity;
			StartCoroutine(Interaction());
		}


		public void StartObserve()
		{
			if(currentInteractor == null)
			{
				outline.enabled = true;
			}
		}
		public void Observe() { }
		public void EndObserve()
		{
			if (currentInteractor == null)
			{
				outline.enabled = false;
			}
		}

		private void OpenWindow()
		{
			containerWindow?.Hide();

			containerWindow = containerHandler.SpawnContainerWindow(Inventory);
			containerWindow.onTakeAll += OnTakeAll;
			containerWindow.ShowPopup();
		}

		private void CloseWindow()
		{
			if(containerWindow != null)
			{
				containerWindow.onTakeAll -= OnTakeAll;
				containerWindow.HidePopup();
			}
			containerWindow = null;
		}


		private IEnumerator Interaction()
		{
			if(currentInteractor == null) yield break;

			outline.enabled = false;

			if (!IsInteractorInRange())
			{
				currentInteractor.Controller.SetDestination(transform.position, settings.maxRange - 0.1f);

				yield return new WaitWhile(() => !currentInteractor.Controller.IsReachedDestination());
			}

			if (IsInteractorInRange())
			{
				OpenWindow();
			}

			while (IsOpened)
			{
				if(!IsInteractorInRange())
				{
					CloseWindow();
				}
				yield return null;
			}

			currentInteractor = null;
		}

		private bool IsInteractorInRange()
		{
			if (currentInteractor == null) return false;
			return Vector3.Distance(transform.position, currentInteractor.Transform.position) <= settings.maxRange;
		}


		private void OnTakeAll()
		{
			containerHandler.CharacterTakeAllFrom(Inventory);
		}


		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, settings.maxRange);
		}

		[System.Serializable]
		public class Settings
		{
			public float maxRange = 3f;
		}

		public class Data
		{
			public bool isSearched = false;
		}
	}
}