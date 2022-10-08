using Game.Entities.Models;
using Game.Managers.InputManager;
using Game.Systems.CombatDamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.CanvasSystem;

using UnityEngine;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : DamageableModel, IContainer, IDestructible
	{
		public bool IsOpened => window?.IsShowing ?? false;
		public bool IsLocked => data.isLocked;
		public bool IsSearched => data.isSearched;

		[field: SerializeField] public ContainerData ContainerData { get; private set; }
		public override InteractionPoint BattlePoint => InteractionPoint;

		public override ISheet Sheet
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
		private ISheet containerSheet;

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
		private FloatingTextSystem.FloatingSystem floatingSystem;

		[Inject]
		private void Construct(UISubCanvas subCanvas,
			UIContainerWindow.Factory containerWindowFactory,
			InputManager inputManager, FloatingTextSystem.FloatingSystem floatingSystem)
		{
			this.subCanvas = subCanvas;
			this.containerWindowFactory = containerWindowFactory;
			this.inputManager = inputManager;
			this.floatingSystem = floatingSystem;
		}

		private void Start()
		{
			if (data == null)
			{
				data = new Data() { isLocked = this.ContainerData.isLocked };
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


		public override void Die()
		{
			Destruct();
		}

		public void Destruct()
		{
			gameObject.SetActive(false);
		}

		#region Open Close Dispose
		public void Open(IInteractable interactor)
		{
			if (window == null)
			{
				if (IsLocked)
				{
					var pos = transform.TransformPoint(DamagePosition);
					floatingSystem.CreateText(pos, "Locked");
					return;
				}

				lastInteractor = interactor;

				window = containerWindowFactory.Create();

				window.transform.SetParent(subCanvas.Windows);
				(window.transform as RectTransform).anchoredPosition = Vector2.zero;

				window.Inventory.SetInventory(Sheet.Inventory);
				window.onClose += Dispose;
				window.onTakeAll += OnTakeAll;
				window.ShowPopup();
			}
			else
			{
				Close();
			}
		}

		public void Close()
		{
			window?.HidePopup();
			if(window != null)
			{
				Dispose();
			}
		}

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
		#endregion

		private void UnLock()
		{

		}

		private void OnTakeAll()
		{
			Dispose();
			//containerHandler.CharacterTakeAllFrom(Sheet.Inventory);
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.TransformPoint(DamagePosition), 0.1f);
		}

		public class Data
		{
			public bool isLocked = false;
			public bool isSearched = false;
		}
	}
}