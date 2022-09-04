using Game.Entities.Models;
using Game.Managers.InputManager;
using Game.Systems.CombatDamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;
using Game.UI;

using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : Model, IContainer, ISheetable, IDamageable, IDestructible
	{
		public bool IsOpened => window?.IsShowing ?? false;
		public bool IsSearched => data.isSearched;

		[field: SerializeField] public ContainerData ContainerData { get; private set; }
		[field: SerializeField] public Vector3 DamagePosition { get; private set; }
		public InteractionPoint BattlePoint => InteractionPoint;
		public InteractionPoint OpportunityPoint => null;

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
		private CombatDamageSystem.CombatDamageSystem combatDamageSystem;

		[Inject]
		private void Construct(UISubCanvas subCanvas,
			UIContainerWindow.Factory containerWindowFactory,
			InputManager inputManager,
			CombatDamageSystem.CombatDamageSystem combatDamageSystem)
		{
			this.subCanvas = subCanvas;
			this.containerWindowFactory = containerWindowFactory;
			this.inputManager = inputManager;
			this.combatDamageSystem = combatDamageSystem;
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

		#region Open Close Dispose
		public void Open(IInteractable interactor)
		{
			if (window == null)
			{
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

		private void OnTakeAll()
		{
			Dispose();
			//containerHandler.CharacterTakeAllFrom(Sheet.Inventory);
		}

		public void ApplyDamage<T>(T value)
		{
			if (value is Damage damage)
			{
				combatDamageSystem.DealDamage(damage, this);
			}
		}

		public Damage GetDamage()
		{
			return null;//container can't deal damage
		}

		public void Destruct()
		{
			DestroyImmediate(gameObject);
		}

		private void OnDrawGizmos()
		{
			Gizmos.DrawSphere(transform.TransformPoint(DamagePosition), 0.1f);
		}

		public class Data
		{
			public bool isSearched = false;
		}
	}
}