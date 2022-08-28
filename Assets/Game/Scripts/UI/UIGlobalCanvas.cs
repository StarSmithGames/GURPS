using Game.Managers.StorageManager;

using UnityEngine;
using DG.Tweening;
using Zenject;
using Game.UI.Windows;

namespace Game.UI
{
	public class UIGlobalCanvas : MonoBehaviour
	{
		public WindowsRegistrator WindowsManager
		{
			get
			{
				if (windowsManager == null)
				{
					windowsManager = new WindowsRegistrator();
				}

				return windowsManager;
			}
		}
		private WindowsRegistrator windowsManager;

		public Transform Windows
		{
			get
			{
				if (windows == null)
				{
					windows = transform.Find("Windows");
				}

				return windows;
			}
		}
		private Transform windows;

		public Transform Transitions
		{
			get
			{
				if(transitions == null)
				{
					transitions = transform.Find("Transitions");
				}

				return transitions;
			}
		}
		private Transform transitions;

		[field: SerializeField] public CanvasGroup GameSaved { get; private set; }

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		private void Start()
		{
			GameSaved.alpha = 0f;

			signalBus?.Subscribe<SignalSaveStorage>(OnSaveStorage);
		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalSaveStorage>(OnSaveStorage);
		}

		private void OnSaveStorage(SignalSaveStorage signal)
		{
			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(GameSaved.DOFade(1f, 0.2f))
				.AppendInterval(1f)
				.Append(GameSaved.DOFade(0f, 0.15f));
			
		}
	}
}