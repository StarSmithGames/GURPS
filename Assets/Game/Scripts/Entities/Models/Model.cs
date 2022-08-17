using EPOOutline;

using Game.Systems.InteractionSystem;
using UnityEngine;

using Zenject;

namespace Game.Entities.Models
{
	public class Model : MonoBehaviour, IInteractable, IObservable
	{
		public bool IsInteractable => true;
		public virtual IInteraction Interaction { get; protected set; }
		public Transform Transform => transform;

		public Outlinable Outline { get; protected set; }

		[Inject]
		private void Construct(Outlinable outline)
		{
			Outline = outline;

			Outline.enabled = false;
		}

		public bool InteractWith(IInteractable interactable)
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
			Outline.enabled = true;
		}

		public virtual void Observe() { }

		public virtual void EndObserve()
		{
			Outline.enabled = false;
		}
		#endregion
	}
}