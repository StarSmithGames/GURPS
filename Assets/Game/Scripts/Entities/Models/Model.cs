using EPOOutline;

using Game.Systems;
using Game.Systems.CombatDamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using UnityEngine;

using Zenject;

namespace Game.Entities.Models
{
	public abstract class Model : MonoBehaviour, IInteractable, IObservable
	{
		public bool IsInteractable { get => isInteractable; protected set => isInteractable = value; }
		protected bool isInteractable = true;

		public virtual IInteraction Interaction { get; protected set; }
		public Transform Transform => transform;

		public Outlinable Outline { get; protected set; }
		public InteractionPoint InteractionPoint { get; protected set; }

		[Inject]
		private void Construct(Outlinable outline, InteractionPoint interactionPoint)
		{
			Outline = outline;
			InteractionPoint = interactionPoint;

			Outline.enabled = false;
		}

		public virtual bool InteractWith(IInteractable interactable)
		{
			if (interactable.IsInteractable)
			{
				if (interactable.Interaction != null)
				{
					interactable.Interaction.Execute(this);

					return true;
				}
			}

			return false;
		}

		#region Observe
		public virtual void StartObserve()
		{
			if (IsInteractable)
			{
				Outline.enabled = true;
			}
		}

		public virtual void Observe() { }

		public virtual void EndObserve()
		{
			if (IsInteractable)
			{
				Outline.enabled = false;
			}
		}
		#endregion
	}

	public abstract class CombatModel : Model, ISheetable, IDamageable, IDieable
	{
		[field: SerializeField] public Vector3 DamagePosition { get; protected set; }
		public virtual InteractionPoint BattlePoint { get; protected set; }
		public virtual ISheet Sheet { get; }

		protected CombatDamageSystem combatDamageSystem;

		[Inject]
		private void Construct(CombatDamageSystem combatDamageSystem)
		{
			this.combatDamageSystem = combatDamageSystem;
		}

		public void ApplyDamage<T>(T value)
		{
			if (value is Damage damage)
			{
				//combatDamageSystem.DealDamage(damage, this);
			}
		}

		public virtual Damage GetDamage()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Die()
		{
			DestroyImmediate(gameObject);
		}
	}
}