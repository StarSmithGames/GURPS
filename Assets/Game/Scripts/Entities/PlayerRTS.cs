using EPOOutline;

using Game.Systems.InteractionSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Entities
{
	public class PlayerRTS : Entity, IObservable, IInteractable
	{
		private Outlinable outline;

		public bool IsInteractable { get; }
		public IInteraction Interaction { get; }

		[Inject]
		private void Construct(Outlinable outline)
		{
			this.outline = outline;
		}

		protected override IEnumerator Start()
		{
			outline.enabled = false;

			return base.Start();
		}

		#region IObservable
		public void StartObserve()
		{
			outline.enabled = true;
		}
		public void Observe() { }
		public void EndObserve()
		{
			outline.enabled = false;
		}
		#endregion

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
	}
}