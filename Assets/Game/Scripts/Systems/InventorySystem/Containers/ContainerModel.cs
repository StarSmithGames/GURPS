using Game.Entities;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using System.Collections;
using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : InteractableModel, ISheetable
	{
		[field: SerializeField] public ContainerData ContainerData { get; private set; }

		public ISheet Sheet
		{
			get
			{
				if(containerSheet == null)
				{
					containerSheet = new ContainerSheet(ContainerData);
				}

				return containerSheet;
			}
		}
		private ContainerSheet containerSheet;

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

		private void Start()
		{
			if (data == null)
			{
				data = new Data();
			}
		}

		#region Observe
		public override void StartObserve()
		{
			base.StartObserve();

			uiManager.Battle.SetSheet(Sheet);
		}
		public override void EndObserve()
		{
			base.EndObserve();

			uiManager.Battle.SetSheet(null);
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
				currentInteractor.Controller.SetDestination(GetIteractionPosition(currentInteractor));

				yield return new WaitWhile(() => !currentInteractor.Navigation.NavMeshAgent.IsReachedDestination());
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

			containerWindow = containerHandler.SpawnContainerWindow(Sheet.Inventory);
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
			containerHandler.CharacterTakeAllFrom(Sheet.Inventory);
		}


		public class Data
		{
			public bool isSearched = false;
		}
	}
}