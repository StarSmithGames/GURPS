using Game.Managers.StorageManager;

using UnityEngine;
using DG.Tweening;
using Zenject;

namespace Game.UI
{
	public class UIGlobalCanvas : MonoBehaviour
	{
		public WindowsManager WindowsManager { get; private set; }

		[field: SerializeField] public CanvasGroup GaveSaved { get; private set; }

		private SignalBus signalBus;

		[Inject]
		private void Construct(SignalBus signalBus, WindowsManager windowsManager)
		{
			this.signalBus = signalBus;
			WindowsManager = windowsManager;
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