using UnityEngine;
using Game.UI;
using UnityEngine.UI;
using Zenject;
using UnityEngine.EventSystems;
using Game.Systems.TooltipSystem;

namespace Game.HUD
{
	public class UISkill : PoolableObject
	{
		[field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
		[field: SerializeField] public Image Icon { get; private set; }

		private bool isInitialized = false;

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
				if(PointerHandler != null)
				{
					PointerHandler.onPointerEnter -= OnPointerEnter;
					PointerHandler.onPointerExit -= OnPointerExit;
				}
			}
		}

		public void SetSkill()
		{

		}

		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				PointerHandler.onPointerEnter += OnPointerEnter;
				PointerHandler.onPointerExit += OnPointerExit;
			}

			isInitialized = true;

			base.OnSpawned(pool);
		}

		private void OnPointerEnter(PointerEventData data)
		{

		}

		private void OnPointerExit(PointerEventData data)
		{

		}

		public class Factory : PlaceholderFactory<UISkill> { } 
	}
}