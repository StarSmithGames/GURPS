using EPOOutline;

using Game.Entities;
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

		[SerializeField] private bool useCustom = false;
		[field: HideIf("useCustom")]
		[field: SerializeField] public Scenes GoTo { get; private set; }
		[field: ShowIf("useCustom")]
		[field: SerializeField] public string SceneName { get; private set; }
		[field: SerializeField] public Transitions In { get; private set; }
		[field: SerializeField] public Transitions Out { get; private set; }
		[field: Space]
		[field: SerializeField] public Outlinable Outline { get; private set; }
		[field: SerializeField] public InteractionPoint InteractionPoint { get; private set; }

		private UIGlobalCanvas globalCanvas;
		private PlayerRTS player;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas, PlayerRTS player)
		{
			this.globalCanvas = globalCanvas;
			this.player = player;
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

			if (useCustom)
			{
				FastStorage.LastTransformOnMap = new DefaultTransform()
				{
					position = player.transform.position,
					rotation = player.transform.rotation,
					scale = player.transform.localScale,
				};
				globalCanvas.WindowsManager.GetAs<WindowInfinityLoading>().Show(SceneName, In, Out);
			}
			else
			{
				globalCanvas.WindowsManager.GetAs<WindowInfinityLoading>().Show(GoTo, In, Out);
			}
		}
	}
}