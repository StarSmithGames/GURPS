
using EPOOutline;

using Game.Entities;
using Game.Systems.InventorySystem;

using UnityEngine;
using UnityEngine.Assertions;

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

	public Markers Markers { get; private set; }
	public Outlinable Outlines { get; private set; }

	public Transform CameraPivot { get; private set; }


	protected SignalBus signalBus;

	[Inject]
	private void Construct(
		SignalBus signalBus,
		NavigationController navigationController,
		CharacterController3D controller,
		Markers markerController,
		Outlinable outline,
		[Inject(Id = "CameraPivot")] Transform cameraPivot)
	{
		this.signalBus = signalBus;

		Navigation = navigationController;
		Controller = controller;
		Markers = markerController;
		Outlines = outline;
		CameraPivot = cameraPivot;

		Validate();
	}

	protected virtual void Start()
	{
		Outlines.enabled = false;
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
		Outlines.enabled = true;
	}

	public virtual void Observe() { }

	public virtual void EndObserve()
	{
		Outlines.enabled = false;
	}


	private void Validate()
	{
		Assert.IsNotNull(Navigation,		$"Entity {gameObject.name} lost component.");
		Assert.IsNotNull(Controller,		$"Entity {gameObject.name} lost component.");
		Assert.IsNotNull(Markers,	$"Entity {gameObject.name} lost component.");
		Assert.IsNotNull(Outlines,			$"Entity {gameObject.name} lost component.");
		Assert.IsNotNull(CameraPivot,		$"Entity {gameObject.name} lost component.");
	}
}