using Game.Entities.Models;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Systems.InteractionSystem;
using Sirenix.OdinInspector;

using Zenject;

namespace Game.Map
{
	public interface IWayPoint : IObservable, IInteractable
	{
		void Action(IInteractable interactable);
	}

	public class WayPointTransition : Model, IWayPoint
	{
		public override IInteraction Interaction
		{
			get
			{
				if (interaction == null)
				{
					interaction = new BaseInteraction(InteractionPoint, Action);
				}

				return interaction;
			}
		}
		private IInteraction interaction = null;

		[HideLabel]
		public SceneName sceneName;

		private LoadingController loadingController;

		[Inject]
		private void Construct(LoadingController loadingController)
		{
			this.loadingController = loadingController;
		}

		private void Start()
		{
			IsInteractable = true;
		}

		public void Action(IInteractable interactable)
		{
			IsInteractable = false;
			loadingController.LoadScene(sceneName);
		}

		public override bool InteractWith(IInteractable interactable)
		{
			return false;//не может не с кем взаимодествовать
		}
	}
}