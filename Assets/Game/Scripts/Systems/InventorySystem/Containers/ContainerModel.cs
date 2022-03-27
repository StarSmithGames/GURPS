using EPOOutline;

using Game.Systems.InteractionSystem;

using Sirenix.OdinInspector;

using System.Collections;
using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : InteractableModel, IObservable
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

		private UIContainerWindow containerWindow = null;

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

		#region Observe
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
		#endregion

		#region Interaction
		public override void InteractFrom(IEntity entity)
		{
			base.InteractFrom(entity);
			StartCoroutine(Interaction());
		}
		private IEnumerator Interaction()
		{
			outline.enabled = false;

			if (!IsInteractorInRange())
			{
				currentInteractor.Controller.SetDestination(InteractPosition);

				yield return new WaitWhile(() => !currentInteractor.Navigation.IsReachedDestination());
			}

			if (IsInteractorInRange())
			{
				OpenWindow();
			}

			while (IsOpened)
			{
				if (!IsInteractorInRange())
				{
					CloseWindow();
				}
				yield return null;
			}

			currentInteractor = null;
		}
		#endregion

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

		private void OnTakeAll()
		{
			containerHandler.CharacterTakeAllFrom(Inventory);
		}


		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(InteractPosition, settings.maxRange);
			Gizmos.DrawSphere(InteractPosition, 0.1f);
		}

		public class Data
		{
			public bool isSearched = false;
		}
	}
}