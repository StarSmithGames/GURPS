using Game.Entities;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using System.Collections;
using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : InteractableModel, IContainer, ISheetable/*, IDamegeable*/
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

		#region OpenClose
		public void Open()
		{
			containerWindow?.Hide();

			containerWindow = containerHandler.SpawnContainerWindow(Sheet.Inventory);
			containerWindow.onTakeAll += OnTakeAll;
			containerWindow.ShowPopup();
		}
		public void Close()
		{
			if(containerWindow != null)
			{
				containerWindow.onTakeAll -= OnTakeAll;
				containerWindow.HidePopup();
			}
			containerWindow = null;
		}
		#endregion

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