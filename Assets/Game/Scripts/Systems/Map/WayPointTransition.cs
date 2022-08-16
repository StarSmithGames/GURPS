using EPOOutline;

using Game.Entities;
using Game.Managers.CharacterManager;
using Game.Managers.SceneManager;
using Game.Managers.StorageManager;
using Game.Managers.TransitionManager;
using Game.Systems.InteractionSystem;
using Game.UI;
using Game.UI.Windows;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Map
{
	public class WayPointTransition : MonoBehaviour, IWayPoint, IObservable, IInteractable
	{
		public bool IsInteractable { get; private set; }
		public IInteraction Interaction
		{
			get
			{
				if(interaction == null)
				{
					interaction = new WayPointInteraction(this);
				}

				return interaction;
			}
		}
		private IInteraction interaction = null;
		public Transform Transform => transform;

		[HideLabel]
		public SceneName sceneName;

		[field: SerializeField] public Transitions In { get; private set; }
		[field: SerializeField] public Transitions Out { get; private set; }
		[field: Space]
		[field: SerializeField] public Outlinable Outline { get; private set; }
		[field: SerializeField] public InteractionPoint InteractionPoint { get; private set; }

		private UIGlobalCanvas globalCanvas;
		private IPlayer player;
		private CharacterManager characterManager;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas, IPlayer player, CharacterManager characterManager)
		{
			this.globalCanvas = globalCanvas;
			this.player = player;
			this.characterManager = characterManager;
		}

		private void Start()
		{
			IsInteractable = true;
			Outline.enabled = false;
		}

		#region IObservable
		public void StartObserve()
		{
			if (IsInteractable)
			{
				Outline.enabled = true;
			}
		}

		public void Observe() { }

		public void EndObserve()
		{
			if (IsInteractable)
			{
				Outline.enabled = false;
			}
		}
		#endregion

		public bool InteractWith(IInteractable interactable)
		{
			return false;//не может не с кем взаимодествовать
		}

		public void Action()
		{
			IsInteractable = false;

			var playerRTS = player.RTSModel.transform;
			FastStorage.LastTransformOnMap = new DefaultTransform()
			{
				position = playerRTS.position,
				rotation = playerRTS.rotation,
				scale = playerRTS.localScale,
			};

			globalCanvas.WindowsManager.GetAs<WindowInfinityLoading>().Show(sceneName.GetScene(), In, Out);
		}
	}
}