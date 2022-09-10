using EPOOutline;

using Game.Managers.StorageManager;
using Game.Systems;
using Game.Systems.InteractionSystem;

using System.Collections;

using Zenject;

namespace Game.Entities.Models
{
	public class PlayerRTSModel : EntityModel, IObservable, IInteractable
	{
		private Outlinable outline;
		private ISaveLoad saveLoad;

		[Inject]
		private void Construct(SignalBus signalBus, Outlinable outline, ISaveLoad saveLoad)
		{
			this.signalBus = signalBus;
			this.outline = outline;
			this.saveLoad = saveLoad;
		}

		protected override IEnumerator Start()
		{
			yield return null;

			LoadTransformOnMap();

			outline.enabled = false;
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

		private void LoadTransformOnMap()
		{
			var map = FastStorage.LastTransformOnMap;

			if (map != null)
			{
				transform.position = map.position;
				transform.rotation = map.rotation;
				transform.localScale = map.scale;
			}
		}
	}
}