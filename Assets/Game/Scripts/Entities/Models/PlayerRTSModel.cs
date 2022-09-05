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
		private IPlayer player;
		private Outlinable outline;
		private ISaveLoad saveLoad;

		[Inject]
		private void Construct(SignalBus signalBus, IPlayer player, Outlinable outline, ISaveLoad saveLoad)
		{
			this.signalBus = signalBus;
			this.player = player;
			this.outline = outline;
			this.saveLoad = saveLoad;

		}

		protected override IEnumerator Start()
		{
			player.Registrate(this);

			yield return null;

			LoadTransformOnMap();

			outline.enabled = false;
		}

		protected override void OnDestroy()
		{
			player.UnRegistrate(this);

			base.OnDestroy();
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