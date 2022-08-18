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

		[field: SerializeField] public CanvasGroup GaveSaved { get; private set; }

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus)
		{
			this.signalBus = signalBus;
		}

		private void Start()
		{
			GaveSaved.alpha = 0f;

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
				.Append(GaveSaved.DOFade(1f, 0.2f))
				.AppendInterval(1f)
				.Append(GaveSaved.DOFade(0f, 0.15f));
			
		}
	}
}