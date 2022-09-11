using EPOOutline;

using Game.Entities;
using Game.Entities.Models;
using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Managers.TransitionManager;
using Game.Systems;
using Game.Systems.InteractionSystem;
using Game.UI;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Map
{
	public interface IWayPoint : IObservable, IInteractable
	{
		void Action();
	}

	public class WayPointTransition : Model, IWayPoint
	{
		//public override IInteraction Interaction
		//{
		//	get
		//	{
		//		if(interaction == null)
		//		{
		//			interaction = new GoToPointAction(InteractionPoint, Action);
		//		}

		//		return interaction;
		//	}
		//}
		//private IInteraction interaction = null;

		[HideLabel]
		public SceneName sceneName;

		[field: SerializeField] public Transitions In { get; private set; }
		[field: SerializeField] public Transitions Out { get; private set; }

		private UIGlobalCanvas globalCanvas;
		private GameManager gameManager;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas, GameManager gameManager)
		{
			this.globalCanvas = globalCanvas;
			this.gameManager = gameManager;
		}

		private void Start()
		{
			IsInteractable = true;
		}

		public override bool InteractWith(IInteractable interactable)
		{
			return false;//не может не с кем взаимодествовать
		}

		public void Action()
		{
			IsInteractable = false;

			//save map location before load scene
			if (gameManager.CurrentGameLocation == GameLocation.Map)
			{
				//var playerTransform = player.Transform;
				//FastStorage.LastTransformOnMap = new DefaultTransform()
				//{
				//	position = playerTransform.position,
				//	rotation = playerTransform.rotation,
				//	scale = playerTransform.localScale,
				//};
			}

			globalCanvas.WindowsRegistrator.GetAs<WindowInfinityLoading>().Show(sceneName.GetScene(), In, Out);
		}
	}
}