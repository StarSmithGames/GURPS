using Game.Entities.Models;
using Game.Managers.InputManager;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;
using Game.UI;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : Model, IContainer, ISheetable/*, IDamegeable*/
	{
		public bool IsOpened => window?.IsShowing ?? false;
		public bool IsSearched => data.isSearched;

		[field: SerializeField] public ContainerData ContainerData { get; private set; }
		[field: SerializeField] public InteractionPoint InteractionPoint { get; private set; }

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

		public override IInteraction Interaction
		{
			get
			{
				if(interaction == null)
				{
					interaction = new ContainerInteraction(this);
				}

				return interaction;
			}
		}
		private IInteraction interaction;

		private Data data;
		private IInteractable lastInteractor;
		private UIContainerWindow window;

		private UISubCanvas subCanvas;
		private UIContainerWindow.Factory containerWindowFactory;
		private InputManager inputManager;

		[Inject]
		private void Construct(UISubCanvas subCanvas, UIContainerWindow.Factory containerWindowFactory, InputManager inputManager)
		{
			this.subCanvas = subCanvas;
			this.containerWindowFactory = containerWindowFactory;
			this.inputManager = inputManager;
		}

		private void Start()
		{
			if (data == null)
			{
				data = new Data();
			}
		}

		private void Update()
		{
			if (IsOpened)
			{
				if (!window.IsInProcess)
				{
					if (inputManager.GetKeyDown(KeyAction.InGameMenu))
					{
						Close();
					}

					if (lastInteractor != null)
					{
						if (!InteractionPoint.IsInRange(lastInteractor.Transform.position))
						{
							Close();
						}
					}
				}
			}
		}

		#region Observe
		public override void StartObserve()
		{
			base.StartObserve();
			//uiManager.Battle.SetSheet(Sheet);
		}

		public override void EndObserve()
		{
			base.EndObserve();
			//uiManager.Battle.SetSheet(null);
		}
		#endregion

		#region OpenClose
		public void Open(IInteractable interactor)
		{
			lastInteractor = interactor;

			Assert.IsNull(window);

			window = containerWindowFactory.Create();

			window.transform.SetParent(subCanvas.Windows);
			(window.transform as RectTransform).anchoredPosition = Vector2.zero;

			window.Inventory.SetInventory(Sheet.Inventory);
			window.onClose += Dispose;
			window.onTakeAll += OnTakeAll;
			window.ShowPopup();
		}

		public void Close()
		{
			window?.HidePopup(Dispose);
		}
		#endregion

		private void Dispose()
		{
			lastInteractor = null;

			if (window != null)
			{
				window.onClose -= Dispose;
				window.onTakeAll -= OnTakeAll;
			}
			window = null;
		}

		private void OnTakeAll()
		{
			Dispose();
			//containerHandler.CharacterTakeAllFrom(Sheet.Inventory);
		}

		public class Data
		{
			public bool isSearched = false;
		}
	}
}