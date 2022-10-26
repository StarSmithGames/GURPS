using DG.Tweening;

using Game.Systems.SheetSystem;
using Game.Systems.TooltipSystem;
using Game.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.Managers.PartyManager
{
	public class UIEffect : PoolableObject
	{
		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
		[field: SerializeField] public Image Filler { get; private set; }
		[field: SerializeField] public Image Icon { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		public IEffect CurrentEffect { get; private set; }

		private bool isInitialized = false;
		private Sequence blinkSequence;
		private bool isCanBlink = false;

		private UITooltip tooltip;

		[Inject]
		private void Construct(UITooltip tooltip)
		{
			this.tooltip = tooltip;
		}

		private void OnDestroy()
		{
			if (isInitialized)
			{
				if (PointerHandler != null)
				{
					PointerHandler.onPointerEnter -= OnPointerEnter;
					PointerHandler.onPointerExit -= OnPointerExit;
					PointerHandler.onPointerClick -= OnPointerClick;
				}
			}
		}

		public void SetEffect(IEffect effect)
		{
			if(this.CurrentEffect != null)
			{
				this.CurrentEffect.onProgress -= UpdateFillAmount;
			}

			this.CurrentEffect = effect;

			isCanBlink = (effect as ProcessEffect)?.IsBlinked ?? (effect as InflictEffect)?.IsBlinked ?? false;

			UpdateUI();

			if(effect != null)
			{
				effect.onProgress += UpdateFillAmount;
			}
		}

		private void UpdateUI()
		{
			Icon.sprite = CurrentEffect?.Data.information.portrait;
			Text.text = "";

			Filler.fillAmount = CurrentEffect?.Progress ?? 0f;
		}

		private void UpdateFillAmount(float value)
		{
			Filler.fillAmount = 1 - value;
		}



		public void Show(UnityAction callback = null)
		{
			CanvasGroup.alpha = 0f;
			CanvasGroup
				.DOFade(1, 0.25f)
				.OnComplete(() =>
				{
					callback?.Invoke();
				});
		}

		public void Hide(UnityAction callback = null)
		{
			KillBlink();

			Sequence sequence = DOTween.Sequence();

			if (isCanBlink)
			{
				blinkSequence = DOTween.Sequence();

				float t = 0;

				blinkSequence
					.AppendCallback(() =>
					{
						CanvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.PingPong(t * 1.5f, 1));

						t += Time.deltaTime;
					}).SetLoops(-1);

				sequence.AppendInterval(1.5f);
			}
			
			sequence
				.Append(CanvasGroup.DOFade(0, 0.2f))
				.OnComplete(() =>
				{
					callback?.Invoke();
					DespawnIt();
				});
		}

		private void KillBlink()
		{
			if (blinkSequence != null)
			{
				if (blinkSequence.IsPlaying())
				{
					blinkSequence.Kill(true);
					blinkSequence = null;
				}
			}
		}

		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				PointerHandler.onPointerEnter += OnPointerEnter;
				PointerHandler.onPointerExit += OnPointerExit;
				PointerHandler.onPointerClick += OnPointerClick;
			}

			base.OnSpawned(pool);

			Show(() => isInitialized = true);
		}

		public override void OnDespawned()
		{
			base.OnDespawned();

			KillBlink();

			if (CurrentEffect != null)
			{
				CurrentEffect.onProgress -= UpdateFillAmount;
			}
		}

		private void OnPointerEnter(PointerEventData data)
		{
			tooltip.SetTarget(CurrentEffect);
			tooltip.SetPosition(data);
			tooltip.Show();
		}

		private void OnPointerExit(PointerEventData data)
		{
			tooltip.Hide();
		}

		private void OnPointerClick(PointerEventData data)
		{

		}

		public class Factory : PlaceholderFactory<UIEffect> { }
	}
}