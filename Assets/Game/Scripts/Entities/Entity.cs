
using EPOOutline;

using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;

using Zenject;

public abstract class Entity : MonoBehaviour, IEntity, IObservable
{
	public EntityData EntityData => entityData;
	[SerializeField] protected EntityData entityData;

	public IInventory Inventory
	{
		get
		{
			if (inventory == null)
			{
				inventory = new Inventory(entityData.inventory);
				//TODO Load data
			}

			return inventory;
		}
	}
	private IInventory inventory;

	public Transform Transform => transform;

	public NavigationController Navigation { get; private set; }
	public CharacterController3D Controller { get; private set; }

	public Markers MarkerController { get; private set; }
	public Outlinable Outline { get; private set; }

	public Transform CameraPivot { get; private set; }


	protected SignalBus signalBus;

	[Inject]
	private void Construct(
		SignalBus signalBus,
		NavigationController navigationController,
		CharacterController3D controller,
		Markers markerController,
		Outlinable outline)
	{
		this.signalBus = signalBus;

		Navigation = navigationController;
		Controller = controller;
		MarkerController = markerController;
		Outline = outline;
	}

	protected virtual void Start()
	{
		Outline.enabled = false;
	}


	public void Freeze(bool trigger)
	{
		if (trigger)
		{
			Controller.Freeze();
		}
		else
		{
			Controller.UnFreeze();
		}
	}


	public virtual void StartObserve()
	{
		Outline.enabled = true;
	}

	public virtual void Observe() { }

	public virtual void EndObserve()
	{
		Outline.enabled = false;
	}
}