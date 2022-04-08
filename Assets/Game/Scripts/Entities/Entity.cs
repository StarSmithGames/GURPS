
using CMF;

using EPOOutline;

using Game.Entities;
using Game.Systems.DamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities
{
	public abstract partial class Entity : InteractableModel, IEntity
	{
		public GameObject GameObject => gameObject;

		public virtual ISheet Sheet { get; private set; }

		public Markers Markers { get; private set; }
		public Outlinable Outlines { get; private set; }

		public Transform CameraPivot { get; private set; }

		protected SignalBus signalBus;
		protected UIManager uiManager;

		[Inject]
		private void Construct(
			SignalBus signalBus,
			AnimatorControl animatorControl,
			NavigationController navigationController,
			CharacterController3D controller,
			Markers markerController,
			Outlinable outline,
			[Inject(Id = "CameraPivot")] Transform cameraPivot,
			UIManager uiManager)
		{
			this.signalBus = signalBus;

			AnimatorControl = animatorControl;
			Navigation = navigationController;
			Controller = controller;
			Markers = markerController;
			Outlines = outline;
			CameraPivot = cameraPivot;
			this.uiManager = uiManager;

			Validate();
		}

		protected virtual void OnDestroy()
		{
			UnSubscribeAnimationEvents();

			Sheet.Stats.HitPoints.onStatChanged -= OnHitPointsChanged;
		}

		protected virtual void Start()
		{
			SubscribeAnimationEvents();

			Sheet.Stats.HitPoints.onStatChanged += OnHitPointsChanged;

			Outlines.enabled = false;

			ResetMarkers();
		}

		public virtual void TryInteractWith(IInteractable interactable) { }

		public void Freeze(bool trigger)
		{
			Controller.Freeze(trigger);
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

		protected virtual void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.transform.parent = null;
			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}


		private void OnHitPointsChanged()
		{
			if(Sheet.Stats.HitPoints.CurrentValue == 0)
			{
				if (!Sheet.Conditions.IsContains<Death>())
				{
					Sheet.Conditions.Add(new Death(this));
				}
			}
		}


		private void Validate()
		{
			Assert.IsNotNull(Navigation, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Controller, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Markers, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Outlines, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(CameraPivot, $"Entity {gameObject.name} lost component.");
		}

	}

	/// <summary>
	/// IAnimatable implementation
	/// <summary>
	partial class Entity : InteractableModel
	{
		public AnimatorControl AnimatorControl { get; private set; }
		
		public virtual void Attack(int attackType = 0)
		{
			AnimatorControl.Attack(attackType);
		}
		public void Hit(int type = -1)
		{
			AnimatorControl.Hit(type);
		}

		protected virtual void SubscribeAnimationEvents()
		{
			AnimatorControl.onAttackEvent += OnAttacked;
		}
		protected virtual void UnSubscribeAnimationEvents()
		{
			if (AnimatorControl != null)
			{
				AnimatorControl.onAttackEvent -= OnAttacked;
			}
		}

		/// <summary>
		/// This Entity attacked lastInteractable
		/// </summary>
		protected void OnAttacked()
		{
			var direction = ((lastInteractable as MonoBehaviour).transform.position - transform.position).normalized;
			(lastInteractable as IAnimatable).Hit(Random.Range(0, 2));//animation
			(lastInteractable as IDamegeable).ApplyDamage(GetDamage());
		}
	}

	//IPathfinderable implementation
	partial class Entity
	{
		public event UnityAction onTargetChanged;
		public event UnityAction onDestinationChanged;

		public Transform Transform => transform;

		public bool IsHasTarget => Controller.IsHasTarget;

		public NavigationController Navigation { get; private set; }
		public CharacterController3D Controller { get; private set; }

		public virtual void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			Navigation.SetTarget(point, maxPathDistance: maxPathDistance);
			onTargetChanged?.Invoke();
		}

		public virtual void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			Controller.SetDestination(destination, maxPathDistance: maxPathDistance);
			onDestinationChanged?.Invoke();
		}
	}

	//IDamegeable implementation
	partial class Entity
	{
		public virtual Damage GetDamage()
		{
			return new Damage()
			{
				amount = GetDamageFromTable(Sheet.Stats.Strength.CurrentValue),
				damageType = DamageType.Crushing,
			};
		}

		public virtual void ApplyDamage<T>(T value) where T : struct
		{
			if (value is Damage damage)
			{
				if (damage.IsPhysicalDamage)
				{
					Sheet.Stats.HitPoints.CurrentValue -= damage.amount;
				}
				else if (damage.IsMagicalDamage)
				{

				}
			}
		}

		private float GetDamageFromTable(float strength)
		{
			return Mathf.Max(Random.Range(1, 7) - 2, 0);
		}
	}
}