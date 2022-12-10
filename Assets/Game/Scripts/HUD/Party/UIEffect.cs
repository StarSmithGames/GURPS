using DG.Tweening;

using Game.Systems.SheetSystem.Effects;
using Game.Systems.TooltipSystem;
using Game.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.HUD
{
	public class UIEffect : PoolableObject
	{
		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public UIAnimationBlinkComponent Blink { get; private set; }
		[field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
		[field: SerializeField] public Image Filler { get; private set; }
		[field: SerializeField] public Image Icon { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		public IEffect CurrentEffect { get; private set; }

		private bool isInitialized = false;
		private bool isCanBlink = false;

		private UIObjectTooltip tooltip;

		[Inject]
		private void Construct(UIObjectTooltip tooltip)
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
			Blink.Kill();

			Sequence sequence = DOTween.Sequence();

			if (isCanBlink)
			{
				Blink.Do(1.5f);
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

			Blink.Kill();

			if (CurrentEffect != null)
			{
				CurrentEffect.onProgress -= UpdateFillAmount;
			}

			if (tooltip.IsShowing)
			{
				tooltip.Hide();
			}
		}

		private void OnPointerEnter(PointerEventData data)
		{
			tooltip.EnterTarget(CurrentEffect);
		}

		private void OnPointerExit(PointerEventData data)
		{
			tooltip.ExitTarget();
		}

		private void OnPointerClick(PointerEventData data)
		{

		}

		public class Factory : PlaceholderFactory<UIEffect> { }
	}
}