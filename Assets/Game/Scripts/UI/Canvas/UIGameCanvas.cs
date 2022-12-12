using DG.Tweening;
using Game.Managers.StorageManager;
using UnityEngine;
using Zenject;

namespace Game.UI.CanvasSystem
{
	public class UIGameCanvas : UISubCanvas
	{
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
			signalBus?.TryUnsubscribe<SignalSaveStorage>(OnSaveStorage);
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