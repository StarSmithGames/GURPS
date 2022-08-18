using EPOOutline;

using Game.Systems.InteractionSystem;
using UnityEngine;

using Zenject;

namespace Game.Entities.Models
{
	public class Model : MonoBehaviour, IInteractable, IObservable
	{
		public bool IsInteractable { get => isInteractable; protected set => isInteractable = value; }
		protected bool isInteractable = true;

		public virtual IInteraction Interaction { get; protected set; }
		public Transform Transform => transform;

		public Outlinable Outline { get; protected set; }

		[field: SerializeField] public InteractionPoint InteractionPoint { get; private set; }

		[Inject]
		private void Construct(Outlinable outline)
		{
			Outline = outline;

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
}